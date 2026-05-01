using UnityEngine;
using System.Collections;

namespace UI
{
    /// <summary>
    /// Отвечает за управление UI-элементы, подписывается на соответствующие события
    /// </summary>
    public class UIController : MonoBehaviour
    {
        public static UIController Instance { get; private set; }

        [Header("Popups")] [SerializeField] private GameObject energyWarningPopup;

        [Header("Durations")] [SerializeField] private float popupDuration = 1.0f;

        private Coroutine currentPopupCoroutine;

        private void Awake()
        {
            if (Instance == null)
                Instance = this;
            else
                Destroy(gameObject);
        }

        private void Start()
        {
            HideAllPopups();
        }

        /// <summary>
        /// Показать предупреждение о нехватке энергии
        /// </summary>
        public void ShowEnergyWarning()
        {
            if (energyWarningPopup == null) return;

            if (currentPopupCoroutine != null)
            {
                StopCoroutine(currentPopupCoroutine);
            }

            currentPopupCoroutine = StartCoroutine(ShowPopupRoutine(energyWarningPopup, popupDuration));
        }

        private IEnumerator ShowPopupRoutine(GameObject popup, float duration)
        {
            popup.SetActive(true);
            yield return new WaitForSeconds(duration);
            popup.SetActive(false);
        }

        private void HideAllPopups()
        {
            if (energyWarningPopup != null)
                energyWarningPopup.SetActive(false);
        }
    }
}