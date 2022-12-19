using System.Globalization;
using _181213013_Hasan_Basri_Ayhaner.Entities;

namespace _181213013_Hasan_Basri_Ayhaner.Data;

public static class GetData
{
    public static async Task<List<Penguin>> GetPenguinListAsync(string filepath)
    {
        List<Penguin> penguins = new List<Penguin>();
        List<string> lines = await GetAllLinesAsList(filepath);

        // Console.WriteLine(lines[0]);

        for (int i = 1; i < lines.Count; i++)
        {
            var columns = lines[i].Split(",");
            if (columns.Any(x => x == "NA")) continue;
            penguins.Add(new()
            {
                Specy = columns[0],
                Island = columns[1],
                CulmenLengthMM = double.Parse(columns[2], CultureInfo.InvariantCulture),
                CulmenDepthMM = double.Parse(columns[3], CultureInfo.InvariantCulture),
                FlipperLengthMM = double.Parse(columns[4], CultureInfo.InvariantCulture),
                BodyMassG = int.Parse(columns[5]),
                Sex = columns[6]
            });
        }

        return penguins;
    }

    private static async Task<List<string>> GetAllLinesAsList(string filepath)
    {
        var x = await File.ReadAllLinesAsync(filepath);
        return x.ToList();
    }
}