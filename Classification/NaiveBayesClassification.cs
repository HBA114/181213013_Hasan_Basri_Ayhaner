using System.Globalization;
using System.Text;
using _181213013_Hasan_Basri_Ayhaner.Entities;
using _181213013_Hasan_Basri_Ayhaner.Helpers;

namespace _181213013_Hasan_Basri_Ayhaner.Classification;

public class NaiveBayesClassification
{
    // TODO: Doğruluk (Accuracy), Kesinlik (Precision), Hassasiyet (Recall) ve F Score metrikleri kullanılacaktır.
    private List<SpecyPossibility> modelData;

    public NaiveBayesClassification() => modelData = new List<SpecyPossibility>();

    public async Task ReadModelFromFile(string modelPath)
    {
        var data = await File.ReadAllLinesAsync(modelPath);
        List<string> lines = data.ToList();
        Dictionary<string, double> islandPossibility = new Dictionary<string, double>();
        List<string> islandKeys = new List<string>();
        Dictionary<string, double> sexPossibility = new Dictionary<string, double>();
        List<string> sexKeys = new List<string>();

        for (int i = 0; i < lines.Count; i++)
        {
            var columns = lines[i].Split(",").ToList();
            if (i == 0)
            {
                islandPossibility.Add(columns[1], 0);
                islandKeys.Add(columns[1]);
                islandPossibility.Add(columns[2], 0);
                islandKeys.Add(columns[2]);
                islandPossibility.Add(columns[3], 0);
                islandKeys.Add(columns[3]);

                sexPossibility.Add(columns[4], 0);
                sexKeys.Add(columns[4]);
                sexPossibility.Add(columns[5], 0);
                sexKeys.Add(columns[5]);
            }
            else
            {
                for (int j = 0; j < islandKeys.Count; j++)
                {
                    islandPossibility[islandKeys[j]] = double.Parse(columns[j + 1], CultureInfo.InvariantCulture);
                }

                for (int j = 0; j < sexKeys.Count; j++)
                {
                    sexPossibility[sexKeys[j]] = double.Parse(columns[j + 4], CultureInfo.InvariantCulture);
                }

                modelData.Add(new()
                {
                    SpecyName = columns[0],
                    IslandPossibility = new Dictionary<string, double>(islandPossibility),
                    SexPossibility = new Dictionary<string, double>(sexPossibility),
                    CulmenLengthMean = double.Parse(columns[6], CultureInfo.InvariantCulture),
                    CulmenLengthStd = double.Parse(columns[7], CultureInfo.InvariantCulture),
                    CulmenDepthMean = double.Parse(columns[8], CultureInfo.InvariantCulture),
                    CulmenDepthStd = double.Parse(columns[9], CultureInfo.InvariantCulture),
                    FlipperLengthMean = double.Parse(columns[10], CultureInfo.InvariantCulture),
                    FlipperLengthStd = double.Parse(columns[11], CultureInfo.InvariantCulture),
                    BodyMassMean = double.Parse(columns[12], CultureInfo.InvariantCulture),
                    BodyMassStd = double.Parse(columns[13], CultureInfo.InvariantCulture)
                });
            }
        }
    }
    public async Task TrainNaiveBayes(List<Penguin> trainPenguins, string savePath = "")
    {
        var species = trainPenguins.Select(x => x.Specy).Distinct().ToList();
        var islands = trainPenguins.Select(x => x.Island).Distinct().ToList();
        islands = islands.Order().ToList();
        await Task.Run(() =>
        {
            foreach (var specy in species)
            {
                int specyCount = specy!.Length;
                var specyValues = trainPenguins.Where(x => x.Specy == specy).ToList();

                Dictionary<string, double> islandPossibilities = new Dictionary<string, double>();
                Dictionary<string, double> sexPossibilities = new Dictionary<string, double>();

                // TODO: Calculate Island Possibilities
                double sumIslandPos = 0;
                foreach (var island in islands)
                {
                    var islandCount = specyValues.Where(x => x.Island == island).Count();
                    double islandPos = (double)islandCount / (double)specyCount;
                    sumIslandPos += islandPos;
                    islandPossibilities.Add(island!, islandPos);
                }

                foreach (var pos in islandPossibilities)
                {
                    islandPossibilities[pos.Key] = (double)pos.Value / (double)sumIslandPos;
                }

                islandPossibilities = islandPossibilities.OrderBy(x => x.Key).ToDictionary(y => y.Key, z => z.Value);
                // TODO: Calculate Sex Possibilities

                int maleCount = specyValues.Where(x => x.Sex == "MALE").Count();
                int femaleCount = specyValues.Where(x => x.Sex == "FEMALE").Count();

                double male = (double)maleCount / (double)specyCount;
                double female = (double)femaleCount / (double)specyCount;

                double malePos = (double)male / (double)(male + female);
                double femalePos = (double)female / (double)(male + female);

                sexPossibilities.Add("MALE", malePos);
                sexPossibilities.Add("FEMALE", femalePos);

                // TODO: Calculate Other Values Possibilities With Mean And Standart Deviation

                List<double> culmenLengthData = new List<double>();
                List<double> culmenDepthData = new List<double>();
                List<double> flipperLengthData = new List<double>();
                List<double> bodyMassData = new List<double>();

                foreach (Penguin penguin in specyValues)
                {
                    culmenLengthData.Add(penguin.CulmenLengthMM);
                    culmenDepthData.Add(penguin.CulmenDepthMM);
                    flipperLengthData.Add(penguin.FlipperLengthMM);
                    bodyMassData.Add(penguin.BodyMassG);
                }

                // TODO: Write Calculated Possibility to Model

                modelData.Add(new()
                {
                    SpecyName = specy,
                    IslandPossibility = islandPossibilities,
                    SexPossibility = sexPossibilities,
                    CulmenLengthMean = Calculations.Mean(culmenLengthData.ToArray()),
                    CulmenLengthStd = Calculations.StandartDeviation(culmenLengthData.ToArray()),
                    CulmenDepthMean = Calculations.Mean(culmenDepthData.ToArray()),
                    CulmenDepthStd = Calculations.StandartDeviation(culmenDepthData.ToArray()),
                    FlipperLengthMean = Calculations.Mean(flipperLengthData.ToArray()),
                    FlipperLengthStd = Calculations.StandartDeviation(flipperLengthData.ToArray()),
                    BodyMassMean = Calculations.Mean(bodyMassData.ToArray()),
                    BodyMassStd = Calculations.StandartDeviation(bodyMassData.ToArray())
                });
            }
        });

        if (savePath != "")
        {
            List<string> lines = new List<string>();
            StringBuilder line = new StringBuilder();
            line.Append("Specy,");
            foreach (var island in islands)
            {
                line.Append($"{island},");
            }
            line.Append("MALE,FEMALE,");
            line.Append("CulmenLengthMean,CulmenLengthStd,CulmenDepthMean,CulmenDepthStd,FlipperLengthMean,FlipperLengthStd,BodyMassMean,BodyMassStd");
            lines.Add(line.ToString());
            foreach (var item in modelData)
            {
                line.Clear();
                // TODO: Find a way to save model with island and sex possibilities
                line.Append($"{item.SpecyName},");
                foreach (var islandPos in item.IslandPossibility!)
                {
                    line.Append($"{islandPos.Value},");
                }
                foreach (var sexPos in item.SexPossibility!)
                {
                    line.Append($"{sexPos.Value},");
                }
                line.Append($"{item.CulmenLengthMean},{item.CulmenLengthStd},{item.CulmenDepthMean},{item.CulmenDepthStd},{item.FlipperLengthMean},{item.FlipperLengthStd},{item.BodyMassMean},{item.BodyMassStd}");
                lines.Add(line.ToString());
            }

            await File.WriteAllLinesAsync(savePath, lines);
        }
    }

    public async Task PredictPenguinSpecy(List<Penguin> testPenguins)
    {
        int truePredictionCount = 0;
        Dictionary<string, Dictionary<string, int>> confusionMatrix = new Dictionary<string, Dictionary<string, int>>();

        var species = testPenguins.Select(x => x.Specy).Distinct().ToList();

        foreach (var specy in species)
        {
            Dictionary<string, int> subDict = new Dictionary<string, int>();
            foreach (var subSpecy in species)
            {
                subDict.Add(subSpecy!, 0);
            }
            confusionMatrix.Add(specy!, new Dictionary<string, int>(subDict));
        }

        await Task.Run(() =>
        {
            foreach (var penguin in testPenguins)
            {
                Dictionary<string, double> specyPossibilities = new Dictionary<string, double>();
                foreach (var item in modelData)
                {
                    double value = 1;
                    value *= item.IslandPossibility[penguin.Island!];
                    value *= item.SexPossibility[penguin.Sex!];
                    value *= CalculateGaussianValue(item.CulmenLengthMean, item.CulmenLengthStd, penguin.CulmenLengthMM);
                    value *= CalculateGaussianValue(item.CulmenDepthMean, item.CulmenDepthStd, penguin.CulmenDepthMM);
                    value *= CalculateGaussianValue(item.FlipperLengthMean, item.FlipperLengthStd, penguin.FlipperLengthMM);
                    value *= CalculateGaussianValue(item.BodyMassMean, item.BodyMassStd, penguin.BodyMassG);
                    specyPossibilities.Add(item.SpecyName, value);
                }

                specyPossibilities = specyPossibilities.OrderByDescending(x => x.Value).ToDictionary(y => y.Key, z => z.Value);
                var predictedClassName = specyPossibilities.First().Key;
                if (predictedClassName == penguin.Specy)
                {
                    truePredictionCount++;
                }
                confusionMatrix[penguin.Specy!][predictedClassName] += 1;
            }
        });

        foreach (var specy in species)
        {
            var truePositives = confusionMatrix[specy!][specy!];
            var falsePositives = 0;
            var trueNegatives = 0;
            var falseNegatives = 0;

            foreach (var item in confusionMatrix)
            {
                if (item.Key == specy)
                {
                    foreach (var sub in confusionMatrix[item.Key])
                    {
                        if (sub.Key != specy!)
                            falseNegatives += sub.Value;
                    }
                }
                else
                {
                    foreach (var sub in confusionMatrix[item.Key])
                    {
                        if (sub.Key == specy!)
                            falsePositives += sub.Value;
                        else
                            trueNegatives += sub.Value;
                    }
                }
            }
            
            var precision = (double)truePositives / (double)(truePositives + falsePositives);
            var recall = (double)truePositives / (double)(truePositives + falseNegatives);
            var f_score = 2 * ((precision * recall) / (precision + recall));
            Console.WriteLine($"Specy: {specy}\n\tTP: {truePositives}, TN: {trueNegatives}, FP: {falsePositives}, FN: {falseNegatives}");
            Console.WriteLine($"\tPrecision: {Math.Round(precision, 2)}, Recall: {Math.Round(recall, 2)}, F-Score: {Math.Round(f_score, 2)}\n");
        }

        Console.WriteLine($"Naive Bayes Test Data Count: {testPenguins.Count}");
        Console.WriteLine($"Naive Bayes True Prediction Count: {truePredictionCount}");
        Console.WriteLine($"Naive Bayes Accuracy: % {Math.Round(((double)truePredictionCount * 100 / (double)testPenguins.Count), 2)}");
    }

    public double CalculateGaussianValue(double mean, double standartDeviation, double penguinValue)
    {
        double firstArea = standartDeviation * Math.Sqrt(2 * Math.PI);
        double secondArea = -Math.Pow((penguinValue - mean) / standartDeviation, 2) / 2;

        double value = 1 / firstArea *
                    Math.Pow(Math.E, secondArea);

        return value;
    }

}

public class SpecyPossibility
{
    public string SpecyName { get; set; }
    public Dictionary<string, double> IslandPossibility { get; set; }
    public Dictionary<string, double> SexPossibility { get; set; }
    public double CulmenLengthMean { get; set; }
    public double CulmenLengthStd { get; set; }
    public double CulmenDepthMean { get; set; }
    public double CulmenDepthStd { get; set; }
    public double FlipperLengthMean { get; set; }
    public double FlipperLengthStd { get; set; }
    public double BodyMassMean { get; set; }
    public double BodyMassStd { get; set; }

    public SpecyPossibility()
    {
        SpecyName = "";
        IslandPossibility = new Dictionary<string, double>();
        SexPossibility = new Dictionary<string, double>();
    }

    public SpecyPossibility(string specyName, Dictionary<string, double> islandPossibility, Dictionary<string, double> sexPossibility, double culmenLengthMean, double culmenLengthStd, double culmenDepthMean, double culmenDepthStd, double flipperLengthMean, double flipperLengthStd, double bodyMassMean, double bodyMassStd) : this()
    {
        SpecyName = specyName;
        IslandPossibility = islandPossibility;
        SexPossibility = sexPossibility;
        CulmenLengthMean = culmenLengthMean;
        CulmenLengthStd = culmenLengthStd;
        CulmenDepthMean = culmenDepthMean;
        CulmenDepthStd = culmenDepthStd;
        FlipperLengthMean = flipperLengthMean;
        FlipperLengthStd = flipperLengthStd;
        BodyMassMean = bodyMassMean;
        BodyMassStd = bodyMassStd;
    }
}