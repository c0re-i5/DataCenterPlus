using Il2Cpp;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using Il2CppInterop.Runtime.Injection;
using MelonLoader;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[assembly: MelonInfo(typeof(DataCenterPlus.Core), "DataCenterPlus", "1.0.0", "brzb0 + contributors")]
[assembly: MelonGame("Waseku", "Data Center")]

namespace DataCenterPlus
{
    public class Core : MelonMod
    {
        internal static int    BaseSwitchType  = -1;   // index of the top QSFP+ switch
        internal static int    BaseSfpType     = -1;   // index of a base SFP transceiver
        internal static int    BaseSfpBoxType  = -1;   // index of a base SFP box (5-pack)
        internal static Sprite BaseSwitchSprite;
        internal static GameObject TemplateHolder { get; private set; }

        public override void OnInitializeMelon()
        {
            ClassInjector.RegisterTypeInIl2Cpp<CartButtonHandler>();
            ClassInjector.RegisterTypeInIl2Cpp<DcpPackTag>();
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

            // Register switch/router/firewall for each networking tier.
            for (int t = 0; t < TierConfig.NetTiers.Length; t++)
            {
                var tier = TierConfig.NetTiers[t];
                RegisterNet(DeviceKind.Switch,   DeviceRegistry.SWITCH_ID_BASE   + t, tier, $"Switch {tier.Label}");
                RegisterNet(DeviceKind.Router,   DeviceRegistry.ROUTER_ID_BASE   + t, tier, $"Router {tier.Label}");
                RegisterNet(DeviceKind.Firewall, DeviceRegistry.FIREWALL_ID_BASE + t, tier, $"Firewall {tier.Label}");
            }

            mgm.switchesPrefabs   = ExtendWithTemplates(mgm, mgm.switchesPrefabs,   DeviceKind.Switch);
            mgm.routersPrefabs    = ExtendWithTemplates(mgm, mgm.routersPrefabs,    DeviceKind.Router);
            mgm.firewallsPrefabs  = ExtendWithTemplates(mgm, mgm.firewallsPrefabs,  DeviceKind.Firewall);
        }

        private static void RegisterNet(DeviceKind kind, int id, NetTier tier, string name)
        {
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
                TargetIops  = 0f,
            });
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
                var srv = go.GetComponent<Server>();
                if (srv != null) baseIops = srv.maxProcessingSpeed;

                float target = ComputeTargetIops(baseIops);

                string baseName = SafeServerName(mgm, i, go);
                int id = DeviceRegistry.SERVER_ID_BASE + i;

                DeviceRegistry.Register(new DeviceRegistry.Entry
                {
                    CustomId    = id,
                    BaseType    = i,
                    Kind        = DeviceKind.Server,
                    DisplayName = baseName + TierConfig.ServerNameSuffix,
                    Price       = 0, // resolved from the base shop item at shop-injection time
                    Color       = TierConfig.ServerTint,
                    IconColor   = TierConfig.ServerIconAccent,
                    SpeedGbps   = 0f,
                    TargetIops  = target,
                });

                MelonLogger.Msg($"DataCenterPlus: server '{baseName}' baseIOPS={baseIops} -> {target} (type {id})");
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

        private static void SetupSfp(MainGameManager mgm)
        {
            var sfps = mgm.sfpPrefabs;
            if (sfps == null || sfps.Length == 0) return;

            int best = -1;
            for (int i = 0; i < sfps.Length; i++) { if (sfps[i] != null) { best = i; break; } }
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
            int boxBest = -1;
            if (boxes != null)
                for (int i = 0; i < boxes.Length; i++) { if (boxes[i] != null) { boxBest = i; break; } }
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
                    TargetIops  = 0f,
                });
            }

            mgm.sfpsBoxedPrefab = ExtendWithTemplates(mgm, mgm.sfpsBoxedPrefab, DeviceKind.SfpBox);
        }

        // Copies the existing array and inserts inactive templates at each custom id
        // of the given kind (needed so save/load can resolve the prefab by type).
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
            }
            return null;
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
            {
                netSwitch.switchType = entry.CustomId;
                netSwitch.switchId   = entry.DisplayName;
            }

            if (entry.Kind == DeviceKind.Router)
            {
                var router = clone.AddComponent<Router>();
                router.routingTable = new Il2CppSystem.Collections.Generic.List<Router.SubnetRoute>();
                router.asn          = entry.CustomId;
                router.switchType   = entry.CustomId;
                router.switchId     = entry.DisplayName;
            }
            else if (entry.Kind == DeviceKind.Firewall)
            {
                var firewall = clone.AddComponent<Firewall>();
                firewall.filterRules = new Il2CppSystem.Collections.Generic.List<Firewall.FilterRule>();
                firewall.clusterIP   = "";
                firewall.switchType  = entry.CustomId;
                firewall.switchId    = entry.DisplayName;
            }

            var usable = clone.GetComponent<UsableObject>();
            if (usable != null) usable.prefabID = entry.CustomId;

            ApplyTint(clone, entry.Color);
            return clone;
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
            {
                srv.serverType = entry.CustomId;
                if (entry.TargetIops > 0f) srv.maxProcessingSpeed = entry.TargetIops;
            }

            var usable = clone.GetComponent<UsableObject>();
            if (usable != null) usable.prefabID = entry.CustomId;

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
            {
                sfp.speed   = entry.SpeedGbps;   // link bandwidth applied on insert
                sfp.sfpType = entry.CustomId;
            }

            var usable = clone.GetComponent<UsableObject>();
            if (usable != null) usable.prefabID = entry.CustomId;

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
            // modules. A tag marks the pack so we upgrade each module as it's taken out.
            var tag = clone.AddComponent<DcpPackTag>();
            tag.moduleId = entry.ModuleId;
            tag.speed    = entry.SpeedGbps;
            tag.sfpType  = entry.ModuleId;

            var usable = clone.GetComponent<UsableObject>();
            if (usable != null) usable.prefabID = entry.CustomId;

            ApplyTint(clone, entry.Color);
            return clone;
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
            if (mgm == null) yield break;
            var shop = mgm.computerShop;
            if (shop == null || shop.shopItems == null) yield break;

            // Discover base shop items. We only strictly need ONE template button to
            // clone; type-specific sources are used for nicer styling/price/sprite.
            ShopItem switchSource = null;
            ShopItem sfpSource = null;
            ShopItem anySource = null;
            var serverSources = new System.Collections.Generic.Dictionary<int, ShopItem>();

            foreach (var si in shop.shopItems)
            {
                if (si == null || si.shopItemSO == null) continue;
                if (anySource == null) anySource = si;
                var so = si.shopItemSO;
                int it = (int)so.itemType;
                if (it == (int)PlayerManager.ObjectInHand.Switch && so.itemID == BaseSwitchType)
                { switchSource = si; BaseSwitchSprite = so.sprite; }
                else if (IsServerType(so.itemType))
                { if (!serverSources.ContainsKey(so.itemID)) serverSources[so.itemID] = si; }
                else if ((it == (int)PlayerManager.ObjectInHand.SFPModule
                       || it == (int)PlayerManager.ObjectInHand.SFPBox) && sfpSource == null)
                { sfpSource = si; }
            }

            var parent = ResolveShopParent(shop);
            if (parent == null) yield break;

            // Guaranteed template: prefer the switch button, else any shop button.
            var template = switchSource ?? anySource;
            if (template == null) { MelonLogger.Warning("DataCenterPlus: no shop item to clone; aborting injection."); yield break; }
            var netSprite = switchSource != null ? switchSource.shopItemSO.sprite : template.shopItemSO.sprite;
            var sfpSprite = sfpSource != null ? sfpSource.shopItemSO.sprite : netSprite;

            int added = 0;

            // Networking (switch/router/firewall) — cloned from the switch/any button.
            foreach (var kv in DeviceRegistry.Entries)
            {
                var e = kv.Value;
                if (e.Kind != DeviceKind.Switch && e.Kind != DeviceKind.Router && e.Kind != DeviceKind.Firewall) continue;
                var oh = e.Kind == DeviceKind.Switch ? PlayerManager.ObjectInHand.Switch
                       : e.Kind == DeviceKind.Router ? PlayerManager.ObjectInHand.Router
                       : PlayerManager.ObjectInHand.Firewall;
                if (AddShopButton(template, parent, e, oh, netSprite) != null) added++;
            }

            // Servers — one HPC clone per base server, matched by base type id.
            foreach (var kv in serverSources)
            {
                int baseType = kv.Key;
                var src = kv.Value;
                if (!DeviceRegistry.TryGet(DeviceRegistry.SERVER_ID_BASE + baseType, out var e)) continue;
                e.Price = Mathf.RoundToInt(src.shopItemSO.price * TierConfig.ServerPriceMultiplier);
                if (AddShopButton(src, parent, e, src.shopItemSO.itemType, src.shopItemSO.sprite) != null) added++;
            }

            // SFP transceivers are sold as 5-packs (SFP boxes), exactly like vanilla.
            foreach (var kv in DeviceRegistry.Entries)
            {
                var e = kv.Value;
                if (e.Kind != DeviceKind.SfpBox) continue;
                if (AddShopButton(template, parent, e, PlayerManager.ObjectInHand.SFPBox, sfpSprite) != null) added++;
            }

            GrowShopContainer(parent, template, added);
            Canvas.ForceUpdateCanvases();
            MelonLogger.Msg($"DataCenterPlus: injected {added} shop item(s).");
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

        private static GameObject AddShopButton(ShopItem source, GameObject parent, DeviceRegistry.Entry entry,
                                                PlayerManager.ObjectInHand itemType, Sprite sprite)
        {
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

            var btnExt = cloned.GetComponent<ButtonExtended>();
            if (btnExt != null)
            {
                btnExt.doSubmitOnSelect = false;
                btnExt.selectOnPointerEnter = false;
                btnExt.functionToBeCalledOnSelect.RemoveAllListeners();
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
