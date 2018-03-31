using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AllyController : MonoBehaviour {

    enum State { Idle, Chase, Attack, Dead, Dodge}
    enum BulletType { GunBullet, Arrow, MagicBall}

    [SerializeField]
    BulletType bulletType;

    [SerializeField]
    Animator animator;

    GameObject target;
    State state;

    [SerializeField]
    float castBulletCT;
    float castBulletCTCounter;
    [SerializeField]
    float castBulletRange;
    [SerializeField]
    Transform castBulletPoint;
    [SerializeField]
    float castBulletSpeed = 10.0f;

    bool isCastingBullet;

    [SerializeField]
    float maxAngleYSpeed = 120.0f;

    [SerializeField]
    float maxMoveSpeed = 10.0f;

    [SerializeField]
    GameObject bulletPrefab;
    GameObject currBullet;

    [SerializeField]
    float castBulletTime = 2.0f;

    [SerializeField]
    float toleratedAngleDiff = 1f;

    [SerializeField]
    GameObject castBulletEffectPrefab;

    [SerializeField]
    AudioSource castBulletAudioSrc;

    Health health;

    NavMeshAgent agent;

    float dodgeCounter = 0;

    [SerializeField]
    float minDodgeTime = 0.5f;
    [SerializeField]
    float maxDodgeTime = 2f;

    [SerializeField]
    float minKeptDist = 15f;

    [SerializeField]
    AudioSource dieAudioSrc;

    void Awake()
    {
        health = GetComponent<Health>();
        agent = GetComponent<NavMeshAgent>();
    }

    void Start () {
		
	}
	
	// Update is called once per frame
	void Update ()
    {

        if (state == State.Dead)
            return;

        if (health.health <= 0)
        {
            Die();
            return;
        }

        if (GameManager.instance.gameState == GameManager.GameState.BeforeDragonCome || GameManager.instance.gameState == GameManager.GameState.AfterDragonCome)
        {
            return;
        }

        switch (state)
        {
            case State.Dodge:
                transform.position = transform.position + transform.forward * maxMoveSpeed * Time.deltaTime;
                animator.SetBool("moving", true);

                if (dodgeCounter <= 0)
                {
                    state = State.Idle;
                    animator.SetBool("moving", false);
                }
                else
                {
                    dodgeCounter -= Time.deltaTime;
                }
                break;
            case State.Idle:
                {                 
                    if (target != null)
                    {
                        state = State.Attack;
                        break;
                    }

                    float targetDist = 0f;
                    FindClosestTarget(out this.target, out targetDist);

                    break;
                }
            case State.Attack:
                {
                    //if (target == null)
                    //{
                    //    state = State.Idle;
                    //    break;
                    //}

                    if (!isCastingBullet)
                    {
                        float targetDist = 0f;
                        FindClosestTarget(out this.target, out targetDist);

                        if (target == null)
                        {
                            state = State.Idle;
                            agent.velocity = Vector3.zero;
                            agent.isStopped = true;
                            animator.SetBool("moving", false);
                            break;
                        }

                        if (targetDist < minKeptDist)
                        {
                            QuickDodgeInDir(transform.position - target.transform.position);
                            break;
                        }

                        bool turning = false;

                        float currAngleY = transform.rotation.eulerAngles.y;         

                        transform.LookAt(target.transform);
                        transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0);
                        //float destAngleY = transform.rotation.eulerAngles.y;

                        //if (Mathf.Abs(destAngleY - currAngleY) > toleratedAngleDiff)
                        //{
                        //    turning = true;
                        //    transform.rotation = Quaternion.Euler(0, Mathf.MoveTowardsAngle(currAngleY, destAngleY, Time.deltaTime * maxAngleYSpeed), 0);
                        //}
                        //else
                        //{
                        //    transform.rotation = Quaternion.Euler(0, currAngleY, 0);
                        //}                      

                        bool moving = false;
                        
                        if (targetDist > castBulletRange)
                        {
                            moving = true;
                            agent.isStopped = false;
                            agent.SetDestination(target.transform.position);
                            //Vector3 destPos = target.transform.position;
                            //destPos.y = transform.position.y;
                            //transform.position = Vector3.MoveTowards(transform.position, destPos, maxMoveSpeed * Time.deltaTime);
                            animator.SetBool("moving", true);
                        }
                        else
                        {
                            agent.velocity = Vector3.zero;
                            agent.isStopped = true;
                            animator.SetBool("moving", false);
                        }
                      
                        if (!turning && !moving && castBulletCTCounter <= 0)
                        {
                            animator.SetBool("moving", false);
                            animator.SetTrigger("castBullet");                    
                            isCastingBullet = true;

                            switch (bulletType)
                            {
                                case BulletType.GunBullet:
                                    {
                                        GameObject bullet = Instantiate(bulletPrefab, castBulletPoint.position, castBulletPoint.rotation);
                                        bullet.GetComponent<BulletController>().Init(BulletController.InventorType.Party);

                                        if (castBulletEffectPrefab != null)
                                        {
                                            GameObject castBulletEffect = Instantiate(castBulletEffectPrefab, castBulletPoint.position, castBulletPoint.rotation);
                                            castBulletEffect.transform.parent = castBulletPoint;
                                            Destroy(castBulletEffect, 0.25f);
                                        }

                                        castBulletAudioSrc.Play();

                                        bullet.transform.LookAt(target.transform);
                                        bullet.GetComponent<Rigidbody>().velocity = bullet.transform.forward * castBulletSpeed;
                                        break;
                                    }
                                case BulletType.Arrow:
                                    {
                                        currBullet = Instantiate(bulletPrefab, castBulletPoint.position, castBulletPoint.rotation);
                                        currBullet.transform.localScale = new Vector3(currBullet.transform.localScale.x * 2f, currBullet.transform.localScale.y * 2f, currBullet.transform.localScale.z * 2f);
                                        currBullet.transform.parent = castBulletPoint;
                                        
                                        Invoke("ReleaseBullet", 0.25f);
                                        break;
                                    }
                                case BulletType.MagicBall:
                                    {
                                        currBullet = Instantiate(bulletPrefab, castBulletPoint.position, castBulletPoint.rotation);
                                        currBullet.transform.parent = castBulletPoint;
                                        currBullet.GetComponent<MagicBallController>().Init(
                                            BulletController.InventorType.Party, gameObject);
                                        Invoke("ReleaseBullet",0);
                                        break;
                                    }
                            }
                          

                            Invoke("OnCastBulletEnd", castBulletTime);
                            castBulletCTCounter = castBulletCT;
                        }
                    }

                    break;
                }
        }

        if(castBulletCTCounter > 0)
        {
            castBulletCTCounter -= Time.deltaTime;
        }
	}

    void FindClosestTarget(out GameObject return1, out float return2)
    {    
        GameObject[] targets = GameObject.FindGameObjectsWithTag("Enemy");

        GameObject closestTarget = null;
        float closestDist = Mathf.Infinity;

        //Debug.Log("targets[0]:" + targets[0]);
        //Debug.Log("targets[1]:" + targets[1]);
        foreach (GameObject target in targets)
        {

            if (target.transform.root.GetComponent<Health>().health <= 0)
                continue;

            Vector3 diff = target.transform.position - transform.position;
            diff.y = 0;
            float dist = diff.sqrMagnitude;
            if (dist < closestDist)
            {
                closestTarget = target;
                closestDist = dist;
            }
        }

        return1 = closestTarget;
        return2 = closestDist;
    }

    public void ReleaseBullet()
    {
        switch (bulletType)
        {
            case BulletType.Arrow:
                {
                    currBullet.transform.parent = null;
                    currBullet.transform.LookAt(target.transform);
                    currBullet.GetComponent<ArrowController>().Cast(castBulletSpeed);
                    currBullet.GetComponent<Rigidbody>().useGravity = false;

                    castBulletAudioSrc.Play();
                    
                    currBullet = null;           

                    break;
                }
            case BulletType.MagicBall:
                {
                    //currBullet.transform.parent = null;
                    currBullet.GetComponent<MagicBallController>().Cast();

                    castBulletAudioSrc.Play();

                    currBullet.transform.LookAt(target.transform);
                    currBullet.GetComponent<Rigidbody>().velocity = currBullet.transform.forward * castBulletSpeed;
                    currBullet = null;
                    break;
                }
        }
     
    }

    void OnCastBulletEnd()
    {
        //Debug.Log("OnCastBulletEnd!");
        isCastingBullet = false;
    }

    //void OnCollisionEnter(Collision collision)
    //{
    //    Debug.Log(gameObject+" Ally OnCollisionEnter: " +collision.gameObject);
    //}

    public void GetHit()
    {
        if (!isCastingBullet && (state != State.Dead))
        {
            animator.SetTrigger("hit");
        }
    }

    void Die()
    {
        state = State.Dead;

        dieAudioSrc.Play();

        Destroy(GetComponent<Rigidbody>());

        Collider[] colliders = GetComponentsInChildren<Collider>();
        foreach (Collider collider in colliders)
        {
            collider.enabled = false;
        }
        health.hpBarController.gameObject.SetActive(false);

        animator.SetBool("dead", true);
    }

    public void QuickDodgeInDir(Vector3 dir)
    {
     
        state = State.Dodge;
        transform.forward = dir;
        dodgeCounter = Random.Range(minDodgeTime, maxDodgeTime);
        
    }
}
