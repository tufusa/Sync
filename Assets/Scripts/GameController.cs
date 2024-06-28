using System;
using System.Collections.Generic;
using UnityEngine;
using BayatGames.SaveGameFree;

public class GameController : MonoBehaviour
{
    static GridController grid;
    [SerializeField] Material playerColorMaterial;
    static Material playerColor;
    public static Color BackgroundColor;
    public static Color OppositeColor;
    List<Operation> moveOperations = new List<Operation>() { Operation.Right, Operation.Left, Operation.Up, Operation.Down };
    public static GridController GetCurrentGrid => grid;
    static ZoneController GetCurrentZone => GetCurrentGrid.GetCurrentZone;
    public static StageController GetCurrentStage => GetCurrentZone.GetCurrentStage;
    public static bool GetStageCleared => GetCurrentStage.GetIsCleard;
    static bool isPlayerControllable = true;
    static Color normalPlayerColor;
    static Color transparentPlayerColor;
    public static bool IsTitle { get; private set; } = true;
    public static bool IsPausing { get; private set; } = false;
    public static bool IsConfig { get; private set; } = false;
    public static bool IsKeyConfig { get; private set; } = false;
    static bool isThankyou = false;
    static bool isTheEnd = false;
    static PauseController pause;
    static TitleController title;
    public static ConfigController config;
    static List<ProgressController> progresses;
    static ZoneProgressController zoneProgress;
    static ThankyouController thankyou;
    static TheEndController theEnd;

    void Awake()
    {
        Init();
    }

    void Init()
    {
        playerColor = playerColorMaterial;
        grid = GameObject.FindGameObjectWithTag("Grid").GetComponent<GridController>();
        progresses = new List<ProgressController>(Array.ConvertAll(GameObject.FindGameObjectsWithTag("Progress"), p => p.GetComponent<ProgressController>()));
        zoneProgress = GetCurrentGrid.GetComponentInChildren<ZoneProgressController>();
        pause = GetComponentInChildren<PauseController>();
        title = GetComponentInChildren<TitleController>();
        config = GetComponentInChildren<ConfigController>();
        thankyou = GetComponentInChildren<ThankyouController>();
        theEnd = GetComponentInChildren<TheEndController>();
        normalPlayerColor = Color.white;
        transparentPlayerColor = normalPlayerColor - new Color(0f, 0f, 0f, 1 - 90 / 255f);
        GetCurrentGrid.FieldInit();
        CursorLock(true);
        SetPlayerTransparent(false);
    }

    void Start()
    {
        config.Init();
        Pause(false);
        Config(false);
        StartCoroutine(CheckInputMove());
        thankyou.Init();
        theEnd.Init();
    }

    void Update()
    {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
        if (Input.GetKeyDown(KeyCode.Return))
            GetCurrentGrid.ForceClearAllZone();
        else if (Input.GetKeyDown(KeyCode.Backspace))
            GetCurrentZone.NextStage();
        else if (Input.GetKeyDown(KeyCode.KeypadEnter))
            GetCurrentZone.StageClear();
#endif
        CheckInput();
    }

    void CheckInput()
    {
        if (isTheEnd || isThankyou || IsTitle || IsKeyConfig) return;

        if (Operation.Pause.GetKeyDown())
        {
            if (IsConfig)
                Config(!IsConfig);
            else
                Pause(!IsPausing);
        }

        if(Operation.Back.GetKeyDown())
        {
            if (IsConfig) Config(false);
            else if (IsPausing) Pause(false);
        }

        if (IsConfig || IsPausing) return;

        if (Operation.Reset.GetKeyDown())
        {
            Reset();
        }
        else if (GetCurrentStage.IsMovableToNext() && Operation.Continue.GetKeyDown())
        {
            GetCurrentZone.NextStage();
        }
        else if(Operation.Back.GetKeyDown())
        {
            GetCurrentZone.BackStage();
        }
        else if (Operation.Transparent.GetKeyDown())
        {
            SetPlayerTransparent(true);
        }
        else if (Operation.Transparent.GetKeyUp())
        {
            SetPlayerTransparent(false);
        }
    }

    IEnumerator<WaitForFixedUpdate> CheckInputMove()
    {
        while(true)
        {
            yield return null;
            if (isTheEnd || isThankyou || IsTitle || IsKeyConfig) continue;
            foreach (var op in moveOperations)
            {
                if (!isTheEnd && !isThankyou && !IsTitle && op.GetKeyDown())
                {
                    if (IsConfig)
                        config.ConfigPlayer.Move(op.GetDir());
                    else if (IsPausing)
                        pause.PausePlayer.Move(op.GetDir());
                    else if (isPlayerControllable)
                        GetCurrentStage.PlayersMove(op.GetDir());
                    yield return new WaitForFixedUpdate();
                    break;
                }
            }
        }
    }

    public static void SetPlayerControllable(bool value)
    {
        isPlayerControllable = value;
    }

    public static void SetIsKeyConfig(bool value)
    {
        IsKeyConfig = value;
    }

    public static void Reset()
    {
        IsPausing = false;
        IsConfig = false;
        GetCurrentStage.Reset();
        SetPlayerControllable(true);
    }

    public static void StartGame()
    {
        GetCurrentGrid.StartGame();
        GetCurrentStage.Reset();
        SetStageSelectPlayerColor(OppositeColor, BackgroundColor);
        title.gameObject.SetActive(false);
        IsTitle = false;
    }

    public static void Pause(bool value)
    {
        IsPausing = value;
        SetPlayerControllable(!value);
        pause.Pause(value);
        if(!value) SetPlayerTransparent(false);
    }

    public static void Config(bool value)
    {
        IsConfig = value;
        SetPlayerControllable(!value);
        config.Config(value);
    }

    public static void BackToTitle()
    {
        Reset();
        GetCurrentGrid.UnloadZone();
        title.gameObject.SetActive(true);
        Config(false);
        Pause(false);
        IsTitle = true;
    }

    public static void SetBackgroundColor(Color color)
    {
        BackgroundColor = color;
        OppositeColor = Color.white - BackgroundColor + Color.black;
        GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>().backgroundColor = BackgroundColor;
        Array.ForEach(GameObject.FindGameObjectsWithTag("Panel"), p => p.GetComponent<SpriteRenderer>().color = BackgroundColor - Color.black * 15 / 255f);
        progresses.ForEach(p => p.SetBandColor(BackgroundColor - Color.black * 55 / 255f));
        title.SetColor(OppositeColor);
        if(GetCurrentGrid.ZoneIdx == 0) SetStageSelectPlayerColor(OppositeColor, BackgroundColor);
    }

    public static void SetStageSelectPlayerBodyColor(Color color)
    {
        GetCurrentGrid.SetStageSelectPlayerBodyColor(color);
    }

    public static void SetStageSelectPlayerEyeColor(Color color)
    {
        GetCurrentGrid.SetStageSelectPlayerEyeColor(color);
    }

    public static void SetStageSelectPlayerColor(Color bodyColor, Color eyeColor)
    {
        SetStageSelectPlayerBodyColor(bodyColor);
        SetStageSelectPlayerEyeColor(eyeColor);
    }

    public static void CursorLock(bool value)
    {
#if !UNITY_EDITOR
        if (value)
            Cursor.lockState = CursorLockMode.Locked;
        else
            Cursor.lockState = CursorLockMode.None;
        Cursor.visible = !value;
#endif
    }

    public static void SetFullScreenMode(bool value)
    {
        Screen.SetResolution(Screen.width, Screen.height, value);
        CursorLock(value);
    }

    public static void SetResolution(int width, int height)
    {
        Screen.SetResolution(width, height, Screen.fullScreen);
        CursorLock(Screen.fullScreen);
    }

    public static void ResetProgress()
    {
        progresses.ForEach(p => p.Reset());
        zoneProgress.Reset();
    }

    static void SetPlayerTransparent(bool value)
    {
        SetPlayerControllable(!value);
        if (value)
            playerColor.color = transparentPlayerColor;
        else
            playerColor.color = normalPlayerColor;
    }

    public static void Quit()
    {
        SetPlayerTransparent(false);
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
        Application.Quit();
    }

    public static void Thankyou(bool value)
    {
        isThankyou = value;
        thankyou.SetActive(isThankyou);
    }

    public static void BackToStageSelect()
    {
        GetCurrentStage.Reset();
        GetCurrentGrid.LoadZone(0);
        SetStageSelectPlayerEyeColor(BackgroundColor);
    }

    public static void StartChallenge()
    {
        if (zoneProgress.LoadClearZoneCount() < 8) return;
        StartGame();
        GetCurrentGrid.LoadZone(9);
        zoneProgress.SaveClearZone(9);
        GetCurrentZone.Progress.Reset();
        GetCurrentZone.UnloadStage();
        GetCurrentZone.LoadStage(0);
    }

    public static void ClearChallenge()
    {
        if (GetCurrentGrid.ZoneIdx != 9) return;

        GetCurrentZone.SetActive(false);
        TheEnd(true);
    }

    public static void TheEnd(bool value)
    {
        isTheEnd = value;
        theEnd.SetActive(isTheEnd);
    }
}
