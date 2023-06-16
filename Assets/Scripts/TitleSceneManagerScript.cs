using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleSceneManagerScript : MonoBehaviour
{
    public string nextScene = "Map00 - Pond";
    public GameObject oldBackground;
    public GameObject newBackground;
    public GameObject fadeScreen;

    public AudioSource sfxPlayer;
    public AudioSource bgmPlayer;

    public AudioClip startSound;
    public AudioClip titleTheme;

    public void Start()
    {
        bgmPlayer.clip = titleTheme;
        bgmPlayer.Play();
    }

    public void StartGame()
    {
        ChangeBackground();
        sfxPlayer.PlayOneShot(startSound);
        Invoke(nameof(GoToNextScene), 2.0f);
    }
    private void ChangeBackground()
    {
        oldBackground.SetActive(false);
        newBackground.SetActive(true);
        fadeScreen.SetActive(true);
    }
    private void GoToNextScene()
    {
       ChangeScene(nextScene);
    }

    public void ChangeScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void ExitGame()
    {
        Application.Quit();
    }

}
