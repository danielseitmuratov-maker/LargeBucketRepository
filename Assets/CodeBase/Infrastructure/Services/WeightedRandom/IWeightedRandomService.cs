using System.Collections.Generic;

namespace _Root._Scripts.Infrastructure.Services.WeightedRandom
{
    public interface IWeightedRandomService
    {
        int GetWeightedRandom(int minValue, int maxValue, List<float> weights);
        int GetWeightedRandom(int minValue, int maxValue, float[] weights);
    }
}