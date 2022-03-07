using System.Collections.Generic;
using UnityEngine;

public static class GameData
{
    public static int playerHpMax = 10;
    public static int playerHp = 10;
    public static int entranceNumber = -1;

    public static Dictionary<string, bool> openDoors = new Dictionary<string, bool>();

    public static void RecordClosedDoor(string doorName)
    {
        if (openDoors.ContainsKey(doorName)) return; 
        Debug.Log("Closed Door Recorded: " + doorName);
        openDoors.Add(doorName, false);
    }
}