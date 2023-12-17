using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] private Slider UI_APPSETTINGS_MASTERVOL;
    [SerializeField] private Toggle UI_APPSETTINGS_FULLSCREEN;
    public void SetGameMode(string value)
    {
        GameManager.gameMode = value;
    }
    public void SetVersusAI(bool value)
    {
        GameManager.versusAI = value;
    }
    public void PlayRandomTeam()
    {
        int value = Random.Range(-9999, 9999);
        GameManager.versusAI = (value % 2 == 0);
    }
    public void SetIsWhite(bool value)
    {
        GameManager.isWhite = value;
    }
    public void SetAISettings(AISettings value)
    {
        GameManager.aiSettings = value;
    }
    public void InitializeNewGame()
    {
        GameManager.loadedGameFEN = "";
        GameManager.loadedGameID = -1;
    }
    public void SetFullscreen(bool value)
    {
        Screen.fullScreen = value;
    }
}
