using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartSceneFromButton : MonoBehaviour
{

    public string sceneToMoveToo;
    public Image fadePanel;

    private bool isFading;
    private Button thisButton;

    void Start()
    {
        thisButton = GetComponent<Button>();
        thisButton.onClick.AddListener(onClick);
        fadePanel.gameObject.SetActive(false);
    }

    void onClick()
    {
        StartCoroutine(fadeToScene(sceneToMoveToo)); 
    }

    private IEnumerator fadeToScene(string scene)
    {
        fadePanel.gameObject.SetActive(true);
        isFading = true;
        for (int i = 0; i < 100; i++)
        {
            fadePanel.color = new Color(0, 0, 0, i * .01f);
            yield return new WaitForSeconds(0.01f);
        }
        isFading = false;
        SceneManager.LoadScene(scene, LoadSceneMode.Single);
    }
}
