# Data Center — Modding Reference

A practical, verified reference for modding the game **Data Center** (by Waseku, on Steam), assembled
from two community projects plus a decompile of the game's own assembly.

- **[Game API Reference](./GAME-API.md)** — auto-generated catalog of every moddable game type
  (fields, data members, methods / Harmony hook points, enum values).
- **[tools/dump-api](./tools/dump-api)** — the generator that produces `GAME-API.md`; re-run it after a
  game update.

The two upstream repos this is based on (cloned as siblings of this folder):

| Repo | What it is |
| --- | --- |
| `Data-Center-Modding-APIs` | A C# **SDK / wrapper library** that gives friendly, null-safe helpers (`PlayerApi`, `NetworkApi`, `UiApi`, `TimeApi`, `LocalisationApi`, `WorldApi`) over the raw game types. |
| `DC-NetworkingPlus-Mod` | A **complete working mod** (adds custom Router/Firewall devices). Bundles all reference DLLs in `lib/`, so it builds with no game install. |

---

## 1. How Data Center modding works

Data Center is a **Unity IL2CPP** game. That one fact drives everything:

- **IL2CPP** compiles the game's C# to C++, so there is no managed `Assembly-CSharp.dll` to reference
  directly. **MelonLoader** + **Il2CppInterop** regenerate *managed proxy assemblies* at install time.
  Those proxies put every game type under the **`Il2Cpp`** namespace — hence `using Il2Cpp;` in mod code.
- **MelonLoader** is the mod loader. A mod is a class that derives from `MelonMod` and is dropped into
  `Data Center/Mods/` as a `.dll`.
- **Harmony** (`0Harmony.dll`) is used to **patch** game methods at runtime (prefix / postfix / transpiler),
  since you can't edit the game's compiled code.

### IL2CPP interop gotchas (important)

These are the things that trip up every new Data Center modder:

- **Game "fields" become C# properties.** In `GAME-API.md` these are listed under *Data members*
  (e.g. `Player.money`, `Server.switchType`). You read/write them like fields: `player.money`.
- **Collections are `Il2CppSystem.*`, not `System.*`.** You must construct e.g.
  `new Il2CppSystem.Collections.Generic.List<Router.SubnetRoute>()`, not a plain `System.Collections.Generic.List`.
- **Arrays are `Il2CppStructArray<T>` / `Il2CppReferenceArray<T>`.** They behave like arrays but are their
  own types; `GAME-API.md` shows them verbatim so you know what to expect.
- **Lambdas don't bind to IL2CPP `UnityAction`.** For UI button callbacks you need a real injected
  `MonoBehaviour` (register it with `ClassInjector.RegisterTypeInIl2Cpp<T>()`). NetworkingPlus does this
  with its `CartButtonHandler`.
- **Custom components must be injected** before `AddComponent<T>()` works
  (`ClassInjector.RegisterTypeInIl2Cpp<T>()` in `OnInitializeMelon`).

---

## 2. Environment & building

### What's already set up here (Linux)
- **.NET 6 SDK** installed at `~/.dotnet` (matches the upstream CI).
- `DC-NetworkingPlus-Mod` builds out of the box because it bundles its reference DLLs:
  ```bash
  cd DC-NetworkingPlus-Mod
  dotnet build DataCenter-NetworkingPlus/NetworkingPlus.csproj -c Release
  # -> DataCenter-NetworkingPlus/bin/Release/net6.0/NetworkingPlus.dll
  ```

### On your Steam machine (where you actually play)
1. Install **MelonLoader** for Data Center (run it once so Il2CppInterop generates the managed proxies
   under `Data Center/MelonLoader/Il2CppAssemblies/` and `.../net6/`).
2. The **SDK** (`Data-Center-Modding-APIs`) references those generated DLLs via the `GameRoot` property.
   Default path in the `.csproj` files is `D:\Juegos\steamapps\common\Data Center` — **change `GameRoot`
   in both `.csproj` files** if your install path differs.
3. Build, then copy the resulting `.dll`(s) into `Data Center/Mods/`. (Both upstream projects auto-copy
   when the game path exists locally.)

### Where the reference DLLs come from
`DC-NetworkingPlus-Mod/lib/` contains a working snapshot: `MelonLoader.dll`, `0Harmony.dll`,
`Il2CppInterop.Runtime.dll`, `Il2Cppmscorlib.dll`, `Assembly-CSharp.dll` (the game), plus Unity modules.
This folder is what makes offline builds — and this whole API reference — possible.

---

## 3. Writing a mod — the anatomy (worked example: NetworkingPlus)

A minimal MelonMod:

```csharp
using Il2Cpp;
using MelonLoader;

[assembly: MelonInfo(typeof(MyMod.Core), "MyMod", "1.0.0", "you")]
[assembly: MelonGame("Waseku", "Data Center")]   // must match the game

namespace MyMod
{
    public class Core : MelonMod
    {
        public override void OnInitializeMelon() { /* register injected types here */ }
        public override void OnSceneWasLoaded(int buildIndex, string sceneName) { /* per-scene setup */ }
    }
}
```

### Harmony patching pattern
NetworkingPlus hooks the shop + spawn pipeline. Each patch maps 1:1 to a method in `GAME-API.md`:

| Patch target (from `GAME-API.md`) | Why |
| --- | --- |
| `MainGameManager.Awake` / `.Start` (postfix) | Build & register custom device prefabs into `routersPrefabs[]` / `firewallsPrefabs[]`. |
| `ComputerShop.SpawnPhysicalItem(GameObject prefab, int price, ObjectInHand itemType)` (prefix, `ref prefab`) | Swap the vanilla prefab for the custom device at spawn time. |
| `ComputerShop.ButtonBuyShopItem(int itemID, int price, ObjectInHand itemType, string displayName, bool isCustomColor)` (prefix) | Implement the full custom buy flow (spawn + cart + money). |

```csharp
[HarmonyPatch(typeof(ComputerShop), nameof(ComputerShop.SpawnPhysicalItem))]
internal static class PatchSpawn
{
    // return false to skip the original; use ref to replace an argument
    static bool Prefix(ref GameObject prefab, int price, PlayerManager.ObjectInHand itemType)
    {
        // ...inspect/replace prefab...
        return true; // let original run with our modified prefab
    }
}
```

Key enum values you'll use (from `GAME-API.md` → Enums): `PlayerManager.ObjectInHand.Router = 11`,
`Firewall = 12`, `Switch = 4`.

---

## 4. The SDK wrappers → game API map

The `Data-Center-Modding-APIs` SDK is a thin, null-safe layer. Use it to avoid boilerplate; drop to the
raw types (`GetRaw()`) when you need something it doesn't cover. What each wrapper actually touches:

| SDK method | Raw game call (see `GAME-API.md`) |
| --- | --- |
| `PlayerApi.GetMoney/Xp/Reputation` | `Player.money` / `.xp` / `.reputation` |
| `PlayerApi.TryAddMoney` | `Player.UpdateCoin(amount, withoutSound)` |
| `PlayerApi.TryAddXp` / `TryAddReputation` | `Player.UpdateXP(amount)` / `Player.UpdateReputation(amount)` |
| `NetworkApi.Get*Snapshot` | `NetworkMap.servers` / `.switches` / `.brokenServers` / `.brokenSwitches` |
| `NetworkApi.TryBreakServer/Switch` | `Server.ItIsBroken()` / `NetworkSwitch.ItIsBroken()` |
| `NetworkApi.TryRepair*` | `*.RepairDevice()`, `.ClearWarningSign()`, `.ClearErrorSign()`, `.PowerButton(true)` |
| `UiApi.TryNotify` / `TryAddMessage` | `StaticUIElements.SetNotification(...)` / `.AddMeesageInField(...)` |
| `TimeApi.*` | `TimeController.day`, `.timeMultiplier`, `.CurrentTimeInHours()`, `.TimeIsBetween(a,b)` |
| `LocalisationApi.*` | `Localisation.ReturnTextByID(uid)`, `.ChangeLocalisation(uid)` |
| `WorldApi.*` | `Object.FindObjectsOfType<ComputerShop>()`, `ComputerShop.networkMapScreen` |

### ⚠️ Known version drift (verified)
The SDK was written against an **older** game build than the one bundled in `DC-NetworkingPlus-Mod/lib/`.
It does **not** compile against the current assembly because the network collections changed type:

- SDK assumes `NetworkMap.servers` / `switches` are `Dictionary<_, Server>` / `Dictionary<_, NetworkSwitch>`.
- Current game exposes them as **`INetworkEndpoint`** values (`Server` now implements `INetworkEndpoint`),
  and adds richer helpers like `NetworkMap.GetAllServers()`, `.GetAllRouters()`, `.GetAllFirewalls()`,
  `.RegisterEndpoint(...)`.

So `NetworkApi.GetServersSnapshot()` (which does `List<Server>.Add(kv.Value)`) fails with
`cannot convert from INetworkEndpoint to Server`. If you adopt the SDK, this is the first thing to fix
(cast/filter `INetworkEndpoint` to `Server`, or switch to the new `GetAll*` helpers).

---

## 5. Moddable subsystems at a glance

Jump into the matching section of **[GAME-API.md](./GAME-API.md)** for full member lists.

- **Economy / progression** — `Player`, `PlayerData`, `PlayerManager`.
- **Network graph** — `NetworkMap` (routing, paths, VLAN/subnet pools, LACP), `INetworkEndpoint`, `ITimedDevice`.
- **Devices** — `Server`, `NetworkSwitch`, `Router`, `Firewall`, `Rack`, `RackMount`, `CableLink`.
- **Device config UIs** — `RouterConfiguration`, `FirewallConfiguration`, `NetworkSwitchConfiguration`
  (plus `RouterRouteRow`, `FirewallRuleRow`).
- **Shop & items** — `ComputerShop`, `ShopItem`, `ShopItemSO`, `ShopCartItem`, `ModShopItem` (the "HL Mods"
  shop section is the intended place to inject custom buyables).
- **UI / localisation** — `StaticUIElements`, `Localisation` (19 languages, see enum).
- **Time** — `TimeController` (`timeMultiplier` for fast-forward, day/hour helpers).
- **Persistence** — `SaveSystem`, `SaveData`, and the `*SaveData` payloads (`ServerSaveData`,
  `SwitchSaveData`, `RouterSaveData`, `FirewallSaveData`, `CableSaveData`, …). Custom devices need inactive
  template prefabs registered so save/load can find them — see NetworkingPlus `SetupRegistry`.

---

## 6. Regenerating the API reference

The game updates; so does its API. To refresh `GAME-API.md` against a newer assembly:

```bash
cd modding-reference/tools/dump-api
# Point at the game's generated proxy on your Steam machine, or the bundled lib copy:
dotnet run -c Release -- "/path/to/Data Center/MelonLoader/Il2CppAssemblies/Assembly-CSharp.dll"
```

The dumper uses **Mono.Cecil** (lazy resolution), so it works even without all the referenced Il2Cpp
assemblies present. It emits detailed sections for core subsystems, all enum values, save-data structures,
and a catalog of the remaining ~175 game types.
