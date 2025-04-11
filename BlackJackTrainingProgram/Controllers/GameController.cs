public class GameController
{
    public List<Player> Players { get; private set; }
    public Dealer Dealer { get; private set; }
    public Shoe Shoe { get; private set; }

    public GameController()
    {
        Players = new List<Player>
        {
            new Player("Speler 1"),
            new Player("Speler 2")
        };
        Dealer = new Dealer();
        Shoe = new Shoe(2);
    }

    public void Start()
    {
        Console.Clear();
        ShowHeader();
        MenuLoop();
    }

    void ShowHeader()
    {
        Console.WriteLine("\n=======================================");
        Console.WriteLine("           Welkom bij het Spel!");
        Console.WriteLine("=======================================");
        Console.WriteLine($"Spelers: {string.Join(", ", Players.Select(p => p.Name))}");
        Console.WriteLine("=======================================");
    }

    void MenuLoop()
    {
        while (true)
        {
            Console.WriteLine("\n======================== Menu ========================");
            Console.WriteLine("1. Geef Kaart aan Speler 1");
            Console.WriteLine("2. Geef Kaart aan Speler 2");
            Console.WriteLine("3. Geef Kaart aan Dealer");
            Console.WriteLine("4. Toon Handen");
            Console.WriteLine("5. Speel Ronde Automatisch");
            Console.WriteLine("6. Afsluiten");
            Console.WriteLine("7. Controleer op Blackjacks");
            Console.WriteLine("======================================================");
            Console.Write("Kies een optie (1-7): ");
            var keuze = Console.ReadLine();

            Console.WriteLine("======================================================");

            switch (keuze)
            {
                case "1": ManualGiveCard(1); break;
                case "2": ManualGiveCard(2); break;
                case "3": ManualGiveCard(0); break;  // Dealer is index 0
                case "4": ShowHands(); break;
                case "5": FinishRound(); break;
                case "6": return;
                case "7": CheckForBlackjacks(); break;
                default: Console.WriteLine("Ongeldige keuze, probeer opnieuw."); break;
            }
        }
    }

    void ManualGiveCard(int playerIndex)
    {
        if (playerIndex == 0)  // Dealer
        {
            var card = Shoe.DrawCard();
            Dealer.Hand.Add(card);
            int dealerScore = Dealer.CalculatePoints();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"Dealer trok: {card}. Nieuwe score: {dealerScore}");
            Console.ResetColor();
        }
        else if (playerIndex == 1 || playerIndex == 2) 
        {
            var player = Players[playerIndex - 1];
            if (player.HasStood)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"Speler {playerIndex} heeft al gepast.");
                Console.ResetColor();
                return;
            }

            var card = Shoe.DrawCard();
            player.Hand.Add(card);
            int playerScore = player.CalculatePoints();
            Console.WriteLine($"Speler {playerIndex} trok: {card}. Nieuwe score: {playerScore}");

            if (playerScore >= 18)
            {
                player.HasStood = true;
                Console.WriteLine($"Speler {playerIndex} past nu met {playerScore}.");
                if (player.CheckBlackjack())
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"🎉 Speler {playerIndex} heeft BLACKJACK!");
                    Console.ResetColor();
                }
            }
        }
    }

    void ShowHands()
    {
        Console.WriteLine("\n==================== Heden Handen ====================");
        for (int i = 0; i < Players.Count; i++)
        {
            var player = Players[i];
            string handStr = string.Join(", ", player.Hand);
            int score = player.CalculatePoints();
            string blackjack = player.Hand.Count == 2 && player.CheckBlackjack() ? " BLACKJACK!" : "";

            if (score > 21)
                Console.ForegroundColor = ConsoleColor.Red;
            else if (score >= 18)
                Console.ForegroundColor = ConsoleColor.Green;

            Console.WriteLine($"Speler {i + 1}: {handStr} = {score}{blackjack}");
            Console.ResetColor();
        }

        string dealerHand = string.Join(", ", Dealer.Hand);
        int dealerScore = Dealer.CalculatePoints();
        string dealerBJ = Dealer.Hand.Count == 2 && Dealer.CheckBlackjack() ? " BLACKJACK!" : "";

        if (dealerScore > 21)
            Console.ForegroundColor = ConsoleColor.Red;
        else if (dealerScore >= 18)
            Console.ForegroundColor = ConsoleColor.Green;

        Console.WriteLine($"Dealer: {dealerHand} = {dealerScore}{dealerBJ}");
        Console.ResetColor();
        Console.WriteLine("=======================================================");
    }

    void CheckForBlackjacks()
    {
        bool anyBlackjack = false;

        foreach (var player in Players)
        {
            int score = player.CalculatePoints();
            if (score == 21 && player.Hand.Count == 2)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"{player.Name} heeft Blackjack!");
                player.HasBlackjack = true;
                player.HasFinished = true;
                anyBlackjack = true;
                Console.ResetColor();
            }
            else if (score > 21)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"{player.Name} is busted met een score van {score}.");
                player.IsBusted = true;
                player.HasFinished = true;
                Console.ResetColor();
            }
        }

        int dealerScore = Dealer.CalculatePoints();
        if (dealerScore == 21 && Dealer.Hand.Count == 2)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Dealer heeft Blackjack!");
            Dealer.HasBlackjack = true;
            Dealer.HasFinished = true;
            anyBlackjack = true;
            Console.ResetColor();
        }
        else if (dealerScore > 21)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Dealer is busted met een score van {dealerScore}.");
            Dealer.IsBusted = true;
            Dealer.HasFinished = true;
            Console.ResetColor();
        }

        if (!anyBlackjack)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Geen Blackjacks gevonden.");
            Console.ResetColor();
        }

        ShowHands();
    }

    void FinishRound()
    {
        bool allStood = false;
        while (!allStood)
        {
            allStood = true;
            foreach (var player in Players)
            {
                if (player.HasFinished) continue;

                player.MakeDecision(Shoe);
                if (!player.HasStood) allStood = false;
            }
        }

        int dealerPoints = Dealer.CalculatePoints();
        foreach (var player in Players)
        {
            int playerPoints = player.CalculatePoints();
            bool correct = playerPoints <= 21 && (playerPoints > dealerPoints || dealerPoints > 21);
            Dealer.AddScore(correct);
            Console.ForegroundColor = correct ? ConsoleColor.Green : ConsoleColor.Red;
            Console.WriteLine($"Speler {Players.IndexOf(player) + 1} heeft {playerPoints} tegen Dealer's {dealerPoints} -> {(correct ? "Correct" : "Fout")}");
            Console.ResetColor();
        }

        Players.ForEach(p =>
        {
            p.Hand.Clear();
            p.HasStood = false;
            p.HasFinished = false;
            p.HasBlackjack = false;
            p.IsBusted = false;
        });

        Dealer.Hand.Clear();
        Dealer.HasFinished = false;
        Dealer.HasBlackjack = false;
        Dealer.IsBusted = false;
    }
}
