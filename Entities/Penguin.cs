namespace _181213013_Hasan_Basri_Ayhaner.Entities;

public class Penguin
{
    public string? Specy { get; set; }
    public string? Island { get; set; }
    public double CulmenLengthMM { get; set; }
    public double CulmenDepthMM { get; set; }
    public double FlipperLengthMM { get; set; }
    public int BodyMassG { get; set; }
    public string? Sex { get; set; }

    public Penguin()
    {
    }

    public Penguin(string specy, string island, double culmenLengthMM, double culmenDepthMM, double flipperLengthMM, int bodyMassG, string sex) : this()
    {
        Specy = specy;
        Island = island;
        CulmenLengthMM = culmenLengthMM;
        CulmenDepthMM = culmenDepthMM;
        FlipperLengthMM = flipperLengthMM;
        BodyMassG = bodyMassG;
        Sex = sex;
    }
}