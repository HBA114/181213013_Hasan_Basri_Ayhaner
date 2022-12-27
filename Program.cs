using _181213013_Hasan_Basri_Ayhaner.Classification;
using _181213013_Hasan_Basri_Ayhaner.Clustering;
using _181213013_Hasan_Basri_Ayhaner.Data;

string datasetFilePath = "Data/penguins_size.csv";
string saveModelFilePath = "Data/nb_model.csv";

#region Path Combine
string projectDirectory = Environment.CurrentDirectory;
string pathVariable = "";
if (projectDirectory.Contains("bin"))
    projectDirectory = projectDirectory.Split("bin")[0];

if (!projectDirectory.Contains("181213013_Hasan_Basri_Ayhaner"))
    pathVariable = "181213013_Hasan_Basri_Ayhaner";

string filePath = Path.Combine(projectDirectory, pathVariable, datasetFilePath);
string saveFilePath = Path.Combine(projectDirectory, pathVariable, saveModelFilePath);
#endregion

// System.Console.WriteLine(datasetFilePath);
// System.Console.WriteLine(filePath);

var penguinsData = new PenguinsData();

var penguins = await penguinsData.GetPenguinListAsync(filePath);

var seperatedData = await penguinsData.SeperateTrainAndTestPenguins();

var trainPenguins = seperatedData.Item1;
var testPenguins = seperatedData.Item2;

Console.WriteLine($"Train Data: {trainPenguins.Count}, Test Data: {testPenguins.Count}\n");

NaiveBayesClassification naiveBayes = new NaiveBayesClassification();

// await naiveBayes.TrainNaiveBayes(trainPenguins, saveFilePath);
await naiveBayes.ReadModelFromFile(saveFilePath);

await naiveBayes.PredictPenguinSpecy(testPenguins);

KMeansClustering kMeansClustering = new KMeansClustering();

await kMeansClustering.KMeans(trainPenguins);