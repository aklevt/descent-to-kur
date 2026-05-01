using System.Collections;
using UnityEngine;

public class EnemyController : BaseEnemy
{
    protected override IEnumerator ExecuteAction()
    {
        yield return TryUseAbility(0);
    }
}