using UnityEngine;

public enum RoomTransitionType
{
    /// <summary>
    /// Показать экран победы
    /// </summary>
    VictoryScreen,
    
    /// <summary>
    /// Бесшовный переход (фазы босса)
    /// </summary>
    Seamless,
    
    /// <summary>
    /// Быстрый переход с затемнением
    /// </summary>
    FadeTransition,
    
    /// <summary>
    /// Показать кастомный экран
    /// </summary>
    CustomScreen
}