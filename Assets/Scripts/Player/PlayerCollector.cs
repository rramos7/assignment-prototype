using System;
using UnityEngine;


[RequireComponent(typeof(HealthSystem))]
public class PlayerCollector : MonoBehaviour
{
    [SerializeField] private int foodHpBoost;

    private HealthSystem _healthSystem;

    public int slimeCount { get; private set; }
    //private int _slimeCountGoal;


    private void Start()
    {
        slimeCount = GameManager.Instance.playerSlimeCount;
        _healthSystem = GetComponent<HealthSystem>();
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.transform.CompareTag("Slime"))
        {
            Destroy(other.gameObject);
            slimeCount++;
            GameEventDispatcher.TriggerSlimeCollected();
        }

        if (other.transform.CompareTag("Food"))
        {
            Destroy(other.gameObject);
            _healthSystem.Heal(5);
        }
    }

    public bool SpendSlimes(int amt)
    {
        if (amt <= slimeCount)
        {
            slimeCount -= amt;
            return true;
        }

        return false;
    }

    private void WriteData()
    {
        GameManager.Instance.playerSlimeCount = slimeCount;
    }

    public void OnEnable()
    {
        GameEventDispatcher.SceneExited += WriteData;
    }

    public void OnDisable()
    {
        GameEventDispatcher.SceneExited -= WriteData;
    }
}