using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookoutBulletsCollider : MonoBehaviour {

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<BulletController>() == null)
        {
            //Debug.Log("no BulletController! " + other.gameObject);
            return;
        }

        //Debug.Log("LookoutBulletsCollider.OnTriggerEnter:" + other.gameObject);
        if(other.GetComponent<BulletController>().inventorType == BulletController.InventorType.Enemy)
        {
            Vector3 bulletDir = other.transform.position - transform.position;
            bulletDir.y = 0;
            Vector3 dodgeDir = new Vector3(bulletDir.z, 0, -bulletDir.x);
            transform.root.GetComponent<AllyController>().QuickDodgeInDir(dodgeDir);
        }
    }
}
