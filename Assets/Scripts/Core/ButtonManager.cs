using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonManager : MonoBehaviour
{
    /// <summary>
    /// Следит за активацией и деактивацией кнопок. Обрабатывает нажатия на кнопки
    /// </summary>
    public static ButtonManager Instance { get; private set; }
    private readonly Dictionary<string, Button> buttons = new();


    private void Awake()
    {
        Instance = this;
    }
    
    /// <summary>
    /// Добавить в TurnManager деактивацию кнопок на время хода противника
    /// </summary>
    private void Start()
    {
        TurnManager.Instance.OnStateChanged += ToggleButtons;
    }

    /// <summary>
    /// Добавить кнопку в обработчик
    /// </summary>
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
        AbilityController.Instance.ChangeAbility(null);
    }

    /// <summary>
    /// Поменять выбранную способность персоонажа при нажатии на одну из кнопок способностей
    /// </summary>
    private void SwitchAbility(string ability)
    {
        var currentAbility = AbilityController.Instance.SelectedAbility;
        if (currentAbility != null) 
            buttons[currentAbility].interactable = true;
        AbilityController.Instance.ChangeAbility(ability);
        buttons[ability].interactable = false;
    }

    /// <summary>
    /// Обработка нажатий кнопок
    /// </summary>
    public void MoveAbility() => SwitchAbility("Move");
    public void PunchAbility() => SwitchAbility("Punch");
    public void ShotAbility() => SwitchAbility("Shot");
}
