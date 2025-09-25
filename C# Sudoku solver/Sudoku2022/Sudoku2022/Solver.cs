using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;


namespace Sudoku2022
{
    // Solver Class, Hier staat alles in dat te maken heeft met het zoekalgoritme
    public class Solver 
    {

        // SwapItem Class, Object die gemaakt wordt voor een swap, één SwapItem is gewoon een Blok in je Grid
        // Het object heeft dus een eigenschap row en index
        public class SwapItem {

            public int row;
            public int index;


            public SwapItem(int r, int i) {
                this.row = r;
                this.index = i;
            }

        }

        //Lijst van Blokken, omdat het Grid niet opgebouwd is door blokken van 3x3 moeten deze apart gemaakt worden
        public List<List<SwapItem>> Blocks = new List<List<SwapItem>>();
        //Lijst van alle mogelijke swaps per block
        public List<List<Tuple<SwapItem, SwapItem>>> Swaps = new List<List<Tuple<SwapItem, SwapItem>>>();

        //Initialiseer functie, deze functie zorgt ervoor dat alle nodige object geinitialiseerd worden
        public void Initialize(Grid G)
        {
            FillBlocks();
            FillSwaps(G);
            FillZeros(G);
        }

        //  Deze functie zorgt ervoor dat de Lijst van 3x3 blokken gevult wordt. De blokken zijn gevult met SwapItems met de juiste row,index waarden
        //  Het grid is opgebouwd van 8 rows van 8 kolommen, een blok bestaat dus altijd uit 3 row waarden en 3 kolom waarden.
        private void FillBlocks() {
            for (int j = 0; j < 3; j++)
            {
                for (int i = 0; i < 3; i++)
                {
                    // Maakt Blok en voegt deze toe aan de Blocks lijst
                    this.Blocks.Add(new List<SwapItem>() {
                        new SwapItem(0 + 3 * j, 0 + 3*i), new SwapItem(0 + 3 * j, 1 + 3*i), new SwapItem(0 + 3 * j, 2 + 3*i),
                        new SwapItem(1 + 3 * j, 0 + 3*i), new SwapItem(1 + 3 * j, 1 + 3*i),  new SwapItem(1 + 3 * j, 2 + 3*i),
                        new SwapItem(2 + 3 * j, 0 + 3*i), new SwapItem(2 + 3 * j, 1 + 3*i), new SwapItem(2 + 3 * j, 2 + 3*i) });

                }
            }
        }

        //  Deze functie zorgt ervoor dat per blok van 3x3 alle mogelijk swaps gecreëerd worden, deze swaps worden vervolgens in een lijst gestopt.
        //  Al deze lijsten worden vervolgens weer in een lijst gestopt waardoor de Swaps lijst op index x het xde blok is.
        //  De functie krijgt een parameter Grid mee, deze grid wordt gebruikt om te checken of een bepaalde waarde op het grid 0 is of niet.
        //  Als de waarde 0 is betekent dat de waarde op het grid niet gefixeerd is en dus geswapped mag worden.
        private void FillSwaps(Grid G)
        {
            foreach (List<SwapItem> L in Blocks) {
                List<Tuple<SwapItem, SwapItem>> T = new List<Tuple<SwapItem, SwapItem>>();
                for (int i = 0; i < L.Count; i++) {
                    for (int j = i + 1; j < L.Count; j++) {
                        if (G.Rows[L[i].row].RC[L[i].index].Value == 0 && G.Rows[L[j].row].RC[L[j].index].Value == 0)
                        {
                            T.Add(new Tuple<SwapItem, SwapItem>(L[i], L[j]));
                        }

                    }
                }
                Swaps.Add(T);
            }
        }

        //  Deze functie zorgt ervoor dat alle nullen op het grid gevult worden met een cijfer van 1-9
        //  Dit gaat per blok zodat er per blok geen cijfer twee keer voorkomen
        private void FillZeros(Grid G) {
            foreach (List<SwapItem> L in Blocks) {

                Random R = new Random();
                //  Temp is een lijst van 1 - 9 
                List<int> Temp = new List<int>() { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
                List<int> Curr = new List<int>();

                // Hier wordt Curr gevult door alle cijfers die voorkomen in het blok (die niet 0 zijn)
                for (int i = 0; i < L.Count; i++) {
                    int Val = G.Rows[L[i].row].RC[L[i].index].Value;
                    if (Val != 0) {
                        Curr.Add(Val);
                    }
                }
                //  Exc is hier nu de lijst van 1-9 zonder de cijfers die al voorkomen in het blok
                List<int> Exc = Temp.Except(Curr).ToList();
                
                //  De cijfers in Exc worden nu ingevuld bij waarden gelijk aan 0
                for (int i = 0; i < L.Count; i++) {
                    if (G.Rows[L[i].row].RC[L[i].index].Value == 0){
                        int rnd = R.Next(0, Exc.Count);
                        G.Rows[L[i].row].RC[L[i].index].Value = Exc[rnd];
                        Exc.RemoveAt(rnd);
                    }
                }
            }


        }


        // Swap functie die een Tuple van SwapItems en het Grid als parameters meekrijgt
        // De functie gebruikt de Tuple van SwapItems om twee waarden op het Grid te wisselen.
        private void Swap(Tuple<SwapItem, SwapItem> SwapIndex, Grid G) {
            //Swap twee waarden
            GridItem T = new GridItem(G.Rows[SwapIndex.Item1.row].RC[SwapIndex.Item1.index].Value);
            G.Rows[SwapIndex.Item1.row].RC[SwapIndex.Item1.index] = G.Rows[SwapIndex.Item2.row].RC[SwapIndex.Item2.index];
            G.Columns[SwapIndex.Item1.index].RC[SwapIndex.Item1.row] = G.Columns[SwapIndex.Item2.index].RC[SwapIndex.Item2.row];

            G.Rows[SwapIndex.Item2.row].RC[SwapIndex.Item2.index] = T;
            G.Columns[SwapIndex.Item2.index].RC[SwapIndex.Item2.row] = T;

            //Roept EvalRC() op om de twee rijen en twee kolommen opnieuw te evalueren
            G.Rows[SwapIndex.Item1.row].EvalRC();
            G.Rows[SwapIndex.Item2.row].EvalRC();
            G.Columns[SwapIndex.Item1.index].EvalRC();
            G.Columns[SwapIndex.Item2.index].EvalRC();


        }

        // De randomwalk functie voor het lokaal zoeken
        private void RandomWalk(Grid G,int itt) {
            Random R = new Random();
            //Ga door totdat er itt randomwalks plaats gevonden hebben
            for (int t = 0; t < itt; t++)
            {
                //Kies willekeurig een swap uit de Swaps lijst
                int r = R.Next(0, 9);
                int i = R.Next(0, Swaps[r].Count);
                Tuple<SwapItem, SwapItem> RandomSwap = new Tuple<SwapItem, SwapItem>(Swaps[r][i].Item1, Swaps[r][i].Item2);

                int[] S = new int[4] { RandomSwap.Item1.row, RandomSwap.Item2.row, RandomSwap.Item1.index, RandomSwap.Item2.index };
                // Evaluatie voor de wissel
                int EvalBeforeSwap = G.Rows[S[0]].Score + G.Rows[S[1]].Score + G.Columns[S[2]].Score + G.Columns[S[3]].Score;
                // Voer de wissel uit
                Swap(RandomSwap, G);
                // Evaluatie na de wissel
                int EvalAfterSwap = G.Rows[S[0]].Score + G.Rows[S[1]].Score + G.Columns[S[2]].Score + G.Columns[S[3]].Score;

                // Update de Score van het Grid
                if (EvalAfterSwap > EvalBeforeSwap) { G.Score = G.Score + (EvalAfterSwap - EvalBeforeSwap); }
                else { G.Score = G.Score - (EvalBeforeSwap - EvalAfterSwap); }

            }

        }

        // Het Hill Climbing algoritme
        public Tuple<int,int> Solve(Grid G,int StopCrit,int S) {
            Random R = new Random();
            bool RWalk = false;
            int Plateau = 0;
            int itt = 0;
            int walks = 0;
            // Ga door totdat Grid score gelijk staat aan 0 (doeltoestand gevonden)
            while (G.Score != 0)
            {
                itt++;

                // Als randomwalk uitgevoerd moet worden voor randomwalk uit

                if (RWalk == true) {
                    walks++;
                    // S is het aantal random walks die er gedaan moeten worden
                    RandomWalk(G,S);
                    RWalk = false;

                }

                // Normaal hill climbing zonder random walk
                else
                {
                    // Kies een van de 9 blokken
                    int r = R.Next(0, 9);
                    Tuple<SwapItem, SwapItem> BestSwap = null;
                    int BestSwapScore = int.MaxValue;
                    int improvement = 0;
                    
                    // Voor alle mogelijk swaps in de blok uit
                    foreach (Tuple<SwapItem, SwapItem> Tup in Swaps[r])
                    {
                        
                        int[] RCToEval = new int[4] { Tup.Item1.row, Tup.Item2.row, Tup.Item1.index, Tup.Item2.index };
                        // Score voordat de swap uitgevoerd is
                        int CurrentScore = G.Rows[RCToEval[0]].Score + G.Rows[RCToEval[1]].Score + G.Columns[RCToEval[2]].Score + G.Columns[RCToEval[3]].Score;
                        Swap(Tup, G); //Swap

                        // Score nadat de swap uitgevoerd is
                        int SwapScore = G.Rows[RCToEval[0]].Score + G.Rows[RCToEval[1]].Score + G.Columns[RCToEval[2]].Score + G.Columns[RCToEval[3]].Score;

                        // Als de toestand na de wissel beter is dan de huidige toestand, en beter dan de beste wissel tot nu toe
                        if (SwapScore <= CurrentScore && SwapScore <= BestSwapScore)
                        {
                            //Update beste wissel
                            BestSwap = new Tuple<SwapItem,SwapItem>(Tup.Item1,Tup.Item2);
                            //Update de beste toestand
                            BestSwapScore = SwapScore;
                            improvement = CurrentScore - SwapScore;
                        }
                        // Wissel weer terug naar originele toestand voor de wissel
                        Swap(new Tuple<SwapItem, SwapItem>(Tup.Item2, Tup.Item1), G); //Swap terug

                    }


                    // Als er een beste wissel is
                    if (BestSwap != null)
                    {
                        // Voer deze wissel uit
                        Swap(BestSwap, G);
                        //Update de grid score
                        G.Score = G.Score - improvement;

                    }

                    // Als er geen verbetering is
                    if (improvement == 0)
                    {
                        // Verhoog Plateau met 1
                        Plateau++;

                    }

                    // Als er wel een verbetering was wordt Plateau weer 0
                    else { Plateau = 0; }

                    // Als er een Plateau wordt geconstateert dan wordt Randomwalk in de volgende itteratie uitgevoerd
                    if (Plateau == StopCrit)
                    {
                        Plateau = 0;
                        RWalk = true;

                    }
                }

            }

            Debug.WriteLine("Itterations: " + itt);
            return new Tuple<int,int>(itt,walks);
        }

    }

}
