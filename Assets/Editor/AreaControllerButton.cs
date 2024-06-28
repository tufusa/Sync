using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(AreaController))]
public class AreaControllerButton : Editor
{
    AreaController area;
    void OnEnable()
    {
        area = target as AreaController;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if(GUILayout.Button("Reset Area"))
        {
            area.SendMessage("ResetArea");
        }
    }
}