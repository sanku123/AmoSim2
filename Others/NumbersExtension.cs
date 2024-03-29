﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmoSim2.Others
{
    public static class NumberExtensions
    {
        public static T Pick<T>(this Random random, Dictionary<T, double> elementToProbability)
        {
            var totalProbability = elementToProbability.Values.Sum();
            var randomValue = random.NextDouble() * totalProbability;

            foreach (var keyValuePair in elementToProbability)
            {
                if (randomValue < keyValuePair.Value)
                    return keyValuePair.Key;

                randomValue -= keyValuePair.Value;
            }
            return default(T);
        }
    }
}