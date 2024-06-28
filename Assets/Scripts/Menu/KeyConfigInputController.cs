using System.Collections;
using TMPro;
using UnityEngine;

public class KeyConfigInputController : KeyConfig
{
    TextMeshPro keyText;
    Coroutine underbarCoroutine;

    void Awake()
    {
        keyText = GetComponentInChildren<TextMeshPro>();
        UpdateText();
    }

    void OnEnable()
    {
        if(!GameController.IsTitle)keyText.color = GameController.GetCurrentGrid.GetCurrentZoneColor;
    }

    public void StartConfigure()
    {
        StartConfigure(this);
        if(underbarCoroutine == null) underbarCoroutine = StartCoroutine(Underbar());
    }

    public void UpdateText()
    {
        if (underbarCoroutine != null)
        {
            StopCoroutine(underbarCoroutine);
            underbarCoroutine = null;
        }
        keyText.text = operation.GetKeyName();
    }

    IEnumerator Underbar()
    {
        while (true)
        {
            keyText.text = "_";
            yield return new WaitForSeconds(0.6f);
            keyText.text = "";
            yield return new WaitForSeconds(0.6f);
        }
    }
}
