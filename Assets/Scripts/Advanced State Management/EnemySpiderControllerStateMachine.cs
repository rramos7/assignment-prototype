using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(HealthSystem))]

public class EnemySpiderControllerStateMachine : StateMachine
{
    [SerializeField] private float walkSpeed;
    [SerializeField] private float patrolChangeDelay = 1.5f;
    [SerializeField] private GameObject spiderFoodPrefab;


    public Rigidbody2D rb { get; private set; }
    public HealthSystem healthSystem { get; private set; }
    public Animator animator { get; private set; }

    public EnemySpiderStateJump JumpState { get; private set; }

    public bool isFacingRight { get; private set; }

    //what you would normally put in Awake goes here
    protected override void InitContext()
    {
        rb = GetComponent<Rigidbody2D>();
        healthSystem = GetComponent<HealthSystem>();
        animator = GetComponent<Animator>();
        isFacingRight = true;
    }

    protected override void InitStates()
    {
        JumpState = new EnemySpiderStateJump(this);
        //PatrolState = new EnemySpiderStatePatrol(this);
        //ObserveState = new EnemySpiderStateObserver(this);
    }

    // Start is called before the first frame update
    private void Start()
    {
        rb.velocity = Vector2.left * walkSpeed;
    }

    public void FlipSprite()
    {
        isFacingRight = !isFacingRight; //invert

        Vector3 transformScale = transform.localScale;
        transformScale.x *= -1;
        transform.localScale = transformScale;
    }

    private void FixedUpdate()
    {
    }

    public void TakeDamage()
    {
        animator.Play("shared_damage");
    }

    public void AcceptDefeat()
    {
        GameEventDispatcher.TriggerEnemyDefeated();
        Destroy(gameObject);

        //instantiate spider food in place of spider
        if (!spiderFoodPrefab) return;

        var food = Instantiate(spiderFoodPrefab, transform.position, Quaternion.identity);
        if (!food) return;

        var foodRb = food.GetComponent<Rigidbody2D>();
        if (!foodRb) return;

        foodRb.velocity = rb.velocity;
        foodRb.gravityScale = rb.gravityScale;
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