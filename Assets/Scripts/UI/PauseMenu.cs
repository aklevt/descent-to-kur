using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class PauseMenu : MonoBehaviour
    {
        [SerializeField] private GameObject panel;
        [SerializeField] private Button resumeButton;
        [SerializeField] private Button quitButton;

        private void Start()
        {
            if (resumeButton != null)
                resumeButton.onClick.AddListener(OnResumeClicked);

            if (quitButton != null)
                quitButton.onClick.AddListener(OnQuitClicked);

            Hide();
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