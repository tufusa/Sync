using System.Collections.Generic;
using UnityEngine;
using BayatGames.SaveGameFree;

public class ConfigController : MonoBehaviour
{
    public ConfigPlayerController ConfigPlayer { get; private set; }
    List<MainConfigController> mainConfigs;
    [System.NonSerialized] public MainConfigController activeMainConfig;
    readonly string volumeLevel = "VolumeLevel";
    readonly string background = "Background";

    void Awake()
    {
        ConfigPlayer = GetComponentInChildren<ConfigPlayerController>();
        mainConfigs = new List<MainConfigController>(GetComponentsInChildren<MainConfigController>());
        KeyConfig.Init();
    }

    public void Init()
    {
        SetBackgroundColor(SaveGame.Load(background, Color.white));
        SetVolumeLevel(SaveGame.Load(volumeLevel, 9));
    }

    public void Config(bool value)
    {
        gameObject.SetActive(value);
        mainConfigs.ForEach(mc => mc.Select(false));
        ConfigPlayer.Reset();
    }

    public void SetBackgroundColor(Color color)
    {
        GameController.SetBackgroundColor(color);
        SaveGame.Save(background, color);
    }

    public void SetVolumeLevel(int level)
    {
        AudioListener.volume = level / 9f;
        SaveGame.Save(volumeLevel, level);
    }
}
