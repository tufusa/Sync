using System.Collections;
using System.Collections.ObjectModel;
using UnityEngine;

public class RotateController : GimmickController
{
    [SerializeField] Sprite rot90;
    [SerializeField] Sprite rot180;
    [SerializeField] Sprite rot270;
    [SerializeField] Angle angle = Angle.d90;
    [SerializeField] RotDir dir = RotDir.CCW;
    AreaController area;
    SpriteRenderer spriteRenderer;
    ReadOnlyCollection<PlayerController> Players => area.GetPlayers;
    ReadOnlyCollection<ConveyorController> Conveyors => area.GetConveyors;
    bool isChanged = false;
    public bool IsCheckingRot { get; private set; } = false;

    void Awake()
    {
        area = transform.parent.GetComponent<AreaController>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        SetSprite();
    }

    void OnTriggerStay2D(Collider2D collision)
    {
        if (!isChanged && collision.IsPlayer())
        {
            StartCheckRotate();
            isChanged = true;
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.IsPlayer())
        {
            isChanged = false;
            IsCheckingRot = false;
            StopAllCoroutines();
        }
    }

    void StartCheckRotate()
    {
        GameController.SetPlayerControllable(false);
        area.CheckerCollider.transform.localPosition = Vector3.zero;
        area.CheckerCollider.transform.rotation = area.transform.rotation;
        transform.SetParent(GameController.GetCurrentStage.transform); // ���̃}�X��Area����O��
        area.CheckerCollider.transform.SetParent(transform); // Area��CheckerCollider�����̃}�X�̎q�ɂ���
        transform.Rotate(0f, 0f, (int)dir * (float)angle); // ���̃}�X����]
        area.CheckerCollider.transform.SetParent(area.transform); // Area��CheckerCollider��Area�̎q�ɖ߂�
        transform.Rotate(0f, 0f, -(int)dir * (float)angle); // ���̃}�X���t��]
        transform.SetParent(area.transform); // ���̃}�X��Area�̎q�ɖ߂�
        area.StartCheckRot();
        StartCoroutine(CheckRot());
    }

    void FinishCheckRotate(bool isRotatable)
    {
        IsCheckingRot = false;
        transform.SetParent(GameController.GetCurrentStage.transform); // ���̃}�X��Area����O��
        transform.Rotate(0f, 0f, (int)dir * (float)angle); // ���̃}�X����]
        area.CheckerCollider.transform.SetParent(transform); // Area��CheckerCollider�����̃}�X�̎q�ɂ���
        transform.Rotate(0f, 0f, -(int)dir * (float)angle); // ���̃}�X���t��]
        area.CheckerCollider.transform.SetParent(area.transform); // Area��CheckerCollider��Area�̎q�ɖ߂�
        area.CheckerCollider.transform.localPosition = Vector3.zero;
        area.CheckerCollider.transform.rotation = area.transform.rotation;
        transform.SetParent(area.transform); // ���̃}�X��Area�̎q�ɖ߂�
        area.FinishCheckRot();
        if (isRotatable) Rotate();
        GameController.SetPlayerControllable(true);
        foreach (var p in area.GetPlayers) p.ResetColliders();
    }

    void Rotate()
    {
        transform.SetParent(GameController.GetCurrentStage.transform); // ���̃}�X��Area����O��
        area.transform.SetParent(transform); // Area�����̃}�X�̎q�ɂ���
        transform.Rotate(0f, 0f, (int)dir * (float)angle); // ���̃}�X����]
        foreach (var p in Players) p.transform.Rotate(0f, 0f, -(int)dir * (float)angle); // player���t��]
        foreach (var c in Conveyors) c.transform.Rotate(0f, 0f, -(int)dir * (float)angle); // conveyor���t��]
        area.transform.SetParent(GameController.GetCurrentStage.transform); // Area��Stage�̎q�ɖ߂�
        transform.SetParent(area.transform); // ���̃}�X��Area�̎q�ɖ߂�
    }

    void SetSprite()
    {
        if (angle == Angle.d90)
            spriteRenderer.sprite = rot90;
        else if (angle == Angle.d180)
            spriteRenderer.sprite = rot180;
        else if (angle == Angle.d270)
            spriteRenderer.sprite = rot270;

        if (dir == RotDir.CCW)
            spriteRenderer.flipX = false;
        else if (dir == RotDir.CW)
            spriteRenderer.flipX = true;
    }

    IEnumerator CheckRot()
    {
        IsCheckingRot = true;
        while(true)
        {
            if (area.WaitFrame == 0)
            {
                FinishCheckRotate(area.IsRotatable());
                yield break;
            }
            else if (area.WaitFrame == -1)
            {
                FinishCheckRotate(false);
                yield break;
            }
            yield return null;
        }
    }

    public override void Reset()
    {
        transform.SetParent(GameController.GetCurrentStage.transform); // ���̃}�X��Area����O��
        area.transform.SetParent(transform); // Area�����̃}�X�̎q�ɂ���
        transform.rotation = Quaternion.identity; // ���̃}�X�̉�]��߂�
        foreach (var p in Players) p.InitParent(); // Area���player�����̐e�ɖ߂�
        area.transform.SetParent(GameController.GetCurrentStage.transform); // Area��Stage�̎q�ɖ߂�
        transform.SetParent(area.transform); // ���̃}�X��Area�̎q�ɖ߂�

        isChanged = false;
    }
}

enum Angle
{
    d90 = 90, d180 = 180, d270 = 270
}

enum RotDir
{ 
    CW = -1, CCW = 1
}
