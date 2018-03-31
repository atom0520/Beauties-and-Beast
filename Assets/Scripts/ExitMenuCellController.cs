using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitMenuCellController : MenuCellController {

    private void OnMouseEnter()
    {
        Debug.Log("OnMouseEnter!");
    }

    private void OnMouseExit()
    {
        Debug.Log("OnMouseExit!");
    }
}
