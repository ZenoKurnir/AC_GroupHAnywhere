# AC_GroupHAnywhere

[日本語版はこちら (Japanese)](README.ja.md)

3P and 5P on every map in **Aicomi**. This is a BepInEx 6 (IL2CPP) plugin.

## The problem

In the base game, small maps like the cafe and the library offer the 3P invite ("Feeling watched..") and the second girl joins, but the H scene then shows no 3P positions. The same happens with 5P.

The cause is that the game lists a position only when the map has an H spot of the kind that position asks for. 3P positions only accept the game's dedicated 3P spots, and 5P positions only accept the dedicated 5P spots. The cafe and the library have neither.

## What this plugin does

When a map has no spot that can take a 3P or 5P position, the position borrows one of the map's other spots, trying in this order:

1. For 5P, a 3P spot.
2. A floor spot.
3. A standing spot.
4. Any other spot the map has.

On maps that already have proper 3P and 5P spots, nothing changes. Everything is put back when the H scene ends.

## Requirements

- Aicomi
- [BepInEx 6 (IL2CPP, bleeding edge)](https://builds.bepinex.dev/projects/bepinex_be), as included in the usual modpacks

No other plugins are needed. It works on its own, and alongside other H plugins.

## Installation

1. Download `AC_GroupHAnywhere-v1.0.0.zip` from the [Releases](../../releases) page.
2. Extract it into the game folder (the one with `Aicomi.exe`), so you end up with `BepInEx/plugins/AC_GroupHAnywhere.dll`.
3. Start the game.

## Configuration

Edit `BepInEx/config/AC_GroupHAnywhere.cfg`, or use the ConfigurationManager plugin (F1).

| Setting | Default | What it does |
| --- | --- | --- |
| Enabled | On | Lets 3P and 5P positions use other spots on maps without 3P or 5P spots |
| Detailed Log | Off | Writes the map's H spots and each position that gets a spot to the BepInEx log, for bug reports |

## Notes

- The borrowed spot is sized for two or three people, so on a crowded spot a 5P can overlap nearby furniture.
- When reporting a bug, turn on **Detailed Log**, reproduce it, and attach `BepInEx/LogOutput.log`.

## Building

You need the .NET 6 SDK or newer, and a game folder where BepInEx has run once.

```
cd src
dotnet build -c Release -p:GameDir="C:\path\to\Aicomi\BepInEx"
```

If your install keeps the generated assemblies in `BepInEx\interop-Aicomi\interop` instead of `BepInEx\interop`, add `-p:InteropDir="C:\path\to\Aicomi\BepInEx\interop-Aicomi\interop"`.

The DLL is written to `src/bin/Release/net6.0/AC_GroupHAnywhere.dll`.

## License

MIT. See [LICENSE](LICENSE).
