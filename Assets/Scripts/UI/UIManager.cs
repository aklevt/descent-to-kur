using UnityEngine;

namespace UI
{
    /// <summary>
    /// Управляет UI-окнами
    /// </summary>
    public class UIManager : MonoBehaviour
    {
        public static UIManager Instance { get; private set; }

        [Header("UI Panels")]
        [SerializeField] private PauseMenu pauseMenu;

        private void Awake()
        {
            if (Instance == null)
                Instance = this;
            else
                Destroy(gameObject);
        }

        private void Start()
        {
            if (pauseMenu != null)
                pauseMenu.Hide();
        }

        /// <summary>
        /// Переключить паузу (ESC)
        /// </summary>
        public void TogglePause()
        {
            if (Core.GameStateManager.Instance == null) return;
            
            Debug.Log("Pause toggle");
            
            if (Core.GameStateManager.Instance.CurrentState == Core.GameState.Paused)
            {
                ClosePause();
            }
            else if (Core.GameStateManager.Instance.CanPause)
            {
                OpenPause();
            }
        }

        private void OpenPause()
        {
            if (pauseMenu != null)
                pauseMenu.Show();
            
            Core.GameStateManager.Instance?.SetState(Core.GameState.Paused);
        }

        private void ClosePause()
        {
            if (pauseMenu != null)
                pauseMenu.Hide();
            
            Core.GameStateManager.Instance?.SetState(Core.GameState.Gameplay);
        }

        // public void ShowDialog(DialogData data) { }
        // public void CloseDialog() { }
    }
}