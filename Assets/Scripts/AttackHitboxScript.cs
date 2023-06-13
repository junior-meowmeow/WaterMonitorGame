using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackHitboxScript : MonoBehaviour
{
    public BoxCollider boxCollider;
    public string targetTag = "EnemyHitbox";
    private bool isCollideTarget;
    private float createdTime;
    public float decayTime = 1f;

    public int damage = 10;
    public float staggerDuration = 0;
    public Vector2 horizontalKnockbackVelocity = Vector2.zero;
    public float verticalKnockbackVelocity = 0;

    void Start()
    {
        boxCollider = this.GetComponent<BoxCollider>();
        isCollideTarget = false;
        createdTime = Time.timeSinceLevelLoad;
    }

    void Update()
    {
        if (isCollideTarget || Time.timeSinceLevelLoad - createdTime >= decayTime)
        {
            Destroy(this.gameObject);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag(targetTag))
        {
            print("hitStay " + targetTag + ": " + other.gameObject.name);
            IDamagable damagable = other.GetComponent<IDamagable>();
            if (damagable != null)
            {
                damagable.RecieveDamage(damage, staggerDuration,horizontalKnockbackVelocity,verticalKnockbackVelocity);
                isCollideTarget = true;
            }
        }

    }
}
