using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.Networking;
using UnityEngine.UI;

public class DBGameManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;
    //[SerializeField] private UIManager uiManager;
    private void Start()
    {
        savedGamesButtons = new List<GameObject>();
    }
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
        Update_SavedGamesLists();
    }
    public void LogOut()
    {
        DBManager.username = null;
        DBManager.score = 0;
    }

    /// <summary>
    /// Get all the saved games hehehehhehe
    /// </summary>
    private int current_page = 0;
    private int current_numRows = 0;
    private List<GameObject> savedGamesButtons;
    [SerializeField] private GameObject savedGamesButtonPrefab;
    [SerializeField] private Transform parentSavedGamesButton;
    [SerializeField] private Button nextButton;
    [SerializeField] private Button prevButton;
    public void NextPage()
    {
        current_page++;
        DestroyListOfSavedGames();
        StartCoroutine(GetAllSavedGames());
    }
    public void PreviousPage()
    {
        current_page--;
        current_page = Mathf.Max(current_page, 0);
        DestroyListOfSavedGames();
        StartCoroutine(GetAllSavedGames());
    }
    public void GetSavedGames()
    {
        StartCoroutine(GetAllSavedGames());
    }
    public void CallCloseSavedGames()
    {
        current_page = 0;
        if (savedGamesButtons.Count > 0)
        {
            DestroyListOfSavedGames();
        }
        current_numRows = 0;
    }
    public void Update_SavedGamesLists()
    {
        if (current_numRows > 15)
        {
            nextButton.gameObject.SetActive(true);
            prevButton.gameObject.SetActive(true);

            if (current_page <= 0)
            {
                prevButton.interactable = false;
            }
            else
            {
                prevButton.interactable = true;
            }

            if ((current_page + 1) * 15 < current_numRows)
            {
                nextButton.interactable = true;
            }
            else
            {
                nextButton.interactable = false;
            }
        }
        else
        {
            if (current_page <= 0)
            {
                prevButton.interactable = false;
                nextButton.gameObject.SetActive(false);
                prevButton.gameObject.SetActive(false);
            }
            else
            {
                nextButton.gameObject.SetActive(true);
                prevButton.gameObject.SetActive(true);
                prevButton.interactable = true;
                nextButton.interactable = false;
            }
        }


    }
    private IEnumerator GetAllSavedGames()
    {
        WWWForm form = new WWWForm();
        form.AddField("username", DBManager.username);
        form.AddField("current_index", current_page * 15);
        WWW www = new WWW("http://localhost/sqlconnect/GameList.php", form);
        yield return www;
        string[] savedGameDataStrings = www.text.Split('\n');
        current_numRows = savedGameDataStrings.Length - 1;//int.Parse(savedGameDataStrings[0].Split('\t')[8]);
        for (int i = 0; i < savedGameDataStrings.Length - 1; i++) // - 1 karena di akhir ada \n doang
        {
            if (i >= 15)
                break;
            string[] content = savedGameDataStrings[i].Split('\t');
            GameObject savedGameButton = Instantiate(savedGamesButtonPrefab, parentSavedGamesButton);
            SavedGamesButtonVariables buttonVars;
            savedGameButton.TryGetComponent(out buttonVars);
            buttonVars.datetime.SetText(content[7]);
            buttonVars.gamemode.SetText(content[3]);
            int isAgainstAI = int.Parse(content[4]);
            if (isAgainstAI == 1)
            {
                buttonVars.vsai_level.SetText("AI (" + content[5] + ") " + "White: " + content[6]);
            }
            else
            {
                buttonVars.vsai_level.SetText("Player");
            }
            buttonVars.FEN = content[2];
            if (content[6] == "1")
            {
                buttonVars.isWhite = true;
            }
            else
            {
                buttonVars.isWhite = false;
            }
            buttonVars.savedGameID = int.Parse(content[1]);
            savedGamesButtons.Add(savedGameButton);
        }
    }
    private void DestroyListOfSavedGames()
    {
        if (savedGamesButtons.Count <= 0)
            return;
        for (int i = 0; i < savedGamesButtons.Count; i++)
        {
            Destroy(savedGamesButtons[i]);
        }
        savedGamesButtons.Clear();
    }

    /// <summary>
    /// UI SETTINGS
    /// </summary>
    public void UPDATE_Preferences()
    {
        IENUM_UpdatePreference();
    }
    public void GET_Preferences()
    {
        IENUM_GetPreference();
    }
    private IEnumerator IENUM_UpdatePreference()
    {
        WWWForm form = new WWWForm();
        form.AddField("username", DBManager.username);
        form.AddField("fullscreen", DBManager.fullscreen);
        form.AddField("mastervolume", DBManager.masterVol.ToString());
        form.AddField("musicvolume", DBManager.masterVol.ToString());
        form.AddField("sfxvolume", DBManager.masterVol.ToString());
        WWW www = new WWW("http://localhost/sqlconnect/SaveConfiguration.php", form);
        yield return www;
    }
    private IEnumerator IENUM_GetPreference()
    {
        WWWForm form = new WWWForm();
        form.AddField("username", DBManager.username);
        WWW www = new WWW("http://localhost/sqlconnect/GetConfiguration.php", form);
        yield return www;
        string[] data = www.text.Split('\t');
        if(data[1] == "1")
        {
            DBManager.fullscreen = 1;
        }
        else
        {
            DBManager.fullscreen = 0;
        }
        DBManager.masterVol = float.Parse(data[2]);
        DBManager.musicVol = float.Parse(data[2]);
        DBManager.sfxVol = float.Parse(data[2]);
    }
    //private void UI_UpdateGameSettings()
    //{
    //    if (DBManager.fullscreen == 1)
    //    {
    //        uiManager.SetFullscreen(true);
    //    }
    //    else
    //    {
    //        uiManager.SetFullscreen(false);
    //    }
        

    //}

}
