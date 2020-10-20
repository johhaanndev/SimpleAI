using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class EnemyMovement_ex4 : MonoBehaviour
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
    
    // Chase parameters
    public float maxDistanceChase = 30f;
    private int startChaseCount = 1;

    // Ward Off parameters
    public float maxDistanceWardOff = 15f;
    private int startWardOffCount = 1;


    private enum State { wander, chase, wardOff}; // wander = 0, chase = 1, wardOff = 2
    private State state;
    private Animator sfm;


    public GameObject initialChasePoint;
    public GameObject initialWardOffPoint;

    
    // Start is called before the first frame update
    void Start()
    {
        sfm = GetComponent<Animator>();
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
        
        // State machine changes
        if (state == State.wander)
        {
            alarmObject.SetActive(false);
            Wander();
            startWardOffCount = 1;
            startChaseCount = 1;
            
            isSighted = InFov(transform, target.transform, maxAngle, maxRadius);
            if (isSighted)
            {
                state = State.chase;
                sfm.SetInteger("state", 1);
            }
            else
            {
                state = State.wander;
                sfm.SetInteger("state", 0);
            }
        }
        
        if (state == State.chase)
        {
            Chase();
            alarmObject.SetActive(true);
            if(startChaseCount == 1)
            {
                // set the reference position and add 1 to the count, this way the reference position won't be moving
                initialChasePoint.transform.position = agent.transform.position;
                startChaseCount++;
            }
            else
            {
                // when it reaches the distance, change to ward off
                if (Mathf.Abs((agent.transform.position - initialChasePoint.transform.position).magnitude) >= maxDistanceChase) 
                {
                    state = State.wardOff;
                    sfm.SetInteger("state", 2);
                }
            }
            
        }
        
        if(state == State.wardOff)
        {   
            WardOff();
            alarmObject.SetActive(false);

            if (startWardOffCount == 1)
            {
                // set the reference position and add 1 to the count, this way the reference position won't be moving
                initialWardOffPoint.transform.position = agent.transform.position;
                startWardOffCount++;
            }
            else
            {
                // when it reaches the distance, change to wander
                if (Mathf.Abs((agent.transform.position - initialWardOffPoint.transform.position).magnitude) >= maxDistanceWardOff)
                {
                    state = State.wander;
                    sfm.SetInteger("state", 0);
                }
            }
            
        }
        Debug.Log("SFM value: " + sfm.GetInteger("state"));
        
    }
    
    Vector3 wanderTarget = Vector3.zero;
    // Wandering state method
    void Wander()
    {
        wanderTarget += new Vector3(UnityEngine.Random.Range(-1.0f, 1.0f) * wanderJitter, 0, UnityEngine.Random.Range(-1.0f, 1.0f) * wanderJitter);

        wanderTarget.Normalize();
        wanderTarget *= wanderRadius;
        Vector3 targetLocal = wanderTarget + new Vector3(0, 0, wanderDistance);
        Vector3 targetWorld = gameObject.transform.InverseTransformVector(targetLocal);

        agent.SetDestination(targetWorld);
    }
    
    private void Chase()
    {
        agent.SetDestination(target.transform.position);
    }

    private void WardOff()
    {
        agent.SetDestination(-target.transform.position);
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
