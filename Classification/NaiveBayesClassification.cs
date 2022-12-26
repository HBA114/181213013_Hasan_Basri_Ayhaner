using System.Runtime.CompilerServices;
using System.Text;
using System.Security.Cryptography.X509Certificates;
using System.Globalization;
using _181213013_Hasan_Basri_Ayhaner.Entities;
using _181213013_Hasan_Basri_Ayhaner.Helpers;

namespace _181213013_Hasan_Basri_Ayhaner.Classification;

public class NaiveBayesClassification
{
    // TODO: Doğruluk (Accuracy), Kesinlik (Precision), Hassasiyet (Recall) ve F Score metrikleri kullanılacaktır.
    // private string _modelPath = "";
    private List<SpecyPossibility> modelData;

    public NaiveBayesClassification() => modelData = new List<SpecyPossibility>();

    #region Read Data    
    // private async Task ReadModel(string modelPath)
    // {
    //     string[] modelDataRaw = await File.ReadAllLinesAsync(modelPath);
    //     this.modelData = modelDataRaw.ToList();
    //     this.classCount = 1;
    //     string className = modelData[0].Split(",")[0];
    //     for (int i = 0; i < modelData.Count(); i++)
    //     {
    //         if (modelData[i].Split(",")[0] != className)
    //         {
    //             className = modelData[i].Split(",")[0];
    //             this.classCount++;
    //         }
    //     }
    // }
    #endregion

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
            line.Append("MALE,Female,");
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
            }
        }
    }

    #region old
    // private async Task TrainNaiveBayesModel(List<string> trainDataset, bool saveModel = false, string modelPath = "")
    // {
    //     _modelPath = modelPath;
    //     // her sınıf için ortalama ve standart sapma değerleri hesaplanır.
    //     // bir dosyaya burada karşılaşılan ger sınıf için gerekli veriler kaydedilir ve model kaydedilmiş olur.
    //     // modeli kaydetmek istenilmemesi durumunda String değişken içerisinde dosyada saklanacağı gibi saklanır.
    //     string className = "";
    //     List<List<double>> values = new List<List<double>>();
    //     for (int i = 1; i < trainDataset.Count(); i++)
    //     {
    //         List<string> columns = trainDataset[i].Split(",").ToList();
    //         if (columns[columns.Count() - 1] != className || i == trainDataset.Count() - 1)
    //         {
    //             classCount++;
    //             //! class changed make calculations and assign them to variable
    //             await CalculateAndWriteFromValuesAsync(values, className);

    //             // clearing list and updating className variable
    //             className = columns[columns.Count() - 1];
    //             values.Clear();
    //         }
    //         List<double> columnValues = new List<double>();
    //         for (int j = 0; j < columns.Count() - 1; j++)
    //         {
    //             columnValues.Add(double.Parse(columns[j], CultureInfo.InvariantCulture));
    //         }
    //         values.Add(columnValues);
    //     }
    // }
    #endregion


    #region Test
    // private double TestNaiveBayesModel(List<string> testDataset)
    // {
    //     // her sınıf için hesaplanan veriler kullanılarak test verisinin sınıfı tahmin edilmeli.
    //     // her tahmin doğruluğu veya yanlışlığı belirlenerek listelenmeli ve sonuç olarak doğruluk ölçüsü Accuracy gösterilmeli.
    //     //TODO test datasetin her bir satırı için :
    //     //TODO      modelData verisindeki her sınıfın her sütunu için olan verilerinden 
    //     //TODO      en yakın olanları hangisi ise onu seç ve test verisi içindeki sınıf ismi
    //     //TODO      ile karşılaştırarak doğruluk oranı (accuracy) hesaplamasını yap!

    //     int sumTrue = 0;
    //     for (int i = 1; i < testDataset.Count(); i++)
    //     {
    //         if (PredictBeanClass(testDataset[i]))
    //             sumTrue++;
    //     }

    //     System.Console.WriteLine("Naive Bayes True Prediction Count = " + sumTrue);
    //     System.Console.WriteLine("Naive Bayes Test Data Count = " + (testDataset.Count() - 1));
    //     // testDataSet.Count() - 1 because first line is contains column names
    //     double accuracy = (double)sumTrue / (double)(testDataset.Count() - 1);
    //     return accuracy;
    // }
    #endregion

    #region Calculation
    // private async Task CalculateAndWriteFromValuesAsync(List<List<double>> values, string className)
    // {
    //     if (values.Count() == 0) return;
    //     List<double> data;
    //     int j = 0;
    //     for (int i = 0; i < values[0].Count(); i++)
    //     {
    //         data = new List<double>();
    //         for (j = 0; j < values.Count(); j++)
    //         {
    //             data.Add(values[j][i]);
    //         }

    //         #region Test in Linux
    //         //! class data should be like "BARBUNYA,69846.580543933,10174.440752337463"
    //         string mean = Calculations.Mean(data.ToArray()).ToString().Replace(",", ".");
    //         string standartDeviation = Calculations.StandartDeviation(data.ToArray()).ToString().Replace(",", ".");
    //         #endregion

    //         string classData = "";
    //         classData += className + "," + mean + "," + standartDeviation;
    //         modelData.Add(classData);
    //     }

    //     if (_modelPath != "" || _modelPath != null)
    //     {
    //         await File.WriteAllLinesAsync(_modelPath!, modelData);
    //     }
    // }
    #endregion

    #region Prediction
    //TODO: Change To Penguin
    // private bool PredictBeanClass(string beanData)
    // {
    //     int dataCountForClass = modelData.Count() / classCount;
    //     List<string> beanDataColumns = beanData.Split(",").ToList();
    //     List<Dictionary<string, double>> possibilities = new List<Dictionary<string, double>>();
    //     for (int i = 0; i < dataCountForClass; i++)
    //     {
    //         Dictionary<string, double> pos = new Dictionary<string, double>();
    //         for (int j = i; j < modelData.Count(); j += dataCountForClass)
    //         {
    //             List<string> data = modelData[j].Split(",").ToList();

    //             double mean, standartDeviation, beanVal;

    //             mean = double.Parse(data[1], CultureInfo.InvariantCulture);
    //             standartDeviation = double.Parse(data[2], CultureInfo.InvariantCulture);

    //             beanVal = double.Parse(beanDataColumns[i], CultureInfo.InvariantCulture);


    //             double firstArea = standartDeviation * Math.Sqrt(2 * Math.PI);
    //             double secondArea = -Math.Pow((beanVal - mean) / standartDeviation, 2) / 2;

    //             double value = 1 / firstArea *
    //                         Math.Pow(Math.E, secondArea);
    //             pos.Add(data[0], value);
    //         }
    //         possibilities.Add(pos);
    //     }

    //     // System.Console.WriteLine(possibilities);
    //     List<Dictionary<string, double>> percentiles = new List<Dictionary<string, double>>();
    //     foreach (var possibility in possibilities)
    //     {
    //         Dictionary<string, double> percentilePerPos = new Dictionary<string, double>();
    //         int count = 0;
    //         double sum = 0;
    //         double classPossibility = 0;
    //         foreach (var pos in possibility)
    //         {
    //             count++;
    //             sum += pos.Value;
    //         }
    //         foreach (var pos in possibility)
    //         {
    //             classPossibility = (pos.Value / sum);
    //             percentilePerPos.Add(pos.Key, classPossibility);
    //         }
    //         percentiles.Add(percentilePerPos);
    //     }

    //     Dictionary<string, double> finalPercentile = new Dictionary<string, double>();
    //     foreach (var percentile in percentiles)
    //     {
    //         foreach (var per in percentile)
    //         {
    //             if (!finalPercentile.ContainsKey(per.Key))
    //                 finalPercentile.Add(per.Key, per.Value / 16);
    //             else
    //                 finalPercentile[per.Key] += per.Value / 16;
    //         }
    //     }

    //     finalPercentile = finalPercentile.OrderByDescending(x => x.Value).ToDictionary(x => x.Key, x => x.Value);

    //     string predictedClass = finalPercentile.First().Key;
    //     return predictedClass.Equals(beanDataColumns[beanDataColumns.Count() - 1]);
    // }
    #endregion
}

public class SpecyPossibility
{
    public string? SpecyName { get; set; }
    public Dictionary<string, double>? IslandPossibility { get; set; }
    public Dictionary<string, double>? SexPossibility { get; set; }
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
    }
}