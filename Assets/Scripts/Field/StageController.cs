using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

public class StageController : FieldController
{
    List<AreaController> areas;
    List<PlayerController> players;
    public ReadOnlyCollection<PlayerController> GetPlayers => players.AsReadOnly();
    public ReadOnlyCollection<AreaController> GetAreas => areas.AsReadOnly();
    bool isCleared = false;
    public bool GetIsCleard => isCleared;
    [SerializeField] GameObject areaPrefab;
    ZoneController parentZone;
    int stageIdx;

    void FixedUpdate()
    {
        if (Application.isPlaying)
        {
            CheckClear();
        }
    }

    public override void Init()
    {
        areas = new List<AreaController>(GetComponentsInChildren<AreaController>());
        players = new List<PlayerController>();
        foreach (var a in areas) players.AddRange(a.GetPlayers);
        parentZone = transform.parent.GetComponent<ZoneController>();
    }

    void CheckClear()
    {
        if (!isCleared && players.Count != 0 && players.TrueForAll(p => p.GetFloorName == "Goal_3")) StageClear();
    }

    public void PlayersMove(Dir dir)
    {
        players.ForEach(p => p.Move(dir));
    }

    public override void SetActive(bool value)
    {
        base.SetActive(value);
    }

    void StageClear()
    {
        isCleared = true;
        parentZone.StageClear();
        players.ForEach(p => p.StartSquat());
    }

    public void SetStageIdx(int idx)
    {
        stageIdx = idx;
    }

    public bool IsMovableToNext()
    {
        return isCleared || parentZone.Progress.LoadClearStageCount() > stageIdx;
    }

    public bool OnStage(Collider2D collision)
    {
        foreach(var a in areas)
        {
            if (a.CheckerCollider.IsTouching(collision))
                return true;
        }
        return false;
    }


#if UNITY_EDITOR
    void AddArea()
    {
        GameObject newArea = UnityEditor.PrefabUtility.InstantiatePrefab(areaPrefab, transform) as GameObject;
        UnityEditor.Undo.RegisterCreatedObjectUndo(newArea, "Add Area");
        newArea.name = "Area." + transform.childCount.ToString();
    }
#endif

    public void Reset()
    {
        Array.ForEach(GetComponentsInChildren<GimmickController>(), g => g.Reset());
        Array.ForEach(GetComponentsInChildren<AreaController>(), a => a.Reset());
        players.ForEach(p => p.Reset());
        isCleared = false;
    }
}
