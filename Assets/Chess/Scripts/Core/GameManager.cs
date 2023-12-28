using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UtilityLib;
public class GameManager : MonoBehaviour {

    // Game State.
	private enum Result { Playing, WhiteIsMated, BlackIsMated,
                          Stalemate, Repetition, FiftyMoveRule,
                          InsufficientMaterial, Paused, BlackTimeOut, WhiteTimeOut }
    // Tipe Player.
	private enum PlayerType { Human, AI }

    // FEN adalah kode string yang digunakan untuk
    // menyimpan setiap posisi dari catur.
    // More info: https://www.chess.com/terms/fen-chess
    public bool loadFEN;
	public string customFEN = "1rbq1r1k/2pp2pp/p1n3p1/2b1p3/R3P3/1BP2N2/1P3PPP/1NBQ1RK1 w - - 0 1";
    public string currentFEN = "";

    // Dalam permainan catur sudah dipastikan
    // hanya ada dua pemain yaitu tim putih & hitam.
	[SerializeField] private PlayerType whiteTeam;
    [SerializeField] private PlayerType blackTeam;

    [SerializeField] private Color[] colors;
    [SerializeField] private Clock whiteClock;
    [SerializeField] private Clock blackClock;

    public bool pause = false;

    // Database UI
    [Header("Database UI variables")]
    [SerializeField] private TextMeshProUGUI usernameText;
    [SerializeField] private TextMeshProUGUI currentELOText;
    [SerializeField] private TextMeshProUGUI winText;
    [SerializeField] private TextMeshProUGUI loseText;
    [SerializeField] private TextMeshProUGUI drawText;

    // UI untuk ketika game telah selesai
    [Header("End Game UI variables")]
    [SerializeField] private TextMeshProUGUI endTitleText;
    [SerializeField] private TextMeshProUGUI endELOText;
    [SerializeField] private TextMeshProUGUI prevELOText;
    [SerializeField] private GameObject endGroupUI;
    [SerializeField] private GameObject goBackButton;

    [Header("Game UI Variables")]
    [SerializeField] private GameObject clockGO;
    [SerializeField] private GameObject noClockGO;

    // Extra variable untuk database dll
    //[SerializeField] private string gameMode = "BULLET";
    private int scoreGained = 0;

    // Variable statis utk nyimpan data game dari scene ke scene lain
    public static bool isWhite;
    public static AISettings aiSettings;
    public static string gameMode; // 0 utk no time limit (useClocks direplace dgn gameModeForCurrentGame > 0)
    public static bool versusAI;
    public static string loadedGameFEN;
    public static int loadedGameID;

    // State atau Kondisi dari permainan yang sedang berlangsung
    private Result gameResult;
    private Result prevGameResult;

    // Variabel player, dll
    private Player whitePlayer;
    private Player blackPlayer;
    private Player playerToMove;
    private List<Move> gameMoves;
    private BoardUI boardUI;

    // Variabel board untuk permainan
    // Variabel searchBoard untuk perhitungan di AI-nya
    public Board board { get; private set; }
    private Board searchBoard;

    /// <summary>
    /// Method-method yang diturunkan dari main class
    /// Start() -> dipanggil 1 kali ketika awal mulai permainan
    /// Update() -> dipanggil terus-menerus hingga aplikasi berhenti
    /// </summary>
	private void Start() {
        if (loadedGameFEN != "" && loadedGameFEN != null)
        {
            customFEN = loadedGameFEN;
            loadFEN = true;
        }
		if (gameMode == "NO_TIME_LIMIT") {
            whiteClock.enabled = false;
            blackClock.enabled = false;
            clockGO.SetActive(false);
            noClockGO.SetActive(true);
		}
        else
        {
            whiteClock.isTurnToMove = false;
            blackClock.isTurnToMove = false;
            whiteClock.enabled = true;
            blackClock.enabled = true;
            clockGO.SetActive(true);
            noClockGO.SetActive(false);
            if (gameMode == "BULLET")
            {
                whiteClock.startSeconds = 60;
                blackClock.startSeconds = 60;
            }
            else if (gameMode == "BLITZ")
            {
                whiteClock.startSeconds = 300;
                blackClock.startSeconds = 300;
            }
            else if (gameMode == "RAPID")
            {
                whiteClock.startSeconds = 600;
                blackClock.startSeconds = 600;
            }
        }
        if (versusAI)
        {
            if (isWhite)
            {
                whiteTeam = PlayerType.Human;
                blackTeam = PlayerType.AI;
            }
            else
            {
                whiteTeam = PlayerType.AI;
                blackTeam = PlayerType.Human;
            }
        }
        else
        {
            whiteTeam = PlayerType.Human;
            blackTeam = PlayerType.Human;
        }
		boardUI = FindObjectOfType<BoardUI> ();
		gameMoves = new List<Move> ();
		board = new Board ();
		searchBoard = new Board ();

		NewGame(whiteTeam, blackTeam);
        HandleDatabase();
	}
    private void Update () {
        playerToMove.Update();
        if (gameMode != "NO_TIME_LIMIT")
        {
            if (whiteClock.isTurnToMove && whiteClock.secondsRemaining <= 0)
            {
                gameResult = Result.WhiteTimeOut;
            }
            else if (blackClock.isTurnToMove && blackClock.secondsRemaining <= 0)
            {
                gameResult = Result.BlackTimeOut;
            }
            whiteClock.isTurnToMove = board.WhiteToMove;
            blackClock.isTurnToMove = !board.WhiteToMove;
        }
    }

    /// <summary>
    /// Method-method yang digunakan untuk permainan catur
    /// </summary>
    public void PauseGame(bool value)
    {
        if (value)
        {
            prevGameResult = gameResult;
            gameResult = Result.Paused;
        }
        else
        {
            gameResult = prevGameResult;
        }
        whiteClock.isPaused = value;
        blackClock.isPaused = value;
    }
	void OnMoveChosen (Move move) {
		bool animateMove = playerToMove is AIPlayer;
		board.MakeMove (move);
		searchBoard.MakeMove (move);

		gameMoves.Add(move);
		//onMoveMade?.Invoke (move);
		boardUI.OnMoveMade(board, move, animateMove);

		NotifyPlayerToMove ();
	}
	private void NewGame (PlayerType whiteTeam, PlayerType blackTeam) {
        if(whiteTeam == PlayerType.Human)
        {
            boardUI.SetPerspective(true);
        }
        else
        {
            boardUI.SetPerspective(false);
        }
		gameMoves.Clear ();
        
		if (loadFEN) {
			board.LoadPosition(customFEN);
			searchBoard.LoadPosition(customFEN);
		} else {
			board.LoadStartPosition ();
			searchBoard.LoadStartPosition ();
		}

        //onPositionLoaded?.Invoke();
        boardUI.UpdatePosition(board);
		boardUI.ResetSquareColours ();

		CreatePlayer (ref whitePlayer, whiteTeam);
		CreatePlayer (ref blackPlayer, blackTeam);

		gameResult = Result.Playing;
        GameResultState(gameResult);

		NotifyPlayerToMove ();

	}
    private void NotifyPlayerToMove () {
		gameResult = GetGameState();
        GameResultState(gameResult);

		if (gameResult == Result.Playing) {
			playerToMove = (board.WhiteToMove) ? whitePlayer : blackPlayer;
			playerToMove.NotifyTurnToMove ();

		} else {
			Debug.Log ("Game Over");
		}
	}

    /// <summary>
    /// Method-method yang digunakan untuk keperluan database
    /// </summary>
    private void HandleDatabase()
    {
        if (!DBManager.loggedIn)
            SceneManager.LoadScene(0);
        usernameText.SetText("User: " + DBManager.username);
        currentELOText.SetText("Current ELO: " + DBManager.score);
        winText.SetText("Games Won: " + DBManager.gamesWon);
        loseText.SetText("Games Lost: " + DBManager.gamesLost);
        drawText.SetText("Games Draw: " + DBManager.gamesDraw);
    }
    private int GetVSAI()
    {
        return whiteTeam == blackTeam ? 0 : 1;
    }
    private void GameResultState (Result result) {
        if (result == Result.Playing || result == Result.Paused)
        {
            return;
        }
        else if (result == Result.WhiteIsMated || result == Result.WhiteTimeOut)
        {
            endTitleText.SetText("Black Wins!");
            if (whiteTeam == PlayerType.AI)
            {
                EvaluateScore(true);
            }
            else
            {
                EvaluateScore(false);
            }
        }
        else if (result == Result.BlackIsMated || result == Result.BlackTimeOut)
        {
            endTitleText.SetText("White Wins!");
            if (blackTeam == PlayerType.AI)
            {
                EvaluateScore(true);
            }
            else
            {
                EvaluateScore(false);
            }
        }
        else if (result == Result.FiftyMoveRule) {
            endTitleText.SetText("Draw! (50 Move Rule)");
            if (whiteTeam != blackTeam)
                DBManager.gamesDraw += 1;
        }
        else if (result == Result.Repetition) {
            endTitleText.SetText("Draw! (3 Fold Repetition)");
            if (whiteTeam != blackTeam)
                DBManager.gamesDraw += 1;
        }
        else if (result == Result.Stalemate) {
            endTitleText.SetText("Draw! (Stalemate)");
            if (whiteTeam != blackTeam)
                DBManager.gamesDraw += 1;
        }
        else if (result == Result.InsufficientMaterial) {
            endTitleText.SetText("Draw! (Insufficient Material)");
            if (whiteTeam != blackTeam)
                DBManager.gamesDraw += 1;
        }

        if (gameMode != "NO_TIME_LIMIT")
        {
            whiteClock.isTurnToMove = false;
            blackClock.isTurnToMove = false;
            whiteClock.isPaused = true;
            blackClock.isPaused = true;
        }
        endELOText.SetText("Your Current Score: " + DBManager.score);
        endGroupUI.SetActive(true);
        goBackButton.SetActive(false);
        StartCoroutine(InsertGameHistory());
    }
    private void EvaluateScore(bool playerWins)
    {
        if (whiteTeam == blackTeam)
            return;
        prevELOText.SetText("Previous Score: " + DBManager.score);
        int val = 0;
        if (aiSettings.difficulty == AISettings.Difficulty.EASY)
        {
            val = Random.Range(3, 9);
        }
        else if (aiSettings.difficulty == AISettings.Difficulty.MEDIUM)
        {
            val = Random.Range(11, 23);
        }
        else if (aiSettings.difficulty == AISettings.Difficulty.HARD)
        {
            val = Random.Range(25, 37);
        }
        else if (aiSettings.difficulty == AISettings.Difficulty.INSANE)
        {
            val = Random.Range(40, 53);
        }

        if (playerWins)
        {
            DBManager.score += val;
            DBManager.gamesWon += 1;
            scoreGained = val;
        }
        else
        {
            DBManager.score -= val;
            DBManager.gamesLost += 1;
            scoreGained = -val;
        }
        if (DBManager.score > DBManager.highestScore)
        {
            DBManager.highestScore = DBManager.score;
        }
    }
    public void CallInsertSavedGame()
    {
        StartCoroutine(InsertSavedGame());
    }
    private IEnumerator InsertGameHistory()
    {
        // Create new WWWForm
        WWWForm form = new WWWForm();

        // Add forms, username, etc
        form.AddField("username", DBManager.username);
        form.AddField("gameresult", (int)gameResult);
        form.AddField("gamemode", gameMode);
        form.AddField("vsai", GetVSAI());
        form.AddField("scoregained", scoreGained);
        form.AddField("fen", FenUtility.CurrentFen(board));
        form.AddField("_newcurrentscore", DBManager.score);
        form.AddField("win", DBManager.gamesWon);
        form.AddField("lose", DBManager.gamesLost);
        form.AddField("draw", DBManager.gamesDraw);

        // Jika bermain lawan AI, maka set AI difficulty > 0
        // Jika bermain local / sesama player, set difficulty -1
        if (GetVSAI() > 0)
        {
            form.AddField("difficultyai", (int)aiSettings.difficulty);
        }
        else
        {
            form.AddField("difficultyai", -1);
        }

        // Jika user yang sedang bermain adalah tim putih, maka set isWhite = 1
        // jika tidak, maka isWhite = 0
        // jika bermain local / sesama player, biarkan isWhite = 1
        if(whiteTeam == PlayerType.Human)
        {
            form.AddField("iswhite", 1);
        }
        else
        {
            form.AddField("iswhite", 0);
        }

        // Jika user ingin bermain game yang sudah disaved sebelumnya,
        // maka set nilai "overridegame" menjadi "TRUE" dan savedGameID menjadi
        // ID game yang telah dipilih.
        // Nilai dari "overridegame" akan menjadi "FALSE" jika user ingin
        // bermain game baru (local ataupun AI) dan juga nilai dari savedGameID menjadi -1.
        // Nilai "overridegame" hanya ada di script PHP dan digunakan
        // hanya untuk pengkondisian, sedangkan savedGameID ada di PHP dan juga database.
        if (loadFEN)
        {
            form.AddField("overridegame", "TRUE");
            form.AddField("savedgameid", loadedGameID);
        }
        else
        {
            form.AddField("overridegame", "FALSE");
            form.AddField("savedgameid", "-1");
        }

        // Create new WWW dan akses file PHP untuk
        // melakukan penambahan game baru / load game baru
        WWW www = new WWW("http://localhost/sqlconnect/InsertGameHistory.php", form);
        yield return www;
        if (www.text == "0")
        {
            Debug.Log("Insert game history success");
        }
        else
        {
            Debug.LogError("Insert game history failed.");
            Debug.LogError(www.text);
        }
    }
    private IEnumerator InsertSavedGame()
    {
        WWWForm form = new WWWForm();

        // bagian insert to savedgames
        form.AddField("username", DBManager.username);
        form.AddField("gamemode", gameMode);
        form.AddField("vsai", GetVSAI());
        if (GetVSAI() > 0)
        {
            form.AddField("difficultyai", (int)aiSettings.difficulty);
        }
        else
        {
            form.AddField("difficultyai", -1);
        }
            
        form.AddField("fen", FenUtility.CurrentFen(board));
        if (whiteTeam == PlayerType.Human) //help 
        {
            form.AddField("iswhite", 1);
        }
        else
        {
            form.AddField("iswhite", 0);
        }

        if (loadFEN)
        {
            form.AddField("overridegame", "TRUE");
            form.AddField("savedgameid", loadedGameID);
        }
        else
        {
            form.AddField("overridegame", "FALSE");
            form.AddField("savedgameid", "-1");
        }

        WWW www = new WWW("http://localhost/sqlconnect/SaveGame.php", form);
        yield return www;
        if (www.text == "0")
        {
            Debug.Log("Insert savedgame success");
        }
        else
        {
            Debug.LogError("Insert savedgame failed.");
            Debug.LogError(www.text);
        }
        SceneManager.LoadScene(3);
    }
    private Result GetGameState() {
		MoveGenerator moveGenerator = new MoveGenerator ();
		var moves = moveGenerator.GenerateMoves (board);

        if (gameMode != "NO_TIME_LIMIT")
        {
            if (whiteClock.isTurnToMove && whiteClock.secondsRemaining <= 0)
            {
                return Result.WhiteTimeOut;
            }
            else if(blackClock.isTurnToMove && blackClock.secondsRemaining <= 0)
            {
                return Result.BlackTimeOut;
            }
        }

		// Look for mate/stalemate
		if (moves.Count == 0) {
			if (moveGenerator.InCheck ()) {
				return (board.WhiteToMove) ? Result.WhiteIsMated : Result.BlackIsMated;
			}
			return Result.Stalemate;
		}

		// Fifty move rule
		if (board.fiftyMoveCounter >= 100) {
			return Result.FiftyMoveRule;
		}

		// Threefold repetition
		int repCount = board.RepetitionPositionHistory.Count ((x => x == board.ZobristKey));
		if (repCount == 3) {
			return Result.Repetition;
		}

		// Look for insufficient material (not all cases implemented yet)
		int numPawns = board.pawns[Board.WhiteIndex].Count + board.pawns[Board.BlackIndex].Count;
		int numRooks = board.rooks[Board.WhiteIndex].Count + board.rooks[Board.BlackIndex].Count;
		int numQueens = board.queens[Board.WhiteIndex].Count + board.queens[Board.BlackIndex].Count;
		int numKnights = board.knights[Board.WhiteIndex].Count + board.knights[Board.BlackIndex].Count;
		int numBishops = board.bishops[Board.WhiteIndex].Count + board.bishops[Board.BlackIndex].Count;

		if (numPawns + numRooks + numQueens == 0) {
			if (numKnights == 1 || numBishops == 1) {
				return Result.InsufficientMaterial;
			}
		}
            

		return Result.Playing;
	}
    private void CreatePlayer(ref Player player, PlayerType playerType) {
		if (player != null) {
			player.onMoveChosen -= OnMoveChosen;
		}
		if (playerType == PlayerType.Human) {
			player = new HumanPlayer (board);
		} else {
			player = new AIPlayer (searchBoard, aiSettings);
		}
		player.onMoveChosen += OnMoveChosen;
	}
}
