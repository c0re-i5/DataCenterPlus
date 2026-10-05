using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mono.Cecil;

class Dump {
  static ModuleDefinition mod;
  static StringBuilder sb = new();
  static void W(string s="") => sb.AppendLine(s);

  static string TN(TypeReference t) {
    if (t == null) return "?";
    if (t is ArrayType at) return TN(at.ElementType) + "[]";
    if (t is ByReferenceType br) return "ref " + TN(br.ElementType);
    if (t is GenericInstanceType gi) {
      var baseName = gi.ElementType.Name;
      int tick = baseName.IndexOf('`'); if (tick>=0) baseName = baseName.Substring(0,tick);
      return baseName + "<" + string.Join(", ", gi.GenericArguments.Select(TN)) + ">";
    }
    var n = t.Name;
    if ((n=="Il2CppStructArray`1"||n=="Il2CppReferenceArray`1") && t is GenericInstanceType) {}
    return n;
  }

  static string Vis(MethodDefinition m) =>
    m.IsPublic ? "public" : m.IsFamily ? "protected" : m.IsAssembly ? "internal" : m.IsFamilyOrAssembly ? "protected internal" : "private";
  static string Vis(FieldDefinition f) =>
    f.IsPublic ? "public" : f.IsFamily ? "protected" : f.IsAssembly ? "internal" : "private";

  static bool Generated(string n) => n.Contains("<") || n.StartsWith("__") || n.Contains("k__") || n.Contains("$") || n.Contains("_b__") || n.Contains("_g__") || n.Contains("|");

  static void DumpType(TypeDefinition t, bool full) {
    string kind = t.IsInterface ? "interface" : t.IsEnum ? "enum" : t.IsValueType ? "struct" : "class";
    string disp = t.DeclaringType!=null ? t.DeclaringType.Name + "." + t.Name : t.Name;
    W($"### `{disp}` ({kind})");
    var bases = new List<string>();
    if (t.BaseType != null && t.BaseType.Name != "Object" && t.BaseType.Name != "ValueType" && t.BaseType.Name != "Enum")
      bases.Add(TN(t.BaseType));
    foreach (var i in t.Interfaces) {
      var iname = TN(i.InterfaceType);
      if (iname.StartsWith("I") && !iname.StartsWith("Il2Cpp") && iname.Length>1) bases.Add(iname);
    }
    if (bases.Count>0) W($"*: {string.Join(", ", bases.Distinct())}*");
    W();

    if (t.IsEnum) {
      W("| value | name |"); W("|---|---|");
      foreach (var f in t.Fields.Where(f=>f.IsStatic && f.HasConstant))
        W($"| {f.Constant} | `{f.Name}` |");
      W(); return;
    }

    // fields (public + protected + internal, skip generated, skip static consts noise)
    var fields = t.Fields.Where(f => !Generated(f.Name) && !f.IsPrivate && !(f.IsStatic && f.HasConstant==false && f.Name.StartsWith("k")))
                         .OrderBy(f=>f.Name).ToList();
    if (fields.Count>0) {
      W("**Fields**");
      W();
      foreach (var f in fields)
        W($"- `{(f.IsStatic?"static ":"")}{TN(f.FieldType)} {f.Name}`{(f.IsPublic?"":" *("+Vis(f)+")*")}");
      W();
    }

    // properties (IL2CPP interop exposes game data members as properties: money, xp, switchType, IP, isOn...)
    var props = t.Properties.Where(pr => !Generated(pr.Name)).OrderBy(pr=>pr.Name).ToList();
    // hide pure-Unity/engine noise props inherited representation
    string[] noise = {"enabled","gameObject","tag","name","transform","hideFlags","useGUILayout","isActiveAndEnabled","WasCollected","destroyCancellationToken","transformHandle"};
    props = props.Where(pr => !noise.Contains(pr.Name) && !pr.Name.StartsWith("m_") && !pr.Name.Contains(".")).ToList();
    if (props.Count>0) {
      W("**Data members** *(read/write state)*");
      W();
      foreach (var pr in props) {
        bool canSet = pr.SetMethod != null;
        W($"- `{TN(pr.PropertyType)} {pr.Name}`{(canSet?"":" *(read-only)*")}");
      }
      W();
    }

    // methods: real methods (not getters/setters/ctors generated), include non-public as hook points
    var methods = t.Methods.Where(m => !m.IsGetter && !m.IsSetter && !m.IsConstructor && !Generated(m.Name)
                      && m.Name != "Finalize").OrderBy(m=>m.Name).ToList();
    if (!full) methods = methods.Where(m=>m.IsPublic).ToList();
    if (methods.Count>0) {
      W("**Methods**" + (full? " (incl. non-public — Harmony hook points)":""));
      W();
      foreach (var m in methods) {
        var ps = string.Join(", ", m.Parameters.Select(p => $"{TN(p.ParameterType)} {p.Name}"));
        var vis = m.IsPublic ? "" : $" *({Vis(m)})*";
        W($"- `{(m.IsStatic?"static ":"")}{TN(m.ReturnType)} {m.Name}({ps})`{vis}");
      }
      W();
    }
  }

  static int Main(string[] args) {
    var asmPath = args.Length > 0 ? args[0]
        : "../../../DC-NetworkingPlus-Mod/lib/Assembly-CSharp.dll";
    var outPath = args.Length > 1 ? args[1] : "../../GAME-API.md";
    if (!System.IO.File.Exists(asmPath)) { Console.Error.WriteLine($"Assembly not found: {asmPath}"); return 1; }
    mod = ModuleDefinition.ReadModule(asmPath);
    bool InGame(TypeDefinition t){ var d=t; while(d.DeclaringType!=null) d=d.DeclaringType; return d.Namespace=="Il2Cpp"; }
    var game = mod.GetTypes().Where(t => InGame(t) && !Generated(t.Name)).ToList();
    var byName = game.Where(t=>t.DeclaringType==null).GroupBy(t=>t.Name).ToDictionary(g=>g.Key, g=>g.First());

    // Subsystem groupings -> list of type names (core, documented in full)
    var groups = new (string title, string[] names)[] {
      ("Game Bootstrap & Managers", new[]{"MainGameManager","PlayerManager","TimeController","AudioManager","InputManager","SteamManager","TechnicianManager","DeviceTimerManager"}),
      ("Player & Economy", new[]{"Player","PlayerData"}),
      ("Network Core", new[]{"NetworkMap","INetworkEndpoint","ITimedDevice"}),
      ("Devices", new[]{"Server","NetworkSwitch","Router","Firewall","Rack","RackMount","RackDoor","RackPosition","CableLink"}),
      ("Device Configuration UIs", new[]{"RouterConfiguration","FirewallConfiguration","NetworkSwitchConfiguration","FirewallRuleRow","RouterRouteRow"}),
      ("Shop & Items", new[]{"ComputerShop","ShopItem","ShopItemSO","ShopItemConfig","ShopCartItem","ModShopItem"}),
      ("UI & Localisation", new[]{"StaticUIElements","Localisation","UI_Section"}),
      ("Save System", new[]{"SaveSystem","SaveData"}),
    };

    var documented = new HashSet<string>();
    foreach (var g in groups) {
      W($"## {g.title}"); W();
      foreach (var n in g.names) {
        if (byName.TryGetValue(n, out var t)) { DumpType(t, full:true); documented.Add(n); }
        else W($"*(type `{n}` not found)*\n");
      }
    }

    // Enums catalog
    W("## Enums"); W();
    foreach (var t in game.Where(t=>t.IsEnum).OrderBy(t=>(t.DeclaringType!=null?t.DeclaringType.Name+".":"")+t.Name)) { DumpType(t, true); documented.Add(t.Name); }

    // Save data payloads
    W("## Save Data Structures"); W();
    foreach (var t in game.Where(t=>t.Name.EndsWith("SaveData")).OrderBy(t=>t.Name)) { DumpType(t, false); documented.Add(t.Name); }

    // Full catalog of remaining game types
    W("## Full Game Type Catalog (remaining)"); W();
    W("| type | kind | fields | methods |"); W("|---|---|---|---|");
    foreach (var t in game.Where(t=>t.DeclaringType==null && !documented.Contains(t.Name)).OrderBy(t=>t.Name)) {
      string kind = t.IsInterface?"interface":t.IsEnum?"enum":t.IsValueType?"struct":"class";
      int fc = t.Fields.Count(f=>!Generated(f.Name));
      int mc = t.Methods.Count(m=>!m.IsGetter&&!m.IsSetter&&!Generated(m.Name));
      W($"| `{t.Name}` | {kind} | {fc} | {mc} |");
    }

    System.IO.File.WriteAllText(outPath, sb.ToString());
    Console.WriteLine($"Wrote {sb.Length} chars to {outPath}. Game types: {game.Count}, documented in detail: {documented.Count}");
    return 0;
  }
}
