using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(StageController))]
public class StageControllerButton : Editor
{
    StageController stage;
    void OnEnable()
    {
        stage = target as StageController;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (GUILayout.Button("Add Area Template"))
        {
            stage.SendMessage("AddArea");
        }
    }
}
