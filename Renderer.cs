using System;
using System.Diagnostics;
using System.Formats.Asn1;
using System.Text;

namespace Solitaire
{
    /// <summary>
    /// Provides functionality for rendering text and cards with different alignments on the console.
    /// </summary>
    public static class Renderer
    {
        /// <summary>
        /// Specifies text alignment options for rendering.
        /// </summary>
        public enum Align
        {
            /// <summary>Align text to the left.</summary>
            Left,
            /// <summary>Align text to the center.</summary>
            Center,
            /// <summary>Align text to the right.</summary>
            Right
        }

        /// <summary>
        /// Contains methods for displaying text on the console.
        /// </summary>
        public static class Text
        {
            /// <summary>
            /// Writes the specified text to the console with formatting and alignment.
            /// </summary>
            /// <param name="text">Text to display.</param>
            /// <param name="align">Desired text alignment (default is Left).</param>
            /// <param name="x">X-coordinate offset or additional padding for Left/Right alignment (default is 0).</param>
            /// <param name="y">Y-coordinate starting position (default is 0).</param>
            /// <param name="foregroundColor">Foreground color for the text (default is White).</param>
            public static void Write(string text, Align align = Align.Left, int x = 0, int y = 0, ConsoleColor foregroundColor = ConsoleColor.White)
            {
                WriteModified(text, align, x, y, foregroundColor);
            }

            /// <summary>
            /// Writes the specified text to the console with formatting (card frames too) and alignment.
            /// </summary>
            /// <param name="text">Text to display.</param>
            /// <param name="align">Desired text alignment (default is Left).</param>
            /// <param name="x">X-coordinate offset or additional padding for Left/Right alignment (default is 0).</param>
            /// <param name="y">Y-coordinate starting position (default is 0).</param>
            /// <param name="foregroundColor">Foreground color for the text (default is White).</param>
            /// <param name="cardFrameColor">Foreground color for the card frame (default is White).</param>
            internal static void WriteModified(string text, Align align = Align.Left, int x = 0, int y = 0, ConsoleColor foregroundColor = ConsoleColor.White, ConsoleColor cardFrameColor = ConsoleColor.White)
            {
                // Store the current color to restore it after writing.
                ConsoleColor previousColor = Console.ForegroundColor;
                Console.ForegroundColor = foregroundColor;

                // Split the text into individual lines using standard newline characters.
                var lines = text.Split(["\r\n", "\n"], StringSplitOptions.None);

                // Write each line with the specified alignment.
                foreach (var line in lines)
                {
                    int posX = CalculateAlignedX(line, align, x);
                    Console.SetCursorPosition(posX, y);

                    int start = 0;
                    ConsoleColor? lastColor = null;
                    for (int i = 0; i <= line.Length; i++)
                    {
                        ConsoleColor color = foregroundColor;
                        if (i < line.Length)
                        {
                            char c = line[i];
                            if (c == '┏' || c == '┓' || c == '┛' || c == '┗' || c == '┃' || c == '┅' || c == '┇')
                                color = cardFrameColor;
                            else if (c == '♥' || c == '♦')
                                color = ConsoleColor.Red;
                            else if (c == '♣' || c == '♠')
                                color = ConsoleColor.DarkGray;
                            else if (c == '$')
                                color = ConsoleColor.Blue;

                            // Leave spaces unchanged (spaces are invisible).
                        }
                        else
                        {
                            color = lastColor ?? foregroundColor;
                        }

                        if (i == line.Length || color != lastColor)
                        {
                            if (lastColor != null && start < i)
                            {
                                Console.ForegroundColor = lastColor.Value;
                                Console.Write(line[start..i]);
                            }
                            start = i;
                            lastColor = color;
                        }
                    }
                    y++;
                }

                // Restore the original console color.
                Console.ForegroundColor = previousColor;
            }

            /// <summary>
            /// Calculates the starting X position based on the specified alignment.
            /// </summary>
            /// <param name="line">The line of text to display.</param>
            /// <param name="align">Desired text alignment.</param>
            /// <param name="offset">X-coordinate offset for Left or Right alignment.</param>
            /// <returns>The calculated X position for the text.</returns>
            private static int CalculateAlignedX(string line, Align align, int offset)
            {
                return align switch
                {
                    Align.Center => Math.Max(0, (Console.WindowWidth - line.Length) / 2),
                    Align.Right => Math.Max(0, Console.WindowWidth - line.Length - offset),
                    _ => offset, // Default for Left alignment.
                };
            }
        }

        /// <summary>
        /// Contains methods for displaying cards on the console.
        /// </summary>
        public static class Cards
        {
            /// <summary>
            /// Number of visible lines when cards are stacked (protruding effect).
            /// </summary>
            private const int ProtrudingLines = 3;

            /// <summary>
            /// Draws a single card at the specified position, with optional pointer and selection highlighting.
            /// </summary>
            /// <param name="card">The card to draw.</param>
            /// <param name="x">X-coordinate for the card's position.</param>
            /// <param name="y">Y-coordinate for the card's position.</param>
            /// <param name="pointedCard">Optional card to highlight as pointed.</param>
            /// <param name="selectedCard">Optional card to highlight as selected.</param>
            /// <param name="pointerColor">Color for the pointer highlight.</param>
            /// <param name="selectionColor">Color for the selection highlight.</param>
            public static void DrawCard(Card card, int x = 0, int y = 0, Card? pointedCard = null, Card? selectedCard = null, ConsoleColor pointerColor = ConsoleColor.DarkYellow, ConsoleColor selectionColor = ConsoleColor.DarkGreen)
            {
                string cardString = card.AsciiCardRepresentation;
                ConsoleColor frameColor = ConsoleColor.White;

                if ((pointedCard != null && card.Equals(pointedCard)) || (pointedCard == null && card.Equals(new Card(customCardIcon: AsciiGraphics.GetEmptyCard()))))
                {
                    frameColor = pointerColor;
                }
                else if (selectedCard != null && card.Equals(selectedCard))
                {
                    frameColor = selectionColor;
                }

                Text.WriteModified(cardString, Align.Left, x, y, ConsoleColor.White, frameColor);
            }

            /// <summary>
            /// Draws a pile of cards at the specified position, with optional pointer and selection highlighting.
            /// </summary>
            /// <param name="cards">The list of cards to draw.</param>
            /// <param name="x">X-coordinate for the pile's position.</param>
            /// <param name="y">Y-coordinate for the pile's position.</param>
            /// <param name="pointedCard">Optional card to highlight as pointed.</param>
            /// <param name="selectedCard">Optional card to highlight as selected.</param>
            /// <param name="pointerColor">Color for the pointer highlight.</param>
            /// <param name="selectionColor">Color for the selection highlight.</param>
            public static void DrawPile(List<Card> cards, int x = 0, int y = 0, Card? pointedCard = null, Card? selectedCard = null, ConsoleColor pointerColor = ConsoleColor.DarkYellow, ConsoleColor selectionColor = ConsoleColor.DarkGreen)
            {
                int currentY = y;

                for (int i = 0; i < cards.Count; i++)
                {
                    Card card = cards[i];
                    string cardString = card.AsciiCardRepresentation;
                    if (i != cards.Count - 1)
                    {
                        cardString = string.Join(Environment.NewLine, cardString
                            .Split(["\r\n", "\n"], StringSplitOptions.None)
                            .Take(ProtrudingLines));
                    }

                    ConsoleColor frameColor;
                    if (pointedCard != null && card.Equals(pointedCard))
                    {
                        frameColor = pointerColor;
                    }
                    else if (selectedCard != null && card.Equals(selectedCard))
                    {
                        frameColor = selectionColor;
                    }
                    else
                    {
                        frameColor = ConsoleColor.White;
                    }
                    Text.WriteModified(cardString, Align.Left, x, currentY, ConsoleColor.White, frameColor);

                    // Move Y down for next card (protrudingLines for all but last, else full card height)
                    int lines = cardString.Split(["\r\n", "\n"], StringSplitOptions.None).Length;
                    currentY += lines;
                }
            }

            /// <summary>
            /// Draws a pile of cards at the specified position, with optional pointer and selection highlighting.
            /// </summary>
            /// <param name="cards">The list of cards to draw.</param>
            /// <param name="x">X-coordinate for the pile's position.</param>
            /// <param name="y">Y-coordinate for the pile's position.</param>
            /// <param name="pointedCard">Optional card to highlight as pointed.</param>
            /// <param name="selectedCards">Optional list of cards to highlight as selected.</param>
            /// <param name="pointerColor">Color for the pointer highlight.</param>
            /// <param name="selectionColor">Color for the selection highlight.</param>
            /// <remarks>
            /// If <paramref name="selectedCards"/> is not null, any card in the pile that matches an element in <paramref name="selectedCards"/> will be highlighted.
            /// </remarks>
            public static void DrawPile(List<Card> cards, int x = 0, int y = 0, Card? pointedCard = null, List<Card?>? selectedCards = null, ConsoleColor pointerColor = ConsoleColor.DarkYellow, ConsoleColor selectionColor = ConsoleColor.DarkGreen)
            {
                int currentY = y;

                for (int i = 0; i < cards.Count; i++)
                {
                    Card card = cards[i];
                    string cardString = card.AsciiCardRepresentation;
                    if (i != cards.Count - 1)
                    {
                        cardString = string.Join(Environment.NewLine, cardString
                            .Split(["\r\n", "\n"], StringSplitOptions.None)
                            .Take(ProtrudingLines));
                    }

                    ConsoleColor frameColor;
                    if (pointedCard != null && card.Equals(pointedCard))
                    {
                        frameColor = pointerColor;
                    }
                    else if (selectedCards != null && selectedCards.Any(card.Equals))
                    {
                        frameColor = selectionColor;
                    }
                    else
                    {
                        frameColor = ConsoleColor.White;
                    }
                    Text.WriteModified(cardString, Align.Left, x, currentY, ConsoleColor.White, frameColor);

                    // Move Y down for next card (protrudingLines for all but last, else full card height)
                    int lines = cardString.Split(["\r\n", "\n"], StringSplitOptions.None).Length;
                    currentY += lines;
                }
            }
        }
    }
}