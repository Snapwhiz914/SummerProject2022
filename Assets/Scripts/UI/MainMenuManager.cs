using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{

    public Button resumeOrStartButton;
    public Animator mainMenuAnimator;
    public Animator startMenuAnimator;
    public Canvas startMenuCanvas;
    public Button openOptionsButton;
    public Button exitButton;
    public Button backToMainButton;
    public OptionsMenu optionsMenu;
    
    void Start()
    {
        int iEnced = 4 ^ 6;
        Debug.Log(iEnced);
        int iDeenced = iEnced ^ 6;
        Debug.Log(iDeenced);
        mainMenuAnimator.SetBool("Changing", false);
        startMenuAnimator.SetBool("Changing", true);
        resumeOrStartButton.onClick.AddListener(onStart);
        backToMainButton.onClick.AddListener(onBackFromStart);
        openOptionsButton.onClick.AddListener(onClickOptions);
        exitButton.onClick.AddListener(onExitButton);
        optionsMenu.gameObject.GetComponent<Canvas>().enabled = false;
        startMenuCanvas.enabled = false;
    }

    void onStart()
    {
        Debug.Log("Clicked os");
        startMenuCanvas.enabled = true;
        mainMenuAnimator.SetBool("Changing", true);
        startMenuAnimator.SetBool("Changing", false);
    }

    void onBackFromStart()
    {
        mainMenuAnimator.SetBool("Changing", false);
        startMenuAnimator.SetBool("Changing", true);
    }

    void onClickOptions()
    {
        Debug.Log("Clicked oco");
        optionsMenu.gameObject.GetComponent<Canvas>().enabled = true;
        mainMenuAnimator.SetBool("Changing", true);
        optionsMenu.openMenu(optionsMenuCloseCallback);
    }

    void optionsMenuCloseCallback()
    {
        mainMenuAnimator.SetBool("Changing", false);
        //optionsMenu.gameObject.GetComponent<Canvas>().enabled = false;
    }

    void onExitButton()
    {
        Application.Quit(0);
    }
}
