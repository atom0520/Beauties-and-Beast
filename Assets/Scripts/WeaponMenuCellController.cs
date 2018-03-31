using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponMenuCellController : MenuCellController
{
    [SerializeField]
    GameObject gunMenuObj;
    [SerializeField]
    GameObject bowMenuObj;
    [SerializeField]
    GameObject wandMenuObj;

    override public void OpenSubmenu()
    {
        // Debug.Log("WeaponMenuCellController.OpenSubmenu!");
        if (submenu != null)
        {
            gunMenuObj.SetActive(true);
            gunMenuObj.transform.GetChild(0).GetComponent<MenuCellController>().ShowArrowCell(false);
            bowMenuObj.SetActive(true);
            bowMenuObj.transform.GetChild(0).GetComponent<MenuCellController>().ShowArrowCell(false);
            wandMenuObj.SetActive(true);
            wandMenuObj.transform.GetChild(0).GetComponent<MenuCellController>().ShowArrowCell(false);

            switch (GameManager.instance.currWeaponType)
            {
                case GameManager.WeaponType.Hands:
                    bowMenuObj.transform.GetChild(0).GetComponent<MenuCellController>().ShowArrowCell(true);
                    break;
                case GameManager.WeaponType.Gun:
                    gunMenuObj.SetActive(false);
                    submenu.transform.localPosition = new Vector3(submenu.transform.localPosition.x, 0, submenu.transform.localPosition.z);
                    bowMenuObj.transform.GetChild(0).GetComponent<MenuCellController>().ShowArrowCell(true);
                    break;
                case GameManager.WeaponType.Bow:
                    bowMenuObj.SetActive(false);
                    submenu.transform.localPosition = new Vector3(submenu.transform.localPosition.x, 0, submenu.transform.localPosition.z);
                    gunMenuObj.transform.GetChild(0).GetComponent<MenuCellController>().ShowArrowCell(true);
                    break;
                case GameManager.WeaponType.Wand:
                    wandMenuObj.SetActive(false);
                    submenu.transform.localPosition = new Vector3(submenu.transform.localPosition.x, 0, submenu.transform.localPosition.z);
                    gunMenuObj.transform.GetChild(0).GetComponent<MenuCellController>().ShowArrowCell(true);
                    break;
            }

            submenu.SetActive(true);
        }
    }
}
