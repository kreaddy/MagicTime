using Kingmaker.Blueprints;
using Kingmaker.Blueprints.Classes;
using Kingmaker.Blueprints.Classes.Selection;
using Kingmaker.Blueprints.Facts;
using Kingmaker.Designers.EventConditionActionSystem.Actions;
using Kingmaker.ElementsSystem;
using Kingmaker.EntitySystem.Stats;
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
using System.Reflection;
using UnityEngine;

namespace MagicTime.Utilities
{
    internal static class Helpers
    {
        public static T Create<T>(Action<T> init = null) where T : new()
        {
            var result = new T();
            init?.Invoke(result);
            return result;
        }
        public static T CreateBlueprint<T>(string asset_name, Action<T> init = null) where T : BlueprintScriptableObject, new()
        {
            var result = new T();
            result.name = asset_name;
            result.AssetGuid = Resources.AddAsset(asset_name, result);
            init?.Invoke(result);
            return result;
        }

        private static readonly MethodInfo CloneMethod = typeof(object).GetMethod("MemberwiseClone", BindingFlags.NonPublic | BindingFlags.Instance);

        public static T Clone<T>(T original_bp, string new_name = null) where T : SimpleBlueprint, new()
        {
            var clone = CloneMethod.Invoke(original_bp, null) as T;
            /*
            var fields = new List<FieldInfo>();
            foreach (var field in original_bp.GetType().GetFields())
            {
                fields.Add(field);
            }
            var fields_array = fields.ToArray();
            var clone = new T();
            if (new_name == null)
            {
                new_name = "Cloned" + original_bp.name;
                Resources.UpdateDatabase(Guid.NewGuid().ToString(), "Cloned" + original_bp.name);
            }
            clone.name = new_name;
            for (int i = 0; i < fields_array.Length; i++)
            {
                var field = clone.GetType().GetFields().ToArray()[i];
                var new_field = fields_array[i];
                field.SetValue(clone, new_field.GetValue(original_bp));

            }
            */
            clone.AssetGuid = Resources.AddAsset(new_name, clone);
            return clone;
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

        public static BlueprintAbilityResource CreateAbilityResourceFixed(string asset_name, int base_value)
        {
            var result = CreateBlueprint<BlueprintAbilityResource>(asset_name, bp =>
            {
                bp.m_MaxAmount = new BlueprintAbilityResource.Amount
                {
                    BaseValue = base_value,
                };
                bp.m_Max = base_value;
            });
            return result;
        }

        public static BlueprintAbilityResource CreateAbilityResourceVariable(string asset_name, int base_value, bool with_level,
            int lv_increase, params BlueprintCharacterClassReference[] classes)
        {
            var result = CreateBlueprint<BlueprintAbilityResource>(asset_name, bp =>
            {
                bp.m_UseMax = false;
                bp.m_MaxAmount = new BlueprintAbilityResource.Amount
                {
                    BaseValue = base_value,
                    IncreasedByLevel = with_level,
                    LevelIncrease = lv_increase,
                    m_Class = classes
                };
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

        public static BlueprintProgression.ClassWithLevel CreateClassBasedProgression(BlueprintCharacterClass cclass, int level)
        {
            var result = new BlueprintProgression.ClassWithLevel();
            result.m_Class = cclass.ToReference<BlueprintCharacterClassReference>();
            result.AdditionalLevel = level;
            return result;
        }

        public static BlueprintProgression.ArchetypeWithLevel CreateArchetypeBasedProgression(BlueprintArchetype arch, int level)
        {
            var result = new BlueprintProgression.ArchetypeWithLevel();
            result.m_Archetype = arch.ToReference<BlueprintArchetypeReference>();
            result.AdditionalLevel = level;
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
            var selection = DB.GetSelection("Feat Selection");
            var list = selection.m_AllFeatures.ToList();
            list.Add(feat.ToReference<BlueprintFeatureReference>());
            selection.m_AllFeatures = list.ToArray();
            selection = DB.GetSelection("Mythic Feat Extra Feat Selection");
            list = selection.m_AllFeatures.ToList();
            list.Add(feat.ToReference<BlueprintFeatureReference>());
            selection.m_AllFeatures = list.ToArray();
        }

        public static void AddNewWizardFeat(BlueprintFeature feat)
        {
            AddNewFeat(feat);
            var selection = DB.GetSelection("Wizard Feat Selection");
            var list = selection.m_AllFeatures.ToList();
            list.Add(feat.ToReference<BlueprintFeatureReference>());
            selection.m_AllFeatures = list.ToArray();
        }

        public static void AddNewMythicAbility(BlueprintFeature fact)
        {
            var selection = DB.GetSelection("Mythic Ability Selection");
            var list = selection.m_AllFeatures.ToList();
            list.Add(fact.ToReference<BlueprintFeatureReference>());
            list.OrderBy(f => f.Get().m_DisplayName);
            selection.m_AllFeatures = list.ToArray();
            selection = DB.GetSelection("Extra Mythic Ability Selection");
            list = selection.m_AllFeatures.ToList();
            list.Add(fact.ToReference<BlueprintFeatureReference>());
            list.OrderBy(f => f.Get().m_DisplayName);
            selection.m_AllFeatures = list.ToArray();
        }

        public static void AddNewMythicFeat(BlueprintFeature fact)
        {
            var selection = DB.GetSelection("Mythic Feat Selection");
            var list = selection.m_AllFeatures.ToList();
            list.Add(fact.ToReference<BlueprintFeatureReference>());
            selection.m_AllFeatures = list.ToArray();
        }

        public static void AddNewCombatTrick(BlueprintFeature feat)
        {
            var selection = DB.GetSelection("Combat Trick");
            var list = selection.m_AllFeatures.ToList();
            list.Add(feat.ToReference<BlueprintFeatureReference>());
            selection.m_AllFeatures = list.ToArray();
        }

        public static void AddNewMagusArcana(BlueprintFeature fact)
        {
            var selection = DB.GetSelection("Magus Arcana Selection");
            var list = selection.m_AllFeatures.ToList();
            list.Add(fact.ToReference<BlueprintFeatureReference>());
            selection.m_AllFeatures = list.ToArray();
            selection = DB.GetSelection("Eldritch Scion Arcana Selection");
            list = selection.m_AllFeatures.ToList();
            list.Add(fact.ToReference<BlueprintFeatureReference>());
            selection.m_AllFeatures = list.ToArray();
            selection = DB.GetSelection("Hexcrafter Arcana Selection");
            list = selection.m_AllFeatures.ToList();
            list.Add(fact.ToReference<BlueprintFeatureReference>());
            selection.m_AllFeatures = list.ToArray();
        }

        public static void AddNewFighterFeat(BlueprintFeature feat)
        {
            var selection = DB.GetSelection("Fighter Feat Selection");
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

        public static ContextActionDealDamage CreateContextActionDamageNoDice(DamageType damage_type, DamageEnergyType energy)
        {
            var result = new ContextActionDealDamage();
            result.Half = false;
            result.IgnoreCritical = true;
            result.ReadPreRolledFromSharedValue = false;
            result.PreRolledSharedValue = Kingmaker.UnitLogic.Abilities.AbilitySharedValue.Damage;
            result.DamageType = new DamageTypeDescription();
            result.DamageType.Type = damage_type;
            result.DamageType.Energy = energy;
            result.Value = new ContextDiceValue();
            result.Value.DiceType = Kingmaker.RuleSystem.DiceType.Zero;
            result.Value.DiceCountValue = new ContextValue();
            result.Value.DiceCountValue.ValueType = ContextValueType.Simple;
            result.Value.BonusValue = new ContextValue();
            result.Value.BonusValue.ValueType = ContextValueType.Rank;
            result.Value.BonusValue.ValueShared = Kingmaker.UnitLogic.Abilities.AbilitySharedValue.Damage;
            result.Value.BonusValue.ValueRank = AbilityRankType.Default;
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

        public static LevelEntry CreateLevelEntryByList(int level, List<BlueprintFeatureBaseReference> features)
        {
            var result = new LevelEntry();
            result.Level = level;
            result.m_Features = features;
            return result;
        }

        public static void RegisterClass(BlueprintCharacterClass ClassToRegister)
        {
            var progression = ResourcesLibrary.GetRoot().Progression;
            List<BlueprintCharacterClassReference> list = ((IEnumerable<BlueprintCharacterClassReference>)progression.m_CharacterClasses).ToList<BlueprintCharacterClassReference>();
            list.Add(ClassToRegister.ToReference<BlueprintCharacterClassReference>());
            list.Sort((Comparison<BlueprintCharacterClassReference>)((x, y) => {
                BlueprintCharacterClass blueprint1 = ResourcesLibrary.TryGetBlueprint<BlueprintCharacterClass>(x.guid);
                BlueprintCharacterClass blueprint2 = ResourcesLibrary.TryGetBlueprint<BlueprintCharacterClass>(y.guid);
                return blueprint1 == null || blueprint2 == null ? 1 : (blueprint1.PrestigeClass == blueprint2.PrestigeClass ? blueprint1.NameSafe().CompareTo(blueprint2.NameSafe()) : (blueprint1.PrestigeClass ? 1 : -1));
            }));
            progression.m_CharacterClasses = list.ToArray();
            if (!ClassToRegister.IsArcaneCaster && !ClassToRegister.IsDivineCaster)
                return;
            BlueprintProgression.ClassWithLevel classWithLevel = ClassToClassWithLevel(ClassToRegister);
            BlueprintProgression blueprint = ResourcesLibrary.TryGetBlueprint<BlueprintProgression>("fe9220cdc16e5f444a84d85d5fa8e3d5");
            var newdata = new BlueprintProgression.ClassWithLevel[] { classWithLevel };
            blueprint.m_Classes = blueprint.m_Classes.Concat(newdata).ToArray();
        }

        public static BlueprintProgression.ClassWithLevel ClassToClassWithLevel(
        BlueprintCharacterClass orig,
        int addLevel = 0)
        {
            return new BlueprintProgression.ClassWithLevel()
            {
                m_Class = orig.ToReference<BlueprintCharacterClassReference>(),
                AdditionalLevel = addLevel
            };
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
            var strings = LocalizationManager.CurrentPack.m_Strings;
            LocalizationPack.StringEntry oldValue;
            if (strings.TryGetValue(key, out oldValue) && value != oldValue.Text)
            {
#if DEBUG
                Main.Log($"Info: duplicate localized string `{key}`, different text.");
#endif
            }
            strings[key] = new LocalizationPack.StringEntry() { Text = value };
            localized = new LocalizedString
            {
                m_Key = key
            };
            textToLocalizedString[value] = localized;
            return localized;
        }
    }
}