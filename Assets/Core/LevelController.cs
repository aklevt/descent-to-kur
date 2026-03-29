using UnityEngine;

public class LevelController : MonoBehaviour 
{
    private void Start() 
    {
        Debug.Log("Инициализация первого уровня");
            
        TurnManager.Instance.SetState(TurnState.PlayerTurn);
    }
}