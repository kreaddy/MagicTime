using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kingmaker.Blueprints;
using Kingmaker.Blueprints.Classes;
using Kingmaker.Blueprints.Classes.Selection;
using Kingmaker.EntitySystem.Stats;
using Kingmaker.Enums;
using Kingmaker.UnitLogic.Mechanics.Components;
using MagicTime.Utilities;

namespace MagicTime.Mythic
{
    static class WarriorPriest
    {
        private static BlueprintFeature warrior_priest;

        public static void Create()
        {
            warrior_priest = Helpers.CreateFeature(
                "MythicWarriorPriest",
                "Warrior Priest (Mythic)",
                "Your faith speeds you in battle and further strengthens your mind and confidence.\nBenefit: You gain a bonus equal to half your " +
                "mythic rank both on initiative checks and on concentration checks to cast a spell or use a spell-like ability when casting " +
                "defensively. These bonuses stack with the bonuses from Warrior Priest.", null, BlueprintsDatabase.WarriorPriest.Icon, false);
            warrior_priest.CreateFeatureRestriction(BlueprintsDatabase.WarriorPriest);
            warrior_priest.CreateFeatureTags(FeatureTag.Magic & FeatureTag.Defense);
            warrior_priest.CreateContextRankConfigMythicRank(ContextRankProgression.OnePlusDiv2);
            warrior_priest.CreateAddStatBonusContext(StatType.Initiative, ModifierDescriptor.UntypedStackable, AbilityRankType.Default);
            warrior_priest.CreateConcentrationBonus(0, false, Kingmaker.UnitLogic.Mechanics.ContextValueType.Rank);

            BlueprintsDatabase.WarriorPriest.IsPrerequisiteFor.Add(warrior_priest.ToReference<BlueprintFeatureReference>());

            Helpers.AddNewMythicFeat(warrior_priest);
        }
    }
}
