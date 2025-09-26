using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;

namespace Schijven
{
    class Program
    {
        static void Main(string[] args)
        {
            string s = Console.ReadLine();
            string[] S = s.Split();
            long AantalStdB = Int64.Parse(S[0]);
            List<long> sizeStdB = new List<long>();
            sizeStdB.Add(0);

            long a = 0;
            while (a < AantalStdB)
            {
                string ReadValue = Console.ReadLine();
                string[] SplitValue = ReadValue.Split();
                long V = Int64.Parse(SplitValue[0]);
                sizeStdB.Add(V);
                a += 1;
            }
            sizeStdB.TrimExcess();

            string z = Console.ReadLine();
            string[] Z = z.Split();
            long AantalSteunB = Int64.Parse(Z[0]);
            List<long> sizeSteunB = new List<long>();

            long b = 0;
            while (b < AantalSteunB)
            {
                string ReadSteunBvalue = Console.ReadLine();
                string[] SplitSteunBvalue = ReadSteunBvalue.Split();
                long val = Int64.Parse(SplitSteunBvalue[0]);
                sizeSteunB.Add(val);
                b += 1;
            }
            sizeSteunB.TrimExcess();

            long sum = 0;
            foreach (long steunB in sizeSteunB)
            {
                sum += Functions.calcDisks(sizeStdB, steunB);
            }
            Console.WriteLine(sum);
        }
    }

    class Functions
    {
        public static long binarySearch(List<long> l, long v)
        {
            bool t = true;
            long r = 0;
            long i = 0;
            long j = l.Count - 1;

            while (t)
            {
                int m = calcMiddle(i, j);
                if (i == j)
                {
                    t = false;
                    r = l[m];
                    break;
                }

                if (v < l[m])
                {
                    j = m - 1;
                }
                else
                {
                    i = m;
                }
            }

            return r;
        }

        public static int calcMiddle(long i, long j)
        {
            int m = (int)Math.Ceiling((decimal)(i + j) / 2);
            return m;
        }

        public static long calcDisks(List<long> l, long val)
        {
            long StdBlok = binarySearch(l, val);
            long res = val - StdBlok;

            if (res < 0)
            {
                res = val;
            }

            return res;
        }
    }
}



