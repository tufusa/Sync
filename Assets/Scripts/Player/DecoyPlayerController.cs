using UnityEngine;

public class DecoyPlayerController : Player
{
    [SerializeField] bool isSleepable = true;

    void Awake()
    {
        Init();
    }

    void OnEnable()
    {
        InitTurn();
        ShakeOff();
    }

    void FixedUpdate()
    {
        sleepiness += Time.deltaTime;
        if (isSleepable && sleepiness > 60f) Sleep();
        else if (sleepiness > 10f) StartBlink();
    }
}
