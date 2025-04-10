public class Player
{
    public string Name { get; }
    public List<Card> Hand { get; private set; }
    public bool HasStood { get; set; }

    public Player(string name)
    {
        Name = name;
        Hand = new List<Card>();
        HasStood = false;
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
        if (score < 18)
        {
            Hand.Add(shoe.DrawCard());
        }
        else
        {
            HasStood = true;
        }
    }
}
