using System.Collections.Generic;
using UnityEngine;
using System;

public class ZoneController : FieldController
{
    List<StageController> stages;
    int stageIdx = 0;
    [SerializeField] GameObject stagePrefab;

    public StageController GetCurrentStage => stages[stageIdx];
    public ProgressController Progress { get; private set; }
    GridController parentGrid;

    public override void Init()
    {
        stages = new List<StageController>(GetComponentsInChildren<StageController>());
        stages.Sort((x, y) => x.name.CompareTo(y.name));
        for (int i = 0; i < stages.Count; i++) stages[i].SetStageIdx(i);
        Progress = GetComponentInChildren<ProgressController>();
        parentGrid = transform.parent.GetComponent<GridController>();
    }

    public void NextStage()
    {
        if (stageIdx + 1 >= stages.Count)
        {
            GameController.BackToStageSelect();
            return;
        }
        UnloadStage();
        LoadStage(++stageIdx);
    }

    public void BackStage()
    {
        if (stageIdx - 1 < 0) return;
        UnloadStage();
        LoadStage(--stageIdx);
    }

    public void LoadStage(int index)
    {
        if (index < 0 || index >= stages.Count) return;
        stages[index].SetActive(true);
        Progress.SetPlayerMark(index);
        Progress.SaveLatestStage(index);
        stageIdx = index;
    }

    public void UnloadStage()
    {
        stages[stageIdx].SetActive(false);
        stages[stageIdx].Reset();
    }

    public override void SetActive(bool value)
    {
        stages.ForEach(s => s.SetActive(false));
        base.SetActive(value);
        if (value) LoadStage(Progress.LoadLatestStage());
    }

    public void StageClear()
    {
        Progress.SaveClearStageCount(stageIdx);
        if(stageIdx == stages.Count - 1) parentGrid.ZoneClear();
    }

    public void LoadZonePort()
    {
        Array.ForEach(GetComponentsInChildren<ZonePortController>(), p => p.Load());
    }

#if UNITY_EDITOR
    void AddStage()
    {
        GameObject newStage = UnityEditor.PrefabUtility.InstantiatePrefab(stagePrefab, transform) as GameObject;
        UnityEditor.Undo.RegisterCreatedObjectUndo(newStage, "Add Stage");
        newStage.name = "Stage." + transform.childCount.ToString();
    }
#endif
}
