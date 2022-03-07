using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using Vector2 = UnityEngine.Vector2;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(HealthSystem))]

public class EnemyController : MonoBehaviour
{

    [SerializeField] private float patrolChangeDelay = 1.5f;
    [SerializeField] private List<Vector2> patrolDirections = new List<Vector2>();
    [SerializeField] private float patrolSpeed = 3;
    
    private Rigidbody2D _rb;
    private WaypointPath _waypointPath;
    private Vector2 _patrolTargetPosition;
    private Animator _animator;
    private HealthSystem _healthSystem;
    
    // Awake is called before Start
    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _waypointPath = GetComponentInChildren<WaypointPath>();
        _healthSystem = GetComponent<HealthSystem>();
        _animator = GetComponent<Animator>();
    }

    // Start is called before the first frame update
    private IEnumerator Start()
    {
        if (_waypointPath)
        {
            _patrolTargetPosition = _waypointPath.GetNextWaypointPosition();
        }
        else
        {
            foreach (var dir in patrolDirections)
            {
                _rb.velocity = dir;
                yield return new WaitForSeconds(patrolChangeDelay);
            }
            StartCoroutine(nameof(Start));
        }
    }

    private void FixedUpdate()
    {
        if (!_waypointPath) return;

        //set our direction toward that waypoint:
        //subtracting our position from target position
        //gives us the slope line between the two
        //We can get direction by normalizing it
        //We can get distance by magnitude
        var dir = _patrolTargetPosition - (Vector2)transform.position;
        
        //if we are close enough to the target,
        //time to get the next waypoint
        if (dir.magnitude <= 0.1)
        {
            //get next waypoint
            _patrolTargetPosition = _waypointPath.GetNextWaypointPosition();
            
            //change direction
            dir = _patrolTargetPosition - (Vector2)transform.position;
        }

        //normalized reduces dir magnitude to 1, so we can
        //keep at the speed we want by multiplying
        _rb.velocity = dir.normalized * patrolSpeed; 
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

    //TakeDamage is called when the healthSystem
    //lets us know there was damage (Damaged event is raised)
    //we register for this in OnEnable below
    public void TakeDamage(int damageAmount)
    {
        //Debug.Log("damaged: " + damageAmount);
        _animator.Play("shared_damage");
    }

    public void AcceptDefeat()
    {
        GameEventDispatcher.TriggerEnemyDefeated();
        Destroy(gameObject);
    }

    //EVENT CODE
    private void OnEnable()
    {
        //Register us as a subscriber for the OnDamaged Event
        //and call our TakeDamage when OnDamaged is raised
        //also for OnZero to call AcceptDefeat
        
        /*
        _healthSystem.OnDamaged += TakeDamage;
        _healthSystem.OnZero += AcceptDefeat;
    */
    }

    private void OnDisable()
    {
        //Because this enemy will go away, we also need
        //to deregister! Otherwise, it will try to call
        //TakeDamage when there is no object.
        /*
        _healthSystem.OnDamaged -= TakeDamage;
        _healthSystem.OnZero -= AcceptDefeat;
    */
    }
}
