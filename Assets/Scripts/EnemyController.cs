using System.Collections;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    private void Start() 
    {
        if (TurnManager.Instance != null)
            TurnManager.Instance.OnStateChanged += ExecuteTurn;
    }

    private void OnDisable() 
    {
        if (TurnManager.Instance != null)
            TurnManager.Instance.OnStateChanged -= ExecuteTurn;
    }

    private void ExecuteTurn(TurnState state)
    {
        if (state == TurnState.EnemyTurn)
        {
            StartCoroutine(EnemyRoutine());
        }
    }

    private IEnumerator EnemyRoutine()
    {
        Debug.Log("Противник готовится к ходу...");
        yield return new WaitForSeconds(1f);
        
        transform.position += Vector3.right;
        
        Debug.Log("Противник закончил ход");
        TurnManager.Instance.SetState(TurnState.PlayerTurn);
    }
}
