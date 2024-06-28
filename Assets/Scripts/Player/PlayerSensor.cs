using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerSensor : MonoBehaviour
{
    bool hasNeighbor = false;
    bool canMove = false;
    bool isClosed = false;
    string floorName = "";
    Tilemap tilemap;
    public bool GetCanMove => canMove;
    public string GetFloorName => floorName;
    [SerializeField] Dir sensorDir;
    public Dir GetSensorDir => sensorDir;
    Dir switchSensorDir;
    public PlayerController Player { get; private set; }
    public bool IsChecked { get; private set; }
    public BoxCollider2D collision { get; private set; }

    void Start()
    {
        Player = transform.parent.GetComponent<PlayerController>();
        collision = GetComponent<BoxCollider2D>();
    }

    void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.IsPlayer())
        {
            if(sensorDir == Dir.Center)
            {
                GameController.Reset();
                return;
            }

            hasNeighbor = true;
            PlayerSensor nextPlayerSensor = collision.GetComponent<PlayerSensor>().Player.Sensor(sensorDir);

            if (nextPlayerSensor.floorName != null)
            {
                floorName = "";
                if (!isClosed) SetCanMove(nextPlayerSensor.GetCanMove);
                else SetCanMove(false);
            }
            else
            {
                SetCanMove(false);
            }
        }
        else if (!hasNeighbor && collision.CompareTag("Area"))
        {
            SetCanMove(true);
            tilemap = collision.GetComponent<Tilemap>();
            if (LayerMask.NameToLayer("Config") != collision.gameObject.layer)
            {
                try
                {
                    floorName = tilemap.GetTile(tilemap.WorldToCell(transform.position)).name;
                }
                catch (System.NullReferenceException)
                {
                    floorName = null;
                }
            }
        }
        else if (collision.CompareTag("Gimmick"))
        {
            floorName = collision.name;
            if (collision.name == "HiddenFloor")
            {
                try
                {
                    switchSensorDir = collision.GetComponent<HiddenFloorController>().GetSwitch.GetSensorDir;
                }
                catch (System.NullReferenceException)
                {
                    switchSensorDir = Dir.None;
                }

                if (sensorDir != Dir.Center && switchSensorDir == sensorDir)
                    SetCanMove(false);
                else
                {
                    if (collision.GetComponent<HiddenFloorController>().GetIsHidnig)
                    {
                        SetCanMove(false);
                        isClosed = true;
                    }
                    else
                        SetCanMove(true);
                }
            }
            else if (collision.TryGetComponent(out ConveyorController conveyor))
            {
                isClosed = conveyor.GetDir.Opposite() == sensorDir;
                if (sensorDir != Dir.Center && !hasNeighbor)
                    SetCanMove(!isClosed);
            }
            else if(!hasNeighbor)
            {
                SetCanMove(true);
            }
        }
        else if(collision.CompareTag("Config"))
        {
            floorName = collision.name;
            if (collision.TryGetComponent(out SubConfigController _))
            {
                SetCanMove(true);
            }
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Area") || collision.CompareTag("Gimmick"))
        {
            canMove = false;
            isClosed = false;
            StartWaitCheck();
            floorName = "";
        }
        else if (collision.IsPlayer())
            hasNeighbor = false;     
    }

    IEnumerator<WaitForFixedUpdate> WaitCheck()
    {
        IsChecked = false;
        yield return new WaitForFixedUpdate();
        IsChecked = true;
        yield break;
    }

    void StartWaitCheck()
    {
        if(gameObject.activeInHierarchy) StartCoroutine(WaitCheck());
    }

    void SetCanMove(bool value)
    {
        canMove = value;
    }

    public void Reset()
    {
        floorName = null;
        canMove = false;
        hasNeighbor = false;
        isClosed = false;
        StartWaitCheck();
    }
}
