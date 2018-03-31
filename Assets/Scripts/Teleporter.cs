using UnityEngine;
using System.Collections;

public class Teleporter : MonoBehaviour {
	
	public string teleportToMap = "BattleScene";

  
	void OnTriggerEnter(Collider other){
        Debug.Log("Teleporter.OnTriggerEnter!");
        if (other.tag == "PlayerPhysicCollider"){
            Debug.Log("is PlayerPhysicCollider!");
            Application.LoadLevel(teleportToMap);
        }
	}
}