using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Qck
{
    class Program
    {
        static void Main(string[] args)
        {
            string nk = Console.ReadLine();
            string[] splitCountP = nk.Split();
            int n = Int32.Parse(splitCountP[0]);
            int k = Int32.Parse(splitCountP[1]);
            if (k < 0) { k = 0; }
            List<Tuple<int, int>> Input = new List<Tuple<int, int>>();
            int i = 0;

            while (i < n)
            {
                string input = Console.ReadLine();
                string[] split = input.Split();
                Input.Add(new Tuple<int, int>(Int32.Parse(split[0]), Int32.Parse(split[1])));
                i += 1;
            }

            Sorts.SortAll(Input, 0, n - 1, k);
            Console.WriteLine(n);
            foreach (Tuple<int, int> t in Input)
            {
                Console.WriteLine(t.Item1 + " " + t.Item2);
            }
        }
    }

    public class Sorts
    {
        public static void SortAll(List<Tuple<int, int>> Input, int L, int R, int k)
        {
            if (R - L + 1 <= k)
            {
                Bubblesort(Input, L, R);
            }
            else
            {
                int pivot = Partition(Input, L, R);
                SortAll(Input, L, pivot - 1, k);
                SortAll(Input, pivot + 1, R, k);
            }
        }

        public static int Partition(List<Tuple<int, int>> Input, int L, int R)
        {
            Tuple<int, int> pivot = Input[R];
            int i = L - 1;
            for (int j = L; j <= R - 1; j++)
            {
                if (Functions.Compare(Input[j], pivot) == true)
                {
                    i = i + 1;
                    Tuple<int, int> TempT = new Tuple<int, int>(Input[i].Item1, Input[i].Item2);
                    Input[i] = new Tuple<int, int>(Input[j].Item1, Input[j].Item2);
                    Input[j] = TempT;
                }
            }
            Tuple<int, int> Temp = Input[i + 1];
            Input[i + 1] = pivot;
            Input[R] = Temp;
            return i + 1;
        }

        public static void Bubblesort(List<Tuple<int, int>> Input, int L, int R)
        {
            for (int i = L; i < R; i++)
            {
                for (int j = L; j < R - i + L; j++)
                {
                    if (Functions.Compare(Input[j + 1], Input[j]) == true)
                    {
                        Tuple<int, int> TempTuple = Input[j];
                        Input[j] = Input[j + 1];
                        Input[j + 1] = TempTuple;
                    }
                }
            }
        }
    }

    public class Functions
    {
        public static bool Compare(Tuple<int, int> one, Tuple<int, int> two)
        {
            long a = one.Item1;
            long b = two.Item1;
            long c = one.Item2;
            long d = two.Item2;
            if ((d * a) < (b * c)) { return true; }
            else if ((d * a) == (b * c))
            {
                if (c < d) { return true; }
                else { return false; }
            }
            else { return false; }
        }
    }
}
