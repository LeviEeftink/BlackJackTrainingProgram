public class Shoe
{
    public List<Card> Cards { get; private set; }
    private Random rng = new Random();

    public Shoe(int numberOfDecks)
    {
        Cards = new List<Card>();
        for (int i = 0; i < numberOfDecks; i++)
            Cards.AddRange(new Deck().Cards);

        Shuffle();
    }

    public void Shuffle() => Cards = Cards.OrderBy(c => rng.Next()).ToList();

    public Card DrawCard()
    {
        if (Cards.Count == 0) throw new Exception("No cards left in shoe!");
        var card = Cards[0];
        Cards.RemoveAt(0);
        return card;
    }
}
