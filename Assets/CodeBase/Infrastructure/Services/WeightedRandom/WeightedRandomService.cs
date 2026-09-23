using System.Collections.Generic;
using UnityEngine;

namespace _Root._Scripts.Infrastructure.Services.WeightedRandom
{
    public class WeightedRandomService : IWeightedRandomService
    {
        public int GetWeightedRandom(
            int minValue,
            int maxValue,
            List<float> weights)
        {
            if (weights == null || weights.Count == 0)
            {
                Debug.LogWarning(
                    "[WeightedRandomService] Weights list is null or empty.");

                return Random.Range(minValue, maxValue + 1);
            }

            int expectedCount = maxValue - minValue + 1;

            if (weights.Count != expectedCount)
            {
                Debug.LogError(
                    $"[WeightedRandomService] Weights count " +
                    $"({weights.Count}) doesn't match range " +
                    $"({minValue}..{maxValue}).");

                return Random.Range(minValue, maxValue + 1);
            }

            float totalWeight = 0f;

            for (int i = 0; i < weights.Count; i++)
            {
                if (weights[i] < 0f)
                {
                    Debug.LogWarning(
                        $"[WeightedRandomService] Negative weight at index {i}. " +
                        "It will be treated as zero.");
                }

                totalWeight += Mathf.Max(0f, weights[i]);
            }

            if (totalWeight <= 0f)
            {
                Debug.LogError(
                    "[WeightedRandomService] Total weight must be greater than zero.");

                return Random.Range(minValue, maxValue + 1);
            }

            float randomValue = Random.Range(0f, totalWeight);
            float cumulativeWeight = 0f;

            for (int i = 0; i < weights.Count; i++)
            {
                cumulativeWeight += Mathf.Max(0f, weights[i]);

                if (randomValue < cumulativeWeight)
                    return minValue + i;
            }

            return maxValue;
        }

        public int GetWeightedRandom(
            int minValue,
            int maxValue,
            float[] weights)
        {
            if (weights == null || weights.Length == 0)
            {
                Debug.LogWarning(
                    "[WeightedRandomService] Weights array is null or empty.");

                return Random.Range(minValue, maxValue + 1);
            }

            return GetWeightedRandom(
                minValue,
                maxValue,
                new List<float>(weights));
        }
    }
}