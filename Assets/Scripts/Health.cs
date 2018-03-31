using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Health : MonoBehaviour {

    [SerializeField]
    int maxHealth = 50;

    public int health {  get; private set; }

    [SerializeField]
    public ProgressBarController hpBarController;

    [SerializeField]
    Text healthValueText;

    void Start () {
        health = maxHealth;
        if (hpBarController != null)
        {
            hpBarController.SetValue((float)health / maxHealth);
        }  

        if (healthValueText != null)
        {
            healthValueText.text = health.ToString() + " / " + maxHealth.ToString();
        }
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void DecreaseHealth(int value)
    {
        health -= value;
        //Debug.Log("health:" + health);
        health = Mathf.Clamp(health, 0, maxHealth);

        if (hpBarController != null)
        {
            hpBarController.SetValue((float)health / maxHealth);
        }

        if (healthValueText != null)
        {
            healthValueText.text = health.ToString() + " / " + maxHealth.ToString();
        }

    }
}
