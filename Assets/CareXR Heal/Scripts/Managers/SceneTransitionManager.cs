using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransitionManager : MonoBehaviour {
    public FadeScreen fadeScreen;

    private static SceneTransitionManager _instance = null;
    public static SceneTransitionManager Instance {
        get { return _instance; }
        set {
            if (_instance == null) {
                _instance = value;
            } else {
                Destroy(value);
            }
        }
    }

    public static Dictionary<string, int> Scenes = new Dictionary<string, int>()
        {
            { "Start", 0 },
            { "Lobby", 1 },
            { "Panoramic Session", 2 },

        };


    void Awake() {
        if (_instance == null)
            _instance = this;
        else {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);

    }

    private void Start() {
        DontDestroyOnLoad(gameObject);
    }

    public void GoToScene(int sceneIndex, Action onLoadComplete = null)
    {
        StartCoroutine(GoToSceneRoutine(sceneIndex, onLoadComplete));

    }

    IEnumerator GoToSceneRoutine(int sceneIndex, Action onLoadComplete = null)
    {
        fadeScreen.FadeOut();
        yield return new WaitForSeconds(fadeScreen.fadeDuration);

        //Launch the new scene
        SceneManager.LoadScene(sceneIndex);
        onLoadComplete?.Invoke();

    }

    public void GoToSceneAsync(int sceneIndex, Action onLoadComplete = null)
    {
        StartCoroutine(GoToSceneAsyncRoutine(sceneIndex, onLoadComplete));
    }

    IEnumerator GoToSceneAsyncRoutine(int sceneIndex, Action onLoadComplete = null)
    {
        fadeScreen.FadeOut();
        //Launch the new scene
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneIndex);

        operation.completed += (AsyncOperation ap) => {
            onLoadComplete?.Invoke();
            fadeScreen = FindAnyObjectByType<FadeScreen>();

        };

        SessionManager.InStartScene = sceneIndex == 0;
        Debug.Log("Is in Start Scene? " + SessionManager.InStartScene);

        operation.allowSceneActivation = false;

        float timer = 0;
        while(timer <= fadeScreen.fadeDuration && !operation.isDone)
        {
            timer += Time.deltaTime;
            yield return null;
        }

        operation.allowSceneActivation = true;

        
    }
}
