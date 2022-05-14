using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LogoUI : MonoBehaviour
{

    bool sceneReady = false;
    bool checkScene = false;
    AsyncOperation operation;

    public void LoadAsync(string scene) {
        operation = SceneManager.LoadSceneAsync(scene);
        checkScene = true;
    }

    private void Update()
    {

    }

    public void LoadScene(string scene) {
        SceneManager.LoadScene(scene);
    }
}
