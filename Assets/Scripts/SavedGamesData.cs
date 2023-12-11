using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SavedGamesData
{
    public int savedGameID;
    public string FEN;
    public string gameMode;
    public bool vsAI;
    public int difficultyAI;
    public bool isWhite;
    public string lastModified;
    public int numRows;
}
