using Kingmaker.Blueprints;
using Kingmaker.Blueprints.Classes;
using Kingmaker.Blueprints.Classes.Prerequisites;
using Kingmaker.Blueprints.Classes.Selection;
using Kingmaker.Blueprints.Classes.Spells;
using Kingmaker.Blueprints.Facts;
using Kingmaker.Blueprints.Items.Armors;
using Kingmaker.Designers.Mechanics.Buffs;
using Kingmaker.Designers.Mechanics.Facts;
using Kingmaker.Designers.Mechanics.Recommendations;
using Kingmaker.ElementsSystem;
using Kingmaker.EntitySystem.Stats;
using Kingmaker.Enums;
using Kingmaker.Enums.Damage;
using Kingmaker.RuleSystem.Rules;
using Kingmaker.RuleSystem.Rules.Damage;
using Kingmaker.UnitLogic.Abilities;
using Kingmaker.UnitLogic.Abilities.Blueprints;
using Kingmaker.UnitLogic.Abilities.Components;
using Kingmaker.UnitLogic.Abilities.Components.Base;
using Kingmaker.UnitLogic.Abilities.Components.CasterCheckers;
using Kingmaker.UnitLogic.ActivatableAbilities;
using Kingmaker.UnitLogic.Alignments;
using Kingmaker.UnitLogic.Buffs.Blueprints;
using Kingmaker.UnitLogic.FactLogic;
using Kingmaker.UnitLogic.Mechanics;
using Kingmaker.UnitLogic.Mechanics.Actions;
using Kingmaker.UnitLogic.Mechanics.Components;
using Kingmaker.Utility;
using Kingmaker.Visual.Animation.Kingmaker.Actions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MagicTime.Utilities
{
    internal static class Extensions
    {
        public static void AddFeatures(this BlueprintFeatureSelection selection, params BlueprintFeatureReference[] features)
        {
            selection.m_AllFeatures.Union(features);
            selection.m_Features.Union(features);
            selection.m_AllFeatures = selection.m_AllFeatures.OrderBy(feature => feature.Get().Name).ToArray();
            selection.m_Features = selection.m_Features.OrderBy(feature => feature.Get().Name).ToArray();
        }

        internal static void AddArchetype(this BlueprintCharacterClass bp, BlueprintArchetype archetype)
        {
            var result = bp.m_Archetypes.ToList();
            result.Add(archetype.ToReference<BlueprintArchetypeReference>());
            bp.m_Archetypes = result.ToArray();
        }

        internal static void AppendToActionList(this ActionList list, params GameAction[] actions)
        {
            var result = list.Actions.Concat(actions).ToArray();
            list.Actions = result;
        }

        internal static void CreateAbilityBonus(this BlueprintBuff bp, int value, StatType stat = StatType.Dexterity,
            ModifierDescriptor descriptor = ModifierDescriptor.UntypedStackable)
        {
            var result = new AddStatBonusAbilityValue();
            result.Stat = stat;
            result.Descriptor = descriptor;
            result.Value = new ContextValue();
            result.Value.ValueType = ContextValueType.Simple;
            result.Value.Value = value;
            bp.CreateComponents(result);
        }

        internal static void CreateAbilityEffectRunAction(this BlueprintScriptableObject bp, params GameAction[] actions)
        {
            var result = new AbilityEffectRunAction();
            result.Actions = Helpers.CreateActionList(actions);
            bp.CreateComponents(result);
        }

        internal static void CreateAbilityResourceLogic(this BlueprintAbility bp, BlueprintAbilityResource resource, int amount)
        {
            var result = new AbilityResourceLogic();
            result.m_IsSpendResource = true;
            result.ResourceCostDecreasingFacts = new List<BlueprintUnitFactReference>();
            result.ResourceCostIncreasingFacts = new List<BlueprintUnitFactReference>();
            result.m_RequiredResource = resource.ToReference<BlueprintAbilityResourceReference>();
            result.Amount = amount;
            bp.CreateComponents(result);
        }

        internal static void CreateAbilityResourceLogic(this BlueprintActivatableAbility bp, BlueprintAbilityResource resource,
            ActivatableAbilityResourceLogic.ResourceSpendType spend_type)
        {
            var result = new ActivatableAbilityResourceLogic();
            result.m_RequiredResource = resource.ToReference<BlueprintAbilityResourceReference>();
            result.SpendType = spend_type;
            bp.CreateComponents(result);
        }

        internal static void CreateAbilityTargetAround(this BlueprintAbility bp, TargetType target_type, float radius, float speed)
        {
            var result = new AbilityTargetsAround();
            result.m_TargetType = target_type;
            result.m_Radius = new Feet();
            result.m_Radius.m_Value = radius;
            result.m_SpreadSpeed = new Feet();
            result.m_SpreadSpeed.m_Value = speed;
            result.m_Condition = new ConditionsChecker();
            result.m_Condition.Conditions = new Condition[0];
            result.m_Condition.Operation = Operation.And;
            result.m_IncludeDead = false;
            bp.CreateComponents(result);
        }

        internal static void CreateAbilityUseTrigger(this BlueprintScriptableObject bp, bool from_spellbook, BlueprintSpellbook spellbook,
            bool after_cast, params ContextAction[] actions)
        {
            var result = new AddAbilityUseTrigger();
            result.Action = Helpers.CreateActionList(actions);
            if (spellbook != null)
            {
                result.FromSpellbook = from_spellbook;
                result.m_Spellbooks = new BlueprintSpellbookReference[] { spellbook.ToReference<BlueprintSpellbookReference>() };
            }
            else
            {
                result.FromSpellbook = false;
                result.CheckAbilityType = true;
                result.Type = AbilityType.Spell;
            }
            result.AfterCast = after_cast;
            bp.CreateComponents(result);
        }

        internal static void CreateACBonusAgainstAlignment(this BlueprintScriptableObject bp, AlignmentComponent alignment, int value,
            ModifierDescriptor descriptor, ContextValueType value_type = ContextValueType.Simple, AbilityRankType rank_type = AbilityRankType.Default)
        {
            var result = new ArmorClassBonusAgainstAlignment();
            result.alignment = alignment;
            result.Value = value;
            result.Bonus = new ContextValue();
            result.Bonus.ValueType = value_type;
            result.Bonus.ValueRank = rank_type;
            result.Descriptor = descriptor;
            bp.CreateComponents(result);
        }

        internal static void CreateAddAbilityResource(this BlueprintFeature bp, BlueprintAbilityResource resource, int amount = 0,
            bool restore_amount = true)
        {
            var result = new AddAbilityResources();
            result.m_Resource = resource.ToReference<BlueprintAbilityResourceReference>();
            result.Amount = amount;
            result.RestoreAmount = restore_amount;
            bp.CreateComponents(result);
        }

        internal static void CreateAddDRUntyped(this BlueprintScriptableObject bp, int base_value, ContextValueType value_type)
        {
            var result = new AddDamageResistancePhysical();
            result.Material = PhysicalDamageMaterial.Adamantite;
            result.MinEnhancementBonus = 1;
            result.Alignment = DamageAlignment.Good;
            result.Reality = DamageRealityType.Ghost;
            result.Pool = new ContextValue();
            result.Pool.Value = 12;
            result.Value = new ContextValue();
            result.Value.Value = base_value;
            result.Value.ValueType = value_type;
            bp.CreateComponents(result);
        }

        internal static void CreateAddFacts(this BlueprintFeature bp, params BlueprintUnitFact[] facts)
        {
            var result = new AddFacts();
            var list = new List<BlueprintUnitFactReference>();
            foreach (var fact in facts)
            {
                list.Add(fact.ToReference<BlueprintUnitFactReference>());
            }
            result.m_Facts = list.ToArray();
            bp.CreateComponents(result);
        }

        internal static void CreateAddSpellKnown(this BlueprintFeature bp, BlueprintCharacterClass cclass, BlueprintArchetype arch,
            BlueprintAbility spell, int level)
        {
            var result = new AddKnownSpell();
            result.m_CharacterClass = cclass.ToReference<BlueprintCharacterClassReference>();
            result.m_Archetype = arch.ToReference<BlueprintArchetypeReference>();
            result.m_Spell = spell.ToReference<BlueprintAbilityReference>();
            result.SpellLevel = level;
            bp.CreateComponents(result);
        }

        internal static void CreateAddStatBonus(this BlueprintScriptableObject bp, int value, ModifierDescriptor descriptor, params StatType[] stats)
        {
            foreach (var stat in stats)
            {
                var result = new AddStatBonus()
                {
                    Stat = stat,
                    Value = value,
                    Descriptor = descriptor,
                    ScaleByBasicAttackBonus = false
                };
                bp.CreateComponents(result);
            }
        }

        internal static void CreateAddStatBonusContext(this BlueprintScriptableObject bp, StatType stat, ModifierDescriptor descriptor,
            AbilityRankType rank)
        {
            var result = new AddStatBonusAbilityValue();
            result.Stat = stat;
            result.Descriptor = descriptor;
            result.Value = new ContextValue();
            result.Value.ValueType = ContextValueType.Rank;
            result.Value.ValueRank = rank;
            bp.CreateComponents(result);
        }

        internal static void CreateAddStatToCMB(this BlueprintScriptableObject bp, StatType stat, CombatManeuver man_type = CombatManeuver.None)
        {
            var result = new ManeuverBonusFromStat();
            result.Stat = stat;
            result.Type = man_type;
            result.Descriptor = ModifierDescriptor.UntypedStackable;
            bp.CreateComponents(result);
        }

        internal static void CreateAlignmentRestriction(this BlueprintScriptableObject bp, AlignmentMaskType mask)
        {
            var result = new PrerequisiteAlignment();
            result.Alignment = mask;
            bp.CreateComponents(result);
        }

        internal static void CreateArchetypeBan(this BlueprintFeature bp, BlueprintArchetype archetype)
        {
            var result = new PrerequisiteNoArchetype();
            result.m_Archetype = archetype.ToReference<BlueprintArchetypeReference>();
            bp.CreateComponents(result);
        }

        internal static void CreateAutoMetamagic(this BlueprintScriptableObject bp, Metamagic kind, int max_level, BlueprintSpellbook spellbook = null,
            AutoMetamagic.AllowedType type = AutoMetamagic.AllowedType.SpellOnly, SpellSchool school = SpellSchool.None)
        {
            var result = new AutoMetamagic();
            result.Metamagic = kind;
            result.MaxSpellLevel = max_level;
            if (spellbook != null)
            {
                result.CheckSpellbook = true;
                result.m_Spellbook = spellbook.ToReference<BlueprintSpellbookReference>();
            }
            result.m_AllowedAbilities = type;
            result.School = school;
            bp.CreateComponents(result);
        }

        internal static void CreateBuffExtraEffects(this BlueprintScriptableObject bp, BlueprintBuff checked_buff, BlueprintBuff new_buff)
        {
            var result = new BuffExtraEffects();
            result.m_CheckedBuff = checked_buff.ToReference<BlueprintBuffReference>();
            result.m_ExtraEffectBuff = new_buff.ToReference<BlueprintBuffReference>();
            bp.CreateComponents(result);
        }

        internal static void CreateBuffSummons(this BlueprintScriptableObject bp, BlueprintBuff buff)
        {
            var result = new OnSpawnBuff();
            result.IsInfinity = true;
            result.m_buff = buff.ToReference<BlueprintBuffReference>();
            bp.CreateComponents(result);
        }

        internal static void CreateClassLevelRestriction(this BlueprintScriptableObject bp, BlueprintCharacterClass cclass, int level)
        {
            var result = new PrerequisiteClassLevel();
            result.m_CharacterClass = cclass.ToReference<BlueprintCharacterClassReference>();
            result.Level = level;
            bp.CreateComponents(result);
        }

        internal static void CreateComponents(this BlueprintScriptableObject bp, params BlueprintComponent[] components)
        {
            bp.ComponentsArray = bp.ComponentsArray.Concat(components).ToArray();
            // Fix possible serialization issues.
            var names = new HashSet<string>();
            foreach (var c in bp.ComponentsArray)
            {
                if (string.IsNullOrEmpty(c.name))
                {
                    c.name = $"${c.GetType().Name}";
                }
                if (!names.Add(c.name))
                {
                    string name;
                    for (int i = 0; !names.Add(name = $"{c.name}${i}"); i++) ;
                    c.name = name;
                }
            }
            bp.OnEnable();
        }

        internal static void CreateConcentrationBonus(this BlueprintScriptableObject bp, int bonus, bool check_fact = false,
            ContextValueType value_type = ContextValueType.Simple, AbilityRankType value_rank = AbilityRankType.Default)
        {
            var result = new ConcentrationBonus();
            result.Value = bonus;
            result.Value.ValueType = value_type;
            result.Value.ValueRank = value_rank;
            result.CheckFact = check_fact;
            bp.CreateComponents(result);
        }

        internal static void CreateContextRankConfig(this BlueprintScriptableObject bp, ContextRankBaseValueType value_type,
            ContextRankProgression progression, int step_level, AbilityRankType type = AbilityRankType.Default,
            BlueprintCharacterClass c_class = null, bool custom_progression = false, int base_value = 0, int progression_value = 0)
        {
            var result = new ContextRankConfig();
            result.m_BaseValueType = value_type;
            result.m_Progression = progression;
            result.m_Type = type;
            result.m_StepLevel = step_level;
            if (custom_progression)
            {
                result.m_CustomProgression = new ContextRankConfig.CustomProgressionItem[]
                {
                    new ContextRankConfig.CustomProgressionItem()
                    {
                        BaseValue = base_value,
                        ProgressionValue = progression_value
                    }
                };
            }
            if (c_class != null)
            {
                result.m_Class = new BlueprintCharacterClassReference[] { c_class.ToReference<BlueprintCharacterClassReference>() };
            }
            bp.CreateComponents(result);
        }

        internal static void CreateContextRankConfigClassLevel(this BlueprintScriptableObject bp, AbilityRankType type,
            BlueprintCharacterClass c_class, ContextRankProgression progression = ContextRankProgression.AsIs, int base_value = 0,
            int progression_value = 0)
        {
            CreateContextRankConfig(bp,
                ContextRankBaseValueType.ClassLevel, progression, 1, type, c_class, progression == ContextRankProgression.Custom, base_value,
                progression_value);
        }

        internal static void CreateContextRankConfigMythicRank(this BlueprintScriptableObject bp, ContextRankProgression progression,
            int step_level = 1, AbilityRankType type = AbilityRankType.Default)
        {
            CreateContextRankConfig(bp, ContextRankBaseValueType.MythicLevel, progression, 1, type);
        }

        internal static void CreateContextRankConfigStat(this BlueprintScriptableObject bp, AbilityRankType rank_type, StatType stat)
        {
            var result = new ContextRankConfig();
            result.m_BaseValueType = ContextRankBaseValueType.StatBonus;
            result.m_Stat = stat;
            result.m_Type = rank_type;
            bp.CreateComponents(result);
        }

        internal static void CreateDerivativeStat(this BlueprintFeature bp, StatType stat, StatType derivative)
        {
            var comp1 = new DerivativeStatBonus();
            comp1.BaseStat = stat;
            comp1.DerivativeStat = derivative;
            var comp2 = new RecalculateOnStatChange();
            comp2.Stat = stat;
            bp.CreateComponents(comp1, comp2);
        }

        internal static void CreateDisableAttack(this BlueprintBuff bp)
        {
            var result = new DisableAttackType();
            result.m_AttackType = Kingmaker.RuleSystem.AttackTypeFlag.Melee | Kingmaker.RuleSystem.AttackTypeFlag.Ranged;
            bp.CreateComponents(result);
        }

        internal static void CreateDispelBonus(this BlueprintScriptableObject bp, ContextValueType value_type, AbilityRankType value_rank)
        {
            var result = new DispelCasterLevelCheckBonus();
            result.Value = new ContextValue();
            result.Value.ValueType = value_type;
            result.Value.ValueRank = value_rank;
            bp.CreateComponents(result);
        }

        internal static void CreateEnergyImmunity(this BlueprintScriptableObject bp, DamageEnergyType elem)
        {
            var result = new AddEnergyImmunity();
            result.Type = elem;
            bp.CreateComponents(result);
        }

        internal static void CreateFastHealing(this BlueprintBuff bp, int amount)
        {
            var result = new HealOverTime();
            result.Heal = amount;
            bp.CreateComponents(result);
        }

        internal static void CreateFeatureRestriction(this BlueprintScriptableObject bp, BlueprintFeature feature,
            Prerequisite.GroupType group = Prerequisite.GroupType.All, bool hidden = false)
        {
            var result = new PrerequisiteFeature();
            result.m_Feature = feature.ToReference<BlueprintFeatureReference>();
            result.Group = group;
            result.HideInUI = hidden;
            bp.CreateComponents(result);
        }

        internal static void CreateFeatureRestrictionInv(this BlueprintScriptableObject bp, BlueprintFeature feature,
            Prerequisite.GroupType group = Prerequisite.GroupType.Any, bool hidden = false)
        {
            var result = new PrerequisiteNoFeature();
            result.m_Feature = feature.ToReference<BlueprintFeatureReference>();
            result.Group = group;
            result.HideInUI = hidden;
            bp.CreateComponents(result);
        }

        internal static void CreateFeatureTags(this BlueprintFeature bp, FeatureTag tags)
        {
            var result = new FeatureTagsComponent();
            result.FeatureTags = tags;
            bp.CreateComponents(result);
        }

        internal static void CreateGenericComponent<T>(this BlueprintScriptableObject bp, Action<T> init = null) where T : BlueprintComponent, new()
        {
            var result = new T();
            init?.Invoke(result);
            bp.CreateComponents(result);
        }

        internal static void CreateIncreaseResource(this BlueprintFeature bp, BlueprintAbilityResource res, int amount)
        {
            var result = new IncreaseResourceAmount();
            result.m_Resource = res.ToReference<BlueprintAbilityResourceReference>();
            result.Value = amount;
            bp.CreateComponents(result);
        }

        internal static void CreateIncreaseSpellbookCL(this BlueprintScriptableObject bp, int value, params BlueprintSpellbookReference[] spellbooks)
        {
            var result = new AddCasterLevelForSpellbook();
            result.m_Spellbooks = spellbooks;
            result.Descriptor = ModifierDescriptor.UntypedStackable;
            result.Bonus = value;
            bp.CreateComponents(result);
        }

        internal static void CreateIncreaseSpellbookDC(this BlueprintScriptableObject bp, int value, params BlueprintSpellbookReference[] spellbooks)
        {
            var result = new IncreaseSpellSpellbookDC();
            result.m_Spellbooks = spellbooks;
            result.Descriptor = ModifierDescriptor.UntypedStackable;
            result.BonusDC = value;
            bp.CreateComponents(result);
        }

        internal static void CreateLearnSpells(this BlueprintFeature bp, BlueprintCharacterClass cclass, params BlueprintAbility[] spells)
        {
            var result = new LearnSpells();
            result.m_CharacterClass = cclass.ToReference<BlueprintCharacterClassReference>();
            var list = new List<BlueprintAbilityReference>();
            foreach (var spell in spells)
            {
                list.Add(spell.ToReference<BlueprintAbilityReference>());
            }
            result.m_Spells = list.ToArray();
            bp.CreateComponents(result);
        }

        internal static void CreateModifyD20(this BlueprintScriptableObject bp, RuleType rule, int rolls, bool take_best, ModifierDescriptor desc,
            ContextValueType value_type, int value)
        {
            var result = new ModifyD20();
            result.Rule = rule;
            result.RollsAmount = rolls;
            result.TakeBest = take_best;
            result.BonusDescriptor = desc;
            result.Bonus = new ContextValue();
            result.Bonus.ValueType = value_type;
            result.Bonus.Value = value;
            bp.CreateComponents(result);
        }

        internal static void CreateOutgoingDamageBonus(this BlueprintScriptableObject bp, float factor, DamageEnergyType dtype = 0,
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
            bp.CreateComponents(result);
        }

        internal static void CreateProjectile(this BlueprintAbility bp, bool attack_roll, AbilityProjectileType type, string key)
        {
            var result = new AbilityDeliverProjectile();
            result.NeedAttackRoll = attack_roll;
            result.Type = type;
            var projectile = DB.GetBP<BlueprintProjectile>(key);
            result.m_Projectiles = new BlueprintProjectileReference[] { projectile.ToReference<BlueprintProjectileReference>() };
            bp.CreateComponents(result);
        }

        internal static void CreateRecommendationRequiresSpellbook(this BlueprintFeature bp)
        {
            var result = new RecommendationRequiresSpellbook();
            bp.CreateComponents(result);
        }

        internal static void CreateRemoveFact(this BlueprintFeature bp, BlueprintFact fact)
        {
            var result = new RemoveFeatureOnApply();
            result.m_Feature = fact.ToReference<BlueprintUnitFactReference>();
            bp.CreateComponents(result);
        }

        internal static void CreateResistEnergyFlat(this BlueprintScriptableObject bp, DamageEnergyType elem, int value)
        {
            var result = new ResistEnergy();
            result.Type = elem;
            result.Value = new ContextValue();
            result.Value.ValueType = ContextValueType.Simple;
            result.Value.Value = value;
            result.UsePool = false;
            result.UseValueMultiplier = false;
            bp.CreateComponents(result);
        }

        internal static void CreateRestTrigger(this BlueprintFeature bp, params ContextAction[] actions)
        {
            var result = new AddRestTrigger();
            result.Action = Helpers.CreateActionList(actions);
            bp.CreateComponents(result);
        }

        internal static void CreateSavingThrowTrigger(this BlueprintBuff bp, params ContextAction[] actions)
        {
            var result = new AddInitiatorSavingThrowTrigger();
            result.Action = Helpers.CreateActionList(actions);
            bp.CreateComponents(result);
        }

        internal static void CreateSizeChange(this BlueprintBuff bp, Size size)
        {
            var result = new ChangeUnitSize();
            result.m_Type = ChangeUnitSize.ChangeType.Value;
            result.Size = size;
            bp.CreateComponents(result);
        }

        internal static void CreateSpawnFx(this BlueprintAbility bp, string asset_id, AbilitySpawnFxAnchor anchor = AbilitySpawnFxAnchor.Caster,
            AbilitySpawnFxTime time = AbilitySpawnFxTime.OnApplyEffect)
        {
            var result = new AbilitySpawnFx();
            result.PrefabLink = new Kingmaker.ResourceLinks.PrefabLink();
            result.PrefabLink.AssetId = asset_id;
            result.PositionAnchor = anchor;
            result.Time = time;
            bp.CreateComponents(result);
        }

        internal static void CreateSpellComponent(this BlueprintAbility bp, SpellSchool school)
        {
            var result = new SpellComponent();
            result.School = school;
            bp.CreateComponents(result);
        }

        internal static void CreateSpellDescriptor(this BlueprintAbility bp, SpellDescriptor descriptor)
        {
            var result = new SpellDescriptorComponent();
            result.Descriptor = descriptor;
            bp.CreateComponents(result);
        }

        internal static void CreateSpellListComponent(this BlueprintAbility bp, int level, BlueprintSpellList spell_list)
        {
            var result = new SpellListComponent();
            result.SpellLevel = level;
            result.m_SpellList = spell_list.ToReference<BlueprintSpellListReference>();
            bp.CreateComponents(result);
        }

        internal static void CreateSpellPenBonus(this BlueprintScriptableObject bp, int bonus, bool check_fact, ContextValueType type,
            AbilityRankType rank)
        {
            var result = new SpellPenetrationBonus();
            result.Value = new ContextValue();
            result.Value.ValueType = type;
            result.Value.ValueRank = rank;
            result.Value.Value = bonus;
            result.CheckFact = check_fact;
            result.Descriptor = ModifierDescriptor.UntypedStackable;
            bp.CreateComponents(result);
        }

        internal static void CreateSpellSlotOverride(this BlueprintScriptableObject bp, BlueprintAbilityResource res, int mult)
        {
            var result = new AbilityResourceOverride();
            result.m_AbilityResource = res.ToReference<BlueprintAbilityResourceReference>();
            result.m_SaveSpellSlot = true;
            result.m_AdditionalCost = new ContextValue();
            result.m_AdditionalCost.ValueType = ContextValueType.Simple;
            result.m_AdditionalCost.ValueShared = AbilitySharedValue.Damage;
            result.m_AdditionalCost.Value = 0;
            result.m_AdditionalCost.ValueRank = AbilityRankType.Default;
            result.m_LevelMultiplier = new ContextValue();
            result.m_LevelMultiplier.ValueType = ContextValueType.Simple;
            result.m_LevelMultiplier.ValueShared = AbilitySharedValue.Damage;
            result.m_LevelMultiplier.Value = mult;
            result.m_LevelMultiplier.ValueRank = AbilityRankType.Default;
            bp.CreateComponents(result);
        }

        internal static void CreateStatRestriction(this BlueprintScriptableObject bp, StatType stat, int value)
        {
            var result = new PrerequisiteStatValue();
            result.Stat = stat;
            result.Value = value;
            bp.CreateComponents(result);
        }

        internal static void CreateTransferShieldACToTouch(this BlueprintFeature bp, int max)
        {
            var result = new TransferDescriptorBonusToTouchAC();
            result.CheckArmorCategory = true;
            result.Descriptor = ModifierDescriptor.Shield & ModifierDescriptor.ShieldEnhancement;
            result.Category = ArmorProficiencyGroup.LightShield | ArmorProficiencyGroup.HeavyShield | ArmorProficiencyGroup.TowerShield;
            result.MaxBonus = max;
            bp.CreateComponents(result);
        }

        internal static void CreateVariants(this BlueprintAbility bp, params BlueprintAbility[] variants)
        {
            var result = new AbilityVariants();
            var list = new List<BlueprintAbilityReference>();
            foreach (var ability in variants)
            {
                list.Add(ability.ToReference<BlueprintAbilityReference>());
            }
            result.m_Variants = list.ToArray();
            bp.CreateComponents(result);
        }

        internal static void CreateWeaponProficiencies(this BlueprintFeature bp, params WeaponCategory[] weapons)
        {
            var result = new AddProficiencies();
            result.WeaponProficiencies = weapons;
            bp.CreateComponents(result);
        }

        internal static void DeleteComponents(this BlueprintScriptableObject bp, params BlueprintComponent[] components)
        {
            bp.ComponentsArray = bp.ComponentsArray.Except(components).ToArray();
            // Fix possible serialization issues.
            var names = new HashSet<string>();
            foreach (var c in bp.ComponentsArray)
            {
                if (string.IsNullOrEmpty(c.name))
                {
                    c.name = $"${c.GetType().Name}";
                }
                if (!names.Add(c.name))
                {
                    string name;
                    for (int i = 0; !names.Add(name = $"{c.name}${i}"); i++) ;
                    c.name = name;
                }
            }
            bp.OnEnable();
        }

        internal static void DisableIfFact(this BlueprintAbility bp, params BlueprintUnitFact[] facts)
        {
            var result = new AbilityCasterHasNoFacts();
            var list = new List<BlueprintUnitFactReference>();
            foreach (var fact in facts)
            {
                list.Add(fact.ToReference<BlueprintUnitFactReference>());
            }
            result.m_Facts = list.ToArray();
            bp.CreateComponents(result);
        }

        internal static void DisableIfNotFact(this BlueprintAbility bp, params BlueprintUnitFact[] facts)
        {
            var result = new AbilityCasterHasFacts();
            var list = new List<BlueprintUnitFactReference>();
            foreach (var fact in facts)
            {
                list.Add(fact.ToReference<BlueprintUnitFactReference>());
            }
            result.m_Facts = list.ToArray();
            bp.CreateComponents(result);
        }

        internal static void NoSelectionIfFeature(this BlueprintFeatureSelection bp)
        {
            var result = new NoSelectionIfAlreadyHasFeature();
            result.AnyFeatureFromSelection = true;
            bp.CreateComponents(result);
        }

        internal static void RestrictByStat(this BlueprintFeature bp, StatType stat, int value)
        {
            var result = new PrerequisiteStatValue();
            result.Stat = stat;
            result.Value = value;
            bp.CreateComponents(result);
        }
        internal static void RestrictSelectionToNewFeatures(this BlueprintFeatureSelection bp)
        {
            bp.Mode = SelectionMode.OnlyNew;
        }

        internal static void RestrictToMC(this BlueprintCharacterClass bp)
        {
            var result = new PrerequisiteMainCharacter();
            result.HideInUI = true;
            bp.CreateComponents(result);
        }

        internal static void SetParentOf(this BlueprintAbility bp, params BlueprintAbility[] children)
        {
            foreach (var child in children)
            {
                child.m_Parent = bp.ToReference<BlueprintAbilityReference>();
            }
        }

        internal static void SetShowOnlyIfFact(this BlueprintAbility bp, BlueprintUnitFact fact)
        {
            var result = new AbilityShowIfCasterHasFact();
            result.m_UnitFact = fact.ToReference<BlueprintUnitFactReference>();
            bp.CreateComponents(result);
        }

        internal static void SetSpellAttack(this BlueprintAbility bp, AbilityRange range)
        {
            bp.Type = AbilityType.Spell;
            bp.SpellResistance = true;
            bp.NotOffensive = false;
            bp.Animation = UnitAnimationActionCastSpell.CastAnimationStyle.Omni;
            bp.Range = range;
            bp.CanTargetEnemies = true;
            bp.CanTargetFriends = false;
            bp.CanTargetPoint = false;
            bp.CanTargetSelf = false;
        }

        internal static void SetSpellSupport(this BlueprintAbility bp, AbilityRange range)
        {
            bp.Type = AbilityType.Spell;
            bp.SpellResistance = false;
            bp.NotOffensive = true;
            bp.Animation = UnitAnimationActionCastSpell.CastAnimationStyle.Touch;
            bp.Range = range;
            bp.CanTargetEnemies = false;
            bp.CanTargetFriends = true;
            bp.CanTargetPoint = false;
            bp.CanTargetSelf = true;
        }

        internal static void SetSupernaturalHarmful(this BlueprintAbility bp, AbilityRange range)
        {
            bp.Type = AbilityType.Supernatural;
            bp.SpellResistance = false;
            bp.NotOffensive = false;
            bp.EffectOnAlly = AbilityEffectOnUnit.Harmful;
            bp.EffectOnEnemy = AbilityEffectOnUnit.Harmful;
            bp.Animation = UnitAnimationActionCastSpell.CastAnimationStyle.Omni;
            bp.Range = range;
            bp.CanTargetEnemies = true;
            bp.CanTargetFriends = false;
            bp.CanTargetPoint = false;
            bp.CanTargetSelf = false;
        }

        internal static void SetSupernaturalSelf(this BlueprintAbility bp,
                                                    UnitAnimationActionCastSpell.CastAnimationStyle style = UnitAnimationActionCastSpell.CastAnimationStyle.Omni)
        {
            bp.Type = AbilityType.Supernatural;
            bp.SpellResistance = false;
            bp.NotOffensive = true;
            bp.EffectOnAlly = AbilityEffectOnUnit.Helpful;
            bp.Animation = style;
            bp.Range = AbilityRange.Personal;
            bp.CanTargetEnemies = false;
            bp.CanTargetFriends = false;
            bp.CanTargetPoint = false;
            bp.CanTargetSelf = true;
        }
    }
}