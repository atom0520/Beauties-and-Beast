using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class DragonBossController : MonoBehaviour {

    enum State { Idle, Chase, Attack, Dying, Dead }

    [SerializeField]
    GameObject fireBreathInstance;
    ParticleSystem.EmissionModule emision;
 
    GameObject target;
    Animator animator;

    [SerializeField]
    float fireBallCT = 3f;
    float fireBallCTCounter;
    [SerializeField]
    Transform castFireBallPoint;
    [SerializeField]
    GameObject fireBallPrefab;
    [SerializeField]
    float fireBallSpeed = 10.0f;
    [SerializeField]
    float castFireBallRange = 80.0f;

    [SerializeField]
    float maxAngleYSpeed = 120.0f;

    State state;
    GameObject currFireBall;

    bool isCastingFireBall;

    [SerializeField]
    float attack1Range = 10f;
    [SerializeField]
    float attack1CT = 3f;
    float attack1CTCounter;
    [SerializeField]
    GameObject attack1BulletPrefab;
    [SerializeField]
    GameObject castAttack1BulletPoint;
    bool isAttacking1;

    [SerializeField]
    float maxMoveSpeed = 60.0f;

    Health health;

    [SerializeField]
    float toleratedAngleDiff = 1f;

    [SerializeField]
    AudioSource attack1AudioSrc;

    [SerializeField]
    AudioSource castFireballAudioSrc;

    [SerializeField]
    AudioSource shoutAudioSrc;

    [SerializeField]
    int fireBallAtk = 10;

    [SerializeField]
    int attack1Atk = 20;

    NavMeshAgent agent;

    [SerializeField]
    Transform spawnPoint;
    [SerializeField]
    Transform destPoint;

    [SerializeField]
    int fixTargetProb = 25;
    [SerializeField]
    float fixTargetTime = 3;
    float fixTargetCounter;

    float fixAttack1Counter = 0;
    [SerializeField]
    int fixAttack1Prob = 50;

    [SerializeField]
    AudioSource dieAudioSrc;

    [SerializeField]
    float fixAttack1Time = 3f;

    // Use this for initialization
    void Start () {
        emision = fireBreathInstance.GetComponent<ParticleSystem>().emission;
        emision.rateOverTime = new ParticleSystem.MinMaxCurve(0);
        currFireBall = null;
        animator = GetComponent<Animator>();
        state = State.Idle;

        isCastingFireBall = false;
        isAttacking1 = false;

        health = GetComponent<Health>();
        agent = GetComponent<NavMeshAgent>();

        transform.position = spawnPoint.position;
    }



    // Update is called once per frame
    void Update()
    {
        if (GameManager.instance.gameState == GameManager.GameState.BeforeDragonCome)
        {
            Vector3 diff = destPoint.position - transform.position;
            Vector3 diffH = diff;
            diffH.y = 0;
            if(diffH.magnitude > 20)
            {
                Vector3 destPos = destPoint.position;
                destPos.y = transform.position.y;
                transform.position = Vector3.MoveTowards(transform.position, destPos, maxMoveSpeed * Time.deltaTime);
                animator.SetBool("flying", true);
                
            }
            else
            {
                if(diff.magnitude > 0.2f)
                {
                    Vector3 destPos = destPoint.position;
                    transform.position = Vector3.MoveTowards(transform.position, destPos, maxMoveSpeed * Time.deltaTime);
                    animator.SetBool("flying", true);
                    shoutAudioSrc.loop = false;
                }
                else
                {
                    GameManager.instance.gameState = GameManager.GameState.AfterDragonCome;
                    GameManager.instance.StartCoroutine(GameManager.instance.StartFightingDragon());
                    animator.SetBool("flying", false);
                    //agent.enabled = true;
                }        
            }
        }


        if (GameManager.instance.gameState != GameManager.GameState.FightDragon)
            return;

        if (state == State.Dead)
            return;

        if(health.health <= 0)
        {
            Die();
            return;
        }

        switch (state)
        {
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

                    if (!isCastingFireBall && !isAttacking1)
                    {

                        float targetDist = 0f;

                        if (fixTargetCounter <= 0)
                        {
                            
                            FindClosestTarget(out this.target, out targetDist);

                            if (Random.Range(0, 100) < fixTargetProb)
                            {
                                //Debug.Log("Dragon fix target:"+target);
                                fixTargetCounter = fixTargetTime+Random.Range(-1.5f,1.5f);
                            }

                            if (target == null)
                            {
                                state = State.Idle;
                                break;
                            }
                        }
                        else
                        {
                            if (target.transform.root.GetComponent<Health>().health<=0)
                            {
                                target = null;
                                state = State.Idle;
                                break;
                            }

                            Vector3 diff = target.transform.position - transform.position;
                            diff.y = 0;
                            targetDist = diff.magnitude;
                            //Debug.Log("fixed target Dist:" + targetDist);
                           
                        }                      

                        float currAngleX = transform.rotation.eulerAngles.x;
                        float currAngleY = transform.rotation.eulerAngles.y;
                        float currAngleZ = transform.rotation.eulerAngles.z;

                        transform.LookAt(target.transform);

                        float destAngleY = transform.rotation.eulerAngles.y;

                        bool turning = false;
                        if (Mathf.Abs(destAngleY - currAngleY) > toleratedAngleDiff)
                        {                           
                            turning = true;
                            transform.rotation = Quaternion.Euler(0, Mathf.MoveTowardsAngle(currAngleY, destAngleY, Time.deltaTime * maxAngleYSpeed), 0);
                            animator.SetBool("walking", true);
                        }
                        else
                        {                           
                            transform.rotation = Quaternion.Euler(0, currAngleY, 0);
                            animator.SetBool("walking", false);
                        }

                        bool moving = false;
                        if(targetDist <= attack1Range)
                        {
                            if (!turning && attack1CTCounter <= 0)
                            {
                                //agent.isStopped = true;
                                //agent.velocity = Vector3.zero;

                                animator.SetBool("running", false);
                                animator.SetTrigger("attack1");
                           
                                isAttacking1 = true;
                                Invoke("OnAttack1End", 0.667f / 0.6f);
                                attack1CTCounter = attack1CT;
                            }
                        }
                        else 
                        {    
                            if(fixAttack1Counter > 0)
                            {
                                Vector3 destPos = target.transform.position;
                                destPos.y = transform.position.y;
                                transform.position = Vector3.MoveTowards(transform.position, destPos, maxMoveSpeed * Time.deltaTime);

                                animator.SetBool("running", true);
                                moving = true;
                            }
                            else
                            {
                                if (targetDist > castFireBallRange)
                                {
                                    //agent.isStopped = false;
                                    //agent.SetDestination(target.transform.position);

                                    Vector3 destPos = target.transform.position;
                                    destPos.y = transform.position.y;
                                    transform.position = Vector3.MoveTowards(transform.position, destPos, maxMoveSpeed * Time.deltaTime);

                                    animator.SetBool("running", true);
                                    moving = true;
                                }
                                else
                                {
                                    animator.SetBool("running", false);

                                    if (!turning && !moving && fireBallCTCounter <= 0)
                                    {
                                        //CastFireBall();
                                        currFireBall = GameObject.Instantiate(fireBallPrefab, castFireBallPoint.position, castFireBallPoint.rotation);
                                        currFireBall.transform.parent = castFireBallPoint;

                                        animator.SetTrigger("castFireBall");
                                        animator.SetBool("walking", false);

                                        isCastingFireBall = true;
                                        Invoke("OnCastFireBallEnd", 2.0f);
                                        fireBallCTCounter = fireBallCT;
                                    }
                                }

                                if (Random.Range(0, 100) < fixAttack1Prob)
                                {
                                    fixAttack1Counter = fixAttack1Time;
                                }
                            }                           
                        }                                                                      
                    }                    
                    break;
                }
        }

        if (fixTargetCounter > 0)
            fixTargetCounter -= Time.deltaTime;

        if (fixAttack1Counter > 0)
            fixAttack1Counter -= Time.deltaTime;

        if (fireBallCTCounter > 0)
        {
            fireBallCTCounter -= Time.deltaTime;
        }

        if (attack1CTCounter > 0)
        {
            attack1CTCounter -= Time.deltaTime;
        }
    }

    public void ShowFireBreath()
    {
        emision.rateOverTime = new ParticleSystem.MinMaxCurve(500f);
    }

    public void HideFireBreath()
    {
        emision.rateOverTime = new ParticleSystem.MinMaxCurve(0);
    }

    public void ReleaseFireBall()
    {
        shoutAudioSrc.Play();
        castFireballAudioSrc.Play();        
        currFireBall.transform.parent = null;
        currFireBall.GetComponent<FireballController>().Init(FireballController.InventorType.Enemy, fireBallAtk);
        //Debug.Log("target.transform.position:" + target.transform.position);
        currFireBall.transform.LookAt(target.transform);
        currFireBall.GetComponent<Rigidbody>().velocity = currFireBall.transform.forward * fireBallSpeed;
        currFireBall = null;
    }

    public void OnCastFireBallEnd()
    {
        //Debug.Log("OnCastFireBallEnd!");
        isCastingFireBall = false;
    }

    public void CastAttack1Bullet()
    {
        //Debug.Log("CastAttack1Bullet!");
        attack1AudioSrc.Play();
        GameObject attack1Bullet = Instantiate(attack1BulletPrefab, castAttack1BulletPoint.transform.position, castAttack1BulletPoint.transform.rotation);
        attack1Bullet.GetComponent<BulletController>().Init(BulletController.InventorType.Enemy, attack1Atk);
        attack1Bullet.transform.parent = castAttack1BulletPoint.transform;
    }

    public void OnAttack1End()
    {
        //Debug.Log("OnAttack1End!");
        isAttacking1 = false;
    }

    void FindClosestTarget(out GameObject return1, out float return2)
    {
        GameObject[] targets = GameObject.FindGameObjectsWithTag("Ally");
        
        
        GameObject closestTarget = GameObject.FindGameObjectWithTag("Player");
        Vector3 diff = closestTarget.transform.position - transform.position;
        diff.y = 0;
        float closestDist = diff.sqrMagnitude;

        if (closestTarget.transform.root.GetComponent<Health>().health <= 0)
        {
            closestTarget = null;
            closestDist = Mathf.Infinity;
        }
        
        foreach(GameObject target in targets)
        {
            if (target.transform.root.GetComponent<Health>().health <= 0)
                continue;

            diff = target.transform.position - transform.position;
            diff.y = 0;
            float dist = diff.sqrMagnitude;
            if(dist < closestDist)
            {
                closestTarget = target;
                closestDist = dist;
            }
        }

        return1 = closestTarget;
        return2 = closestDist;
    }

    void Die()
    {
        state = State.Dead;
        Collider[] colliders = GetComponentsInChildren<Collider>();
        foreach (Collider collider in colliders)
        {
            collider.enabled = false;
        }
        health.hpBarController.gameObject.SetActive(false);

        animator.SetBool("dead",true);

        dieAudioSrc.Play();

        GameManager.instance.StartCoroutine(GameManager.instance.WinGame());
    }

    public void GetHit()
    {
        if(!isAttacking1 && !isCastingFireBall && (state != State.Dead))
        {
            
            //Debug.Log("animator.SetTrigger(hit)!");
            animator.SetTrigger("hit");
        }
           
    }
}
