using HarmonyLib;
using MapValueTracker.Utils;

namespace MapValueTracker.Patches
{
    [HarmonyPatch(typeof(ValuableObject))]
    static class ValuableObjectPatches
    {
        [HarmonyPatch("DollarValueSetRPC")]
        [HarmonyPostfix]
        static void DollarValueSet(ValuableObject __instance, float value)
        {
            MapValueTracker.Logger.LogDebug($"ValuableObject::DollarValueSetRPC | New Valuable Object: {__instance.name} - ${value:N0}");
            ValuableTracker.Instance.Add(__instance);
        }
        [HarmonyPatch("DollarValueSetLogic")]
        [HarmonyPostfix]
        static void DollarValueSetLogic(ValuableObject __instance)
        {
            if (!SemiFunc.IsMasterClientOrSingleplayer())
                return;

            var currentValue = Traverse.Create(__instance).Field("dollarValueCurrent").GetValue<float>();
            MapValueTracker.Logger.LogDebug($"ValuableObject::DollarValueSetLogic | New Valuable Object: {__instance.name} - ${currentValue:N0}");
            ValuableTracker.Instance.Add(__instance);
        }

        [HarmonyPatch("AddToDollarHaulList")]
        [HarmonyPostfix]
        static void AddToDollarHaulList(ValuableObject __instance)
        {
            var currentValue = Traverse.Create(__instance).Field("dollarValueCurrent").GetValue<float>();
            MapValueTracker.Logger.LogDebug($"ValuableObject::AddToDollarHaulList | Moved to Extract: {__instance.name} - ${currentValue:N0}");
            ValuableTracker.Instance.Update();
        }
        [HarmonyPatch("AddToDollarHaulListRPC")]
        [HarmonyPostfix]
        static void AddToDollarHaulListRPC(ValuableObject __instance)
        {
            var currentValue = Traverse.Create(__instance).Field("dollarValueCurrent").GetValue<float>();
            MapValueTracker.Logger.LogDebug($"ValuableObject::AddToDollarHaulListRPC | Moved to Extract: {__instance.name} - ${currentValue:N0}");
            ValuableTracker.Instance.Update();
        }

        [HarmonyPatch("RemoveFromDollarHaulList")]
        [HarmonyPostfix]
        static void RemoveFromDollarHaulList(ValuableObject __instance)
        {
            var currentValue = Traverse.Create(__instance).Field("dollarValueCurrent").GetValue<float>();
            MapValueTracker.Logger.LogDebug($"ValuableObject::RemoveFromDollarHaulList | Removed from Extract: {__instance.name} - ${currentValue:N0}");
            ValuableTracker.Instance.Update();
        }
        [HarmonyPatch("RemoveFromDollarHaulListRPC")]
        [HarmonyPostfix]
        static void RemoveFromDollarHaulListRPC(ValuableObject __instance)
        {
            var currentValue = Traverse.Create(__instance).Field("dollarValueCurrent").GetValue<float>();
            MapValueTracker.Logger.LogDebug($"ValuableObject::RemoveFromDollarHaulListRPC | Removed from Extract: {__instance.name} - ${currentValue:N0}");
            ValuableTracker.Instance.Update();
        }
    }
}
