public class Player
{
    public string Name { get; }
    public List<Card> Hand { get; private set; }
    public bool HasStood { get; set; }
    public bool HasBlackjack { get; set; }
    public bool IsBusted { get; set; }
    public bool HasFinished { get; set; }

    public Player(string name)
    {
        Name = name;
        Hand = new List<Card>();
        HasStood = false;
        HasBlackjack = false;
        IsBusted = false;
        HasFinished = false;
    }

    public int CalculatePoints()
    {
        int total = Hand.Sum(card => card.Value);
        int aces = Hand.Count(c => c.Rank == "A");

        while (total > 21 && aces-- > 0)
            total -= 10;

        return total;
    }

    public bool CheckBlackjack()
    {
        return Hand.Count == 2 && CalculatePoints() == 21;
    }

    public void MakeDecision(Shoe shoe)
    {
        if (HasStood) return;

        int score = CalculatePoints();
        if (score >= 18 && score <= 21)
        {
            HasStood = true;
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"{Name} stopt met een score van {score}.");
            Console.ResetColor();
        }
        else if (score < 18)
        {
            var card = shoe.DrawCard();
            Hand.Add(card);
            score = CalculatePoints();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"{Name} trekt een kaart: {card}. Nieuwe score: {score}");
            Console.ResetColor();
        }
    }

}
