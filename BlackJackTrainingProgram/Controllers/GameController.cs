public class GameController
{
    public List<Player> Players { get; private set; }
    public Dealer Dealer { get; private set; }
    public Shoe Shoe { get; private set; }

    public GameController()
    {
        Dealer = new Dealer();
        Shoe = new Shoe(2);
    }

    public void Start()
    {
        Console.Clear();
        ShowHeader();
        VraagAantalSpelers();
        MenuLoop();
    }

    void VraagAantalSpelers()
    {
        Console.Write("Hoeveel spelers? (1-4): ");
        int aantal;
        while (!int.TryParse(Console.ReadLine(), out aantal) || aantal < 1 || aantal > 4)
            Console.Write("Ongeldige invoer. Probeer opnieuw (1-4): ");

        Players = new List<Player>();
        for (int i = 1; i <= aantal; i++)
            Players.Add(new Player($"Speler {i}"));
    }

    void ShowHeader()
    {
       
    }

    void MenuLoop()
    {
        while (true)
        {
            Console.Clear();
            ShowHeader();
            ToonHanden();

            Console.WriteLine("\nMENU:");
            Console.WriteLine("1. Deel kaart aan speler");
            Console.WriteLine("2. Deel kaart aan dealer");
            Console.WriteLine("3. Nieuwe ronde starten");
            Console.WriteLine("4. Stoppen");
            Console.Write("Keuze: ");

            string keuze = Console.ReadLine();
            switch (keuze)
            {
                case "1": DeelKaartAanSpeler(); break;
                case "2": DeelKaartAanDealer(); break;
                case "3": NieuweRonde(); break;
                case "4": return;
                default: Console.WriteLine("Ongeldige keuze."); break;
            }
        }
    }

    void DeelKaartAanSpeler()
    {
        Console.Write($"Welke speler (1-{Players.Count}): ");
        if (int.TryParse(Console.ReadLine(), out int index) && index >= 1 && index <= Players.Count)
        {
            var speler = Players[index - 1];
            if (speler.HasFinished)
            {
                Console.WriteLine($"{speler.Name} is al klaar.");
                return;
            }

            int huidigeScore = speler.CalculatePoints();
            if (huidigeScore >= 18)
            {
                speler.HasStood = true;
                speler.HasFinished = true;
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"{speler.Name} past automatisch met {huidigeScore} punten.");
                Console.ResetColor();
            }
            else
            {
                var kaart = Shoe.DrawCard();
                speler.Hand.Add(kaart);
                int nieuweScore = speler.CalculatePoints();
                Console.WriteLine($"{speler.Name} kreeg: {kaart}");

                if (speler.Hand.Count == 2 && speler.CheckBlackjack())
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"{speler.Name} heeft BLACKJACK!");
                    speler.HasBlackjack = true;
                    speler.HasFinished = true;
                }
                else if (nieuweScore > 21)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"{speler.Name} is busted met {nieuweScore}.");
                    speler.IsBusted = true;
                    speler.HasFinished = true;
                }
                else if (nieuweScore >= 18)
                {
                    speler.HasStood = true;
                    speler.HasFinished = true;
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($"{speler.Name} past automatisch met {nieuweScore} punten.");
                    Console.ResetColor();
                }
            }
        }
        else
        {
            Console.WriteLine("Ongeldige speler.");
        }

        Console.WriteLine("Druk op Enter om verder te gaan...");
        Console.ReadLine();
    }

    void DeelKaartAanDealer()
    {
        var kaart = Shoe.DrawCard();
        Dealer.Hand.Add(kaart);
        Console.WriteLine($"Dealer kreeg: {kaart}");

        int score = Dealer.CalculatePoints();

        if (Dealer.Hand.Count == 2 && Dealer.CheckBlackjack())
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Dealer heeft BLACKJACK!");
            Dealer.HasBlackjack = true;
            Dealer.HasFinished = true;
        }
        else if (score > 21)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Dealer is busted met {score}.");
            Dealer.IsBusted = true;
            Dealer.HasFinished = true;
        }

        Console.ResetColor();
        Console.WriteLine("Druk op Enter om verder te gaan...");
        Console.ReadLine();
    }

    void LaatSpelersBeslissen()
    {
        foreach (var speler in Players)
        {
            if (speler.HasFinished) continue;
            speler.MakeDecision(Shoe);

            if (speler.CalculatePoints() > 21)
            {
                speler.IsBusted = true;
                speler.HasFinished = true;
            }
        }

        Console.WriteLine("Spelers hebben beslissingen genomen.");
        Console.WriteLine("Druk op Enter om verder te gaan...");
        Console.ReadLine();
    }

    void NieuweRonde()
    {
        foreach (var speler in Players)
        {
            speler.Hand.Clear();
            speler.HasBlackjack = false;
            speler.IsBusted = false;
            speler.HasFinished = false;
            speler.HasStood = false;
        }

        Dealer.Hand.Clear();
        Dealer.HasBlackjack = false;
        Dealer.IsBusted = false;
        Dealer.HasFinished = false;

        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("Nieuwe ronde gestart.");
        Console.ResetColor();
        Console.WriteLine("Druk op Enter om verder te gaan...");
        Console.ReadLine();
    }

    void ToonHanden()
    {
        Console.WriteLine("HUIDIGE HANDEN:\n");

        for (int i = 0; i < Players.Count; i++)
        {
            var speler = Players[i];
            string kaarten = string.Join(", ", speler.Hand);
            int punten = speler.CalculatePoints();
            string status = speler.HasBlackjack ? "BLACKJACK" :
                            speler.IsBusted ? "BUSTED" :
                            speler.HasStood ? "STOOD" : "";

            Console.ForegroundColor = SpelerKleur(i);
            Console.WriteLine($"{speler.Name}: {kaarten} = {punten} {status}");
        }

        Console.ForegroundColor = ConsoleColor.White;
        string dealerKaarten = string.Join(", ", Dealer.Hand);
        int dealerScore = Dealer.CalculatePoints();
        string dealerStatus = Dealer.HasBlackjack ? "BLACKJACK" :
                              Dealer.IsBusted ? "BUSTED" : "";
        Console.WriteLine($"Dealer: {dealerKaarten} = {dealerScore} {dealerStatus}");
        Console.ResetColor();
    }

    ConsoleColor SpelerKleur(int index)
    {
        return index switch
        {
            0 => ConsoleColor.Cyan,
            1 => ConsoleColor.Yellow,
            2 => ConsoleColor.Magenta,
            3 => ConsoleColor.Blue,
            _ => ConsoleColor.Gray,
        };
    }
}
