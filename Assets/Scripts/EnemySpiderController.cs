using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(HealthSystem))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Rigidbody2D))]
public class EnemySpiderController : MonoBehaviour
{
    [SerializeField] private float walkSpeed = 3f;
    [SerializeField] private float jumpForce = 16f;
    [SerializeField] private float patrolChangeDelay = 1.5f;
    [SerializeField] private GameObject spiderFoodPrefab;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float groundCheckDistance = 0.0625f;
    
    

    #region component reference fields

    private Rigidbody2D _rb;
    private Animator _animator;
    private HealthSystem _healthSystem;
    private BoxCollider2D _collider;

    #endregion
    
    private bool _isFacingRight = true;
    private bool _isJumping = false;
    private bool _isWaiting = false;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _healthSystem = GetComponent<HealthSystem>();
        _animator = GetComponent<Animator>();
        _collider = GetComponent<BoxCollider2D>();
        _isFacingRight = true;
        _isJumping = false;
        _isWaiting = false;
    }

 

    // Start is called before the first frame update
    private IEnumerator Start()
    {

        while (true)
        {
            int randomStateChoice = Random.Range(1, 4);
            switch (randomStateChoice)
            {
                case 1:
                    //wait
                    _isWaiting = true;
                    _isJumping = false;
                    _rb.velocity = new Vector2(0f, _rb.velocity.y);
                    _animator.Play("spider_idle");
                    break;
                case 2:
                    //walk
                    _isWaiting = false;
                    _isJumping = false;
                    _animator.Play("spider_walk");
                    break;
                case 3:
                    //jump
                    _isWaiting = false;
                    _isJumping = true;
                    _animator.Play("spider_walk");
                    break;
            }
            
            float timeBeforeNextChange = Random.Range(0.1f, patrolChangeDelay);
            yield return new WaitForSeconds(timeBeforeNextChange);
        }
        yield break;
    }

    public void FlipSprite()
    {
        _isFacingRight = !_isFacingRight; //invert

        Vector3 transformScale = transform.localScale;
        transformScale.x *= -1;
        transform.localScale = transformScale;
    }


    private void Update()
    {
        //Visuals
        
        float horizontalSpeed = _rb.velocity.x;
        if (horizontalSpeed > 0 && !_isFacingRight
            || horizontalSpeed < 0 && _isFacingRight)
        {
            FlipSprite();
        }
    }

    private void FixedUpdate()
    {
        if (!_isWaiting)
        {
            //bounce off walls if against them
            if (isAgainstWall())
            {
                FlipSprite();
            }
            
            //move forward at walking speed
            _rb.velocity = new Vector2(walkSpeed * (_isFacingRight ? 1 : -1), _rb.velocity.y);
            
            //check for jump
            if (IsOnGround() && _isJumping)
            {
                _isJumping = false;
                _rb.velocity = new Vector2(_rb.velocity.x, jumpForce);
            }
        }


    }

    private bool isAgainstWall()
    {
        var bounds = _collider.bounds;
        float forwardX = _isFacingRight ? bounds.max.x : bounds.min.x;
        
        //var top = new Vector2(forwardX, bounds.max.y);
        
        var bottom = new Vector2(forwardX, bounds.min.y);
        
        /*
        RaycastHit2D hitTop = Physics2D.Raycast(top, 
            new Vector2((_isFacingRight? 1 : -1), 0), 
            groundCheckDistance, 
            groundLayer);
        */
        RaycastHit2D hitBottom = Physics2D.Raycast(bottom, 
            new Vector2((_isFacingRight? 1 : -1), 0),
            groundCheckDistance, 
            groundLayer);

        return hitBottom.collider; // hitBottom.collider || hitTop.collider;        
    }
    
    private bool IsOnGround()
    {
        var bounds = _collider.bounds;
        var bottomCornerRight = new Vector2(bounds.max.x, bounds.min.y);
        var bottomCornerLeft = new Vector2(bounds.min.x, bounds.min.y);
        
        RaycastHit2D hitRight = Physics2D.Raycast(bottomCornerRight,
            Vector2.down,
            groundCheckDistance,
            groundLayer);
        RaycastHit2D hitLeft = Physics2D.Raycast(bottomCornerLeft, 
            Vector2.down, 
            groundCheckDistance, 
            groundLayer);

        return hitRight.collider || hitLeft.collider;
    }
    
    private void OnDrawGizmos()
    {
        var bounds = GetComponent<BoxCollider2D>().bounds;
        
        var bottomCornerRight = new Vector2(bounds.max.x, bounds.min.y);
        var bottomCornerLeft = new Vector2(bounds.min.x, bounds.min.y);
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(bottomCornerRight,
            new Vector3(bottomCornerRight.x, bottomCornerRight.y - groundCheckDistance, 0f));
        Gizmos.DrawLine(bottomCornerLeft,
            new Vector3(bottomCornerLeft.x, bottomCornerLeft.y - groundCheckDistance, 0f));
    }

    public void TakeDamage()
    {
        _animator.Play("shared_damage");
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.transform.CompareTag("Player") == true)
        {
            other.transform.GetComponent<HealthSystem>()?.Damage(3);
            Vector2 awayDirection = other.transform.position - transform.position;
            other.transform.GetComponent<PlayerController>().KickBack(awayDirection * 3f);
        }
    }

    public void AcceptDefeat()
    {
        GameEventDispatcher.TriggerEnemyDefeated();
        Destroy(gameObject);

        //instantiate spider food in place of spider
        if (!spiderFoodPrefab) return;

        var food = Instantiate(spiderFoodPrefab, transform.position, Quaternion.identity);
        if (!food) return;

        //transfer physics to food
        var foodRb = food.GetComponent<Rigidbody2D>();
        if (!foodRb) return;

        foodRb.velocity = _rb.velocity;
        foodRb.gravityScale = _rb.gravityScale;
    }
}