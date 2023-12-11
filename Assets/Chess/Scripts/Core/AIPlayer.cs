using System.Threading.Tasks;
using System.Threading;
using UtilityLib;

public class AIPlayer : Player
{
    private Search search;
    private AISettings settings;
    private bool moveFound;
    private Move move;
    private Board board;
    private CancellationTokenSource cancelSearchTimer;
    private System.Random rng;

    private Book book;

    // Constructor
    public AIPlayer(Board board, AISettings settings)
    {
        this.settings = settings;
        this.board = board;
        rng = new System.Random();
        settings.requestAbortSearch += TimeOutThreadedSearch;

        search = new Search(board, settings);
        search.onSearchComplete += OnSearchComplete;
        book = BookCreator.LoadBookFromFile(settings.book);
    }

    // Method overrides
    public override void Update()
    {
        if (moveFound)
        {
            moveFound = false;
            ChoseMove(move);
        }
    }
    public override void NotifyTurnToMove()
    {
        moveFound = false;

        Move bookMove = Move.InvalidMove;
        if (settings.useBook && board.plyCount <= settings.maxBookPly)
        {
            if (book.HasPosition(board.ZobristKey))
            {
                bookMove = book.GetRandomBookMoveWeighted(board.ZobristKey);
            }
        }

        if (bookMove.IsInvalid)
        {
            if (settings.useThreading)
            {
                StartThreadedSearch();
            }
            else
            {
                StartSearch();
            }
        }
        else
        {
            int waitTime = (int)(settings.bookMoveDelayMs + (settings.bookMoveDelayRandomExtraMs * rng.NextDouble()));
            Task.Delay(waitTime).ContinueWith((t) => PlayBookMove(bookMove));
        }
    }

    // Custom method untuk searching dll
    void StartSearch()
    {
        search.StartSearch();
        moveFound = true;
    }
    void StartThreadedSearch()
    {
        Task.Factory.StartNew(() => search.StartSearch(), TaskCreationOptions.LongRunning);
        if (!settings.endlessSearchMode)
        {
            cancelSearchTimer = new CancellationTokenSource();
            Task.Delay(settings.searchTimeMillis, cancelSearchTimer.Token).ContinueWith((t) => TimeOutThreadedSearch());
        }
    }
    void TimeOutThreadedSearch()
    {
        if (cancelSearchTimer == null || !cancelSearchTimer.IsCancellationRequested)
        {
            search.EndSearch();
        }
    }
    void PlayBookMove(Move bookMove)
    {
        this.move = bookMove;
        moveFound = true;
    }
    void OnSearchComplete(Move move)
    {
        // Cancel search timer in case search finished before timer ran out (can happen when a mate is found)
        cancelSearchTimer?.Cancel();
        moveFound = true;
        this.move = move;
    }
}
