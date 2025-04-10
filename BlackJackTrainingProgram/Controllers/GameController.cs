public class GameController
{
    public List<Player> Players { get; private set; }
    public Dealer Dealer { get; private set; }
    public Shoe Shoe { get; private set; }

    public GameController()
    {
        Players = new List<Player>
        {
            new Player("Player 1"),
            new Player("Player 2")
        };
        Dealer = new Dealer();
        Shoe = new Shoe(2);
    }

    public void Start()
    {
        Console.Clear();
        Console.WriteLine("🃏 Welcome to Blackjack Dealer Trainer 🃏");
        ShowHeader();
        MenuLoop();
    }

    void ShowHeader()
    {
        Console.WriteLine($"Players: {string.Join(", ", Players.Select(p => p.Name))}");
    }

    void MenuLoop()
    {
        while (true)
        {
            Console.WriteLine("\n1. Give Card (players decide)\n2. Show Hands\n3. Auto Play Round\n4. Show Dealer Score\n5. Exit");
            Console.Write("Choose: ");
            var choice = Console.ReadLine();
            switch (choice)
            {
                case "1": GiveCard(); break;
                case "2": ShowHands(); break;
                case "3": FinishRound(); break;
                case "4": Dealer.ShowScore(); break;
                case "5": return;
            }
        }
    }

    void GiveCard()
    {
        Console.WriteLine("\n================================");

        for (int i = 0; i < Players.Count; i++)
        {
            var player = Players[i];

            if (player.HasStood)
            {
                Console.WriteLine($"Player {i + 1} already stood.");
                continue;
            }

            int score = player.CalculatePoints();

            if (score >= 18)
            {
                player.HasStood = true;
                Console.WriteLine($"Player {i + 1} wants to stand. His score is {score}.");

                if (player.CheckBlackjack())
                    Console.WriteLine($"🎉 Player {i + 1} has BLACKJACK!");
            }
            else
            {
                var card = Shoe.DrawCard();
                player.Hand.Add(card);
                score = player.CalculatePoints();
                Console.WriteLine($"Player {i + 1} hit. Drew {card}. New score: {score}");

                if (score >= 18)
                {
                    player.HasStood = true;
                    Console.WriteLine($"Player {i + 1} now wants to stand with {score}.");
                    if (player.CheckBlackjack())
                        Console.WriteLine($"🎉 Player {i + 1} has BLACKJACK!");
                }
            }
            Console.WriteLine("\n================================");
        }

        var dealerCard = Shoe.DrawCard();
        Dealer.Hand.Add(dealerCard);
        Console.WriteLine($"Dealer drew {dealerCard}. Dealer score: {Dealer.CalculatePoints()}");
    }

    void ShowHands()
    {
        for (int i = 0; i < Players.Count; i++)
        {
            var player = Players[i];
            string handStr = string.Join(", ", player.Hand);
            int score = player.CalculatePoints();
            string blackjack = player.CheckBlackjack() ? " 🎉 BLACKJACK!" : "";
            Console.WriteLine($"Player {i + 1}: {handStr} = {score}{blackjack}");
        }

        string dealerHand = string.Join(", ", Dealer.Hand);
        int dealerScore = Dealer.CalculatePoints();
        string dealerBJ = Dealer.CheckBlackjack() ? " 🎉 BLACKJACK!" : "";
        Console.WriteLine($"Dealer: {dealerHand} = {dealerScore}{dealerBJ}");
    }

    void FinishRound()
    {
        bool allStood = false;
        while (!allStood)
        {
            allStood = true;
            foreach (var player in Players)
            {
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
            Console.WriteLine($"Player {Players.IndexOf(player) + 1} has {playerPoints} vs Dealer's {dealerPoints} -> {(correct ? "Correct" : "Wrong")}");
        }

        Players.ForEach(p => { p.Hand.Clear(); p.HasStood = false; });
        Dealer.Hand.Clear();
    }
}
