using BayatGames.SaveGameFree;
using System.Collections.Generic;
using UnityEngine;

public class ProgressController : MonoBehaviour
{
    [SerializeField] bool isBarEnabled = true;
    [SerializeField] Sprite onLamp;
    [SerializeField] Sprite offLamp;
    [SerializeField] List<SpriteRenderer> lamps;
    GameObject playerMark;
    GameObject band;
    string path;
    string clearStageCount;
    string latestStage;

    public void Init()
    {
        SaveGame.Encode = true;
        playerMark = transform.Find("PlayerMark").gameObject;
        band = transform.Find("Band").gameObject;
        SetKeyStrings();
        if (!isBarEnabled)
        {
            playerMark.SetActive(false);
            band.SetActive(false);
            lamps.ForEach(l => l.gameObject.SetActive(false));
        }
        LitLamps();
    }

    void SetKeyStrings()
    {
        path = gameObject.GetPath();
        clearStageCount = path + "_ClearStageCount";
        latestStage = path + "_LatestStage";
    }

    public void SaveLatestStage(int idx)
    {
        SaveGame.Save(latestStage, idx, true);
    }

    public int LoadLatestStage()
    {
        return SaveGame.Load(latestStage, 0);
    }

    public void SaveClearStageCount(int idx)
    {
        if(LoadClearStageCount() < idx + 1) SaveGame.Save(clearStageCount, idx + 1, true);
        LitLamps();
    }
    
    public int LoadClearStageCount()
    {
        return SaveGame.Load(clearStageCount, 0, true);
    }

    void LitLamps()
    {
        int clearCount = LoadClearStageCount();
        for (int i = 0; i < lamps.Count; i++)
        {
            if (i < clearCount)
                lamps[i].sprite = onLamp;
            else
                lamps[i].sprite = offLamp;
        }
    }

    public void SetPlayerMark(int stageIdx)
    {
        playerMark.transform.position = lamps[stageIdx].transform.position;
    }

    public void SetBandColor(Color color)
    {
        band.GetComponent<SpriteRenderer>().color = color;
    }

    public void Reset()
    {
        Debug.Log("Reset!");
        SaveGame.Save(clearStageCount, 0, true);
        SaveGame.Save(latestStage, 0, true);
        LitLamps();
    }
}
