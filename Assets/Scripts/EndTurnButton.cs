using UnityEngine;
using UnityEngine.UI;

namespace Sprites
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

        private void OnDisable()
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