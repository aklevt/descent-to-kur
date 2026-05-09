using System.Collections.Generic;
using UnityEngine;

namespace UI.Dialogue
{
    /// <summary>
    /// Контейнер для диалога
    /// </summary>
    [CreateAssetMenu(fileName = "New Dialogue", menuName = "Dialogue/Dialogue Data")]
    public class DialogueData : ScriptableObject
    {
        [Header("Dialogue Content")]
        [Tooltip("Список реплик в диалоге")]
        public List<DialogueLine> lines = new();
        
        [Header("Style")]
        public DialogueStyle style = DialogueStyle.InGame;
        
        [Header("Settings")]
        [Tooltip("Можно ли пропустить весь диалог по ESC")]
        public bool canSkip = true;
        
        [Tooltip("Скорость печати (символов в секунду)")]
        public float typewriterSpeed = 30f;
        
        [Tooltip("Не показывать этот диалог повторно после перезапуска комнаты (рестарт/смерть)")]
        public bool showOnlyOnce = false;
        
        /// <summary>
        /// Уникальный идентификатор диалога для системы сохранения
        /// </summary>
        public string DialogueID => name;

    }
}