using UnityEngine;
using System;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class LevelLoaading : MonoBehaviour
{
    [Header("stattic references")]
    public static LevelLoaading levelLoaading;
    [Header("game object")]
    public GameObject loadingPanel;
    [Header("int")]
    public int targetScene;
    [Header("animations")]
    //public Animator mushroom;
    public Animator Loading;
    [Header("floats")]
    public float MinLoadTime;
    public float fadeTime;
    [Header("image")]
    public Image fade_Image;
    [Header("bool")]
    private bool isLoading;

    private void Awake()
    {
        // sets objects
        if (loadingPanel != null)
        {
            loadingPanel.SetActive(false);
        }
        if (fade_Image != null)
        {
            fade_Image.gameObject.SetActive(false);
        }
        Time.timeScale = 1;
    }

    /// <summary>
    /// load chosen scene
    /// </summary>
    /// <param name="index"></param>
    public void LoadScene(int index)
    {

        targetScene = index;
        StartCoroutine(LoadSceneRoutine());
        
 
    }
    
    /// <summary>
    /// run method to play animation and include smooth transition between scenes
    /// </summary>
    /// <returns></returns>
    private IEnumerator LoadSceneRoutine()
    {

        isLoading = true;

        if (fade_Image != null)
        {
            fade_Image.gameObject.SetActive(true);

            fade_Image.canvasRenderer.SetAlpha(0);
        }

        while (!Fade(1))
            yield return null;

        if (loadingPanel != null)
        {
            loadingPanel.SetActive(true);
        }

        //mushroom.Play("Mushroom");
        if (Loading != null)
        {
            Loading.Play("Loadings");
        }

        while (!Fade(0))
            yield return null;


       AsyncOperation operation = SceneManager.LoadSceneAsync(targetScene);
        float elapsedLoadTime = 0f;
      

        while (!operation.isDone)
        {
            elapsedLoadTime += Time.deltaTime;
            yield return null;
        }

        while(elapsedLoadTime < MinLoadTime)
        {
            elapsedLoadTime += Time.deltaTime;
            yield return null;
        }

        while (!Fade(1))
            yield return null;

        if (loadingPanel != null)
        {
            loadingPanel.SetActive(false);
        }

        while (!Fade(0))
            yield return null;

        isLoading = false;
    }

    /// <summary>
    /// function to complete fade transtion
    /// </summary>
    /// <param name="target"></param>
    /// <returns></returns>
    private bool Fade(float target)
    {

        if (fade_Image == null) return true;

        fade_Image.CrossFadeAlpha(target, fadeTime, true);

        if(Mathf.Abs(fade_Image.canvasRenderer.GetAlpha() - target) <= 0.05f)
        {
            fade_Image.canvasRenderer.SetAlpha(target);
            return true;
        }
        return false;
    }
}
