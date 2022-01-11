using HarmonyLib;
using Kingmaker.Blueprints;
using Kingmaker.Blueprints.Classes;
using Kingmaker.Blueprints.Facts;
using Kingmaker.Blueprints.JsonSystem;
using Kingmaker.PubSubSystem;
using Kingmaker.RuleSystem.Rules;
using Kingmaker.UnitLogic;
using Kingmaker.UnitLogic.Abilities;
using Kingmaker.UnitLogic.Abilities.Blueprints;
using Kingmaker.UnitLogic.FactLogic;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Starion.BPExtender.UnitFact
{
    public class Container
    {
        public bool dispel_success_on_20;
        public bool spell_pen_success_on_20;
    }

    internal static class Extension
    {
        internal static Container GetExtraData(this BlueprintUnitFact __instance)
        {
            if (EXData.Data.UnitFactContainer.ContainsKey(__instance.AssetGuid.m_Guid.ToString()))
            {
                return EXData.Data.UnitFactContainer[__instance.AssetGuid.m_Guid.ToString()];
            }
            return null;
        }

        internal static void CreateExtraData(this BlueprintUnitFact __instance)
        {
            if (__instance.GetExtraData() == null)
            {
                EXData.Data.UnitFactContainer.Add(__instance.AssetGuid.m_Guid.ToString(), new Container());
            }
        }

        internal static void CreateComponents(this BlueprintUnitFact __instance, params BlueprintComponent[] components)
        {
            __instance.ComponentsArray = __instance.ComponentsArray.Concat(components).ToArray();
            var names = new HashSet<string>();
            foreach (var c in __instance.ComponentsArray)
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
            __instance.OnEnable();
        }

        internal static void CreateComponent<T>(this BlueprintUnitFact __instance, Action<T> init = null) where T : BlueprintComponent, new()
        {
            var comp = new T();
            init?.Invoke(comp);
            __instance.CreateComponents(comp);
        }

        /// <summary>
        /// Add a metamagic feat that doesn't exist in the base game.
        /// </summary>
        /// <param name="metamagic">A different enum from the regular metamagic one.</param>
        internal static void AddNewMetamagic(this BlueprintUnitFact __instance, MetamagicExtender.ExtraMetamagic metamagic)
        {
            var comp = new AddMetamagicFeat();
            comp.Metamagic = (Metamagic)metamagic;
            __instance.CreateComponents(comp);
        }
    }

    [AllowedOn(typeof(BlueprintProgression), false)]
    [TypeId("78e379fb-a496-419f-ab83-8dd22dfae0a3")]
    public class SpellListContainer : BlueprintComponent
    {
        public Dictionary<BlueprintAbility, int> spell_list = new Dictionary<BlueprintAbility, int>();
    }

    [AllowedOn(typeof(BlueprintUnitFact), false)]
    [TypeId("fb656052-fe08-4ad4-a8f9-b13854adf460")]
    public class ReduceMetamagicCostForSpellFromFeatureGroups : UnitFactComponentDelegate, IInitiatorRulebookHandler<RuleApplyMetamagic>,
    IRulebookHandler<RuleApplyMetamagic>, ISubscriber, IInitiatorRulebookSubscriber
    {
        public int value;
        public FeatureGroup group = FeatureGroup.None;
        public FeatureGroup group2 = FeatureGroup.None;

        public void OnEventAboutToTrigger(RuleApplyMetamagic evt)
        {
            if (evt.Spellbook == null || evt.Initiator != Owner) { return; }
            foreach (var fact in Owner.Progression.Features)
            {
                if (!fact.Blueprint.Groups.Contains(group) && !fact.Blueprint.Groups.Contains(group2))
                {
                    continue;
                }
                var progression = (BlueprintProgression)fact.Blueprint;
                var level = Owner.Progression.GetProgression(progression).Level;
                var container = fact.GetComponent<SpellListContainer>();
                if (container != null && container.spell_list.ContainsKey(evt.Spell) && container.spell_list[evt.Spell] <= level &&
                    evt.AppliedMetamagics.Count > 0)
                {
                    evt.ReduceCost(value);
                    return;
                }
            }
        }

        public void OnEventDidTrigger(RuleApplyMetamagic evt)
        {
        }
    }

    [HarmonyPatch(typeof(RuleDispelMagic))]
    internal class UnitFactExtender_RuleDispelMagic
    {
        [HarmonyPatch("IsSuccessRoll", typeof(int))]
        [HarmonyPostfix]
        private static void UFE_IsSuccessRoll(RuleDispelMagic __instance, ref bool __result, int d20)
        {
            if (d20 != 20) { return; }
            var d20_auto_success = EXData.FetchUnitFactContainers(__instance.Initiator.Facts).Select(f => f.dispel_success_on_20 == true).Count() > 0;
            if (d20_auto_success) { __result = true; }
        }
    }

    [HarmonyPatch(typeof(RuleSpellResistanceCheck))]
    internal class UnitFactExtender_RuleSpellResistanceCheck
    {
        [HarmonyPatch("get_IsSpellResisted")]
        [HarmonyPostfix]
        private static void UFE_get_IsSpellResisted(RuleSpellResistanceCheck __instance, ref bool __result)
        {
            if (__instance.Roll.Result != 20) { return; }
            var d20_auto_success = EXData.FetchUnitFactContainers(__instance.Initiator.Facts).Select(f => f.spell_pen_success_on_20 == true).Count() > 0;
            if (d20_auto_success) { __result = true; }
        }
    }
}