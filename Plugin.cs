using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using TMPro;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using MapValueTracker.Config;

namespace MapValueTracker
{
    [BepInPlugin(PLUGIN_GUID, PLUGIN_NAME, PLUGIN_VERSION)]
    public class MapValueTracker : BaseUnityPlugin
    {
        private const string PLUGIN_GUID = "MapValueTracker";
        private const string PLUGIN_NAME = "MapValueTracker";
        private const string PLUGIN_VERSION = "1.0.0";
        private readonly Harmony harmony = new Harmony("Luken.REPO.MapValueTracker");

        public static new ManualLogSource Logger;
        public static MapValueTracker instance;

        public static GameObject textInstance;
        public static TextMeshProUGUI valueText;

        public static float strayValue = 0f;
        public static List<ValuableObject> strayList = [];

        public void Awake()
        {
            Logger = base.Logger;
            Logger.LogInfo($"{PLUGIN_GUID} loaded");

            if (instance == null)
                instance = this;

            Configuration.Init(Config);
            harmony.PatchAll();
        }

        public static void ResetValues()
        {
            if (SemiFunc.RunIsLevel())
                return;

            strayValue = 0f;
            strayList.Clear();
        }

        public static void UpdateTracker(ValuableObject destroyed = null)
        {
            //!Traverse.Create(RoundDirector.instance).Field("allExtractionPointsCompleted").GetValue<bool>()

            var items = FindObjectsOfType<ValuableObject>().ToList();
            var objectsInExtract = RoundDirector.instance.dollarHaulList;

            strayValue = 0f;
            strayList.Clear();

            foreach (var item in items)
            {
                if (objectsInExtract.Contains(item.gameObject) || item == destroyed)
                    continue;

                var value = Traverse.Create(item).Field("dollarValueCurrent").GetValue<float>();
                strayValue += value;
                strayList.Add(item);
            }

            Logger.LogDebug($"MapValueTracker::UpdateTracker | StrayCount: {strayList.Count} Value: {strayValue:N0}");
        }

        public static float GetItemDistance()
        {
            if (strayList.Count == 0)
                return 0f;

            ValuableObject closest = null;
            var closestDist = 0f;
            var playerPos = PlayerController.instance.transform.position;

            foreach (var valuableObject in strayList)
            {
                if (valuableObject == null || valuableObject.gameObject == null)
                    continue;

                var itemPos = valuableObject.gameObject.transform.position;
                var dist = (itemPos - playerPos).magnitude;

                if (closest != null && !(dist < closestDist))
                    continue;

                closest = valuableObject;
                closestDist = dist;
            }

            return closestDist;
        }
    }
}
