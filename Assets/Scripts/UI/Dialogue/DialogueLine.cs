using System;
using UnityEngine;

namespace UI.Dialogue
{
    /// <summary>
    /// Одна реплика в диалоге
    /// </summary>
    [Serializable]
    public class DialogueLine
    {
        [TextArea(2, 4)]
        public string text;
        
        [Tooltip("Имя говорящего")]
        public string speakerName;
        
        [Tooltip("Портрет говорящего (Fullscreen only)")]
        public Sprite speakerPortrait;
        
        [Tooltip("Задержка после показа реплики (секунды)")]
        public float delayAfter = 0.5f;
    }
}