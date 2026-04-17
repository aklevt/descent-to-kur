using System.Collections;
using System.Collections.Generic;
using Abilities;
using UnityEngine;
using UnityEngine.UI;

namespace Core
{
    public class AbilityController : MonoBehaviour
    {
        public static AbilityController Instance { get; private set; }

        [SerializeField] private AbilityBar abilityBar;

        private IReadOnlyList<AbilityData> PlayerAbilities =>
            PlayerMovement.Instance?.Abilities;

        private List<Vector3Int> availableCells = new();
        private AbilityData selectedAbility;
        private bool isExecuting;
        private bool isDead;

        private bool IsPlayerTurnActive =>
            !isDead &&
            TurnManager.Instance != null &&
            TurnManager.Instance.CurrentState == TurnState.PlayerTurn &&
            PlayerMovement.Instance != null;

        public List<Vector3Int> AvailableCells => availableCells;

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

            if (ShouldRefreshAbilityGrid())
                RefreshAbilityOverlay();
        }

        private bool ShouldRefreshAbilityGrid() =>
            !PlayerMovement.Instance.IsMoving &&
            selectedAbility != null &&
            availableCells.Count == 0;

        private void HandleTurnChanged(TurnState newState)
        {
            if (newState != TurnState.PlayerTurn)
            {
                ClearSelection();
                return;
            }

            var abilities = PlayerAbilities;
            if (abilities != null && abilities.Count > 0)
                SelectAbilityByIndex(0);
            else
            {
                ClearSelection();
            }
        }

        public void SelectAbilityByIndex(int index)
        {
            var abilities = PlayerAbilities;
            if (abilities == null || index >= abilities.Count)
            {
                return;
            }

            abilityBar?.OnAbilitySelected(index);
            SelectAbility(abilities[index]);
        }


        private void SelectAbility(AbilityData ability)
        {
            if (selectedAbility == ability) return;
            selectedAbility = ability;
            RefreshAbilityOverlay();
        }

        public void HandleCellClick(Vector3Int clickedCell)
        {
            if (!IsPlayerTurnActive || isExecuting) return;
            if (PlayerMovement.Instance.IsMoving) return;
            if (selectedAbility == null) return;
            if (!availableCells.Contains(clickedCell)) return;

            if (!selectedAbility.IsValidTarget(clickedCell, PlayerMovement.Instance))
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

        private void RefreshAbilityOverlay()
        {
            ClearSelection();

            if (!IsPlayerTurnActive || selectedAbility == null) return;

            availableCells = selectedAbility.GetTargetCells(PlayerMovement.Instance);
            GridHighlighter.Instance.HighlightCells(availableCells, selectedAbility.highlightColor);
        }

        private void ClearSelection()
        {
            availableCells.Clear();
            GridHighlighter.Instance.Clear();
        }

        public void DisableAllOverlaysAfterDeath()
        {
            isDead = true;
            selectedAbility = null;
            ClearSelection();
        }
    }
}