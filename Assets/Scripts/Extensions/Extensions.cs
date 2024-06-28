using UnityEngine;

static class Extensions
{
    public static string GetPath(this GameObject gameObject)
    {
        string path = gameObject.name;
        Transform _transform = gameObject.transform;
        while (_transform.parent != null)
        {
            _transform = _transform.parent;
            path = _transform.name + "/" + path;
        }
        return path;
    }

    public static Vector3 GetVector(this Dir dir)
    {
        return dir switch
        {
            Dir.Right => Vector3.right,
            Dir.Left => Vector3.left,
            Dir.Down => Vector3.down,
            Dir.Up => Vector3.up,

            _ => Vector3.zero
        };
    }

    public static Dir Opposite(this Dir dir)
    {
        return dir switch
        { 
            Dir.Right => Dir.Left,
            Dir.Left => Dir.Right,
            Dir.Up => Dir.Down,
            Dir.Down => Dir.Up,

            _ => Dir.None
        };
    }

    public static bool IsPlayer(this Collider2D collision)
    {
        return collision.CompareTag("PlayerSensor") && collision.name == "Center";
    }
}