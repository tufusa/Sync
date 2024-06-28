using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ZoneController))]
public class ZoneControllerButton : Editor
{
    ZoneController zone;
    void OnEnable()
    {
        zone = target as ZoneController;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if(GUILayout.Button("Add Stage Template"))
        {
            zone.SendMessage("AddStage");
        }
    }
}
