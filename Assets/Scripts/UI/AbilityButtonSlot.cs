using Abilities;
using Core;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Entities;

namespace UI
{
    [System.Serializable]
    public class AbilityButtonSlot
    {
        public Button button;

        [Header("Visual Feedback")] 
        public GameObject selectedFrame;
        public Image buttonImage;

        [Header("UI Elements")] 
        public TextMeshProUGUI nameText;
        public TextMeshProUGUI hotkeyText;
        public TextMeshProUGUI costText;

        public bool IsSelected { get; private set; }
        
        private Coroutine shakeCoroutine;
        private AbilityData currentAbility;
        private int currentIndex;

        public void Setup(AbilityData ability, int index)
        {
            button.gameObject.SetActive(true);
            
            currentAbility = ability;
            currentIndex = index;

            if (nameText != null)
                nameText.text = ability.abilityName;

            if (hotkeyText != null)
                hotkeyText.text = (index + 1).ToString();

            if (costText != null)
            {
                if (ability.energyCost > 0)
                {
                    costText.text = ability.energyCost.ToString();
                    costText.gameObject.SetActive(true);
                }
                else
                {
                    costText.gameObject.SetActive(false);
                }
            }

            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(OnButtonClick);
        }

        private void OnButtonClick()
        {
            if (PlayerMovement.Instance == null)
            {
                AbilityController.Instance.SelectAbilityByIndex(currentIndex);
                return;
            }

            var stats = PlayerMovement.Instance.Stats;
            var shouldFlash = false;

            if (currentAbility is MoveAbilityData)
            {
                if (stats.RemainingSteps <= 0 || stats.Energy <= 0)
                {
                    shouldFlash = true;
                }
            }

            else
            {
                if (!stats.HasEnergyForAction(currentAbility.energyCost))
                {
                    shouldFlash = true;
                }
            }

            if (shouldFlash)
            {
                PlayWarningEffect();
            }

            AbilityController.Instance.SelectAbilityByIndex(currentIndex);
        }

        private void PlayWarningEffect()
        {
            if (buttonImage == null || button == null) return;
            
            if (shakeCoroutine != null)
            {
                button.StopCoroutine(shakeCoroutine);
            }
            
            shakeCoroutine = button.StartCoroutine(FlashRed());
        }

        private IEnumerator FlashRed()
        {
            var originalColor = buttonImage.color;
            buttonImage.color = new Color(1f, 0.3f, 0.3f);
            
            yield return new WaitForSeconds(0.15f);
            
            buttonImage.color = originalColor;
        }

        public void Hide()
        {
            button.gameObject.SetActive(false);
        }

        public void SetSelected(bool selected)
        {
            IsSelected = selected;

            if (selectedFrame != null)
                selectedFrame.SetActive(selected);
        }

        public void SetInteractable(bool value)
        {
            button.interactable = value;
        }
        
        public void TriggerFlash()
        {
            PlayWarningEffect();
        }
    }
}