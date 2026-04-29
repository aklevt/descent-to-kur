using Entities;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.HUD
{
    public class PlayerHUD : MonoBehaviour
    {
        [SerializeField] private Slider hpSlider;
        [SerializeField] private Slider energySlider;
        [SerializeField] private TextMeshProUGUI hpText;
        [SerializeField] private TextMeshProUGUI energyText;

        private void Update()
        {
            if (PlayerMovement.Instance == null) return;

            var stats = PlayerMovement.Instance.Stats;

            if (hpSlider != null)
            {
                hpSlider.maxValue = stats.MaxHealth;
                hpSlider.value = stats.Health;
            }

            if (energySlider != null)
            {
                energySlider.maxValue = stats.MaxEnergy;
                energySlider.value = stats.Energy;
            }

            if (hpText != null) hpText.text = $"{stats.Health}/{stats.MaxHealth}";
            if (energyText != null) energyText.text = $"{stats.Energy}/{stats.MaxEnergy}";
        }
    }
}