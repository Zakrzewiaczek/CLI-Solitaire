using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Solitaire
{
    /// <summary>
    /// Represents the four suits in a standard deck of cards.
    /// </summary>
    public enum Suit
    {
        Spades,
        Clubs,
        Hearts,
        Diamonds
    }

    /// <summary>
    /// Extension methods for the <see cref="Suit"/> enum.
    /// </summary>
    public static class SuitExtensions
    {
        /// <summary>
        /// Gets the Unicode symbol for the suit.
        /// </summary>
        /// <param name="suit">The suit to get the symbol for.</param>
        /// <returns>The Unicode symbol for the suit.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if the suit is not recognized.</exception>
        public static string ToSymbol(this Suit suit)
        {
            return suit switch
            {
                Suit.Hearts => "♥",
                Suit.Diamonds => "♦",
                Suit.Clubs => "♣",
                Suit.Spades => "♠",
                _ => throw new ArgumentOutOfRangeException(nameof(suit), suit, null)
            };
        }

        /// <summary>
        /// Determines if the specified suit is red (Hearts or Diamonds).
        /// </summary>
        /// <param name="suit">The suit to evaluate.</param>
        /// <returns>True if the suit is Hearts or Diamonds; otherwise, false.</returns>
        public static bool IsRed(this Suit suit)
        {
            return suit == Suit.Hearts || suit == Suit.Diamonds;
        }
    }

    /// <summary>
    /// Represents the ranks in a standard deck of cards.
    /// </summary>
    public enum Rank
    {
        Two = 2,
        Three,
        Four,
        Five,
        Six,
        Seven,
        Eight,
        Nine,
        Ten,
        Jack,
        Queen,
        King,
        Ace
    }

    /// <summary>
    /// Extension methods for the <see cref="Rank"/> enum.
    /// </summary>
    public static class RankExtensions
    {
        /// <summary>
        /// Gets the symbol for the rank.
        /// </summary>
        /// <param name="rank">The rank to get the symbol for.</param>
        /// <returns>The symbol for the rank.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if the rank is not recognized.</exception>
        public static string ToSymbol(this Rank rank)
        {
            return rank switch
            {
                Rank.Two => "2",
                Rank.Three => "3",
                Rank.Four => "4",
                Rank.Five => "5",
                Rank.Six => "6",
                Rank.Seven => "7",
                Rank.Eight => "8",
                Rank.Nine => "9",
                Rank.Ten => "10",
                Rank.Jack => "J",
                Rank.Queen => "Q",
                Rank.King => "K",
                Rank.Ace => "A",
                _ => throw new ArgumentOutOfRangeException(nameof(rank), rank, null)
            };
        }
    }

    /// <summary>
    /// Represents a playing card, which may be a standard card or a custom ASCII-art card (e.g., for empty slots or suit symbols).
    /// </summary>
    public class Card
    {
        /// <summary>
        /// The suit of the card.
        /// </summary>
        private readonly Suit _suit;
        /// <summary>
        /// The rank of the card.
        /// </summary>
        private readonly Rank _rank;
        /// <summary>
        /// Indicates if the card has a suit.
        /// </summary>
        private readonly bool _hasSuit;
        /// <summary>
        /// Indicates if the card has a rank.
        /// </summary>
        private readonly bool _hasRank;
        /// <summary>
        /// Indicates whether the card is face up.
        /// </summary>
        private bool _isFaceUp;
        /// <summary>
        /// The ASCII representation of the card (if custom or face up).
        /// </summary>
        private readonly string? _AsciiCardRepresentation = null;
        /// <summary>
        /// Gets the suit of the card.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown if the card does not have a suit.</exception>
        public Suit Suit
        {
            get
            {
                if (!_hasSuit)
                    throw new InvalidOperationException("This card does not have a suit.");
                return _suit;
            }
        }

        /// <summary>
        /// Gets the rank of the card.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown if the card does not have a rank.</exception>
        public Rank Rank
        {
            get
            {
                if (!_hasRank)
                    throw new InvalidOperationException("This card does not have a rank.");
                return _rank;
            }
        }

        /// <summary>
        /// Gets or sets whether the card is face up.
        /// </summary>
        public bool IsFaceUp
        {
            get => _isFaceUp;
            set => _isFaceUp = value;
        }

        /// <summary>
        /// Gets the ASCII representation of the card if face up, or the card back if face down.
        /// </summary>
        public string AsciiCardRepresentation => _isFaceUp ? _AsciiCardRepresentation! : AsciiGraphics.GetCardBack();

        /// <summary>
        /// Initializes a new instance of the <see cref="Card"/> class with the specified rank, suit, and face-up status.
        /// </summary>
        /// <param name="rank">The rank of the card.</param>
        /// <param name="suit">The suit of the card.</param>
        /// <param name="isFaceUp">Indicates whether the card is face up.</param>
        public Card(Rank rank, Suit suit, bool isFaceUp)
        {
            _rank = rank;
            _suit = suit;
            _hasRank = true;
            _hasSuit = true;
            _isFaceUp = isFaceUp;
            _AsciiCardRepresentation = AsciiGraphics.GetCard(this);
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="Card"/> class with a custom ASCII representation, optional rank, suit, and face-up status.
        /// </summary>
        /// <param name="customCardIcon">The custom ASCII representation of the card.</param>
        /// <param name="rank">The rank of the card (optional).</param>
        /// <param name="suit">The suit of the card (optional).</param>
        /// <param name="isFaceUp">Indicates whether the card is face up.</param>
        public Card(string customCardIcon, Rank? rank = null, Suit? suit = null, bool isFaceUp = true)
        {
            _hasRank = rank is not null;
            _rank = rank ?? Rank.Two; // Default to Two if rank is not provided
            _hasSuit = suit is not null;
            _suit = suit ?? Suit.Spades; // Default to Spades if suit is not provided
            _isFaceUp = isFaceUp;
            _AsciiCardRepresentation = customCardIcon;
        }

        /// <summary>
        /// Returns a string representation of the card (rank and suit, or custom icon).
        /// </summary>
        /// <returns>String representation of the card.</returns>
        public override string ToString()
        {
            if (_hasRank && _hasSuit)
                return $"{Rank.ToSymbol()}{Suit.ToSymbol()}";
            return _AsciiCardRepresentation ?? base.ToString() ?? string.Empty;
        }

        /// <summary>
        /// Checks if two cards are equal (by rank and suit, or by custom icon).
        /// </summary>
        /// <param name="left">First card.</param>
        /// <param name="right">Second card.</param>
        /// <returns>True if the cards are equal.</returns>
        public static bool operator ==(Card? left, Card? right)
        {
            if (ReferenceEquals(left, right)) return true;
            if (left is null || right is null) return false;
            if (!left._hasRank || !left._hasSuit || !right._hasRank || !right._hasSuit)
                return left._AsciiCardRepresentation == right._AsciiCardRepresentation;
            return left.Rank == right.Rank && left.Suit == right.Suit;
        }
        /// <summary>
        /// Checks if two cards are not equal.
        /// </summary>
        /// <param name="left">First card.</param>
        /// <param name="right">Second card.</param>
        /// <returns>True if the cards are not equal.</returns>
        public static bool operator !=(Card? left, Card? right)
        {
            return !(left == right);
        }

        /// <summary>
        /// Determines whether this card is equal to another object.
        /// </summary>
        /// <param name="obj">The object to compare with.</param>
        /// <returns>True if the objects are equal.</returns>
        public override bool Equals(object? obj)
        {
            return this == obj as Card;
        }
        /// <summary>
        /// Gets the hash code for the card.
        /// </summary>
        /// <returns>The hash code.</returns>
        public override int GetHashCode()
        {
            if (_hasRank && _hasSuit)
                return HashCode.Combine(Rank, Suit);
            return _AsciiCardRepresentation?.GetHashCode() ?? 0;
        }
    }
}
