using Il2Cpp;
using Il2CppInterop.Runtime;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using Il2CppInterop.Runtime.Injection;
using MelonLoader;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[assembly: MelonInfo(typeof(DataCenterPlus.Core), "DataCenterPlus", "1.0.16", "brzb0 + contributors")]
[assembly: MelonGame("Waseku", "Data Center")]

namespace DataCenterPlus
{
    public class Core : MelonMod
    {
        internal static int    BaseSwitchType  = -1;   // index of the top QSFP+ switch
        internal static int    BaseSfpType     = -1;   // index of a base SFP transceiver
        internal static int    BaseSfpBoxType  = -1;   // index of a base SFP box (5-pack)
        internal static int    BaseCableType   = -1;   // index of a base fiber cable reel
        internal static GameObject TemplateHolder { get; private set; }

        public override void OnInitializeMelon()
        {
            ClassInjector.RegisterTypeInIl2Cpp<ShopButtonHandler>();
        }

        // ---------------------------------------------------------------- REGISTRY
        internal static void SetupRegistry(MainGameManager mgm)
        {
            if (mgm == null) return;
            DeviceRegistry.Clear();

            if (TemplateHolder != null) Object.Destroy(TemplateHolder);
            TemplateHolder = new GameObject("DataCenterPlus_TemplateHolder");
            TemplateHolder.SetActive(false);
            Object.DontDestroyOnLoad(TemplateHolder);

            SetupNetworking(mgm);
            SetupServers(mgm);
            SetupSfp(mgm);
            SetupCables(mgm);

            int nSwitch = 0, nRouter = 0, nFire = 0, nServer = 0, nSfp = 0, nBox = 0, nCable = 0;
            foreach (var kv in DeviceRegistry.Entries)
                switch (kv.Value.Kind)
                {
                    case DeviceKind.Switch: nSwitch++; break;
                    case DeviceKind.Router: nRouter++; break;
                    case DeviceKind.Firewall: nFire++; break;
                    case DeviceKind.Server: nServer++; break;
                    case DeviceKind.Sfp: nSfp++; break;
                    case DeviceKind.SfpBox: nBox++; break;
                    case DeviceKind.Cable: nCable++; break;
                }
            MelonLogger.Msg($"DataCenterPlus: arrays — switches={Len(mgm.switchesPrefabs)} routers={Len(mgm.routersPrefabs)} " +
                            $"firewalls={Len(mgm.firewallsPrefabs)} servers={Len(mgm.serverPrefabs)} " +
                            $"sfp={Len(mgm.sfpPrefabs)} sfpBoxes={Len(mgm.sfpsBoxedPrefab)} cables={Len(mgm.cableSpinnerPrefab)}");
            MelonLogger.Msg($"DataCenterPlus: registered — switch={nSwitch} router={nRouter} firewall={nFire} " +
                            $"server={nServer} sfpModule={nSfp} sfpBox={nBox} cable={nCable}");
        }

        private static int Len(Il2CppReferenceArray<GameObject> a) => a?.Length ?? -1;

        // Logs all prefab names, then returns the index of the first prefab matching the
        // HIGHEST-priority keyword (keywords are tried in order), else first non-null.
        private static int PickByName(Il2CppReferenceArray<GameObject> arr, params string[] keywords)
        {
            if (arr == null) return -1;
            int firstNonNull = -1;
            var names = new System.Text.StringBuilder();
            for (int i = 0; i < arr.Length; i++)
            {
                if (arr[i] == null) continue;
                if (firstNonNull < 0) firstNonNull = i;
                names.Append($" [{i}]{arr[i].name}");
            }
            if (TierConfig.LogPortDetails)
                MelonLogger.Msg($"DataCenterPlus: candidates:{names}");

            foreach (var k in keywords)
                for (int i = 0; i < arr.Length; i++)
                    if (arr[i] != null && arr[i].name.ToLowerInvariant().Contains(k))
                        return i;
            return firstNonNull;
        }

        private static void SetupNetworking(MainGameManager mgm)
        {
            var switches = mgm.switchesPrefabs;
            if (switches == null || switches.Length == 0) return;

            int best = -1;
            for (int i = 0; i < switches.Length; i++)
            {
                var go = switches[i];
                if (go == null) continue;
                string n = go.name.ToLowerInvariant();
                if (n.Contains("32") && n.Contains("qsfp")) { best = i; break; }
            }
            if (best < 0) best = switches.Length - 1;
            if (best < 0) return;
            BaseSwitchType = best;
            MelonLogger.Msg($"DataCenterPlus: base switch = '{switches[best].name}' (type {best})");

            // Register switch/router/firewall for each networking tier. Each tier's
            // ports are set to accept that tier's transceiver (SFP_ID_BASE + t).
            for (int t = 0; t < TierConfig.NetTiers.Length; t++)
            {
                var tier = TierConfig.NetTiers[t];
                int portType = TierConfig.MatchDevicePortsToTier ? DeviceRegistry.SFP_ID_BASE + t : -1;
                RegisterNet(DeviceKind.Switch,   DeviceRegistry.SWITCH_ID_BASE   + t, tier, $"Switch {tier.Label}",   portType);
                RegisterNet(DeviceKind.Router,   DeviceRegistry.ROUTER_ID_BASE   + t, tier, $"Router {tier.Label}",   portType);
                RegisterNet(DeviceKind.Firewall, DeviceRegistry.FIREWALL_ID_BASE + t, tier, $"Firewall {tier.Label}", portType);
            }

            mgm.switchesPrefabs   = ExtendWithTemplates(mgm, mgm.switchesPrefabs,   DeviceKind.Switch);
            mgm.routersPrefabs    = ExtendWithTemplates(mgm, mgm.routersPrefabs,    DeviceKind.Router);
            mgm.firewallsPrefabs  = ExtendWithTemplates(mgm, mgm.firewallsPrefabs,  DeviceKind.Firewall);
        }

        private static void RegisterNet(DeviceKind kind, int id, NetTier tier, string name, int portSfpType)
        {
            int shopType = kind == DeviceKind.Switch ? (int)PlayerManager.ObjectInHand.Switch
                         : kind == DeviceKind.Router ? (int)PlayerManager.ObjectInHand.Router
                         : (int)PlayerManager.ObjectInHand.Firewall;
            DeviceRegistry.Register(new DeviceRegistry.Entry
            {
                CustomId    = id,
                BaseType    = BaseSwitchType,
                Kind        = kind,
                DisplayName = name,
                Price       = tier.Price,
                Color       = tier.Color,
                IconColor   = tier.Color,
                SpeedGbps   = tier.SpeedGbps,
                ShopItemType = shopType,
                PortSfpType = portSfpType,
                TargetIops  = 0f,
            });
        }

        // Walks a cloned device's CableLink ports; logs them and (optionally) retypes
        // the SFP ports so they accept a given module type. Returns a short summary.
        internal static void ConfigurePorts(GameObject root, int portSfpType, float portSpeed, string label, bool verbose = true)
        {
            if (root == null) return;
            var links = root.GetComponentsInChildren<CableLink>(true);
            if (links == null) return;
            int count = links.Count, sfpPorts = 0, changed = 0;
            string sample = "";
            for (int i = 0; i < links.Count; i++)
            {
                var lk = links[i];
                if (lk == null) continue;
                bool isSfp = false, isFibre = false; int supported = -999; float spd = -1f;
                try { isSfp = lk.isSFPPort; } catch { }
                try { isFibre = lk.isFibrePort; } catch { }
                try { supported = lk.sfpTypeSupported; } catch { }
                try { spd = lk.connectionSpeed; } catch { }
                if (i < 2) sample += $" [p{i}: sfpSup={supported} spd={spd} sfp={isSfp} fib={isFibre}]";

                if (isSfp || isFibre) sfpPorts++;

                // Retype every port (not just flagged ones): on an inactive template the
                // isSFPPort/isFibrePort flags may not be initialised yet.
                if (portSfpType >= 0)
                { TrySet(() => lk.sfpTypeSupported = portSfpType, "port.sfpTypeSupported"); changed++; }

                if (portSpeed > 0f)
                    TrySet(() => lk.SetConnectionSpeed(portSpeed / TierConfig.SpeedDivisor), "port.connectionSpeed");
            }
            if (verbose && TierConfig.LogPortDetails)
                MelonLogger.Msg($"DataCenterPlus: {label} ports={count} sfpPorts={sfpPorts} retyped={changed} ->{portSfpType}{sample}");
        }

        private static void SetupServers(MainGameManager mgm)
        {
            var servers = mgm.serverPrefabs;
            if (servers == null || servers.Length == 0) return;

            for (int i = 0; i < servers.Length; i++)
            {
                var go = servers[i];
                if (go == null) continue;

                float baseIops = 0f;
                int shopType = (int)PlayerManager.ObjectInHand.Server3U;
                int baseServerType = -1, baseAppId = -1;
                var srv = go.GetComponent<Server>();
                if (srv != null)
                {
                    baseIops = srv.maxProcessingSpeed;
                    try { shopType = (int)srv.objectInHandType; } catch { }
                    try { baseServerType = srv.serverType; } catch { }
                    try { baseAppId = srv.appID; } catch { }
                }

                float target = ComputeTargetIops(baseIops);

                string baseName = SafeServerName(mgm, i, go);
                int id = DeviceRegistry.SERVER_ID_BASE + i;

                DeviceRegistry.Register(new DeviceRegistry.Entry
                {
                    CustomId    = id,
                    BaseType    = i,
                    Kind        = DeviceKind.Server,
                    DisplayName = BuildHpcName(baseName),
                    Price       = Mathf.Max(1, Mathf.RoundToInt(target)), // IOPS-based default; refined from shop price if found
                    Color       = TierConfig.ServerTint,
                    IconColor   = TierConfig.ServerIconAccent,
                    SpeedGbps   = 0f,
                    ShopItemType = shopType,
                    TargetIops  = target,
                });

                MelonLogger.Msg($"DataCenterPlus: server[{i}] '{baseName}' baseServerType={baseServerType} baseAppID={baseAppId} baseIOPS={baseIops} -> {target} (customId {id}, shopType {shopType})");
            }

            mgm.serverPrefabs = ExtendWithTemplates(mgm, mgm.serverPrefabs, DeviceKind.Server);
        }

        internal static float ComputeTargetIops(float baseIops)
        {
            if (TierConfig.ServerUseAbsoluteIops)
                return baseIops < TierConfig.ServerSmallLargeSplit
                    ? TierConfig.ServerAbsoluteIopsSmall
                    : TierConfig.ServerAbsoluteIopsLarge;
            return baseIops * TierConfig.ServerIopsMultiplier;
        }

        private static string SafeServerName(MainGameManager mgm, int type, GameObject go)
        {
            string n = null;
            try { n = mgm.ReturnServerNameFromType(type); } catch { }
            if (string.IsNullOrEmpty(n)) n = go.name;
            return n;
        }

        // Base server names embed their IOPS (e.g. "System X 3U 5000 IOPS"); rewrite that
        // number to the boosted value so the label isn't misleading, then add the suffix.
        private static string BuildHpcName(string baseName)
        {
            string n = baseName ?? "";
            if (!TierConfig.ServerUseAbsoluteIops)
            {
                try
                {
                    var mt = System.Text.RegularExpressions.Regex.Match(n, @"(\d+)(\s*[Ii][Oo][Pp])");
                    if (mt.Success)
                    {
                        int oldVal = int.Parse(mt.Groups[1].Value);
                        int newVal = Mathf.RoundToInt(oldVal * TierConfig.ServerIopsMultiplier);
                        int at = mt.Groups[1].Index, len = mt.Groups[1].Length;
                        n = n.Substring(0, at) + newVal + n.Substring(at + len);
                    }
                }
                catch { }
            }
            return n + TierConfig.ServerNameSuffix;
        }

        private static void SetupSfp(MainGameManager mgm)
        {
            var sfps = mgm.sfpPrefabs;
            if (sfps == null || sfps.Length == 0) return;

            int best = PickByName(sfps, "qsfp", "40");
            if (best < 0) return;
            BaseSfpType = best;
            MelonLogger.Msg($"DataCenterPlus: base SFP = '{sfps[best].name}' (type {best})");

            for (int t = 0; t < TierConfig.SfpTiers.Length; t++)
            {
                var tier = TierConfig.SfpTiers[t];
                DeviceRegistry.Register(new DeviceRegistry.Entry
                {
                    CustomId    = DeviceRegistry.SFP_ID_BASE + t,
                    BaseType    = BaseSfpType,
                    Kind        = DeviceKind.Sfp,
                    DisplayName = tier.Label,
                    Price       = tier.Price,
                    Color       = new Color(0, 0, 0, 0),
                    IconColor   = tier.Color,
                    SpeedGbps   = tier.SpeedGbps,
                    TargetIops  = 0f,
                });
            }

            mgm.sfpPrefabs = ExtendWithTemplates(mgm, mgm.sfpPrefabs, DeviceKind.Sfp);

            // SFP boxes (5-packs) — this is how transceivers are actually sold. We clone
            // a base box (keeps the vanilla fill of 5) and upgrade each module on the way out.
            var boxes = mgm.sfpsBoxedPrefab;
            int boxBest = PickByName(boxes, "qsfp", "40");
            if (boxBest < 0)
            {
                MelonLogger.Warning("DataCenterPlus: no base SFP box found; transceiver packs will not be sellable.");
                return;
            }
            BaseSfpBoxType = boxBest;
            MelonLogger.Msg($"DataCenterPlus: base SFP box = '{boxes[boxBest].name}' (type {boxBest})");

            for (int t = 0; t < TierConfig.SfpTiers.Length; t++)
            {
                var tier = TierConfig.SfpTiers[t];
                DeviceRegistry.Register(new DeviceRegistry.Entry
                {
                    CustomId    = DeviceRegistry.SFPBOX_ID_BASE + t,
                    BaseType    = BaseSfpBoxType,
                    Kind        = DeviceKind.SfpBox,
                    DisplayName = tier.Label + " (5-pack)",
                    Price       = tier.Price,
                    Color       = tier.Color,
                    IconColor   = tier.Color,
                    SpeedGbps   = tier.SpeedGbps,
                    ModuleId    = DeviceRegistry.SFP_ID_BASE + t,
                    ShopItemType = (int)PlayerManager.ObjectInHand.SFPBox,
                    TargetIops  = 0f,
                });
            }

            mgm.sfpsBoxedPrefab = ExtendWithTemplates(mgm, mgm.sfpsBoxedPrefab, DeviceKind.SfpBox);
        }

        private static void SetupCables(MainGameManager mgm)
        {
            var cables = mgm.cableSpinnerPrefab;
            if (cables == null || cables.Length == 0)
            {
                MelonLogger.Warning("DataCenterPlus: no cable prefabs; tier cables will not be added.");
                return;
            }
            // Prefer a fiber QSFP cable as the clone base.
            // Candidates use British spelling "Fibre"; prefer the 4-lane (QSFP) fibre cable.
            int best = PickByName(cables, "fibre 4", "fibre", "qsfp");
            if (best < 0) best = PickByName(cables, "fiber");
            if (best < 0) return;
            BaseCableType = best;
            MelonLogger.Msg($"DataCenterPlus: base cable = '{cables[best].name}' (type {best})");

            for (int t = 0; t < TierConfig.CableTiers.Length; t++)
            {
                var ct = TierConfig.CableTiers[t];
                var color = t < TierConfig.SfpTiers.Length ? TierConfig.SfpTiers[t].Color : new Color(1, 1, 1, 1);
                DeviceRegistry.Register(new DeviceRegistry.Entry
                {
                    CustomId     = DeviceRegistry.CABLE_ID_BASE + t,
                    BaseType     = BaseCableType,
                    Kind         = DeviceKind.Cable,
                    DisplayName  = ct.Label,
                    Price        = ct.Price,
                    Color        = color,
                    IconColor    = color,
                    ShopItemType = (int)PlayerManager.ObjectInHand.CableSpinner,
                    TargetIops   = 0f,
                });
            }

            mgm.cableSpinnerPrefab = ExtendWithTemplates(mgm, mgm.cableSpinnerPrefab, DeviceKind.Cable);
        }
        private static Il2CppReferenceArray<GameObject> ExtendWithTemplates(
            MainGameManager mgm, Il2CppReferenceArray<GameObject> existing, DeviceKind kind)
        {
            int existingLen = existing?.Length ?? 0;
            int maxId = existingLen - 1;
            foreach (var kv in DeviceRegistry.Entries)
                if (kv.Value.Kind == kind && kv.Value.CustomId > maxId) maxId = kv.Value.CustomId;

            var extended = new GameObject[maxId + 1];
            for (int i = 0; i < existingLen; i++) extended[i] = existing[i];

            foreach (var kv in DeviceRegistry.Entries)
            {
                var e = kv.Value;
                if (e.Kind != kind) continue;
                var template = BuildPrefab(mgm, e, TemplateHolder.transform);
                if (template != null)
                {
                    template.name = $"DCP_template_{e.CustomId}";
                    extended[e.CustomId] = template;
                }
            }
            return extended;
        }

        // ------------------------------------------------------------- PREFAB BUILD
        internal static GameObject BuildPrefab(MainGameManager mgm, DeviceRegistry.Entry entry, Transform parent = null)
        {
            switch (entry.Kind)
            {
                case DeviceKind.Switch:
                case DeviceKind.Router:
                case DeviceKind.Firewall:
                    return BuildNetDevice(mgm, entry, parent);
                case DeviceKind.Server:
                    return BuildServer(mgm, entry, parent);
                case DeviceKind.Sfp:
                    return BuildSfp(mgm, entry, parent);
                case DeviceKind.SfpBox:
                    return BuildSfpBox(mgm, entry, parent);
                case DeviceKind.Cable:
                    return BuildCable(mgm, entry, parent);
            }
            return null;
        }

        private static GameObject BuildCable(MainGameManager mgm, DeviceRegistry.Entry entry, Transform parent)
        {
            var basePrefab = (entry.BaseType >= 0 && entry.BaseType < mgm.cableSpinnerPrefab.Length)
                ? mgm.cableSpinnerPrefab[entry.BaseType] : null;
            var clone = Clone(basePrefab, parent);
            if (clone == null) return null;
            clone.name = $"DCP_cable_{entry.CustomId}";

            var usable = clone.GetComponent<UsableObject>();
            if (usable != null) TrySet(() => usable.prefabID = entry.CustomId, "cable.prefabID");

            ApplyNameAndHover(clone, entry);
            ApplyTint(clone, entry.Color);
            return clone;
        }

        private static GameObject Clone(GameObject basePrefab, Transform parent)
        {
            if (basePrefab == null) return null;
            return parent != null ? Object.Instantiate(basePrefab, parent, false)
                                   : Object.Instantiate(basePrefab);
        }

        private static GameObject BuildNetDevice(MainGameManager mgm, DeviceRegistry.Entry entry, Transform parent)
        {
            var basePrefab = (BaseSwitchType >= 0 && BaseSwitchType < mgm.switchesPrefabs.Length)
                ? mgm.switchesPrefabs[BaseSwitchType] : null;
            var clone = Clone(basePrefab, parent);
            if (clone == null) return null;
            clone.name = $"DCP_net_{entry.CustomId}";

            var netSwitch = clone.GetComponent<NetworkSwitch>();
            if (netSwitch != null)
                TrySet(() => netSwitch.switchType = entry.CustomId, "net.switchType");

            // Note: Router.routingTable / asn and Firewall.filterRules / clusterIP are
            // initialised by the game on Awake and their setters vary across game
            // versions, so we deliberately do NOT set them here (avoids MissingMethod).
            // We also never set switchId — it's the unique network-graph node ID.
            if (entry.Kind == DeviceKind.Router)
            {
                try
                {
                    var router = clone.AddComponent<Router>();
                    TrySet(() => router.switchType = entry.CustomId, "router.switchType");
                }
                catch (System.Exception e) { MelonLogger.Warning($"DataCenterPlus: AddComponent<Router> failed: {e.Message}"); }
            }
            else if (entry.Kind == DeviceKind.Firewall)
            {
                try
                {
                    var firewall = clone.AddComponent<Firewall>();
                    TrySet(() => firewall.switchType = entry.CustomId, "firewall.switchType");
                }
                catch (System.Exception e) { MelonLogger.Warning($"DataCenterPlus: AddComponent<Firewall> failed: {e.Message}"); }
            }

            var usable = clone.GetComponent<UsableObject>();
            if (usable != null) TrySet(() => usable.prefabID = entry.CustomId, "net.prefabID");

            // SFP ports stay 0/0 when empty (vanilla behaviour) — the inserted
            // transceiver's speed drives the port. We don't set a rated port speed here.
            ConfigurePorts(clone, entry.PortSfpType, 0f, $"net {entry.CustomId}");

            ApplyNameAndHover(clone, entry);
            ApplyTint(clone, entry.Color);
            return clone;
        }

        // Drift guard: run a property setter, logging and continuing if the game
        // version doesn't have it (so one missing setter can't abort the whole setup).
        internal static void TrySet(System.Action set, string what)
        {
            try { set(); }
            catch (System.Exception e) { MelonLogger.Warning($"DataCenterPlus: skipped {what} ({e.GetType().Name})"); }
        }

        private static GameObject BuildServer(MainGameManager mgm, DeviceRegistry.Entry entry, Transform parent)
        {
            var basePrefab = (entry.BaseType >= 0 && entry.BaseType < mgm.serverPrefabs.Length)
                ? mgm.serverPrefabs[entry.BaseType] : null;
            var clone = Clone(basePrefab, parent);
            if (clone == null) return null;
            clone.name = $"DCP_server_{entry.CustomId}";

            var srv = clone.GetComponent<Server>();
            if (srv != null)
                // IMPORTANT: do NOT change serverType — it is the app type (0-3) the
                // customer matches servers on (GetServerTypeForIP). Keep the base value
                // so the server is recognised as serving the app. We only boost IOPS and
                // identify the HPC variant via prefabID.
                TrySet(() => { if (entry.TargetIops > 0f) srv.maxProcessingSpeed = entry.TargetIops; }, "server IOPS");

            var usable = clone.GetComponent<UsableObject>();
            if (usable != null) TrySet(() => usable.prefabID = entry.CustomId, "server.prefabID");

            // Log the server's onboard ports and (optionally) raise their speed so the
            // boosted IOPS can actually flow instead of bottlenecking on a 1GbE port.
            float portSpeed = TierConfig.ApplyServerPortSpeed ? TierConfig.ServerPortSpeedGbps : 0f;
            ConfigurePorts(clone, TierConfig.ServerPortSfpType, portSpeed, $"server {entry.CustomId}");

            ApplyNameAndHover(clone, entry);
            ApplyTint(clone, entry.Color);
            return clone;
        }

        private static GameObject BuildSfp(MainGameManager mgm, DeviceRegistry.Entry entry, Transform parent)
        {
            var basePrefab = (entry.BaseType >= 0 && entry.BaseType < mgm.sfpPrefabs.Length)
                ? mgm.sfpPrefabs[entry.BaseType] : null;
            var clone = Clone(basePrefab, parent);
            if (clone == null) return null;
            clone.name = $"DCP_sfp_{entry.CustomId}";

            var sfp = clone.GetComponent<SFPModule>();
            if (sfp != null)
                TrySet(() => sfp.speed = entry.SpeedGbps / TierConfig.SpeedDivisor, "sfp.speed");
            // NB: we deliberately keep the native sfpType (QSFP) so cable/box/port
            // compatibility works; the tier is identified by prefabID + speed instead.

            var usable = clone.GetComponent<UsableObject>();
            if (usable != null) TrySet(() => usable.prefabID = entry.CustomId, "sfp.prefabID");

            ApplyNameAndHover(clone, entry);
            return clone;
        }

        private static GameObject BuildSfpBox(MainGameManager mgm, DeviceRegistry.Entry entry, Transform parent)
        {
            var basePrefab = (entry.BaseType >= 0 && entry.BaseType < mgm.sfpsBoxedPrefab.Length)
                ? mgm.sfpsBoxedPrefab[entry.BaseType] : null;
            var clone = Clone(basePrefab, parent);
            if (clone == null) return null;
            clone.name = $"DCP_sfpbox_{entry.CustomId}";

            // Keep the base sfpBoxType so the vanilla box still fills itself with 5
            // modules. The box is identified by its prefabID (set below); TakeSFPFromBox
            // looks that up to stamp the tier speed on each dispensed module.
            var usable = clone.GetComponent<UsableObject>();
            if (usable != null) TrySet(() => usable.prefabID = entry.CustomId, "sfpbox.prefabID");

            ApplyNameAndHover(clone, entry);
            ApplyTint(clone, entry.Color);
            return clone;
        }

        // Gives a cloned item a hover/label name via a synthetic localisation UID that
        // our Localisation.ReturnTextByID patch resolves to the entry's DisplayName.
        internal static void ApplyNameAndHover(GameObject clone, DeviceRegistry.Entry entry)
        {
            if (clone == null) return;
            var usable = clone.GetComponent<UsableObject>();
            if (usable == null) return;
            int uid = DeviceRegistry.NAME_UID_BASE + entry.CustomId;
            TrySet(() => usable.onHoverTextUID = uid, "onHoverTextUID");
            TrySet(() => usable.toolTipLocalisationID = uid, "toolTipLocalisationID");
            TrySet(() => usable.labelText = entry.DisplayName, "labelText");
        }

        internal static void ApplyTint(GameObject root, Color tint)
        {
            if (root == null || tint.a <= 0f) return;
            string[] props = { "_Color", "_BaseColor", "_MainColor", "_TintColor", "_Tint", "_AlbedoColor" };
            var renderers = root.GetComponentsInChildren<Renderer>(true);
            foreach (var rend in renderers)
            {
                if (rend == null) continue;
                var mats = rend.materials;
                bool changed = false;
                for (int m = 0; m < mats.Length; m++)
                {
                    if (mats[m] == null) continue;
                    foreach (var p in props)
                        if (mats[m].HasProperty(p)) { mats[m].SetColor(p, tint); changed = true; }
                }
                if (changed) { rend.materials = mats; break; }
            }
        }

        // ------------------------------------------------------------- SHOP INJECT
        public override void OnSceneWasLoaded(int buildIndex, string sceneName)
        {
            if (buildIndex != 0) MelonCoroutines.Start(AddShopItems());
        }

        private IEnumerator AddShopItems()
        {
            yield return new WaitForSeconds(1.5f);

            var mgm = MainGameManager.instance;
            if (mgm == null) { MelonLogger.Warning("DataCenterPlus: no MainGameManager at shop injection."); yield break; }
            var shop = mgm.computerShop;
            if (shop == null || shop.shopItems == null) { MelonLogger.Warning("DataCenterPlus: no computerShop/shopItems at injection."); yield break; }

            // Diagnostic dump of the existing shop so we can see item types/ids/names.
            MelonLogger.Msg($"DataCenterPlus: shop has {shop.shopItems.Length} items:");
            ShopItem anySource = null, switchSource = null, anySwitch = null, boxSource = null, anyBox = null, cableSource = null, anyCable = null;
            var serverSources = new System.Collections.Generic.Dictionary<int, ShopItem>();
            for (int i = 0; i < shop.shopItems.Length; i++)
            {
                var si = shop.shopItems[i];
                if (si == null || si.shopItemSO == null) continue;
                if (anySource == null) anySource = si;
                var so = si.shopItemSO;
                MelonLogger.Msg($"    [{i}] type={(int)so.itemType}({so.itemType}) id={so.itemID} name='{so.itemName}'");

                int it = (int)so.itemType;
                if (it == (int)PlayerManager.ObjectInHand.Switch)
                { if (anySwitch == null) anySwitch = si; if (so.itemID == BaseSwitchType) switchSource = si; }
                else if (it == (int)PlayerManager.ObjectInHand.SFPBox)
                { if (anyBox == null) anyBox = si; if (so.itemID == BaseSfpBoxType) boxSource = si; }
                else if (it == (int)PlayerManager.ObjectInHand.CableSpinner)
                { if (anyCable == null) anyCable = si; if (so.itemID == BaseCableType) cableSource = si; }
                else if (IsServerType(so.itemType) && !serverSources.ContainsKey(so.itemID))
                    serverSources[so.itemID] = si;
            }
            switchSource = switchSource ?? anySwitch;
            boxSource = boxSource ?? anyBox;
            cableSource = cableSource ?? anyCable;

            var parent = ResolveShopParent(shop);
            if (parent == null) { MelonLogger.Warning("DataCenterPlus: could not resolve shop parent."); yield break; }
            MelonLogger.Msg($"DataCenterPlus: shop parent='{parent.name}', switchSource={(switchSource!=null)}, boxSource={(boxSource!=null)}, cableSource={(cableSource!=null)}, serverSources={serverSources.Count}");

            if (anySource == null) { MelonLogger.Warning("DataCenterPlus: no shop item to clone; aborting injection."); yield break; }

            int added = 0;
            foreach (var kv in DeviceRegistry.Entries)
            {
                var e = kv.Value;
                if (e.Kind == DeviceKind.Sfp) continue; // loose modules aren't sold; packs (SfpBox) are

                // Clone the closest-matching base card so icons/styling look right.
                ShopItem src = anySource;
                if (e.Kind == DeviceKind.Switch || e.Kind == DeviceKind.Router || e.Kind == DeviceKind.Firewall)
                    src = switchSource ?? anySource;
                else if (e.Kind == DeviceKind.SfpBox)
                    src = boxSource ?? anySource;
                else if (e.Kind == DeviceKind.Cable)
                    src = cableSource ?? anySource;
                else if (e.Kind == DeviceKind.Server && serverSources.TryGetValue(e.BaseType, out var ssrc))
                {
                    src = ssrc;
                    e.Price = Mathf.Max(1, Mathf.RoundToInt(ssrc.shopItemSO.price * TierConfig.ServerPriceMultiplier));
                }

                var sprite = src.shopItemSO.sprite;
                if (AddShopButton(shop, src, parent, e, sprite) != null) added++;
                else MelonLogger.Warning($"DataCenterPlus: failed to add button for '{e.DisplayName}' (id {e.CustomId}).");
            }

            GrowShopContainer(parent, anySource, added);
            Canvas.ForceUpdateCanvases();
            MelonLogger.Msg($"DataCenterPlus: injected {added}/{DeviceRegistry.Entries.Count} shop item(s).");
        }

        private static bool IsServerType(PlayerManager.ObjectInHand t)
        {
            return t == PlayerManager.ObjectInHand.Server1U
                || t == PlayerManager.ObjectInHand.Server2U
                || t == PlayerManager.ObjectInHand.Server3U;
        }

        private static GameObject ResolveShopParent(ComputerShop shop)
        {
            var shopParent = shop.shopItemParent;
            if (shopParent == null) return null;
            var section = shopParent.transform.Find(TierConfig.ShopSectionName);
            return section != null ? section.gameObject : shopParent;
        }

        private static GameObject AddShopButton(ComputerShop shop, ShopItem source, GameObject parent,
                                                DeviceRegistry.Entry entry, Sprite sprite)
        {
            var itemType = (PlayerManager.ObjectInHand)entry.ShopItemType;

            var so = ScriptableObject.CreateInstance<ShopItemSO>();
            so.itemName   = entry.DisplayName;
            so.price      = entry.Price;
            so.xpToUnlock = 0;
            so.itemType   = itemType;
            so.itemID     = entry.CustomId;
            so.eol        = source.shopItemSO.eol;
            so.sprite     = sprite != null ? sprite : source.shopItemSO.sprite;

            var cloned = Object.Instantiate(source.gameObject, parent.transform, false);
            cloned.name = $"ShopItem_DCP_{entry.CustomId}";
            cloned.transform.localPosition = Vector3.zero;
            cloned.transform.localScale = Vector3.one;

            var shopItem = cloned.GetComponent<ShopItem>();
            if (shopItem == null) { Object.Destroy(cloned); return null; }

            shopItem.shopItemSO = so;
            shopItem.guid = $"dcp_{entry.CustomId}";
            if (shopItem.txtName != null) shopItem.txtName.text = entry.DisplayName;
            if (shopItem.txtPrice != null) shopItem.txtPrice.text = $"${entry.Price:N0}";
            shopItem.itemDisplayName = entry.DisplayName;

            // Distinct shop icon via recolor (no new texture needed).
            if (entry.IconColor.a > 0f && shopItem.itemIcon != null)
                shopItem.itemIcon.color = entry.IconColor;

            // Drive the purchase with our own handler instead of relying on the game's
            // button wiring (which varies by version and left clicks inert).
            var btnExt = cloned.GetComponent<ButtonExtended>();
            if (btnExt != null)
            {
                btnExt.doSubmitOnSelect = false;
                btnExt.selectOnPointerEnter = false;

                var h = cloned.AddComponent<ShopButtonHandler>();
                h.shop = shop;
                h.itemID = entry.CustomId;
                h.price = entry.Price;
                h.itemType = itemType;
                h.displayName = entry.DisplayName;
                var action = DelegateSupport.ConvertDelegate<UnityEngine.Events.UnityAction>(h.OnClick);

                if (btnExt.onClick != null) btnExt.onClick.RemoveAllListeners();
                btnExt.m_OnClick.AddListener(action);
                btnExt.functionToBeCalledOnSelect.RemoveAllListeners();
                btnExt.functionToBeCalledOnSelect.AddListener(action);
            }

            cloned.SetActive(true);
            return cloned;
        }

        private static void GrowShopContainer(GameObject parent, ShopItem source, int addedCount)
        {
            if (addedCount <= 0 || source == null) return;
            float itemHeight = 0f;
            var srcRt = source.GetComponent<RectTransform>();
            if (srcRt != null) itemHeight = srcRt.rect.height;
            if (itemHeight <= 0f) return;

            float grow = (itemHeight + 20f) * addedCount + 100f;
            var rt = parent.GetComponent<RectTransform>();
            if (rt != null) { var sd = rt.sizeDelta; sd.y += grow; rt.sizeDelta = sd; }

            var contentParent = parent.transform.parent;
            if (contentParent != null)
            {
                var crt = contentParent.GetComponent<RectTransform>();
                if (crt != null) { var csd = crt.sizeDelta; csd.y += grow; crt.sizeDelta = csd; }
            }
        }
    }
}
