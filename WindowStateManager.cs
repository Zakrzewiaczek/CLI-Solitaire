namespace Solitaire
{
    /// <summary>
    /// Manages the state of the console window, including minimum size requirements and window size change notifications.
    /// </summary>
    public static class WindowStateManager
    {
        /// <summary>
        /// Minimum required width of the console window.
        /// </summary>
        private static readonly int _minimumWidth = 209;
        /// <summary>
        /// Minimum required height of the console window.
        /// </summary>
        private static readonly int _minimumHeight = 62;
        /// <summary>
        /// Thread for watching window size changes.
        /// </summary>
        private static Thread? _windowWatcherThread;
        /// <summary>
        /// Indicates if the window watcher is running.
        /// </summary>
        private static bool _watcherRunning = false;

        /// <summary>
        /// Gets the minimum required width of the console window.
        /// </summary>
        public static int MinimumWidth => _minimumWidth;
        /// <summary>
        /// Gets the minimum required height of the console window.
        /// </summary>
        public static int MinimumHeight => _minimumHeight;

        /// <summary>
        /// Indicates whether the window currently meets the minimum size requirements.
        /// </summary>
        public static bool IsWindowAvailable { get; private set; } = true;

        /// <summary>
        /// Checks if the current window size meets the minimum requirements.
        /// </summary>
        /// <returns>True if the window is large enough, false otherwise.</returns>
        public static bool HasWindowMinimumSize()
        {
            return Console.WindowWidth >= _minimumWidth && Console.WindowHeight >= _minimumHeight;
        }

        /// <summary>
        /// Starts a background watcher that calls the provided callback when the window size changes.
        /// </summary>
        /// <param name="onWindowSizeChanged">Callback to invoke when the window size changes or the window becomes available/unavailable.</param>
        public static void StartWindowSizeWatcher(Action onWindowSizeChanged)
        {
            if (_windowWatcherThread != null && _windowWatcherThread.IsAlive)
                return;

            _watcherRunning = true;
            int lastWidth = Console.WindowWidth;
            int lastHeight = Console.WindowHeight;
            IsWindowAvailable = HasWindowMinimumSize();

            _windowWatcherThread = new Thread(() =>
            {
                while (_watcherRunning)
                {
                    int width = Console.WindowWidth;
                    int height = Console.WindowHeight;
                    bool available = width >= _minimumWidth && height >= _minimumHeight;
                    if (width != lastWidth || height != lastHeight || available != IsWindowAvailable)
                    {
                        lastWidth = width;
                        lastHeight = height;
                        IsWindowAvailable = available;
                        onWindowSizeChanged?.Invoke();
                    }
                    Thread.Sleep(100); // Polling interval
                }
            })
            {
                IsBackground = true
            };
            _windowWatcherThread.Start();
        }

        /// <summary>
        /// Stops the window size watcher thread.
        /// </summary>
        public static void StopWindowSizeWatcher()
        {
            _watcherRunning = false;
            _windowWatcherThread = null;
        }
    }
}