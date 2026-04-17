# Hearts Card Game

A classic Hearts card game built with C# using WPF (Windows Presentation Foundation) and .NET 8. Play against three AI opponents in a full graphical interface.

---

## Prerequisites

Before running this project, make sure you have the following installed on your machine:

1. **Windows 10 or later** — WPF applications only run on Windows.
2. **.NET 8 SDK** — Download and install from: https://dotnet.microsoft.com/en-us/download/dotnet/8.0
3. **Visual Studio 2022** (recommended, Community Edition is free) — Download from: https://visualstudio.microsoft.com/
   - During installation, select the **".NET desktop development"** workload.

To verify .NET 8 is installed, open a terminal (Command Prompt or PowerShell) and run:

```
dotnet --version
```

You should see a version starting with `8.x.x`.

---

## How to Run the Game

### Option A: Using Visual Studio (Recommended)

1. Open the solution file `Hearts_Game.sln` in Visual Studio 2022.
2. Visual Studio will automatically restore any NuGet packages.
3. Make sure the startup project is set to **Hearts_Game** (it should be bold in the Solution Explorer; if not, right-click it and select "Set as Startup Project").
4. Click the green **Start** button (or press `F5`) to build and run the game.

### Option B: Using the Command Line

1. Open a terminal (Command Prompt or PowerShell).
2. Navigate to the project root folder (the folder containing `Hearts_Game.sln`):

```
cd path\to\Hearts_Game-master
```

3. Restore dependencies and build:

```
dotnet build
```

4. Run the game:

```
dotnet run --project Hearts_Game
```

---

## How to Play

1. When the game launches, you will see a **Start Screen**. Enter your player name and click **"Start Game"**.
2. Click **"New Game"** to deal the cards and begin playing.
3. You (South) play against three AI opponents: CPU West, CPU North, and CPU East.
4. **Click on a card** in your hand to play it during your turn.

### Hearts Rules

- Each Heart card is worth **1 penalty point**. The Queen of Spades is worth **5 penalty points**.
- The goal is to finish with the **fewest penalty points**.
- You **must follow the lead suit** if you have a card of that suit. If you don't, you may play any card.
- **Hearts cannot be led** until a Heart has been played on a previous trick ("hearts broken"), unless you have only Hearts left.
- The player who plays the highest card of the lead suit wins the trick and leads the next one.
- The game continues until a player reaches the score limit (configurable to 50 or 100 points via the Options menu).

---

## Menu Options

- **Game > New Game** — Start a fresh round.
- **Game > Quit Game** — Exit the application.
- **Options > Card Theme** — Change the card back color (Red, Blue, or Green).
- **Options > Score Limit** — Set the game-ending score to 50 or 100 points.
- **Developer > X-Ray Cards** — Toggle a developer mode that reveals all cards face-up.
- **Help** — View game help information.

---

## Project Structure

```
Hearts_Game.sln                  — Visual Studio solution file
│
├── Hearts_Game/                 — WPF UI project (the application)
│   ├── MainWindow.xaml/.cs      — Main game window and event handling
│   ├── App.xaml/.cs             — Application entry point
│   ├── ViewModels/              — MVVM view models
│   ├── GameAssets/              — Card UI controls and image assets
│   │   ├── CardUI.xaml/.cs      — Custom card user control
│   │   └── Images/Cards/        — Card face and back images (PNG)
│   └── Hearts_Game.csproj       — Project file (targets net8.0-windows)
│
├── Hearts_Logic/                — Game logic class library
│   ├── Actors/                  — Player classes
│   │   ├── Player.cs            — Abstract base class for all players
│   │   ├── HumanPlayer.cs       — Human player implementation
│   │   ├── AIPlayer.cs          — AI opponent with automated strategy
│   │   └── TestPlayer.cs        — Player class used for testing
│   ├── Managers/
│   │   └── GameManager.cs       — Singleton game controller (deals, tricks, scoring)
│   ├── Models/Objects/
│   │   ├── Card.cs              — Card data model and penalty values
│   │   ├── Deck.cs              — Deck creation and shuffling
│   │   └── Hand.cs              — Player hand management
│   ├── Services/
│   │   ├── DatabaseManager.cs   — Database connectivity
│   │   └── StatsService.cs      — Player statistics tracking
│   └── Hearts_Logic.csproj      — Project file (targets net8.0-windows)
│
└── LICENSE.txt                  — GNU GPL v3 license
```

---

## Troubleshooting

- **"SDK not found" error** — Make sure .NET 8 SDK is installed. Run `dotnet --list-sdks` to check.
- **Build errors about WPF** — Ensure you are running on Windows and have the ".NET desktop development" workload installed in Visual Studio.
- **Cards not showing** — The card images are located in `Hearts_Game/GameAssets/Images/Cards/`. Make sure these files are present after cloning.

---

## License

This project is licensed under the GNU General Public License v3.0. See `LICENSE.txt` for details.
