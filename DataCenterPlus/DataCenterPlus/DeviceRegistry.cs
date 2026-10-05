using System.Collections.Generic;
using UnityEngine;

namespace DataCenterPlus
{
    internal enum DeviceKind { Switch, Router, Firewall, Server, Sfp, SfpBox }

    internal static class DeviceRegistry
    {
        // Custom type-ID ranges. Kept high and distinct so they don't collide with
        // the base game or the NetworkingPlus mod (which uses 100/200).
        internal const int SWITCH_ID_BASE   = 500;
        internal const int ROUTER_ID_BASE   = 600;
        internal const int FIREWALL_ID_BASE = 700;
        internal const int SERVER_ID_BASE   = 1000;
        internal const int SFP_ID_BASE      = 2000;
        internal const int SFPBOX_ID_BASE   = 2100;

        internal sealed class Entry
        {
            internal int        CustomId;      // the custom type/prefab ID
            internal int        BaseType;      // index of the base prefab we clone
            internal DeviceKind Kind;
            internal string     DisplayName;
            internal int        Price;
            internal Color      Color;         // body tint (alpha 0 = no tint)
            internal Color      IconColor;     // shop-icon accent (alpha 0 = no recolor)
            internal float      SpeedGbps;     // for SFP / net labels
            internal float      TargetIops;    // for servers (0 = n/a)
            internal int        ModuleId;      // for SFP boxes: the SFP module type they dispense
            internal int        ShopItemType;  // ObjectInHand value to use for the shop item
        }

        private static readonly Dictionary<int, Entry> _entries = new();

        internal static IReadOnlyDictionary<int, Entry> Entries => _entries;

        internal static void Register(Entry entry) => _entries[entry.CustomId] = entry;

        internal static bool TryGet(int id, out Entry entry) => _entries.TryGetValue(id, out entry);

        internal static bool IsCustom(int id) => _entries.ContainsKey(id);

        internal static void Clear() => _entries.Clear();
    }
}
