public class Dealer : Player
{
    public int Score { get; private set; }

    public Dealer() : base("Dealer") { }

    public void AddScore(bool correct) => Score += correct ? 1 : 0;

    public void ShowScore() => Console.WriteLine($"Dealer Score: {Score}");

    public bool CheckBlackjack()
    {
        return Hand.Count == 2 && CalculatePoints() == 21;
    }
}
