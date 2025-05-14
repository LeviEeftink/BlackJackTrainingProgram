using static System.Formats.Asn1.AsnWriter;
using static System.Net.Mime.MediaTypeNames;
using System.Net.Mail;
using System.Runtime.Intrinsics.X86;
using System.Xml.Linq;
using System;
using System;
using System.Collections.Generic;

namespace Blackjack
{
    using System;
    using System.Collections.Generic;

    namespace Blackjack
    {
        /// <summary>
        /// Regelt alle spel-logica en menubesturing.
        /// </summary>
        public class GameController
        {
            public List<Player> Players { get; private set; }
            public Dealer Dealer { get; private set; }
            public Shoe Shoe { get; private set; }

            public GameController()
            {
                Dealer = new Dealer();
                Shoe = new Shoe(2);          // twee decks in de shoe
            }

            /* =========================================================
             *  BASIS-FLOW
             * =========================================================*/
            public void Start()
            {
                Console.Clear();
                ShowHeader();
                VraagAantalSpelers();
                MenuLoop();
            }

            /* =========================================================
             *  INITIALISATIE
             * =========================================================*/
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
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("================================");
                Console.WriteLine("         BLACKJACK DEALER       ");
                Console.WriteLine("================================");
                Console.ResetColor();
                Console.WriteLine();
            }

            /* =========================================================
             *  MENULOOP
             * =========================================================*/
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
                    Console.WriteLine("3. Dealer past");          // << nieuw
                    Console.WriteLine("4. Nieuwe ronde starten");
                    Console.WriteLine("5. Stoppen");
                    Console.Write("Keuze: ");

                    switch (Console.ReadLine())
                    {
                        case "1": DeelKaartAanSpeler(); break;
                        case "2": DeelKaartAanDealer(); break;
                        case "3": DealerPast(); break;   // << nieuw
                        case "4": NieuweRonde(); break;
                        case "5": return;
                        default:
                            Console.WriteLine("Ongeldige keuze.");
                            Console.WriteLine("Druk op Enter om verder te gaan...");
                            Console.ReadLine();
                            break;
                    }
                }
            }

            /* =========================================================
             *  ACTIES - SPELER
             * =========================================================*/
            void DeelKaartAanSpeler()
            {
                Console.Write($"Welke speler (1-{Players.Count}): ");
                if (int.TryParse(Console.ReadLine(), out int index) &&
                    index >= 1 && index <= Players.Count)
                {
                    var speler = Players[index - 1];

                    if (speler.HasFinished)
                    {
                        Console.WriteLine($"{speler.Name} is al klaar.");
                    }
                    else
                    {
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
                }
                else
                {
                    Console.WriteLine("Ongeldige speler.");
                }

                Console.WriteLine("Druk op Enter om verder te gaan...");
                Console.ReadLine();
            }

            /* =========================================================
             *  ACTIES - DEALER
             * =========================================================*/
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

            /// <summary>
            /// Handmatige ‘stand’ / ‘pas’ voor de dealer (POV-speelstijl).
            /// </summary>
            void DealerPast()
            {
                if (Dealer.HasFinished)
                {
                    Console.WriteLine("Dealer was al klaar.");
                }
                else
                {
                    Dealer.HasStood = true;
                    Dealer.HasFinished = true;
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($"Dealer past met {Dealer.CalculatePoints()} punten.");
                    Console.ResetColor();
                }

                Console.WriteLine("Druk op Enter om verder te gaan...");
                Console.ReadLine();
            }

            /* =========================================================
             *  RONDE & BESLISSING
             * =========================================================*/
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
                Dealer.HasStood = false;

                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Nieuwe ronde gestart.");
                Console.ResetColor();
                Console.WriteLine("Druk op Enter om verder te gaan...");
                Console.ReadLine();
            }

            /* =========================================================
             *  UI-HULP
             * =========================================================*/
            void ToonHanden()
            {
                Console.WriteLine("HUIDIGE HANDEN:\n");

                for (int i = 0; i < Players.Count; i++)
                {
                    var speler = Players[i];
                    string kaarten = speler.Hand.Count > 0 ? string.Join(", ", speler.Hand) : "(leeg)";
                    int punten = speler.CalculatePoints();
                    string status = speler.HasBlackjack ? "BLACKJACK"
                                  : speler.IsBusted ? "BUSTED"
                                  : speler.HasStood ? "STOOD"
                                  : "";

                    Console.ForegroundColor = SpelerKleur(i);
                    Console.WriteLine($"{speler.Name}: {kaarten} = {punten} {status}");
                }

                Console.ForegroundColor = ConsoleColor.White;
                string dealerKaarten = Dealer.Hand.Count > 0 ? string.Join(", ", Dealer.Hand) : "(leeg)";
                int dealerScore = Dealer.CalculatePoints();
                string dealerStatus = Dealer.HasBlackjack ? "BLACKJACK"
                                  : Dealer.IsBusted ? "BUSTED"
                                  : Dealer.HasStood ? "STOOD"
                                  : "";
                Console.WriteLine($"Dealer: {dealerKaarten} = {dealerScore} {dealerStatus}");
                Console.ResetColor();
            }

            ConsoleColor SpelerKleur(int index) => index switch
            {
                0 => ConsoleColor.Cyan,
                1 => ConsoleColor.Yellow,
                2 => ConsoleColor.Magenta,
                3 => ConsoleColor.Blue,
                _ => ConsoleColor.Gray
            };
        }
    }
}