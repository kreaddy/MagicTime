using HarmonyLib;
using Kingmaker.RuleSystem.Rules;
using Kingmaker.UI.Common;
using Kingmaker.UI.MVVM._VM.ServiceWindows.Spellbook.Metamagic;
using Kingmaker.UnitLogic;
using Kingmaker.UnitLogic.Abilities;
using Kingmaker.UnitLogic.Abilities.Blueprints;
using Kingmaker.UnitLogic.FactLogic;
using Kingmaker.UnitLogic.Mechanics;
using Kingmaker.UnitLogic.Mechanics.Components;
using MagicTime.Utilities;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Starion
{
    public static class MetamagicExtender
    {
        [Flags]
        public enum ExtraMetamagic
        {
            Intensified = 2048
        }

        public static Dictionary<ExtraMetamagic, int> MetamagicDefaultCost = new Dictionary<ExtraMetamagic, int>()
        {
            { ExtraMetamagic.Intensified, 1 }
        };

        public static bool IsMetamagicAvailableForThisSpell(ExtraMetamagic metamagic, BlueprintAbility spell)
        {
            switch (metamagic)
            {
                case ExtraMetamagic.Intensified:
                    return spell.AvailableMetamagic.HasMetamagic(Metamagic.Bolstered);

                default:
                    return false;
            }
        }

        public static int CalculateIntensifiedSpellDices(ContextRankConfig crc, MechanicsContext context)
        {
            var value = crc.ApplyProgression(crc.GetBaseValue(context));
            return Math.Min(value, crc.m_Max + 5);
        }
    }

    [HarmonyPatch(typeof(RuleApplyMetamagic), "OnTrigger")]
    internal class MetamagicExtender_RuleApplyMetamagic
    {
        [HarmonyPostfix]
        private static void ME_OnTrigger(RuleApplyMetamagic __instance)
        {
            var lv_adjustment = 0;
            if (__instance.AppliedMetamagics.Contains((Metamagic)MetamagicExtender.ExtraMetamagic.Intensified) &&
                __instance.Initiator.HasFact(BPExtender.Mechanics.FavoriteMetamagicIntensified))
            {
                lv_adjustment++;
            }
            __instance.Result.SpellLevelCost -= lv_adjustment;
            var corr = 0;
            if (__instance.AppliedMetamagics.Contains(Metamagic.CompletelyNormal))
            {
                corr = 1;
            }
            if (__instance.BaseLevel + __instance.Result.SpellLevelCost + corr < 0)
            {
                __instance.Result.SpellLevelCost = -__instance.BaseLevel;
            }
        }
    }

    [HarmonyPatch(typeof(RuleCollectMetamagic), "AddMetamagic")]
    internal class MetamagicExtender_RuleCollectMetamagic
    {
        [HarmonyPostfix]
        private static void ME_AddMetamagic(RuleCollectMetamagic __instance, Feature metamagicFeature)
        {
            if (MagicTime.Main.static_constructor_uiutilitytexts_safe == false)
            {
                var old_method = AccessTools.Method(typeof(UIUtilityTexts), "GetMetamagicList");
                var new_method = AccessTools.Method(typeof(MetamagicExtender_UIUtilityTexts), "ME_GetMetamagicList");
                var harmony = new Harmony(MagicTime.Main.Mod.Info.Id);
                harmony.Patch(old_method, postfix: new HarmonyMethod(new_method));
                MagicTime.Main.static_constructor_uiutilitytexts_safe = true;
            }

            if (!__instance.KnownMetamagics.Contains(metamagicFeature)) { return; }
            if (__instance.m_SpellLevel < 0)
            {
                return;
            }
            if (__instance.m_SpellLevel >= 10)
            {
                return;
            }
            AddMetamagicFeat component = metamagicFeature.GetComponent<AddMetamagicFeat>();
            Metamagic metamagic = component.Metamagic;
            if (__instance.m_SpellLevel + component.Metamagic.DefaultCost() > 10)
            {
                return;
            }
            if (__instance.Spell != null && !__instance.SpellMetamagics.Contains(metamagicFeature) &&
                MetamagicExtender.IsMetamagicAvailableForThisSpell((MetamagicExtender.ExtraMetamagic)metamagic, __instance.Spell))
            {
                __instance.SpellMetamagics.Add(metamagicFeature);
            }
        }
    }

    [HarmonyPatch(typeof(ContextRankConfig), "GetValue", typeof(MechanicsContext))]
    internal class MetamagicExtender_ContextRankConfig
    {
        [HarmonyPrefix]
        private static bool ME_GetValue_Prefix(ContextRankConfig __instance, MechanicsContext context, out int __state)
        {
            __state = 0;
            if (__instance.m_BaseValueType != ContextRankBaseValueType.CasterLevel) { return true; }
            if (context.MaybeCaster == null) { return true; }
            if (context.HasMetamagic((Metamagic)MetamagicExtender.ExtraMetamagic.Intensified))
            {
                __state = MetamagicExtender.CalculateIntensifiedSpellDices(__instance, context);
            }
            return true;
        }

        [HarmonyPostfix]
        private static void ME_GetValue_Postfix(ref int __result, int __state)
        {
            if (__state > 0 && __state > __result)
            {
                __result = __state;
            }
        }
    }

    [HarmonyPatch(typeof(MetamagicHelper))]
    internal class MetamagicExtender_MetamagicHelper
    {
        [HarmonyPatch("DefaultCost")]
        [HarmonyPostfix]
        private static void ME_DefaultCost(ref int __result, Metamagic metamagic)
        {
            switch ((MetamagicExtender.ExtraMetamagic)metamagic)
            {
                case MetamagicExtender.ExtraMetamagic.Intensified:
                    __result = 1;
                    break;
            }
        }

        [HarmonyPatch("SpellIcon")]
        [HarmonyPostfix]
        private static void ME_SpellIcon(ref Sprite __result, Metamagic metamagic)
        {
            switch ((MetamagicExtender.ExtraMetamagic)metamagic)
            {
                case MetamagicExtender.ExtraMetamagic.Intensified:
                    __result = AssetLoader.LoadInternal("Icons", "m_intensified");
                    break;
            }
        }
    }

    [HarmonyPatch(typeof(SpellbookMetamagicSelectorVM))]
    internal class MetamagicExtender_SpellbookMetamagicSelectorVM
    {
        [HarmonyPatch("GetCost")]
        [HarmonyPostfix]
        private static void ME_GetCost(SpellbookMetamagicSelectorVM __instance, ref int __result, Metamagic metamagic)
        {
            if ((MetamagicExtender.ExtraMetamagic)metamagic == MetamagicExtender.ExtraMetamagic.Intensified)
            {
                if (__instance.m_Unit.Value.HasFact(BPExtender.Mechanics.FavoriteMetamagicIntensified))
                {
                    __result--;
                }
            }
        }

        [HarmonyPatch("AddMetamagic")]
        [HarmonyPostfix]
        private static void ME_AddMetamagic(SpellbookMetamagicSelectorVM __instance)
        {
            var corr = 0;
            if (__instance.m_MetamagicBuilder.Value.AppliedMetamagics.Contains(Metamagic.CompletelyNormal))
            {
                corr = 1;
            }
            if (__instance.CurrentTemporarySpell.Value.SpellLevel < __instance.m_MetamagicBuilder.Value.BaseSpellLevel - corr)
            {
                __instance.CurrentTemporarySpell.Value.SpellLevel = __instance.m_MetamagicBuilder.Value.BaseSpellLevel - corr;
                __instance.m_MetamagicBuilder.Value.ResultSpellLevel = __instance.m_MetamagicBuilder.Value.BaseSpellLevel - corr;
            }
        }

        [HarmonyPatch("RemoveMetamagic")]
        [HarmonyPostfix]
        private static void ME_RemoveMetamagic(SpellbookMetamagicSelectorVM __instance)
        {
            var corr = 0;
            if (__instance.m_MetamagicBuilder.Value.AppliedMetamagics.Contains(Metamagic.CompletelyNormal))
            {
                corr = 1;
            }
            if (__instance.CurrentTemporarySpell.Value.SpellLevel < __instance.m_MetamagicBuilder.Value.BaseSpellLevel - corr)
            {
                __instance.CurrentTemporarySpell.Value.SpellLevel = __instance.m_MetamagicBuilder.Value.BaseSpellLevel - corr;
                __instance.m_MetamagicBuilder.Value.ResultSpellLevel = __instance.m_MetamagicBuilder.Value.BaseSpellLevel - corr;
            }
        }
    }

    internal static class MetamagicExtender_UIUtilityTexts
    {
        private static void ME_GetMetamagicList(ref string __result, Metamagic mask)
        {
            var to_append = "";

            if ((mask & (Metamagic)MetamagicExtender.ExtraMetamagic.Intensified) != 0)
            {
                to_append += "Intensified, ";
            }
            if (to_append.Length > 2)
            {
                to_append = to_append.Substring(0, to_append.Length - 2);
            }
            if (__result.Length > 0)
            {
                __result += ", ";
            }

            __result += to_append;
        }
    }
}