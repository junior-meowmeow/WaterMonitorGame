using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class HoleScript : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PlayerHitbox"))
        {
            GameObject player = other.gameObject;
            Vector2 currentPosition2D = this.transform.position;
            Vector2 targetPosition2D = player.transform.position;
            Vector2 displacement = targetPosition2D - currentPosition2D;
            int damage = 5;
            Vector2 horizontalKnockback = 1.5f * (displacement.normalized);
            float verticalKnockback = 5f;
            player.GetComponent<PlayerScript>().RecieveDamage(this.transform.position, damage, 0f, horizontalKnockback, verticalKnockback);
        }
        if (other.CompareTag("EnemyHitbox"))
        {
            other.gameObject.GetComponent<EnemyScript>().FallToDead();
        }
    }
}
