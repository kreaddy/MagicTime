using Kingmaker.Blueprints;
using Kingmaker.Blueprints.Classes;
using Kingmaker.Blueprints.Classes.Prerequisites;
using Kingmaker.Blueprints.Classes.Selection;
using Kingmaker.Blueprints.Classes.Spells;
using Kingmaker.Blueprints.Facts;
using Kingmaker.Blueprints.Items.Armors;
using Kingmaker.Designers.Mechanics.Buffs;
using Kingmaker.Designers.Mechanics.Facts;
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
using Kingmaker.UnitLogic.Alignments;
using Kingmaker.UnitLogic.Buffs.Blueprints;
using Kingmaker.UnitLogic.FactLogic;
using Kingmaker.UnitLogic.Mechanics;
using Kingmaker.UnitLogic.Mechanics.Actions;
using Kingmaker.UnitLogic.Mechanics.Components;
using Kingmaker.Utility;
using Kingmaker.Visual.Animation.Kingmaker.Actions;
using System.Collections.Generic;
using System.Linq;

namespace MagicTime.Utilities
{
    internal static class Extensions
    {
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
            BlueprintCharacterClass c_class = null)
        {
            var result = new ContextRankConfig();
            result.m_BaseValueType = value_type;
            result.m_Progression = progression;
            result.m_Type = type;
            result.m_StepLevel = step_level;
            if (c_class != null)
            {
                result.m_Class = new BlueprintCharacterClassReference[] { c_class.ToReference<BlueprintCharacterClassReference>() };
            }
            bp.CreateComponents(result);
        }

        internal static void CreateContextRankConfigClassLevel(this BlueprintScriptableObject bp, AbilityRankType type,
            BlueprintCharacterClass c_class, ContextRankProgression progression = ContextRankProgression.AsIs)
        {
            CreateContextRankConfig(bp,
                ContextRankBaseValueType.ClassLevel, progression, 1, type, c_class);
        }

        internal static void CreateContextRankConfigMythicRank(this BlueprintScriptableObject bp, ContextRankProgression progression,
            int step_level = 1, AbilityRankType type = AbilityRankType.Default)
        {
            CreateContextRankConfig(bp, ContextRankBaseValueType.MythicLevel, progression, 1, type);
        }

        internal static void CreateFastHealing(this BlueprintBuff bp, int amount)
        {
            var result = new HealOverTime();
            result.Heal = amount;
            bp.CreateComponents(result);
        }

        internal static void CreateFeatureRestriction(this BlueprintScriptableObject bp, BlueprintFeature feature,
            Prerequisite.GroupType group = Prerequisite.GroupType.All)
        {
            var result = new PrerequisiteFeature();
            result.m_Feature = feature.ToReference<BlueprintFeatureReference>();
            result.Group = group;
            bp.CreateComponents(result);
        }

        internal static void CreateFeatureRestrictionInv(this BlueprintScriptableObject bp, BlueprintFeature feature,
            Prerequisite.GroupType group = Prerequisite.GroupType.Any)
        {
            var result = new PrerequisiteNoFeature();
            result.m_Feature = feature.ToReference<BlueprintFeatureReference>();
            result.Group = group;
            bp.CreateComponents(result);
        }

        internal static void CreateFeatureTags(this BlueprintFeature bp, FeatureTag tags)
        {
            var result = new FeatureTagsComponent();
            result.FeatureTags = tags;
        }

        internal static void CreateGenericComponent<T>(this BlueprintScriptableObject bp) where T : BlueprintComponent, new()
        {
            var result = new T();
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

        internal static void CreateSpellPenBonus(this BlueprintScriptableObject bp, int bonus, bool check_fact = false)
        {
            var result = new SpellPenetrationBonus();
            result.Value = bonus;
            result.CheckFact = check_fact;
            result.Descriptor = ModifierDescriptor.UntypedStackable;
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