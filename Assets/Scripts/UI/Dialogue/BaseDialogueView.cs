using System.Collections;
using UnityEngine;

namespace UI.Dialogue
{
    /// <summary>
    /// Базовая логика для всех диалоговых view
    /// </summary>
    public abstract class BaseDialogueView : MonoBehaviour, IDialogueView
    {
        [Header("Animation")]
        [SerializeField] protected float fadeInDuration = 0.3f;
        [SerializeField] protected float fadeOutDuration = 0.3f;
        
        public bool IsTyping { get; protected set; }
        
        protected bool shouldCompleteInstantly;
        protected string currentFullText;
        
        protected Coroutine showCoroutine;
        protected Coroutine hideCoroutine;
        protected Coroutine displayCoroutine;

        protected virtual void Awake()
        {
            gameObject.SetActive(false);
            InitializeView();
            SetupInputListeners();
        }

        protected virtual void OnDestroy()
        {
            CleanupInputListeners();
        }

        /// <summary>
        /// Инициализация конкретного view
        /// </summary>
        protected abstract void InitializeView();
        
        /// <summary>
        /// Настройка слушателей ввода
        /// </summary>
        protected abstract void SetupInputListeners();
        
        /// <summary>
        /// Очистка слушателей ввода
        /// </summary>
        protected abstract void CleanupInputListeners();
        
        /// <summary>
        /// Очистка элементов интерфейса
        /// </summary>
        protected abstract void ClearUI();

        public IEnumerator Show()
        {
            StopAllActiveCoroutines();
            
            ClearUI();
            gameObject.SetActive(true);
            
            showCoroutine = StartCoroutine(ShowAnimation());
            yield return showCoroutine;
            showCoroutine = null;
        }

        public IEnumerator Hide()
        {
            StopAllActiveCoroutines();
            
            hideCoroutine = StartCoroutine(HideAnimation());
            yield return hideCoroutine;
            
            ClearUI();
            gameObject.SetActive(false);
            hideCoroutine = null;
        }

        public IEnumerator DisplayLine(DialogueLine line, float typewriterSpeed)
        {
            displayCoroutine = StartCoroutine(DisplayLineAnimation(line, typewriterSpeed));
            yield return displayCoroutine;
            displayCoroutine = null;
        }

        public virtual void CompleteCurrentLine()
        {
            shouldCompleteInstantly = true;
            IsTyping = false;
            OnLineCompleted();
        }

        /// <summary>
        /// Анимация появления
        /// </summary>
        protected abstract IEnumerator ShowAnimation();
        
        /// <summary>
        /// Анимация исчезновения
        /// </summary>
        protected abstract IEnumerator HideAnimation();
        
        /// <summary>
        /// Анимация отображения строки
        /// </summary>
        protected abstract IEnumerator DisplayLineAnimation(DialogueLine line, float typewriterSpeed);
        
        /// <summary>
        /// Вызывается когда строка завершена
        /// </summary>
        protected abstract void OnLineCompleted();

        protected void StopAllActiveCoroutines()
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

        protected void OnInputPressed()
        {
            DialogueManager.Instance?.AdvanceDialogue();
        }

        /// <summary>
        /// Базовая анимация посимвольной печати текста
        /// </summary>
        protected IEnumerator TypewriterEffect(TMPro.TextMeshProUGUI textComponent, string text, float speed)
        {
            IsTyping = true;
            shouldCompleteInstantly = false;
            currentFullText = text;

            textComponent.text = currentFullText;
            textComponent.ForceMeshUpdate();

            var totalChars = textComponent.textInfo.characterCount;
            textComponent.maxVisibleCharacters = 0;

            for (var i = 0; i <= totalChars; i++)
            {
                if (shouldCompleteInstantly)
                {
                    textComponent.maxVisibleCharacters = totalChars;
                    break;
                }

                textComponent.maxVisibleCharacters = i;

                var delay = 1f / speed;

                if (i < totalChars)
                {
                    var character = textComponent.textInfo.characterInfo[i].character;
                    delay = GetCharacterDelay(character, delay);
                }

                yield return new WaitForSeconds(delay);
            }

            IsTyping = false;
        }

        protected float GetCharacterDelay(char character, float baseDelay)
        {
            return character switch
            {
                '.' or '!' or '?' => baseDelay * 3f,
                ',' or ';' => baseDelay * 2f,
                _ when char.IsWhiteSpace(character) => 0f,
                _ => baseDelay
            };
        }
    }
}