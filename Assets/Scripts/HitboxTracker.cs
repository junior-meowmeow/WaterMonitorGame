using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitboxTracker : MonoBehaviour
{
    public BoxCollider m_collider;

    void Update()
    {
        this.transform.position = m_collider.transform.position;
        this.transform.localScale = m_collider.size;
    }
}
