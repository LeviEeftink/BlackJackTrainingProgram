public enum Suit { Hearts, Diamonds, Clubs, Spades }

public class Card
{
    public Suit Suit { get; }
    public string Rank { get; }
    public int Value { get; }

    public Card(Suit suit, string rank, int value)
    {
        Suit = suit;
        Rank = rank;
        Value = value;
    }

    public override string ToString() => $"{Rank} of {Suit}";
}
