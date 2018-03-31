using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrosshairManager : MonoBehaviour {

    static public CrosshairManager instance;

    [SerializeField]
    GameObject cameraAnchor;

    [SerializeField]
    GameObject crosshairCanvas;

    [SerializeField]
    float defaultCrosshairDepth;
    float crosshairDepth;

    [SerializeField]
    float unscaledCrosshairDiameter;

    [SerializeField]
    float crosshairTargetAngularSize;

    //[SerializeField]
    public GameObject weapon;

    void Awake()
    {
        if (instance == null)
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

        crosshairCanvas.SetActive(weapon != null);

    }
	
	// Update is called once per frame
	void Update () {

        RaycastHit hit;
        //Debug.Log("weapon:" + weapon);
        if (weapon!=null && Physics.Raycast(weapon.transform.position, weapon.transform.forward, out hit, Mathf.Infinity, LayerMask.GetMask("DefaultCrosshairDepth")))
        {
            
            crosshairCanvas.transform.position = hit.point;
            //Debug.Log("hit.point:" + hit.point);

            crosshairCanvas.transform.LookAt(cameraAnchor.transform.position);
            crosshairCanvas.transform.Rotate(new Vector3(0, 180, 0));
        
            crosshairDepth = Vector3.Project(crosshairCanvas.transform.position - cameraAnchor.transform.position, cameraAnchor.transform.forward).magnitude;

        }
       
        
        //crosshairDepth = newCrosshairDepth;
        //crosshairCanvas.transform.localPosition = new Vector3(0.0f, 0.0f, crosshairDepth);

        float desiredSize = Mathf.Tan(crosshairTargetAngularSize * Mathf.Deg2Rad * 0.5f) * 2 * crosshairDepth;
        float requiredScale = desiredSize / unscaledCrosshairDiameter;
        crosshairCanvas.transform.localScale = new Vector3(requiredScale, requiredScale, requiredScale);
    }

    public void ShowCrosshair()
    {
        crosshairCanvas.SetActive(true);
    }

    public void HideCrosshair()
    {
        crosshairCanvas.SetActive(false);
    }

    public void SetWeapon(GameObject weapon)
    {
        //Debug.Log("SetWeapon!");
        this.weapon = weapon;
        crosshairCanvas.SetActive(weapon != null);
    }
}
