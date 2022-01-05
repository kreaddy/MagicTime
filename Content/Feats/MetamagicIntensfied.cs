using Kingmaker.Blueprints;
using Kingmaker.Blueprints.Classes;
using Kingmaker.Blueprints.Classes.Selection;
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
    internal static class MetamagicIntensfied
    {
        public static void Create()
        {
            var intensified_spell_feature = Helpers.CreateFeature("MetamagicItensified", "Metamagic (Intensified Spell)", "Your spells can go beyond " +
                "several normal limitations.\nBenefit: An intensified spell increases the maximum number of damage dice by 5 levels. You must " +
                "actually have sufficient caster levels to surpass the maximum in order to benefit from this feat. No other variables of the " +
                "spell are affected, and spells that inflict damage that is not modified by caster level are not affected by this feat.\n" +
                "Level increase: +1 (An intensified spell uses up a spell slot one level higher than the spell's actual level.)",
                "m_intensified_spell");
            intensified_spell_feature.Groups = new FeatureGroup[] { FeatureGroup.Feat, FeatureGroup.WizardFeat };
            intensified_spell_feature.CreateFeatureTags(FeatureTag.Magic & FeatureTag.Metamagic);
            intensified_spell_feature.RestrictByStat(Kingmaker.EntitySystem.Stats.StatType.Intelligence, 3);
            intensified_spell_feature.CreateRecommendationRequiresSpellbook();
            intensified_spell_feature.AddNewMetamagic(Starion.MetamagicExtender.ExtraMetamagic.Intensified);
            Helpers.AddNewWizardFeat(intensified_spell_feature);

            var mythic_variant = Helpers.CreateFeature("FavoriteMetamagicIntensified", "Favorite Metamagic — Intensified", "You can now use " +
                "your mythic powers to fuel your metamagic abilities.\nBenefit: Select one kind of metamagic.The level cost for its use " +
                "decreases by one(to a minimum of 0).");
            mythic_variant.CreateFeatureRestriction(intensified_spell_feature);
            var list = DB.GetSelection("Favorite Metamagic").m_AllFeatures.ToList();
            list.Add(mythic_variant.ToReference<BlueprintFeatureReference>());
            DB.GetSelection("Favorite Metamagic").m_AllFeatures = list.ToArray();

            intensified_spell_feature.IsPrerequisiteFor = new List<BlueprintFeatureReference>()
            {
                mythic_variant.ToReference<BlueprintFeatureReference>()
            };

            Starion.BPExtender.Mechanics.FavoriteMetamagicIntensified = mythic_variant;

            // If Metamagic Insight exists in the game then add this feat to the list of variants.
            if (ArcaneDiscoveries.MetamagicInsight.Initialized)
            {
                var itensify_buff = Helpers.CreateBuff(
                    "MIIntensifyBuff",
                    "Metamagic Insight — Intensified Spell",
                    "Your next spell will be automatically Intensified.",
                    null, null, BlueprintBuff.Flags.RemoveOnRest);
                itensify_buff.m_Icon = intensified_spell_feature.Icon;
                itensify_buff.CreateAbilityUseTrigger(false, null, true, new ContextActionRemoveSelf());
                itensify_buff.CreateAutoMetamagic((Metamagic)Starion.MetamagicExtender.ExtraMetamagic.Intensified, 10);

                var intensify_ability = Helpers.CreateAbility(
                    "MIIntensifyAbility",
                    "Metamagic Insight — Intensified Spell",
                    "Your next spell will be automatically Intensified.",
                    null,
                    "1 round", "",
                    UnitCommand.CommandType.Free);
                intensify_ability.m_Icon = intensified_spell_feature.Icon;
                intensify_ability.SetSupernaturalSelf();
                intensify_ability.SetShowOnlyIfFact(intensified_spell_feature);
                intensify_ability.DisableIfFact(itensify_buff);
                intensify_ability.CreateAbilityResourceLogic(ArcaneDiscoveries.MetamagicInsight.metamagic_resource, 1);
                intensify_ability.CreateAbilityEffectRunAction(
                    Helpers.CreateContextActionBuff(itensify_buff, false, true, Helpers.CreateContextDurationValue(1, DurationRate.Rounds)));

                var comp = ArcaneDiscoveries.MetamagicInsight.metamagic_ability.GetComponent<AbilityVariants>();
                var variant_list = comp.m_Variants.ToList();
                variant_list.Add(intensify_ability.ToReference<BlueprintAbilityReference>());
                comp.m_Variants = variant_list.ToArray();

                intensify_ability.m_Parent = ArcaneDiscoveries.MetamagicInsight.metamagic_ability.ToReference<BlueprintAbilityReference>();
            }
        }
    }
}