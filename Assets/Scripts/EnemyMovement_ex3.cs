﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class EnemyMovement_ex3 : MonoBehaviour
{
    public NavMeshAgent agent;
    public GameObject target;
    public GameObject alarmObject;

    // wandering parameters
    public float wanderRadius;
    public float wanderDistance;
    public float wanderJitter;

    
    // Sight parameters
    public float maxAngle = 15f;
    public float maxRadius;
    private bool isSighted = false;
    
    // Start is called before the first frame update
    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
        isSighted = InFov(transform, target.transform, maxAngle, maxRadius);
        if (isSighted)
        {
            alarmObject.SetActive(true);
        }
        else
        {
            alarmObject.SetActive(false);
        }
        
    }
    
    public bool CanSeePlayer()
    {
        Vector3 direction = target.transform.position - agent.transform.position;
        float angle = Vector3.Angle(direction, agent.transform.forward);

        if (direction.magnitude < maxRadius && angle < maxAngle)
        {
            return true;
        }
        return false;
    }

    // Draw gizmos just to see the the enemy field of view
    private void OnDrawGizmos()
    {
        Vector3 fovLine1 = Quaternion.AngleAxis(maxAngle, transform.up) * transform.forward * maxRadius;
        Vector3 fovLine2 = Quaternion.AngleAxis(-maxAngle, transform.up) * transform.forward * maxRadius;

        // Field of view lines
        Gizmos.color = Color.cyan;
        Gizmos.DrawRay(transform.position, fovLine1);
        Gizmos.DrawRay(transform.position, fovLine2);

        // Ray to target
        if (!isSighted)
            Gizmos.color = Color.red;
        else
            Gizmos.color = Color.green;
        Gizmos.DrawRay(transform.position, (target.transform.position - transform.position).normalized * maxRadius);

        // Ray to forward sight
        Gizmos.color = Color.black;
        Gizmos.DrawRay(transform.position, transform.forward * maxRadius);
    }

    public static bool InFov(Transform checkObject, Transform target, float maxAngle, float maxRadius)
    {
        Collider[] overlaps = new Collider[30];
        int count = Physics.OverlapSphereNonAlloc(checkObject.position, maxRadius, overlaps);

        for (int i = 0; i < count; i++)
        {
            if (overlaps[i] != null)
            {
                // if the target position is the same as the i-th element of the overlap, get the angle between enemy's forward and target position
                if (overlaps[i].transform == target)
                {
                    Vector3 directionBetween = (target.position - checkObject.position).normalized;
                    directionBetween.y *= 0; // just to make sure y is not a factor

                    float angle = Vector3.Angle(checkObject.forward, directionBetween);
                    //Debug.Log("Angle detected: " + angle); -> manual debug

                    // if the angle is less than field of view
                    if (angle <= maxAngle)
                    {
                        Ray ray = new Ray(checkObject.position, target.position - checkObject.position);
                        RaycastHit hit;

                        // if the ray did not hit a wall, then it target is in Field Of View 
                        if (Physics.Raycast(ray, out hit, maxRadius))
                        {
                            if (hit.transform == target)
                            {
                                return true;
                            }
                        }

                    }
                }
            }
        }

        return false;
    }
    
}
