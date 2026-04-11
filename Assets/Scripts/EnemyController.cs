using System.Collections;
using UnityEngine;

public class EnemyController : EnemyBase
{
    protected override IEnumerator ExecuteAction()
    {
        if (PlayerMovement.Instance == null) yield break;

        var player = PlayerMovement.Instance;
        var playerCell = player.CurrentCell;

        if (Vector3Int.Distance(CurrentCell, playerCell) <= 1.1f)
        {
            player.TryGetComponent<Health>(out var playerHealth);

            yield return StartCoroutine(PunchAnimation(player.transform.position, () => 
            {
                if (playerHealth != null)
                {
                    playerHealth.TakeDamage(AttackDamage);
                }
            }));
        }
    }
}