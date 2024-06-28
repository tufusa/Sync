using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(GridController))]
public class GridControllerButton : Editor
{
    GridController grid;
    void OnEnable()
    {
        grid = target as GridController;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (GUILayout.Button("Add Zone Template"))
        {
            grid.SendMessage("AddZone");
        }
    }
}
