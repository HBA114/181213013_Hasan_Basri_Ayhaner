using _181213013_Hasan_Basri_Ayhaner.Entities;

namespace _181213013_Hasan_Basri_Ayhaner.Clustering;

public class KMeansClustering
{
    public async Task KMeans(List<Penguin> trainPenguins)
    {
        int k = trainPenguins.Select(x => x.Specy).Distinct().Count();
        double E2 = 0;
        var splitKRandomList = new List<Penguin>(trainPenguins);
        var clusters = new List<PenguinCluster>();

        Random random = new Random();

        await Task.Run(() =>
        {
            for (int i = 0; i < k; i++)
            {
                clusters.Add(new()
                {
                    ClusterId = i,
                    Penguins = new List<Penguin>()
                });
            }

            int index = 0;
            while (splitKRandomList.Count > 1)
            {
                int randomNumber = random.Next(0, splitKRandomList.Count);
                var randomPenguin = splitKRandomList[randomNumber];
                clusters.Where(x => x.ClusterId == index).First().Penguins.Add(randomPenguin);
                splitKRandomList.RemoveAt(randomNumber);
                index++;
                index = index % k;
            }
        });
    }

    public class PenguinCluster
    {
        public int ClusterId { get; set; }
        public List<Penguin> Penguins { get; set; }

        public PenguinCluster()
        {
            Penguins = new List<Penguin>();
        }

        public PenguinCluster(int clusterId, List<Penguin> penguins)
        {
            ClusterId = clusterId;
            Penguins = penguins;
        }
    }
}