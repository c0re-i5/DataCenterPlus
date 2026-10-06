using System;
using HarmonyLib;
using Il2Cpp;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using MelonLoader;
using UnityEngine;
using Object = UnityEngine.Object;

namespace DataCenterPlus
{
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

    // Resolve our synthetic name UIDs to the item's DisplayName so hover tooltips,
    // labels and any other localisation lookups show the custom name.
    [HarmonyPatch(typeof(Localisation), nameof(Localisation.ReturnTextByID))]
    internal static class PatchReturnTextByID
    {
        private static void Postfix(int _uid, ref string __result)
        {
            if (_uid < DeviceRegistry.NAME_UID_BASE) return;
            if (DeviceRegistry.TryGet(_uid - DeviceRegistry.NAME_UID_BASE, out var e))
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
            // IMPORTANT: do NOT touch switchId — it is the unique node ID the network
            // graph routes by (GetSwitchById / cable endpoints). Overwriting it with a
            // shared display name desyncs the switch from the graph and kills traffic.
            // The tier name is shown via the hover tooltip instead.
            // Re-assert port config after the game's Awake/insert logic (ports stay 0/0
            // when empty; the inserted transceiver drives the speed).
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
    // The game inserts via SFPModule.InsertedInSFPPort / InsertDirectlyIntoPort (not
    // always CableLink.InsertSFP), and sets the port speed from the SFP's native
    // nominal (0 for our custom modules), so we re-assert the tier speed afterwards.
    internal static class SfpSpeed
    {
        internal static void Apply(SFPModule module, CableLink link, string via)
        {
            if (module == null || link == null) return;
            int pid = -1;
            Core.TrySet(() => pid = module.GetComponent<UsableObject>().prefabID, "");
            if (!DeviceRegistry.TryGet(pid, out var e) || e.Kind != DeviceKind.Sfp) return;
            if (e.SpeedGbps > 0f)
            {
                float before = -1f; Core.TrySet(() => before = link.connectionSpeed, "");
                Core.TrySet(() => link.SetConnectionSpeed(e.SpeedGbps / TierConfig.SpeedDivisor), "insert link speed");
                float after = -1f; Core.TrySet(() => after = link.connectionSpeed, "");
                MelonLogger.Msg($"DCP insert[{via}]: tier prefabID={pid} link.connSpeed {before}->{after}");
            }
        }
    }

    [HarmonyPatch(typeof(SFPModule), nameof(SFPModule.InsertedInSFPPort))]
    internal static class PatchInsertedInSFPPort
    {
        private static void Postfix(SFPModule __instance, CableLink _link, bool immediate) => SfpSpeed.Apply(__instance, _link, "InsertedInSFPPort");
    }

    [HarmonyPatch(typeof(SFPModule), nameof(SFPModule.InsertDirectlyIntoPort))]
    internal static class PatchInsertDirectlyIntoPort
    {
        private static void Postfix(SFPModule __instance, CableLink _link) => SfpSpeed.Apply(__instance, _link, "InsertDirectly");
    }

    [HarmonyPatch(typeof(CableLink), nameof(CableLink.InsertSFP))]
    internal static class PatchInsertSFP
    {
        private static void Postfix(CableLink __instance, float speed, int type, SFPModule module) => SfpSpeed.Apply(module, __instance, "CableLink.InsertSFP");
    }

    // Diagnostic: dump a port's full state when hovered (rate-limited), so we can see
    // exactly where the 0/0 comes from on a connected custom port.
    [HarmonyPatch(typeof(CableLink), nameof(CableLink.OnHoverOver))]
    internal static class PatchCableHoverDump
    {
        private static float _last;
        private static void Postfix(CableLink __instance)
        {
            if (!TierConfig.LogPortDetails) return;
            float now = Time.unscaledTime;
            if (now - _last < 1.0f) return;
            _last = now;
            float cs = -1f; int sfpIn = -1, sfpSup = -1, swType = -2, srvType = -2; float sfpSpd = -1f; int sfpTy = -1;
            Core.TrySet(() => cs = __instance.connectionSpeed, "");
            Core.TrySet(() => sfpIn = __instance.sfpTypeInserted, "");
            Core.TrySet(() => sfpSup = __instance.sfpTypeSupported, "");
            Core.TrySet(() => { if (__instance.parentSwitch != null) swType = __instance.parentSwitch.switchType; }, "");
            Core.TrySet(() => { if (__instance.parentServer != null) srvType = __instance.parentServer.serverType; }, "");
            Core.TrySet(() => { if (__instance.insertedSFP != null) { sfpSpd = __instance.insertedSFP.speed; sfpTy = __instance.insertedSFP.sfpType; } }, "");
            MelonLogger.Msg($"DCP hover port: connSpeed={cs} sfpInserted={sfpIn} sfpSupported={sfpSup} switchType={swType} serverType={srvType} insertedSFP.speed={sfpSpd} insertedSFP.sfpType={sfpTy}");
        }
    }

    // Upgrade each module taken out of our custom 5-packs. We identify the pack by its
    // native prefabID (which survives Instantiate, unlike injected-component fields) and
    // look up the tier speed/module-id from the registry, then stamp them on the module.
    [HarmonyPatch(typeof(SFPBox), nameof(SFPBox.TakeSFPFromBox))]
    internal static class PatchTakeSFPFromBox
    {
        private static void Postfix(SFPBox __instance, ref SFPModule __result)
        {
            if (__result == null) return;
            var boxUsable = __instance.GetComponent<UsableObject>();
            if (boxUsable == null) return;
            if (!DeviceRegistry.TryGet(boxUsable.prefabID, out var box) || box.Kind != DeviceKind.SfpBox) return;

            var module = __result;
            // insertedSFP.speed drives the port's connectionSpeed natively, so this alone
            // makes the link report the tier speed when the module is inserted.
            Core.TrySet(() => module.speed = box.SpeedGbps / TierConfig.SpeedDivisor, "pack module speed");
            var usable = module.GetComponent<UsableObject>();
            if (usable != null) Core.TrySet(() => usable.prefabID = box.ModuleId, "pack module prefabID");
        }
    }

    // Backup + diagnostics: ensure a custom server reports its base type's required
    // ports even if the extended array somehow isn't consulted.
    [HarmonyPatch(typeof(MainGameManager), nameof(MainGameManager.GetRequiredPortsForServer))]
    internal static class PatchGetRequiredPortsForServer
    {
        private static void Postfix(MainGameManager __instance, INetworkEndpoint server, ref Il2CppStructArray<int> __result)
        {
            if (server == null) return;
            int sType = -1;
            Core.TrySet(() => sType = server.serverType, "");
            if (!DeviceRegistry.TryGet(sType, out var e) || e.Kind != DeviceKind.Server) return;

            int len = __result != null ? __result.Length : 0;
            if (len == 0 && MainGameManager.defaultPortsPerServerType != null
                && e.BaseType >= 0 && e.BaseType < MainGameManager.defaultPortsPerServerType.Length)
            {
                __result = MainGameManager.defaultPortsPerServerType[e.BaseType];
                len = __result != null ? __result.Length : 0;
            }
            if (TierConfig.LogPortDetails)
                MelonLogger.Msg($"DCP reqPorts: serverType={sType} baseType={e.BaseType} ports={len}");
        }
    }

    // Raise the per-port rated speed on ALL patch panels so 400G links aren't capped.
    [HarmonyPatch(typeof(PatchPanel), nameof(PatchPanel.InsertedInRack))]
    internal static class PatchPatchPanelInserted
    {
        private static void Postfix(PatchPanel __instance, PatchPanelSaveData saveData)
        {
            if (!TierConfig.RaisePatchPanelCap) return;
            Core.ConfigurePorts(__instance.gameObject, -1, TierConfig.PatchPanelPortSpeedGbps, "patchpanel (rack)", false);
        }
    }

    [HarmonyPatch(typeof(PatchPanel), nameof(PatchPanel.Awake))]
    internal static class PatchPatchPanelAwake
    {
        private static void Postfix(PatchPanel __instance)
        {
            if (!TierConfig.RaisePatchPanelCap) return;
            Core.ConfigurePorts(__instance.gameObject, -1, TierConfig.PatchPanelPortSpeedGbps, "patchpanel (awake)", false);
        }
    }
}
