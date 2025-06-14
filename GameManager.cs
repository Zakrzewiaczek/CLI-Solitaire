namespace Solitaire
{
    using System.Diagnostics;
    using static SoundsManager.SoundFX.SoundType;


    public enum Difficulty
    {
        Easy,
        Hard
    }

    public class GameManager
    {
        /// <summary>
        /// Stopwatch for measuring elapsed game time.
        /// </summary>
        private readonly Stopwatch _stopwatch = new();
        /// <summary>
        /// Indicates whether music is enabled.
        /// </summary>
        private bool _musicEnabled = true;
        /// <summary>
        /// Indicates whether sound effects are enabled.
        /// </summary>
        private bool _soundsFXEnabled = true;
        /// <summary>
        /// Reference to the current board manager (game state).
        /// </summary>
        private BoardManager? _boardManager;
        /// <summary>
        /// Current game difficulty.
        /// </summary>
        private Difficulty _difficulty;
        public Difficulty Difficulty => _difficulty;

        /// <summary>
        /// Gets the elapsed time since the game started.
        /// </summary>
        public TimeSpan ElapsedTime => _stopwatch.Elapsed;

        /// <summary>
        /// Indicates whether music is enabled.
        /// </summary>
        public bool MusicEnabled
        {
            get => _musicEnabled;
            set
            {
                _musicEnabled = value;
            }
        }

        /// <summary>
        /// Indicates whether sound effects are enabled.
        /// </summary>
        public bool SoundsFXEnabled
        {
            get => _soundsFXEnabled;
            set
            {
                _soundsFXEnabled = value;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GameManager"/> class and sets up the game environment.
        /// </summary>
        public GameManager()
        {
            _boardManager = null;

            Initialize();
        }

        /// <summary>
        /// Initializes the console environment for the game (encoding, cursor, window size prompt).
        /// </summary>
        private static void Initialize()
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.CursorVisible = false;

            Renderer.Text.Write(
                text: $"Please resize the console window to at least {WindowStateManager.MinimumWidth}x{WindowStateManager.MinimumHeight} characters.",
                x: 0,
                y: 0,
                foregroundColor: ConsoleColor.Red);
            while (!WindowStateManager.HasWindowMinimumSize())
                Thread.Sleep(50);

            Console.Clear();
            Thread.Sleep(20);
        }

        /// <summary>
        /// Displays the start menu and allows the user to select game options.
        /// </summary>
        public void ShowStartMenu()
        {
            SoundsManager.Music.PlayMusic();
            _difficulty = StartMenu.ShowAndWait(ref _musicEnabled, ref _soundsFXEnabled);
        }

        /// <summary>
        /// Initializes the card stacks (tableau and stock) for a new game based on the selected difficulty.
        /// </summary>
        public void InitializeCardStacks()
        {
            List<List<Card>> tableau;
            List<Card> stock;
            (tableau, stock) = BoardGenerator.Generate(_difficulty);
            _boardManager = new(tableau, stock, _difficulty);
        }

        /// <summary>
        /// Handles window size changes, pausing or resuming the game and redrawing as needed.
        /// </summary>
        private void OnWindowSizeChanged()
        {
            if (!WindowStateManager.IsWindowAvailable)
            {
                if (_stopwatch.IsRunning)
                    _stopwatch.Stop();

                Console.Clear();
                Renderer.Text.Write(
                    text: $"Please resize the console window to at least {WindowStateManager.MinimumWidth}x{WindowStateManager.MinimumHeight} characters.",
                    x: 0,
                    y: 0,
                    foregroundColor: ConsoleColor.Red);
            }
            else
            {
                if (_stopwatch.IsRunning)
                    _stopwatch.Start();

                // Redraw game
                _boardManager!.DrawGame();
            }
        }

        /// <summary>
        /// Main game loop. Handles user input, game state, and victory detection.
        /// </summary>
        public void Start()
        {
            _stopwatch.Start();
            WindowStateManager.StartWindowSizeWatcher(OnWindowSizeChanged);

            bool isGameRunning = true;
            while (isGameRunning)
            {
                _boardManager!.DrawGame();

                ConsoleKey key = Helpers.ReadKey();

                while (!WindowStateManager.IsWindowAvailable)
                    Thread.Sleep(50);

                switch (key)
                {
                    case ConsoleKey.UpArrow:
                        _boardManager.MovePointer(Direction.Up);
                        break;

                    case ConsoleKey.DownArrow:
                        _boardManager.MovePointer(Direction.Down);
                        break;

                    case ConsoleKey.LeftArrow:
                        _boardManager.MovePointer(Direction.Left);
                        break;

                    case ConsoleKey.RightArrow:
                        _boardManager.MovePointer(Direction.Right);
                        break;

                    case ConsoleKey.Enter:
                        _boardManager.PointerAction();
                        break;

                    case ConsoleKey.Spacebar:
                        _boardManager.ResetSelection();
                        break;

                    case ConsoleKey.Escape:
                        _stopwatch.Stop();
                        SoundsManager.SoundFX.PlaySound(GamePause);

                        int selection;
                        do
                        {
                            selection = EscapeMenu.ShowPauseMenuAndWait();
                            switch (selection)
                            {
                                case 1:
                                    EscapeMenu.ShowOptionsMenuAndWait(ref _musicEnabled, ref _soundsFXEnabled);
                                    SoundsManager.Music.IsMusicEnabled = _musicEnabled;
                                    SoundsManager.SoundFX.IsSoundFXEnabled = _soundsFXEnabled;
                                    break;
                                case 2:
                                    isGameRunning = false;
                                    selection = 0;
                                    break;
                            }
                        }
                        while (selection != 0);

                        SoundsManager.SoundFX.PlaySound(GameResume);
                        _stopwatch.Start();
                        break;
                }

                if (_boardManager!.CheckVictory())
                {
                    _stopwatch.Stop();
                    isGameRunning = false;

                    SoundsManager.Music.IsMusicEnabled = false;

                    Victory.ShowAnimation();

                    SoundsManager.Music.IsMusicEnabled = _musicEnabled;

                    Victory.ShowInfoAboutGame(_boardManager.Difficulty, _stopwatch.Elapsed, _boardManager.MovesCount);
                }
            }

            WindowStateManager.StopWindowSizeWatcher();
        }
    }
}