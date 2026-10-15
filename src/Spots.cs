using H;
using H.DataClass.AnimationInfo;
using Il2CppInterop.Runtime.InteropTypes.Arrays;

namespace AC_GroupHAnywhere;

internal static class Spots
{
    // Positions given an extra place in this scene, with the places they named before. They are shared game data,
    // so they are put back when the scene ends and a map with the proper spots never sees the extra place.
    private static readonly Dictionary<IntPtr, (AnimationListInfo info, int[] was)> _changed = new();
    // Positions no spot on this map can host, so they aren't checked again.
    private static readonly HashSet<IntPtr> _hopeless = new();
    private static IntPtr _loggedList;

    // 3P (one man, two girls) and 5P (one man, four girls) positions.
    public static bool IsGroup(AnimationListInfo info) =>
        info.Mode == (int)Define.PostureCategory.M1F2 || info.Mode == (int)Define.PostureCategory.M1F4;

    // Places tried in order: a 5P can use a 3P spot; then the floor and standing spots; then any other kind of
    // spot the map has.
    private static List<int> Fallbacks(HPointList list, int[] was)
    {
        int h3p = Define.Place.H3P, h5p = Define.Place.H5P;
        var order = new List<int>();
        if (was.Contains(h5p)) order.Add(h3p);
        order.Add(Define.Place.FLOOR);
        order.Add(Define.Place.STAND);
        var groups = list._hPointGroups;
        for (var i = 0; groups != null && i < groups.Length; i++)
            if (groups[i] != null) order.Add(groups[i].PlaceID);
        return order.Where(p => !was.Contains(p) && p != h5p && (p != h3p || was.Contains(h5p))).Distinct().ToList();
    }

    // If no spot on the map can host the position under its own places, add the first fallback place that can.
    public static void Prepare(HPointList list, AnimationListInfo info)
    {
        LogMap(list);
        if (_changed.ContainsKey(info.Pointer) || _hopeless.Contains(info.Pointer)) return;
        var was = info.Positions?.ToArray() ?? Array.Empty<int>();
        if (was.Any(p => Hosts(list, p, info))) return;
        var add = Fallbacks(list, was);
        foreach (var place in add)
        {
            if (!Hosts(list, place, info)) continue;
            var arr = new Il2CppStructArray<int>(was.Length + 1);
            for (var i = 0; i < was.Length; i++) arr[i] = was[i];
            arr[was.Length] = place;
            _changed[info.Pointer] = (info, was);
            info.Positions = arr;
            Plugin.Debug($"{Name(info)}: places [{string.Join(",", was)}] -> [{string.Join(",", arr.ToArray())}].");
            return;
        }
        _hopeless.Add(info.Pointer);
        Plugin.Debug($"{Name(info)}: places [{string.Join(",", was)}], no spot here for it or for [{string.Join(",", add)}].");
    }

    // Whether the map has an active spot of this place the position isn't limited at, as the game checks it.
    private static bool Hosts(HPointList list, int place, AnimationListInfo info)
    {
        var groups = list._hPointGroups;
        for (var i = 0; groups != null && i < groups.Length; i++)
        {
            var g = groups[i];
            if (g != null && g.PlaceID == place && g.FindIndex(out _, info.Mode, info.ID) != -1) return true;
        }
        return false;
    }

    private static void LogMap(HPointList list)
    {
        if (list.Pointer == _loggedList || !Plugin.DetailedLog.Value) return;
        _loggedList = list.Pointer;
        var parts = new List<string>();
        var groups = list._hPointGroups;
        for (var i = 0; groups != null && i < groups.Length; i++)
        {
            var g = groups[i];
            if (g == null) continue;
            var n = g.Groups?.Length ?? 0;
            var on = 0;
            for (var j = 0; j < n; j++)
                try { if (g.Groups[j] != null && g.Groups[j].gameObject.activeSelf) on++; } catch { }
            parts.Add($"{g.PlaceID}: {on}/{n}");
        }
        Plugin.Log.LogInfo($"H spots on this map (place: active/all): {string.Join(", ", parts)}. " +
                           $"Floor {Define.Place.FLOOR}, stand {Define.Place.STAND}, 3P {Define.Place.H3P}, 5P {Define.Place.H5P}.");
    }

    public static void RestoreAll()
    {
        foreach (var (info, was) in _changed.Values)
        {
            try
            {
                var arr = new Il2CppStructArray<int>(was.Length);
                for (var i = 0; i < was.Length; i++) arr[i] = was[i];
                info.Positions = arr;
            }
            catch { }
        }
        if (_changed.Count > 0) Plugin.Debug($"{_changed.Count} positions back to their own places.");
        _changed.Clear();
        _hopeless.Clear();
        _loggedList = IntPtr.Zero;
    }

    private static string Name(AnimationListInfo info)
    {
        try { return $"{(Define.PostureCategory)info.Mode} {info.ID} {info.NameAnimation}"; } catch { return "position"; }
    }
}
