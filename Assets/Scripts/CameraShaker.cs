using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShaker : MonoBehaviour {

    [SerializeField]
    float shakeSpeed = 10f;
    float shakeSpeedCounter;
    Vector3 shakeRange = new Vector3(2, 2, 2);
    float shakeTimer = 0f;
    [SerializeField]
    float shakeTime = 0.75f;
    [SerializeField]
    float shakeTimeScale = 0.5f;
    bool shake = false;

    Vector3 originalPosition;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.K))
        {
            CameraShake();
        }

        if (shake)
        {
            if(shakeTimer > shakeTime*Time.timeScale)
            {
                shakeTimer = 0;
                shake = false;
                Time.timeScale = 1;

                Camera.main.transform.position = new Vector3(originalPosition.x, originalPosition.y, originalPosition.z);
            }
            else
            {
                shakeTimer += Time.deltaTime;
                Camera.main.transform.position = originalPosition + Vector3.Scale(SmoothRandom.GetVector3(shakeSpeedCounter--), shakeRange);

                shakeSpeedCounter *= -1;

                shakeRange = new Vector3(shakeRange.x * -1, shakeRange.y * -1);
            }
        }
        
	}

    public void CameraShake()
    {
        Debug.Log("CameraShake!");
        originalPosition = Camera.main.transform.position;

        shakeSpeedCounter = shakeSpeed;

        Time.timeScale = shakeTimeScale;
        shake = true;
    }
}
