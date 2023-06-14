using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamagable
{
    void RecieveDamage(Vector3 attackerPosition,int damage, float staggerDuration, Vector2 horizontalKnockbackVelocity, float verticalKnockbackVelocity);
}
