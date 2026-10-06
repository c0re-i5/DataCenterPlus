using System;
using HarmonyLib;
using Il2Cpp;
using MelonLoader;
using UnityEngine;
using Object = UnityEngine.Object;

namespace DataCenterPlus
{
    // Marks a cloned SFP box (5-pack) so modules taken out of it get upgraded.
    public class DcpPackTag : MonoBehaviour
    {
        public DcpPackTag(IntPtr ptr) : base(ptr) { }
        internal int   moduleId;
        internal float speed;
        internal int   sfpType;
    }

    // Explicit click handler for our injected shop buttons. It simply forwards to the
    // vanilla ButtonBuyShopItem, so the game's own (current-version) buy/cart/spawn
    // logic runs — we only supply a custom itemID that resolves to our prefab.
    public class ShopButtonHandler : MonoBehaviour
    {
        public ShopButtonHandler(IntPtr ptr) : base(ptr) { }
        internal ComputerShop shop;
        internal int itemID;
        internal int price;
        internal PlayerManager.ObjectInHand itemType;
        internal string displayName;
        private float _lastFire;

        public void OnClick()
        {
            if (shop == null) return;
            float now = Time.unscaledTime;
            if (now - _lastFire < 0.3f) return;   // debounce double-fire (click + select)
            _lastFire = now;
            try { shop.ButtonBuyShopItem(itemID, price, itemType, displayName, false); }
            catch (Exception e) { MelonLogger.Warning($"DataCenterPlus: buy failed for id {itemID} ({e.GetType().Name})"); }
        }
    }

    // ---------------------------------------------------------------- REGISTRY HOOKS
    [HarmonyPatch(typeof(MainGameManager), nameof(MainGameManager.Awake))]
    internal static class PatchMGMAwake
    {
        private static void Postfix(MainGameManager __instance) => Core.SetupRegistry(__instance);
    }

    [HarmonyPatch(typeof(MainGameManager), nameof(MainGameManager.Start))]
    internal static class PatchMGMStart
    {
        private static void Postfix(MainGameManager __instance)
        {
            if (DeviceRegistry.Entries.Count == 0) Core.SetupRegistry(__instance);
        }
    }

    // ------------------------------------------------ PREFAB RESOLUTION (buy + save/load)
    // The vanilla buy flow and save/load both resolve a prefab by type via these
    // Get*Prefab methods; we return our custom template for custom type IDs.
    [HarmonyPatch(typeof(MainGameManager), nameof(MainGameManager.GetServerPrefab))]
    internal static class PatchGetServerPrefab
    {
        private static void Postfix(MainGameManager __instance, int serverType, ref GameObject __result)
        {
            if (DeviceRegistry.IsCustom(serverType)
                && __instance.serverPrefabs != null && serverType < __instance.serverPrefabs.Length)
                __result = __instance.serverPrefabs[serverType];
        }
    }

    [HarmonyPatch(typeof(MainGameManager), nameof(MainGameManager.GetSwitchPrefab))]
    internal static class PatchGetSwitchPrefab
    {
        private static void Postfix(MainGameManager __instance, int switchType, ref GameObject __result)
        {
            if (DeviceRegistry.IsCustom(switchType)
                && __instance.switchesPrefabs != null && switchType < __instance.switchesPrefabs.Length)
                __result = __instance.switchesPrefabs[switchType];
        }
    }

    [HarmonyPatch(typeof(MainGameManager), nameof(MainGameManager.GetRouterPrefab))]
    internal static class PatchGetRouterPrefab
    {
        private static void Postfix(MainGameManager __instance, int routerType, ref GameObject __result)
        {
            if (DeviceRegistry.IsCustom(routerType)
                && __instance.routersPrefabs != null && routerType < __instance.routersPrefabs.Length)
                __result = __instance.routersPrefabs[routerType];
        }
    }

    [HarmonyPatch(typeof(MainGameManager), nameof(MainGameManager.GetFirewallPrefab))]
    internal static class PatchGetFirewallPrefab
    {
        private static void Postfix(MainGameManager __instance, int firewallType, ref GameObject __result)
        {
            if (DeviceRegistry.IsCustom(firewallType)
                && __instance.firewallsPrefabs != null && firewallType < __instance.firewallsPrefabs.Length)
                __result = __instance.firewallsPrefabs[firewallType];
        }
    }

    [HarmonyPatch(typeof(MainGameManager), nameof(MainGameManager.GetSfpPrefab))]
    internal static class PatchGetSfpPrefab
    {
        private static void Postfix(MainGameManager __instance, int prefabID, ref GameObject __result)
        {
            if (DeviceRegistry.IsCustom(prefabID)
                && __instance.sfpPrefabs != null && prefabID < __instance.sfpPrefabs.Length)
                __result = __instance.sfpPrefabs[prefabID];
        }
    }

    [HarmonyPatch(typeof(MainGameManager), nameof(MainGameManager.GetSfpBoxPrefab))]
    internal static class PatchGetSfpBoxPrefab
    {
        private static void Postfix(MainGameManager __instance, int prefabID, ref GameObject __result)
        {
            if (DeviceRegistry.IsCustom(prefabID)
                && __instance.sfpsBoxedPrefab != null && prefabID < __instance.sfpsBoxedPrefab.Length)
                __result = __instance.sfpsBoxedPrefab[prefabID];
        }
    }

    [HarmonyPatch(typeof(MainGameManager), nameof(MainGameManager.GetCableSpinnerPrefab))]
    internal static class PatchGetCableSpinnerPrefab
    {
        private static void Postfix(MainGameManager __instance, int prefabID, ref GameObject __result)
        {
            if (DeviceRegistry.IsCustom(prefabID)
                && __instance.cableSpinnerPrefab != null && prefabID < __instance.cableSpinnerPrefab.Length)
                __result = __instance.cableSpinnerPrefab[prefabID];
        }
    }

    [HarmonyPatch(typeof(MainGameManager), nameof(MainGameManager.ReturnServerNameFromType))]
    internal static class PatchReturnServerName
    {
        private static void Postfix(int type, ref string __result)
        {
            if (DeviceRegistry.TryGet(type, out var e) && e.Kind == DeviceKind.Server)
                __result = e.DisplayName;
        }
    }

    [HarmonyPatch(typeof(MainGameManager), nameof(MainGameManager.ReturnSwitchNameFromType))]
    internal static class PatchReturnSwitchName
    {
        private static void Postfix(int type, ref string __result)
        {
            if (DeviceRegistry.TryGet(type, out var e) &&
                (e.Kind == DeviceKind.Switch || e.Kind == DeviceKind.Router || e.Kind == DeviceKind.Firewall))
                __result = e.DisplayName;
        }
    }

    // --------------------------------------------------------------- DEVICE BEHAVIOUR
    [HarmonyPatch(typeof(NetworkSwitch), nameof(NetworkSwitch.ButtonShowNetworkSwitchConfig))]
    internal static class PatchConfigButton
    {
        private static bool Prefix(NetworkSwitch __instance)
        {
            var mgm = MainGameManager.instance;
            if (mgm == null) return true;
            var usable = __instance.GetComponent<UsableObject>();
            if (usable == null) return true;
            if (!DeviceRegistry.TryGet(usable.prefabID, out var entry)) return true;

            if (entry.Kind == DeviceKind.Router)
            {
                var router = __instance.GetComponent<Router>();
                if (router != null) { try { mgm.ShowRouterConfigCanvas(router); } catch { } return false; }
            }
            else if (entry.Kind == DeviceKind.Firewall)
            {
                var firewall = __instance.GetComponent<Firewall>();
                if (firewall != null) { try { mgm.ShowFirewallConfigCanvas(firewall); } catch { } return false; }
            }
            return true;
        }
    }

    [HarmonyPatch(typeof(NetworkSwitch), nameof(NetworkSwitch.SwitchInsertedInRack))]
    internal static class PatchSwitchInsertedInRack
    {
        private static void Postfix(NetworkSwitch __instance, SwitchSaveData switchSaveData)
        {
            var usable = __instance.GetComponent<UsableObject>();
            if (usable == null) return;
            if (!DeviceRegistry.TryGet(usable.prefabID, out var entry)) return;
            Core.TrySet(() => __instance.switchId = entry.DisplayName, "switch label");
            if (__instance.txtScreen != null)
                Core.TrySet(() => __instance.txtScreen.text = entry.DisplayName, "switch screen text");
            // Re-assert port types after the game's Awake/insert logic has run.
            Core.ConfigurePorts(__instance.gameObject, entry.PortSfpType, 0f, $"net {entry.CustomId} (rack)");
        }
    }

    // Force IOPS for our high-tier servers after the game initialises them.
    [HarmonyPatch(typeof(Server), nameof(Server.Awake))]
    internal static class PatchServerAwake
    {
        private static void Postfix(Server __instance) => BoostServer.Apply(__instance);
    }

    [HarmonyPatch(typeof(Server), nameof(Server.ServerInsertedInRack))]
    internal static class PatchServerInserted
    {
        private static void Postfix(Server __instance, ServerSaveData serverSaveData)
        {
            BoostServer.Apply(__instance);
            int id = __instance.serverType;
            if (DeviceRegistry.TryGet(id, out var e) && e.Kind == DeviceKind.Server)
            {
                float portSpeed = TierConfig.ApplyServerPortSpeed ? TierConfig.ServerPortSpeedGbps : 0f;
                Core.ConfigurePorts(__instance.gameObject, TierConfig.ServerPortSfpType, portSpeed, $"server {id} (rack)");
            }
        }
    }

    internal static class BoostServer
    {
        internal static void Apply(Server srv)
        {
            if (srv == null) return;
            int id = srv.serverType;
            if (!DeviceRegistry.TryGet(id, out var e) || e.Kind != DeviceKind.Server) return;
            if (e.TargetIops > 0f && !Mathf.Approximately(srv.maxProcessingSpeed, e.TargetIops))
                Core.TrySet(() => srv.maxProcessingSpeed = e.TargetIops, "boost server IOPS");
        }
    }

    // Apply the transceiver's bandwidth to the cable when a custom SFP is inserted.
    [HarmonyPatch(typeof(CableLink), nameof(CableLink.InsertSFP))]
    internal static class PatchInsertSFP
    {
        private static void Postfix(CableLink __instance, float speed, int type, SFPModule module)
        {
            if (module == null) return;
            var usable = module.GetComponent<UsableObject>();
            if (usable == null) return;
            if (!DeviceRegistry.TryGet(usable.prefabID, out var e) || e.Kind != DeviceKind.Sfp) return;
            if (e.SpeedGbps > 0f) Core.TrySet(() => __instance.SetConnectionSpeed(e.SpeedGbps), "set link speed");
        }
    }

    // Upgrade each module as it's taken out of one of our custom 5-packs.
    [HarmonyPatch(typeof(SFPBox), nameof(SFPBox.TakeSFPFromBox))]
    internal static class PatchTakeSFPFromBox
    {
        private static void Postfix(SFPBox __instance, ref SFPModule __result)
        {
            if (__result == null) return;
            var tag = __instance.GetComponent<DcpPackTag>();
            if (tag == null) return;
            var module = __result;
            Core.TrySet(() => { module.speed = tag.speed; module.sfpType = tag.sfpType; }, "upgrade pack module");
            var usable = module.GetComponent<UsableObject>();
            if (usable != null) Core.TrySet(() => usable.prefabID = tag.moduleId, "pack module prefabID");
        }
    }

    // Let our custom packs accept their own (custom-typed) modules back into the box.
    [HarmonyPatch(typeof(SFPBox), nameof(SFPBox.CanAcceptSFP))]
    internal static class PatchCanAcceptSFP
    {
        private static void Postfix(SFPBox __instance, int sfpType, ref bool __result)
        {
            if (__result) return;
            var tag = __instance.GetComponent<DcpPackTag>();
            if (tag != null && sfpType == tag.sfpType) __result = true;
        }
    }
}
