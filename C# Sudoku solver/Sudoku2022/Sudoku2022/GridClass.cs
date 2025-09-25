using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;


namespace Sudoku2022
{
    // Grid class 
    // Grid bestaat uit 8 rijen en 8 kolommen
    // Grid.Rows[0].RC[0] staat dus voor de waarde helemaal links boven in een 8x8 Grid
    public class Grid
    {
        
        public GridRC[] Rows;
        public GridRC[] Columns;
        // Toestand score
        public int Score;

        // Constructor voor grid object
        public Grid(int size)
        {
            this.Rows = new GridRC[size];
            this.Columns = new GridRC[size];
            this.Score = 0;
            for (int i = 0; i < 9; i++) {
                Rows[i] = new GridRC(9);
                Columns[i] = new GridRC(9);
            }

        }

        // Vul het grid met alle waardes in de lijst Values
        public void FillGrid(List<string> Values)
        {
            int V = 0;
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++) {
                    int T = Int32.Parse(Values[V]);
                    GridItem G = new GridItem(T);
                    Rows[j].RC[i] = G;
                    Columns[i].RC[j] = G;
                    V++;
                }

            }

        }


        // Evalueer de toestand in het begin
        public void StartEval() {
            this.Score = 0;
            foreach (GridRC R in Rows) {
                R.EvalRC();
                this.Score += R.Score;
            }

            foreach (GridRC C in Columns) {
                C.EvalRC();
                this.Score += C.Score;
            }

        }
    }
    
    // Class voor een rij / kolom van het grid
    // Rij / kolom bestaat uit 8 GridItems
    public class GridRC
    {
        // Score van de rij / kolom 
        public int Score;
        //Array van griditems
        public GridItem[] RC;

        public GridRC(int value)
        {
            this.RC = new GridItem[value];


        }

        // Functie om de rij / kolom score te berekenen
        public void EvalRC()
        {
            HashSet<int> HSet = new HashSet<int>(); 
            foreach (GridItem I in RC) {
                HSet.Add(I.Value);
            }

            if (RC.Length == HSet.Count) { this.Score = 0;} 
            this.Score = RC.Length - HSet.Count;

        }



    }

    // Class voor een GridItem
    // Een GridItem Bestaat uit een waarde
    public class GridItem
    {

        public int Value;

        public GridItem(int value)
        {
            this.Value = value;


        }
    }



}
