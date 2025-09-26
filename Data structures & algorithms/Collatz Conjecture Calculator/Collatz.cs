using System;
using System.Collections.Generic;

namespace CollatzConjecture
{
    class Program
    {
        static void Main(string[] args)
        {
            string input = Console.ReadLine();
            string[] parts = input.Split();
            int numberCount = int.Parse(parts[0]);

            List<ulong> values = new List<ulong>();

            for (int i = 0; i < numberCount; i++)
            {
                string valueInput = Console.ReadLine();
                string[] valueParts = valueInput.Split();
                ulong value = ulong.Parse(valueParts[0]);
                values.Add(value);
            }

            foreach (ulong value in values)
            {
                Console.WriteLine(CollatzCalculator.CalculateSteps(value));
            }
        }
    }

    public static class CollatzCalculator
    {
        public static ulong CalculateSteps(ulong number)
        {
            ulong steps = 0;

            while (number != 1)
            {
                if (number % 2 == 0)
                {
                    number = number / 2;
                }
                else
                {
                    number = (3 * number) + 1;
                }
                steps++;
            }

            return steps;
        }
    }
}