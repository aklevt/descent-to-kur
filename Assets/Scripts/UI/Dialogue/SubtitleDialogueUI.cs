using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Dialogue
{
    public class SubtitleDialogueUI : BaseDialogueView
    {
        [Header("UI Elements")]
        [SerializeField] private CanvasGroup container;
        [SerializeField] private TextMeshProUGUI subtitleText;
        [SerializeField] private Button clickArea;

        protected override void InitializeView()
        {
            if (container != null)
            {
                container.alpha = 0f;
                container.gameObject.SetActive(false);
            }
            ClearUI();
        }

        protected override void SetupInputListeners()
        {
            if (clickArea != null)
                clickArea.onClick.AddListener(OnInputPressed);
        }

        protected override void CleanupInputListeners()
        {
            if (clickArea != null)
                clickArea.onClick.RemoveListener(OnInputPressed);
        }

        protected override void ClearUI()
        {
            if (subtitleText != null)
            {
                subtitleText.text = "";
                subtitleText.maxVisibleCharacters = 0;
            }
        }

        protected override IEnumerator ShowAnimation()
        {
            if (container == null) yield break;

            container.gameObject.SetActive(true);
            
            var elapsed = 0f;
            while (elapsed < fadeInDuration)
            {
                elapsed += Time.deltaTime;
                container.alpha = Mathf.Lerp(0f, 1f, elapsed / fadeInDuration);
                yield return null;
            }
            container.alpha = 1f;
        }

        protected override IEnumerator HideAnimation()
        {
            if (container == null) yield break;

            var elapsed = 0f;
            while (elapsed < fadeOutDuration)
            {
                elapsed += Time.deltaTime;
                container.alpha = Mathf.Lerp(1f, 0f, elapsed / fadeOutDuration);
                yield return null;
            }
            container.alpha = 0f;
            container.gameObject.SetActive(false);
        }

        protected override IEnumerator DisplayLineAnimation(DialogueLine line, float typewriterSpeed)
        {
            yield return TypewriterEffect(subtitleText, line.text, typewriterSpeed);
        }

        protected override void OnLineCompleted()
        {
            if (subtitleText != null && subtitleText.textInfo != null)
            {
                subtitleText.maxVisibleCharacters = subtitleText.textInfo.characterCount;
            }
        }
    }
}