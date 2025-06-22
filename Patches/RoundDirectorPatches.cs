using System.Collections.Generic;
using HarmonyLib;
using MapValueTracker.Config;
using TMPro;
using UnityEngine;

namespace MapValueTracker.Patches
{
    [HarmonyPatch(typeof(RoundDirector))]
    public static class RoundDirectorPatches
    {
        // [HarmonyPatch("ExtractionCompleted")]
        // [HarmonyPostfix]
        // public static void ExtractionCompleted()
        // {
        //     if (!SemiFunc.RunIsLevel())
        //         return;
        //
        //     MapValueTracker.Logger.LogDebug("Extraction Completed");
        // }

        [HarmonyPatch(typeof(RoundDirector), "Update")]
        [HarmonyPostfix]
        public static void UpdateUI()
        {
            if (!SemiFunc.RunIsLevel())
                return;

            if (MapValueTracker.textInstance == null && !InitTextOverlay())
                return;

            var allExtractionPointsCompleted = Traverse.Create(RoundDirector.instance).Field("allExtractionPointsCompleted").GetValue<bool>();
            var extractionPoints = Traverse.Create(RoundDirector.instance).Field("extractionPoints").GetValue<int>();
            var extractionPointsCompleted = Traverse.Create(RoundDirector.instance).Field("extractionPointsCompleted").GetValue<int>();
            var currentHaul = Traverse.Create(RoundDirector.instance).Field("currentHaul").GetValue<int>();
            var extractionHaulGoal = Traverse.Create(RoundDirector.instance).Field("extractionHaulGoal").GetValue<int>();
            var mapToggled = SemiFunc.InputHold(InputKey.Map) || Traverse.Create(MapToolController.instance).Field("mapToggled").GetValue<bool>();

            if (allExtractionPointsCompleted || MapValueTracker.strayList.Count == 0 || (!mapToggled && !Configuration.AlwaysOn.Value))
            {
                MapValueTracker.textInstance.SetActive(false);
                return;
            }

            MapValueTracker.textInstance.SetActive(true);

            var component = MapValueTracker.textInstance.GetComponent<RectTransform>();
            SetCoordinates(component);

            var textParts = new List<string>();
            var isLastExtractGoalMet = extractionPointsCompleted >= extractionPoints - 1 && currentHaul >= extractionHaulGoal;
            var canShowItemDistance = isLastExtractGoalMet || Configuration.AlwaysShowDistance.Value;

            if (Configuration.ShowItemDistance.Value && canShowItemDistance)
            {
                var closestDist = MapValueTracker.GetItemDistance();

                if (Configuration.UsePreciseDistance.Value)
                {
                    textParts.Add($"{closestDist:N1}m");
                }
                else
                {
                    switch (closestDist)
                    {
                        case < 5:
                            textParts.Add("•••"); break;
                        case < 15:
                            textParts.Add("••"); break;
                        case < 25:
                            textParts.Add("•"); break;
                        default:
                            textParts.Add(""); break;
                    }
                }
            }

            if (Configuration.ShowTotalValue.Value)
                textParts.Add($"${MapValueTracker.strayValue:N0}");

            if (Configuration.ShowItemCount.Value)
                textParts.Add($"x{MapValueTracker.strayList.Count}");

            MapValueTracker.valueText.SetText(textParts.Join(null, " "));
        }

        private static bool InitTextOverlay()
        {
            var hud = GameObject.Find("Game Hud");
            var haul = GameObject.Find("Tax Haul");

            if (hud == null || haul == null)
                return false;

            var font = haul.GetComponent<TMP_Text>().font;
            MapValueTracker.textInstance = new GameObject();
            MapValueTracker.textInstance.SetActive(false);
            MapValueTracker.textInstance.name = "Value HUD";
            MapValueTracker.textInstance.AddComponent<TextMeshProUGUI>();

            MapValueTracker.valueText = MapValueTracker.textInstance.GetComponent<TextMeshProUGUI>();
            MapValueTracker.valueText.font = font;
            MapValueTracker.valueText.color = new Vector4(0.7882f, 0.9137f, 0.902f, 1);
            MapValueTracker.valueText.fontSize = 24f;
            MapValueTracker.valueText.enableWordWrapping = false;
            MapValueTracker.valueText.alignment = TextAlignmentOptions.BaselineRight;
            MapValueTracker.valueText.horizontalAlignment = HorizontalAlignmentOptions.Right;
            MapValueTracker.valueText.verticalAlignment = VerticalAlignmentOptions.Baseline;

            MapValueTracker.textInstance.transform.SetParent(hud.transform, false);

            return true;
        }

        private static void SetCoordinates(RectTransform component)
        {
            switch (Configuration.UIPosition.Value)
            {
                case Positions.Default:
                    component.pivot = new Vector2(1f, 1f);
                    component.anchoredPosition = new Vector2(1f, -1f);
                    component.anchorMin = new Vector2(0f, 0f);
                    component.anchorMax = new Vector2(1f, 0f);
                    component.sizeDelta = new Vector2(0f, 0f);
                    component.offsetMax = new Vector2(0f, 225f);
                    component.offsetMin = new Vector2(0f, 225f);
                    break;
                case Positions.LowerRight:
                    component.pivot = new Vector2(1f, 1f);
                    component.anchoredPosition = new Vector2(1f, -1f);
                    component.anchorMin = new Vector2(0f, 0f);
                    component.anchorMax = new Vector2(1f, 0f);
                    component.sizeDelta = new Vector2(0f, 0f);
                    component.offsetMax = new Vector2(0f, 125f);
                    component.offsetMin = new Vector2(0f, 125f);
                    break;
                case Positions.BottomRight:
                    component.pivot = new Vector2(1f, 1f);
                    component.anchoredPosition = new Vector2(1f, -1f);
                    component.anchorMin = new Vector2(0f, 0f);
                    component.anchorMax = new Vector2(1f, 0f);
                    component.sizeDelta = new Vector2(0f, 0f);
                    component.offsetMax = new Vector2(0f, 0f);
                    component.offsetMin = new Vector2(0f, 0f);
                    break;
                case Positions.Custom:
                    component.pivot = new Vector2(1f, 1f);
                    component.anchoredPosition = new Vector2(1f, -1f);
                    component.anchorMin = new Vector2(0f, 0f);
                    component.anchorMax = new Vector2(1f, 0f);
                    component.sizeDelta = new Vector2(0f, 0f);
                    component.offsetMax = Configuration.CustomPositionCoords.Value;
                    component.offsetMin = Configuration.CustomPositionCoords.Value;
                    break;
                default:
                    component.pivot = new Vector2(1f, 1f);
                    component.anchoredPosition = new Vector2(1f, -1f);
                    component.anchorMin = new Vector2(0f, 0f);
                    component.anchorMax = new Vector2(1f, 0f);
                    component.sizeDelta = new Vector2(0f, 0f);
                    component.offsetMax = new Vector2(0, 225f);
                    component.offsetMin = new Vector2(0f, 225f);
                    break;
            }
        }
    }
}
