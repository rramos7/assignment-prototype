using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileShooter : MonoBehaviour
{
    [SerializeField] private Vector2 projectileOffset;
    [SerializeField] private GameObject[] projectilePrefabs;
    [SerializeField] private Color[] colors;

    private SpriteRenderer _playerSpriteRenderer;
    
    private int currentShot = 0;

    private void Awake()
    {
        _playerSpriteRenderer = GetComponent<SpriteRenderer>();
        _playerSpriteRenderer.color = Color.red;
    }

    private void LateUpdate()
    {
        //changes color after animation may have changed colors
        //right now this sacrifices damage animation for seeing
        //current shot color
        _playerSpriteRenderer.color = colors[currentShot];
    }
    

    public void Fire(Vector2 direction)
    {
        if (projectilePrefabs.Length <= 0) return;
        
        var ball = 
            Instantiate(projectilePrefabs[currentShot], 
                (Vector2)transform.position + projectileOffset * transform.localScale,
                Quaternion.identity);
        ball.GetComponent<Projectile>()?.SetDirection(direction);
    }

    public void ReadyNext()
    {
        currentShot++;
        if (currentShot >= projectilePrefabs.Length) currentShot = 0;
    }
    
    private void OnDrawGizmos()
    {
        Vector2 position = (Vector2) transform.position + projectileOffset * transform.localScale;
        Gizmos.DrawIcon(position, "Animation.FilterBySelection" );
        //icon names can be found here: https://unitylist.com/p/5c3/Unity-editor-icons
    }
}
