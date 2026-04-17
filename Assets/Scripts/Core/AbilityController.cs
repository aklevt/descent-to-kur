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

        [Header("UI Buttons (optional for now)")]
        [SerializeField] private List<Button> abilityButtons = new();

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
            BindButtons();
            
            if (TurnManager.Instance != null)
                TurnManager.Instance.OnStateChanged += HandleTurnChanged;
        }

        private void OnDestroy()
        {
            if (TurnManager.Instance != null)
                TurnManager.Instance.OnStateChanged -= HandleTurnChanged;
        }

        private void BindButtons()
        {
            for (var i = 0; i < abilityButtons.Count; i++)
            {
                var index = i;
                if (abilityButtons[i] != null)
                    abilityButtons[i].onClick.AddListener(() => SelectAbilityByIndex(index));
            }
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
            if (newState != TurnState.PlayerTurn) return;

            var abilities = PlayerAbilities;
            if (abilities != null && abilities.Count > 0)
                SelectAbilityByIndex(0);
            else
            {
                ClearSelection();
                UpdateButtonsState();
            }
        }

        public void SelectAbilityByIndex(int index)
        {
            var abilities = PlayerAbilities;
            if (abilities == null || index >= abilities.Count) return;
            SelectAbility(abilities[index]);
        }

        private void SelectAbility(AbilityData ability)
        {
            if (selectedAbility == ability) return;
            selectedAbility = ability;
            RefreshAbilityOverlay();
            UpdateButtonsState();
        }

        public void HandleCellClick(Vector3Int clickedCell)
        {
            if (!IsPlayerTurnActive || isExecuting) return;
            if (PlayerMovement.Instance.IsMoving) return;
            if (selectedAbility == null) return;
            if (!availableCells.Contains(clickedCell)) return;

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
            UpdateButtonsState();

            if (!IsPlayerTurnActive || selectedAbility == null) return;

            availableCells = selectedAbility.GetTargetCells(PlayerMovement.Instance);
            GridHighlighter.Instance.HighlightCells(availableCells, selectedAbility.highlightColor);
        }

        private void ClearSelection()
        {
            availableCells.Clear();
            GridHighlighter.Instance.Clear();
        }

        private void UpdateButtonsState()
        {
            var abilities = PlayerAbilities;

            for (var i = 0; i < abilityButtons.Count; i++)
            {
                if (abilityButtons[i] == null) continue;

                var hasAbility = abilities != null && i < abilities.Count;
                var isSelected = hasAbility && abilities[i] == selectedAbility;

                abilityButtons[i].gameObject.SetActive(hasAbility);
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
    }
}