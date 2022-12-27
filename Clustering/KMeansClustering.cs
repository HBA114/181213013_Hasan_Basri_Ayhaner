using _181213013_Hasan_Basri_Ayhaner.Entities;

namespace _181213013_Hasan_Basri_Ayhaner.Clustering;

public class KMeansClustering
{
    public async Task KMeans(List<Penguin> trainPenguins)
    {
        int k = trainPenguins.Select(x => x.Specy).Distinct().Count();
        var splitKRandomList = new List<Penguin>(trainPenguins);
        double errorRate = 0;
        var clusters = new List<PenguinCluster>();

        var uniqueIslandTags = trainPenguins.Select(x => x.Island).Distinct().ToList();
        var uniqueSexTags = trainPenguins.Select(x => x.Sex).Distinct().ToList();

        Dictionary<string, int> islandEncoded = new Dictionary<string, int>();
        Dictionary<string, int> sexEncoded = new Dictionary<string, int>();

        int encoderIndex = 0;
        foreach (var tag in uniqueIslandTags)
        {
            islandEncoded.Add(tag!, encoderIndex);
            encoderIndex++;
        }
        encoderIndex = 0;
        foreach (var tag in uniqueSexTags)
        {
            sexEncoded.Add(tag!, encoderIndex);
            encoderIndex++;
        }

        Random random = new Random();

        await Task.Run(async () =>
        {
            //* Creating K clusters
            for (int i = 0; i < k; i++)
            {
                clusters.Add(new()
                {
                    ClusterId = i,
                    Center = new List<double>(),
                    Penguins = new List<Penguin>()
                });
            }

            //* Filling K clusters with random data
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

            // TODO: Calculate Every Clusters Center With Euclidian Formula
            var loop = true;
            while (loop)
            {
                foreach (var cluster in clusters)
                {
                    double islandMean = 0,
                            sexMean = 0,
                            culmenLenghtMean = 0,
                            culmenDephtMean = 0,
                            flipperLengthMean = 0,
                            bodyMassMean = 0;
                    foreach (var penguin in cluster.Penguins)
                    {
                        islandMean += islandEncoded[penguin.Island!];
                        sexMean += sexEncoded[penguin.Sex!];
                        culmenLenghtMean += penguin.CulmenLengthMM;
                        culmenDephtMean += penguin.CulmenDepthMM;
                        flipperLengthMean += penguin.FlipperLengthMM;
                        bodyMassMean += penguin.BodyMassG;
                    }
                    if (cluster.Penguins.Count > 0)
                    {
                        islandMean = islandMean / (double)cluster.Penguins.Count;
                        sexMean = sexMean / (double)cluster.Penguins.Count;
                        culmenLenghtMean = culmenLenghtMean / (double)cluster.Penguins.Count;
                        culmenDephtMean = culmenDephtMean / (double)cluster.Penguins.Count;
                        flipperLengthMean = flipperLengthMean / (double)cluster.Penguins.Count;
                        bodyMassMean = bodyMassMean / (double)cluster.Penguins.Count;
                    }
                    cluster.Center.Clear();
                    cluster.Center.Add(islandMean);
                    cluster.Center.Add(sexMean);
                    cluster.Center.Add(culmenLenghtMean);
                    cluster.Center.Add(culmenDephtMean);
                    cluster.Center.Add(flipperLengthMean);
                    cluster.Center.Add(bodyMassMean);
                }

                // TODO: Calculate Mean Error
                double ESum = 0;
                foreach (var cluster in clusters)
                {
                    ESum += await CalculateErrorRate(cluster, islandEncoded, sexEncoded);
                }

                if (errorRate != ESum)
                {
                    errorRate = ESum;
                }
                else
                {
                    loop = false;
                }


                Console.WriteLine($"E² Sum: {ESum}");

                foreach (var cluster in clusters)
                {
                    var penguins = new List<Penguin>(cluster.Penguins);
                    foreach (var penguin in penguins)
                    {
                        Dictionary<int, double> calculatedClusters = new Dictionary<int, double>();
                        for (int i = 0; i < clusters.Count; i++)
                        {
                            double value = 0;
                            value += Math.Pow(clusters[i].Center[0] - islandEncoded[penguin.Island!], 2);
                            value += Math.Pow(clusters[i].Center[1] - sexEncoded[penguin.Sex!], 2);
                            value += Math.Pow(clusters[i].Center[2] - penguin.CulmenLengthMM, 2);
                            value += Math.Pow(clusters[i].Center[3] - penguin.CulmenDepthMM, 2);
                            value += Math.Pow(clusters[i].Center[4] - penguin.FlipperLengthMM, 2);
                            value += Math.Pow(clusters[i].Center[5] - penguin.BodyMassG, 2);

                            value = Math.Sqrt(value);
                            double x = value;
                            calculatedClusters.Add(clusters[i].ClusterId, x);

                            // var temp = clusters.OrderBy(x => x.E2 - value).ToList();
                            // if (temp.First().ClusterId != cluster.ClusterId)
                            // {
                            //     clusters.Where(x => x.ClusterId == temp.First().ClusterId).First().Penguins.Add(penguin);
                            //     cluster.Penguins.Remove(penguin);
                            // }
                        }

                        calculatedClusters = calculatedClusters.OrderBy(x => x.Value).ToDictionary(y => y.Key, z => z.Value);
                        if (calculatedClusters.First().Key != cluster.ClusterId)
                        {
                            clusters.Where(x => x.ClusterId == calculatedClusters.First().Key).First().Penguins.Add(penguin);
                            cluster.Penguins.Remove(penguin);
                        }
                    }
                }
            }

            foreach (var cluster in clusters)
            {
                var speciesInCluster = cluster.Penguins.Select(x => x.Specy).Distinct().ToList();
                foreach (var specy in speciesInCluster)
                {
                    Console.WriteLine($"Specy: {specy}, Count: {cluster.Penguins.Where(x => x.Specy == specy).Count()}");
                }
                Console.WriteLine();
            }
        });
    }

    public async Task<double> CalculateErrorRate(PenguinCluster cluster, Dictionary<string, int> islandEncoded, Dictionary<string, int> sexEncoded)
    {
        double e = 0;
        await Task.Run(() =>
        {
            foreach (var penguin in cluster.Penguins)
            {
                e += Math.Pow(islandEncoded[penguin.Island!] - cluster.Center[0], 2);
                e += Math.Pow(sexEncoded[penguin.Sex!] - cluster.Center[1], 2);
                e += Math.Pow(penguin.CulmenLengthMM - cluster.Center[2], 2);
                e += Math.Pow(penguin.CulmenDepthMM - cluster.Center[3], 2);
                e += Math.Pow(penguin.FlipperLengthMM - cluster.Center[4], 2);
                e += Math.Pow(penguin.BodyMassG - cluster.Center[5], 2);
            }

            cluster.E2 = e;
        });

        return e;
    }

    public class PenguinCluster
    {
        public int ClusterId { get; set; }
        public double E2 { get; set; }
        public List<double> Center { get; set; }
        public List<Penguin> Penguins { get; set; }

        public PenguinCluster()
        {
            Center = new List<double>();
            Penguins = new List<Penguin>();
        }

        public PenguinCluster(int clusterId, double e2, List<double> center, List<Penguin> penguins)
        {
            ClusterId = clusterId;
            E2 = e2;
            Center = center;
            Penguins = penguins;
        }
    }
}