using System.Collections;
using System.Collections.Generic;
using Abilities;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Core
{
    public class AbilityController : MonoBehaviour
    {
        public static AbilityController Instance { get; private set; }

        [Header("Abilities")] 
        [SerializeField] private AbilityData moveAbility;
        [SerializeField] private AbilityData simpleAttackAbility;
        [SerializeField] private AbilityData rangedAttackAbility;

        [Header("Ability Buttons")] [SerializeField]
        private Button moveButton;

        [SerializeField] private Button simpleAttackButton;
        [SerializeField] private Button rangedAttackButton;

        [Header("Abilities")] [SerializeField] private List<AbilityData> abilities;
        [SerializeField] private List<Button> abilityButtons;

        private List<Vector3Int> availableCells = new();

        private AbilityData selectedAbility;

        private bool isExecuting;

        /// <summary>
        /// Разрешено ли взаимодействие со способностями в текущем состоянии игры
        /// </summary>
        private bool IsPlayerTurnActive =>
            !isDead &&
            TurnManager.Instance != null &&
            TurnManager.Instance.CurrentState == TurnState.PlayerTurn &&
            PlayerMovement.Instance != null;

        /// <summary>
        /// Список координат клеток, доступных для текущей выбранной способности
        /// </summary>
        public List<Vector3Int> AvailableCells => availableCells;

        private bool isDead = false;

        private void Awake()
        {
            if (Instance == null)
                Instance = this;
            else
                Destroy(gameObject);
        }

        private void Start()
        {
            if (TurnManager.Instance != null)
                TurnManager.Instance.OnStateChanged += HandleTurnChanged;
        }

        private void OnDestroy()
        {
            if (TurnManager.Instance != null)
                TurnManager.Instance.OnStateChanged -= HandleTurnChanged;
        }

        private void Update()
        {
            if (!IsPlayerTurnActive || isExecuting) return;

            // Автоматическое обновление сетки после завершения движения
            if (ShouldRefreshAbilityGrid())
            {
                RefreshAbilityOverlay();
            }
        }

        /// <summary>
        /// Проверка условий, когда нужно перерисовать сетку:
        /// 1. Ход игрока
        /// 2. Игрок физически остановился
        /// 3. Сетка в данный момент пуста, так как была очищена
        /// </summary>
        private bool ShouldRefreshAbilityGrid()
        {
            return !PlayerMovement.Instance.IsMoving &&
                   selectedAbility != null &&
                   availableCells.Count == 0;
        }

        private void HandleTurnChanged(TurnState newState)
        {
            if (newState != TurnState.PlayerTurn) return;
            
            if (abilities.Count > 0)
                SelectAbility(abilities[0]);
            else
            {
                ClearSelection();
                UpdateButtonsState();
            }
        }


        /// <summary>
        /// Обрабатывает нажатие игрока на клетку игрового поля
        /// </summary>
        public void HandleCellClick(Vector3Int clickedCell)
        {
            if (!IsPlayerTurnActive || isExecuting)
                return;
            if (PlayerMovement.Instance.IsMoving)
                return;
            if (selectedAbility == null)
                return;
            if (!availableCells.Contains(clickedCell))
                return;

            StartCoroutine(ExecuteSelectedAbility(clickedCell));
        }

        private IEnumerator ExecuteSelectedAbility(Vector3Int targetCell)
        {
            isExecuting = true;
            var ability = selectedAbility;

            ClearSelection();

            yield return ability.Execute(PlayerMovement.Instance, targetCell);

            isExecuting = false;
        }

        // private void PerformRangedAttack(Vector3Int targetCell)
        // {
        //     var target = GridManager.Instance.GetEntityAt(targetCell);
        //     if (target == null) return;
        //
        //     if (target.TryGetComponent<Health>(out var targetHealth))
        //     {
        //         var damage = PlayerMovement.Instance.Stats.AttackDamage;
        //         targetHealth.TakeDamage(damage);
        //     }
        //
        //     SelectAbility((int)AbilityType.None);
        // }
        //
        // private void PerformAttack(Vector3Int targetCell)
        // {
        //     var target = GridManager.Instance.GetEntityAt(targetCell);
        //
        //     if (target != null && target != PlayerMovement.Instance.gameObject)
        //     {
        //         StartCoroutine(PlayerAttackSequence(target));
        //     }
        //     else
        //     {
        //         Debug.Log("На клетке нет противника");
        //         SelectAbility((int)AbilityType.None);
        //     }
        // }

        // private IEnumerator PlayerAttackSequence(GameObject target)
        // {
        //     var targetHealth = target.GetComponent<Health>();
        //     var damage = PlayerMovement.Instance.Stats.AttackDamage;
        //
        //     yield return StartCoroutine(PlayerMovement.Instance.PunchAnimation(
        //         target.transform.position,
        //         () =>
        //         {
        //             if (targetHealth != null)
        //                 targetHealth.TakeDamage(damage);
        //
        //             CameraFollow.Instance?.ShakeMedium();
        //         }
        //     ));
        //
        //     SelectAbility((int)AbilityType.None);
        // }
        //
        // private void PerformMovement(Vector3Int targetCell)
        // {
        //     PlayerMovement.Instance.ExecuteMove(targetCell);
        //     ClearSelection();
        // }

        private void RefreshAbilityOverlay()
        {
            ClearSelection();
            UpdateButtonsState();

            if (!IsPlayerTurnActive || selectedAbility == null) return;

            availableCells = selectedAbility.GetTargetCells(PlayerMovement.Instance);
            GridHighlighter.Instance.HighlightCells(availableCells, selectedAbility.highlightColor);
        }

        /// <summary>
        /// Заполняет список клеток, доступных для атаки
        /// </summary>
        private void PrepareAttackArea(Vector3Int playerCell)
        {
            var attackable = GridManager.Instance.GetAttackableCellsInRadius(playerCell, 1);
            availableCells.AddRange(attackable);
        }

        private void PrepareRangeAttackArea(Vector3Int playerCell)
        {
            var attackable = GridManager.Instance.GetAttackableCellsInRadius(playerCell, 3, 2);

            availableCells.AddRange(attackable);
        }

        /// <summary>
        /// Заполняет список доступных клеток для перемещения игрока
        /// </summary>
        private void PrepareMoveArea(Vector3Int playerCell)
        {
            var walkable = GridManager.Instance.GetWalkableTilesInRange(
                playerCell,
                PlayerMovement.Instance.Stats.MoveRange,
                PlayerMovement.Instance.gameObject
            );
            availableCells.AddRange(walkable);
        }

        public void HandleCellHover(Vector3Int hoveredCell)
        {
            if (!IsPlayerTurnActive) return;

            if (!availableCells.Contains(hoveredCell))
            {
                GridHighlighter.Instance.ClearEffect();
                return;
            }

            var effectCells = selectedAbility.GetEffectCells(hoveredCell, PlayerMovement.Instance);
            GridHighlighter.Instance.HighlightEffect(effectCells, selectedAbility.effectColor);
        }

        // private List<Vector3Int> GetEffectCells(Vector3Int hoveredCell)
        // {
        //     switch (selectedAbility)
        //     {
        //         case AbilityType.Attack:
        //             return new List<Vector3Int> { hoveredCell };
        //
        //         case AbilityType.RangedAttack:
        //             return new List<Vector3Int> { hoveredCell };
        //
        //         default:
        //             return new List<Vector3Int>();
        //     }
        // }

        // private Color GetEffectColor()
        // {
        //     return selectedAbility switch
        //     {
        //         AbilityType.Attack => new Color(1f, 0.2f, 0.2f, 0.9f),
        //         AbilityType.RangedAttack => new Color(1f, 0.5f, 0f, 0.9f),
        //         _ => Color.white
        //     };
        // }


        // private void RenderSelection(Color color)
        // {
        //     if (GridHighlighter.Instance != null && availableCells.Count > 0)
        //     {
        //         GridHighlighter.Instance.HighlightCells(availableCells, color);
        //     }
        // }

        private void ClearSelection()
        {
            availableCells.Clear();
            GridHighlighter.Instance.Clear();
        }

        /// <summary>
        /// Переключает текущую активную способность, вызывается кнопками UI
        /// </summary>
        private void SelectAbility(AbilityData ability)
        {
            if (selectedAbility == ability) return;
            selectedAbility = ability;
            RefreshAbilityOverlay();
            UpdateButtonsState();
        }

        /// <summary>
        /// Управляет доступностью кнопок в зависимости от выбранного режима
        /// </summary>
        private void UpdateButtonsState()
        {
            for (var i = 0; i < abilityButtons.Count; i++)
            {
                if (abilityButtons[i] == null) continue;

                var isSelected = i < abilities.Count && abilities[i] == selectedAbility;
                abilityButtons[i].interactable = IsPlayerTurnActive && !isSelected;
            }
        }

        public void DisableAllOverlaysAfterDeath()
        {
            isDead = true;
            selectedAbility = null;
            ClearSelection();
            UpdateButtonsState();
        }

        // public void SetMoveMode() => SelectAbility((int)AbilityType.Move);
        // public void SetAttackMode() => SelectAbility((int)AbilityType.Attack);
        // public void SetRangedAttackMode() => SelectAbility((int)AbilityType.RangedAttack);

        /// <summary>
        /// Вызываются кнопками UI
        /// </summary>
        public void SelectMoveAbility() => SelectAbility(moveAbility);
        public void SelectSimpleAttackAbility() => SelectAbility(simpleAttackAbility);
        public void SelectRangedAttackAbility() => SelectAbility(rangedAttackAbility);
    }
}