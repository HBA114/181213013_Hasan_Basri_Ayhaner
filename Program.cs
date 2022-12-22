using _181213013_Hasan_Basri_Ayhaner.Data;

Console.WriteLine("Hello, World!");


string datasetCSVFilePath = "Data/penguins_size.csv";

#region Path Combine
string projectDirectory = Environment.CurrentDirectory;
string pathVariable = "";
if (projectDirectory.Contains("bin"))
    projectDirectory = projectDirectory.Split("bin")[0];

if (!projectDirectory.Contains("181213013_Hasan_Basri_Ayhaner"))
    pathVariable = "181213013_Hasan_Basri_Ayhaner";

string filePath = Path.Combine(projectDirectory, pathVariable, datasetCSVFilePath);
#endregion

System.Console.WriteLine(datasetCSVFilePath);
System.Console.WriteLine(filePath);

var penguinsData = new PenguinsData();

var penguins = await penguinsData.GetPenguinListAsync(filePath);

var seperatedRawData = await penguinsData.SeperateTrainAndTestPenguins();

var trainData = seperatedRawData.Item1;
var testData = seperatedRawData.Item2;

Console.WriteLine($"Train Data: {trainData.Count}, Test Data: {testData.Count}");