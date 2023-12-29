using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
public class SavedGamesButtonVariables : MonoBehaviour
{
    public AISettings easy, med, hard, insane;
    public TextMeshProUGUI datetime, vsai_level, gamemode;
    public string FEN;
    public bool isWhite;
    public int savedGameID;
    public int white_time;
    public int black_time;
    public void CallBehaviour()
    {
        GameManager.gameMode = gamemode.text;
        string[] concat_value = vsai_level.text.Split(' ');
        if (concat_value[0] == "AI")
        {
            GameManager.versusAI = true;
        }
        else
        {
            GameManager.versusAI = false;
        }
        GameManager.isWhite = isWhite;
        if (concat_value[1] == "(0)")
        {
            GameManager.aiSettings = easy;
        }
        else if (concat_value[1] == "(1)")
        {
            GameManager.aiSettings = med;
        }
        else if (concat_value[1] == "(2)")
        {
            GameManager.aiSettings = hard;
        }
        else if (concat_value[1] == "(3)")
        {
            GameManager.aiSettings = insane;
        }
        GameManager.loadedGameFEN = FEN;
        GameManager.loadedGameID = savedGameID;
        GameManager.white_time = white_time;
        GameManager.black_time = black_time;
        SceneManager.LoadScene(4);
    }
}
