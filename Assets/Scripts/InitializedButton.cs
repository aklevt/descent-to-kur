using UnityEngine;
using UnityEngine.UI;

public class InitializedButton : MonoBehaviour
{
    /// <summary>
    /// —крипт, добавл€ющий кнопку в ButtonManager
    /// </summary>
    private Button _button;
    [SerializeField] public string ButtonName;

    private void Awake() => _button = GetComponent<Button>();
    private void Start() => ButtonManager.Instance.AddButton(ButtonName, _button);


    /// <summary>
    /// —ледующие 3 метода отключают кнопку на врем€ вражеского хода???
    /// </summary>
    //private void OnEnable() => TurnManager.Instance.OnStateChanged += Toggle;
    //private void OnDisable() => TurnManager.Instance.OnStateChanged -= Toggle;
    //private void Toggle(TurnState state) => _button.interactable = (state == TurnState.PlayerTurn);
}
