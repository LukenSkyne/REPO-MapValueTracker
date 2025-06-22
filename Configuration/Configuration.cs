using BepInEx.Configuration;
using HarmonyLib;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace MapValueTracker.Config
{
    public enum Positions
    {
        Default,
        LowerRight,
        BottomRight,
        Custom
    }

	internal class Configuration
    {
        public static ConfigEntry<bool> AlwaysOn;
        public static ConfigEntry<bool> ShowTotalValue;
        public static ConfigEntry<bool> ShowItemCount;
        public static ConfigEntry<bool> ShowItemDistance;
        public static ConfigEntry<bool> UsePreciseDistance;
        public static ConfigEntry<bool> AlwaysShowDistance;
        public static ConfigEntry<Positions> UIPosition;
        public static ConfigEntry<Vector2> CustomPositionCoords;

        public static void Init(ConfigFile config)
        {
            config.SaveOnConfigSet = false;

            AlwaysOn = config.Bind(
                "Default",
                "AlwaysOn",
                false,
                "Toggle to always display map value when an extraction goal is active. If false, use the menu key to pull up the tracker (Tab by default)."
            );
            ShowTotalValue = config.Bind(
                "Default",
                "ShowTotalValue",
                true,
                "Toggle to show the total value of all stray items on the map."
            );
            ShowItemCount = config.Bind(
                "Default",
                "ShowItemCount",
                true,
                "Toggle to show the number of stray items on the map."
            );
            ShowItemDistance = config.Bind(
                "Default",
                "ShowItemDistance",
                true,
                "Toggle to display the distance to the closest valuable."
            );
            UsePreciseDistance = config.Bind(
                "Default",
                "UsePreciseDistance",
                false,
                "Toggle to display the distance in meters instead of a rough estimate."
            );
            AlwaysShowDistance = config.Bind(
                "Default",
                "AlwaysShowDistance",
                false,
                "Toggle to always display the distance, instead of only being active after meeting the last extract goal."
            );
            UIPosition = config.Bind(
                "UIPosition",
                "UIPosition",
                Positions.Default,
                "Preset Position of the Value Tracker UI element. Default is on the right side, below the extraction targets."
            );
            CustomPositionCoords = config.Bind(
                "UIPosition",
                "CustomPositionCoords",
                new Vector2(0, 0),
                "Custom X,Y coordates of the Value Tracker UI element. Bottom Right corner is 0,0. Default position is 0,225."
            );

            ClearOrphanedEntries(config);
            config.Save();
            config.SaveOnConfigSet = true;
        }

        static void ClearOrphanedEntries(ConfigFile cfg)
        {
            // Find the private property `OrphanedEntries` from the type `ConfigFile` //
            PropertyInfo orphanedEntriesProp = AccessTools.Property(typeof(ConfigFile), "OrphanedEntries");
            // And get the value of that property from our ConfigFile instance //
            var orphanedEntries = (Dictionary<ConfigDefinition, string>)orphanedEntriesProp.GetValue(cfg);
            // And finally, clear the `OrphanedEntries` dictionary //
            orphanedEntries.Clear();
        }
    }
}
