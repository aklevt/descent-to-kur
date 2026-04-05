using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonManager : MonoBehaviour
{
    /// <summary>
    /// 
    /// </summary>
    public static ButtonManager Instance { get; private set; }
    private readonly Dictionary<string, Button> buttons = new();
    //private readonly Dictionary<string, str>
    private string currentAbility;


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        TurnManager.Instance.OnStateChanged += ToggleButtons;
    }

    public void AddButton(string key, Button button) => buttons.Add(key, button);

    /// <summary>
    /// Деактивирует кнопки на ходу противника. Активирует на ходу игрока
    /// </summary>
    private void ToggleButtons(TurnState state)
    {
        foreach(var button in buttons.Values)
        {
            button.interactable = (state == TurnState.PlayerTurn);
        }
        currentAbility = null;
    }

    private void SwitchAbility(string ability)
    {
        if (currentAbility != null) 
            buttons[currentAbility].interactable = true;
        currentAbility = ability;
        buttons[currentAbility].interactable = false;
    }

    public void MoveAbility() => SwitchAbility("Move");
    public void PunchAbility() => SwitchAbility("Punch");
    public void ShotAbility() => SwitchAbility("Shot");
}
