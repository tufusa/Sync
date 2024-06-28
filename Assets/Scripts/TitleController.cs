using System;
using System.Collections;
using UnityEngine;

public class TitleController : MonoBehaviour
{
    SpriteRenderer spaceSpriteRenderer;
    SpriteRenderer sync;
    [SerializeField] string watchWord;
    [SerializeField] Sprite spaceBrank;
    [SerializeField] Sprite spaceFilled;

    void Awake()
    {
        spaceSpriteRenderer = transform.Find("Space").GetComponent<SpriteRenderer>();
        sync = transform.Find("SYNC").GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (Operation.Continue.GetKeyDown())
        {
            GameController.StartGame();
        }
    }

    void OnEnable()
    {
        StartCoroutine(Space());
        StartCoroutine(InputSecret());
    }

    void OnDisable()
    {
        StopAllCoroutines();
    }

    IEnumerator Space()
    {
        while(true)
        {
            spaceSpriteRenderer.sprite = spaceBrank;
            yield return new WaitForSeconds(2f);
            spaceSpriteRenderer.sprite = spaceFilled;
            yield return new WaitForSeconds(1.5f);
        }
    }

    IEnumerator InputSecret()
    {
        int i = 0;
        while(true)
        {
            if (Input.anyKeyDown && Input.inputString.Length == 1)
            {
                if (Input.inputString[0] == watchWord[i])
                    i++;
                else
                    i = 0;
                if(i == watchWord.Length)
                {
                    GameController.StartChallenge();
                    yield break;
                }
            }
            yield return null;
        }
    }

    public void SetColor(Color color)
    {
        sync.color = color;
        spaceSpriteRenderer.color = color;
        Array.ForEach(GetComponentsInChildren<DecoyPlayerController>(), p => p.SetBodyColor(color));
        Array.ForEach(GetComponentsInChildren<DecoyPlayerController>(), p => p.SetEyeColor(Color.white - color + Color.black));
    }
}
