// See https://aka.ms/new-console-template for more information
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

var penguins = await GetData.GetPenguinListAsync(filePath);

Console.WriteLine(penguins.Count);