﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class WaypointPath : MonoBehaviour
{

    [SerializeField] private List<Vector2> points;
    private int _currentPointIndex = 0;

    private void Awake()
    {
        var transforms = GetComponentsInChildren<Transform>(true);
        foreach (var t in transforms)
        {
            //if (t.gameObject != gameObject) //if you want to exclude the gameobject this is on
            points.Add(t.position);
        }

        //just in case there were no child objects
        //we add a single point at 0,0,0 to avoid issues
        if (points.Count <= 0)
        {
            points.Add(new Vector2(0, 0));
        }
    }

    private void OnDrawGizmos()
    {
        var transforms = GetComponentsInChildren<Transform>(true);

        if (transforms.Length >= 2)
        {
            
            for (int i = 0, j = 1; j < transforms.Length; i++, j++)
            {
                Gizmos.color = Color.magenta;
                Gizmos.DrawLine(transforms[i].position, transforms[j].position);
            }

            Gizmos.DrawLine(transforms[transforms.Length - 1].position, transforms[0].position);
        }
    }

    public Vector2 GetNextWaypointPosition()
    {
        var prevIndex = _currentPointIndex;
        _currentPointIndex++;
        if (_currentPointIndex >= points.Count) _currentPointIndex = 0;

        Debug.DrawLine(points[prevIndex], points[_currentPointIndex], Color.magenta, 1);
        
        return points[_currentPointIndex];
    }

    
    /*
    //probably let's not do this:
    public void ClaimAtNearestWaypoint(Vector2 fromPosition)
    {
        var nearestWaypointIndex = -1;
        var shortestDistance = (fromPosition - points[0]).magnitude;
        for (var i = 0; i < points.Count; i++)
        {
            var distance = (fromPosition - points[i]).magnitude;
            if (distance < shortestDistance)
            {
                shortestDistance = distance;
                nearestWaypointIndex = i;
            }
        }
        _currentPointIndex = nearestWaypointIndex;
    }
    */
}
