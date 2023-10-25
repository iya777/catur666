using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class DBGameManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;
    private void Update()
    {
        if (!DBManager.loggedIn)
        {
            SceneManager.LoadScene(0);
            Debug.LogWarning("User is logged out");
        }
        else
        {
            text.SetText("Logged in as: \n" + DBManager.username);
        }
    }
}
