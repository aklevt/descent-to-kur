using UnityEngine;

/// <summary>
/// Параметры проверки клетки при поиске
/// </summary>
public struct CellCheckOptions
{
    public bool checkEntities; // Учитывать ли сущности при проверке проходимости
    public bool useBFS; // Использовать BFS (true) или простой перебор (false)
    public GameObject currentEntity; // Сущность, для которой выполняется проверка

    public static CellCheckOptions ForMovement(GameObject entity = null)
    {
        return new CellCheckOptions { checkEntities = true, useBFS = true, currentEntity = entity };
    }

    public static CellCheckOptions ForAttack()
    {
        return new CellCheckOptions { checkEntities = false, useBFS = false, currentEntity = null };
    }
}