using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamagable
{
    void RecieveDamage(int damage, float staggerDuration, Vector2 horizontalKnockbackVelocity, float verticalKnockbackVelocity);
}
