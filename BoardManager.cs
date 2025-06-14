namespace Solitaire
{
    using static SoundsManager.SoundFX.SoundType;


    /// <summary>
    /// Represents a pointer for navigating the board, with clamped row and column values.
    /// </summary>
    /// <param name="maxRow">Maximum allowed row index.</param>
    public struct Pointer(uint maxRow)
    {
        private int _row;
        private int _col;

        /// <summary>
        /// Gets or sets the row index. Value is clamped between 0 and maxRow.
        /// </summary>
        public int Row
        {
            readonly get => _row;
            set => _row = Math.Clamp(value, 0, (int)maxRow); // There are 8 rows (max -> in the tableau and stock/waste/foundation row)
        }
        /// <summary>
        /// Gets or sets the column index. Value is clamped between 0 and 10.
        /// </summary>
        public int Col
        {
            readonly get => _col;
            set => _col = Math.Clamp(value, 0, 10);
            // There are 11 columns in the tableau (stock, up to 3 waste cards and tableau (foundation piles are included in tableau columns))
        }
    }

    /// <summary>
    /// Represents the direction for pointer movement.
    /// </summary>
    public enum Direction
    {
        Up,
        Down,
        Left,
        Right
    }

    /// <summary>
    /// Manages the state and logic of the Solitaire board, including tableau, foundation, stock, and waste piles.
    /// </summary>
    public class BoardManager
    {
        // --- Constants ---
        /// <summary>
        /// Maximum tableau length (number of columns).
        /// </summary>
        private static readonly uint maxTabelauLength = 10;
        /// <summary>
        /// Order of ranks for sorting and validation.
        /// </summary>
        private static readonly List<Rank> _ranksOrder = [Rank.Ace, Rank.Two, Rank.Three, Rank.Four, Rank.Five, Rank.Six, Rank.Seven, Rank.Eight, Rank.Nine, Rank.Ten, Rank.Jack, Rank.Queen, Rank.King];

        // --- Drawing positions ---
        /// <summary>
        /// X coordinate for the start of the tableau.
        /// </summary>
        private readonly int tabelauStartX = 55;
        /// <summary>
        /// Y coordinate for the tableau.
        /// </summary>
        private readonly int tabelauY = 19;
        /// <summary>
        /// X coordinate for the start of the foundation piles.
        /// </summary>
        private readonly int fountationStartX = 94;
        /// <summary>
        /// Y coordinate for the foundation piles.
        /// </summary>
        private readonly int fountationY = 0;
        /// <summary>
        /// X coordinate for the stock pile.
        /// </summary>
        private readonly int stockX = 3;
        /// <summary>
        /// Y coordinate for the stock pile.
        /// </summary>
        private readonly int stockY = 0;
        /// <summary>
        /// X coordinate for the waste pile.
        /// </summary>
        private readonly int wasteStartX = 25;
        /// <summary>
        /// Y coordinate for the waste pile.
        /// </summary>
        private readonly int wasteY = 0;


        /// <summary>
        /// Random instance (with random seed) for shuffling.
        /// </summary>
        private readonly Random _random = new(Guid.NewGuid().GetHashCode());

        /// <summary>
        /// Tableau piles (columns of cards).
        /// </summary>
        private readonly List<List<Card>> _tabelau;
        /// <summary>
        /// Gets the tableau piles.
        /// </summary>
        public List<List<Card>> Tabelau => _tabelau;

        /// <summary>
        /// Foundation piles (one for each suit).
        /// </summary>
        private readonly List<List<Card>> _foundationPiles;
        /// <summary>
        /// Gets the foundation piles.
        /// </summary>
        public List<List<Card>> FoundationPiles => _foundationPiles;

        /// <summary>
        /// Stock pile (draw pile).
        /// </summary>
        private readonly List<Card> _stockPile;
        /// <summary>
        /// Gets the stock pile.
        /// </summary>
        public List<Card> StockPile => _stockPile;

        /// <summary>
        /// Waste pile (discarded cards).
        /// </summary>
        private readonly List<Card> _wastePile;
        /// <summary>
        /// Gets the waste pile.
        /// </summary>
        public List<Card> WastePile => _wastePile;

        /// <summary>
        /// Current game difficulty.
        /// </summary>
        private readonly Difficulty _difficulty;
        /// <summary>
        /// Gets the current game difficulty.
        /// </summary>
        public Difficulty Difficulty => _difficulty;

        /// <summary>
        /// Pointer for navigation.
        /// </summary>
        private Pointer _pointer = new(maxTabelauLength);
        /// <summary>
        /// Gets the pointer for navigation.
        /// </summary>
        public Pointer Pointer => _pointer;

        /// <summary>
        /// The card currently pointed at (may be null).
        /// </summary>
        private Card? _pointedCard = null;
        /// <summary>
        /// Gets the card currently pointed at (may be null).
        /// </summary>
        public Card? PointedCard => _pointedCard;

        /// <summary>
        /// The currently selected card (may be null).
        /// </summary>
        private Card? _selectedCard = null;
        /// <summary>
        /// Gets the currently selected card (may be null).
        /// </summary>
        public Card? SelectedCard => _selectedCard;

        /// <summary>
        /// Number of moves made in the current game.
        /// </summary>
        private uint _movesCount = 0;
        /// <summary>
        /// Gets the number of moves made in the current game.
        /// </summary>
        public uint MovesCount => _movesCount;

        /// <summary>
        /// Initializes a new instance of the <see cref="BoardManager"/> class.
        /// </summary>
        /// <param name="tabelau">The tableau piles (columns of cards).</param>
        /// <param name="stock">The stock pile (draw pile).</param>
        /// <param name="difficulty">The game difficulty.</param>
        /// <exception cref="ArgumentException">Thrown if the provided piles are invalid.</exception>
        public BoardManager(List<List<Card>> tabelau, List<Card> stock, Difficulty difficulty)
        {
            _tabelau = tabelau;
            _foundationPiles = [];
            _stockPile = stock;
            _wastePile = [];
            _difficulty = difficulty;

            // Adding empty pile markers to foundation piles
            _foundationPiles.Add([new(customCardIcon: AsciiGraphics.GetSymbolCard(Suit.Clubs), suit: Suit.Clubs)]);
            _foundationPiles.Add([new(customCardIcon: AsciiGraphics.GetSymbolCard(Suit.Spades), suit: Suit.Spades)]);
            _foundationPiles.Add([new(customCardIcon: AsciiGraphics.GetSymbolCard(Suit.Hearts), suit: Suit.Hearts)]);
            _foundationPiles.Add([new(customCardIcon: AsciiGraphics.GetSymbolCard(Suit.Diamonds), suit: Suit.Diamonds)]);

            // Check if given piles are correct
            if (!CheckPilesCorrectness())
                throw new ArgumentException("One or more pile is incorrect");
        }

        /// <summary>
        /// Draws the current game state to the console, including all piles and pointer highlights.
        /// </summary>
        public void DrawGame()
        {
            Console.Clear();

            // Find selected card
            _pointedCard = null;

            if (_pointer.Row == 0) // Stock, waste or foundation
            {
                if (_pointer.Col == 0) // stock pile
                {
                    if (_stockPile.Count > 0)
                    {
                        _pointedCard = _stockPile[0];
                    }
                }
                else if (_pointer.Col == 1) // waste pile
                {
                    if (_wastePile.Count > 0)
                    {
                        _pointedCard = _wastePile[^1]; // Get the last card in the waste pile (the one on top, that we can use)
                    }
                }
                else // foundation piles
                {
                    int pileIndex = Math.Clamp(_pointer.Col - 2, 0, _foundationPiles.Count - 1); // Ensure pileIndex is within bounds
                    if (pileIndex >= 0)
                    {
                        _pointedCard = _foundationPiles[pileIndex][^1]; // Get the last card in the foundation pile
                    }
                }
            }
            else // Tabelau piles
            {
                int pileIndex = Math.Clamp(_pointer.Col, 0, 6); // Ensure pileIndex is within bounds

                if (_tabelau[pileIndex].Count == 0 && _pointer.Row > 0)
                {
                    _pointedCard = null;
                    _pointer.Row = 1; // Reset row to 1 if the pile is empty
                }
                else
                {
                    int cardPositionInPile = Math.Clamp(_pointer.Row - 1, 0, _tabelau[pileIndex].Count - 1); // Ensure cardPositionInPile is within bounds
                    if (pileIndex >= 0 && cardPositionInPile >= 0 && _tabelau[pileIndex].Count > 0)
                    {
                        cardPositionInPile = Math.Min(_tabelau[pileIndex].Count - 1, _pointer.Row - 1);

                        if (pileIndex < _tabelau.Count && cardPositionInPile < _tabelau[pileIndex].Count)
                            _pointedCard = _tabelau[pileIndex][cardPositionInPile];
                        else
                            _pointedCard = null;
                    }
                }
            }

            // Draw tabelau piles
            int localTabelauStartX = tabelauStartX;
            foreach (var pile in _tabelau)
            {
                if (pile.Count == 0 && _pointer.Row > 0 && _pointer.Col == _tabelau.IndexOf(pile))
                    Renderer.Cards.DrawCard(new(customCardIcon: AsciiGraphics.GetEmptyCard()), localTabelauStartX, tabelauY, _pointedCard, _selectedCard);
                else
                {
                    List<Card?>? selectedCards;
                    List<Card> emptyCard = [];

                    if (_selectedCard is null) // If no card is selected, draw the pile normally
                        selectedCards = null;
                    else // If a card is selected, draw the pile with the selected card
                    {
                        int pointedCardIndex = pile.IndexOf(_pointedCard!);
                        int selectedCardIndex = pile.IndexOf(_selectedCard);

                        if (selectedCardIndex == -1) // If the selected card is not in this pile
                            selectedCards = null;
                        else
                            selectedCards = [.. pile.Skip(selectedCardIndex).Cast<Card?>()]; // Get the selected cards from the pile (from _selectedCard to the end of the pile)

                        int selectedCardPileIndex = _tabelau.FindIndex(p => p.Contains(_selectedCard)); // Find the index of the pile containing the selected card
                        int selectedCardIndexInSelectedPile = selectedCardPileIndex == -1 ? -1 : _tabelau[selectedCardPileIndex].IndexOf(_selectedCard); // Get the index of the selected card in its pile

                        // If the selected card is in this pile and pile won't be too long, we need to show the empty card at the end of the pile
                        bool isPointedCardInPile = pile.IndexOf(_pointedCard!) != -1;
                        bool isDifferentPile = _tabelau.IndexOf(pile) != _tabelau.FindIndex(p => p.Contains(_selectedCard));
                        bool canAddEmptyCard = selectedCardPileIndex != -1 &&
                            pile.Count + (pile.Contains(_selectedCard) ?
                                0 : _tabelau[selectedCardPileIndex].Count - (selectedCardIndexInSelectedPile + 1) + 1
                            ) <= maxTabelauLength;

                        if (isPointedCardInPile && isDifferentPile && canAddEmptyCard)
                            emptyCard.Add(new(customCardIcon: AsciiGraphics.GetEmptyCard())); // Show an empty card to the pile (if all conditions are met)
                    }

                    Card pointedCard = emptyCard.Count > 0 ? emptyCard[0] : _pointedCard!; // If the pile is not full, show an empty card at the end of the pile (if the pile is full, don't show it (because we can't place any card there))
                    Renderer.Cards.DrawPile([.. pile, .. emptyCard], localTabelauStartX, tabelauY, pointedCard, selectedCards);
                }

                localTabelauStartX += 21; // Move to the right for the next pile
            }

            // Draw foundation piles
            int localFountationStartX = fountationStartX;
            foreach (var pile in _foundationPiles)
            {
                Renderer.Cards.DrawCard(pile[^1], localFountationStartX, fountationY, _pointedCard, _selectedCard);
                localFountationStartX += 23; // Move to the right for the next pile
            }

            // Draw stock pile
            if (_stockPile.Count > 0)
                Renderer.Cards.DrawCard(_stockPile[0], stockX, stockY, _pointedCard, _selectedCard);
            else if (_pointer.Row == 0 && _pointer.Col == 0)
                Renderer.Cards.DrawCard(new(customCardIcon: AsciiGraphics.GetEmptyCard()), stockX, stockY, _pointedCard, _selectedCard);

            // Draw the waste pile
            if (_wastePile.Count > 0)
            {
                int count = Math.Min(3, _wastePile.Count);
                for (int i = count; i >= 1; i--)
                {
                    // ^1 - first card (that we can use)
                    // ^2 - second card (that we can't use), is under the card ^1
                    // ^3 - third card (that we can't use), is under the card ^2
                    int offset = (i - 1) * 7;
                    Renderer.Cards.DrawCard(_wastePile[^i], wasteStartX + offset, wasteY, _pointedCard, _selectedCard);
                }
            }
        }

        /// <summary>
        /// Moves the pointer in the specified direction, wrapping around as necessary.
        /// </summary>
        /// <param name="direction">The direction to move the pointer.</param>
        public void MovePointer(Direction direction)
        {
            SoundsManager.SoundFX.PlaySound(Operation);

            // [For convenience] 
            // If we go to Direction.Left but we are in the first pile of the table, we go to the stock pile
            if (direction == Direction.Left && _pointer.Col == 0 && _pointer.Row > 0)
            {
                _pointer.Row = 0;
                _pointer.Col = 0;
            }

            switch (direction) // There is no restriction on the selected index as it is already implemented in the Col and Row setter
            {
                case Direction.Up:
                    _pointer.Row--;
                    break;
                case Direction.Down:
                    _pointer.Row++;
                    break;
                case Direction.Left:
                    _pointer.Col--;
                    break;
                case Direction.Right:
                    _pointer.Col++;
                    break;
            }

            if (_pointer.Row == 0) // Ensure col is within bounds
                _pointer.Col = Math.Clamp(_pointer.Col, 0, 5); // 5 because stock, waste and 4 foundation piles (0-5)

            else // Ensure row is within bounds for tableau piles
            {
                _pointer.Col = Math.Clamp(_pointer.Col, 0, 6); // 6 because 7 tabelau piles (0-6)
                int minRow = _tabelau[_pointer.Col].TakeWhile(card => !card.IsFaceUp).Count() + 1; // Count the number of face-down cards in the pile (and +1 because faced up card has index = count + 1)
                int maxRow = _tabelau[_pointer.Col].Count; // Get the length of the pile

                maxRow = Math.Max(maxRow, 1); // Ensure maxRow is at least 1 (if pile is empty, we want to show empty card)
                if (_selectedCard is not null)
                    minRow = maxRow; // If we want to move a card to the tableau, we can only place it at the end (bottom)

                // If we want to go to the top (waste/foundation pile) but we have hidden cards below us, we go to the waste/foundation pile (_pointer.Row = 0)
                if (direction == Direction.Up && _pointer.Row > 0 && _pointer.Row + 1 == minRow)
                {
                    _pointer.Row = 0;
                    _pointer.Col = Math.Clamp(_pointer.Col, 0, 5); // We correct the scope again (e.g. for passing from the _tabelau[^1])
                }
                else
                    _pointer.Row = Math.Clamp(_pointer.Row, minRow, maxRow);
            }

            // If the waste pile is empty and we are on index 1 (waste pile)
            if (_wastePile.Count == 0 && _pointer.Col == 1 && _pointer.Row == 0)
            {
                if (direction == Direction.Left || direction == Direction.Up) // From foundation piles or second tabelau pile
                    _pointer.Col = 0; // Go to stock pile

                else if (direction == Direction.Right) // From stock pile
                    _pointer.Col = 2; // Go to first foundation pile
            }
        }

        /// <summary>
        /// Performs the action associated with the current pointer position, such as selecting or moving cards.
        /// </summary>
        public void PointerAction()
        {
            if (_pointer.Row > 0) // Tabelau - two-step action
            {
                if (_selectedCard is null) // First action
                {
                    _selectedCard = _pointedCard;
                    return;
                }

                // Second action

                // Check if we are moving to another pile tableau. If so, we check if we can put this card on this place (game rules)
                Card card = _tabelau[_pointer.Col].Count == 0 ? _selectedCard : _tabelau[_pointer.Col][_pointer.Row - 1]; // Get the target card in the pile (if pile is empty, we want to check the selected card there)
                int cardRankIndex = _ranksOrder.IndexOf(card.Rank); // Get the index of the target card in the ranks order
                int selectedCardRankIndex = _ranksOrder.IndexOf(_selectedCard.Rank); // Get the index of the selected card in the ranks order

                // If the selected card is one rank lower than the target card and they have different colors, or if the selected card is King and we are moving to an empty pile
                if (!((cardRankIndex == selectedCardRankIndex + 1 && card.Suit.IsRed() != _selectedCard.Suit.IsRed() && _pointedCard is not null) || (card.Rank == _ranksOrder[^1] && _pointedCard is null)))
                {
                    _selectedCard = null; // Reset the selection
                    return;
                }

                // Find the pile containing the selected card
                List<Card> sourcePile = _tabelau.FirstOrDefault(pile => pile.Contains(_selectedCard))!;
                int cardIndex = sourcePile != null ? sourcePile.IndexOf(_selectedCard) : -1; // Get the index of the selected card in its pile
                int currentLength = _tabelau[_pointer.Col].Count; // Get the length of the pile
                int selectedCardsCount = sourcePile != null && cardIndex != -1
                    ? sourcePile.Count - cardIndex
                    : -1;

                if (selectedCardsCount == -1 && currentLength + 1 <= maxTabelauLength) // From waste pile, if the pile is not full
                {
                    _tabelau[_pointer.Col].Add(_selectedCard); // Add the selected card to the pile
                    _wastePile.Remove(_selectedCard); // Remove the selected card from the waste pile
                }
                else if (selectedCardsCount > 0 && currentLength + selectedCardsCount <= maxTabelauLength) // From tabelau pile, if the pile is not full
                {
                    // Move the selected cards to the new pile (if this is not the same pile)
                    if (_tabelau[_pointer.Col] != sourcePile)
                    {
                        _tabelau[_pointer.Col].AddRange(sourcePile!.Skip(cardIndex)); // Add the selected cards to the new pile
                        sourcePile!.RemoveRange(cardIndex, selectedCardsCount); // Remove the selected cards from the old pile
                    }
                }

                // Displaying ASCII rotation animation of a card

                // Flip the last card in the pile if not empty
                if (sourcePile?.Count > 0) // sourcePile is null if we are moving from waste pile
                    sourcePile[^1].IsFaceUp = true;

                _selectedCard = null; // Reset the selected card

                SoundsManager.SoundFX.PlaySound(CardMove);
            }
            else if (_pointer.Col == 0) // Stock - one-step action
            {
                if (_selectedCard is not null) // Second action
                {
                    _selectedCard = null;
                    return;
                }

                // First action
                if (_stockPile.Count > 0) // If there are any cards, we move a given number of cards (depending on the difficulty level) to the waste pile
                {
                    int cardsCount = _difficulty == Difficulty.Easy ? 1 : 3; // Get the number of cards to move to waste pile (1 for easy or 3 for other difficulties)
                    int loops = Math.Min(cardsCount, _stockPile.Count); // If there are at least cardsCount, we move that amount. If there aren't any, we move the rest
                    for (int i = 0; i < loops; i++)
                    {
                        Card card = _stockPile[0]; // Get the last card from the stock pile
                        card.IsFaceUp = true; // Turn it around
                        _wastePile.Add(card); // Add it to the waste pile
                        _stockPile.RemoveAt(0); // Remove it from the stock pile
                    }

                    SoundsManager.SoundFX.PlaySound(CardMove);
                }
                else // If there are no cards, we move the waste pile to the stock pile
                {
                    if (_wastePile.Count > 0)
                    {
                        _stockPile.AddRange(_wastePile);
                        _stockPile.ForEach(x => x.IsFaceUp = false); // Turn all cards face down
                        _wastePile.Clear();

                        // Shuffle the stock pile (after moving the cards from waste pile)
                        int length = _stockPile.Count;
                        while (length > 1)
                        {
                            length--;
                            int k = _random.Next(length + 1);
                            (_stockPile[k], _stockPile[length]) = (_stockPile[length], _stockPile[k]);
                        }
                        SoundsManager.SoundFX.PlaySound(CardShuffle);
                    }
                }
            }
            else if (_pointer.Col <= 1) // Waste - two-step action (second action only)
            {
                if (_selectedCard is null) // First action
                {
                    _selectedCard = _pointedCard;
                    return;
                }

                // Second action
                _selectedCard = null;
            }
            else // Foundation - two-step action (second action only)
            {
                if (_selectedCard is null) // If this is first action
                    return;

                // Second action
                // Check if only one card is selected (we didn't choose somewhere in the middle in _tabelau)
                if (_wastePile.IndexOf(_selectedCard) == -1 && !_tabelau.Any(pile => pile.Count > 0 && pile[^1].Equals(_selectedCard)))
                {
                    _selectedCard = null; // Reset the selection
                    return;
                }

                // Check Suit for correctness
                int foundationIndex = _pointer.Col - 2; // Get the index of the foundation pile
                Suit foundationSuit = _foundationPiles[foundationIndex][0].Suit; // Get the suit of the foundation pile
                if (_selectedCard.Suit != foundationSuit)
                {
                    _selectedCard = null; // If the suit is incorrect, reset the selected card
                    return;
                }
                // Check Rank for correctness
                if (_foundationPiles[foundationIndex].Count > 1) // If the foundation pile is not empty (only the empty card)
                {
                    Rank lastCardRank = _foundationPiles[foundationIndex][^1].Rank; // Get the rank of the last card in the foundation pile
                    int lastCardIndex = _ranksOrder.IndexOf(lastCardRank); // Get the index of the last card in the ranks order
                    int selectedCardIndex = _ranksOrder.IndexOf(_selectedCard.Rank); // Get the index of the selected card in the ranks order
                    if (selectedCardIndex != lastCardIndex + 1) // If the selected card is one rank higher than the last card in the foundation pile
                    {
                        _selectedCard = null; // If the rank is incorrect, reset the selected card
                        return;
                    }
                }
                else
                {
                    if (_selectedCard.Rank != _ranksOrder[0]) // If the foundation pile is empty, only first suit in order (default Ace) can be added
                    {
                        _selectedCard = null; // If the rank is incorrect, reset the selected card
                        return;
                    }
                }

                // Flip the last card in the pile if not empty (until we moved the card to foundation and we know where it was)
                List<Card> sourcePile = _tabelau.FirstOrDefault(pile => pile.Contains(_selectedCard))!;
                if (sourcePile?.Count > 1) // sourcePile is null if we are moving from waste pile
                    sourcePile[^2].IsFaceUp = true; // ^2 instead of ^1 because we haven't removed the card we want to move yet (so the penultimate card will be the one we need to turn over after moving the card)

                // Move the selected card to the foundation pile
                _foundationPiles[foundationIndex].Add(_selectedCard); // Add the selected card to the foundation pile
                if (!_wastePile.Remove(_selectedCard)) // Remove the selected card from the waste pile (if it exists there)
                    sourcePile!.Remove(_selectedCard); // Remove the selected card from the tableau pile (if it not exists in waste pile)

                _selectedCard = null; // Reset the selected card

                SoundsManager.SoundFX.PlaySound(CardPlacedInFoundation);
            }

            _movesCount++; // Increment the moves count
        }

        /// <summary>
        /// Resets the current selection, clearing the selected card.
        /// </summary>
        public void ResetSelection()
        {
            _selectedCard = null; // Reset the selected card
            SoundsManager.SoundFX.PlaySound(Operation);
        }

        /// <summary>
        /// Checks the correctness of the initial piles configuration (tableau, stock, foundation).
        /// </summary>
        /// <returns>True if the piles configuration is correct, false otherwise.</returns>
        private bool CheckPilesCorrectness()
        {
            // Checking tabelau
            for (int stack = 1; stack <= 7; stack++)
            {
                if (_tabelau[stack - 1].Count != stack)
                    return false;
            }

            if (_stockPile.Count == 0)
                return false;

            return true;
        }

        /// <summary>
        /// Checks if the player has won the game (all foundation piles are complete).
        /// </summary>
        /// <returns>True if the player has won, false otherwise.</returns>
        public bool CheckVictory()
        {
            // Check if all foundation piles are full
            return _foundationPiles.All(pile => pile.Count == 14); // 13 cards + empty card
        }
    }
}