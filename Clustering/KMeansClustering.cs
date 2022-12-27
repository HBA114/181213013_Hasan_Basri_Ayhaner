using _181213013_Hasan_Basri_Ayhaner.Entities;

namespace _181213013_Hasan_Basri_Ayhaner.Clustering;

public class KMeansClustering
{
    public async Task KMeans(List<Penguin> trainPenguins)
    {
        int k = trainPenguins.Select(x => x.Specy).Distinct().Count();

        await Task.Run(()=>{});
    }
}