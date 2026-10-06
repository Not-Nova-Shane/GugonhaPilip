using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GugoPilip
{
    internal class Buff_VanishingMind
    {
        public static BUFF_UNIQUE_KEYWORD Vanishing_Mind = Lethe.Patches.CustomBuffs.ParseBuffUniqueKeyword("ENTER_BUFF_ID_HERE");
        public static int ExtraVibrationInflictedThisTurn = 0;

        [HarmonyPatch(typeof(BattleObjectManager), nameof(BattleObjectManager.OnRoundStart_Model))]
        [HarmonyPostfix]
        private static void resetadders()
        {
            ExtraVibrationInflictedThisTurn = 0;
        }

        [HarmonyPatch(typeof(BuffModel), nameof(BuffModel.GetSkillPowerAdder))] 
        [HarmonyPostfix]
        private static void GainBasePower(BuffModel __instance, ref int __result)
        {
            if (__instance.GetMainKeyword() != Vanishing_Mind) { return; }
            __result += 2;
        }

        [HarmonyPatch(typeof(BuffModel), nameof(BuffModel.GetCoinScaleAdder))]
        [HarmonyPostfix]
        private static void LoseCoinPower(BuffModel __instance, ref int __result)
        {
            if (__instance.GetMainKeyword() != Vanishing_Mind) { return; }
            __result -= 3;
        }

        [HarmonyPatch(typeof(BuffModel), nameof(BuffModel.GetMaxSpeedAdder))]
        [HarmonyPostfix]
        private static void LoseMaxSpeed(BuffModel __instance, ref int __result)
        {
            if (__instance.GetMainKeyword() != Vanishing_Mind) { return; }
            __result -= 2;
        }

        [HarmonyPatch(typeof(BuffModel), nameof(BuffModel.GetAttackDmgMultiplier))] 
        [HarmonyPostfix]
        private static void IncreaseDamageMul(BattleActionModel action, ref float __result, BuffModel __instance)
        {
            if (__instance.GetMainKeyword() != Vanishing_Mind) { return; }
            if (!action.Model.HasSkillAsDefaultSkill(action.Skill)) { return; }
            __result += 15 * 0.01f;
        }

        [HarmonyPatch(typeof(BuffModel), nameof(BuffModel.GetDefenseLevelAdder))] 
        [HarmonyPostfix]
        private static void IncreaseDefenseLevel( ref int __result, BuffModel __instance)
        {
            if (__instance.GetMainKeyword() != Vanishing_Mind) { return; }
            __result += 4;
        }

        [HarmonyPatch(typeof(BattleUnitModel), nameof(BattleUnitModel.OnChangeMp))] 
        [HarmonyPostfix]
        private static void InflictExtraTremor(int oldMp, int newMp, BattleUnitModel __instance)
        {
            if (__instance.GetUnitID() != Passive_CooledHeart.pilipUnitId) { return; } // simple unit check to see if giver is pilip
            if (__instance.GetActivatedBuf(Vanishing_Mind) == null) { return; } // return if they dont have buff
            if (newMp >= 25)
            {
                __instance.ChangeMp(25);
            }
        }



    }
}
