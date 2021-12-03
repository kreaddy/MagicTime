using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kingmaker.Blueprints.Classes;
using Kingmaker.Enums;
using Kingmaker.UnitLogic.Abilities.Blueprints;
using Kingmaker.UnitLogic.Buffs.Blueprints;
using Kingmaker.UnitLogic.Mechanics;
using Kingmaker.UnitLogic.Mechanics.Components;
using MagicTime.Utilities;
using static MagicTime.Utilities.BlueprintsDatabase;

namespace MagicTime.MagusArcana
{
    static class CircleOfOrder
    {
        private static BlueprintFeature circle_feature;
        private static BlueprintAbility circle_ability;
        private static BlueprintBuff circle_buff;

        public static void Create()
        {
            circle_buff = Helpers.CreateBuff(
                "CircleOfOrderBuff",
                "Circle of Order",
                "As a swift action, the magus can spend 1 point from his arcane pool to fortify his defenses against chaotic attacks. This grants " +
                "him a dodge bonus to his AC equal to half his magus level (maximum +10 at 20th level) against chaotic-aligned attacks and effects " +
                "and outsiders with the chaotic subtype until the beginning of his next turn.", null);
            circle_buff.m_Icon = ShieldOfLaw.Icon;
            circle_buff.CreateACBonusAgainstAlignment(AlignmentComponent.Chaotic, 0, ModifierDescriptor.Dodge, ContextValueType.Rank);
            circle_buff.CreateContextRankConfigClassLevel(AbilityRankType.Default, Magus, ContextRankProgression.Div2);

            circle_ability = Helpers.CreateAbility(
                "CircleOfOrderAbility",
                "Circle of Order",
                "As a swift action, the magus can spend 1 point from his arcane pool to fortify his defenses against chaotic attacks. This grants " +
                "him a dodge bonus to his AC equal to half his magus level (maximum +10 at 20th level) against chaotic-aligned attacks and effects " +
                "and outsiders with the chaotic subtype until the beginning of his next turn.", null,
                "1 round", "", Kingmaker.UnitLogic.Commands.Base.UnitCommand.CommandType.Swift);
            circle_ability.m_Icon = ShieldOfLaw.Icon;
            circle_ability.SetSupernaturalSelf();
            circle_ability.CreateAbilityResourceLogic(MagusArcanePool, 1);
            circle_ability.CreateSpawnFx("92150879041b1fb48acfbcf7034e8b33");
            circle_ability.CreateAbilityEffectRunAction(Helpers.CreateContextActionBuff(
                circle_buff, false, true, Helpers.CreateContextDurationValue(1, DurationRate.Rounds)));

            circle_feature = Helpers.CreateFeature(
                "CircleOfOrderFeature",
                "Circle of Order",
                "As a swift action, the magus can spend 1 point from his arcane pool to fortify his defenses against chaotic attacks. This grants " +
                "him a dodge bonus to his AC equal to half his magus level (maximum +10 at 20th level) against chaotic-aligned attacks and effects " +
                "and outsiders with the chaotic subtype until the beginning of his next turn.",
                null, ShieldOfLaw.Icon);
            circle_feature.HideInCharacterSheetAndLevelUp = true;
            circle_feature.CreateAddFacts(circle_ability);
            circle_feature.CreateClassLevelRestriction(Magus, 9);
            circle_feature.Groups = new FeatureGroup[] { FeatureGroup.MagusArcana };
            Helpers.AddNewMagusArcana(circle_feature);
        }
    }
}
