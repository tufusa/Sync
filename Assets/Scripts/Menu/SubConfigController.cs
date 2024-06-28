using System;
using UnityEngine;

public class SubConfigController : MonoBehaviour
{
    [SerializeField] SubConfig configType;
    [SerializeField] string value;
    ConfigController config => GameController.config;

    public void Configurate()
    {
        switch(configType)
        {
            case SubConfig.Resolution:
#if !UNITY_EDITOR
                if(int.TryParse(value, out int coef))
                    GameController.SetResolution(coef * 320, coef * 180);
#endif
                break;
            case SubConfig.FullScreen:
                if (value == "True")
                    GameController.SetFullScreenMode(true);
                else if (value == "False")
                    GameController.SetFullScreenMode(false);
                break;
            case SubConfig.Sound:
                if (int.TryParse(value, out int level))
                    config.SetVolumeLevel(level);
                break;
            case SubConfig.KeyConfig:
                if (value == "Reset")
                {
                    KeyConfig.Reset();
                    Array.ForEach(transform.parent.GetComponentsInChildren<KeyConfigInputController>(), k => k.UpdateText());
                }
                else
                    GetComponent<KeyConfigInputController>().StartConfigure();
                break;
            case SubConfig.Background:
                if (value == "White")
                    config.SetBackgroundColor(Color.white);
                else if (value == "Black")
                    config.SetBackgroundColor(Color.black);
                break;
            case SubConfig.Delete:
                if (value == "Delete")
                {
                    GameController.ResetProgress();
                    GameController.BackToTitle();
                }
                break;
        }
    }


}

enum SubConfig
{ 
    Resolution, FullScreen, Sound, KeyConfig, Background, Delete
}
