using BayatGames.SaveGameFree;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class ZoneProgressController : MonoBehaviour
{
    readonly string clearZone = "ClearZone";
    List<bool> zoneIsCleared;

    void Start()
    {
        SaveGame.Encode = true;
        zoneIsCleared = LoadClearZoneList();
    }

    public int LoadClearZoneCount()
    {
        return zoneIsCleared.Count(b => b);
    }

    public void SaveClearZone(int index)
    {
        zoneIsCleared[index] = true;
        SaveGame.Save(clearZone, zoneIsCleared, true);
    }

    List<bool> LoadClearZoneList()
    {
        return SaveGame.Load(clearZone, new List<bool>(new bool[GameController.GetCurrentGrid.GetZoneCount]));
    }

    public void Reset()
    {
        zoneIsCleared = new List<bool>(new bool[GameController.GetCurrentGrid.GetZoneCount]);
        SaveGame.Save(clearZone, zoneIsCleared, true);
    }
}
