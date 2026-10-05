# Installing & Testing DataCenterPlus

A step‑by‑step guide to install the mod with **MelonLoader** and verify each feature in‑game.

> **Which mod system is this?** DataCenterPlus is a **MelonLoader** mod (external loader + Harmony
> patching). It is installed **manually** by dropping a `.dll` into the game's `Mods/` folder — it is
> **not** a Steam Workshop item. The Steam Workshop serves the game's *native* ModLoader content packs,
> which are a different system. See the project [README](./README.md) for that distinction.

---

## 0. Prerequisites

- **Data Center** installed via Steam (Windows).
- The built **`DataCenterPlus.dll`** (from `DataCenterPlus/bin/Release/net6.0/`, or build it — see §6).
- ~10 minutes for first‑time MelonLoader setup.

Find your game folder: in Steam, right‑click **Data Center → Manage → Browse local files**. That folder
(e.g. `…\steamapps\common\Data Center`) is referred to below as **`<GameDir>`**.

---

## 1. Install MelonLoader

1. Download the **MelonLoader** installer from <https://melonwiki.xyz/> (the automated installer).
2. Run it, click **Select** and point it at **`<GameDir>\Data Center.exe`**.
3. Choose a MelonLoader version with **IL2CPP + .NET 6** support (the latest stable is fine), and install.

MelonLoader adds a `MelonLoader\` folder, a `version.dll`/`dobby` shim, and an empty **`Mods\`** folder
inside `<GameDir>`.

## 2. First launch (generates the IL2CPP assemblies)

1. Launch the game normally from Steam **once**.
2. A **MelonLoader console window** opens alongside the game. On first run it **generates the IL2CPP
   proxy assemblies** (`<GameDir>\MelonLoader\Il2CppAssemblies\`). This can take a few minutes — let it
   finish and reach the main menu, then quit.

> If the game is set to launch without a console, you can still read logs later in
> `<GameDir>\MelonLoader\Latest.log`.

## 3. Install the mod

Copy **`DataCenterPlus.dll`** into **`<GameDir>\Mods\`**.

That's the only file you need — MelonLoader, Harmony and the Il2Cpp interop assemblies are provided by
MelonLoader at runtime (the mod references them with `Private=false`, so they are **not** shipped).

## 4. Launch & verify it loaded

Launch the game again and watch the MelonLoader console (or `Latest.log`). At startup MelonLoader lists
each loaded mod with its info — look for **`DataCenterPlus v1.0.0`** (by `brzb0 + contributors`). Then,
once a game scene loads, the mod prints its own lines (each shown with a `[DataCenterPlus]` prefix):

```
[DataCenterPlus] DataCenterPlus: base switch = '…' (type N)
[DataCenterPlus] DataCenterPlus: server '…' baseIOPS=5000 -> 25000 (type 1000)
[DataCenterPlus] DataCenterPlus: server '…' baseIOPS=12000 -> 60000 (type 1001)
…
[DataCenterPlus] DataCenterPlus: base SFP = '…' (type N)
[DataCenterPlus] DataCenterPlus: base SFP box = '…' (type N)
[DataCenterPlus] DataCenterPlus: injected 16 shop item(s).
```

**What the lines tell you**
- `base switch / base SFP / base SFP box` — the mod found the vanilla prefabs it clones. If any says it
  couldn't be found, that category won't appear (see Troubleshooting).
- `server '…' baseIOPS=X -> Y` — confirms the IOPS boost per server. If `baseIOPS=0`, the game computes
  IOPS at runtime; switch to absolute values (see §5 / Troubleshooting).
- `injected N shop item(s)` — how many buttons were added. Expect roughly **14–16**: 6 networking
  (switch/router/firewall × 2 tiers) + one HPC server **per base server** (8 if there are 4 colours ×
  2 sizes) + 2 transceiver 5‑packs.

---

## 5. In‑game testing checklist

Open the in‑game computer shop and find the **HL Mods** section (if that section doesn't exist, items
are appended to the bottom of the main shop list instead).

**Networking tiers**
- [ ] `Switch / Router / Firewall QSFP28 100G` and `… QSFP-DD 400G` are listed (teal = 100 G, gold = 400 G).
- [ ] Buy one of each, place in a rack.
- [ ] Clicking a **Router** opens the router config UI; a **Firewall** opens the firewall config UI.
- [ ] Rack label shows the tier name.

**Higher‑IOPS servers**
- [ ] An HPC variant exists for each base server (name ends with ` HPC`, warm‑tinted shop icon).
- [ ] Buy one, insert into a rack, and check its IOPS: a 3U‑class should read **25,000** and a 7U‑class
      **60,000** (i.e. 5× the base). Compare against a vanilla server of the same colour.

**Transceivers (5‑packs)**
- [ ] `QSFP28 Transceiver 100G (5-pack)` and `QSFP-DD … 400G (5-pack)` are listed.
- [ ] Buy a pack, **open the box, take a module out**, and insert it into a cable's SFP port.
- [ ] The link reports the expected **100 / 400 Gbps**.

**Persistence**
- [ ] Save, quit to menu, reload: custom servers/switches keep their type, names and stats; inserted
      transceivers keep their speed. *(Known edge case: an **unused** pack left in the world may revert
      to base speed after reload — modules already taken out/inserted persist fine.)*

---

## 6. Build from source (optional)

Requires the **.NET 6 SDK**. Reference DLLs are bundled in `lib/`, so no game install is needed to compile:

```bash
dotnet build DataCenterPlus/DataCenterPlus.csproj -c Release
# -> DataCenterPlus/bin/Release/net6.0/DataCenterPlus.dll
```

To auto‑copy the DLL into your game's `Mods\` on every build, set `<GameDir>` in
`DataCenterPlus/DataCenterPlus.csproj` to your install path.

---

## 7. Tuning

All balance/visual values live in [`DataCenterPlus/TierConfig.cs`](./DataCenterPlus/TierConfig.cs)
(IOPS multiplier, tier speeds/prices/colours, icon accents). Edit → rebuild → replace the DLL in `Mods\`.

---

## 8. Troubleshooting

| Symptom | Fix |
|---|---|
| No `[DataCenterPlus]` line in the console | DLL isn't in `<GameDir>\Mods\`, or MelonLoader didn't finish first‑run asset generation. Confirm the console/`Latest.log` shows MelonLoader starting. |
| `MelonLoader: unsupported / no assemblies` | Reinstall MelonLoader choosing the **IL2CPP + .NET 6** build. |
| `injected 0 shop item(s)` or `no shop item to clone` | The shop wasn't ready; the mod waits 1.5 s after a scene load. If it persists, the shop layout changed — capture the log and we'll adjust discovery. |
| HPC servers show `baseIOPS=0` / wrong IOPS | The game computes IOPS at runtime. In `TierConfig.cs` set `ServerUseAbsoluteIops = true` and adjust `ServerAbsoluteIopsSmall/Large`; rebuild. |
| Transceiver link speed not applied | Verify the module came from a **custom pack** (taken out after purchase). Unused packs after a reload are the known caveat above. |
| Mod broke after a **game update** | Steam updated the game. Reinstall MelonLoader if needed. If APIs changed, rebuild against the new `Assembly-CSharp.dll` (regenerate the API reference with `modding-reference/tools/dump-api`). |
| Conflicts with **NetworkingPlus** | Both can coexist (distinct type‑ID ranges). If issues arise, test with one at a time to isolate. |

---

## 9. Uninstall

Delete `DataCenterPlus.dll` from `<GameDir>\Mods\`. To remove MelonLoader entirely, re‑run its installer
and choose **Un‑Install**, or delete the `MelonLoader\` folder and the `version.dll` shim.

> Removing the mod from a save that placed custom devices may leave those items unresolved on next load.
> Prefer selling/removing custom devices in‑game before uninstalling.
