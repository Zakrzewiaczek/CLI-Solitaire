namespace Solitaire
{
    /// <summary>
    /// Provides methods to initialize and retrieve ASCII graphics for cards.
    /// </summary>
    public static class AsciiGraphics
    {
        /// <summary>
        /// Stores the ASCII representations of cards. Each entry represents a single card's ASCII art.
        /// </summary>
        private static readonly List<string> _cards = [];

        /// <summary>
        /// Number of lines representing a single card in the ASCII art file.
        /// </summary>
        private const int LinesPerCard = 16;

        /// <summary>
        /// Static constructor to initialize card graphics from the 'cards.txt' file.
        /// Loads all ASCII art representations from the file and populates the internal card list.
        /// </summary>
        /// <exception cref="FileNotFoundException">Thrown if the 'cards.txt' file is not found.</exception>
        /// <exception cref="IOException">Thrown if there is an error reading the file.</exception>
        static AsciiGraphics()
        {
            // Read all lines from the ASCII card art file.
            string[] allLines = File.ReadAllLines(@"cards.txt");
            int cardCount = allLines.Length / LinesPerCard;

            // Parse each card's ASCII art and add to the list.
            for (int cardIndex = 0; cardIndex < cardCount; cardIndex++)
            {
                int startIndex = cardIndex * LinesPerCard;
                // Join the lines representing a single card.
                string cardRepresentation = string.Join(Environment.NewLine, allLines[startIndex..(startIndex + LinesPerCard)]);
                _cards.Add(cardRepresentation);
            }
        }

        /// <summary>
        /// Retrieves the ASCII representation for a specific card.
        /// </summary>
        /// <param name="card">The card for which the ASCII graphic is needed.</param>
        /// <returns>The ASCII representation of the card.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if the card index is out of range.</exception>
        public static string GetCard(Card card)
        {
            // Calculate index based on suit and rank (cards start at 2 and each suit has 13 cards).
            int rankValue = (int)card.Rank;
            int suitValue = (int)card.Suit;
            int index = suitValue * 13 + (rankValue - 2) + 6; // 6 is the offset for back, empty, and symbol cards

            if (index < 0 || index >= _cards.Count)
                throw new ArgumentOutOfRangeException(nameof(card), "Card index is out of range for ASCII graphics.");
            return _cards[index];
        }

        /// <summary>
        /// Retrieves the ASCII representation for the back of a card.
        /// </summary>
        /// <returns>The ASCII representation of a card back.</returns>
        public static string GetCardBack()
        {
            return _cards[0];
        }

        /// <summary>
        /// Retrieves the ASCII representation for an empty card slot.
        /// </summary>
        /// <returns>The ASCII representation of an empty card.</returns>
        public static string GetEmptyCard()
        {
            return _cards[1];
        }

        /// <summary>
        /// Retrieves the ASCII representation for a symbol card (e.g., foundation pile marker for a suit).
        /// </summary>
        /// <param name="suit">The suit for which the symbol card is needed.</param>
        /// <returns>The ASCII representation of the symbol card for the given suit.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if the suit index is out of range.</exception>
        public static string GetSymbolCard(Suit suit)
        {
            int index = (int)suit + 2; // +2 because 0 is back and 1 is empty card
            if (index < 2 || index >= _cards.Count)
                throw new ArgumentOutOfRangeException(nameof(suit), "Suit index is out of range for symbol card.");
            return _cards[index];
        }
    }
}
