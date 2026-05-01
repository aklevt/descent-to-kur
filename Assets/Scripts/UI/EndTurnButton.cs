using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class EndTurnButton : MonoBehaviour
    {
        private Button _button;

        private void Awake() => _button = GetComponent<Button>();

        private void Start()
        {
            if (TurnManager.Instance != null)
            {
                TurnManager.Instance.OnStateChanged += Toggle;
                Toggle(TurnManager.Instance.CurrentState);
            }
        }

        private void OnDestroy()
        {
            if (TurnManager.Instance != null)
            {
                TurnManager.Instance.OnStateChanged -= Toggle;
            }
        }

        private void Toggle(TurnState state)
        {
            if (_button != null)
            {
                _button.interactable = (state == TurnState.PlayerTurn);
            }
        }
    }
}