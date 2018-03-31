using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour {

    [SerializeField]
    PerlinShake screenShaker;


    //[SerializeField]
    //Color bloodViewFlashColor;

    //[SerializeField]
    //float bloodViewFlashSpeed;

    [SerializeField]
    Health health;
    //[SerializeField]
    //float deadFadeOutSpeed = 1;

    bool dead;

    // Use this for initialization
    void Start () {
        dead = false;
        if (health == null)
        {
            health = GetComponent<Health>();
        }
	}
	
	// Update is called once per frame
	void Update () {
		if(health.health<=0 && dead == false)
        {
            Die();
        }
	}

    public void GetHit()
    {
        if (dead)
            return;
        screenShaker.PlayShake();
        FadingView.instance.FlashBloodView();
    }  

    void Die()
    {
        //Debug.Log("Player die!");
        dead = true;
        FadingView.instance.FadeOutBloodView();
        GameManager.instance.StartCoroutine(GameManager.instance.LoseGame());
    }
}
