using UnityEngine;

public class PlayerMovement : BaseEntity
{
    public static PlayerMovement Instance { get; private set; }
        
    protected override void Awake()
    {
        base.Awake();

        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }


    /*public void ExecuteMove(Vector3Int targetCell)
    {
        MoveToCell(targetCell);
    }*/
}