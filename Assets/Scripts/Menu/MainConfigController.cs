using System.Collections.Generic;
using UnityEngine;

public class MainConfigController : MonoBehaviour
{
    List<SubConfigController> subConfigs;
    ConfigController config;
    Vector3 Pos => transform.localPosition;
    Vector3 PlayerPos => config.ConfigPlayer.transform.localPosition;

    void Awake()
    {
        subConfigs = new List<SubConfigController>(GetComponentsInChildren<SubConfigController>());
        config = GetComponentInParent<ConfigController>();
    }

    public void Select(bool value)
    {
        if (!value && config.activeMainConfig == this) config.activeMainConfig = null;
        if (config.activeMainConfig == null)
        {
            subConfigs.ForEach( s => s.gameObject.SetActive(value));
            if (value) config.activeMainConfig = this;
        }
    }

    void FixedUpdate()
    {
        Select(Mathf.Approximately(Pos.y, PlayerPos.y) || (name == "KeyConfig" && Mathf.Approximately(Pos.x + 0.8f, PlayerPos.x)));
    }
}
