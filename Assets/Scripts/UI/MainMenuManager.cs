using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{

    public Button resumeOrStartButton;
    public Animator mainMenuAnimator;
    public Button openOptionsButton;
    public Button exitButton;
    public OptionsMenu optionsMenu;
    
    void Start()
    {
        int iEnced = 4 ^ 6;
        Debug.Log(iEnced);
        int iDeenced = iEnced ^ 6;
        Debug.Log(iDeenced);
        mainMenuAnimator.SetBool("Changing", false);
        resumeOrStartButton.onClick.AddListener(onResumeOrStart);
        openOptionsButton.onClick.AddListener(onClickOptions);
        exitButton.onClick.AddListener(onExitButton);
        optionsMenu.gameObject.GetComponent<Canvas>().enabled = false;
    }

    void onResumeOrStart()
    {
        Debug.Log("Clicked oros");
        mainMenuAnimator.SetBool("Changing", true);
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
