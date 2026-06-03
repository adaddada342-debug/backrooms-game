# Unity MCP Commands

## Start Unity MCP

1. Open the Unity project:

```txt
C:\Users\Adrian Iliev\My project
```

2. In Unity, open:

```txt
Tools > MCP Unity > Server Window
```

3. Click:

```txt
Start Server
```

4. Leave Unity open while using Codex MCP tools.

The default Unity WebSocket port is `8090`.

## Codex Project Config

Codex project config path:

```txt
C:\Users\Adrian Iliev\My project\.codex\config.toml
```

Configured MCP server:

```toml
[mcp_servers.mcp-unity]
command = "node"
args = ["Library/PackageCache/com.gamelovers.mcp-unity@aade29c7dd84/Server~/build/index.js"]
```

## Manual Server Build

Use this only if `build/index.js` is missing after a package reinstall or update:

```powershell
cd "C:\Users\Adrian Iliev\My project\Library\PackageCache\com.gamelovers.mcp-unity@aade29c7dd84\Server~"
npm.cmd install
npm.cmd run build
```

## Verify Node MCP Server

From:

```txt
C:\Users\Adrian Iliev\My project\Library\PackageCache\com.gamelovers.mcp-unity@aade29c7dd84\Server~
```

Run a protocol-level tool listing with an MCP client. A successful check should list about 30 tools.

Known tools from this install include:

- `get_scene_info`
- `execute_menu_item`
- `select_gameobject`
- `get_gameobject`
- `update_gameobject`
- `update_component`
- `get_console_logs`
- `send_console_log`
- `create_scene`
- `load_scene`
- `save_scene`
- `run_tests`
- `batch_execute`

## Verify Unity Connection

After clicking `Start Server` in Unity:

1. Confirm port `8090` is open:

```powershell
Test-NetConnection -ComputerName 127.0.0.1 -Port 8090
```

2. Ask Codex MCP to list scene info:

```txt
Use mcp-unity get_scene_info
```

3. Harmless test asset workflow:

```txt
Create Assets/_MCP_Verification
Create a small verification text asset if supported
List it
Delete the test asset/folder
```

Do not create gameplay files during verification.

## Rollback

To remove MCP Unity:

1. Remove this dependency from `Packages/manifest.json`:

```json
"com.gamelovers.mcp-unity": "https://github.com/CoderGamester/mcp-unity.git"
```

2. Let Unity refresh packages.
3. Delete `.codex/config.toml` if it only contains the MCP Unity config.
4. Do not manually delete archive documentation files.
