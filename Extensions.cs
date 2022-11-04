namespace DominantGene;

public static partial class Extensions
{
    public static List<Gene> GetEndogenes(this Pawn pawn) => GetGenes(pawn, p => p?.genes?.Endogenes);
    public static List<Gene> GetXenogenes(this Pawn pawn) => GetGenes(pawn, p => p?.genes?.Xenogenes);
    public static List<Gene> GetGenes(this Pawn pawn, Func<Pawn, List<Gene>> getter) => (getter(pawn) ?? Enumerable.Empty<Gene>()).ToList();
    public static bool HasDominantGene(this IEnumerable<GeneDef> genes) => genes.Any(x => x == Defs.DominantGene);
    public static bool HasDominantGene(this Pawn pawn, out List<Gene> endoGenes, out List<Gene> xenoGenes) => (endoGenes = pawn.GetEndogenes()).Concat(xenoGenes = pawn.GetXenogenes()).Select(x => x.def).HasDominantGene();
    public delegate void InheritGenes(List<GeneDef> genes);
    public delegate void InheritXenotype(ref XenotypeDef xenotype);
    public static bool CanInheritParentDominantGenes(Pawn parent, ref InheritGenes inherit)
    {
        if (parent is null) return false;
        var result = parent.HasDominantGene(out var parentEndogenes, out _);
        if (result)
            inherit ??= genes => {
                foreach (var parentGene in parentEndogenes)
                    genes.AddDistinct(parentGene.def);
            };
#if DEBUG
        Log.Message($"{parent.NameFullColored} genes {result}");
#endif
        return result;
    }
    public static bool CanInheritParentDominantXenotype(Pawn parent, ref InheritXenotype inherit)
    {
        if (parent is null) return false;
        var type = parent.genes?.Xenotype;
        //if (type is not { inheritable: true }) return false;
        var result = parent.HasDominantGene(out var _, out var _);
        if (result)
        {
            inherit ??= (ref XenotypeDef xenotype) => xenotype = type;
            dominantParent = parent;
        }
#if DEBUG
        Log.Message($"{parent.NameFullColored} xenotype {result}: {type.defName}");
#endif
        return result;
    }
    internal static Pawn dominantParent;
}