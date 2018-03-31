using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class FollowMaster : MonoBehaviour {

    NavMeshAgent agent;
    [SerializeField]
    Transform target;
    [SerializeField]
    Animator animator;
    [SerializeField]
    float keptDist = 4.0f;

    // Use this for initialization
    void Start () {
        agent = GetComponent<NavMeshAgent>();
        if (target == null)
        {
            target = GameObject.FindGameObjectWithTag("Player").transform;
        }
    }
	
	// Update is called once per frame
	void Update () {

        transform.LookAt(target);
        transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0);

        Vector3 diff = target.transform.position - transform.position;
        diff.y = 0;
        float dist = diff.magnitude;

        if (dist > keptDist)
        {
            agent.isStopped = false;
            agent.SetDestination(target.transform.position);
            animator.SetBool("moving", true);
        }
        else
        {
            agent.velocity = Vector3.zero;
            agent.isStopped = true;
            animator.SetBool("moving", false);
        }
    }
}
