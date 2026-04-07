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
        logicalCellPos = GridManager.Instance.WorldToCell(transform.position);
        GridManager.Instance.RegisterFixedEntity(logicalCellPos, gameObject);
        
        UpdateTargetPosition(logicalCellPos);
        transform.position = targetWorldPos;
    }
    
    private void OnDisable()
    {
        if (TurnManager.Instance != null)
        {
            TurnManager.Instance.UnregisterEnemy(this);
        }
    }
    
    public IEnumerator DoTurn()
    {
        Debug.Log("Противник готовится к ходу...");
        yield return new WaitForSeconds(0.1f);

        var bestMove = GetBestMove();

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

        TryAttackPlayer();
        
        yield return new WaitForSeconds(0.1f);

        Debug.Log("Противник закончил ход");
    }
    
    private void TryAttackPlayer()
    {
        if (PlayerMovement.Instance == null) return;

        var playerCell = PlayerMovement.Instance.CurrentCell;
        
        Debug.Log("Attack");
        
        if (Vector3Int.Distance(logicalCellPos, playerCell) <= 1.1f)
        {
            if (PlayerMovement.Instance.TryGetComponent<Health>(out var playerHealth))
            {
                StartCoroutine(PunchAnimation(PlayerMovement.Instance.transform.position));
            
                Debug.Log($"<color=red>{gameObject.name} атакует игрока</color>");
                playerHealth.TakeDamage(2);
            }
        }
    }

    private IEnumerator PunchAnimation(Vector3 targetPos)
    {
        var startPos = transform.position;
        var punchPos = Vector3.Lerp(startPos, targetPos, 0.3f);
    
        var elapsed = 0f;
        var duration = 0.15f;

        // Идет вперед
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            transform.position = Vector3.Lerp(startPos, punchPos, elapsed / duration);
            yield return null;
        }

        // Возвращается назад
        elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            transform.position = Vector3.Lerp(punchPos, startPos, elapsed / duration);
            yield return null;
        }

        transform.position = startPos;
    }
    
    private Vector3Int? GetBestMove()
    {
        if (PlayerMovement.Instance == null) return null;
        
        var playerCell = PlayerMovement.Instance.CurrentCell;
        var possibleMoves = GridManager.Instance.GetWalkableTilesInRange(logicalCellPos, 1, gameObject);
        
        if (Vector3Int.Distance(logicalCellPos, playerCell) <= 1.1f)
        {
            return logicalCellPos; 
        }
    
        return possibleMoves
            .OrderBy(pos => Vector3
                .Distance(pos, PlayerMovement.Instance.CurrentCell))

            .Cast<Vector3Int?>()
            .FirstOrDefault();
    }

    private void MoveSmoothly()
    {
        transform.position = Vector3.MoveTowards(transform.position, targetWorldPos, moveSpeed * Time.deltaTime);
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
}