using Kingmaker.Blueprints;
using Kingmaker.Blueprints.Classes;
using Kingmaker.UnitLogic.FactLogic;
using MagicTime.Utilities;

namespace MagicTime.Mythic
{
    internal static class MythicPoisons
    {
        private static BlueprintFeature mythic_poisons;

        public static void Create()
        {
            mythic_poisons = Helpers.CreateFeature(
                "MythicPoisons",
                "Mythic Poisons",
                "Your skill at creating poisons — whether mundane or magical — is nothing short of mythical. If bleeds, you can " +
                "poison it.\nBenefit: You ignore all poison immunities when using poisoned weapons or casting spells with the poison " +
                "descriptor, as long as the target is not undead or mechanical.", null, DB.GetAbility("Assassin Poison").Icon, false);

            DB.GetFeature("Poison Immunity").GetComponent<BuffDescriptorImmunity>().m_IgnoreFeature = mythic_poisons.ToReference<BlueprintUnitFactReference>();
            DB.GetFeature("Poison Immunity").GetComponent<SpellImmunityToSpellDescriptor>().m_CasterIgnoreImmunityFact =
                mythic_poisons.ToReference<BlueprintUnitFactReference>();
            DB.GetFeature("Subtype Demon").GetComponent<BuffDescriptorImmunity>().m_IgnoreFeature = mythic_poisons.ToReference<BlueprintUnitFactReference>();
            DB.GetFeature("Subtype Demon").GetComponent<SpellImmunityToSpellDescriptor>().m_CasterIgnoreImmunityFact =
                mythic_poisons.ToReference<BlueprintUnitFactReference>();
            DB.GetFeature("Subtype Demodand").GetComponent<BuffDescriptorImmunity>().m_IgnoreFeature = mythic_poisons.ToReference<BlueprintUnitFactReference>();
            DB.GetFeature("Subtype Demodand").GetComponent<SpellImmunityToSpellDescriptor>().m_CasterIgnoreImmunityFact =
                mythic_poisons.ToReference<BlueprintUnitFactReference>();
            Helpers.AddNewMythicAbility(mythic_poisons);
        }
    }
}