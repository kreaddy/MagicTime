using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kingmaker.Blueprints;
using Kingmaker.Blueprints.Classes;
using Kingmaker.UnitLogic.FactLogic;
using MagicTime.Utilities;
using static MagicTime.Utilities.BlueprintsDatabase;

namespace MagicTime.Mythic
{
    static class MythicPoisons
    {
        private static BlueprintFeature mythic_poisons;

        public static void Create()
        {
            mythic_poisons = Helpers.CreateFeature(
                "MythicPoisons",
                "Mythic Poisons",
                "Your skill at creating poisons — whether mundane or magical — is nothing short of mythical. If bleeds, you can " +
                "poison it.\nBenefit: You ignore all poison immunities when using poisoned weapons or casting spells with the poison " +
                "descriptor, as long as the target is not undead or mechanical.", null, AssassinCreatePoison.Icon, false);

            PoisonImmunity.GetComponent<BuffDescriptorImmunity>().m_IgnoreFeature = mythic_poisons.ToReference<BlueprintUnitFactReference>();
            PoisonImmunity.GetComponent<SpellImmunityToSpellDescriptor>().m_CasterIgnoreImmunityFact =
                mythic_poisons.ToReference<BlueprintUnitFactReference>();
            SubtypeDemon.GetComponent<BuffDescriptorImmunity>().m_IgnoreFeature = mythic_poisons.ToReference<BlueprintUnitFactReference>();
            SubtypeDemon.GetComponent<SpellImmunityToSpellDescriptor>().m_CasterIgnoreImmunityFact =
                mythic_poisons.ToReference<BlueprintUnitFactReference>();
            SubtypeDemodand.GetComponent<BuffDescriptorImmunity>().m_IgnoreFeature = mythic_poisons.ToReference<BlueprintUnitFactReference>();
            SubtypeDemodand.GetComponent<SpellImmunityToSpellDescriptor>().m_CasterIgnoreImmunityFact =
                mythic_poisons.ToReference<BlueprintUnitFactReference>();
            Helpers.AddNewMythicAbility(mythic_poisons);
        }
    }
}
