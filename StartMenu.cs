namespace Solitaire
{
    using static SoundsManager.SoundFX.SoundType;


    /// <summary>
    /// Displays the start menu and handles user input.
    /// Renders the game title, author art, and menu options.
    /// </summary>
    public static class StartMenu
    {
        // --- Menu layout constants ---
        private const int EasyModeY = 33;
        private const int HardModeY = 36;
        private const int OptionsY = 40; // nowa opcja
        private const int ExitY = 44;    // przesunięte w dół
        private const int MenuStartLine = 33;
        private const int MenuEndLine = 46;
        private const int TotalOptions = 4;

        // --- Menu appearance constants ---
        private const ConsoleColor selectedItemColor = ConsoleColor.DarkYellow;
        private const ConsoleColor defaultItemColor = ConsoleColor.White;
        private const int FrameWidth = 24;
        private const int CenteredTextWidth = 20;

        // --- Menu option texts ---
        private const string EasyModeText = "New game - Easy mode";
        private const string HardModeText = "New game - Hard mode";
        private const string OptionsText = "Options"; // nowa opcja
        private const string ExitText = "Exit";

        // --- Title and subtitle (ASCII art) ---
        /// <summary>ASCII art for the main game title.</summary>
        private const string TitleArt =
@"   ▄████████  ▄██████▄  ▄██   ▄      ▄████████  ▄█               ▄████████  ▄██████▄   ▄█        ▄█      ███        ▄████████  ▄█     ▄████████    ▄████████
  ███    ███ ███    ███ ███   ██▄   ███    ███ ███              ███    ███ ███    ███ ███       ███  ▀█████████▄   ███    ███ ███    ███    ███   ███    ███
  ███    ███ ███    ███ ███▄▄▄███   ███    ███ ███              ███    █▀  ███    ███ ███       ███▌    ▀███▀▀██   ███    ███ ███▌   ███    ███   ███    █▀ 
 ▄███▄▄▄▄██▀ ███    ███ ▀▀▀▀▀▀███   ███    ███ ███              ███        ███    ███ ███       ███▌     ███   ▀   ███    ███ ███▌  ▄███▄▄▄▄██▀  ▄███▄▄▄    
▀▀███▀▀▀▀▀   ███    ███ ▄██   ███ ▀███████████ ███            ▀███████████ ███    ███ ███       ███▌     ███     ▀███████████ ███▌ ▀▀███▀▀▀▀▀   ▀▀███▀▀▀    
▀███████████ ███    ███ ███   ███   ███    ███ ███                     ███ ███    ███ ███       ███      ███       ███    ███ ███  ▀███████████   ███    █▄ 
  ███    ███ ███    ███ ███   ███   ███    ███ ███▌    ▄         ▄█    ███ ███    ███ ███▌    ▄ ███      ███       ███    ███ ███    ███    ███   ███    ███
  ███    ███  ▀██████▀   ▀█████▀    ███    █▀  █████▄▄██       ▄████████▀   ▀██████▀  █████▄▄██ █▀      ▄████▀     ███    █▀  █▀     ███    ███   ██████████
  ███    ███                                   ▀                                      ▀                                              ███    ███              ";

        /// <summary>ASCII art for the subtitle/author line.</summary>
        private const string SubtitleArt =
            "            ___     _  _                _            _              _                 ____            _                                               _        _    \r\n" +
            "    o O O  | _ )   | || |    o O O   _ | |  __ _    | |__   _  _   | |__      o O O  |_  /   __ _    | |__     _ _     ___    ___   __ __ __  ___    | |__    (_)   \r\n" +
            "   o       | _ \\    \\_, |   o       | || | / _` |   | / /  | +| |  | '_ \\    o        / /   / _` |   | / /    | '_|   |_ /   / -_)  \\ V  V / (_-<    | / /    | |   \r\n" +
            "  TS__[O]  |___/   _|__/   TS__[O]  _\\__/  \\__,_|   |_\\_\\   \\_,_|  |_.__/   TS__[O]  /___|  \\__,_|   |_\\_\\   _|_|_   _/__|   \\___|   \\_/\\_/  /__/_   |_\\_\\   _|_|_  \r\n" +
            " {======|_|\"\"\"\"\"|_| \"\"\"\"| {======|_|\"\"\"\"\"|_|\"\"\"\"\"|_|\"\"\"\"\"|_|\"\"\"\"\"|_|\"\"\"\"\"| {======|_|\"\"\"\"\"|_|\"\"\"\"\"|_|\"\"\"\"\"|_|\"\"\"\"\"|_|\"\"\"\"\"|_|\"\"\"\"\"|_|\"\"\"\"\"|_|\"\"\"\"\"|_|\"\"\"\"\"|_|\"\"\"\"\"| \r\n" +
            "./o--000'\"`-0-0-'\"`-0-0-'./o--000'\"`-0-0-'\"`-0-0-'\"`-0-0-'\"`-0-0-'\"`-0-0-'./o--000'\"`-0-0-'\"`-0-0-'\"`-0-0-'\"`-0-0-'\"`-0-0-'\"`-0-0-'\"`-0-0-'\"`-0-0-'\"`-0-0-'\"`-0-0-' \r\n";

        private const ConsoleColor TitleColor = ConsoleColor.DarkYellow; // Color for the main title
        private const ConsoleColor SubtitleColor = ConsoleColor.Gray; // Color for the subtitle/author art
        private const int TitleY = 7; // Y position for the main title
        private const int SubtitleY = 17; // Y position for the subtitle/author art

        /// <summary>
        /// Shows the start menu and waits for a valid user input.
        /// Displays the game title, author art, and game difficulties (New Game - [...], Exit).
        /// Uses Up/Down arrow keys for navigation and Enter to confirm a selection.
        /// </summary>
        /// <returns>A <see cref="Difficulty"/> enum value representing the selected game difficulty.</returns>
        public static Difficulty ShowAndWait(ref bool musicEnabled, ref bool soundsFXEnabled)
        {
            int selection = 0;
            ConsoleKey key;

            Console.Clear();

            void RedrawMenu()
            {
                Console.Clear();
                DrawTitle();
                DrawMenuOptions(selection);
            }

            void OnWindowSizeChanged()
            {
                if (!WindowStateManager.IsWindowAvailable)
                {
                    Console.Clear();
                    Renderer.Text.Write(
                        text: $"Please resize the console window to at least {WindowStateManager.MinimumWidth}x{WindowStateManager.MinimumHeight} characters.",
                        x: 0,
                        y: 0,
                        foregroundColor: ConsoleColor.Red);
                }
                else
                {
                    RedrawMenu();
                }
            }
            WindowStateManager.StartWindowSizeWatcher(OnWindowSizeChanged);

            DrawTitle();
            DrawMenuOptions(selection);

            do
            {
                key = Helpers.ReadKey();
                SoundsManager.SoundFX.PlaySound(Operation);

                while (!WindowStateManager.IsWindowAvailable)
                    Thread.Sleep(50);

                int newSelection = UpdateSelection(key, selection);
                if (newSelection != selection)
                {
                    ClearMenuArea();
                    DrawMenuOptions(newSelection);
                    selection = newSelection;
                }
                else if (key == ConsoleKey.Enter && selection == 2) // Options
                {
                    Console.Clear();
                    EscapeMenu.ShowOptionsMenuAndWait(ref musicEnabled, ref soundsFXEnabled);
                    SoundsManager.Music.IsMusicEnabled = musicEnabled;
                    SoundsManager.SoundFX.IsSoundFXEnabled = soundsFXEnabled;
                    RedrawMenu();
                }
            }
            while (key != ConsoleKey.Enter || selection == 2);

            WindowStateManager.StopWindowSizeWatcher();

            if (selection == 3)
            {
                Environment.Exit(0);
            }

            SoundsManager.SoundFX.PlaySound(StartGame);

            return (Difficulty)Enum.GetValues<Difficulty>().GetValue(selection > 1 ? selection - 1 : selection)!;
        }

        /// <summary>
        /// Clears the area where menu options are drawn to reduce flickering.
        /// Clears lines from MenuStartLine to MenuEndLine.
        /// </summary>
        private static void ClearMenuArea()
        {
            for (int i = MenuStartLine; i <= MenuEndLine; i++)
            {
                Console.SetCursorPosition(0, i);
                Console.Write(new string(' ', Console.WindowWidth));
            }
        }

        /// <summary>
        /// Draws the game title and introductory author art.
        /// </summary>
        private static void DrawTitle()
        {
            Renderer.Text.Write(
                text: TitleArt,
                align: Renderer.Align.Center,
                y: TitleY,
                foregroundColor: TitleColor);

            Renderer.Text.Write(
                text: SubtitleArt,
                align: Renderer.Align.Center,
                y: SubtitleY,
                foregroundColor: SubtitleColor);
        }

        /// <summary>
        /// Draws all menu options with highlighting for the selected option.
        /// </summary>
        /// <param name="selection">The current index of the selected menu option.</param>
        private static void DrawMenuOptions(int selection)
        {
            DrawMenuOption(EasyModeY, selection == 0, EasyModeText);
            DrawMenuOption(HardModeY, selection == 1, HardModeText);
            DrawMenuOption(OptionsY, selection == 2, OptionsText);
            DrawMenuOption(ExitY, selection == 3, ExitText);
        }

        /// <summary>
        /// Draws an individual menu option with centered text and a custom frame.
        /// </summary>
        /// <param name="y">Y-coordinate for rendering the menu option.</param>
        /// <param name="isSelected">Determines if the option should be highlighted.</param>
        /// <param name="text">The menu option text.</param>
        private static void DrawMenuOption(int y, bool isSelected, string text)
        {
            ConsoleColor color = isSelected ? selectedItemColor : defaultItemColor;
            // Center the text within a fixed inner width.
            string centeredText = CenterText(text, CenteredTextWidth);
            // Build a framed text with a fixed frame width of 24.
            string framedText = string.Format("╔{0}╗\r\n║  {1}  ║\r\n╚{0}╝", new string('═', FrameWidth), centeredText);

            Renderer.Text.Write(
                text: framedText,
                align: Renderer.Align.Center,
                y: y,
                foregroundColor: color);
        }

        /// <summary>
        /// Updates the selection index based on the pressed key.
        /// Supports UpArrow and DownArrow navigation with wrapping.
        /// </summary>
        /// <param name="key">The key pressed by the user.</param>
        /// <param name="currentSelection">The current selection index.</param>
        /// <returns>The updated selection index.</returns>
        private static int UpdateSelection(ConsoleKey key, int currentSelection)
        {
            return key switch
            {
                ConsoleKey.UpArrow => (currentSelection - 1 + TotalOptions) % TotalOptions,
                ConsoleKey.DownArrow => (currentSelection + 1) % TotalOptions,
                _ => currentSelection
            };
        }

        /// <summary>
        /// Centers the provided text within the specified width.
        /// If the text exceeds the width, it is truncated.
        /// </summary>
        /// <param name="text">The text to be centered.</param>
        /// <param name="width">The width available for the text.</param>
        /// <returns>A centered (or truncated) string.</returns>
        private static string CenterText(string text, int width)
        {
            if (text.Length >= width)
            {
                return text[..width];
            }
            int leftPadding = (width - text.Length) / 2;
            int rightPadding = width - text.Length - leftPadding;
            return new string(' ', leftPadding) + text + new string(' ', rightPadding);
        }
    }
}