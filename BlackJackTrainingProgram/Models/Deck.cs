public class Deck
{
    public List<Card> Cards { get; private set; }

    public Deck()
    {
        Cards = new List<Card>();
        string[] ranks = { "2", "3", "4", "5", "6", "7", "8", "9", "10", "J", "Q", "K", "A" };
        int[] values = { 2, 3, 4, 5, 6, 7, 8, 9, 10, 10, 10, 10, 11 };

        foreach (Suit suit in Enum.GetValues(typeof(Suit)))
        {
            for (int i = 0; i < ranks.Length; i++)
            {
                Cards.Add(new Card(suit, ranks[i], values[i]));
            }
        }
    }
}
