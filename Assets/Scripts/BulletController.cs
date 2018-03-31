using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour {

    public enum InventorType
    {
        Party,
        Enemy
    }

    [SerializeField]
    protected float lifeTime=5.0f;

    [SerializeField]
    protected GameObject hitEffectPrefab;

    public InventorType inventorType;
    protected bool initialized;

    protected int atk;

    void Awake () {
        initialized = false;
        GetComponent<Collider>().enabled = false;
    }

    virtual public void Init(InventorType inventorType, int atk = 2)
    {
        this.inventorType = inventorType;
        this.atk = atk;

        GetComponent<Collider>().enabled = true;
        initialized = true;

        Destroy(gameObject, lifeTime);
    }
    // Update is called once per frame
    //void Update () {

    //}

    void OnCollisionEnter(Collision collision)
    {
        if (!initialized)
            return;

        switch (collision.gameObject.tag)
        {
            case "Wall":
                {
                    if (hitEffectPrefab != null)
                    {
                        GameObject hitEffect = Instantiate(hitEffectPrefab, collision.contacts[0].point, transform.rotation);
                        Destroy(hitEffect, 3.0f);
                    }

                    Destroy(gameObject);
                    break;
                }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (!initialized)
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
                        other.transform.parent.GetComponent<Health>().DecreaseHealth(atk);
                        break;
                    }
                case "Wall":
                    {
                        
                        if (hitEffectPrefab != null)
                        {
                            GameObject hitEffect = Instantiate(hitEffectPrefab, transform.position, transform.rotation);
                            Destroy(hitEffect, 3.0f);
                        }                           
                      
                        Destroy(gameObject);
                        break;
                    }
            }
        }else if(inventorType == InventorType.Enemy)
        {
            switch (other.gameObject.tag)
            {
                case "PlayerHitCollider":
                    {                       
                        if (hitEffectPrefab != null)
                        {
                            GameObject hitEffect = Instantiate(hitEffectPrefab, transform.position, transform.rotation);
                            Destroy(hitEffect, 3.0f);
                        }
                        other.gameObject.transform.root.GetComponent<Player>().GetHit();
                        other.gameObject.transform.root.GetComponent<Health>().DecreaseHealth(atk);
                        Destroy(gameObject);
                        break;
                    }
                case "AllyHitCollider":
                    {
                        if (hitEffectPrefab != null)
                        {
                            GameObject hitEffect = Instantiate(hitEffectPrefab, transform.position, transform.rotation);
                            Destroy(hitEffect, 3.0f);
                        }

                        other.gameObject.transform.root.GetComponent<AllyController>().GetHit();
                        other.gameObject.transform.root.GetComponent<Health>().DecreaseHealth(atk);
                        Destroy(gameObject);
                        break;
                    }
                case "Wall":
                    {
                        if (hitEffectPrefab != null)
                        {
                            GameObject hitEffect = Instantiate(hitEffectPrefab, transform.position, transform.rotation);
                            Destroy(hitEffect, 3.0f);
                        }
                        Destroy(gameObject);
                        break;
                    }
            }
        }        

    }
}
