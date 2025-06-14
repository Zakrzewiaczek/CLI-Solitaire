namespace Solitaire
{
    public static class Victory
    {
        /// <summary>
        /// Maximum X coordinate for the victory animation.
        /// </summary>
        private static readonly int _animationMaxX = 187;
        /// <summary>
        /// Maximum Y coordinate for the victory animation.
        /// </summary>
        private static readonly int _animationMaxY = 41;

        public static void ShowAnimation()
        {
            SoundsManager.SoundFX.PlaySound(SoundsManager.SoundFX.SoundType.Victory);

            Random random = new(Guid.NewGuid().GetHashCode());

            int animationDurationMs = 6750; // total animation time in ms
            int minDelay = 5;    // ms, fastest at the start
            int maxDelay = 1000;  // ms, slowest at the end
            double slowPhaseRatio = 0.25;

            int elapsed = 0;
            int cardCount = 0;
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();

            while (elapsed < animationDurationMs)
            {
                double t = (double)elapsed / animationDurationMs;

                // Ease out: slow only at the very end
                double delayT = t < (1.0 - slowPhaseRatio)
                    ? t / (1.0 - slowPhaseRatio) * 0.5 // fast phase, stays low
                    : 0.5 + (t - (1.0 - slowPhaseRatio)) / slowPhaseRatio * 0.5; // slow phase, ramps up

                int delay = (int)(minDelay + (maxDelay - minDelay) * Math.Pow(delayT, 2));

                int randomX = random.Next(0, _animationMaxX);
                int randomY = random.Next(0, _animationMaxY);

                var rankValues = Enum.GetValues<Rank>();
                Rank randomRank = rankValues[random.Next(rankValues.Length)];
                var suitValues = Enum.GetValues<Suit>();
                Suit randomSuit = suitValues[random.Next(suitValues.Length)];

                Card randomCard = new(randomRank, randomSuit, true);

                Renderer.Cards.DrawCard(randomCard, randomX, randomY, null);

                Thread.Sleep(delay);
                elapsed = (int)stopwatch.ElapsedMilliseconds;
                cardCount++;
            }
        }

        // --- Constans for info box ---

        // --- Layout constants for info box ---

        /// <summary>
        /// Delay in milliseconds between info box animation steps.
        /// </summary>
        private const int InfoAnimationDelayMs = 500;
        /// <summary>
        /// Total duration in milliseconds for the info box animation.
        /// </summary>
        private const int InfoAnimationTotalDurationMs = 1900;
        /// <summary>
        /// Delay in milliseconds for each step of the info box animation.
        /// </summary>
        private const int InfoAnimationStepDelayMs = 25;
        /// <summary>
        /// Text displayed on the continue button in the info box.
        /// </summary>
        private const string ContinueButtonText = "Continue";
        /// <summary>
        /// Color of the info box title.
        /// </summary>
        private const ConsoleColor InfoTitleColor = ConsoleColor.DarkYellow;
        /// <summary>
        /// ASCII art for the info box title.
        /// </summary>
        private const string InfoTitle =
@"          ___                             
         / __| __ _  _ __   ___           
        | (_ |/ _` || '  \ / -_)          
         \___|\__,_||_|_|_|\___|          
 ___                                      
/ __| _  _  _ __   _ __   __ _  _ _  _  _ 
\__ \| || || '  \ | '  \ / _` || '_|| || |
|___/ \_,_||_|_|_||_|_|_|\__,_||_|   \_, |
                                     |__/ ";
        /// <summary>
        /// Width of the info frame in the victory screen.
        /// </summary>
        private const int InfoFrameWidth = 66;
        /// <summary>
        /// Height of the info frame in the victory screen.
        /// </summary>
        private const int InfoFrameHeight = 25;
        /// <summary>
        /// Y position where the info frame starts.
        /// </summary>
        private const int InfoFrameStartY = 15;
        /// <summary>
        /// Y offset for the info title.
        /// </summary>
        private const int InfoTitleOffsetY = 2;
        /// <summary>
        /// Y position where the stats start in the info frame.
        /// </summary>
        private const int InfoStatsStartY = 12;
        /// <summary>
        /// Spacing between stats in the info frame.
        /// </summary>
        private const int InfoStatsSpacingY = 2;
        /// <summary>
        /// Y offset for the score in the info frame.
        /// </summary>
        private const int ScoreOffsetY = 3;
        /// <summary>
        /// Margin for the info frame.
        /// </summary>
        private const int InfoFrameMargin = 1;


        public static void ShowInfoAboutGame(Difficulty difficulty, TimeSpan time, uint movesCount)
        {
            int score = 1000;
            int divider = difficulty switch
            {
                Difficulty.Easy => 1,
                Difficulty.Hard => 2,
                _ => 1
            };
            score -= ((int)time.TotalSeconds + (int)movesCount) / divider;
            if (score < 0)
                score = 0;

            // Precompute positions
            int left = (Console.WindowWidth - InfoFrameWidth) / 2;
            int top = InfoFrameStartY;
            int marginLeft = left - (InfoFrameMargin + 2);
            int marginTop = top - InfoFrameMargin;
            int marginWidth = InfoFrameWidth + 2 * (InfoFrameMargin + 2);
            int marginHeight = InfoFrameHeight + 2 * InfoFrameMargin;
            int statsX = left + 2;
            int movesY = top + InfoStatsStartY;
            int timeY = movesY + InfoStatsSpacingY;
            int scoreY = timeY + ScoreOffsetY;

            DrawMargin(marginLeft, marginTop, marginWidth, marginHeight);

            DrawInfoFrame(left, top);

            // Draw multi-line title (centered)
            string[] titleLines = [.. InfoTitle.Split('\n', '\r').Where(l => !string.IsNullOrWhiteSpace(l))];
            for (int i = 0; i < titleLines.Length; i++)
            {
                string line = titleLines[i];
                int titleLeft = left + (InfoFrameWidth - line.Length) / 2;
                Console.SetCursorPosition(titleLeft, top + InfoTitleOffsetY + i);
                Console.ForegroundColor = InfoTitleColor;
                Console.Write(line);
            }
            Console.ResetColor();

            int steps = Math.Max(1, InfoAnimationTotalDurationMs / InfoAnimationStepDelayMs);

            bool skip = false;
            Helpers.FlushConsoleBuffer();

            // Animate Moves
            for (int i = 1; i <= steps; i++)
            {
                if (Console.KeyAvailable && Console.ReadKey(true).Key == ConsoleKey.Enter)
                {
                    skip = true;
                    break;
                }
                int displayedMoves = (int)Math.Round(movesCount * i / (double)steps);
                DrawStat(statsX, movesY, "Moves:", displayedMoves, InfoFrameWidth);
                SoundsManager.SoundFX.PlaySound(SoundsManager.SoundFX.SoundType.PointsSound);
                Thread.Sleep(InfoAnimationStepDelayMs);
            }
            DrawStat(statsX, movesY, "Moves:", movesCount, InfoFrameWidth);

            if (!skip)
                Thread.Sleep(InfoAnimationDelayMs);
            skip = false; // Reset skip for next animation
            Helpers.FlushConsoleBuffer();

            // Animate Time
            int totalSeconds = (int)time.TotalSeconds;
            if (!skip)
            {
                for (int i = 1; i <= steps; i++)
                {
                    if (Console.KeyAvailable && Console.ReadKey(true).Key == ConsoleKey.Enter)
                    {
                        skip = true;
                        break;
                    }
                    int displayedSeconds = (int)Math.Round(totalSeconds * i / (double)steps);
                    DrawStat(statsX, timeY, "Time:", FormatTime(displayedSeconds), InfoFrameWidth);
                    SoundsManager.SoundFX.PlaySound(SoundsManager.SoundFX.SoundType.PointsSound);
                    Thread.Sleep(InfoAnimationStepDelayMs);
                }
            }
            DrawStat(statsX, timeY, "Time:", FormatTime(time.TotalSeconds), InfoFrameWidth);

            if (!skip)
                Thread.Sleep(InfoAnimationDelayMs);
            skip = false;
            Helpers.FlushConsoleBuffer(); // Reset skip for next animation

            // Animate Score
            if (!skip)
            {
                for (int i = 1; i <= steps; i++)
                {
                    if (Console.KeyAvailable && Console.ReadKey(true).Key == ConsoleKey.Enter)
                    {
                        skip = true;
                        break;
                    }
                    int displayedScore = (int)Math.Round(score * i / (double)steps);
                    DrawStat(statsX, scoreY, "Total score:", displayedScore, InfoFrameWidth, ConsoleColor.DarkYellow);
                    if (score > 0)
                        SoundsManager.SoundFX.PlaySound(SoundsManager.SoundFX.SoundType.PointsSound);
                    Thread.Sleep(InfoAnimationStepDelayMs);
                }
            }
            DrawStat(statsX, scoreY, "Total score:", score, InfoFrameWidth, ConsoleColor.DarkYellow);

            if (!skip)
                Thread.Sleep(InfoAnimationDelayMs);

            // Draw Continue button (framed, centered, like in EscapeMenu)
            string btn = ContinueButtonText;
            int btnBoxWidth = btn.Length + 6;
            int btnLeft = left + (InfoFrameWidth - btnBoxWidth) / 2;
            int btnY = scoreY + 3;
            DrawButtonBox(btnLeft, btnY, btn);
            Thread.Sleep(300); // Prevent from enter spam (to skip animations)

            // Wait for Enter
            while (Console.ReadKey(true).Key != ConsoleKey.Enter) { }
        }

        private static void DrawMargin(int left, int top, int width, int height)
        {
            var prevBg = Console.BackgroundColor;
            Console.BackgroundColor = ConsoleColor.Black; // kolor tła konsoli
            for (int y = 0; y < height; y++)
            {
                Console.SetCursorPosition(left, top + y);
                Console.Write(new string(' ', width));
            }
            Console.BackgroundColor = prevBg;
        }

        private static void DrawInfoFrame(int left, int top)
        {
            Console.ForegroundColor = ConsoleColor.Gray;
            // Top border
            Console.SetCursorPosition(left, top);
            Console.Write("╔" + new string('═', InfoFrameWidth - 2) + "╗");
            // Middle
            for (int y = 1; y < InfoFrameHeight - 1; y++)
            {
                Console.SetCursorPosition(left, top + y);
                Console.Write("║" + new string(' ', InfoFrameWidth - 2) + "║");
            }
            // Bottom border
            Console.SetCursorPosition(left, top + InfoFrameHeight - 1);
            Console.Write("╚" + new string('═', InfoFrameWidth - 2) + "╝");
            Console.ResetColor();
        }

        private static void DrawStat(int left, int y, string label, object value, int frameWidth, ConsoleColor? color = null)
        {
            string text = $"{label} {value}";
            int statLeft = left + (frameWidth - 4 - text.Length) / 2; // 2 for border, 2 for padding
            Console.SetCursorPosition(statLeft, y);
            Console.ForegroundColor = color ?? ConsoleColor.White;
            Console.Write(text); // <-- nie padujemy spacjami!
            Console.ResetColor();
        }

        private static void DrawButtonBox(int x, int y, string text)
        {
            int width = text.Length + 6;
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            // Top
            Console.SetCursorPosition(x, y);
            Console.Write("┏" + new string('┅', width - 2) + "┓");
            // Middle
            Console.SetCursorPosition(x, y + 1);
            Console.Write("┃  " + text + "  ┃");
            // Bottom
            Console.SetCursorPosition(x, y + 2);
            Console.Write("┗" + new string('┅', width - 2) + "┛");
            Console.ResetColor();
        }

        private static string FormatTime(double totalSeconds)
        {
            int sec = (int)totalSeconds % 60;
            int min = (int)totalSeconds / 60;
            return $"{min:D2}:{sec:D2}";
        }
    }
}