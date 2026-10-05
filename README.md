# Data Center — Modding

Modding work for the Steam game **[Data Center](https://store.steampowered.com/app/4170200/Data_Center/)**
(Unity IL2CPP + MelonLoader). This repository contains two things:

## 📦 [`DataCenterPlus/`](./DataCenterPlus) — the mod
A MelonLoader mod that adds:
- **Higher‑bandwidth networking tiers** — 100 GbE (QSFP28) and 400 GbE (QSFP‑DD) **Switch / Router /
  Firewall**, plus matching **transceiver 5‑packs** that actually set link bandwidth.
- **Higher‑IOPS "HPC" servers** — a boosted variant of every base server, tuned for late‑game balance.

See **[DataCenterPlus/README.md](./DataCenterPlus/README.md)** for features and
**[DataCenterPlus/INSTALL.md](./DataCenterPlus/INSTALL.md)** for install + testing.

## 📚 [`modding-reference/`](./modding-reference) — docs & tooling
- **[GAME-API.md](./modding-reference/GAME-API.md)** — a generated reference of the game's moddable
  type surface (classes, data members, methods / Harmony hook points, enum values).
- **[tools/dump-api](./modding-reference/tools/dump-api)** — the Mono.Cecil generator that produces it;
  re‑run after a game update.

## Building

Requires the **.NET 6 SDK**. The reference assemblies under `DataCenterPlus/lib/` are **not committed**
(proprietary game/engine binaries) — see [DataCenterPlus/lib/README.md](./DataCenterPlus/lib/README.md)
for how to supply them, then:

```bash
dotnet build DataCenterPlus/DataCenterPlus.csproj -c Release
```

## Credits & upstream

Built on the approach of [DC‑NetworkingPlus‑Mod](https://github.com/brzb0/DC-NetworkingPlus-Mod) and
[Data‑Center‑Modding‑APIs](https://github.com/brzb0/Data-Center-Modding-APIs) by **brzb0**. The
`GAME-API.md` reference is community‑generated from the game's interop assembly for interoperability /
modding purposes.
