using UnityEngine;
using UnityEngine.UI;

namespace Sprites
{
public class EndTurnButton : MonoBehaviour
    {
        private Button _button;

        private void Awake() => _button = GetComponent<Button>();

        private void OnEnable() => TurnManager.Instance.OnStateChanged += Toggle;
        private void OnDisable() => TurnManager.Instance.OnStateChanged -= Toggle;

        private void Toggle(TurnState state) => _button.interactable = (state == TurnState.PlayerTurn);
    }}