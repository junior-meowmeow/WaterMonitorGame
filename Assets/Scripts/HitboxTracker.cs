using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitboxTracker : MonoBehaviour
{
    public BoxCollider collider;

    void Update()
    {
        this.transform.position = collider.transform.position;
        this.transform.localScale = collider.size;
    }
}
