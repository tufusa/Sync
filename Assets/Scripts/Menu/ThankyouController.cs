using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ThankyouController : MonoBehaviour
{
    string tyfp = "THANK  YOU  FOR  PLAYING!!";
    TextMeshPro text;
    List<GameObject> threeEyeses = new List<GameObject>();
    GameObject moon;
    GameObject backToTitle;
    AudioSource audioSource;

    public void Init()
    {
        text = GetComponentInChildren<TextMeshPro>();
        text.text = "";
        moon = transform.Find("Moon").gameObject;
        moon.SetActive(false);
        backToTitle = transform.Find("BackToTitle").gameObject;
        backToTitle.SetActive(false);
        audioSource = GetComponent<AudioSource>();
        foreach(Transform t in transform.Find("3Eyeses").transform)
        {
            threeEyeses.Add(t.gameObject);
            t.gameObject.SetActive(false);
        }
        threeEyeses.Sort((x, y) => (int)(100 * (x.transform.position.x - y.transform.position.x)));
        gameObject.SetActive(false);
    }

    public void SetActive(bool value)
    {
        gameObject.SetActive(value);
        if (value) StartCoroutine(ShowText());
    }

    void Update()
    {
        if(Operation.Continue.GetKeyDown() && backToTitle.activeSelf)
        {
            threeEyeses.ForEach(p => p.SetActive(false));
            moon.SetActive(false);
            backToTitle.SetActive(false);
            audioSource.Stop();
            GameController.Thankyou(false);
            GameController.BackToTitle();
        }
    }

    IEnumerator<WaitForSeconds> ShowText()
    {
        text.text = "";
        int idx = 0;
        int decoyIdx = 0;
        int max = tyfp.Length;
        int maxDecoy = threeEyeses.Count;
        SpriteRenderer panelSp = transform.Find("Panel").GetComponent<SpriteRenderer>();
        threeEyeses.ForEach(p => p.GetComponent<Player>().SetBodyColor(GameController.OppositeColor));
        text.color = GameController.OppositeColor;
        panelSp.color = Color.clear;

        while(true)
        {
            panelSp.color += GameController.BackgroundColor * Time.deltaTime / 3f;
            if (((Vector4)GameController.BackgroundColor - (Vector4)panelSp.color).sqrMagnitude < 0.0001f)
            {
                panelSp.color = GameController.BackgroundColor;
                break;
            }
            yield return null;
        }

        while (true)
        {
            text.text += tyfp[idx];
            if(decoyIdx < maxDecoy) threeEyeses[decoyIdx].SetActive(true);
            if (idx == max) yield break;
            if (tyfp[idx] == ' ')
                yield return null;
            else
            {
                decoyIdx++;
                yield return new WaitForSeconds(0.4f);
            }
            if (++idx == max)
            {
                text.color = GameController.OppositeColor;
                audioSource.Play();
                yield return new WaitForSeconds(0.5f);
                moon.SetActive(true);
                foreach (var t in threeEyeses)
                {
                    Player p = t.GetComponent<Player>();
                    p.StartGradColor(GameController.BackgroundColor, 3f);
                }
                SpriteRenderer playerSp = threeEyeses[0].GetComponentInChildren<SpriteRenderer>();
                SpriteRenderer moonSp = moon.GetComponent<SpriteRenderer>();
                while (playerSp.color != GameController.BackgroundColor)
                {
                    text.color = playerSp.color;
                    moonSp.color = Color.white - playerSp.color + Color.black;
                    yield return new WaitForSeconds(0.01f);
                }
                yield return new WaitForSeconds(3f);
                backToTitle.SetActive(true);
                backToTitle.GetComponent<TextMeshPro>().color = GameController.BackgroundColor;
                backToTitle.GetComponentInChildren<SpriteRenderer>().color = GameController.BackgroundColor;
                yield break;
            }
        }
    }
}
