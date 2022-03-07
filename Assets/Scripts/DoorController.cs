using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DoorController : MonoBehaviour
{
    [SerializeField] private string doorName = "Make Each Name Unique!";
    [SerializeField] private int requiredSlimeCount = 3;
    [SerializeField] private GameObject blockingObject;
    [SerializeField] private GameObject speechBubble;
    [SerializeField] private TextMeshProUGUI amountText;
    [SerializeField] private GameObject[] confinerObjectsToOpen;

    public void Awake()
    {
        GameManager.Instance.RecordClosedDoor(doorName);
        amountText.text = "" + requiredSlimeCount;
    }

    public void Start()
    {
        if (GameManager.Instance.openDoors[doorName])
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.transform.CompareTag("Player"))
        {
            //Check for necessary slimes
            var collector = other.transform.GetComponent<PlayerCollector>();
            if (!collector) return;

            if (collector.SpendSlimes(requiredSlimeCount))
            {
                OpenDoor();
            }
            else
            {
                speechBubble.SetActive(true);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.transform.CompareTag("Player"))
        {
            speechBubble.SetActive(false);
        }
    }

    private void OpenDoor()
    {
        GameManager.Instance.openDoors[doorName] = true;

        foreach (var o in confinerObjectsToOpen)
        {
            o.SetActive(true);
        }
        
        //this really should be much nicer with animation and a jingle, etc.
        Destroy(gameObject);
    }
}
