using UnityEngine;

public static class KeyExtensions
{
    public static KeyCode GetKeyCode(this Operation operation)
    {
        return KeyConfig.GetKeyCode(operation);
    }

    public static bool GetKeyDown(this Operation operation)
    {
        return Input.GetKeyDown(operation.GetKeyCode());
    }


    public static bool GetKeyUp(this Operation operation)
    {
        return Input.GetKeyUp(operation.GetKeyCode());
    }

    public static Dir GetDir(this Operation operation)
    {
        return operation switch
        {
            Operation.Right => Dir.Right,
            Operation.Left => Dir.Left,
            Operation.Up => Dir.Up,
            Operation.Down => Dir.Down,

            _ => Dir.None
        };
    }

    public static string GetKeyName(this Operation operation)
    {
        return operation.GetKeyCode().ToString();
    }
}
