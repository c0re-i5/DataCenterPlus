# `lib/` — reference assemblies (not committed)

These DLLs are **not** included in the repository because they are the game's proprietary
code and the Unity engine / MelonLoader runtime. Copy them here locally before building.

Required files:

| File | Source (from a MelonLoader‑patched Data Center install) |
| --- | --- |
| `MelonLoader.dll` | `<GameDir>\MelonLoader\net6\` |
| `Il2CppInterop.Runtime.dll` | `<GameDir>\MelonLoader\net6\` |
| `0Harmony.dll` | `<GameDir>\MelonLoader\net6\` |
| `Assembly-CSharp.dll` | `<GameDir>\MelonLoader\Il2CppAssemblies\` |
| `Il2Cppmscorlib.dll` | `<GameDir>\MelonLoader\Il2CppAssemblies\` |
| `UnityEngine.dll` | `<GameDir>\MelonLoader\Il2CppAssemblies\` |
| `UnityEngine.CoreModule.dll` | `<GameDir>\MelonLoader\Il2CppAssemblies\` |
| `UnityEngine.UI.dll` | `<GameDir>\MelonLoader\Il2CppAssemblies\` |
| `UnityEngine.UIModule.dll` | `<GameDir>\MelonLoader\Il2CppAssemblies\` |
| `Unity.TextMeshPro.dll` | `<GameDir>\MelonLoader\Il2CppAssemblies\` |

`<GameDir>` is your Steam install, e.g. `…\steamapps\common\Data Center`. Run the game once
after installing MelonLoader so the `Il2CppAssemblies` are generated.

Once these are in place, build with:

```bash
dotnet build DataCenterPlus/DataCenterPlus.csproj -c Release
```
