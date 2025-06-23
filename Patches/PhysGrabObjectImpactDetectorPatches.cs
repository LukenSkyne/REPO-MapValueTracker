using HarmonyLib;
using MapValueTracker.Utils;

namespace MapValueTracker.Patches
{
    [HarmonyPatch(typeof(PhysGrabObjectImpactDetector))]
    public static class PhysGrabObjectImpactDetectorPatches
    {
        [HarmonyPatch("BreakRPC")]
        [HarmonyPostfix]
        static void BreakRPC(float valueLost, PhysGrabObjectImpactDetector? __instance, bool _loseValue)
        {
            if (!_loseValue)
                return;

            MapValueTracker.Logger.LogDebug($"PhysGrabObjectImpactDetector::BreakRPC | Lost: {valueLost}");
            ValuableTracker.Instance.Update();
        }

        [HarmonyPatch(typeof(PhysGrabObject), "DestroyPhysGrabObjectRPC")]
        [HarmonyPostfix]
        public static void DestroyPhysGrabObjectRPC(PhysGrabObject __instance)
        {
            if (!SemiFunc.RunIsLevel())
                return;

            var vo = __instance.GetComponent<ValuableObject>();

            if (vo == null)
                return;

            var originalValue = Traverse.Create(vo).Field("dollarValueOriginal").GetValue<float>();
            MapValueTracker.Logger.LogDebug($"PhysGrabObjectImpactDetector::DestroyPhysGrabObjectRPC | Original Value: {originalValue}");
            ValuableTracker.Instance.Remove(vo);
        }
    }
}
