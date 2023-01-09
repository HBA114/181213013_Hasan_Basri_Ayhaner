using System.Text;
using _181213013_Hasan_Basri_Ayhaner.Classification;
using _181213013_Hasan_Basri_Ayhaner.Clustering;
using _181213013_Hasan_Basri_Ayhaner.Data;
using _181213013_Hasan_Basri_Ayhaner.Entities;

#region Path Define
string datasetFilePathRaw = "Data/penguins_size.csv";
string saveModelFilePathRaw = "Data/nb_model.csv";
#endregion

#region Path Combine
string projectDirectory = Environment.CurrentDirectory;
string pathVariable = "";
if (projectDirectory.Contains("bin"))
    projectDirectory = projectDirectory.Split("bin")[0];

if (!projectDirectory.Contains("181213013_Hasan_Basri_Ayhaner"))
    pathVariable = "181213013_Hasan_Basri_Ayhaner";

string datasetFilePath = Path.Combine(projectDirectory, pathVariable, datasetFilePathRaw);
string saveModelFilePath = Path.Combine(projectDirectory, pathVariable, saveModelFilePathRaw);
#endregion

#region Creating Instances
var penguinsData = new PenguinsData();
StringBuilder sb = new StringBuilder();
#endregion


#region Data Operations
var penguins = await penguinsData.GetPenguinListAsync(datasetFilePath);
Tuple<List<Penguin>, List<Penguin>>? seperatedData = null;
List<Penguin>? trainPenguins = null, testPenguins = null;
#endregion


// await naiveBayes.TrainNaiveBayes(trainPenguins, saveModelFilePath);
// await naiveBayes.ReadModelFromFile(saveModelFilePath);

// await naiveBayes.PredictPenguinSpecy(testPenguins);

// await kMeansClustering.KMeans(penguins);

List<string> baseOperations = new() { "Data Operations", "Naive Bayes Operations", "KMeans", "Exit" };
List<string> dataOperations = new() { "Seperate Train And Test Data" };
List<string> naiveBayesOperations = new() { "Train New Model and Run Test", "Run Test With Saved Model" };
List<string> kmeansOperations = new() { "Run KMeans Test" };

bool exit = false;
Console.Clear();
while (!exit)
{
    #region Choose Operation
    int baseOperation = 0;

    sb.Remove(0, sb.Length);

    for (int i = 0; i < baseOperations.Count; i++)
    {
        sb.Append((i + 1) + ": " + baseOperations[i] + "\n");
    }
    while (true)
    {
        Console.WriteLine(sb.ToString());
        Console.WriteLine("Please enter number...");
        string? input = Console.ReadLine();
        Int32.TryParse(input, out baseOperation);
        if (baseOperation < 1 || baseOperation > baseOperations.Count)
            continue;
        else
        {
            if ((trainPenguins == null || testPenguins == null) && baseOperation != 1 && baseOperation != 4)
            {
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("There is no Train And Test Data. Please select one of Data Operation options.");
                Console.ForegroundColor = ConsoleColor.White;
                continue;
            }
            break;
        };
    }
    #endregion

    switch (baseOperation)
    {
        case 1:
            #region Seperate Data or Read from Saved
            int selectedDataOperation = 0;
            sb.Remove(0, sb.Length);    // resetting string builder

            // Adding current options to string builder
            for (int i = 0; i < dataOperations.Count; i++)
            {
                sb.Append((i + 1) + ": " + dataOperations[i] + "\n");
            }

            while (true)
            {
                Console.WriteLine(sb.ToString());
                Console.WriteLine("Please enter number...");
                string? input = Console.ReadLine();
                Int32.TryParse(input, out selectedDataOperation);
                if (selectedDataOperation < 1 || selectedDataOperation > dataOperations.Count)
                    continue;
                else break;
            }

            if (selectedDataOperation == 1)
            {

                //! if you do not want to save train and test data:
                //! Use SeperateTrainAndTest method with only 1 argument trainDataPercentile
                //! Warning: If you do not have train and test data files as csv uncomment next line
                seperatedData = await penguinsData.SeperateTrainAndTestPenguins();
            }
            //! else read seperated data and assign to penguin list
            trainPenguins = seperatedData?.Item1;
            testPenguins = seperatedData?.Item2;
            #endregion
            break;

        case 2:
            #region Train Naive Bayes or Read From Saved Model and Test
            NaiveBayesClassification naiveBayes = new NaiveBayesClassification();
            int selectedNaiveBayesOperation = 0;
            sb.Remove(0, sb.Length);    // resetting string builder

            // Adding current options to string builder
            for (int i = 0; i < naiveBayesOperations.Count; i++)
            {
                sb.Append((i + 1) + ": " + naiveBayesOperations[i] + "\n");
            }

            while (true)
            {
                Console.WriteLine(sb.ToString());
                Console.WriteLine("Please enter number...");
                string? input = Console.ReadLine();
                Int32.TryParse(input, out selectedNaiveBayesOperation);
                if (selectedNaiveBayesOperation < 1 || selectedNaiveBayesOperation > naiveBayesOperations.Count)
                    continue;
                else break;
            }

            if (selectedNaiveBayesOperation == 1)
            {
                //! in case you have not saved Naive Bayes model then you should train algorithm with function below (uncomment next line for train)
                await naiveBayes.TrainNaiveBayes(trainPenguins!, saveModelFilePath);
            }
            else
            {
                //! if you save the model file you can read model with:
                //! if you do not have any model saved: comment next line and run train:
                await naiveBayes.ReadModelFromFile(saveModelFilePath);
            }

            await naiveBayes.PredictPenguinSpecy(testPenguins!);

            #endregion
            Console.WriteLine("Press enter to continue...");
            Console.ReadLine();
            break;

        case 3:
            #region KMeans
            KMeansClustering kMeansClustering = new KMeansClustering();
            await kMeansClustering.KMeans(penguins);
            #endregion
            Console.WriteLine("Press enter to continue...");
            Console.ReadLine();
            break;

        case 4:
            exit = true;
            break;

        default:
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Invalid input! Press enter and try again please...");
            Console.ForegroundColor = ConsoleColor.White;
            Console.ReadLine();
            break;
    }
    if (!exit)
        Console.Clear();
}