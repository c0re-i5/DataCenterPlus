using System;
using HarmonyLib;
using Il2Cpp;
using Il2CppInterop.Runtime;
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

    // Injected helper for cart +/- buttons (IL2CPP UnityAction can't bind lambdas).
    public class CartButtonHandler : MonoBehaviour
    {
        public CartButtonHandler(IntPtr ptr) : base(ptr) { }

        internal ShopCartItem cart;
        internal ComputerShop shop;
        internal bool isAdd;
        internal int itemID;
        internal PlayerManager.ObjectInHand itemType;

        private GameObject FindPrefab()
        {
            var mgm = MainGameManager.instance;
            if (mgm == null) return null;
            if (!DeviceRegistry.TryGet(itemID, out var entry)) return null;
            return Core.BuildPrefab(mgm, entry);
        }

        private int FindFreeSpawnPoint()
        {
            var spawns = shop.transformProductItemsSpawns;
            if (spawns == null || spawns.Length == 0) return -1;
            for (int i = 0; i < spawns.Length; i++)
                if (shop.itemsSpawnsInUse == null || i >= shop.itemsSpawnsInUse.Length || shop.itemsSpawnsInUse[i] == 0)
                    return i;
            return -1;
        }

        private int SpawnAt(int spawnIndex)
        {
            var prefab = FindPrefab();
            if (prefab == null) return -1;
            var spawns = shop.transformProductItemsSpawns;
            var pos = spawns[spawnIndex];
            var obj = Object.Instantiate(prefab, pos.position, pos.rotation);
            Object.Destroy(prefab);
            if (shop.itemsSpawnsInUse != null && spawnIndex < shop.itemsSpawnsInUse.Length)
                shop.itemsSpawnsInUse[spawnIndex] = 1;
            int uid = shop.uniqueID++;
            if (shop.spawnedItems != null) shop.spawnedItems[uid] = obj;
            if (shop.spawnedItemPositions != null) shop.spawnedItemPositions[uid] = spawnIndex;
            return uid;
        }

        public void HandleClick()
        {
            if (cart == null || shop == null || cart.spawnedItemUIDs == null) return;

            if (isAdd)
            {
                if (cart.spawnedItemUIDs.Count >= 99) return;
                int spawnIdx = FindFreeSpawnPoint();
                if (spawnIdx < 0) return;
                int uid = SpawnAt(spawnIdx);
                if (uid < 0) return;
                cart.spawnedItemUIDs.Add(uid);
                cart.UpdateDisplay();
                shop.UpdateCartTotal();
            }
            else
            {
                int lastUid = cart.spawnedItemUIDs[cart.spawnedItemUIDs.Count - 1];
                if (shop.spawnedItems != null && shop.spawnedItems.ContainsKey(lastUid))
                {
                    var obj = shop.spawnedItems[lastUid];
                    if (obj != null) Object.Destroy(obj);
                    shop.spawnedItems.Remove(lastUid);
                }
                if (shop.spawnedItemPositions != null && shop.spawnedItemPositions.ContainsKey(lastUid))
                {
                    int posIdx = shop.spawnedItemPositions[lastUid];
                    if (shop.itemsSpawnsInUse != null && posIdx < shop.itemsSpawnsInUse.Length)
                        shop.itemsSpawnsInUse[posIdx] = 0;
                    shop.spawnedItemPositions.Remove(lastUid);
                }

                if (cart.spawnedItemUIDs.Count <= 1)
                {
                    if (shop.cartUIItems != null) shop.cartUIItems.Remove(cart);
                    shop.UpdateCartTotal();
                    Object.Destroy(cart.gameObject);
                    return;
                }

                cart.spawnedItemUIDs.RemoveAt(cart.spawnedItemUIDs.Count - 1);
                cart.UpdateDisplay();
                shop.UpdateCartTotal();
            }
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

    // -------------------------------------------------------------- SPAWN (save/load)
    [HarmonyPatch(typeof(ComputerShop), nameof(ComputerShop.SpawnPhysicalItem))]
    internal static class PatchSpawnPhysicalItem
    {
        private static bool Prefix(ref GameObject prefab, int price, PlayerManager.ObjectInHand itemType)
        {
            if (prefab == null) return true;
            var mgm = MainGameManager.instance;
            if (mgm == null) return true;

            if (TrySwap(mgm.routersPrefabs,   DeviceRegistry.ROUTER_ID_BASE,   ref prefab, mgm)) return true;
            if (TrySwap(mgm.firewallsPrefabs, DeviceRegistry.FIREWALL_ID_BASE, ref prefab, mgm)) return true;
            if (TrySwap(mgm.switchesPrefabs,  DeviceRegistry.SWITCH_ID_BASE,   ref prefab, mgm)) return true;
            if (TrySwap(mgm.serverPrefabs,    DeviceRegistry.SERVER_ID_BASE,   ref prefab, mgm)) return true;
            if (TrySwap(mgm.sfpPrefabs,       DeviceRegistry.SFP_ID_BASE,      ref prefab, mgm)) return true;
            if (TrySwap(mgm.sfpsBoxedPrefab,  DeviceRegistry.SFPBOX_ID_BASE,   ref prefab, mgm)) return true;
            return true;
        }

        private static bool TrySwap(Il2CppInterop.Runtime.InteropTypes.Arrays.Il2CppReferenceArray<GameObject> arr,
                                    int idBase, ref GameObject prefab, MainGameManager mgm)
        {
            if (arr == null) return false;
            for (int i = 0; i < arr.Length; i++)
            {
                if (arr[i] != prefab) continue;
                if (DeviceRegistry.TryGet(i, out var e))
                {
                    var c = Core.BuildPrefab(mgm, e);
                    if (c != null) { prefab = c; }
                }
                return true;
            }
            return false;
        }
    }

    // ------------------------------------------------------------------- CUSTOM BUY
    [HarmonyPatch(typeof(ComputerShop), nameof(ComputerShop.ButtonBuyShopItem))]
    internal static class PatchButtonBuyShopItem
    {
        private static bool Prefix(ComputerShop __instance, int itemID, int price,
                                   PlayerManager.ObjectInHand itemType, string displayName, bool isCustomColor)
        {
            if (!DeviceRegistry.TryGet(itemID, out var entry)) return true;

            var mgm = MainGameManager.instance;
            if (mgm == null) return true;
            var prefab = Core.BuildPrefab(mgm, entry);
            if (prefab == null) return true;

            var spawns = __instance.transformProductItemsSpawns;
            if (spawns == null || spawns.Length == 0) { Object.Destroy(prefab); return false; }

            int idx = -1;
            for (int i = 0; i < spawns.Length; i++)
                if (__instance.itemsSpawnsInUse == null || i >= __instance.itemsSpawnsInUse.Length || __instance.itemsSpawnsInUse[i] == 0)
                { idx = i; break; }
            if (idx < 0) { Object.Destroy(prefab); return false; }

            var pos = spawns[idx];
            var obj = Object.Instantiate(prefab, pos.position, pos.rotation);
            Object.Destroy(prefab);

            if (__instance.itemsSpawnsInUse != null && idx < __instance.itemsSpawnsInUse.Length)
                __instance.itemsSpawnsInUse[idx] = 1;

            int uid = __instance.uniqueID++;
            if (__instance.spawnedItems != null) __instance.spawnedItems[uid] = obj;
            if (__instance.spawnedItemPositions != null) __instance.spawnedItemPositions[uid] = idx;

            // Already in cart? stack it.
            if (__instance.cartUIItems != null)
            {
                for (int i = 0; i < __instance.cartUIItems.Count; i++)
                {
                    var ci = __instance.cartUIItems[i];
                    if (ci != null && ci.itemID == itemID && ci.itemType == itemType)
                    {
                        ci.spawnedItemUIDs.Add(uid);
                        ci.UpdateDisplay();
                        __instance.UpdateCartTotal();
                        return false;
                    }
                }
            }

            if (__instance.shopCartItemPrefab != null && __instance.parentForShopCartItems != null)
            {
                var cartGo = Object.Instantiate(__instance.shopCartItemPrefab, __instance.parentForShopCartItems);
                if (cartGo != null)
                {
                    var cart = cartGo.GetComponent<ShopCartItem>();
                    if (cart != null)
                    {
                        cart.shop = __instance;
                        cart.itemID = itemID;
                        cart.price = price;
                        cart.itemType = itemType;
                        cart.itemName = entry.DisplayName;
                        cart.spawnedItemUIDs = new Il2CppSystem.Collections.Generic.List<int>();
                        cart.spawnedItemUIDs.Add(uid);

                        if (__instance.cartUIItems == null)
                            __instance.cartUIItems = new Il2CppSystem.Collections.Generic.List<ShopCartItem>();
                        __instance.cartUIItems.Add(cart);

                        WireCartButton(cartGo, cart, __instance, true,  itemID, itemType, cart.btnAdd);
                        WireCartButton(cartGo, cart, __instance, false, itemID, itemType, cart.btnRemove);

                        cart.UpdateDisplay();
                    }
                }
            }

            __instance.UpdateCartTotal();
            return false;
        }

        private static void WireCartButton(GameObject cartGo, ShopCartItem cart, ComputerShop shop,
                                           bool isAdd, int itemID, PlayerManager.ObjectInHand itemType,
                                           UnityEngine.UI.ButtonExtended btn)
        {
            if (btn == null) return;
            btn.onClick.RemoveAllListeners();
            var h = cartGo.AddComponent<CartButtonHandler>();
            h.cart = cart; h.shop = shop; h.isAdd = isAdd; h.itemID = itemID; h.itemType = itemType;
            btn.m_OnClick.AddListener(DelegateSupport.ConvertDelegate<UnityEngine.Events.UnityAction>(h.HandleClick));
        }
    }

    [HarmonyPatch(typeof(ShopCartItem), nameof(ShopCartItem.UpdateDisplay))]
    internal static class PatchUpdateDisplay
    {
        private static void Postfix(ShopCartItem __instance)
        {
            if (!DeviceRegistry.TryGet(__instance.itemID, out var entry)) return;
            int qty = __instance.spawnedItemUIDs != null ? __instance.spawnedItemUIDs.Count : 1;
            if (__instance.txtItemName != null) __instance.txtItemName.text = entry.DisplayName;
            if (__instance.txtAmount != null) __instance.txtAmount.text = qty.ToString();
            if (__instance.txtPrice != null) __instance.txtPrice.text = $"{__instance.price * qty} $";
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
                if (router != null) { mgm.ShowRouterConfigCanvas(router); return false; }
            }
            else if (entry.Kind == DeviceKind.Firewall)
            {
                var firewall = __instance.GetComponent<Firewall>();
                if (firewall != null) { mgm.ShowFirewallConfigCanvas(firewall); return false; }
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
            __instance.switchId = entry.DisplayName;
            if (__instance.txtScreen != null) __instance.txtScreen.text = entry.DisplayName;
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
        private static void Postfix(Server __instance, ServerSaveData serverSaveData) => BoostServer.Apply(__instance);
    }

    internal static class BoostServer
    {
        internal static void Apply(Server srv)
        {
            if (srv == null) return;
            int id = srv.serverType;
            if (!DeviceRegistry.TryGet(id, out var e) || e.Kind != DeviceKind.Server) return;
            if (e.TargetIops > 0f && !Mathf.Approximately(srv.maxProcessingSpeed, e.TargetIops))
                srv.maxProcessingSpeed = e.TargetIops;
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
            if (e.SpeedGbps > 0f) __instance.SetConnectionSpeed(e.SpeedGbps);
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
            __result.speed   = tag.speed;
            __result.sfpType = tag.sfpType;
            var usable = __result.GetComponent<UsableObject>();
            if (usable != null) usable.prefabID = tag.moduleId;
        }
    }

    // ---------------------------------------------------------- SAVE/LOAD PREFAB LOOKUP
    [HarmonyPatch(typeof(MainGameManager), nameof(MainGameManager.GetServerPrefab))]
    internal static class PatchGetServerPrefab
    {
        private static void Postfix(MainGameManager __instance, int serverType, ref GameObject __result)
        {
            if (__result == null && DeviceRegistry.IsCustom(serverType)
                && __instance.serverPrefabs != null && serverType < __instance.serverPrefabs.Length)
                __result = __instance.serverPrefabs[serverType];
        }
    }

    [HarmonyPatch(typeof(MainGameManager), nameof(MainGameManager.GetSwitchPrefab))]
    internal static class PatchGetSwitchPrefab
    {
        private static void Postfix(MainGameManager __instance, int switchType, ref GameObject __result)
        {
            if (__result == null && DeviceRegistry.IsCustom(switchType)
                && __instance.switchesPrefabs != null && switchType < __instance.switchesPrefabs.Length)
                __result = __instance.switchesPrefabs[switchType];
        }
    }

    [HarmonyPatch(typeof(MainGameManager), nameof(MainGameManager.GetSfpPrefab))]
    internal static class PatchGetSfpPrefab
    {
        private static void Postfix(MainGameManager __instance, int prefabID, ref GameObject __result)
        {
            if (__result == null && DeviceRegistry.IsCustom(prefabID)
                && __instance.sfpPrefabs != null && prefabID < __instance.sfpPrefabs.Length)
                __result = __instance.sfpPrefabs[prefabID];
        }
    }

    [HarmonyPatch(typeof(MainGameManager), nameof(MainGameManager.GetSfpBoxPrefab))]
    internal static class PatchGetSfpBoxPrefab
    {
        private static void Postfix(MainGameManager __instance, int prefabID, ref GameObject __result)
        {
            if (__result == null && DeviceRegistry.IsCustom(prefabID)
                && __instance.sfpsBoxedPrefab != null && prefabID < __instance.sfpsBoxedPrefab.Length)
                __result = __instance.sfpsBoxedPrefab[prefabID];
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
}
