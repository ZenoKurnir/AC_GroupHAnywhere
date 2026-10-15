using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;

namespace AC_GroupHAnywhere;

// 3P and 5P on every map. The game lists a position only when the map has an H spot of a place the position
// names (AnimationListInfo.Positions). 3P positions only name the 3P spot and 5P positions the 5P spot, and small
// maps like the cafe and the library have neither, so after the 3P invite the scene has no 3P position to pick.
// When no spot on the map can host a 3P or 5P position, this lends it one of the map's other spots.
[BepInProcess("Aicomi")]
[BepInPlugin(GUID, "AC_GroupHAnywhere", Version)]
public class Plugin : BasePlugin
{
    public const string GUID = "AC_GroupHAnywhere";
    public const string Version = "1.0.0";

    internal static new ManualLogSource Log;
    internal static ConfigEntry<bool> Enabled;
    internal static ConfigEntry<bool> DetailedLog;

    internal static void Debug(string message)
    {
        if (DetailedLog.Value) Log.LogInfo(message);
    }

    public override void Load()
    {
        Log = base.Log;
        Enabled = Config.Bind("General", "Enabled", true,
            "When a map has no spot for 3P or 5P positions, they use one of the map's other spots.");
        DetailedLog = Config.Bind("General", "Detailed Log", false,
            "Write the map's H spots and every position that gets a spot to the BepInEx log, for bug reports.");
        Harmony.CreateAndPatchAll(typeof(Hooks), GUID);
    }
}
