using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireballController : BulletController {

    //public enum InventorType
    //{
    //    Party,
    //    Enemy
    //}

    //[SerializeField]
    //float lifeDuraiton = 5.0f;

    //InventorType inventorType;
    //bool initialized;

    [SerializeField]
    GameObject smoke;

    //[SerializeField]
    //GameObject explosionPrefab;

    void Awake()
    {
        //Debug.Log("FireballController.Awake!");
        initialized = false;
        GetComponent<SphereCollider>().enabled = false;
        smoke.SetActive(false);
    }

    override public void Init(InventorType inventorType, int atk=2)
    {
        this.inventorType = inventorType;
        this.atk = atk;

        GetComponent<SphereCollider>().enabled = true;
        smoke.SetActive(true);
        initialized = true;

        Destroy(gameObject, lifeTime);
    }
  
    //void OnCollisionEnter(Collision collision)
    //{
    //    if (!initialized)
    //        return;

    //    if (inventorType == InventorType.Enemy)
    //    {
    //        switch (collision.gameObject.tag)
    //        {
    //            case "Wall":
    //                {
    //                    Debug.Log("OnCollisionEnter! hit the wall!");
    //                    GameObject explosion = Instantiate(hitEffectPrefab, transform.position, transform.rotation);
    //                    Destroy(explosion, 3.0f);
    //                    Destroy(gameObject);
    //                    break;
    //                }
    //        }
    //    }
    //}

    void OnTriggerEnter(Collider other)
    {
        if (!initialized)
            return;

        if (inventorType == InventorType.Enemy)
        {
            switch (other.gameObject.tag)
            {
                case "PlayerHitCollider":
                    {
                        //Debug.Log("OnTriggerEnter! Player is hit!");
                        GameObject explosion = Instantiate(hitEffectPrefab, transform.position, transform.rotation);
                        Destroy(explosion, 3.0f);
                        other.gameObject.transform.root.GetComponent<Player>().GetHit();
                        other.gameObject.transform.root.GetComponent<Health>().DecreaseHealth(this.atk);
                        Destroy(gameObject);
                        break;
                    }
                case "AllyHitCollider":
                    {
                        //Debug.Log("OnTriggerEnter! Player is hit!");
                        GameObject explosion = Instantiate(hitEffectPrefab, transform.position, transform.rotation);
                        Destroy(explosion, 3.0f);

                        other.gameObject.transform.root.GetComponent<AllyController>().GetHit();
                        other.gameObject.transform.root.GetComponent<Health>().DecreaseHealth(this.atk);
                        Destroy(gameObject);
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
