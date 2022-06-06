using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class OptionsMenu : MonoBehaviour
{
    [SerializeField]
    private TMP_Dropdown resolutionOptions;
    [SerializeField]
    private TMP_Dropdown graphicsOptions;
    [SerializeField]
    private Toggle fullscreen;
    [SerializeField]
    private Button closeButton;

    private Animator animator;
    private UnityEngine.Events.UnityAction closeAction;

    Resolution[] resolutions;
    
    void Start()
    {
        resolutions = Screen.resolutions;

        List<string> resInStrings = new List<string>();

        int i = 0;
        foreach (Resolution r in resolutions) {
            resInStrings.Add(r.ToString());
            if (Screen.currentResolution.width == r.width && Screen.currentResolution.height == r.height)
            {
                resolutionOptions.value = i;
            }
            i++;
        }

        resolutionOptions.ClearOptions();
        resolutionOptions.AddOptions(resInStrings);

        graphicsOptions.value = QualitySettings.GetQualityLevel();
        fullscreen.isOn = Screen.fullScreen;
        closeButton.onClick.AddListener(onClose);
        animator = GetComponent<Animator>();
        animator.SetBool("Changing", true);
    }

    public void onChangeGraphics(int newIndex)
    {
        QualitySettings.SetQualityLevel(newIndex, true);
    }

    public void onChangeFullscreen(bool change)
    {
        Screen.fullScreen = change;
    }

    public void onChangeResolution(int i)
    {
        Screen.SetResolution(resolutions[i].width, resolutions[i].height, Screen.fullScreen, resolutions[i].refreshRate);
    }

    public void openMenu(UnityEngine.Events.UnityAction onClose)
    {
        closeAction = onClose;
        animator.SetBool("Changing", false);
    }

    public void onClose()
    {
        animator.SetBool("Changing", true);
        closeAction.Invoke();
    }
}
