using System;
using UnityEngine;

public class PlayerController : Player
{
    string floorName;
    public string GetFloorName => floorName;
    PlayerSensor[] sensors;
    Transform initParent;
    Collider2D[] overlap;

    public override void Init()
    {
        base.Init();
        initParent = transform.parent;
        sensors = new PlayerSensor[Enum.GetValues(typeof(Dir)).Length - 1];
        foreach (Dir dir in Enum.GetValues(typeof(Dir)))
        {
            if (dir == Dir.None) continue;
            sensors[(int)dir] = transform.Find(dir.ToString()).GetComponent<PlayerSensor>();
        }

        InitTurn();

        overlap = new Collider2D[100];
    }

    void Update()
    {
        floorName = Sensor(Dir.Center).GetFloorName;
    }

    void FixedUpdate()
    {
        sleepiness += Time.deltaTime;
        if (GameController.GetStageCleared) sleepiness = 0f;

        if (sleepiness > 60f) Sleep();
        else if (sleepiness > 10f) StartBlink();

        if (Sensor(Dir.Center).collision.OverlapCollider(new ContactFilter2D().NoFilter(), overlap) == 0 || transform.eulerAngles != Quaternion.identity.eulerAngles)
            GameController.Reset();
    }

    public void Move(Dir dir)
    {
        if (dir == Dir.Center || dir == Dir.None) return;

        if(!GameController.GetStageCleared) ShakeOff();

        Turn(dir);

        if (Sensor(dir).GetCanMove)
        {
            transform.position += 0.8f * dir.GetVector();
            ResetColliders();
        }
    }

    public override void Reset()
    {
        base.Reset();
        floorName = null;
        Motion(Eyes.Open, Legs.Stand);
        InitTurn();
        ResetColliders();
        StopAllCoroutines();
    }

    public PlayerSensor Sensor(Dir dir)
    {
        return sensors[(int)dir];
    }

    public void InitParent()
    {
        transform.SetParent(initParent);
    }

    public void ResetColliders()
    {
        Array.ForEach(sensors, s => s.Reset());
    }
}
