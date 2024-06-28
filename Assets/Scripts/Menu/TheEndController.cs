using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TheEndController : MonoBehaviour
{
    List<Player> threeEyeses = new List<Player>();
    [SerializeField] List<TextMeshPro> texts;
    [SerializeField] TextMeshPro time;
    SpriteRenderer panel;
    GameObject backToTitle;

    [SerializeField] ChallengeController challengeController;

    public void Init()
    {
        foreach (Transform threeEyes in transform.Find("3Eyeses"))
        {
            threeEyeses.Add(threeEyes.GetComponent<Player>());
            threeEyes.gameObject.SetActive(false);
        }
        threeEyeses.Sort((x, y) => (int)(100 * (x.transform.position.x - y.transform.position.x)));

        panel = transform.Find("Panel").GetComponent<SpriteRenderer>();
        backToTitle = transform.Find("BackToTitle").gameObject;

        backToTitle.SetActive(false);
        gameObject.SetActive(false);
    }

    public void SetActive(bool value)
    {
        gameObject.SetActive(value);
        if(value) StartCoroutine(ShowText());
    }

    void Update()
    {
        if (Operation.Continue.GetKeyDown() && backToTitle.activeSelf)
        {
            threeEyeses.ForEach(t => t.gameObject.SetActive(false));
            backToTitle.SetActive(false);
            GameController.TheEnd(false);
            GameController.BackToTitle();
        }
    }

    IEnumerator<WaitForSeconds> ShowText()
    {        
        panel.color = GameController.BackgroundColor;
        threeEyeses.ForEach(t => t.SetBodyColor(GameController.BackgroundColor));
        threeEyeses.ForEach(t => t.SetEyeColor(GameController.OppositeColor));
        texts.ForEach(t => t.color = GameController.BackgroundColor);
        backToTitle.GetComponentInChildren<SpriteRenderer>().color = GameController.BackgroundColor;
        time.text = $"time  {challengeController.ClearTime.ToString(@"m\'ss\""ff")}";

        yield return new WaitForSeconds(2f);

        foreach (Player t in threeEyeses)
        {
            t.gameObject.SetActive(true);
            yield return new WaitForSeconds(2f);
        }

        yield return new WaitForSeconds(1f);

        float gradSec = 3f;
        Color targetColor = GameController.OppositeColor;

        while (true)
        {
            Color difColor = targetColor - panel.color;
            panel.color += difColor * Time.deltaTime / gradSec;
            if (((Vector4)(targetColor - panel.color)).sqrMagnitude < 0.001f)
            {
                panel.color = targetColor;
                break;
            }
            yield return null;
        }

        yield return new WaitForSeconds(1f);

        backToTitle.SetActive(true);

        yield break;
    }
}
