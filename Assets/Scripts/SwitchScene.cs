using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SwitchScene : MonoBehaviour
{
    public void ChangeScene(int sceneID)
    {
        SceneManager.LoadScene(sceneID);
    }
    public void QuitGame()
    {
        Application.Quit();
    }
}
