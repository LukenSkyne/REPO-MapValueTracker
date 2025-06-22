using HarmonyLib;

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
            MapValueTracker.ResetValues();
        }

        [HarmonyPatch("GenerateDone")]
        [HarmonyPrefix]
        public static void GenerateDonePostfix()
        {
            MapValueTracker.Logger.LogInfo("Generation Finished");
            MapValueTracker.UpdateTracker();
        }
    }
}
