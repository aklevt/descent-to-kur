using System.Collections.Generic;

namespace UI.Dialogue
{
    /// <summary>
    /// Сохранение прочитанных диалогов во время игры
    /// </summary>
    public static class DialogueProgress
    {
        private static HashSet<string> completedDialogues = new HashSet<string>();
        
        public static bool IsCompleted(string dialogueID)
        {
            return completedDialogues.Contains(dialogueID);
        }
        
        public static void MarkCompleted(string dialogueID)
        {
            completedDialogues.Add(dialogueID);
        }
        
        /// <summary>
        /// Сбросить все диалоги 
        /// </summary>
        public static void ResetAll() // ❗ Добавить в меню на кнопку Новая игра, т.к. статические классы живут дольше, чем сцены
        {
            completedDialogues.Clear();
        }
    }
}