using System.Collections.Generic;
using UnityEngine;

public class SwitchController : GimmickController
{
    bool isActivated;
    bool isChenged = false;
    [SerializeField] List<HiddenFloorController> hiddenFloors;
    [SerializeField] Sprite switchTrue;
    [SerializeField] Sprite switchFalse;
    [SerializeField] bool initialValue = false;
    SpriteRenderer spriteRenderer;
    Dir sensorDir = Dir.None;
    public Dir GetSensorDir => sensorDir;
    AreaController parentArea;
    Coroutine switchCor;

    void Awake()
    {
        isActivated = initialValue;
        spriteRenderer = GetComponent<SpriteRenderer>();
        SwitchSprite(isActivated);
        hiddenFloors.ForEach(f => f.SetSwitch(this));
        parentArea = transform.parent.GetComponent<AreaController>();
    }

    void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("PlayerSensor"))
        {
            if (switchCor == null) switchCor = StartCoroutine(StartSwitch(collision));
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.IsPlayer())
        {
            isChenged = false;
        }
        sensorDir = Dir.None;
        if (switchCor != null) StopCoroutine(switchCor);
        switchCor = null;
    }

    void FlipFloors()
    {
        hiddenFloors.ForEach(f => f.Flip());
    }

    void SwitchSprite(bool isActivated)
    {
        if (isActivated)
            spriteRenderer.sprite = switchTrue;
        else
            spriteRenderer.sprite = switchFalse;
    }

    IEnumerator<WaitForFixedUpdate> StartSwitch(Collider2D collision)
    {
        sensorDir = collision.GetComponent<PlayerSensor>().GetSensorDir;
        yield return new WaitForFixedUpdate();
        if (!isChenged && parentArea.CountChecingRot() == 0 && collision.name == "Center" && hiddenFloors.TrueForAll(f => !f.HasPlayer))
        {
            isChenged = true;
            isActivated = !isActivated;
            FlipFloors();
            SwitchSprite(isActivated);
        }
        switchCor = null;
        yield break;
    }

    public override void Reset()
    {
        isActivated = initialValue;
        SwitchSprite(isActivated);
        if (switchCor != null) StopCoroutine(switchCor);
        switchCor = null;
    }
}
