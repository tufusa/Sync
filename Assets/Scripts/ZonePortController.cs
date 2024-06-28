using UnityEngine;

public class ZonePortController : MonoBehaviour
{
    [SerializeField] int zoneIdx;
    [SerializeField] int needCleadZoneCount;
    [SerializeField] Sprite activePort;
    [SerializeField] Sprite inactiveSprite;
    SpriteRenderer spriteRenderer;
    bool isSelected;
    bool isActive;
    Color zoneColor;

    void Awake()
    {
        zoneColor = GameController.GetCurrentGrid.GetColor(zoneIdx);
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void Load()
    {
        if (GameController.GetCurrentGrid.ClearZoneCount() >= needCleadZoneCount)
        {
            isActive = true;
            spriteRenderer.sprite = activePort;
            spriteRenderer.color = zoneColor;
        }
        else
        {
            isActive = false;
            spriteRenderer.sprite = inactiveSprite;
            spriteRenderer.color = GameController.GetCurrentGrid.GetCurrentZoneColor;
        }
    }

    void Update()
    {
        if (isActive && isSelected && !GameController.IsPausing && Operation.Continue.GetKeyDown())
        {
            GameController.SetStageSelectPlayerColor(zoneColor, Color.white);
            GameController.GetCurrentGrid.UnloadZone();
            if (zoneIdx == 9) 
                GameController.StartChallenge();
            else 
                GameController.GetCurrentGrid.LoadZone(zoneIdx);
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (isActive && collision.IsPlayer())
        {
            collision.GetComponent<PlayerSensor>().Player.StartGradColor(zoneColor, 0.4f);
            isSelected = true;
        }
    }
    
    void OnTriggerExit2D(Collider2D collision)
    {
        if (isActive && collision.IsPlayer())
        {
            collision.GetComponent<PlayerSensor>().Player.StartGradColor(GameController.OppositeColor,0.4f);
            isSelected = false;
        }
    }
}
