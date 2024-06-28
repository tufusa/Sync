using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GridController : FieldController
{
    List<ZoneController> zones;
    public int ZoneIdx { get; private set; } = 0;
    [SerializeField] GameObject zonePrefab;
    [SerializeField] Material zoneMaterial;
    [SerializeField] List<Color> zoneColors = new List<Color>() { Color.black };

    public ZoneController GetCurrentZone => zones[ZoneIdx];
    public Color GetCurrentZoneColor => zoneColors[ZoneIdx];
    public int GetZoneCount => zones.Count;
    ZoneProgressController zoneProgress;

    public override void Init()
    {
        zones = new List<ZoneController>(GetComponentsInChildren<ZoneController>());
        zones.Sort((x, y) => x.name.CompareTo(y.name));
        zones.ForEach(z => z.SetActive(false));
        zoneProgress = GetComponentInChildren<ZoneProgressController>();
    }

    public void StartGame()
    {
        ZoneIdx = 0;
        LoadZone(ZoneIdx);
    }

    public void NextZone()
    {
        if (ZoneIdx + 1 >= zones.Count) return;
        UnloadZone();
        LoadZone(++ZoneIdx);
    } 

    public void LoadZone(int idx)
    {
        UnloadZone();
        if (idx >= zones.Count) return;
        ZoneIdx = idx;
        zoneMaterial.color = GetCurrentZoneColor;
        GetCurrentZone.SetActive(true);
        if (idx == 0) GetCurrentZone.LoadZonePort();
    }

    public void UnloadZone()
    {
        if (ZoneIdx >= zones.Count) return;
        GetCurrentZone.SetActive(false);
        if (ZoneIdx != 0) GetCurrentZone.UnloadStage();
    }

    public void FieldInit()
    {
        Array.ForEach(GetComponentsInChildren<PlayerController>(), p => p.Init());
        Array.ForEach(GetComponentsInChildren<AreaController>(), a => a.Init());
        Array.ForEach(GetComponentsInChildren<StageController>(), s => s.Init());
        Array.ForEach(GetComponentsInChildren<ProgressController>(), p => p.Init());
        Array.ForEach(GetComponentsInChildren<ZoneController>(), z => z.Init());
        Init();
    }

    public Color GetColor(int zoneIdx)
    {
        return zoneColors[zoneIdx];
    }

    public void SetStageSelectPlayerBodyColor(Color color)
    {
        PlayerController p = zones[0].GetCurrentStage.GetPlayers[0];
        if (p.Color == Color.white || p.Color == Color.black || GameController.IsTitle) p.SetBodyColor(color);
    }

    public void SetStageSelectPlayerEyeColor(Color color)
    {
        zones[0].GetCurrentStage.GetPlayers[0].SetEyeColor(color);
    }

    public void ZoneClear()
    {
        zoneProgress.SaveClearZone(ZoneIdx);
        if (ZoneIdx == 8) GameController.Thankyou(true);
        if (ZoneIdx == 9) GameController.ClearChallenge();
    }
    public int ClearZoneCount()
    {
        return zoneProgress.LoadClearZoneCount();
    }

#if UNITY_EDITOR
    void AddZone()
    {
        GameObject newZone = UnityEditor.PrefabUtility.InstantiatePrefab(zonePrefab, transform) as GameObject;
        UnityEditor.Undo.RegisterCreatedObjectUndo(newZone, "Add Zone");
        newZone.name = "Zone." + transform.childCount.ToString();
    }
#endif

#if UNITY_EDITOR || DEVELOPMENT_BUILD
    public void ForceClearAllZone()
    {
        foreach(int i in Enumerable.Range(1, zones.Count - 1)) zoneProgress.SaveClearZone(i);
    }
#endif
}
