using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightAttack2Script : MonoBehaviour
{
    public SpriteRenderer sprite;
    public SpriteRenderer targetSprite;

    void Update()
    {
        Vector3 newPosition = this.transform.localPosition;

        newPosition.x = 2.44f * (targetSprite.flipX ? 1 : -1);

        sprite.flipX = targetSprite.flipX;
        sprite.sortingOrder = targetSprite.sortingOrder - 10;

        this.transform.localPosition = newPosition;
    }
}
