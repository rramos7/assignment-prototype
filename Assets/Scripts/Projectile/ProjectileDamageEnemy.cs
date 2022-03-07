using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileDamageEnemy : Projectile
{
    [SerializeField] private int damage = 5;
    [SerializeField] private string tagToDamage;
    //[SerializeField] private LayerMask layersToDamage; //check by layer

    public override void SetDirection(Vector2 dir)
    {
        base.SetDirection(dir);
        Invoke(nameof(Vanish), 3);
    }
    
    private void Vanish()
    {
        Destroy(gameObject);
    }

    private void OnCollisionEnter2D(Collision2D other)    
    {
        //this projectile goes away when hitting anything
        Destroy(gameObject);
        
        //Check by layer 
        //if ((layersToDamage.value & (1 << other.gameObject.layer)) > 0)

        if (other.transform.CompareTag(tagToDamage))
        {
            other.transform.GetComponent<HealthSystem>()?.Damage(damage);
        }
    }
}