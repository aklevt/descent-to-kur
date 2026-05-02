using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

public class Mumi : BaseEnemy
{
    public override void Initialize()
    {
        MaxHealth = 25;
        AnimationSpeed = 5;
        Freeze = 0;
        Health = MaxHealth;
        MoveRange = 3;
    }
}
