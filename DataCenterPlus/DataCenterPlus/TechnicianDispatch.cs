using System;
using HarmonyLib;
using Il2Cpp;
using Il2CppInterop.Runtime;
using Il2CppTMPro;
using MelonLoader;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace DataCenterPlus
{
    // Click handler for the injected "Send Tech: All" button (IL2CPP UnityAction
    // can't bind lambdas, so we need a real injected MonoBehaviour).
    public class DispatchButtonHandler : MonoBehaviour
    {
        public DispatchButtonHandler(IntPtr ptr) : base(ptr) { }
        public void OnClick() => TechnicianDispatch.DispatchAll(TierConfig.DispatchIncludeBroken, TierConfig.DispatchIncludeEol);
    }

    // "Send a technician to all EOL/broken devices" in one click.
    internal static class TechnicianDispatch
    {
        internal static void DispatchAll(bool includeBroken, bool includeEol)
        {
            var tm = TechnicianManager.instance;
            if (tm == null) { MelonLogger.Warning("DataCenterPlus: dispatch-all — no TechnicianManager."); return; }

            int sent = 0, eol = 0, broken = 0, skipped = 0, scanned = 0;
            int minEol = int.MaxValue, maxEol = int.MinValue, eolLeZero = 0, signs = 0;

            var servers = Object.FindObjectsOfType<Server>();
            if (servers != null)
                for (int i = 0; i < servers.Length; i++)
                {
                    var s = servers[i];
                    if (s == null) continue;
                    scanned++;
                    bool isBroken = false, hasSign = false; int eolt = 1;
                    Core.TrySet(() => isBroken = s.isBroken, "");
                    Core.TrySet(() => eolt = s.eolTime, "");
                    Core.TrySet(() => hasSign = s.existingWarningSigns > 0 || s.existingErrorSigns > 0, "");
                    if (eolt < minEol) minEol = eolt;
                    if (eolt > maxEol) maxEol = eolt;
                    if (eolt <= 0) eolLeZero++;
                    if (hasSign) signs++;

                    if (!NeedsTech(isBroken, eolt, hasSign, includeBroken, includeEol)) continue;
                    if (isBroken) broken++; else eol++;

                    bool assigned = false;
                    Core.TrySet(() => assigned = tm.IsDeviceAlreadyAssigned(null, s), "");
                    if (assigned) { skipped++; continue; }
                    Core.TrySet(() => tm.SendTechnician(null, s), "send tech (server)");
                    sent++;
                }

            var switches = Object.FindObjectsOfType<NetworkSwitch>();
            if (switches != null)
                for (int i = 0; i < switches.Length; i++)
                {
                    var sw = switches[i];
                    if (sw == null) continue;
                    scanned++;
                    bool isBroken = false, hasSign = false; int eolt = 1;
                    Core.TrySet(() => isBroken = sw.isBroken, "");
                    Core.TrySet(() => eolt = sw.eolTime, "");
                    Core.TrySet(() => hasSign = sw.existingWarningSigns > 0 || sw.existingErrorSigns > 0, "");
                    if (eolt < minEol) minEol = eolt;
                    if (eolt > maxEol) maxEol = eolt;
                    if (eolt <= 0) eolLeZero++;
                    if (hasSign) signs++;

                    if (!NeedsTech(isBroken, eolt, hasSign, includeBroken, includeEol)) continue;
                    if (isBroken) broken++; else eol++;

                    bool assigned = false;
                    Core.TrySet(() => assigned = tm.IsDeviceAlreadyAssigned(sw, null), "");
                    if (assigned) { skipped++; continue; }
                    Core.TrySet(() => tm.SendTechnician(sw, null), "send tech (switch)");
                    sent++;
                }

            MelonLogger.Msg($"DataCenterPlus: dispatch-all — sent={sent} (eol={eol} broken={broken}) " +
                            $"skipped(assigned)={skipped} scanned={scanned} | diag: eolTime[min={minEol} max={maxEol}] " +
                            $"eol<=0={eolLeZero} withSign={signs}");
            Notify(sent > 0 ? $"Dispatched {sent} technician(s): {eol} EOL, {broken} broken"
                            : "No devices needed a technician");
        }

        private static bool NeedsTech(bool isBroken, int eolTime, bool hasSign, bool includeBroken, bool includeEol)
        {
            if (isBroken) return includeBroken;              // broken handled only when requested
            if (includeEol && TierConfig.DispatchIncludeEol && eolTime <= 0) return true;
            if (TierConfig.DispatchIncludeWarningSigns && hasSign) return true;
            return false;
        }

        private static void Notify(string msg)
        {
            Core.TrySet(() =>
            {
                var ui = StaticUIElements.instance;
                if (ui != null) ui.AddMeesageInField(msg);
            }, "dispatch notify");
        }

        // ---------------------------------------------------------------- UI injection
        internal static void InjectButton(AssetManagement am)
        {
            if (am == null) return;
            var src = am.buttonReturn != null ? am.buttonReturn.gameObject : null;
            if (src == null) return;
            var parent = src.transform.parent;
            if (parent == null) return;
            if (parent.Find("DCP_DispatchAll") != null) return; // already injected

            var clone = Object.Instantiate(src, parent, false);
            clone.name = "DCP_DispatchAll";

            var srcRt = src.GetComponent<RectTransform>();
            var rt = clone.GetComponent<RectTransform>();
            if (srcRt != null && rt != null)
            {
                rt.anchorMin = srcRt.anchorMin; rt.anchorMax = srcRt.anchorMax; rt.pivot = srcRt.pivot;
                rt.sizeDelta = srcRt.sizeDelta;
                rt.anchoredPosition = srcRt.anchoredPosition + TierConfig.DispatchButtonOffset;
            }

            var txt = clone.GetComponentInChildren<TextMeshProUGUI>(true);
            if (txt != null) txt.text = TierConfig.DispatchButtonLabel;

            var btn = clone.GetComponent<ButtonExtended>();
            if (btn != null)
            {
                Core.TrySet(() => btn.onClick.RemoveAllListeners(), "btn clear onClick");
                Core.TrySet(() => btn.functionToBeCalledOnSelect.RemoveAllListeners(), "btn clear onSelect");
                var h = clone.AddComponent<DispatchButtonHandler>();
                var action = DelegateSupport.ConvertDelegate<UnityEngine.Events.UnityAction>(h.OnClick);
                Core.TrySet(() => btn.m_OnClick.AddListener(action), "btn add onClick");
                Core.TrySet(() => btn.functionToBeCalledOnSelect.AddListener(action), "btn add onSelect");
            }

            clone.SetActive(true);
            MelonLogger.Msg("DataCenterPlus: injected 'Send Tech: All' button into Asset Management.");
        }
    }

    // Inject the button whenever the Asset Management screen opens.
    [HarmonyPatch(typeof(AssetManagement), nameof(AssetManagement.OnEnable))]
    internal static class PatchAssetManagementOnEnable
    {
        private static void Postfix(AssetManagement __instance) => TechnicianDispatch.InjectButton(__instance);
    }

    // Extend the game's existing "add all broken to queue" button to also dispatch EOL.
    [HarmonyPatch(typeof(AssetManagement), nameof(AssetManagement.ButtonAddAllBrokenDevicesToQueue))]
    internal static class PatchAddAllBrokenToQueue
    {
        private static void Postfix() => TechnicianDispatch.DispatchAll(includeBroken: false, includeEol: true);
    }
}
