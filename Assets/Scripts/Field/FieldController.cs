using UnityEngine;

[ExecuteInEditMode]
public class FieldController : MonoBehaviour
{
    public virtual void Init() { }

    public virtual void SetActive(bool value)
    {
        gameObject.SetActive(value);
    }
}
