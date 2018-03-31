using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowController : MonoBehaviour {

    [SerializeField]
    public float lifeTime = 3;

    [SerializeField]
    KeyCode attachArrowKey = KeyCode.Space;

    bool isAttached;
    public bool isShooted { set; get; }

    [SerializeField]
    GameObject arrowHitEffectPrefab;

    public int atk { set; get; }

    void Awake()
    {
        GetComponent<TrailRenderer>().enabled = false;
    }

    void Start()
    {
        isAttached = false;
        isShooted = false;

    }

    void Update()
    {
        if (isShooted)
        {
            if(transform.position == Vector3.zero)
            {
                Debug.Log("!transform.position:" + transform.position);
            }

            transform.LookAt(transform.position + transform.GetComponent<Rigidbody>().velocity);
          
        }    
    }

    void OnTriggerStay(Collider other)
    {
        if (!isShooted)
        {
            if (other.gameObject.tag == "Bow")
                TryAttachArrow();
        }           
    }

    void OnTriggerEnter(Collider other)
    {
        if (!isShooted)
        {
            switch (other.gameObject.tag)
            {
                case "Bow":
                    {
                        TryAttachArrow();
                        break;
                    }
            }
        }
        else
        {
            switch (other.gameObject.tag)
            {
                case "DragonBossHitCollider":
                    {
                        GameObject arrowHitEffect = Instantiate(arrowHitEffectPrefab, transform.position, transform.rotation);
                        Destroy(arrowHitEffect, 3.0f);
                        Destroy(gameObject);                       
                       
                        other.transform.parent.GetComponent<DragonBossController>().GetHit();
                        other.transform.parent.GetComponent<Health>().DecreaseHealth(this.atk);
                        break;
                    }
                case "Wall":
                    {
                        GameObject arrowHitEffect = Instantiate(arrowHitEffectPrefab, transform.position, transform.rotation);
                        Destroy(arrowHitEffect, 3.0f);
                        Destroy(gameObject);

                        break;
                    }
            }
        }
    }

    void TryAttachArrow()
    {
       
        if (!isAttached && OVRInput.Get(ArrowManager.instance.attachArrowButton))        
        {  
            ArrowManager.instance.AttachArrowToBow();

            isAttached = true;
       
        }
    }

    public void Cast(float speed)
    {
        isShooted = true;
        if (lifeTime > 0)
            Destroy(gameObject, lifeTime);
        Rigidbody rb = GetComponent<Rigidbody>();
        rb.isKinematic = false;
        rb.velocity = transform.forward * speed;
        rb.useGravity = true;
        GetComponent<TrailRenderer>().Clear();
        GetComponent<TrailRenderer>().enabled = true;
    }
    //void OnCollisionEnter(Collision collision)
    //{
    
    //    switch (collision.gameObject.tag)
    //    {
    //        case "Wall":
    //            {
    //                GameObject arrowHitEffect = Instantiate(arrowHitEffectPrefab, transform.position, transform.rotation);
    //                Destroy(arrowHitEffect, 3.0f);
    //                Destroy(gameObject);

    //                //transform.GetComponent<Rigidbody>().isKinematic = true;
    //                break;
    //            }
    //    }   
    //}
}
