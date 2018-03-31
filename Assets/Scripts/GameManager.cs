using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public enum GameState
    {
        BeforeDragonCome,
        AfterDragonCome,
        FightDragon,
        AfterDragonDie,
        AfterPlayerDie,
        AfterExitGame
    }

    public enum WeaponType
    {
        Hands,
        Gun,
        Bow,
        Wand
    }

    [HideInInspector]
    public GameState gameState;

    [SerializeField]
    GameObject inGameMenu;

    public WeaponType currWeaponType;

    [SerializeField]
    GameObject weaponGun;
    [SerializeField]
    GameObject weaponBow;
    [SerializeField]
    GameObject weaponWand;

    public Sprite selArrowMenuCellSprite;
    public Sprite unselArrowMenuCellSprite;
    public Sprite selMenuCellSprite;
    public Sprite unselMenuCellSprite;

    public OVRInput.Button openMenuBtn = OVRInput.Button.Two;
    public KeyCode openMenuKey = KeyCode.M;

    public OVRGazePointer ovrGazePointer;

    public OVRInputModule overInputModule;

    public GameObject rightHandAnchor;

    [SerializeField]
    GameObject gazePointerRingImage;

    [SerializeField]
    GameObject defeatCanvas;
    [SerializeField]
    GameObject victoryCanvas;

    [SerializeField]
    GameObject readyGoCanvas;

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

    // Use this for initialization
    void Start()
    {
        gameState = GameState.BeforeDragonCome;
        weaponGun.SetActive(false);
        weaponBow.SetActive(false);
        weaponWand.SetActive(false);

        GameObject pcTestWand = GameObject.FindGameObjectWithTag("PCTestWand");
        if (pcTestWand != null)
        {
            weaponWand = pcTestWand;
            ChangeWeapon(3);
        }
        else
        {
            ChangeWeapon(0);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (gameState == GameState.AfterPlayerDie)
            return;

        //if (Input.GetKeyDown(KeyCode.Escape))
        if (OVRInput.GetDown(openMenuBtn) || Input.GetKeyDown(openMenuKey))
        {
            if (inGameMenu.activeSelf == false)
            {
                OpenInGameMenu();
                AudioManager.instance.callInGameMenuAudioSrc.Play();
            }
            else
            {
                CloseInGameMenu();
                AudioManager.instance.callInGameMenuAudioSrc.Play();
            }

        }
    }

    public IEnumerator StartFightingDragon()
    {
        readyGoCanvas.SetActive(true);
        yield return new WaitForSeconds(1.0f);
        AudioManager.instance.startFightingAudioSrc.Play();
        gameState = GameState.FightDragon;
        AudioManager.instance.bgmAudioSrc.Play();
        yield return new WaitForSeconds(1.0f);
        readyGoCanvas.SetActive(false);
    }

    void OpenInGameMenu()
    {
        CrosshairManager.instance.HideCrosshair();
        gazePointerRingImage.SetActive(true);
        inGameMenu.SetActive(true);
    }

    void CloseInGameMenu()
    {
        MenuCellController[] cellControllers = inGameMenu.GetComponentsInChildren<MenuCellController>();
        foreach (MenuCellController cellController in cellControllers)
        {
            cellController.CloseSubmenu();
            cellController.GetUnselected();
        }

        gazePointerRingImage.SetActive(false);
        if (CrosshairManager.instance.weapon != null)
            CrosshairManager.instance.ShowCrosshair();

        inGameMenu.SetActive(false);
    }

    public void ChangeWeapon(int weaponIndex)
    {
        WeaponType weaponType = (WeaponType)weaponIndex;

        switch (currWeaponType)
        {
            case WeaponType.Gun:
                weaponGun.SetActive(false);
                break;
            case WeaponType.Bow:
                weaponBow.SetActive(false);
                Destroy(weaponBow.GetComponent<ArrowManager>().currentArrow);
                break;
            case WeaponType.Wand:
                weaponWand.SetActive(false);
                break;
        }

        switch (weaponType)
        {
            case WeaponType.Hands:
                CrosshairManager.instance.SetWeapon(null);
                overInputModule.rayTransform = rightHandAnchor.transform;
                ovrGazePointer.rayTransform = rightHandAnchor.transform;
                break;

            case WeaponType.Gun:
                weaponGun.SetActive(true);
                CrosshairManager.instance.SetWeapon(weaponGun);

                overInputModule.rayTransform = weaponGun.transform;
                ovrGazePointer.rayTransform = weaponGun.transform;

                break;
            case WeaponType.Bow:
                CrosshairManager.instance.SetWeapon(null);
                weaponBow.SetActive(true);

                //overInputModule.rayTransform = weaponBow.transform;
                //ovrGazePointer.rayTransform = weaponBow.transform;
                break;
            case WeaponType.Wand:
                weaponWand.SetActive(true);
                CrosshairManager.instance.SetWeapon(weaponWand);

                overInputModule.rayTransform = weaponWand.transform;
                ovrGazePointer.rayTransform = weaponWand.transform;
                break;
        }

        currWeaponType = weaponType;

        CloseInGameMenu();
    }

    public IEnumerator LoseGame()
    {
        gameState = GameState.AfterPlayerDie;
        yield return new WaitForSeconds(4.0f);
        while (AudioManager.instance.bgmAudioSrc.volume > 0)
        {
            AudioManager.instance.bgmAudioSrc.volume -= Time.deltaTime * 2f;
        }
        AudioManager.instance.bgmAudioSrc.Stop();
        AudioManager.instance.defeatAudioSrc.Play();
        defeatCanvas.SetActive(true);
        yield return new WaitForSeconds(10.0f);
        Exit();
    }

    public IEnumerator WinGame()
    {
        gameState = GameState.AfterDragonDie;
        while (AudioManager.instance.bgmAudioSrc.volume > 0)
        {
            AudioManager.instance.bgmAudioSrc.volume -= Time.deltaTime * 2f;
        }
        AudioManager.instance.bgmAudioSrc.Stop();
        AudioManager.instance.victoryAudioSrc.Play();
        victoryCanvas.SetActive(true);
        yield return new WaitForSeconds(3.0f);
        //victoryCanvas.SetActive(false);
    }

    public void OnClickInGameMenuExitBtn()
    {
        gameState = GameState.AfterExitGame;
        CloseInGameMenu();
        FadingView.instance.FadeOutView();
        Invoke("Exit", 4.0f);
    }

    public void Exit()
    {
#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#else
		Application.Quit();
#endif
    }
}
