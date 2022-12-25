using System.Globalization;
using _181213013_Hasan_Basri_Ayhaner.Entities;

namespace _181213013_Hasan_Basri_Ayhaner.Data;

public class PenguinsData
{
    public List<string> Data { get; set; }
    public List<Penguin> PenguinList { get; set; }

    public PenguinsData()
    {
        Data = new List<string>();
        PenguinList = new List<Penguin>();
    }

    public async Task<List<Penguin>> GetPenguinListAsync(string filepath)
    {
        List<string> lines = await GetAllLinesAsList(filepath);

        for (int i = 1; i < lines.Count; i++)
        {
            var columns = lines[i].Split(",");
            if (columns.Any(x => x == "NA" || x == ".")) continue;
            Penguin penguin = new()
            {
                Specy = columns[0],
                Island = columns[1],
                CulmenLengthMM = double.Parse(columns[2], CultureInfo.InvariantCulture),
                CulmenDepthMM = double.Parse(columns[3], CultureInfo.InvariantCulture),
                FlipperLengthMM = double.Parse(columns[4], CultureInfo.InvariantCulture),
                BodyMassG = int.Parse(columns[5]),
                Sex = columns[6]
            };
            Data.Add(lines[i]);
            PenguinList.Add(penguin);
        }

        return PenguinList;
    }


    public async Task<Tuple<List<Penguin>, List<Penguin>>> SeperateTrainAndTestPenguins(float trainDataPercentile = 0.7f)
    {
        List<Penguin> trainData = new List<Penguin>();
        List<Penguin> testData = new List<Penguin>();

        await Task.Run(() =>
        {

            int trainDataCounter = Int32.Parse(Math.Round((trainDataPercentile * 100), 0).ToString());
            int counter = Int32.Parse(((Data.Count() * trainDataCounter) / 100).ToString());

            Random rnd = new Random();
            List<int> randomList = new List<int>();
            List<int> testListIndexes = Enumerable.Range(0, PenguinList.Count - 1).ToList();
            for (int i = 0; i < counter; i++)
            {
                bool isUsedBefore = true;
                int index = -1;
                while (isUsedBefore)
                {
                    index = rnd.Next(1, Data.Count);
                    if (!randomList.Contains(index))
                    {
                        isUsedBefore = false;
                        randomList.Add(index);
                    }
                }
                trainData.Add(PenguinList[index]);
            }

            testListIndexes.RemoveAll(x => randomList.Contains(x));

            foreach (var testIndex in testListIndexes)
            {
                testData.Add(PenguinList[testIndex]);
            }
        });

        trainData = trainData.OrderBy(x => x.Specy).ToList();
        testData = testData.OrderBy(x => x.Specy).ToList();
        return new(trainData, testData);
    }

    public async Task<Tuple<List<string>, List<string>>> SeperateTrainAndTestAndSave(string trainDataFilePath, string testDataFilePath, float trainDataPercentile = 0.7f)
    {
        List<string> trainData = new List<string>();
        List<string> testData = new List<string>();

        await Task.Run(() =>
        {

            int trainDataCounter = Int32.Parse(Math.Round((trainDataPercentile * 100), 0).ToString());
            int counter = Int32.Parse(((Data.Count() * trainDataCounter) / 100).ToString());

            Random rnd = new Random();
            List<int> randomList = new List<int>();
            List<int> testListIndexes = Enumerable.Range(0, Data.Count - 1).ToList();
            for (int i = 0; i < counter; i++)
            {
                bool isUsedBefore = true;
                int index = -1;
                while (isUsedBefore)
                {
                    index = rnd.Next(1, Data.Count);
                    if (!randomList.Contains(index))
                    {
                        isUsedBefore = false;
                        randomList.Add(index);
                    }
                }
                trainData.Add(Data[index]);
            }

            testListIndexes.RemoveAll(x => randomList.Contains(x));

            foreach (var testIndex in testListIndexes)
            {
                testData.Add(Data[testIndex]);
            }
        });

        trainData = trainData.Order().ToList();
        testData = testData.Order().ToList();

        if (trainDataFilePath != "")
        {
            await File.WriteAllLinesAsync(trainDataFilePath, trainData);
        }

        if (testDataFilePath != "")
        {
            await File.WriteAllLinesAsync(testDataFilePath, testData);
        }

        return new(trainData, testData);
    }

    private async Task<List<string>> GetAllLinesAsList(string filepath)
    {
        var x = await File.ReadAllLinesAsync(filepath);
        return x.ToList();
    }
}