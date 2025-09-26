using System;
using System.Diagnostics;
using System.Collections.Generic;

namespace ConsoleApp5
{
    class Program
    {
        static void Main(string[] args)
        {
            string countP = Console.ReadLine();
            string[] splitCountP = countP.Split();
            int countPlanes = Int32.Parse(splitCountP[0]);
            List<Tuple<int, int>> input = new List<Tuple<int, int>>();

            int i = 0;
            while (i < countPlanes)
            {
                string inputLine = Console.ReadLine();
                string[] split = inputLine.Split();
                input.Add(new Tuple<int, int>(Int32.Parse(split[0]), Int32.Parse(split[1])));
                i += 1;
            }

            Console.WriteLine(RecursiveMinDistance(SortByX(input), SortByY(input)));
        }

        public static List<Tuple<int, int>> SortByX(List<Tuple<int, int>> input)
        {
            input.Sort((x, y) => x.Item1.CompareTo(y.Item1));
            List<Tuple<int, int>> sortedByX = new List<Tuple<int, int>>(input);
            return sortedByX;
        }

        public static List<Tuple<int, int>> SortByY(List<Tuple<int, int>> input)
        {
            input.Sort((x, y) => x.Item2.CompareTo(y.Item2));
            List<Tuple<int, int>> sortedByY = new List<Tuple<int, int>>(input);
            return sortedByY;
        }

        public static int RecursiveMinDistance(List<Tuple<int, int>> sortedByX, List<Tuple<int, int>> sortedByY)
        {
            double m = sortedByX.Count / 2;

            if (sortedByX.Count <= 3)
            {
                double distance = int.MaxValue;
                for (int i = 0; i < sortedByX.Count; ++i)
                {
                    for (int j = i + 1; j < sortedByX.Count; ++j)
                    {
                        if (Distance(sortedByX[i].Item1, sortedByX[j].Item1, sortedByX[i].Item2, sortedByX[j].Item2) < distance)
                        {
                            distance = Distance(sortedByX[i].Item1, sortedByX[j].Item1, sortedByX[i].Item2, sortedByX[j].Item2);
                        }
                    }
                }

                return (int)distance;
            }
            else
            {
                Tuple<int, int> midden = sortedByX[(int)m];
                List<Tuple<int, int>> sortedByXLS = new List<Tuple<int, int>>(sortedByX.GetRange(0, (int)m));
                List<Tuple<int, int>> sortedByXRS = new List<Tuple<int, int>>(sortedByX.GetRange((int)m, (int)m));
                List<Tuple<int, int>> sortedByYLS = new List<Tuple<int, int>>();
                List<Tuple<int, int>> sortedByYRS = new List<Tuple<int, int>>();

                foreach (Tuple<int, int> tuple in sortedByY)
                {
                    if (tuple.Item1 <= midden.Item1)
                    {
                        sortedByYLS.Add(tuple);
                    }
                    else
                    {
                        sortedByYRS.Add(tuple);
                    }
                }

                int distL = RecursiveMinDistance(sortedByXLS, sortedByYLS);
                int distR = RecursiveMinDistance(sortedByXRS, sortedByYRS);
                int delta = Math.Min(distL, distR);

                List<Tuple<int, int>> inDelta = new List<Tuple<int, int>>();
                foreach (Tuple<int, int> tuple in sortedByY)
                {
                    if (midden.Item1 - delta < tuple.Item1)
                    {
                        inDelta.Add(tuple);
                    }
                    else if (tuple.Item1 < midden.Item1 + delta)
                    {
                        inDelta.Add(tuple);
                    }
                }

                for (int i = 0; i < inDelta.Count; i++)
                {
                    for (int j = i + 1; j < Math.Min(i + 7, inDelta.Count); j++)
                    {
                        double d = Distance(inDelta[i].Item1, inDelta[j].Item1, inDelta[i].Item2, inDelta[j].Item2);
                        if (d < delta)
                        {
                            delta = (int)d;
                        }
                    }
                }

                return delta;
            }
        }

        public static double Distance(int x1, int x2, int y1, int y2)
        {
            double d = Math.Pow((x2 - x1), 2) + Math.Pow((y2 - y1), 2);
            return d;
        }
    }
}