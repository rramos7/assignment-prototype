using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(ProjectileShooter))]
[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(HealthSystemPlayer))]
public class PlayerController : MonoBehaviour
{
    #region serialized fields exposed in editor

    [SerializeField] private float speed = 10f;
    [SerializeField] private float jumpForce = 20f;
    [SerializeField] private float gravityFall = 12f;
    [SerializeField] private float gravityFloat = 5f;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float groundCheckDistance = 0.0625f;
    
    #endregion
    
    #region component reference private fields
    
    private Rigidbody2D _rb;
    private BoxCollider2D _collider;
    private Animator _animator;
    private ProjectileShooter _shooter;
    private HealthSystemPlayer _healthSystem;
    
    #endregion

    #region input private fields

    private InputActions _inputActions; 
    private float _horizontalInput = 0f;
    private bool _isJumpDown = false;
    private bool _isJumpHeldDown = false;

    #endregion

    #region state private fields
    
    private bool _isFacingRight = true;
    private bool _isInKickBack = false;
    private bool _isFainting = false;

    #endregion
    private bool top;
    
    private void Awake()
    {
        _inputActions = new InputActions();
        _inputActions.Player.Enable();
        _rb = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
        _collider = GetComponent<BoxCollider2D>();
        _shooter = GetComponent<ProjectileShooter>();
        _healthSystem = GetComponent<HealthSystemPlayer>();
        
        //set fall gravity
        _rb.gravityScale = gravityFall;
        
        //Find entrance location and move to it if there is one
        var starts = FindObjectsOfType<Entrance>();
        foreach (var entrance in starts)
        {
            if (entrance.GetNumber() == GameManager.Instance.entranceNumber)
            {
                transform.position = entrance.transform.position;
            }
        }
    }

    private void Update()
    {
        if (_isFainting) return;
        
        //process buttons in Update
        //(changes involving physics rigidbodies go in FixedUpdate)
        if (_inputActions.Player.Fire.WasPerformedThisFrame())          
        {
            _shooter.Fire(new Vector2(_isFacingRight ? 1 : -1, 0));
            _healthSystem.SelfDamage(1);
        }

        if (_inputActions.Player.CycleShot.WasPerformedThisFrame())
        {
            _shooter.ReadyNext();
        }

        if (_inputActions.Player.Jump.WasPerformedThisFrame())
        {
            _isJumpDown = true;
        }

        _isJumpHeldDown = _inputActions.Player.Jump.IsPressed();


        _horizontalInput = _inputActions.Player.Move.ReadValue<Vector2>().x;
        
        //Change visuals based on Inputs
        
        if (_horizontalInput > 0 && !_isFacingRight) FlipSprite();
        if (_horizontalInput < 0 && _isFacingRight) FlipSprite();

        //.Equals() is used because floating-point precision rarely hits 0.0 exactly
        if (_horizontalInput.Equals(0))
        {
            _animator.Play("player_idle");
        }
        else if (_horizontalInput > 0 || _horizontalInput < 0)
        {
            _animator.Play("player_walk");
        }

    }
    
    private void FixedUpdate()
    {
        if (_isFainting) return;
        
        //Movement and Jumping
        if (!_isInKickBack)
        {
            _rb.velocity = new Vector2(_horizontalInput * speed, _rb.velocity.y);

            //Jump
            if (_isJumpDown)
            {
                _isJumpDown = false; //we have now used up this button press

                _rb.gravityScale *= -1;
                Rotation();

 
            }
            
            // if jump is held down
            // lower gravity to affect a lift while jumping up
            // otherwise gravity goes back to normal (gravityFall)
            /*if (_isJumpHeldDown && _rb.velocity.y > 0)
            {
                _rb.gravityScale = gravityFloat;
            }
            else
            {
                _rb.gravityScale = gravityFall;
            }*/
        }
    }

    void Rotation()
    {
        if(top == false)
        {
            transform.eulerAngles = new Vector3(0, 0, 180f);
        } else
        {
            transform.eulerAngles = Vector3.zero;
        }

        top = !top;
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

    private void FlipSprite()
    {
        _isFacingRight = !_isFacingRight; //invert
        
        Vector3 transformScale = transform.localScale;
        transformScale.x *= -1;
        transform.localScale = transformScale;
    }


    public void KickBack(Vector2 directionVector)
    {
        _rb.AddForce(directionVector, ForceMode2D.Impulse);
        _isInKickBack = true;
        Invoke(nameof(ReturnControl), .2f);
    }

    public void ReturnControl()
    {
        _isInKickBack = false;
    }

    public void TakeDamage()
    {
        if (_isFainting) return;
        
        _animator.Play("player_damage");
    }

    public void AcceptDefeat()
    {
        if (_isFainting) return;
        
        StartCoroutine(nameof(Faint));
    }

    public IEnumerator Faint()
    {
        _isFainting = true;
        _animator.Play("player_faint");
        yield return new WaitForSeconds(3);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    
    private void OnEnable()
    {
        /*
        _healthSystem.OnDamaged += TakeDamage;
        _healthSystem.OnZero += AcceptDefeat;
    */
    }

    private void OnDisable()
    {
        /*
        _healthSystem.OnDamaged -= TakeDamage;
        _healthSystem.OnZero += AcceptDefeat;
    */
    }
}
