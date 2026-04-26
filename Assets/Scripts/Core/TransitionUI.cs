using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Core
{
    public class TransitionUI : MonoBehaviour
    {
        public static TransitionUI Instance { get; private set; }

        [Header("Victory Screen")]
        [SerializeField] private GameObject victoryPanel;
        [SerializeField] private TextMeshProUGUI victoryText;
        [SerializeField] private Button continueButton;

        [Header("Fade Screen")]
        [SerializeField] private CanvasGroup fadePanel;
        [SerializeField] private float fadeDuration = 0.5f;

        private void Awake()
        {
            if (Instance == null)
                Instance = this;
            else
                Destroy(gameObject);
        }

        private void Start()
        {
            HideAll();
            
            if (continueButton != null)
                continueButton.onClick.AddListener(OnContinueClicked);
        }

        /// <summary>
        /// Показать экран победы
        /// </summary>
        public void ShowVictoryScreen(string message, Action onContinue)
        {
            if (victoryPanel == null) return;

            if (victoryText != null)
                victoryText.text = message;

            victoryPanel.SetActive(true);

            continueButton.onClick.RemoveAllListeners();
            continueButton.onClick.AddListener(() =>
            {
                HideVictoryScreen();
                onContinue?.Invoke();
            });
        }

        public void HideVictoryScreen()
        {
            if (victoryPanel != null)
                victoryPanel.SetActive(false);
        }

        public void HideAll()
        {
            if (victoryPanel != null) victoryPanel.SetActive(false);
            if (fadePanel != null) fadePanel.gameObject.SetActive(false);
        }

        private void OnContinueClicked()
        {
        }
    }
}