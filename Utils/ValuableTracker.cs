using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using UnityEngine;

namespace MapValueTracker.Utils;

public class ValuableTracker
{
    private static ValuableTracker _instance;

    public static ValuableTracker Instance => _instance ??= new ValuableTracker();

    private List<ValuableObject> _allItems = [];
    public List<ValuableObject> StrayItems { get; private set; } = [];
    public float StrayValue { get; private set; } = 0f;

    public void Add(ValuableObject item)
    {
        if (!item.isActiveAndEnabled)
            return;

        MapValueTracker.Logger.LogDebug($"ValuableTracker::Add | {item.name}");

        _allItems.Add(item);
        Update();
    }

    public void Remove(ValuableObject item)
    {
        _allItems.Remove(item);
        Update();
    }

    public void Reset()
    {
        _allItems.Clear();
        StrayValue = 0f;
        StrayItems.Clear();
    }

    public void Update()
    {
        //var items = FindObjectsOfType<ValuableObject>().ToList();
        var objectsInExtract = RoundDirector.instance.dollarHaulList;

        _allItems.RemoveAll(item => item == null || item.gameObject == null);
        StrayValue = 0f;
        StrayItems.Clear();

        foreach (var item in _allItems)
        {
            if (objectsInExtract.Contains(item.gameObject))
                continue;

            var value = Traverse.Create(item).Field("dollarValueCurrent").GetValue<float>();
            StrayValue += value;
            StrayItems.Add(item);
        }

        MapValueTracker.Logger.LogDebug($"ValuableTracker::Update | StrayCount: {StrayItems.Count} Value: {StrayValue:N0}");
    }

    public float GetItemDistance()
    {
        if (StrayItems.Count == 0)
            return 0f;

        ValuableObject closest = null;
        var closestDist = 0f;
        var playerPos = PlayerController.instance.transform.position;

        foreach (var item in StrayItems)
        {
            if (item == null || item.gameObject == null)
                continue;

            var discovered = Traverse.Create(item).Field("discovered").GetValue<bool>();

            if (discovered)
                continue;

            var itemPos = item.gameObject.transform.position;
            var dist = (itemPos - playerPos).magnitude;

            if (closest != null && !(dist < closestDist))
                continue;

            closest = item;
            closestDist = dist;
        }

        return closestDist;
    }
}
