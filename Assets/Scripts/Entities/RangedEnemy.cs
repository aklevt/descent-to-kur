using System.Collections;
using UnityEngine;

namespace Entities
{
    /// <summary>
    /// Враг дальнего боя. Держится на дистанции и стреляет снарядами
    /// </summary>
    public class RangedEnemy : EnemyBase
    {
        protected override IEnumerator ExecuteAction()
        {
            yield return TryUseAbility(0);
        }
    }
}