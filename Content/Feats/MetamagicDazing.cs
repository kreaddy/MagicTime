using Kingmaker.Blueprints;
using Kingmaker.Blueprints.Classes;
using Kingmaker.Blueprints.Classes.Selection;
using Kingmaker.EntitySystem.Stats;
using Kingmaker.UnitLogic.Abilities;
using Kingmaker.UnitLogic.Abilities.Components;
using Kingmaker.UnitLogic.Buffs.Blueprints;
using Kingmaker.UnitLogic.Commands.Base;
using Kingmaker.UnitLogic.Mechanics;
using Kingmaker.UnitLogic.Mechanics.Actions;
using MagicTime.Utilities;
using Starion.BPExtender.UnitFact;
using System.Collections.Generic;
using System.Linq;

namespace MagicTime.Feats
{
    internal static class MetamagicDazing
    {
        public static void Create()
        {
            var dazing_spell_feature = Helpers.CreateFeature("MetamagicDazing", "Metamagic (Dazing Spell)", "You can daze creatures with the " +
                "power of your spells.\nBenefit: You can modify a spell to daze a creature damaged by the spell. When a creature takes damage " +
                "from this spell, they become dazed for a number of rounds equal to the original level of the spell. If the spell allows a " +
                "saving throw, a successful save negates the daze effect. If the spell does not allow a save, the target can make a Will save " +
                "to negate the daze effect. Spells that do not inflict damage do not benefit from this feat.\n" +
                "Level increase: +3 (An dazing spell uses up a spell slot three levels higher than the spell's actual level.)", "m_dazing_spell");
            dazing_spell_feature.Groups = new FeatureGroup[] { FeatureGroup.Feat, FeatureGroup.WizardFeat };
            dazing_spell_feature.CreateFeatureTags(FeatureTag.Magic & FeatureTag.Metamagic);
            dazing_spell_feature.RestrictByStat(StatType.Intelligence, 3);
            dazing_spell_feature.CreateRecommendationRequiresSpellbook();
            dazing_spell_feature.AddNewMetamagic(Starion.MetamagicExtender.ExtraMetamagic.Dazing);
            Helpers.AddNewWizardFeat(dazing_spell_feature);

            var mythic_variant = Helpers.CreateFeature("FavoriteMetamagicDazing", "Favorite Metamagic — Dazing", "You can now use " +
                "your mythic powers to fuel your metamagic abilities.\nBenefit: Select one kind of metamagic.The level cost for its use " +
                "decreases by one(to a minimum of 0).");
            mythic_variant.CreateFeatureRestriction(dazing_spell_feature);
            var list = DB.GetSelection("Favorite Metamagic").m_AllFeatures.ToList();
            list.Add(mythic_variant.ToReference<BlueprintFeatureReference>());
            DB.GetSelection("Favorite Metamagic").m_AllFeatures = list.ToArray();

            dazing_spell_feature.IsPrerequisiteFor = new List<BlueprintFeatureReference>()
            {
                mythic_variant.ToReference<BlueprintFeatureReference>()
            };

            Starion.BPExtender.Mechanics.FavoriteMetamagicIntensified = mythic_variant;

            // If Metamagic Insight exists in the game then add this feat to the list of variants.
            if (ArcaneDiscoveries.MetamagicInsight.Initialized)
            {
                var dazing_buff = Helpers.CreateBuff(
                    "MIDazingBuff",
                    "Metamagic Insight — Dazing Spell",
                    "Your next spell will be automatically Dazing.",
                    null, null, BlueprintBuff.Flags.RemoveOnRest);
                dazing_buff.m_Icon = dazing_spell_feature.Icon;
                dazing_buff.CreateAbilityUseTrigger(false, null, true, new ContextActionRemoveSelf());
                dazing_buff.CreateAutoMetamagic((Metamagic)Starion.MetamagicExtender.ExtraMetamagic.Dazing, 10);

                var dazing_ability = Helpers.CreateAbility(
                    "MIDazingAbility",
                    "Metamagic Insight — Dazing Spell",
                    "Your next spell will be automatically Dazing.",
                    null,
                    "1 round", "",
                    UnitCommand.CommandType.Free);
                dazing_ability.m_Icon = dazing_spell_feature.Icon;
                dazing_ability.SetSupernaturalSelf();
                dazing_ability.SetShowOnlyIfFact(dazing_spell_feature);
                dazing_ability.DisableIfFact(dazing_buff);
                dazing_ability.CreateAbilityResourceLogic(ArcaneDiscoveries.MetamagicInsight.metamagic_resource, 1);
                dazing_ability.CreateAbilityEffectRunAction(
                    Helpers.CreateContextActionBuff(dazing_buff, false, true, Helpers.CreateContextDurationValue(1, DurationRate.Rounds)));

                var comp = ArcaneDiscoveries.MetamagicInsight.metamagic_ability.GetComponent<AbilityVariants>();
                var variant_list = comp.m_Variants.ToList();
                variant_list.Add(dazing_ability.ToReference<BlueprintAbilityReference>());
                comp.m_Variants = variant_list.ToArray();

                dazing_ability.m_Parent = ArcaneDiscoveries.MetamagicInsight.metamagic_ability.ToReference<BlueprintAbilityReference>();
            }
        }
    }
}