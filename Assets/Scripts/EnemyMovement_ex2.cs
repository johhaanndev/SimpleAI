using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class EnemyMovement_ex2 : MonoBehaviour
{
    public NavMeshAgent agent;
    public GameObject target;

    // wandering parameters
    public float wanderRadius;
    public float wanderDistance;
    public float wanderJitter;

    
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        Wander();
        
    }

    Vector3 wanderTarget = Vector3.zero;
    // Wandering state method
    void Wander()
    {
        wanderTarget += new Vector3(UnityEngine.Random.Range(-1.0f, 1.0f) * wanderJitter, 0, UnityEngine.Random.Range(-1.0f, 1.0f) * wanderJitter);

        wanderTarget.Normalize();
        wanderTarget *= wanderRadius;
        Vector3 targetLocal = wanderTarget + new Vector3(0, 0, wanderDistance); // target position of the Reynolds graph
        Vector3 targetWorld = gameObject.transform.InverseTransformVector(targetLocal); // actual position in the space ground

        agent.SetDestination(targetWorld);
    }
}
