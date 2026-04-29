using System.Collections;

namespace Entities
{
    public class EnemyController : EnemyBase
    {
        protected override IEnumerator ExecuteAction()
        {
            yield return TryUseAbility(0);
        }
    }
}