using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour {

    enum State { Idle, Chase, Attack, Dying, Dead}

    NavMeshAgent agent;
    GameObject target;
    Animator animator;

    State state;

    [SerializeField]
    float attackRange = 4.0f;

    [SerializeField]
    float fireCoolTime = 1.5f;
    float fireCTCounter;

    [SerializeField]
    GameObject bulletPrefab;
    [SerializeField]
    Transform gunBarrelEnd;
    [SerializeField]
    float fireSpeed = 60.0f;

    // Use this for initialization
    void Start () {
        agent = GetComponent<NavMeshAgent>();
        
        animator = GetComponent<Animator>();
        state = State.Idle;
    }
	
	// Update is called once per frame
	void Update () {
        switch (state)
        {
            case State.Idle:
                //Debug.Log("Enemy is idle");
                if (target != null)
                {
                    state = State.Chase;
                    animator.SetBool("isMoving", true);
                    agent.isStopped = false;
                    break;
                }

                target = GameObject.FindGameObjectWithTag("Player");

                break;

            case State.Chase:
                {
                    //Debug.Log("Enemy is chasing");
                    if (target == null)
                    {
                        agent.isStopped = true;
                        state = State.Idle;
                        animator.SetBool("isMoving", false);
                        break;
                    }

                    transform.LookAt(target.transform);
                    transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0);

                    Vector3 diff = target.transform.position - transform.position;
                    diff.y = 0;
                    float dist = diff.magnitude;

                    if (dist > attackRange)
                    {
                        agent.SetDestination(target.transform.position);
                    
                    }
                    else
                    {
                        agent.isStopped = true;
                        animator.SetBool("isMoving", false);
                        animator.SetBool("isAttacking", true);
                        state = State.Attack;
                    }

                    break;
                }
            case State.Attack:
                {
                    //Debug.Log("Enemy is attacking");
                    if (target == null)
                    {
                        state = State.Idle;
                        animator.SetBool("isAttacking", false);
                        break;
                    }

                    transform.LookAt(target.transform);
                    transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0);
                    
                    Vector3 diff = target.transform.position - transform.position;
                    diff.y = 0;
                    float dist = diff.magnitude;

                    if (dist > attackRange)
                    {
                        state = State.Chase;
                        animator.SetBool("isAttacking", false);
                        animator.SetBool("isMoving", true);
                        agent.isStopped = false;
                        break;
                    }

                    if (fireCTCounter < 0)
                    {
                        Fire();
                       fireCTCounter = fireCoolTime;
                    }
                    else
                    {
                        fireCTCounter -= Time.deltaTime;
                    }

                    break;
                }
        }
       
        
    }

    void Fire()
    {
        GameObject bullet = GameObject.Instantiate(bulletPrefab, gunBarrelEnd.position, gunBarrelEnd.rotation);
        bullet.GetComponent<BulletController>().Init(BulletController.InventorType.Enemy);

        bullet.transform.LookAt(target.transform);
        bullet.GetComponent<Rigidbody>().velocity = bullet.transform.forward * fireSpeed;
    }
}
