# Royal Solitaire (.NET 9.0, C#)

## How to Run the Project

1. **Requirements:**
   - .NET 9.0 SDK _(or newer)_ (only for compilation)
   - `NAudio` library (already included in the project)

2. **Steps:**

  > **Note:** Precompiled binaries for some os are located in the `/bin/Release/net9.0` directory. You can run the game directly from there without building it yourself (search for executable file).

   1. Download all files to your local machine.
   2. Ensure the `cards.txt` file, `music` and `sounds` folders are present in the output directory (`bin/Debug/net9.0/`).
   3. Open a terminal in the project root directory (where the `*.csproj` files are located).
   4. Build the project:

      ```sh
      dotnet build
      ```

   5. Run the project:

      ```sh
      dotnet run
      ```

   6. The game will launch in the console window. Make sure your console window is large enough (minimum 209x62 characters).

  > **Note:** You will get the best look at 1920x1080 resolution, 100% scaling and font `Consolas`, size `11`.

---

## Gameplay Instructions

- **Objective:**
  - Move all cards to the foundation piles, sorted by suit and in ascending order (Ace to King).

- **Controls:**
  - **Arrow Keys:** Move the pointer around the board (tableau, stock, waste, foundation).
  - **Enter:** Select or move a card(s).
  - **Space:** Reset a selection of the card(s).
  - **Escape:** Open the pause/options menu.
  - **In Menus:** Use arrow keys to navigate, Enter to select.

- **Game Modes:**
  - **Easy:** Easier cards are placed near the bottom of columns, cards are moved from stock to waste one by one.
  - **Hard:** Cards are fully shuffled, cards are moved from stock to waste 3 at a time.

- **Other Features:**
  - Sound and music can be toggled in the options menu.
  - The game will prompt you to resize the console if the window is too small.

---

## Classes, Modules, and Methods Overview

| Class/Module         | Description                                                                                 | Key Methods/Properties (Description)                                                                                 |
|----------------------|---------------------------------------------------------------------------------------------|---------------------------------------------------------------------------------------------------------------------|
| `AsciiGraphics`      | Handles loading and providing ASCII art graphics for cards.                                  | `GetCard`, `GetCardBack`, `GetEmptyCard`, `GetSymbolCard`                                                           |
| `BoardGenerator`     | Generates the initial board layout for different difficulties.                               | `Generate`, `GenerateHard`, `GenerateEasy`, `Shuffle`, `GenerateStandardDeck`                                       |
| `BoardManager`       | Manages the state and logic of the board (tableau, foundation, stock, waste).                | `DrawGame`, `Tableau`, `FoundationPiles`, `StockPile`, `WastePile`, `Pointer`, `SelectedCard`, `MovesCount`         |
| `Card`               | Represents a playing card                                                                   | `Suit`, `Rank`, `IsFaceUp`, `AsciiCardRepresentation`, `ToString`, `Equals`, `GetHashCode`                         |
| `SuitExtensions`     | Extension methods for the `Suit` enum, contains auxiliary functions (e.g. converting Suit to a string representing it)                                                      | `ToSymbol`, `IsRed`                                                                                                 |
| `RankExtensions`     | Extension methods for the `Rank` enum, contains auxiliary functions (e.g. converting Rank to a string representing it)                                                      | `ToSymbol`                                                                                                          |
| `EscapeMenu`         | Handles the pause/options menu UI and logic.                                                 | `ShowPauseMenuAndWait`, `ShowOptionsMenuAndWait`                                                                    |
| `GameManager`        | Main game controller: manages game state, menus, and main loop.                             | `ShowStartMenu`, `InitializeCardStacks`, `Start`, `ElapsedTime`, `MusicEnabled`, `SoundsFXEnabled`                  |
| `Helpers`            | Utility methods for console input and buffer management.                                     | `FlushConsoleBuffer`, `ReadKey`                                                                                     |
| `Renderer`           | Handles all console rendering (text, cards, piles, alignment).                              | `Text.Write`, `Text.WriteModified`, `Cards.DrawCard`, `Cards.DrawPile`                                              |
| `SoundsManager`      | Handles sound effects and music playback.                                                    | `SoundFX.PlaySound`, `Music.PlayMusic`, `IsSoundFXEnabled`, `IsMusicEnabled`                                        |
| `StartMenu`          | Displays the start menu and handles user input for game mode/options.                       | `ShowAndWait`                                                                                                       |
| `Victory`            | Handles the victory animation and end-of-game info display.                                 | `ShowAnimation`, `ShowInfoAboutGame`                                                                                |
| `WindowStateManager` | Manages console window size and notifies on changes.                                         | `StartWindowSizeWatcher`, `StopWindowSizeWatcher`, `HasWindowMinimumSize`, `IsWindowAvailable`, `MinimumWidth`, `MinimumHeight` |

> **Note:** For detailed method signatures and parameter descriptions, see the XML documentation in the source code files.
