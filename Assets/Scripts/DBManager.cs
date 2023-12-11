using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class DBManager
{
    // user
    public static string username;
    public static int score;
    public static int highestScore;
    public static int gamesWon;
    public static int gamesLost;
    public static int gamesDraw;

    // savedgames
    public static int saved_GameID;
    public static string saved_FEN;
    public static int saved_gameMode;
    public static int saved_vsAI;
    public static int saved_difficultyAI;

    // preference
    public static int fullscreen;
    public static float masterVol;
    public static float musicVol;
    public static float sfxVol;

    public static bool loggedIn { get { return username != null; } }

    public static void LogOut()
    {
        username = null;
    }
}
