using UnityEngine;
using System.Collections.Generic;

public class ConveyorController : GimmickController
{
    [SerializeField] Dir dir;
    public Dir GetDir => dir;
    bool isConveyable = true;
    AreaController parentArea;
    Coroutine convey;

    void Awake()
    {
        parentArea = transform.parent.GetComponent<AreaController>();
#if UNITY_EDITOR
        if (dir == Dir.Center || dir == Dir.None)
            Debug.LogError("Unvalid directoin is set in " + gameObject.GetPath() + ".");
#endif
    }

    void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("PlayerSensor"))
        {
            if (collision.name == "Center")
            {
                PlayerController player = collision.GetComponent<PlayerSensor>().Player;
                if (convey == null) convey = StartCoroutine(startConvey(player));
            }
            else
            {
                PlayerSensor sensor = collision.GetComponent<PlayerSensor>();
                if (sensor.GetSensorDir.Opposite() == dir)
                    isConveyable = false;
            }
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.CompareTag("PlayerSensor") && collision.name != "Center")
        {
            PlayerSensor sensor = collision.GetComponent<PlayerSensor>();
            if (sensor.GetSensorDir.Opposite() == dir)
                isConveyable = true;
        }
    }

    IEnumerator<WaitForFixedUpdate> startConvey(PlayerController player)
    {
        yield return new WaitForFixedUpdate();
        yield return new WaitForFixedUpdate();
        if (parentArea.CountChecingRot() == 0 && player.Sensor(dir).IsChecked)
        {
            if (isConveyable) player.Move(dir);
        }
        convey = null;
        yield break;
    }
}
