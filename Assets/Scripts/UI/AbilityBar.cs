using System.Collections;
using System.Collections.Generic;
using Entities;
using UnityEngine;

namespace UI
{
    /// <summary>
    /// Управляет отображением кнопок способностей игрока
    /// </summary>
    public class AbilityBar : MonoBehaviour
    {
        [SerializeField] private List<AbilityButtonSlot> slots = new();

        private IEnumerator Start()
        {
            yield return null;

            RefreshSlots();

            // Выбрать первую способность при старте (ход)
            if (slots.Count > 0 && PlayerMovement.Instance?.Abilities != null &&
                PlayerMovement.Instance.Abilities.Count > 0)
            {
                OnAbilitySelected(0);
            }

            if (TurnManager.Instance != null)
                TurnManager.Instance.OnStateChanged += OnTurnChanged;
        }

        private void OnDestroy()
        {
            if (TurnManager.Instance != null)
                TurnManager.Instance.OnStateChanged -= OnTurnChanged;
        }

        private void OnTurnChanged(TurnState state)
        {
            var isPlayerTurn = state == TurnState.PlayerTurn;
            foreach (var slot in slots)
                slot.SetInteractable(isPlayerTurn);
        }

        private void RefreshSlots()
        {
            var abilities = PlayerMovement.Instance?.Abilities;

            for (var i = 0; i < slots.Count; i++)
            {
                if (abilities != null && i < abilities.Count)
                {
                    slots[i].Setup(abilities[i], i);
                }
                else
                {
                    slots[i].Hide();
                }
            }
        }

        public void OnAbilitySelected(int index)
        {
            for (var i = 0; i < slots.Count; i++)
                slots[i].SetSelected(i == index);
        }
        
        public void DeselectAllSlots()
        {
            foreach (var slot in slots)
                slot.SetSelected(false);
        }
        
        public void TriggerWarningFlash(int index)
        {
            if (index >= 0 && index < slots.Count)
            {
                slots[index].TriggerFlash();
            }
        }
    }
}