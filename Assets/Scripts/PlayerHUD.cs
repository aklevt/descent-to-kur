using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerHUD : MonoBehaviour
{
    [SerializeField] private Slider hpSlider;
    [SerializeField] private Slider energySlider;
    [SerializeField] private TextMeshProUGUI hpText;
    [SerializeField] private TextMeshProUGUI energyText;

    private void Update()
    {
        if (Player.Instance == null) return;

        var player = Player.Instance;

        if (hpSlider != null)
        {
            hpSlider.maxValue = player.MaxHealth;
            hpSlider.value = player.Health;
        }

        if (energySlider != null)
        {
            energySlider.maxValue = player.MaxEnergy;
            energySlider.value = player.Energy;
        }

        if (hpText != null) hpText.text = $"{player.Health}/{player.MaxHealth}";
        if (energyText != null) energyText.text = $"{player.Energy}/{player.MaxEnergy}";
    }
}