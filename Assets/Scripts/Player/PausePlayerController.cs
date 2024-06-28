using UnityEngine;

public class PausePlayerController : Player
{
    PauseController pause;
    string floorName;

    void Awake()
    {
        Init();
        pause = GetComponentInParent<PauseController>();
    }

    void Update()
    {
        if(Operation.Continue.GetKeyDown())
        {
            switch (floorName)
            {
                case "Cross":
                    GameController.Quit();
                    break;
                case "Back":
                    pause.Back();
                    break;
                case "Config":
                    pause.Config();
                    break;
            }
        }

        if(Operation.Continue.GetKeyUp())
        {
            switch(floorName)
            {
                case "Continue":
                    pause.Continue();
                    break;
            }
        }
    }


    public void Move(Dir dir)
    {
        if ((dir == Dir.Right && floorName != "Continue") || (dir == Dir.Left && floorName != "Cross"))
        {
            Turn(dir);
            transform.position += dir.GetVector() * 0.8f;
        }
    }

    void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Pause")) floorName = collision.name;
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Pause")) floorName = null;
    }

    public override void Reset()
    {
        base.Reset();
        floorName = null;
    }
}
