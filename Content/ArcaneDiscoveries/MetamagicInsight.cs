using Kingmaker.Blueprints;
using Kingmaker.Blueprints.Classes;
using Kingmaker.UnitLogic.Abilities;
using Kingmaker.UnitLogic.Abilities.Blueprints;
using Kingmaker.UnitLogic.Buffs.Blueprints;
using Kingmaker.UnitLogic.Commands.Base;
using Kingmaker.UnitLogic.Mechanics;
using Kingmaker.UnitLogic.Mechanics.Actions;
using MagicTime.Utilities;

namespace MagicTime.ArcaneDiscoveries
{
    internal static class MetamagicInsight
    {
        public static bool Initialized;
        public static BlueprintAbilityResource metamagic_resource;
        private static BlueprintBuff bolster_buff;
        private static BlueprintAbility bolster_ability;
        private static BlueprintBuff empower_buff;
        private static BlueprintAbility empower_ability;
        private static BlueprintBuff extend_buff;
        private static BlueprintAbility extend_ability;
        private static BlueprintBuff maximize_buff;
        private static BlueprintAbility maximize_ability;
        private static BlueprintBuff persistent_buff;
        private static BlueprintAbility persistent_ability;
        private static BlueprintBuff quicken_buff;
        private static BlueprintAbility quicken_ability;
        private static BlueprintBuff reach_buff;
        private static BlueprintAbility reach_ability;
        private static BlueprintBuff selective_buff;
        private static BlueprintAbility selective_ability;
        public static BlueprintAbility metamagic_ability;
        private static BlueprintFeature metamagic_feature;

        public static void Create()
        {
            // Resource.
            metamagic_resource = Helpers.CreateAbilityResourceFixed("MetamagicInsight", 2);

            // Bolster.
            bolster_buff = Helpers.CreateBuff(
                "MIBolsterBuff",
                "Metamagic Insight — Bolstered Spell",
                "Your next spell will be automatically Bolstered.",
                null, null, BlueprintBuff.Flags.RemoveOnRest);
            bolster_buff.m_Icon = DB.GetFeature("Metamagic Bolster").m_Icon;
            bolster_buff.CreateAbilityUseTrigger(false, null, true, new ContextActionRemoveSelf());
            bolster_buff.CreateAutoMetamagic(Metamagic.Bolstered, 10);

            bolster_ability = Helpers.CreateAbility(
                "MIBolsterAbility",
                "Metamagic Insight — Bolstered Spell",
                "Your next spell will be automatically Bolstered.",
                null,
                "1 round", "",
                UnitCommand.CommandType.Free);
            bolster_ability.m_Icon = DB.GetFeature("Metamagic Bolster").m_Icon;
            bolster_ability.SetSupernaturalSelf();
            bolster_ability.SetShowOnlyIfFact(DB.GetFeature("Metamagic Bolster"));
            bolster_ability.DisableIfFact(bolster_buff);
            bolster_ability.CreateAbilityResourceLogic(metamagic_resource, 1);
            bolster_ability.CreateAbilityEffectRunAction(
                Helpers.CreateContextActionBuff(bolster_buff, false, true, Helpers.CreateContextDurationValue(1, DurationRate.Rounds)));

            // Empower.
            empower_buff = Helpers.CreateBuff(
                "MIEmpowerBuff",
                "Metamagic Insight — Empowered Spell",
                "Your next spell will be automatically Empowered.",
                null, null, BlueprintBuff.Flags.RemoveOnRest);
            empower_buff.m_Icon = DB.GetFeature("Metamagic Empower").m_Icon;
            empower_buff.CreateAbilityUseTrigger(false, null, true, new ContextActionRemoveSelf());
            empower_buff.CreateAutoMetamagic(Metamagic.Empower, 10);

            empower_ability = Helpers.CreateAbility(
                "MIEmpowerAbility",
                "Metamagic Insight — Empowered Spell",
                "Your next spell will be automatically Empowered.",
                null,
                "1 round", "",
                UnitCommand.CommandType.Free);
            empower_ability.m_Icon = DB.GetFeature("Metamagic Empower").m_Icon;
            empower_ability.SetSupernaturalSelf();
            empower_ability.SetShowOnlyIfFact(DB.GetFeature("Metamagic Empower"));
            empower_ability.DisableIfFact(empower_buff);
            empower_ability.CreateAbilityResourceLogic(metamagic_resource, 1);
            empower_ability.CreateAbilityEffectRunAction(
                Helpers.CreateContextActionBuff(empower_buff, false, true, Helpers.CreateContextDurationValue(1, DurationRate.Rounds)));

            // Extend.
            extend_buff = Helpers.CreateBuff(
                "MIExtendBuff",
                "Metamagic Insight — Extended Spell",
                "Your next spell will be automatically Extended.",
                null, null, BlueprintBuff.Flags.RemoveOnRest);
            extend_buff.m_Icon = DB.GetFeature("Metamagic Extend").m_Icon;
            extend_buff.CreateAbilityUseTrigger(false, null, true, new ContextActionRemoveSelf());
            extend_buff.CreateAutoMetamagic(Metamagic.Extend, 10);

            extend_ability = Helpers.CreateAbility(
                "MIExtendAbility",
                "Metamagic Insight — Extended Spell",
                "Your next spell will be automatically Extended.",
                null,
                "1 round", "",
                UnitCommand.CommandType.Free);
            extend_ability.m_Icon = DB.GetFeature("Metamagic Extend").m_Icon;
            extend_ability.SetSupernaturalSelf();
            extend_ability.SetShowOnlyIfFact(DB.GetFeature("Metamagic Extend"));
            extend_ability.DisableIfFact(extend_buff);
            extend_ability.CreateAbilityResourceLogic(metamagic_resource, 1);
            extend_ability.CreateAbilityEffectRunAction(
                Helpers.CreateContextActionBuff(extend_buff, false, true, Helpers.CreateContextDurationValue(1, DurationRate.Rounds)));

            // Maximize.
            maximize_buff = Helpers.CreateBuff(
                "MIMaximizeBuff",
                "Metamagic Insight — Maximized Spell",
                "Your next spell will be automatically Maximized.",
                null, null, BlueprintBuff.Flags.RemoveOnRest);
            maximize_buff.m_Icon = DB.GetFeature("Metamagic Maximize").m_Icon;
            maximize_buff.CreateAbilityUseTrigger(false, null, true, new ContextActionRemoveSelf());
            maximize_buff.CreateAutoMetamagic(Metamagic.Maximize, 10);

            maximize_ability = Helpers.CreateAbility(
                "MIMaximizeAbility",
                "Metamagic Insight — Maximized Spell",
                "Your next spell will be automatically Maximized.",
                null,
                "1 round", "",
                UnitCommand.CommandType.Free);
            maximize_ability.m_Icon = DB.GetFeature("Metamagic Maximize").m_Icon;
            maximize_ability.SetSupernaturalSelf();
            maximize_ability.SetShowOnlyIfFact(DB.GetFeature("Metamagic Maximize"));
            maximize_ability.DisableIfFact(maximize_buff);
            maximize_ability.CreateAbilityResourceLogic(metamagic_resource, 1);
            maximize_ability.CreateAbilityEffectRunAction(
                Helpers.CreateContextActionBuff(maximize_buff, false, true, Helpers.CreateContextDurationValue(1, DurationRate.Rounds)));

            // Persistent.
            persistent_buff = Helpers.CreateBuff(
                "MIPersistentBuff",
                "Metamagic Insight — Persistent Spell",
                "Your next spell will be automatically Persistent.",
                null, null, BlueprintBuff.Flags.RemoveOnRest);
            persistent_buff.m_Icon = DB.GetFeature("Metamagic Persistent").m_Icon;
            persistent_buff.CreateAbilityUseTrigger(false, null, true, new ContextActionRemoveSelf());
            persistent_buff.CreateAutoMetamagic(Metamagic.Persistent, 10);

            persistent_ability = Helpers.CreateAbility(
                "MIPersistentAbility",
                "Metamagic Insight — Persistent Spell",
                "Your next spell will be automatically Persistent.",
                null,
                "1 round", "",
                UnitCommand.CommandType.Free);
            persistent_ability.m_Icon = DB.GetFeature("Metamagic Persistent").m_Icon;
            persistent_ability.SetSupernaturalSelf();
            persistent_ability.SetShowOnlyIfFact(DB.GetFeature("Metamagic Persistent"));
            persistent_ability.DisableIfFact(persistent_buff);
            persistent_ability.CreateAbilityResourceLogic(metamagic_resource, 1);
            persistent_ability.CreateAbilityEffectRunAction(
                Helpers.CreateContextActionBuff(persistent_buff, false, true, Helpers.CreateContextDurationValue(1, DurationRate.Rounds)));

            // Quicken.
            quicken_buff = Helpers.CreateBuff(
                "MIQuickenBuff",
                "Metamagic Insight — Quickened Spell",
                "Your next spell will be automatically Quickened.",
                null, null, BlueprintBuff.Flags.RemoveOnRest);
            quicken_buff.m_Icon = DB.GetFeature("Metamagic Quicken").m_Icon;
            quicken_buff.CreateAbilityUseTrigger(false, null, true, new ContextActionRemoveSelf());
            quicken_buff.CreateAutoMetamagic(Metamagic.Quicken, 10);

            quicken_ability = Helpers.CreateAbility(
                "MIQuickenAbility",
                "Metamagic Insight — Quickened Spell",
                "Your next spell will be automatically Quickened.",
                null,
                "1 round", "",
                UnitCommand.CommandType.Free);
            quicken_ability.m_Icon = DB.GetFeature("Metamagic Quicken").m_Icon;
            quicken_ability.SetSupernaturalSelf();
            quicken_ability.SetShowOnlyIfFact(DB.GetFeature("Metamagic Quicken"));
            quicken_ability.DisableIfFact(quicken_buff);
            quicken_ability.CreateAbilityResourceLogic(metamagic_resource, 1);
            quicken_ability.CreateAbilityEffectRunAction(
                Helpers.CreateContextActionBuff(quicken_buff, false, true, Helpers.CreateContextDurationValue(1, DurationRate.Rounds)));

            // Reach.
            reach_buff = Helpers.CreateBuff(
                "MIReachBuff",
                "Metamagic Insight — Reach Spell",
                "Your next spell will have increased range, as per the Reach metamagic feat.",
                null, null, BlueprintBuff.Flags.RemoveOnRest);
            reach_buff.m_Icon = DB.GetFeature("Metamagic Reach").m_Icon;
            reach_buff.CreateAbilityUseTrigger(false, null, true, new ContextActionRemoveSelf());
            reach_buff.CreateAutoMetamagic(Metamagic.Reach, 10);

            reach_ability = Helpers.CreateAbility(
                "MIReachAbility",
                "Metamagic Insight — Reach Spell",
                "Your next spell will have increased range, as per the Reach metamagic feat.",
                null,
                "1 round", "",
                UnitCommand.CommandType.Free);
            reach_ability.m_Icon = DB.GetFeature("Metamagic Reach").m_Icon;
            reach_ability.SetSupernaturalSelf();
            reach_ability.SetShowOnlyIfFact(DB.GetFeature("Metamagic Reach"));
            reach_ability.DisableIfFact(reach_buff);
            reach_ability.CreateAbilityResourceLogic(metamagic_resource, 1);
            reach_ability.CreateAbilityEffectRunAction(
                Helpers.CreateContextActionBuff(reach_buff, false, true, Helpers.CreateContextDurationValue(1, DurationRate.Rounds)));

            // Selective.
            selective_buff = Helpers.CreateBuff(
                "MISelectiveBuff",
                "Metamagic Insight — Selective Spell",
                "Your next spell will be automatically Selective.",
                null, null, BlueprintBuff.Flags.RemoveOnRest);
            selective_buff.m_Icon = DB.GetFeature("Metamagic Selective").m_Icon;
            selective_buff.CreateAbilityUseTrigger(false, null, true, new ContextActionRemoveSelf());
            selective_buff.CreateAutoMetamagic(Metamagic.Selective, 10);

            selective_ability = Helpers.CreateAbility(
                "MISelectiveAbility",
                "Metamagic Insight — Selective Spell",
                "Your next spell will be automatically Selective.",
                null,
                "1 round", "",
                UnitCommand.CommandType.Free);
            selective_ability.m_Icon = DB.GetFeature("Metamagic Selective").m_Icon;
            selective_ability.SetSupernaturalSelf();
            selective_ability.SetShowOnlyIfFact(DB.GetFeature("Metamagic Selective"));
            selective_ability.DisableIfFact(selective_buff);
            selective_ability.CreateAbilityResourceLogic(metamagic_resource, 1);
            selective_ability.CreateAbilityEffectRunAction(
                Helpers.CreateContextActionBuff(selective_buff, false, true, Helpers.CreateContextDurationValue(1, DurationRate.Rounds)));

            // Main ability.
            metamagic_ability = Helpers.CreateAbility(
                "MIBaseAbility",
                "Metamagic Insight",
                "You can apply any of your known metamagic feats to the next spell your cast.",
                "ad_metamagic_insight",
                "1 round", "",
                UnitCommand.CommandType.Free);
            metamagic_ability.SetSupernaturalSelf();
            metamagic_ability.CreateVariants(bolster_ability, empower_ability, extend_ability, maximize_ability, persistent_ability,
                quicken_ability, reach_ability, selective_ability);
            metamagic_ability.SetParentOf(bolster_ability, empower_ability, extend_ability, maximize_ability, persistent_ability,
                quicken_ability, reach_ability, selective_ability);

            // Feature.
            metamagic_feature = Helpers.CreateFeature(
                "MIFeature",
                "Metamagic Insight",
                "Your understanding of advanced arcane theories reached new heights.\nBenefit: Twice per day, you can apply any of your known " +
                "metamagic feats to the next spell you cast in a round as a free action, without needing to prepare the spell ahead of time. " +
                "This does not increase its casting time or the required slot level.",
                "ad_metamagic_insight");
            metamagic_feature.m_AllowNonContextActions = true;
            metamagic_feature.HideInCharacterSheetAndLevelUp = true;
            metamagic_feature.CreateAddFacts(metamagic_ability);
            metamagic_feature.CreateAddAbilityResource(metamagic_resource, 2);
            metamagic_feature.CreateClassLevelRestriction(DB.GetClass("Wizard Class"), 10);
            Main.AddNewDiscovery(metamagic_feature);

            Initialized = true;
        }
    }
}