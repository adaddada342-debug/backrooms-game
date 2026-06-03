# Unity MCP Setup Log

Last updated: 2026-06-01 19:45:00 +01:00

## Scope

This setup is strictly for Unity MCP integration. No gameplay systems, scenes, assets, multiplayer, VR, procedural generation, or Addressables work was performed.

## Environment

- Project path: `C:\Users\Adrian Iliev\My project`
- Operating system: `Microsoft Windows NT 10.0.26200.0`, 64-bit
- Unity version: `6000.4.9f1 (f7258d6eebbe)`
- Unity executable: `C:\Program Files\Unity\Hub\Editor\6000.4.9f1\Editor\Unity.exe`
- Node.js: `v24.15.0`
- npm: `11.12.1` via `npm.cmd`
- Git: `git version 2.52.0.windows.1`
- Codex CLI: present at `C:\Program Files\WindowsApps\OpenAI.Codex_26.527.3686.0_x64__2p2nqsd0c76g0\app\resources\codex.exe`, but direct `codex --version` from this PowerShell shell returned `Access is denied`.

## Primary MCP

- Repository: `CoderGamester/mcp-unity`
- Package dependency added to `Packages/manifest.json`:

```json
"com.gamelovers.mcp-unity": "https://github.com/CoderGamester/mcp-unity.git"
```

- Resolved package: `com.gamelovers.mcp-unity`
- Package version: `1.3.0`
- Resolved commit/hash: `aade29c7dd84b76daae83ac5ce7776d0647a3734`
- Package cache path: `Library/PackageCache/com.gamelovers.mcp-unity@aade29c7dd84`

## Server Build

The package installed with `Server~` source files but no compiled `build/index.js`, so the Node server was built manually.

Commands run from `Library/PackageCache/com.gamelovers.mcp-unity@aade29c7dd84/Server~`:

```powershell
npm.cmd install --loglevel=verbose
npm.cmd run build
```

Results:

- `npm.cmd install --loglevel=verbose`: exit code `0`
- `npm.cmd run build`: exit code `0`
- Built server: `Library/PackageCache/com.gamelovers.mcp-unity@aade29c7dd84/Server~/build/index.js`
- npm reported `13 vulnerabilities (6 moderate, 5 high, 2 critical)` in the MCP server dependency tree. No `npm audit fix` was run because this is third-party package code.

## Codex MCP Configuration

Project-local Codex config created:

```txt
.codex/config.toml
```

Contents:

```toml
[mcp_servers.mcp-unity]
command = "node"
args = ["Library/PackageCache/com.gamelovers.mcp-unity@aade29c7dd84/Server~/build/index.js"]
```

The global Codex config already marks this project trusted:

```toml
[projects.'c:\users\adrian iliev\my project']
trust_level = "trusted"
```

## Verification Performed

MCP stdio server verification:

```powershell
node -e "..."
```

Result:

- MCP server started.
- MCP client initialized.
- `tools/list` returned `30` tools, including `get_scene_info`, `execute_menu_item`, `update_gameobject`, `create_scene`, `get_console_logs`, and `batch_execute`.

Unity editor bridge verification:

```txt
callTool get_scene_info {}
```

Result:

```txt
Request timed out
```

Port check:

```powershell
Test-NetConnection -ComputerName 127.0.0.1 -Port 8090
```

Result:

```txt
TCP connect to 127.0.0.1:8090 failed
```

Conclusion: the Node MCP stdio server is installed and working, but the Unity Editor-side WebSocket bridge has not been started from the Unity UI yet.

## Follow-Up Live MCP Verification

After Codex loaded the `mcp-unity` tools, a live `get_scene_info` call succeeded once:

```txt
Active Scene: OutdoorsScene
Path: Assets/OutdoorsScene.unity
Build Index: 0
Is Dirty: false
Is Loaded: true
Root Count: 4
```

Subsequent MCP calls timed out, including `send_console_log`, `get_console_logs`, and another `get_scene_info`.

Unity `Editor.log` showed the WebSocket bridge did receive successful `get_scene_info` requests, but also logged closed connection errors:

```txt
WebSocket.Send: This operation isn't available in: closed
WebSocket error: An error has occurred in sending data.
```

Updated conclusion: package install and MCP tool discovery are good, but the live Unity bridge is not stable enough yet to call the whole setup fully verified.

## Files Created

- `.codex/config.toml`
- `docs/_INDEX/UNITY_MCP_SETUP_LOG.md`
- `docs/_INDEX/UNITY_MCP_COMMANDS.md`
- `docs/_INDEX/UNITY_MCP_TROUBLESHOOTING.md`

## Files Modified

- `Packages/manifest.json`
- `Packages/packages-lock.json`

## Fallback MCP

Fallback `CoplayDev/unity-mcp` was not installed. The primary package installed successfully; only live editor bridge startup remains.
