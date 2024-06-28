using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChallengeController : MonoBehaviour
{
    AudioSource audioSource;
    DateTime startTime;
    DateTime endTime;
    public TimeSpan ClearTime => endTime - startTime;

    void OnEnable()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.Play();
        StartCoroutine(Challenge());
        startTime = DateTime.Now;
    }

    void OnDisable()
    {
        audioSource.Stop();
        StopAllCoroutines();
        endTime = DateTime.Now;
    }

    IEnumerator<WaitForSeconds> Challenge()
    {
        yield return new WaitForSeconds(240f);
        GameController.BackToStageSelect();
        yield break;
    }
}
