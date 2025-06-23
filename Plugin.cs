using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using TMPro;
using UnityEngine;
using MapValueTracker.Config;

namespace MapValueTracker
{
    [BepInPlugin(PLUGIN_GUID, PLUGIN_NAME, PLUGIN_VERSION)]
    public class MapValueTracker : BaseUnityPlugin
    {
        private const string PLUGIN_GUID = "MapValueTracker";
        private const string PLUGIN_NAME = "MapValueTracker";
        private const string PLUGIN_VERSION = "1.0.1";
        private readonly Harmony harmony = new Harmony("Luken.REPO.MapValueTracker");

        public static new ManualLogSource Logger;
        public static MapValueTracker instance;

        public static GameObject textInstance;
        public static TextMeshProUGUI valueText;

        public void Awake()
        {
            Logger = base.Logger;
            Logger.LogInfo($"{PLUGIN_GUID} loaded");

            if (instance == null)
                instance = this;

            Configuration.Init(Config);
            harmony.PatchAll();
        }
    }
}
