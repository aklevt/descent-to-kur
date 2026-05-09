using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Dialogue
{
    public class FullscreenDialogueUI : MonoBehaviour, IDialogueView
    {
        [Header("UI Elements")]
        [SerializeField] private CanvasGroup container;
        [SerializeField] private CanvasGroup darkenBackground;
        [SerializeField] private Image portraitImage;
        [SerializeField] private TextMeshProUGUI speakerNameText;
        [SerializeField] private TextMeshProUGUI dialogueText;
        [SerializeField] private Button continueButton;
        [SerializeField] private TextMeshProUGUI continueButtonText;
        
        [Header("Animation")]
        [SerializeField] private float fadeInDuration = 0.05f;
        [SerializeField] private float fadeOutDuration = 0.05f;
        
        public bool IsTyping { get; private set; }
        private bool shouldCompleteInstantly;
        private string currentFullText;
        
        private Coroutine showCoroutine;
        private Coroutine hideCoroutine;
        private Coroutine displayCoroutine;

        private void Awake()
        {
            gameObject.SetActive(false);
            
            if (container != null)
            {
                container.alpha = 0f;
                container.gameObject.SetActive(false);
            }
            
            ClearPanel();
            
            if (continueButton != null)
            {
                continueButton.onClick.AddListener(OnContinuePressed);
            }
        }

        private void OnDestroy()
        {
            if (continueButton != null)
            {
                continueButton.onClick.RemoveListener(OnContinuePressed);
            }
        }

        private void ClearPanel()
        {
            if (dialogueText != null)
            {
                dialogueText.text = "";
                dialogueText.maxVisibleCharacters = 0;
                dialogueText.color = new Color(dialogueText.color.r, dialogueText.color.g, dialogueText.color.b, 0f);
            }
            
            if (speakerNameText != null)
            {
                speakerNameText.text = "";
                speakerNameText.color = new Color(speakerNameText.color.r, speakerNameText.color.g, speakerNameText.color.b, 0f);
            }
            
            if (portraitImage != null)
            {
                portraitImage.sprite = null;
                portraitImage.gameObject.SetActive(false);
            }
            
            if (continueButton != null)
            {
                continueButton.gameObject.SetActive(false);
            }
        }

        private void OnContinuePressed()
        {
            DialogueManager.Instance?.AdvanceDialogue();
        }

        public IEnumerator Show()
        {
            StopAllActiveCoroutines();
            
            if (container == null) yield break;
            
            ClearPanel();
            
            container.alpha = 0f;
            if (darkenBackground != null)
            {
                darkenBackground.alpha = 0f;
            }
            
            gameObject.SetActive(true);
            container.gameObject.SetActive(true);
            
            var elapsed = 0f;
            while (elapsed < fadeInDuration)
            {
                elapsed += Time.deltaTime;
                var t = elapsed / fadeInDuration;
                
                container.alpha = Mathf.Lerp(0f, 1f, t);
                
                if (darkenBackground != null)
                {
                    darkenBackground.alpha = Mathf.Lerp(0f, 0.7f, t);
                }
                
                yield return null;
            }
            
            container.alpha = 1f;
            if (darkenBackground != null)
            {
                darkenBackground.alpha = 0.7f;
            }
            
            showCoroutine = null;
        }

        public IEnumerator Hide()
        {
            StopAllActiveCoroutines();
            
            if (container == null) yield break;
            
            var elapsed = 0f;
            var startAlpha = container.alpha;
            var startBg = darkenBackground != null ? darkenBackground.alpha : 0f;
            
            while (elapsed < fadeOutDuration)
            {
                elapsed += Time.deltaTime;
                var t = elapsed / fadeOutDuration;
                
                container.alpha = Mathf.Lerp(startAlpha, 0f, t);
                
                if (darkenBackground != null)
                {
                    darkenBackground.alpha = Mathf.Lerp(startBg, 0f, t);
                }
                
                yield return null;
            }
            
            container.alpha = 0f;
            if (darkenBackground != null)
            {
                darkenBackground.alpha = 0f;
            }
            
            container.gameObject.SetActive(false);
            ClearPanel();
            
            gameObject.SetActive(false);
            
            hideCoroutine = null;
        }

        public IEnumerator DisplayLine(DialogueLine line, float typewriterSpeed)
        {
            IsTyping = true;
            shouldCompleteInstantly = false;
            currentFullText = line.text;
            
            if (continueButton != null)
            {
                continueButton.gameObject.SetActive(false);
            }
            
            if (dialogueText != null)
            {
                var color = dialogueText.color;
                dialogueText.color = new Color(color.r, color.g, color.b, 1f);
            }

            if (portraitImage != null)
            {
                if (line.speakerPortrait != null)
                {
                    portraitImage.sprite = line.speakerPortrait;
                    portraitImage.gameObject.SetActive(true);
                }
                else
                {
                    portraitImage.gameObject.SetActive(false);
                }
            }

            if (speakerNameText != null)
            {
                speakerNameText.text = line.speakerName ?? "";
            }

            dialogueText.text = currentFullText;
            dialogueText.ForceMeshUpdate();
            
            var totalChars = dialogueText.textInfo.characterCount;
            dialogueText.maxVisibleCharacters = 0;

            for (var i = 0; i <= totalChars; i++)
            {
                if (shouldCompleteInstantly)
                {
                    dialogueText.maxVisibleCharacters = totalChars;
                    break;
                }
                
                dialogueText.maxVisibleCharacters = i;
                
                var delay = 1f / typewriterSpeed;
                
                if (i < totalChars)
                {
                    var c = dialogueText.textInfo.characterInfo[i].character;
                    if (c == '.' || c == '!' || c == '?')
                        delay *= 3f;
                    else if (c == ',' || c == ';')
                        delay *= 2f;
                    else if (char.IsWhiteSpace(c))
                        delay = 0f;
                }
                
                yield return new WaitForSeconds(delay);
            }

            IsTyping = false;
            
            if (continueButton != null)
            {
                continueButton.gameObject.SetActive(true);
            }
            
            displayCoroutine = null;
        }

        public void CompleteCurrentLine()
        {
            shouldCompleteInstantly = true;
            
            if (dialogueText != null && dialogueText.textInfo != null)
            {
                dialogueText.maxVisibleCharacters = dialogueText.textInfo.characterCount;
            }
            
            IsTyping = false;
            
            if (continueButton != null)
            {
                continueButton.gameObject.SetActive(true);
            }
        }
        
        private void StopAllActiveCoroutines()
        {
            if (showCoroutine != null)
            {
                StopCoroutine(showCoroutine);
                showCoroutine = null;
            }
            
            if (hideCoroutine != null)
            {
                StopCoroutine(hideCoroutine);
                hideCoroutine = null;
            }
            
            if (displayCoroutine != null)
            {
                StopCoroutine(displayCoroutine);
                displayCoroutine = null;
            }
            
            IsTyping = false;
            shouldCompleteInstantly = false;
        }
    }
}