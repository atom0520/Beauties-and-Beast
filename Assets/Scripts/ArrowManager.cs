using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowManager : MonoBehaviour {

    public static ArrowManager instance;

    [SerializeField]
    GameObject trackedObj;
    [HideInInspector]
    public GameObject currentArrow { private set; get; }
    [SerializeField]
    GameObject arrowPrefab;

    [SerializeField]
    GameObject stringAttachPoint;
    [SerializeField]
    GameObject arrowStartPoint;
    [SerializeField]
    GameObject stringStartPoint;


    bool isArrowAttached;

    float pullStringDist;

    [SerializeField]
    KeyCode shootArrowKey = KeyCode.Z;
    //[SerializeField]
    //OVRInput.Button shootArrowButton = OVRInput.Button.SecondaryIndexTrigger;

    public OVRInput.Button attachArrowButton = OVRInput.Button.SecondaryIndexTrigger;

    [SerializeField]
    Transform arrowSpawnPoint;

    [SerializeField]
    float arrowSpeedDegree = 50f;

    [SerializeField]
    float pullStringSpeed = 10;

    [SerializeField]
    AudioSource castBulletAudioSrc;

    [SerializeField]
    float shootArrowCT = 1f;
    float shootArrowCTCounter = 0;

    [SerializeField]
    int atk = 5;

    void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        } 
    }

    void OnDestroy()
    {
        if (instance == this)
        {
            instance = null;
        }
    }

    // Use this for initialization
    void Start () {
        isArrowAttached = false;
    }

    // Update is called once per frame
    void Update() {
        if (shootArrowCTCounter <= 0)
        {
            TrySpawnArrow();
        }
        else
        {
            shootArrowCTCounter -= Time.deltaTime;
        }
    
        TryPullString();

      
    }

    void TryPullString()
    {
        if (isArrowAttached)
        {
            Vector3 diff = trackedObj.transform.position - stringStartPoint.transform.position;
            Vector3 projDiff = Vector3.Project(diff, arrowStartPoint.transform.forward);

            //Debug.Log("Vector3.Dot(arrowStartPoint.transform.forward, projDiff):"+Vector3.Dot(arrowStartPoint.transform.forward, projDiff));

            
            if (Vector3.Dot(arrowStartPoint.transform.forward, projDiff) < 0)
            {
                pullStringDist = projDiff.magnitude;
            }
            else
            {
                pullStringDist = 0; 
            }


            //if(Vector3.Dot(arrowStartPoint.transform.forward, projDiff) < 0)
            //{
                stringAttachPoint.transform.localPosition = stringStartPoint.transform.localPosition + new Vector3(pullStringSpeed * pullStringDist, 0, 0);
            //}

            if (!OVRInput.Get(attachArrowButton))
            {
                ShootArrow();
            }
        }
    }

    void ShootArrow()
    {
        currentArrow.transform.parent = null;

        //currentArrow.GetComponent<ArrowController>().isShooted = true;
        //if (currentArrow.GetComponent<ArrowController>().lifeTime > 0)
        //    Destroy(currentArrow, currentArrow.GetComponent<ArrowController>().lifeTime);
        //Rigidbody rb = currentArrow.GetComponent<Rigidbody>();
        //rb.isKinematic = false;
        //rb.velocity = currentArrow.transform.forward * arrowSpeedDegree * pullStringDist;
        //rb.useGravity = true;
        //currentArrow.GetComponent<TrailRenderer>().Clear();
        //currentArrow.GetComponent<TrailRenderer>().enabled = true;
        currentArrow.GetComponent<ArrowController>().Cast(arrowSpeedDegree * pullStringDist);

        stringAttachPoint.transform.localPosition = stringStartPoint.transform.localPosition;

        castBulletAudioSrc.Play();
              
        currentArrow = null;
        isArrowAttached = false;
        CrosshairManager.instance.HideCrosshair();
        CrosshairManager.instance.SetWeapon(null);

        GameManager.instance.overInputModule.rayTransform = GameManager.instance.rightHandAnchor.transform;
        GameManager.instance.ovrGazePointer.rayTransform = GameManager.instance.rightHandAnchor.transform;

        shootArrowCTCounter = shootArrowCT;
    }

    private void TrySpawnArrow()
    {
        if(currentArrow == null)
        {
            currentArrow = Instantiate(arrowPrefab, arrowSpawnPoint.position, arrowSpawnPoint.rotation);
           
            currentArrow.transform.parent = trackedObj.transform;
            //currentArrow.transform.localPosition = new Vector3(0.02f, -0.022f, 0.162f);

            //currentArrow.transform.localPosition = arrowSpawnPoint.localPosition;
            //Debug.Log("arrowSpawnPoint.transform.localPosition:" + arrowSpawnPoint.localPosition);
            //Debug.Log("currentArrow.transform.localPosition:" + currentArrow.transform.localPosition);
            // currentArrow.transform.localRotation = Quaternion.identity;
            //currentArrow.transform.localRotation = arrowSpawnPoint.localRotation;

            currentArrow.GetComponent<ArrowController>().atk = this.atk;

            GameManager.instance.overInputModule.rayTransform = currentArrow.transform;
            GameManager.instance.ovrGazePointer.rayTransform = currentArrow.transform;
        }
    }

    public void AttachArrowToBow()
    {     
        Debug.Log("AttachArrowToBow!");
        currentArrow.transform.parent = stringAttachPoint.transform;
        currentArrow.transform.position = arrowStartPoint.transform.position;
        currentArrow.transform.rotation = arrowStartPoint.transform.rotation;

        CrosshairManager.instance.SetWeapon(currentArrow);
        CrosshairManager.instance.ShowCrosshair();


        isArrowAttached = true;
       
    }
}
