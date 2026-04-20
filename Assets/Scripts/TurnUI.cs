using UnityEngine;
using TMPro;
using System.Collections;

public class TurnUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI turnText;
    [SerializeField] private float displayDuration = 1.5f;

    private void Start()
    {
        if (TurnManager.Instance != null)
            TurnManager.Instance.OnStateChanged += ShowTurnMessage;
    }

    private void OnDestroy()
    {
        if (TurnManager.Instance != null)
            TurnManager.Instance.OnStateChanged -= ShowTurnMessage;
    }

    private void ShowTurnMessage(TurnState state)
    {
        StopAllCoroutines();
        var message = state == TurnState.PlayerTurn ? "ВАШ ХОД" : "ХОД ПРОТИВНИКА";
        var color = state == TurnState.PlayerTurn ? Color.goldenRod : Color.white;
        
        StartCoroutine(DisplayRoutine(message, color));
    }

    private IEnumerator DisplayRoutine(string text, Color color)
    {
        turnText.text = text;
        turnText.color = color;
        turnText.gameObject.SetActive(true);
        yield return new WaitForSeconds(displayDuration);
        turnText.gameObject.SetActive(false);
    }
}