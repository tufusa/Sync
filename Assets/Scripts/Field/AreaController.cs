using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class AreaController : FieldController
{
    List<PlayerController> players;
    List<GimmickController> gimmicks;
    List<RotateController> rots;
    List<ConveyorController> conveyors;
    public Tilemap Tilemap { get; private set; }
    StageController stage;
    public TilemapCollider2D CheckerCollider { get; private set; }
    [SerializeField] Tile nullTile;
    public int WaitFrame { get; private set; }

    public ReadOnlyCollection<PlayerController> GetPlayers => players.AsReadOnly();
    public ReadOnlyCollection<ConveyorController> GetConveyors => conveyors.AsReadOnly();
    Vector3 initPos;

    public override void Init()
    {
        initPos = transform.position;
        players = new List<PlayerController>(GetComponentsInChildren<PlayerController>());
        gimmicks = new List<GimmickController>(GetComponentsInChildren<GimmickController>());
        rots = new List<RotateController>(GetComponentsInChildren<RotateController>());
        conveyors = new List<ConveyorController>(GetComponentsInChildren<ConveyorController>());
        Tilemap = GetComponent<Tilemap>();
        stage = transform.parent.GetComponent<StageController>();
        gimmicks.ForEach(g => SetNullTile(g.transform.position));
        CheckerCollider = CreateCheckerCollider();
        gimmicks.ForEach(g => RemoveNullTile(g.transform.position));
    }

    void FixedUpdate()
    {
        if (WaitFrame > 0) WaitFrame--;
    }

    public void AddPlayer(PlayerController player)
    {
        if (!players.Contains(player))
        {
            players.Add(player);
            player.transform.SetParent(transform);
        }
    }

    public void RemovePlayer(PlayerController player)
    {
        int idx = players.IndexOf(player);
        if (idx != -1)
        {
            players.RemoveAt(idx);
        }
    }

    public void SetNullTile(Vector3 worldPos)
    {
        Tilemap.SetTile(Tilemap.WorldToCell(worldPos), nullTile);
    }

    public void RemoveNullTile(Vector3 worldPos)
    {
        Tilemap.SetTile(Tilemap.WorldToCell(worldPos), null);
    }

    TilemapCollider2D CreateCheckerCollider()
    {
        GameObject cc = Instantiate(gameObject, transform);
        DestroyImmediate(cc.GetComponent<TilemapRenderer>());
        DestroyImmediate(cc.GetComponent<AreaController>());
        foreach (Transform ch in cc.transform) Destroy(ch.gameObject);
        cc.name = "ColliderChecker";
        cc.tag = "Untagged";
        cc.transform.localPosition = Vector3.zero;
        cc.AddComponent<CheckerColliderController>();
        return cc.GetComponent<TilemapCollider2D>();
    }

    public bool IsRotatable()
    {
        if (CountChecingRot() > 1) return false; // “¯Žž‚É2‚ÂˆÈã‚Ìƒ}ƒX‚Å‰ñ“]‚µ‚æ‚¤‚Æ‚µ‚Ä‚¢‚é‚È‚ç•s‰Â
        foreach (var a in stage.GetAreas)
        {
            if (CheckerCollider.IsTouching(a.CheckerCollider)) return false;
        }
        return true;
    }

    public int CountChecingRot()
    {
        return rots.Count(r => r.IsCheckingRot);
    }

    public void StartCheckRot()
    {
        WaitFrame = 1;
    }

    public void FinishCheckRot()
    {
        WaitFrame = -1;
    }

    public void Reset()
    {
        transform.position = initPos;
        foreach (var c in conveyors) c.transform.rotation = Quaternion.identity;
    }

#if UNITY_EDITOR
    void Awake()
    {
        GetComponent<Tilemap>().CompressBounds();
    }

    string GetPath(Transform transform)
    {
        if (transform.parent == null) return transform.name;
        return GetPath(transform.parent) + "/" + transform.name;
    }

    void ResetArea()
    {
        UnityEditor.PrefabUtility.RevertObjectOverride(GetComponent<Tilemap>(), UnityEditor.InteractionMode.UserAction);
        while(transform.childCount > 0)
        {
            GameObject child = transform.GetChild(transform.childCount - 1).gameObject;
            UnityEditor.Undo.DestroyObjectImmediate(child);
        }
    }
#endif
}
