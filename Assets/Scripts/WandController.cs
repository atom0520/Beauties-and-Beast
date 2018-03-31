using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WandController : MonoBehaviour {

    [SerializeField]
    Transform magicBallCastPoint;

    GameObject currMagicBall;

    [SerializeField]
    GameObject magicBallPrefab;

    [SerializeField]
    Transform target;
    [SerializeField]
    float magicBallSpeed = 10.0f;

    [SerializeField]
    AudioSource castBulletAudioSrc;

    public OVRInput.Button castMaigcBallButton = OVRInput.Button.SecondaryIndexTrigger;

    [SerializeField]
    float magicBallCT = 1f;
    float magicBallCTCounter = 0;

    [SerializeField]
    int baseAtk = 2;
    [SerializeField]
    int maxAtk = 20;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if(currMagicBall == null)
        {
            if (magicBallCTCounter <= 0)
            {
                if (Input.GetKey(KeyCode.Space) || OVRInput.Get(castMaigcBallButton))
                //if (OVRInput.Get(castMaigcBallButton))
                {
                    currMagicBall = GameObject.Instantiate(magicBallPrefab, magicBallCastPoint.position, magicBallCastPoint.rotation);
                    currMagicBall.GetComponent<MagicBallController>().Init(
                        BulletController.InventorType.Party, transform.root.gameObject, baseAtk, maxAtk);
                    currMagicBall.transform.parent = magicBallCastPoint;
                }
            }else
            {
                magicBallCTCounter -= Time.deltaTime;
            }
        }
        else
        {
            currMagicBall.transform.position = magicBallCastPoint.transform.position;
            if (!Input.GetKey(KeyCode.Space) && !OVRInput.Get(castMaigcBallButton))
            //if(!OVRInput.Get(castMaigcBallButton))
            {
                CastMagicBall();
            }
        }

        
	}

    void CastMagicBall()
    {
        currMagicBall.GetComponent<MagicBallController>().Cast();
        currMagicBall.transform.LookAt(target.transform);
        currMagicBall.GetComponent<Rigidbody>().velocity = currMagicBall.transform.forward * magicBallSpeed;
        castBulletAudioSrc.Play();
        currMagicBall = null;

        magicBallCTCounter = magicBallCT;
    }
}
