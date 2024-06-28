using UnityEngine;

public class HiddenFloorController : GimmickController
{
    SpriteRenderer spriteRenderer;
    SwitchController _switch;
    [SerializeField] bool initialHiding = true;
    public SwitchController GetSwitch => _switch;
    bool isHiding;
    public bool GetIsHidnig => isHiding;
    public bool HasPlayer { get; private set; } = false;
    
    void Awake()
    {
        spriteRenderer = transform.GetChild(0).GetComponent<SpriteRenderer>();
        Hide(initialHiding);
    }

    void Hide(bool value)
    {
        isHiding = value;
        spriteRenderer.enabled = !value;
    }

    public void Flip()
    {
        isHiding = !isHiding;
        Hide(isHiding);
    }

    public void SetSwitch(SwitchController sc)
    {
        _switch = sc;
    }

    void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.IsPlayer())
            HasPlayer = true;
    }
    
    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.IsPlayer())
            HasPlayer = false;
    }

    public override void Reset()
    {
        Hide(initialHiding);
    }
}
