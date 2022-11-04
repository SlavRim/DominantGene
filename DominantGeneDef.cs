namespace DominantGene;

partial class Definitions
{
    public static DominantGeneDef DominantGene;
}
public class DominantGeneDef : GeneDef
{
    public const string DefName = nameof(Defs.DominantGene);
    public DominantGeneDef()
    {
        defName = DefName;
        geneClass = typeof(DominantGene);
    }
}