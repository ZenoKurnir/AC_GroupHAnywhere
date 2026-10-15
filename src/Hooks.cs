using H;
using H.DataClass.AnimationInfo;
using HarmonyLib;

namespace AC_GroupHAnywhere;

// Only void and plain methods are hooked; never one that returns a UniTask.
// PostureMainCategory.IsCheck and CheckMotion decide whether a position is listed. Each has its own inlined copy
// of IsPoint, so IsPoint itself can't be patched. Both are hooked before they run: a 3P or 5P position the map
// has no spot for gets a place the map does have, and the game's own check then passes. HPointCtrl.SetPosition
// reads the same places when it moves the scene to a position's spot.
internal static class Hooks
{
    [HarmonyPrefix]
    [HarmonyPatch(typeof(PostureMainCategory), nameof(PostureMainCategory.IsCheck))]
    public static void Pre_IsCheck(PostureMainCategory __instance, AnimationListInfo info) => Prepare(__instance, info);

    [HarmonyPrefix]
    [HarmonyPatch(typeof(PostureMainCategory), nameof(PostureMainCategory.CheckMotion))]
    public static void Pre_CheckMotion(PostureMainCategory __instance, AnimationListInfo info) => Prepare(__instance, info);

    [HarmonyPrefix]
    [HarmonyPatch(typeof(HPointCtrl), nameof(HPointCtrl.SetPosition))]
    public static void Pre_SetPosition(HPointCtrl __instance, AnimationListInfo info)
    {
        if (!Plugin.Enabled.Value || info == null || !Spots.IsGroup(info)) return;
        try { Spots.Prepare(__instance._hPointList, info); }
        catch (Exception e) { Plugin.Log.LogWarning($"Placing a position failed: {e.Message}"); }
    }

    private static void Prepare(PostureMainCategory pmc, AnimationListInfo info)
    {
        if (!Plugin.Enabled.Value || info == null || pmc == null || !Spots.IsGroup(info)) return;
        try
        {
            var list = pmc._hPointCtrl?._hPointList ?? pmc._hScene?.HPointCtrl?._hPointList;
            if (list != null) Spots.Prepare(list, info);
        }
        catch (Exception e) { Plugin.Log.LogWarning($"Checking a position's spots failed: {e.Message}"); }
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(HScene), nameof(HScene.Dispose))]
    public static void Pre_Dispose()
    {
        try { Spots.RestoreAll(); }
        catch (Exception e) { Plugin.Log.LogWarning($"Putting positions back failed: {e.Message}"); }
    }
}
