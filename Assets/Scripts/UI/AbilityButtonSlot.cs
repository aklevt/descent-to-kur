using Abilities;
using Core;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    [System.Serializable]
    public class AbilityButtonSlot
    {
        [Header("Button required")] 
        public Button button;

        [Header("Visual Feedback")] 
        public GameObject selectedFrame;

        [Header("UI Elements")] public TextMeshProUGUI nameText;
        public TextMeshProUGUI hotkeyText;
        public TextMeshProUGUI costText;

        public bool IsSelected { get; private set; }

        public void Setup(AbilityData ability, int index)
        {
            button.gameObject.SetActive(true);

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
            button.onClick.AddListener(() =>
                AbilityController.Instance.SelectAbilityByIndex(index));
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
    }
}