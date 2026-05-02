using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

public class Gigachad : BaseEnemy
{
    public override void Initialize()
    {
        MaxHealth = 50;
        AnimationSpeed = 3;
        Freeze = 0;
        Health = MaxHealth;
        MoveRange = 2;
    }
}
