using UnityEngine;

public class CheckerColliderController : MonoBehaviour
{
    AreaController area;
    void Awake()
    {
        area = transform.parent.GetComponent<AreaController>(); 
    }

    void OnTriggerStay2D(Collider2D collision)
    {
        if(collision.IsPlayer())
        {
            area.AddPlayer(collision.GetComponent<PlayerSensor>().Player);
        }
    }
    
    void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.IsPlayer() && area.CountChecingRot() == 0)
        {
            area.RemovePlayer(collision.GetComponent<PlayerSensor>().Player);
        }
    }
}
