using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] InitDir initialDir = InitDir.Right;
    [SerializeField] char blinkChar = ' ';
    [SerializeField] Sprite playerEyeOpen;
    [SerializeField] Sprite playerEyeClose;
    [SerializeField] float blinkSec = 0.1f;
    GameObject playerBody;
    SpriteRenderer body;
    SpriteRenderer legs;
    SpriteRenderer eyes;
    protected float sleepiness = 0f;
    List<int> morse;
    Coroutine blinkCoroutine;
    Coroutine squatCoroutine;
    Coroutine gradColorCoroutine;
    Vector3 initPos;
    Quaternion initRot;
    public Color Color => body.color;

    public virtual void Init()
    {
        playerBody = transform.Find("Body").gameObject;
        body = playerBody.GetComponent<SpriteRenderer>();
        eyes = body.transform.Find("Eyes").GetComponent<SpriteRenderer>();
        legs = transform.Find("Legs").GetComponent<SpriteRenderer>();
        morse = blinkChar.GetMorse();
        initPos = transform.position;
        initRot = transform.rotation;
    }

    protected void ShakeOff()
    {
        sleepiness = 0f;
        StopBlink();
        Motion(Eyes.Open, Legs.Stand);
    }

    protected void StartBlink()
    {
        if (blinkCoroutine == null) blinkCoroutine = StartCoroutine(Blink());
    }

    protected void StopBlink()
    {
        if(blinkCoroutine != null) StopCoroutine(blinkCoroutine);
        blinkCoroutine = null;
    }

    IEnumerator Blink()
    {
        int idx = 0;
        int count = morse.Count;
        while(true)
        {
            if(idx == count)
            {
                idx = 0;
                yield return new WaitForSeconds(2f);
                continue;
            }

            Motion(eyes: Eyes.Close);
            if (morse[idx] == 0) yield return new WaitForSeconds(blinkSec * 3);
            else if (morse[idx] == 1) yield return new WaitForSeconds(blinkSec);

            Motion(eyes: Eyes.Open);
            yield return new WaitForSeconds(0.2f);

            idx++;
        }
    }

    protected void Sleep()
    {
        StopBlink();
        Motion(Eyes.Close, Legs.Sit);
    }

    public void StartSquat()
    {
        if(squatCoroutine == null) squatCoroutine = StartCoroutine(Squat());
    }

    protected void StopSquat()
    {
        if (squatCoroutine != null) StopCoroutine(squatCoroutine);
    }


    IEnumerator Squat()
    {
        StopBlink();
        while(true)
        {
            yield return new WaitForSeconds(0.3f);
            Motion(legs: Legs.Sit);
            yield return new WaitForSeconds(0.3f);
            Motion(legs: Legs.Stand);
        }
    }

    protected void Motion(Eyes eyes = Eyes.Unchanged, Legs legs = Legs.Unchanged)
    {
        switch (eyes)
        {
            case Eyes.Open:
                body.sprite = playerEyeOpen;
                break;
            case Eyes.Close:
                body.sprite = playerEyeClose;
                break;
        }
        switch (legs)
        {
            case Legs.Stand:
                playerBody.transform.localPosition = Vector3.zero;
                break;
            case Legs.Sit:
                playerBody.transform.localPosition = 0.155f * Vector3.down;
                break;
        }
    }

    public void SetBodyColor(Color color)
    {
        body.color = color;
        legs.color = color;
    }

    public void SetEyeColor(Color color)
    {
        eyes.color = color;
    }

    public void StartGradColor(Color color, float gradSec)
    {
        if (gradColorCoroutine != null) StopCoroutine(gradColorCoroutine);
        if (gameObject.activeInHierarchy) gradColorCoroutine = StartCoroutine(GradColor(color, gradSec));
    }

    IEnumerator GradColor(Color color, float gradSec)
    {
        Color difColor = color - body.color;
        while(true) {
            body.color += difColor * Time.deltaTime / gradSec;
            legs.color += difColor * Time.deltaTime / gradSec;
            if (((Vector4)(color - body.color)).sqrMagnitude < 0.001f)
            {
                gradColorCoroutine = null;
                SetBodyColor(color);
                yield break;
            }
            yield return null;
        }
    }

    protected void Turn(Dir dir)
    {
        switch (dir)
        {
            case Dir.Left:
                if (body == null) Debug.Log(gameObject.GetPath());
                body.flipX = true;
                break;
            case Dir.Right:
                body.flipX = false;
                break;
        }
    }

    protected void InitTurn()
    {
        Turn((Dir)initialDir);
    }

    public virtual void Reset()
    {
        sleepiness = 0f;
        StopAllCoroutines();
        blinkCoroutine = null;
        squatCoroutine = null;
        gradColorCoroutine = null;
        transform.SetPositionAndRotation(initPos, initRot);
        InitTurn();
    }

    protected enum Eyes
    {
        Open, Close, Unchanged
    }

    protected enum Legs
    {
        Stand, Sit, Unchanged
    }
    enum InitDir
    {
        Left, Right
    }
}

public enum Dir
{
    Left, Right, Up, Down, Center, None
}
