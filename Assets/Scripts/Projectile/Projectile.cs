using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]

public class Projectile : MonoBehaviour
{
    [SerializeField] protected float speed = 12;
    protected Rigidbody2D _rb;

    protected virtual void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
    }
    
    public virtual void SetDirection(Vector2 dir)
    {
        dir = dir.normalized;
        _rb.velocity = dir * speed;
    }
}