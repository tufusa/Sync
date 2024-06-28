using BayatGames.SaveGameFree;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

public class KeyConfig : MonoBehaviour
{
    [SerializeField] protected Operation operation;
    static readonly string keyConfigDic = "KeyConfigDic";
    static Dictionary<Operation, KeyCode> keyConfing;
    static readonly ReadOnlyDictionary<Operation, KeyCode> initKeyConfig = new ReadOnlyDictionary<Operation, KeyCode>(new Dictionary<Operation, KeyCode>()
    {
        { Operation.Right, KeyCode.RightArrow },
        { Operation.Left, KeyCode.LeftArrow },
        { Operation.Up, KeyCode.UpArrow },
        { Operation.Down, KeyCode.DownArrow },
        { Operation.Pause, KeyCode.Escape },
        { Operation.Reset, KeyCode.R },
        { Operation.Back, KeyCode.Z },
        { Operation.Continue, KeyCode.Space },
        { Operation.Transparent, KeyCode.LeftShift }
    });
    static readonly ReadOnlyCollection<KeyCode> keycodePool = new ReadOnlyCollection<KeyCode>(new List<KeyCode>()
    {
        KeyCode.A, KeyCode.B, KeyCode.C, KeyCode.D, KeyCode.E, KeyCode.F, KeyCode.G, KeyCode.H, KeyCode.I, KeyCode.J, KeyCode.K, KeyCode.L, KeyCode.M, 
        KeyCode.N, KeyCode.O, KeyCode.P, KeyCode.Q, KeyCode.R, KeyCode.S, KeyCode.T, KeyCode.U, KeyCode.V, KeyCode.W, KeyCode.X, KeyCode.Y, KeyCode.Z,
        KeyCode.Escape, KeyCode.LeftShift, KeyCode.Space, KeyCode.Return, KeyCode.RightShift, KeyCode.LeftControl, KeyCode.RightControl, KeyCode.Backspace,
        KeyCode.RightArrow, KeyCode.LeftArrow, KeyCode.UpArrow, KeyCode.DownArrow
    });

    static Dictionary<Operation, KeyCode> GetInitKeyConfig => new Dictionary<Operation, KeyCode>(initKeyConfig);
    static Coroutine keyConfigCoroutine;

    public static void Init()
    {
        SaveGame.Encode = true;
        keyConfing = LoadKeyConfigDic();
    }

    public static Dictionary<Operation, KeyCode> LoadKeyConfigDic()
    {
        return SaveGame.Load(keyConfigDic, GetInitKeyConfig);
    }

    public static void SaveKeyConfig(Operation operation, KeyCode keyCode)
    {
        keyConfing[operation] = keyCode;
        SaveGame.Save(keyConfigDic, keyConfing, true);
    }

    public static void Reset()
    {
        SaveGame.Save(keyConfigDic, GetInitKeyConfig);
        keyConfing = LoadKeyConfigDic();
    }

    public static KeyCode GetKeyCode(Operation operation)
    {
        return keyConfing[operation];
    }

    protected void StartConfigure(KeyConfigInputController keyConfigInput)
    {
        if(keyConfigCoroutine == null) keyConfigCoroutine = StartCoroutine(ConfigureKeyCode(keyConfigInput));
    }

    IEnumerator ConfigureKeyCode(KeyConfigInputController keyConfigInput)
    {
        GameController.SetIsKeyConfig(true);

        // Continueのキーコンを設定しようとしてContinueを押した際に
        // 下のGetKeyDownが取られてWaitFinishConfigureが走ってしまうので
        // Continueの時は1フレーム空ける
        if (keyConfigInput.operation == Operation.Continue) yield return null;

        while(true)
        {
            foreach(KeyCode keyCode in keycodePool)
            {
                if(Input.GetKeyDown(keyCode) && (keyCode == keyConfigInput.operation.GetKeyCode() || !keyConfing.ContainsValue(keyCode)))
                {
                    Debug.Log(keyCode);
                    SaveKeyConfig(keyConfigInput.operation, keyCode);
                    keyConfigInput.UpdateText();
                    keyConfigCoroutine = null;
                    StartCoroutine(WaitFinishConfigure(keyCode));
                    yield break;
                }
            }
            yield return null;
        }
    }

    IEnumerator WaitFinishConfigure(KeyCode keyCode)
    {
        while(true)
        {
            if(Input.GetKeyUp(keyCode))
            {
                Debug.Log(keyCode);
                GameController.SetIsKeyConfig(false);
                yield break;
            }
            yield return null;
        }
    }
}

public enum Operation
{
    Right, Left, Up, Down, Pause, Reset, Back, Continue, Transparent
}
