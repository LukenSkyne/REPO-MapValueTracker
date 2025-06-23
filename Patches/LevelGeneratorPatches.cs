using HarmonyLib;
using MapValueTracker.Utils;

namespace MapValueTracker.Patches
{
    [HarmonyPatch(typeof(LevelGenerator))]
    static class LevelGeneratorPatches
    {
        [HarmonyPatch("StartRoomGeneration")]
        [HarmonyPrefix]
        public static void StartRoomGeneration()
        {
            MapValueTracker.Logger.LogInfo("Generation Started");
            ValuableTracker.Instance.Reset();
        }

        [HarmonyPatch("GenerateDone")]
        [HarmonyPrefix]
        public static void GenerateDonePostfix()
        {
            MapValueTracker.Logger.LogInfo("Generation Finished");
        }
    }
}
