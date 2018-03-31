using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunController : MonoBehaviour {

    [SerializeField]
    GameObject bulletPrefab;

    [SerializeField]
    OVRInput.Button fireButton = OVRInput.Button.One;

    [SerializeField]
    KeyCode fireKey = KeyCode.Space;

    [SerializeField]
    float fireCoolTime = 1.0f;
    float fireCoolCounter;

    [SerializeField]
    Transform target;

    [SerializeField]
    Transform muzzle;

    [SerializeField]
    float fireSpeed = 60.0f;

    [SerializeField]
    GameObject gunFireEffectPrefab;

    [SerializeField]
    AudioSource castBulletAudioSrc;

    [SerializeField]
    int atk = 2;

    // Update is called once per frame
    void Update () {
        
        if(fireCoolCounter <= 0)
        {
            if (OVRInput.GetDown(fireButton) || Input.GetKey(fireKey))
            {
                Fire();
                fireCoolCounter = fireCoolTime;

            }
        }
        else
        {
            fireCoolCounter -= Time.deltaTime;
        }
		
	}

    void Fire()
    {
        GameObject bullet = Instantiate(bulletPrefab, muzzle.position, muzzle.rotation);
        bullet.GetComponent<BulletController>().Init(BulletController.InventorType.Party, this.atk);

        GameObject gunFireEffect = Instantiate(gunFireEffectPrefab, muzzle.position, muzzle.rotation);
        
        gunFireEffect.transform.parent = muzzle;
        gunFireEffect.transform.localScale = new Vector3(0.25f, 0.25f, 0.25f);

        castBulletAudioSrc.Play();

        Destroy(gunFireEffect, 0.25f);

        bullet.transform.LookAt(target);        
        bullet.GetComponent<Rigidbody>().velocity = bullet.transform.forward * fireSpeed;

    }
}
