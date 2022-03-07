using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerSceneNavigator : MonoBehaviour
{
    private PlayerData playerData;

    private void Start()
    {
        playerData = new PlayerData();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        var exiter = other.transform.GetComponent<ExitTrigger>();
        
        if (exiter)
        {
            //Update entranceNumber for next scene
            GameManager.Instance.entranceNumber = exiter.GetSceneEntranceNumber();
            
            //Send out an event to every object that cares about the stage ending
            //for instance, player will need to store health in GameData...
            GameEventDispatcher.TriggerSceneExited();

            SceneManager.LoadScene(exiter.GetScene());

            //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}
