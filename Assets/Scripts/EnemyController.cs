using System.Collections;
using System.Linq;
using Sprites;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [Header("Settings")] [SerializeField] private float moveSpeed = 3f;

    private Vector3Int logicalCellPos;
    private Vector3 targetWorldPos;
    private bool isMoving;

    private void Start()
    {
        // Добавить монстра в обработчик ходов
        TurnManager.Instance.RegisterEnemy(this);
        
        UpdateTargetPosition(logicalCellPos);
        transform.position = targetWorldPos;
    }
    
    public IEnumerator DoTurn()
    {
        Debug.Log("Противник готовится к ходу...");
        yield return new WaitForSeconds(0.1f);

        var bestMove = GetBestMove();
        //Debug.Log(bestMove.HasValue);
        //Debug.Log(bestMove.Value != logicalCellPos);
        if (bestMove.HasValue && bestMove.Value != logicalCellPos)
        {
            var oldPos = logicalCellPos;
            logicalCellPos = bestMove.Value;
            GridManager.Instance.MoveEntity(oldPos, logicalCellPos, gameObject);
            UpdateTargetPosition(logicalCellPos);

            isMoving = true;
            while (isMoving)
            {
                MoveSmoothly();
                yield return null;
            }
        }
        
        yield return new WaitForSeconds(0.1f);

        Debug.Log("Противник закончил ход");
    }
    
    private Vector3Int? GetBestMove()
    {
        //if (PlayerMovement.Instance == null) return null;
    
        var possibleMoves = GridManager.Instance.GetWalkableTilesInRange(logicalCellPos, 1, gameObject);
        return possibleMoves
            .OrderBy(pos => Vector3
                //.Distance(pos, PlayerMovement.Instance.CurrentCell))
                .Distance(pos, AbilityController.Instance.PlayerPosition))
            .Cast<Vector3Int?>()
            .FirstOrDefault();
    }

    private void MoveSmoothly()
    {
        transform.position = Vector3.MoveTowards(transform.position, targetWorldPos, moveSpeed * Time.deltaTime);
        //Debug.Log(Vector3.Distance(transform.position, targetWorldPos));
        if (Vector3.Distance(transform.position, targetWorldPos) < 0.01f)
        {
            transform.position = targetWorldPos;
            isMoving = false;
        }
    }

    private void UpdateTargetPosition(Vector3Int cell)
    {
        targetWorldPos = GridManager.Instance.GetCellCenterWorld(cell);
        targetWorldPos.z = transform.position.z;
    }
    
    private void Awake()
    {
        logicalCellPos = GridManager.Instance.WorldPointToCell(transform.position);
        GridManager.Instance.RegisterFixedEntity(logicalCellPos, gameObject);
    }
}