using UnityEngine;

public class PlayerDrawer : MonoBehaviour
{
    public static PlayerDrawer Instance;


    private void Awake()
    {
        Instance = this;
    }

    //private void Start()
    //{
    //    spriteRenderer = GetComponent<SpriteRenderer>();
    //}
    // Update is called once per frame
    void Update()
    {
        transform.position = AbilityController.Instance.PlayerPosition;
        

        //if (Transfor) ?????????????
        /*var directionX = selectedCell.x - playerPosition.x;
        UpdateSpriteFlip(directionX);
        SetNewTarget(selectedCell);*/
    }


    //private void UpdateSpriteFlip(int horizontalDirection)
    //{
    //    if (horizontalDirection != 0)
    //        spriteRenderer.flipX = horizontalDirection < 0;
    //}

    //private void MoveSmoothly()
    //{
    //    transform.position = Vector3.MoveTowards(transform.position, targetWorldPos, moveSpeed * Time.deltaTime);
    //    isMoving = Vector3.Distance(transform.position, targetWorldPos) > 0.01f;
    //}

    //private void UpdateTargetPosition(Vector3Int cell)
    //{
    //    targetWorldPos = GridManager.Instance.GetCellCenterWorld(cell);
    //    targetWorldPos.z = transform.position.z;
    //}
}
