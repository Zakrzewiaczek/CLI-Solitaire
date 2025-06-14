using System.Runtime.InteropServices;
using System.Threading;

namespace Solitaire
{
    /// <summary>
    /// Provides helper methods for console input and buffer management.
    /// </summary>
    public static class Helpers
    {
        /// <summary>
        /// Helper method for clearing the console buffer by removing any available key presses.
        /// </summary>
        public static void FlushConsoleBuffer()
        {
            while (Console.KeyAvailable)
                _ = Console.ReadKey(true);
        }

        /// <summary>
        /// Reads a single key from the console after flushing the buffer.
        /// Adds a small delay to prevent flickering and key spamming when a key is held down.
        /// </summary>
        /// <returns>The <see cref="ConsoleKey"/> value of the pressed key.</returns>
        public static ConsoleKey ReadKey()
        {
            FlushConsoleBuffer();
            ConsoleKey key = Console.ReadKey(true).Key;
            Thread.Sleep(5); // Add a small delay to prevent flickering (keys spam when key is held down).

            return key;
        }
    }
}
