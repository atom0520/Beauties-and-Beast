using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuCellController : MonoBehaviour
{

    [SerializeField]
    GameObject bgSelected;
    [SerializeField]
    GameObject iconSelected;
    [SerializeField]
    GameObject textSelected;
    [SerializeField]
    protected GameObject submenu;

    Toggle toggle;

    void Awake() {
        toggle = gameObject.GetComponent<Toggle>();
    }

    //void AutoAssignAttrs()
    //{
    //    if(transform.childCount >= 5)
    //    {
    //        if (bgSelected == null)
    //        {
    //            bgSelected = transform.GetChild(2).gameObject;
    //        }
    //        if (iconSelected == null)
    //        {
    //            iconSelected = transform.GetChild(3).gameObject;
    //        }
    //        if (textSelected == null)
    //        {
    //            textSelected = transform.GetChild(4).gameObject;
    //        }

    //        if (transform.childCount >= 6)
    //        {
    //            if (submenu == null)
    //            {
    //                submenu = transform.GetChild(5).gameObject;
    //            }                
    //        }
    //    }
    //    else
    //    {
    //        if (bgSelected == null)
    //        {
    //            bgSelected = transform.GetChild(0).gameObject;
    //        }

    //        if (transform.childCount >= 2)
    //        {
    //            if (submenu == null)
    //            {
    //                submenu = transform.GetChild(1).gameObject;
    //            }
    //        }
    //    }

    //}

    // Update is called once per frame
    void Update() {

    }

    public void GetSelected()
    {
        //AutoAssignAttrs();
        if (iconSelected != null)
            iconSelected.SetActive(true);
        if (bgSelected != null)
        {
            //Debug.Log("bgSelected.SetActive(true)");
            bgSelected.SetActive(true);
        }

        if (textSelected != null)
            textSelected.SetActive(true);
    }

    public void GetUnselected()
    {
        if (iconSelected != null)
            iconSelected.SetActive(false);
        if (bgSelected != null)
            bgSelected.SetActive(false);
        if (textSelected != null)
            textSelected.SetActive(false);

    }

    virtual public void OpenSubmenu()
    {
        // Debug.Log("OpenSubmenu!");
        if (submenu != null)
        {
            submenu.SetActive(true);
        }
    }

    public void CloseSubmenu()
    {
        if (submenu != null)
        {
            submenu.SetActive(false);
        }
    }

    public void OnToggleChangeValue()
    {        
        if (toggle.isOn)
        {

            int siblingCount = transform.parent.parent.childCount;
            for (int i = 0; i < siblingCount; i += 1)
            {
                transform.parent.parent.GetChild(i).GetChild(0).GetComponent<MenuCellController>().CloseSubmenu();
                transform.parent.parent.GetChild(i).GetChild(0).GetComponent<MenuCellController>().GetUnselected();
            }


            GetSelected();
            OpenSubmenu();
        }
        else
        {
            GetUnselected();
            CloseSubmenu();
        }

        AudioManager.instance.inGameMenuToggleAudioSrc.Play();
    }

    public void ShowArrowCell(bool value)
    {
        if (value)
        {
            GetComponent<Image>().sprite = GameManager.instance.unselArrowMenuCellSprite;            
            bgSelected.GetComponent<Image>().sprite = GameManager.instance.selArrowMenuCellSprite;
        }
        else
        {
            GetComponent<Image>().sprite = GameManager.instance.unselMenuCellSprite;
            bgSelected.GetComponent<Image>().sprite = GameManager.instance.selMenuCellSprite;
        }
        GetComponent<Image>().SetNativeSize();
        bgSelected.GetComponent<Image>().SetNativeSize();
    }
}
