using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    public string currentLevel;

    private void Awake()
    {
        //SceneManager.SetActiveScene(SceneManager.GetSceneByName(currentLevel));
    }
}
