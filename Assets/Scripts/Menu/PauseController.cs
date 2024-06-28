using UnityEngine;

public class PauseController : MonoBehaviour
{
    public PausePlayerController PausePlayer { get; private set; }

    void Awake()
    {
        PausePlayer = GetComponentInChildren<PausePlayerController>();
    }

    public void Pause(bool value)
    {
        gameObject.SetActive(value);
        PausePlayer.Reset();
    }

    public void Back()
    {
        if (GameController.GetCurrentGrid.ZoneIdx == 0) GameController.BackToTitle();
        else GameController.BackToStageSelect();
        GameController.Pause(false);
    }

    public void Config()
    {
        if(!GameController.IsConfig) GameController.Config(true);
    }

    public void Continue()
    {
        GameController.Pause(false);
    }
}
