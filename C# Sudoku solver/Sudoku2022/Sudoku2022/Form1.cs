using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;


namespace Sudoku2022
{
    public partial class Form1 : Form
    {
        public List<int> Itterations = new List<int>();
        public List<int> RandomWalks = new List<int>();
        public string CurrentProblem;
        public Solver S;
        public Grid GRID;
        public Form1()
        {
            InitializeComponent();
            this.Input.SelectedIndex = 0;
            this.RWalkSteps.SelectedIndex = 1;
            this.StopCrit.SelectedIndex = 1;
            this.comboBox1.SelectedIndex = 0;
            Intialize();
            this.Size = new Size(1400, 800);
            this.Paint += this.Draw;
        }
        // Functie voor het tekenen van het Grid + waarden van het grid
        public void Draw(object o, PaintEventArgs pea) {
            int offset = 50 / 3;
            int BlockSize = 750 / 9;
            Rectangle[,] Board = new Rectangle[9,9];
            Graphics G = pea.Graphics;
            Pen GridColor = new Pen(Color.Gray);
            for (int i = 0; i < 9; i++) {
                for (int j = 0; j < 9; j++) {
                    Board[i, j] = new Rectangle(offset + i * BlockSize, j * BlockSize, BlockSize,BlockSize);
                    G.DrawRectangle(GridColor,Board[i, j]);

                    using (Font F = new Font("Monoid", 14))
                    {
                        G.DrawRectangle(GridColor, Rectangle.Round(Board[i, j]));
                        TextRenderer.DrawText(G, GRID.Rows[i].RC[j].Value.ToString(), F, Board[i, j], Color.Black);
                    }

                }
            }
        }


        // Initialiseer alles in de Solver class en het Grid zelf
        public void Intialize() {
            this.S = new Solver();
            // Maak Grid
            GRID = new Grid(9);
            List<string> T = CurrentProblem.Split(' ').ToList();
            // Vul grid met begintoestand
            GRID.FillGrid(T);
            // Initialiseer Solver object met begintoestand
            S.Initialize(GRID);
            // Evalueer het grid op de begintoestand
            GRID.StartEval(); 

        }

        // Reset en initializeer alle waarden bij een nieuwe begintoestand
        private void button1_Click(object sender, EventArgs e)
        {
            Intialize();
            this.Invalidate();
        }

        private void InputBox(object sender, EventArgs e)
        {
            this.CurrentProblem = this.Input.Text;
        }

        // Solve button
        private void button2_Click(object sender, EventArgs e)
        {
            S.Solve(GRID,Int32.Parse(this.StopCrit.Text), Int32.Parse(this.RWalkSteps.Text));
            this.Invalidate();    
        }

        private void button3_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < Int32.Parse(comboBox1.Text); i++)
            {
                Intialize();
                Tuple<int, int> values = S.Solve(GRID, Int32.Parse(this.StopCrit.Text), Int32.Parse(this.RWalkSteps.Text));
                this.Itterations.Add(values.Item1);
                this.RandomWalks.Add(values.Item2);
                this.Invalidate();
            }

            Console.WriteLine(this.Itterations.Max());
            Console.WriteLine(this.Itterations.Min());
            Console.WriteLine(this.Itterations.Average());
            Console.WriteLine(this.RandomWalks.Average() * Int32.Parse(this.RWalkSteps.Text));

            //Reset lijsten
            this.Itterations = new List<int>();
            this.RandomWalks = new List<int>();
    }
    }
}

