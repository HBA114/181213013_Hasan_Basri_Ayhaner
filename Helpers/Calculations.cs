namespace _181213013_Hasan_Basri_Ayhaner.Helpers;

public static class Calculations
{
    public static double Mean(double[] values)
    {
        double mean = 0;
        int i = 0;

        while (i < values.Count())
        {
            mean += values[i] / (double)values.Count();
            i++;
        }

        return mean;
    }

    public static double StandartDeviation(double[] values)
    {
        double sum = 0;
        double mean = Mean(values);
        int count = values.Count() - 1;

        foreach (double number in values)
        {
            sum += Math.Pow((number - mean), 2);
        }
        sum /= count;

        return Math.Sqrt(sum);
    }
}