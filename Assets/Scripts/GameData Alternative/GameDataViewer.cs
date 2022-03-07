using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameDataViewer : MonoBehaviour
{
    [SerializeField] private int playerHp = 0;
    [SerializeField] private int playerHpMax = 0;
    [SerializeField] private int entranceNumber = 0;
    //[SerializeField] List<string> closedDoors;

    private void Start()
    {
        playerHp = GameManager.Instance.playerHp;
        playerHpMax = GameManager.Instance.playerHpMax;
        entranceNumber = GameManager.Instance.entranceNumber;
        
        /*
        var doors = FindObjectsOfType<DoorController>();
        foreach (var d in doors)
        {
            closedDoors.Add(d.doorName);
        }
    */
    }



}
