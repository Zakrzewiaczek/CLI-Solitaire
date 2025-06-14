using System;
using System.Collections.Generic;
using System.Linq;

namespace Solitaire
{
    /// <summary>
    /// Provides static methods for generating Solitaire board layouts for different difficulty levels.
    /// </summary>
    public static class BoardGenerator
    {
        /// <summary>
        /// Random number generator used for shuffling cards.
        /// </summary>
        private readonly static Random _random;

        /// <summary>
        /// Static constructor. Initializes the random number generator with a unique seed.
        /// </summary>
        static BoardGenerator()
        {
            _random = new Random(Guid.NewGuid().GetHashCode());
        }

        /// <summary>
        /// Generates the Solitaire board configuration (tableau and stock) for the given difficulty.
        /// </summary>
        /// <param name="difficulty">The difficulty level (Easy or Hard).</param>
        /// <returns>
        /// A tuple containing:
        /// <list type="bullet">
        /// <item><description><c>Tableau</c>: List of 7 columns, each a list of <see cref="Card"/> objects.</description></item>
        /// <item><description><c>Stock</c>: List of remaining <see cref="Card"/> objects (the stock pile).</description></item>
        /// </list>
        /// </returns>
        public static (List<List<Card>> Tableau, List<Card> Stock) Generate(Difficulty difficulty)
        {
            List<Card> deck = GenerateStandardDeck();
            Shuffle(deck);

            return (difficulty == Difficulty.Hard)
                ? GenerateHard(deck)
                : GenerateEasy();
        }

        /// <summary>
        /// Generates the board layout for Hard difficulty (fully random deck, classic Solitaire deal).
        /// </summary>
        /// <param name="deck">A shuffled standard deck of cards.</param>
        /// <returns>
        /// A tuple containing:
        /// <list type="bullet">
        /// <item><description><c>Tableau</c>: List of 7 columns, each a list of <see cref="Card"/> objects.</description></item>
        /// <item><description><c>Stock</c>: List of remaining <see cref="Card"/> objects (the stock pile).</description></item>
        /// </list>
        /// </returns>
        private static (List<List<Card>> Tableau, List<Card> Stock) GenerateHard(List<Card> deck)
        {
            List<List<Card>> tableau = [];
            int deckIndex = 0;

            for (int i = 0; i < 7; i++)
            {
                List<Card> column = [];
                for (int j = 0; j <= i; j++)
                {
                    Card card = deck[deckIndex++];
                    card.IsFaceUp = j == i; // Only the last card in the column is face up
                    column.Add(card);
                }
                tableau.Add(column);
            }

            List<Card> stock = [.. deck.Skip(deckIndex)];
            stock.ForEach(card => card.IsFaceUp = false); // All cards in the stock are face down
            return (tableau, stock);
        }

        /// <summary>
        /// Generates the board layout for Easy difficulty (well shuffled, but not fully random; easier cards near the bottom of columns).
        /// </summary>
        /// <remarks>
        /// Cards are grouped by difficulty (easy, medium, hard), shuffled within groups, and distributed so that easier cards are more likely to be face up at the bottom of columns.
        /// </remarks>
        /// <returns>
        /// A tuple containing:
        /// <list type="bullet">
        /// <item><description><c>Tableau</c>: List of 7 columns, each a list of <see cref="Card"/> objects.</description></item>
        /// <item><description><c>Stock</c>: List of remaining <see cref="Card"/> objects (the stock pile).</description></item>
        /// </list>
        /// </returns>
        private static (List<List<Card>> Tableau, List<Card> Stock) GenerateEasy()
        {
            // Prepare lists of cards by difficulty
            List<Card> easyCards = [];
            List<Card> mediumCards = [];
            List<Card> hardCards = [];
            foreach (Suit suit in Enum.GetValues<Suit>())
            {
                foreach (Rank rank in Enum.GetValues<Rank>())
                {
                    if (rank == Rank.Ace || rank == Rank.Two || rank == Rank.Three || rank == Rank.Four || rank == Rank.Five)
                        easyCards.Add(new Card(rank, suit, false));
                    else if (rank == Rank.Six || rank == Rank.Seven || rank == Rank.Eight || rank == Rank.Nine)
                        mediumCards.Add(new Card(rank, suit, false));
                    else
                        hardCards.Add(new Card(rank, suit, false));
                }
            }

            Random rnd = new(Guid.NewGuid().GetHashCode());
            // Shuffle each group separately
            void ShuffleList(List<Card> list)
            {
                for (int i = list.Count - 1; i > 0; i--)
                {
                    int j = rnd.Next(i + 1);
                    (list[i], list[j]) = (list[j], list[i]);
                }
            }
            ShuffleList(easyCards);
            ShuffleList(mediumCards);
            ShuffleList(hardCards);

            // Build tableau: at the bottom of columns (face up) put easy cards, then medium, then hard on top
            List<List<Card>> tableau = [];
            int easyIdx = 0, mediumIdx = 0, hardIdx = 0;
            for (int col = 0; col < 7; col++)
            {
                List<Card> column = [];
                int colHeight = col + 1;
                for (int row = 0; row < colHeight; row++)
                {
                    Card card;
                    // The last card (face up) - prefer easy, then medium
                    if (row == colHeight - 1)
                    {
                        if (easyIdx < easyCards.Count)
                            card = easyCards[easyIdx++];
                        else if (mediumIdx < mediumCards.Count)
                            card = mediumCards[mediumIdx++];
                        else
                            card = hardCards[hardIdx++];
                        card.IsFaceUp = true;
                    }
                    // The second to last - prefer medium, then easy
                    else if (row == colHeight - 2 && colHeight > 1)
                    {
                        if (mediumIdx < mediumCards.Count)
                            card = mediumCards[mediumIdx++];
                        else if (easyIdx < easyCards.Count)
                            card = easyCards[easyIdx++];
                        else
                            card = hardCards[hardIdx++];
                        card.IsFaceUp = false;
                    }
                    // The rest - prefer hard, then medium
                    else
                    {
                        if (hardIdx < hardCards.Count)
                            card = hardCards[hardIdx++];
                        else if (mediumIdx < mediumCards.Count)
                            card = mediumCards[mediumIdx++];
                        else
                            card = easyCards[easyIdx++];
                        card.IsFaceUp = false;
                    }
                    column.Add(card);
                }
                tableau.Add(column);
            }

            // Remaining cards go to the stock pile
            List<Card> stock = [];
            stock.AddRange(easyCards.Skip(easyIdx));
            stock.AddRange(mediumCards.Skip(mediumIdx));
            stock.AddRange(hardCards.Skip(hardIdx));

            // Shuffle the stock pile well (full Fisher-Yates)
            ShuffleList(stock);

            return (tableau, stock);
        }

        /// <summary>
        /// Shuffles the given deck of cards in place using the Fisher-Yates algorithm.
        /// </summary>
        /// <param name="deck">The deck of cards to shuffle (modified in place).</param>
        private static void Shuffle(List<Card> deck)
        {
            for (int i = deck.Count - 1; i > 0; i--)
            {
                int j = _random.Next(i + 1);
                (deck[i], deck[j]) = (deck[j], deck[i]); // Swap cards
            }
        }

        /// <summary>
        /// Generates a standard deck of 52 cards (all suits and ranks, face up by default).
        /// </summary>
        /// <returns>A list containing all cards in a standard deck.</returns>
        private static List<Card> GenerateStandardDeck()
        {
            List<Card> deck = [];
            foreach (Suit suit in Enum.GetValues<Suit>())
            {
                foreach (Rank rank in Enum.GetValues<Rank>())
                {
                    deck.Add(new Card(rank, suit, true));
                }
            }
            return deck;
        }
    }
}
