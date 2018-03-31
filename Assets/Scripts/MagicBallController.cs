using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicBallController : BulletController {

    [SerializeField]
    TrailRenderer trailRenderer;
    [SerializeField]
    ParticleSystem flare;
    ParticleSystem.MainModule flareMain;
    [SerializeField]
    ParticleSystem flareSparks;
    ParticleSystem.EmissionModule flareSparksEmission;
    //[SerializeField]
    //float lifeTime = 6.0f;
    [SerializeField]
    float growSpeed = 60f;
    [SerializeField]
    GameObject healingEffectPrefab;

    GameObject inventor;

    //[SerializeField]
    //GameObject hitEffectPrefab;
    int maxAtk;
    int baseAtk;

    bool isCast;

    float percent;

    void Awake()
    {
        trailRenderer.enabled = false;
        flareMain = flare.main;
        flareSparksEmission = flareSparks.emission;
        GetComponent<Collider>().enabled = false;
        isCast = false;
    }

    void Start () {
		
	}

    //override public void Init(InventorType inventorType,GameObject inventor int baseAtk = 2)
    //{
    //    this.inventorType = inventorType;
    //    this.baseAtk = baseAtk;
    //    this.atk = baseAtk;
    //    this.maxAtk = 20;

    //    initialized = true;
    //}


    public void Init(InventorType inventorType, GameObject inventor, int baseAtk=2, int maxAtk=20)
    {
        this.inventorType = inventorType;
        this.inventor = inventor;
        this.baseAtk = baseAtk;
        this.atk = baseAtk;
        this.maxAtk = maxAtk;

        percent = 0;
        initialized = true;
    }

    // Update is called once per frame
    void Update () {
        if (!isCast)
        {
            if(percent < 1)
            {
                float growDegree = 1 * Time.deltaTime;
                percent = percent += growDegree;

                flareMain.startSize = new ParticleSystem.MinMaxCurve(0.2f+0.6f * percent);

                flareSparksEmission.rateOverTime = new ParticleSystem.MinMaxCurve(12 + 36 * percent);

                trailRenderer.widthMultiplier = 0.3f + 0.9f * percent;

                this.atk = this.baseAtk + (int)((maxAtk - baseAtk) * percent);

                GetComponent<SphereCollider>().radius = 0.06f + 0.18f * percent;
            }
            

            //if (flareMain.startSize.constant < 0.8f)
            //{
            //    flareMain.startSize = new ParticleSystem.MinMaxCurve(flareMain.startSize.constant + 0.01f * growDegree);
            //}
            //if (flareSparksEmission.rateOverTime.constant < 48)
            //{
            //    flareSparksEmission.rateOverTime = new ParticleSystem.MinMaxCurve(flareSparksEmission.rateOverTime.constant + 0.6f * growDegree);
            //}
            //if(trailRenderer.widthMultiplier < 1.2f)
            //{
            //    trailRenderer.widthMultiplier += 0.015f * growDegree;
            //}
            //if (this.atk < maxAtk)
            //{
            //    this.atk = (int)(this.atk + (maxAtk-baseAtk) /0.6f * (flareMain.startSize.constant-0.2f));
            //    Debug.Log("MagicBall.atk:"+ this.atk);
            //}
            //if (GetComponent<SphereCollider>().radius < 0.24f)
            //{
            //    GetComponent<SphereCollider>().radius += (0.24f / 80 * growDegree); 
            //}
        }
    }

    public void Cast()
    {
        isCast = true;
        trailRenderer.Clear();
        trailRenderer.enabled = true;
        transform.parent = null;
        GetComponent<Collider>().enabled = true;
        Destroy(gameObject, lifeTime);
    }

    //void OnCollisionEnter(Collision collision)
    //{
    //    if (!isCast)
    //        return;

    //    switch (collision.gameObject.tag)
    //    {
    //        case "Wall":
    //            {
    //                GameObject hitEffect = Instantiate(hitEffectPrefab, transform.position, transform.rotation);
    //                Destroy(hitEffect, 3.0f);
    //                Destroy(gameObject);

    //                //transform.GetComponent<Rigidbody>().isKinematic = true;
    //                break;
    //            }
    //    }
    //}

    void OnTriggerEnter(Collider other)
    {
        if (!isCast)
            return;

        if(inventorType == InventorType.Party)
        {
            switch (other.gameObject.tag)
            {
                case "DragonBossHitCollider":
                    {
                        if (hitEffectPrefab != null)
                        {
                            GameObject hitEffect = Instantiate(hitEffectPrefab, transform.position, transform.rotation);
                            Destroy(hitEffect, 3.0f);
                        }

                        Destroy(gameObject);

                        other.transform.parent.GetComponent<DragonBossController>().GetHit();
                        other.transform.parent.GetComponent<Health>().DecreaseHealth(this.atk);
                        break;
                    }
                case "AllyHitCollider":
                    {
                        if(other.transform.root.gameObject == inventor)
                        {
                            break;
                        }

                        if (hitEffectPrefab != null)
                        {
                            GameObject healingEffect = Instantiate(healingEffectPrefab, transform.position, transform.rotation);
                            Destroy(healingEffect, 1f);
                        }

                        Destroy(gameObject);
         
                        other.transform.parent.GetComponent<Health>().DecreaseHealth(-this.atk);
                        break;
                    }
                case "PlayerHitCollider":
                    {
                        if (other.transform.root.gameObject == inventor)
                        {
                            break;
                        }

                        if (hitEffectPrefab != null)
                        {
                            GameObject healingEffect = Instantiate(healingEffectPrefab, transform.position, transform.rotation);
                            Destroy(healingEffect, 1f);
                        }

                        Destroy(gameObject);

                        other.transform.parent.GetComponent<Health>().DecreaseHealth(-this.atk);
                        break;
                    }
                case "Wall":
                    {
                        //Debug.Log("OnTriggerEnter! hit the wall!");
                        GameObject explosion = Instantiate(hitEffectPrefab, transform.position, transform.rotation);
                        Destroy(explosion, 3.0f);
                        Destroy(gameObject);
                        break;
                    }
            }
        }
       
    }
}
