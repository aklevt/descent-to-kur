using System.Collections;

namespace UI.Dialogue
{
    /// <summary>
    /// Интерфейс для всех визуальных представлений диалога
    /// </summary>
    public interface IDialogueView
    {
        bool IsTyping { get; }
        
        IEnumerator Show();
        IEnumerator Hide();
        IEnumerator DisplayLine(DialogueLine line, float typewriterSpeed);
        void CompleteCurrentLine();
    }
}