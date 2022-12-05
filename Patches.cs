using static DominantGene.Extensions;

namespace DominantGene;

[StaticConstructorOnStartup, Harmony]
public static class Patches
{
    static Patches() { Mod.TryPerformPatches(); }

    [HarmonyPatch]
    public static class GetInheritedGenes
    {
        public static MethodBase TargetMethod() => typeof(PregnancyUtility).GetMethods().Where(x => x.Name == nameof(GetInheritedGenes)).MaxBy(x => x.GetParameters().Length);
        public static void Postfix(ref List<GeneDef> __result, Pawn father, Pawn mother)
        {
            InheritGenes inherit = null;
            if (CanInheritParentDominantGenes(father, ref inherit) & CanInheritParentDominantGenes(mother, ref inherit))
                return;
            if (inherit is null) return;
            __result.Clear();
            inherit?.Invoke(__result);
        }
    }

    [HarmonyPatch(typeof(PregnancyUtility), nameof(TryGetInheritedXenotype))]
    [HarmonyPostfix]
    public static void TryGetInheritedXenotype(ref bool __result, Pawn mother, Pawn father, ref XenotypeDef xenotype)
    {
        dominantParent = null;
        InheritXenotype inherit = null;
        if (CanInheritParentDominantXenotype(mother, ref inherit) & CanInheritParentDominantXenotype(father, ref inherit))
            return;
        if (inherit is null) return;
        inherit?.Invoke(ref xenotype);
        __result = true;
    }

    [HarmonyPatch(typeof(Pawn_GeneTracker), nameof(SetXenotypeDirect))]
    [HarmonyPostfix]
    public static void SetXenotypeDirect(Pawn_GeneTracker __instance, ref XenotypeDef xenotype)
    {
        if (dominantParent is null) return;
        __instance.iconDef = dominantParent.genes.iconDef;
        __instance.xenotypeName = dominantParent.genes.xenotypeName;
    }
}