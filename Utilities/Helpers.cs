using Kingmaker.Blueprints;
using Kingmaker.Blueprints.Classes;
using Kingmaker.Blueprints.Classes.Selection;
using Kingmaker.Blueprints.Facts;
using Kingmaker.Designers.EventConditionActionSystem.Actions;
using Kingmaker.ElementsSystem;
using Kingmaker.Enums;
using Kingmaker.Enums.Damage;
using Kingmaker.Localization;
using Kingmaker.ResourceLinks;
using Kingmaker.RuleSystem.Rules.Damage;
using Kingmaker.UnitLogic.Abilities.Blueprints;
using Kingmaker.UnitLogic.ActivatableAbilities;
using Kingmaker.UnitLogic.Buffs.Blueprints;
using Kingmaker.UnitLogic.Commands.Base;
using Kingmaker.UnitLogic.FactLogic;
using Kingmaker.UnitLogic.Mechanics;
using Kingmaker.UnitLogic.Mechanics.Actions;
using Kingmaker.UnitLogic.Mechanics.Conditions;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MagicTime.Utilities
{
    internal static class Helpers
    {
        public static T CreateBlueprint<T>(string asset_name, Action<T> init = null) where T : BlueprintScriptableObject, new()
        {
            var result = new T();
            result.name = asset_name;
            result.AssetGuid = Resources.AddAsset(asset_name, result);
            init?.Invoke(result);
            return result;
        }

        public static BlueprintBuff CreateBuff(string asset_name, string name, string description, string icon = null,
            PrefabLink fx_on_start = null, BlueprintBuff.Flags buff_flags = 0)
        {
            var result = CreateBlueprint<BlueprintBuff>(asset_name, bp =>
            {
                bp.m_DisplayName = CreateString($"{asset_name}.Name", name);
                bp.m_Description = CreateString($"{asset_name}.Description", DescriptionTools.TagEncyclopediaEntries(description));
                if (icon != null) { bp.m_Icon = AssetLoader.LoadInternal("Icons", icon); }
                bp.FxOnStart = fx_on_start;
                if (fx_on_start == null) { bp.FxOnStart = new PrefabLink(); }
                bp.FxOnRemove = new PrefabLink();
                bp.Components = new BlueprintComponent[0];
                bp.Stacking = StackingType.Replace;
                bp.m_Flags = buff_flags;
            });
            return result;
        }

        public static BlueprintAbility CreateAbility(string asset_name, string name, string description, string icon = null,
            string duration = "", string saving_throws = "", UnitCommand.CommandType action = UnitCommand.CommandType.Standard,
            bool fullround = false)
        {
            var result = CreateBlueprint<BlueprintAbility>(asset_name, bp =>
            {
                bp.m_DisplayName = CreateString($"{asset_name}.Name", name);
                bp.m_Description = CreateString($"{asset_name}.Description", DescriptionTools.TagEncyclopediaEntries(description));
                if (icon != null) { bp.m_Icon = AssetLoader.LoadInternal("Icons", icon); }
                bp.LocalizedDuration = CreateString($"{asset_name}.Duration", duration);
                bp.LocalizedSavingThrow = CreateString($"{asset_name}.SavingThrow", saving_throws);
                bp.Components = new BlueprintComponent[0];
                bp.ActionType = action;
                bp.m_IsFullRoundAction = fullround;
            });
            return result;
        }

        public static BlueprintActivatableAbility CreateActivatableAbility(string asset_name, string name, string description, string icon,
            BlueprintBuff buff, bool def_on = false, bool end_after_combat = false)
        {
            var result = CreateBlueprint<BlueprintActivatableAbility>(asset_name, bp =>
            {
                bp.m_DisplayName = CreateString($"{asset_name}.Name", name);
                bp.m_Description = CreateString($"{asset_name}.Description", DescriptionTools.TagEncyclopediaEntries(description));
                if (icon != null) { bp.m_Icon = AssetLoader.LoadInternal("Icons", icon); }
                bp.Components = new BlueprintComponent[0];
                bp.m_Buff = buff.ToReference<BlueprintBuffReference>();
                bp.IsOnByDefault = def_on;
                bp.DeactivateIfCombatEnded = end_after_combat;
            });
            return result;
        }

        public static BlueprintFeature CreateFeature(string asset_name, string name, string description, string icon_name = null,
            Sprite icon = null, bool class_feature = true)
        {
            var result = CreateBlueprint<BlueprintFeature>(asset_name, bp =>
            {
                bp.m_DisplayName = CreateString($"{asset_name}.Name", name);
                bp.m_Description = CreateString($"{asset_name}.Description", DescriptionTools.TagEncyclopediaEntries(description));
                if (icon_name != null) { bp.m_Icon = AssetLoader.LoadInternal("Icons", icon_name); }
                if (icon != null) { bp.m_Icon = icon; }
                bp.IsClassFeature = class_feature;
                bp.Ranks = 1;
                bp.Components = new BlueprintComponent[0];
            });
            return result;
        }

        public static BlueprintFeatureSelection CreateFeatureSelection(string asset_name, string name, string description, Sprite icon = null,
            bool class_feature = true, FeatureGroup group = FeatureGroup.None, params BlueprintFeature[] features)
        {
            var list = new List<BlueprintFeatureReference>();
            foreach (var feature in features)
            {
                list.Add(feature.ToReference<BlueprintFeatureReference>());
            }
            var result = CreateBlueprint<BlueprintFeatureSelection>(asset_name, bp =>
            {
                bp.m_DisplayName = CreateString($"{asset_name}.Name", name);
                bp.m_Description = CreateString($"{asset_name}.Description", DescriptionTools.TagEncyclopediaEntries(description));
                bp.IsClassFeature = class_feature;
                bp.Group = group;
                bp.m_Icon = null;
                if (icon != null) { bp.m_Icon = icon; }
                bp.m_Features = new BlueprintFeatureReference[0];
                bp.m_AllFeatures = list.ToArray();
                bp.m_AllowNonContextActions = false;
                bp.Ranks = 1;
                bp.Mode = SelectionMode.Default;
            });
            return result;
        }

        public static void AddNewFeat(BlueprintFeature feat)
        {
            var selection = BlueprintsDatabase.FeatSelection;
            var extra = BlueprintsDatabase.MythicFeatSelection;
            var list = selection.m_AllFeatures.ToList();
            list.Add(feat.ToReference<BlueprintFeatureReference>());
            selection.m_AllFeatures = list.ToArray();
            list = extra.m_AllFeatures.ToList();
            extra.m_AllFeatures = list.ToArray();
        }

        public static void AddNewWizardFeat(BlueprintFeature feat)
        {
            AddNewFeat(feat);
            var selection = BlueprintsDatabase.WizardFeatSelection;
            var list = selection.m_AllFeatures.ToList();
            list.Add(feat.ToReference<BlueprintFeatureReference>());
            selection.m_AllFeatures = list.ToArray();
        }

        public static void AddNewMythicAbility(BlueprintFeature fact)
        {
            var selection = BlueprintsDatabase.MythicAbilitySelection;
            var extra = BlueprintsDatabase.MythicExtraAbility;
            var list = selection.m_AllFeatures.ToList();
            list.Add(fact.ToReference<BlueprintFeatureReference>());
            selection.m_AllFeatures = list.ToArray();
            list = extra.m_AllFeatures.ToList();
            extra.m_AllFeatures = list.ToArray();
        }

        public static void AddNewMythicFeat(BlueprintFeature fact)
        {
            var selection = BlueprintsDatabase.MythicFeatSelection;
            var list = selection.m_AllFeatures.ToList();
            list.Add(fact.ToReference<BlueprintFeatureReference>());
            selection.m_AllFeatures = list.ToArray();
        }

        public static void AddNewCombatTrick(BlueprintFeature feat)
        {
            var selection = BlueprintsDatabase.CombatTrick;
            var list = selection.m_AllFeatures.ToList();
            list.Add(feat.ToReference<BlueprintFeatureReference>());
            selection.m_AllFeatures = list.ToArray();
        }

        public static void AddNewMagusArcana(BlueprintFeature fact)
        {
            var selection = BlueprintsDatabase.MagusArcanaSelection;
            var list = selection.m_AllFeatures.ToList();
            list.Add(fact.ToReference<BlueprintFeatureReference>());
            selection.m_AllFeatures = list.ToArray();
            selection = BlueprintsDatabase.ESArcanaSelection;
            list = selection.m_AllFeatures.ToList();
            list.Add(fact.ToReference<BlueprintFeatureReference>());
            selection.m_AllFeatures = list.ToArray();
            selection = BlueprintsDatabase.HexcrafterArcanaSelection;
            list = selection.m_AllFeatures.ToList();
            list.Add(fact.ToReference<BlueprintFeatureReference>());
            selection.m_AllFeatures = list.ToArray();
        }

        public static void AddNewFighterFeat(BlueprintFeature feat)
        {
            var selection = BlueprintsDatabase.FighterFeatSelection;
            var list = selection.m_AllFeatures.ToList();
            list.Add(feat.ToReference<BlueprintFeatureReference>());
            selection.m_AllFeatures = list.ToArray();
        }

        public static UIGroup CreateUIGroup(params BlueprintFeature[] features)
        {
            var result = new UIGroup();
            result.m_Features = new List<BlueprintFeatureBaseReference>();
            foreach (var feature in features)
            {
                result.m_Features.Add(feature.ToReference<BlueprintFeatureBaseReference>());
            }
            return result;
        }

        public static ActionList CreateActionList(params GameAction[] actions)
        {
            var result = new ActionList();
            result.Actions = actions;
            return result;
        }

        public static Conditional CreateConditionCasterHasFact(BlueprintUnitFact fact, ContextAction action_if_true,
            ContextAction action_if_false = null)
        {
            var result = new Conditional();
            result.ConditionsChecker = new ConditionsChecker();
            result.ConditionsChecker.Conditions = new Condition[]
            {
                new ContextConditionCasterHasFact() { m_Fact = fact.ToReference<BlueprintUnitFactReference>() }
            };
            result.IfTrue = CreateActionList(action_if_true);
            if (action_if_false != null)
            {
                result.IfFalse = CreateActionList(action_if_false);
            }
            return result;
        }

        public static AddOutgoingDamageBonus CreateOutgoingDamageBonus(float factor, DamageEnergyType dtype = 0,
            BlueprintUnitFact checked_fact = null, int reason = 0)
        {
            var result = new AddOutgoingDamageBonus();
            result.OriginalDamageFactor = factor;
            result.DamageType = new DamageTypeDescription();
            result.DamageType.Energy = dtype;
            result.DamageType.Type = DamageType.Energy;
            result.Reason = (DamageIncreaseReason)reason;
            if (checked_fact != null)
            {
                result.Condition = DamageIncreaseCondition.Fact;
                result.m_CheckedFact = checked_fact.ToReference<BlueprintUnitFactReference>();
            }
            return result;
        }

        public static ContextActionApplyBuff CreateContextActionBuff(BlueprintBuff buff, bool perm = false, bool as_child = false,
            ContextDurationValue duration = null)
        {
            var result = new ContextActionApplyBuff();
            result.m_Buff = buff.ToReference<BlueprintBuffReference>();
            result.AsChild = as_child;
            result.Permanent = perm;
            if (duration != null) { result.DurationValue = duration; }
            return result;
        }

        public static ContextActionRemoveBuff CreateContextActionRemoveBuff(BlueprintBuff buff)
        {
            var result = new ContextActionRemoveBuff();
            result.m_Buff = buff.ToReference<BlueprintBuffReference>();
            return result;
        }

        public static ContextActionSpawnFx CreateContextActionSpawnFx(string asset_id)
        {
            var result = new ContextActionSpawnFx();
            result.PrefabLink = new PrefabLink();
            result.PrefabLink.AssetId = asset_id;
            return result;
        }

        public static ContextActionReduceBuffDuration CreateReduceBuffDuration(int amount, DurationRate rate, BlueprintBuff buff,
            bool to_target = false)
        {
            var result = new ContextActionReduceBuffDuration();
            result.DurationValue = new ContextDurationValue();
            result.DurationValue.Rate = rate;
            result.DurationValue.DiceCountValue = new ContextValue();
            result.DurationValue.DiceType = Kingmaker.RuleSystem.DiceType.Zero;
            result.DurationValue.BonusValue = new ContextValue();
            result.DurationValue.BonusValue.ValueType = ContextValueType.Simple;
            result.DurationValue.BonusValue.Value = amount;
            result.m_TargetBuff = buff.ToReference<BlueprintBuffReference>();
            result.ToTarget = to_target;
            return result;
        }

        public static ContextDurationValue CreateContextDurationValue(int time, DurationRate rate, bool extendable = true)
        {
            var result = new ContextDurationValue();
            result.Rate = rate;
            result.m_IsExtendable = extendable;
            result.DiceCountValue = new ContextValue();
            result.DiceType = Kingmaker.RuleSystem.DiceType.Zero;
            result.BonusValue = new ContextValue();
            result.BonusValue.ValueType = ContextValueType.Simple;
            result.BonusValue.Value = time;
            return result;
        }

        public static ContextDurationValue CreateContextDurationValueVariable(AbilityRankType rank_type, DurationRate rate)
        {
            var result = new ContextDurationValue();
            result.DiceCountValue = new ContextValue();
            result.BonusValue = new ContextValue();
            result.BonusValue.ValueType = ContextValueType.Rank;
            result.BonusValue.ValueRank = rank_type;
            result.Rate = rate;
            return result;
        }

        public static LevelEntry CreateLevelEntry(int level, params BlueprintFeature[] features)
        {
            var result = new LevelEntry();
            result.Level = level;
            result.m_Features = new List<BlueprintFeatureBaseReference>();
            foreach (var feature in features)
            {
                result.m_Features.Add(feature.ToReference<BlueprintFeatureBaseReference>());
            }
            return result;
        }

        // All localized strings created in this mod, mapped to their localized key. Populated by CreateString.
        private static Dictionary<string, LocalizedString> textToLocalizedString = new Dictionary<string, LocalizedString>();

        public static LocalizedString CreateString(string key, string value)
        {
            // See if we used the text previously.
            // (It's common for many features to use the same localized text.
            // In that case, we reuse the old entry instead of making a new one.)
            LocalizedString localized;
            if (textToLocalizedString.TryGetValue(value, out localized))
            {
                return localized;
            }
            var strings = LocalizationManager.CurrentPack.Strings;
            string oldValue;
            if (strings.TryGetValue(key, out oldValue) && value != oldValue)
            {
#if DEBUG
                Main.Log($"Info: duplicate localized string `{key}`, different text.");
#endif
            }
            strings[key] = value;
            localized = new LocalizedString
            {
                m_Key = key
            };
            textToLocalizedString[value] = localized;
            return localized;
        }
    }
}