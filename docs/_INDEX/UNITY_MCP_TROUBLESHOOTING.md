# Unity MCP Troubleshooting

Last updated: 2026-06-01

## Final Recommendation

Stop debugging `CoderGamester/mcp-unity` for this project. It is installed and built, but remains unstable even after a clean process reset and a Node 22 LTS retest.

Use `CoplayDev/unity-mcp` as the fallback MCP bridge.

Current Codex MCP config:

```toml
[mcp_servers.coplay_unity_mcp]
command = "C:/Users/Adrian Iliev/.local/bin/uvx.exe"
args = ["--from", "mcpforunityserver", "mcp-for-unity", "--transport", "stdio"]
env = { "UV_NO_CACHE" = "1", "UV_TOOL_DIR" = "C:/tmp/uv-cache-coplay", "UNITY_MCP_DISABLE_TELEMETRY" = "true", "DISABLE_TELEMETRY" = "true", "MCP_TOOL_TIMEOUT" = "120000" }
```

Restart Codex after Unity is open so the new MCP config is loaded.

## CoderGamester Findings

This was not a missing install problem.

`com.gamelovers.mcp-unity` was installed and built:

```txt
Package: com.gamelovers.mcp-unity
Version: 1.3.0
Package cache: Library/PackageCache/com.gamelovers.mcp-unity@aade29c7dd84
Unity WebSocket port: 8090
```

Observed failure pattern:

- Unity accepts the WebSocket connection.
- Unity receives MCP requests.
- Unity processes requests late on the Editor main thread.
- The MCP client times out or closes first.
- Unity then fails when sending the response.

Unity error:

```txt
WebSocket.Send: This operation isn't available in: closed
[MCP Unity] WebSocket error: An error has occurred in sending data.
McpUnity.Unity.McpUnitySocketHandler/<HandleMessageAsync>d__4:MoveNext ()
  at .../Editor/UnityBridge/McpUnitySocketHandler.cs:123
```

Codex/client-side errors seen:

```txt
Request timed out
timed out awaiting tools/call after 120s
Transport closed
```

## Clean Reset Performed

Commands/actions performed:

```powershell
Get-Process Unity -ErrorAction SilentlyContinue | Stop-Process -Force
Get-Process node -ErrorAction SilentlyContinue | Stop-Process -Force
```

Result:

```txt
NO_UNITY_PROCESSES
NO_NODE_PROCESSES
```

Node 22 LTS was downloaded from official Node.js release files and unpacked to:

```txt
C:\tmp\node-v22-lts
```

Version check:

```txt
node --version -> v22.22.3
npm.cmd --version -> 10.9.8
```

The Node zip checksum was verified against official `SHASUMS256.txt`.

CoderGamester bridge dependency reinstall/rebuild under Node 22:

```powershell
$env:PATH = "C:\tmp\node-v22-lts;$env:PATH"
cd "C:\Users\Adrian Iliev\My project\Library\PackageCache\com.gamelovers.mcp-unity@aade29c7dd84\Server~"
node --version
npm.cmd --version
npm.cmd install --loglevel=warn
npm.cmd run build
```

Build result:

```txt
v22.22.3
10.9.8
up to date
tsc succeeded
```

Unity was relaunched with the project path quoted:

```powershell
Start-Process -FilePath "C:\Program Files\Unity\Hub\Editor\6000.4.9f1\Editor\Unity.exe" -ArgumentList '-projectPath "C:\Users\Adrian Iliev\My project"'
```

Port state after restart:

```txt
TCP 0.0.0.0:8090 0.0.0.0:0 LISTENING 31224
```

## Tiny Calls Tested Under Node 22

No `get_scene_info` call was made during the Node 22 retest.

Fresh MCP client test results:

| Check | Result |
| --- | --- |
| MCP server connect | Passed in ~161 ms |
| List MCP resources | Passed in ~14 ms |
| Read `unity://packages` | Failed after ~60 s with `Request timed out` |
| Read `unity://assets` | Failed after ~120 s with `Request timed out` |

Server log excerpt:

```txt
[Unity] Using request timeout: 1000 seconds
[Unity] WebSocket connected to Unity
[Unity] Command ... queued at position 1
[Unity] Replaying 1 queued commands
[Unity] Request ... timed out after 60000ms
[Resources] Error handling resource get_packages: McpUnityError: Request timed out
```

This proves Node 22 alone does not fix the Unity-backed request path.

## Coplay Fallback Installation

Added to `Packages/manifest.json`:

```json
"com.coplaydev.unity-mcp": "https://github.com/CoplayDev/unity-mcp.git?path=/MCPForUnity#main"
```

Unity resolved the package:

```txt
Package: com.coplaydev.unity-mcp
Version: 9.7.1
Package cache: Library/PackageCache/com.coplaydev.unity-mcp@efaf786e8772
Fingerprint: efaf786e8772a8591940fdb341524588470469ed
```

Unity compiled/imported Coplay:

```txt
Registered 65 packages
com.coplaydev.unity-mcp@https://github.com/CoplayDev/unity-mcp.git?path=/MCPForUnity#main
MCPForUnity.Runtime.dll copied
MCPForUnity.Editor.dll copied
```

Coplay Python server command was validated:

```powershell
uvx --from mcpforunityserver mcp-for-unity --help
```

Result:

```txt
usage: mcp-for-unity ...
MCP for Unity Server
```

`uvx` initially hit Windows cache/tool-state problems:

```txt
Failed to initialize cache at ...\uv\cache
Cannot create a file when that file already exists.
```

Then `C:\tmp\uv-cache*` reproduced a second uv cache issue:

```txt
failed to open file `...\sdists-v9\.git`: Access is denied.
```

The active config uses:

```txt
UV_NO_CACHE=1
UV_TOOL_DIR=C:/tmp/uv-cache-coplay
```

This avoids the broken uv cache path and the AppData Roaming uv tool lock. Startup is slower but more reliable.

## Important Notes

- No gameplay code was modified.
- No scenes were created.
- No assets were imported.
- `get_scene_info` was not called during the final Node 22 retest.
- `OutdoorsScene` was not used as a test target after the tiny-call investigation started.
- CoderGamester may still listen on `8090` while its Unity package remains installed; Codex is now configured to use Coplay instead.

## Next Verification Steps

After restarting Codex so it loads `.codex/config.toml`, test Coplay only with tiny calls:

1. Editor status.
2. Project info.
3. Root/list assets.

Do not call `get_scene_info` until these pass.
