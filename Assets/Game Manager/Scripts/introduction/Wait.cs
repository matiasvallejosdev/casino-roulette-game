using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Wait : MonoBehaviour
{
    [SerializeField] private float waitTime = 0;

    private void Start()
    {
        StartCoroutine(waitSeconds());
    }

    IEnumerator waitSeconds()
    {
        yield return new WaitForSeconds(waitTime);
        SceneManager.LoadScene("Boot");
    }
}
