using System.Collections;
using UnityEngine;

public class EnemyController : EnemyBase
{
    protected override IEnumerator ExecuteAction()
    {
        yield return TryUseAbility(0);
    }
}