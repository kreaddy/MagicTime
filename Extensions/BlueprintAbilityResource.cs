using HarmonyLib;
using Kingmaker.Blueprints;
using Kingmaker.EntitySystem.Stats;
using Kingmaker.PubSubSystem;
using Kingmaker.UnitLogic;
using System;

namespace Starion.BPExtender.AbilityResource
{
    public class Container
    {
        public bool half_step = false;
        public bool delayed_spending = false;
    }

    internal static class Extension
    {
        internal static Container GetExtraData(this BlueprintAbilityResource __instance)
        {
            if (EXData.Data.ResourceContainer.ContainsKey(__instance.AssetGuid.m_Guid.ToString()))
            {
                return EXData.Data.ResourceContainer[__instance.AssetGuid.m_Guid.ToString()];
            }
            return null;
        }

        internal static void CreateExtraData(this BlueprintAbilityResource __instance)
        {
            if (__instance.GetExtraData() == null)
            {
                EXData.Data.ResourceContainer.Add(__instance.AssetGuid.m_Guid.ToString(), new Container());
            }
        }

        /// <summary>
        /// The maximum amount of available resource will be based on ability score modifiers.
        /// </summary>
        /// <param name="stat_type">The ability score.</param>
        /// <param name="base_value">The minimum amount.</param>
        /// <param name="half_step">Flag to determine whether only half the ability score modifier is considered.</param>
        internal static void SetIncreasedWithStat(this BlueprintAbilityResource __instance, StatType stat_type, int base_value, bool half_step)
        {
            __instance.m_UseMax = false;
            __instance.m_Min = base_value;
            __instance.m_MaxAmount = new BlueprintAbilityResource.Amount
            {
                BaseValue = base_value,
                IncreasedByStat = true,
                ResourceBonusStat = stat_type
            };
            if (half_step && __instance.GetExtraData() != null ) { __instance.GetExtraData().half_step = true; }
        }

        /// <summary>
        /// Recalculates the max amount of resources based on a stat modifier, but only with half the modifier bonus.
        /// For instance, a Pact Wizard can roll twice his D20s a number of times per day equals to 3 + half his Intelligence modifier.
        /// Automatically assumes the calculation is stat-based since class-based ones already have this functionality.
        /// </summary>
        internal static int RecalculateWithHalfModifier(this BlueprintAbilityResource __instance, UnitDescriptor unit)
        {
            var num = __instance.m_MaxAmount.BaseValue;
            var modifiableValueAttributeStat = unit.Stats.GetStat(__instance.m_MaxAmount.ResourceBonusStat) as ModifiableValueAttributeStat;
            if (modifiableValueAttributeStat != null)
            {
                num += modifiableValueAttributeStat.Bonus / 2;
            }
            var bonus = 0;
            EventBus.RaiseEvent(unit.Unit, delegate (IResourceAmountBonusHandler h)
            {
                h.CalculateMaxResourceAmount(__instance, ref bonus);
            });
            return Math.Max(__instance.m_Min, __instance.ApplyMinMax(num) + bonus);
        }
    }

    [HarmonyPatch(typeof(BlueprintAbilityResource), "GetMaxAmount")]
    internal class EXData_BlueprintAbilityResource
    {
        [HarmonyPostfix]
        private static void EXData_GetMaxAmount(BlueprintAbilityResource __instance, ref int __result, UnitDescriptor unit)
        {
            if (__instance.GetExtraData() != null && __instance.GetExtraData().half_step)
            {
                __result = __instance.RecalculateWithHalfModifier(unit);
            }
        }
    }
}