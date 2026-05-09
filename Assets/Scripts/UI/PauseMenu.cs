using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class PauseMenu : MonoBehaviour
    {
        [SerializeField] private GameObject panel;
        [SerializeField] private Button resumeButton;
        [SerializeField] private Button quitButton;
        [SerializeField] private Button restartButton;

        private void Start()
        {
            if (resumeButton != null)
                resumeButton.onClick.AddListener(OnResumeClicked);
            
            if (restartButton != null)
                restartButton.onClick.AddListener(OnRestartClicked);

            if (quitButton != null)
                quitButton.onClick.AddListener(OnQuitClicked);

            // Hide();
        }

        public void Show()
        {
            if (panel != null)
                panel.SetActive(true);
        }

        public void Hide()
        {
            if (panel != null)
                panel.SetActive(false);
        }

        private void OnResumeClicked()
        {
            UIManager.Instance?.TogglePause();
        }
        
        private void OnRestartClicked()
        {
            Debug.Log("[PauseMenu] Комната перезапускается");
            
            UIManager.Instance?.TogglePause();
            
            Core.LevelController.Instance?.RestartCurrentRoom();
        }

        private void OnQuitClicked()
        {
            Debug.Log("[PauseMenu] Quit button pressed");
            Application.Quit();
            
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
        }
    }
}