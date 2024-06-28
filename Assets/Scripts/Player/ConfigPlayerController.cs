using System;
using UnityEngine;

public class ConfigPlayerController : Player
{
    string floorName;
    PlayerSensor[] sensors;
    SubConfigController subConfigFloor;
    public bool OnKeyConfig { get; private set; }

    void Awake()
    {
        Init();
    }

    void Start()
    {
        sensors = new PlayerSensor[Enum.GetValues(typeof(Dir)).Length - 1];
        foreach (Dir dir in Enum.GetValues(typeof(Dir)))
        {
            if (dir == Dir.None) continue;
            sensors[(int)dir] = transform.Find(dir.ToString()).GetComponent<PlayerSensor>();
        }

        InitTurn();
    }

    void Update()
    {
        floorName = Sensor(Dir.Center).GetFloorName;
        if (!GameController.IsKeyConfig && Operation.Continue.GetKeyDown()){
            if (floorName == "BackToPause") GameController.Config(false);
            else if (subConfigFloor != null)
                subConfigFloor.Configurate();
        }
    }

    void FixedUpdate()
    {
        sleepiness += Time.deltaTime;
        if (GameController.GetStageCleared) sleepiness = 0f;

        if (sleepiness > 60f) Sleep();
        else if (sleepiness > 10f) StartBlink();
    }

    public void Move(Dir dir)
    {
        if (dir == Dir.Center || dir == Dir.None) return;

        ShakeOff();

        Turn(dir);

        if (Sensor(dir).GetCanMove)
        {
            transform.position += 0.8f * dir.GetVector();
        }

        ResetColliders();
    }

    public PlayerSensor Sensor(Dir dir)
    {
        return sensors[(int)dir];
    }

    public override void Reset()
    {
        base.Reset();
        floorName = null;
        Motion(Eyes.Open, Legs.Stand);
        ResetColliders();
    }

    void ResetColliders()
    {
        if(sensors != null) Array.ForEach(sensors, s => s.Reset());
    }

    void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out SubConfigController subConfig))
            subConfigFloor = subConfig;
        if (collision.TryGetComponent(out KeyConfigInputController _))
            OnKeyConfig = true;
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out SubConfigController _))
            subConfigFloor = null;
        if (collision.TryGetComponent(out KeyConfigInputController _))
            OnKeyConfig = false;
    }
}
