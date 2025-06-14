namespace Solitaire
{
    using static SoundsManager.SoundFX.SoundType;


    public static class EscapeMenu
    {
        /// <summary>
        /// Pause menu title ASCII art.
        /// </summary>
        private readonly static string _pauseMenuTitle =
            @" ___     _     _   _   ___   ___   ___  " + Environment.NewLine +
            @"| _ \   /_\   | | | | / __| | __| |   \ " + Environment.NewLine +
            @"|  _/  / _ \  | |_| | \__ \ | _|  | |) |" + Environment.NewLine +
            @"|_|   /_/ \_\  \___/  |___/ |___| |___/ ";

        /// <summary>
        /// Options menu title ASCII art.
        /// </summary>
        private readonly static string _optionsMenuTitle =
            @"  ___    ___   _____   ___    ___    _  _   ___ " + Environment.NewLine +
            @" / _ \  | _ \ |_   _| |_ _|  / _ \  | \| | / __|" + Environment.NewLine +
            @"| (_) | |  _/   | |    | |  | (_) | | .` | \__ \" + Environment.NewLine +
            @" \___/  |_|     |_|   |___|  \___/  |_|\_| |___/";

        // --- Layout constants ---

        /// <summary>
        /// Width of the menu frame.
        /// </summary>
        private const int MenuFrameWidth = 75;
        /// <summary>
        /// Y position where the menu starts.
        /// </summary>
        private const int MenuStartY = 18;
        /// <summary>
        /// Height of the menu.
        /// </summary>
        private const int MenuHeight = 20;
        /// <summary>
        /// Y position where menu options start.
        /// </summary>
        private const int MenuOptionStartY = 25;

        // --- Option texts ---

        /// <summary>
        /// Texts for the pause menu options.
        /// </summary>
        private static readonly string[] PauseMenuOptions = ["Back to the game!", "Options", "Back to main menu"];

        // --- Options menu icons ---

        /// <summary>
        /// Icon for sounds off.
        /// </summary>
        private const string SoundsOffIcon = "🔇";
        /// <summary>
        /// Icon for sounds on.
        /// </summary>
        private const string SoundsOnIcon = "🔊";

        // --- Colors ---

        /// <summary>
        /// Color for the pause menu title.
        /// </summary>
        private const ConsoleColor PauseMenuTitleColor = ConsoleColor.White;
        /// <summary>
        /// Color for the options menu title.
        /// </summary>
        private const ConsoleColor OptionsMenuTitleColor = ConsoleColor.Gray;
        /// <summary>
        /// Color for the selected menu option.
        /// </summary>
        private const ConsoleColor MenuSelectedColor = ConsoleColor.DarkYellow;
        /// <summary>
        /// Color for the default menu option.
        /// </summary>
        private const ConsoleColor MenuDefaultColor = ConsoleColor.White;
        /// <summary>
        /// Color for the menu frame.
        /// </summary>
        private const ConsoleColor MenuFrameColor = ConsoleColor.Gray;

        public static int ShowPauseMenuAndWait()
        {
            int selection = 0;
            ConsoleKey key;

            DrawPauseMenu(selection);

            do
            {
                key = Helpers.ReadKey();

                int newSelection = UpdateSelection(key, selection);
                if (newSelection != selection)
                {
                    selection = newSelection;
                    DrawPauseMenu(selection);
                    SoundsManager.SoundFX.PlaySound(Operation);
                }
            }
            while (key != ConsoleKey.Enter);

            SoundsManager.SoundFX.PlaySound(Operation);
            return selection; // 0: Back to game, 1: Options, 2: Back to main menu
        }

        private static void DrawPauseMenu(int selection)
        {
            DrawMenuFrame();
            DrawPauseTitle();
            DrawPauseOptions(selection);
        }

        /// <summary>
        /// Draws the pause menu frame (border and background) in the console window.
        /// </summary>
        private static void DrawMenuFrame()
        {
            Console.ForegroundColor = MenuFrameColor;
            int left = (Console.WindowWidth - MenuFrameWidth) / 2;
            int top = MenuStartY;
            int bottom = top + MenuHeight - 1;

            // Top border
            Console.SetCursorPosition(left, top);
            Console.Write("╔" + new string('═', MenuFrameWidth - 2) + "╗");

            // Middle
            for (int y = top + 1; y < bottom; y++)
            {
                Console.SetCursorPosition(left, y);
                Console.Write("║" + new string(' ', MenuFrameWidth - 2) + "║");
            }

            // Bottom border
            Console.SetCursorPosition(left, bottom);
            Console.Write("╚" + new string('═', MenuFrameWidth - 2) + "╝");
            Console.ResetColor();
        }

        /// <summary>
        /// Draws the ASCII art title for the pause menu.
        /// </summary>
        private static void DrawPauseTitle()
        {
            int left = (Console.WindowWidth - MenuFrameWidth) / 2;
            int y = MenuStartY + 1; // Start drawing the title below the top border
            string[] lines = [.. _pauseMenuTitle.Split('\n', '\r').Where(l => !string.IsNullOrWhiteSpace(l))];
            for (int i = 0; i < lines.Length; i++)
            {
                // Wyśrodkowanie tytułu względem ramki
                int titleLeft = left + (MenuFrameWidth - lines[i].Length) / 2;
                Console.SetCursorPosition(titleLeft, y + i);
                Console.ForegroundColor = PauseMenuTitleColor;
                Console.Write(lines[i]);
            }
            Console.ResetColor();
        }

        /// <summary>
        /// Draws the selectable options for the pause menu, highlighting the current selection.
        /// </summary>
        /// <param name="selection">The index of the currently selected option.</param>
        private static void DrawPauseOptions(int selection)
        {
            int optionTextWidth = PauseMenuOptions.Max(option => option.Length);
            int optionBoxWidth = optionTextWidth + 4; // 4 spaces for padding (2 on each side)

            int left = (Console.WindowWidth - MenuFrameWidth) / 2;
            int frameCenter = left + MenuFrameWidth / 2;
            int boxLeft = frameCenter - optionBoxWidth / 2;

            for (int i = 0; i < PauseMenuOptions.Length; i++)
            {
                int y = MenuOptionStartY + i * 4; // 4 lines for each option (3 lines + 1 empty line)
                DrawOptionBox(boxLeft, y, PauseMenuOptions[i], i == selection, optionTextWidth);
            }
        }

        /// <summary>
        /// Draws a single option box for the menu.
        /// </summary>
        /// <param name="x">X-coordinate for the box.</param>
        /// <param name="y">Y-coordinate for the box.</param>
        /// <param name="text">Text to display inside the box.</param>
        /// <param name="selected">Whether this option is currently selected.</param>
        /// <param name="optionTextWidth">Width of the option text (for centering).</param>
        private static void DrawOptionBox(int x, int y, string text, bool selected, int optionTextWidth)
        {
            ConsoleColor color = selected ? MenuSelectedColor : MenuDefaultColor;
            string centeredText = CenterText(text, optionTextWidth);

            Console.ForegroundColor = color;
            // Top
            Console.SetCursorPosition(x, y);
            Console.Write("┏" + new string('┅', optionTextWidth + 4) + "┓"); // 4 spaces for padding (2 on each side)
            // Middle
            Console.SetCursorPosition(x, y + 1);
            Console.Write("┃  " + centeredText + "  ┃");
            // Bottom
            Console.SetCursorPosition(x, y + 2);
            Console.Write("┗" + new string('┅', optionTextWidth + 4) + "┛"); // 4 spaces for padding (2 on each side)
            Console.ResetColor();
        }

        /// <summary>
        /// Updates the current selection index based on the pressed key.
        /// </summary>
        /// <param name="key">The key pressed by the user.</param>
        /// <param name="currentSelection">The current selection index.</param>
        /// <returns>The new selection index.</returns>
        private static int UpdateSelection(ConsoleKey key, int currentSelection)
        {
            return key switch
            {
                ConsoleKey.UpArrow => (currentSelection - 1 + PauseMenuOptions.Length) % PauseMenuOptions.Length,
                ConsoleKey.DownArrow => (currentSelection + 1) % PauseMenuOptions.Length,
                _ => currentSelection
            };
        }

        /// <summary>
        /// Centers the given text within a specified width.
        /// </summary>
        /// <param name="text">The text to center.</param>
        /// <param name="width">The width to center within.</param>
        /// <returns>The centered text string.</returns>
        private static string CenterText(string text, int width)
        {
            if (text.Length >= width)
                return text[..width];
            int leftPadding = (width - text.Length) / 2;
            int rightPadding = width - text.Length - leftPadding;
            return new string(' ', leftPadding) + text + new string(' ', rightPadding);
        }

        /// <summary>
        /// Draws the options menu, including the frame, title, and option boxes.
        /// </summary>
        /// <param name="selection">The index of the currently selected option.</param>
        /// <param name="music">Whether music is enabled.</param>
        /// <param name="sfx">Whether sound effects are enabled.</param>
        private static void DrawOptionsMenu(int selection, bool music, bool sfx)
        {
            DrawMenuFrame();
            DrawOptionsTitle();
            DrawOptionsOptions(selection, music, sfx);
        }

        /// <summary>
        /// Draws the ASCII art title for the options menu.
        /// </summary>
        private static void DrawOptionsTitle()
        {
            int left = (Console.WindowWidth - MenuFrameWidth) / 2;
            int y = MenuStartY + 1;
            string[] lines = [.. _optionsMenuTitle.Split('\n', '\r').Where(l => !string.IsNullOrWhiteSpace(l))];
            for (int i = 0; i < lines.Length; i++)
            {
                int titleLeft = left + (MenuFrameWidth - lines[i].Length) / 2;
                Console.SetCursorPosition(titleLeft, y + i);
                Console.ForegroundColor = OptionsMenuTitleColor;
                Console.Write(lines[i]);
            }
            Console.ResetColor();
        }

        /// <summary>
        /// Draws the selectable options for the options menu, highlighting the current selection and showing icons.
        /// </summary>
        /// <param name="selection">The index of the currently selected option.</param>
        /// <param name="music">Whether music is enabled.</param>
        /// <param name="sfx">Whether sound effects are enabled.</param>
        private static void DrawOptionsOptions(int selection, bool music, bool sfx)
        {
            // Compose option texts with icons
            string[] displayOptions =
            [
                "Music          " + (music ? SoundsOnIcon : SoundsOffIcon),
                "Sound effects  " + (sfx ? SoundsOnIcon : SoundsOffIcon),
                "     < Back      ",
            ];
            int optionTextWidth = displayOptions.Max(option => option.Length);
            int optionBoxWidth = optionTextWidth + 4;

            int left = (Console.WindowWidth - MenuFrameWidth) / 2;
            int frameCenter = left + MenuFrameWidth / 2;
            int boxLeft = frameCenter - optionBoxWidth / 2;

            for (int i = 0; i < displayOptions.Length; i++)
            {
                int y = MenuOptionStartY + i * 4;
                DrawOptionBox(boxLeft, y, displayOptions[i], i == selection, optionTextWidth);
            }
        }

        /// <summary>
        /// Updates the current selection index for the options menu based on the pressed key and menu length.
        /// </summary>
        /// <param name="key">The key pressed by the user.</param>
        /// <param name="currentSelection">The current selection index.</param>
        /// <param name="optionsLength">The number of options in the menu.</param>
        /// <returns>The new selection index.</returns>
        private static int UpdateSelection(ConsoleKey key, int currentSelection, int optionsLength)
        {
            return key switch
            {
                ConsoleKey.UpArrow => (currentSelection - 1 + optionsLength) % optionsLength,
                ConsoleKey.DownArrow => (currentSelection + 1) % optionsLength,
                _ => currentSelection
            };
        }

        /// <summary>
        /// Shows the options menu, allowing the user to toggle music and sound effects.
        /// Waits for the user to select '< Back' to return.
        /// </summary>
        /// <param name="musicEnabled">Reference to the music enabled flag (will be updated).</param>
        /// <param name="soundsFXEnabled">Reference to the sound effects enabled flag (will be updated).</param>
        public static void ShowOptionsMenuAndWait(ref bool musicEnabled, ref bool soundsFXEnabled)
        {
            int selection = 0;
            ConsoleKey key;

            DrawOptionsMenu(selection, musicEnabled, soundsFXEnabled);

            do
            {
                key = Helpers.ReadKey();
                int newSelection = UpdateSelection(key, selection, 3);
                if (newSelection != selection)
                {
                    selection = newSelection;
                    DrawOptionsMenu(selection, musicEnabled, soundsFXEnabled);
                    SoundsManager.SoundFX.PlaySound(Operation);
                }
                else if (key == ConsoleKey.Enter)
                {
                    switch (selection)
                    {
                        case 0:
                            musicEnabled = !musicEnabled;
                            SoundsManager.Music.IsMusicEnabled = musicEnabled;
                            DrawOptionsMenu(selection, musicEnabled, soundsFXEnabled);
                            SoundsManager.SoundFX.PlaySound(Operation);
                            break;
                        case 1:
                            soundsFXEnabled = !soundsFXEnabled;
                            SoundsManager.SoundFX.IsSoundFXEnabled = soundsFXEnabled;
                            DrawOptionsMenu(selection, musicEnabled, soundsFXEnabled);
                            SoundsManager.SoundFX.PlaySound(Operation);
                            break;
                        case 2:
                            SoundsManager.SoundFX.PlaySound(Operation);
                            return;
                    }
                }
            }
            while (true);
        }
    }
}