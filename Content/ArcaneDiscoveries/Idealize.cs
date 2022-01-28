using HarmonyLib;
using Kingmaker.Blueprints.Classes;
using Kingmaker.Blueprints.Classes.Spells;
using Kingmaker.Enums;
using Kingmaker.UnitLogic;
using Kingmaker.UnitLogic.FactLogic;
using MagicTime.Utilities;

namespace MagicTime.ArcaneDiscoveries
{
    internal static class Idealize
    {
        public static BlueprintFeature idealize_feature;

        public static void Create()
        {
            idealize_feature = Helpers.CreateFeature(
                "ADIdealize",
                "Idealize",
                "In your quest for self-perfection, you have discovered a way to further enhance yourself and others.\n" +
                "Benefit: When a transmutation spell you cast grants an enhancement bonus to an ability score, that bonus increases by 2. " +
                "At 20th level, the bonus increases by 4.",
                null, DB.GetFeature("Powerful Change Feature").Icon);
            idealize_feature.CreateClassLevelRestriction(DB.GetClass("Wizard Class"), 10);
            idealize_feature.CreateFeatureRestrictionInv(DB.GetFeature("Sin Magic Lust"));
            idealize_feature.CreateFeatureRestrictionInv(DB.GetFeature("Sin Magic Pride"));
            Main.AddNewDiscovery(idealize_feature);
        }
    }

    [HarmonyPatch(typeof(AddStatBonus), "OnTurnOn")]
    internal class AddStatBonus_OnTurnOn_IdealizePatch
    {
        private static void Postfix(AddStatBonus __instance)
        {
            if (__instance.Owner.HasFact(Idealize.idealize_feature) == false)
            {
                return;
            }
            if (__instance.Descriptor == ModifierDescriptor.Enhancement && __instance.Context.SourceAbility.IsSpell &&
                __instance.Context.SpellSchool == SpellSchool.Transmutation)
            {
                var wiz_lv = __instance.Owner.Progression.GetClassLevel(DB.GetClass("Wizard Class"));
                var new_bonus = wiz_lv > 19 ? __instance.Value + 4 : __instance.Value + 2;
                __instance.Owner.Stats.GetStat(__instance.Stat).RemoveModifiersFrom(__instance.Runtime);
                __instance.Owner.Stats.GetStat(__instance.Stat).AddModifierUnique(new_bonus, __instance.Runtime, ModifierDescriptor.Enhancement);
            }
        }
    }
}