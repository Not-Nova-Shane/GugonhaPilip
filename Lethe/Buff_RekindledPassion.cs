using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GugoPilip
{
    internal class Buff_RekindledPassion
    {
        public static BUFF_UNIQUE_KEYWORD Rekindled_Passion = Lethe.Patches.CustomBuffs.ParseBuffUniqueKeyword("ENTER_BUFF_ID_HERE");
        public static bool hasRecoveredBreak = false;

        [HarmonyPatch(typeof(BattleObjectManager), nameof(BattleObjectManager.OnStageStart_Model))]
        [HarmonyPostfix]
        private static void StageStart()
        {
            hasRecoveredBreak = false;
        }

        [HarmonyPatch(typeof(BuffModel), nameof(BuffModel.OnRoundStart_After_Event))]
        [HarmonyPostfix]
        private static void SpendScorchfield(BattleUnitModel unit, BATTLE_EVENT_TIMING timing, BuffModel __instance)
        {
            if (__instance.GetMainKeyword() != Rekindled_Passion) { return; }
            BattleUnitBuffManager bufManager = Singleton<BattleUnitBuffManager>.Instance;
            if (bufManager == null)
            {
                Main.log.LogError("BufManager was null, stopping Rekindled scorchfield consume");
                return;
            }
            StageBuffManager stageBufManager = bufManager._stageBuffManager;
            StageBuffModel Scorchfield;
            stageBufManager._stageBuffDictionary.TryGetValue(BUFF_UNIQUE_KEYWORD.FireField, out Scorchfield);
            if (Scorchfield == null)
            {
                Main.log.LogError("Couldn't find scorchfield.");
                return;
            }

            int a =  Scorchfield.Stack.GetDecrypted();
            if (a >= 20)
            {
                Scorchfield.UseBuffStack(unit, BUFF_UNIQUE_KEYWORD.FireField, 20, timing);
                unit.AddBuff_Giver(BUFF_UNIQUE_KEYWORD.AttackUp, 2, unit, timing, 0, 0, ABILITY_SOURCE_TYPE.BUFF, null, out _, out _, out _, out _);
                unit.AddBuff_Giver(BUFF_UNIQUE_KEYWORD.DefenseUp, 2, unit, timing, 0, 0, ABILITY_SOURCE_TYPE.BUFF, null, out _, out _, out _, out _);
            }
            else if (a >= 10)
            {
                Scorchfield.UseBuffStack(unit, BUFF_UNIQUE_KEYWORD.FireField, a, timing);
                unit.AddBuff_Giver(BUFF_UNIQUE_KEYWORD.AttackUp, 1, unit, timing, 0, 0, ABILITY_SOURCE_TYPE.BUFF, null, out _, out _, out _, out _);
                unit.AddBuff_Giver(BUFF_UNIQUE_KEYWORD.DefenseUp, 1, unit, timing, 0, 0, ABILITY_SOURCE_TYPE.BUFF, null, out _, out _, out _, out _);
            }
        }

        [HarmonyPatch(typeof(BuffModel), nameof(BuffModel.OnRoundEnd))]
        [HarmonyPostfix]
        private static void DecreaseBurnRE(BattleUnitModel unit, BATTLE_EVENT_TIMING timing, BuffModel __instance)
        {
            if (__instance.GetMainKeyword() != Rekindled_Passion) { return; }
            unit.LoseBuffStack(BUFF_CATEGORY_KEYWORD.COMBUSTION, 10, timing);
        }

        [HarmonyPatch(typeof(BuffModel), nameof(BuffModel.GetAttackLevelAdder))]
        [HarmonyPostfix]
        private static void IncreaseOffenseLevelByBurn(BattleUnitModel unit, BuffModel __instance, ref int __result)
        {
            if (__instance.GetMainKeyword() != Rekindled_Passion) { return; }
            BuffInfo a = unit.GetActivatedBuf(BUFF_UNIQUE_KEYWORD.Combustion);
            if (a == null) { return; }
            int stack = a.GetStack();
            int adder = stack / 10;
            if (adder > 3) { adder = 3; }
            __result += adder;
        }

        [HarmonyPatch(typeof(BuffModel), nameof(BuffModel.OnBreak))]
        [HarmonyPostfix]
        private static void IncreaseOffenseLevelByBurn(BattleUnitModel unit, BATTLE_EVENT_TIMING timing, bool isBreakForcely, BuffModel __instance)
        {
            if (__instance.GetMainKeyword() != Rekindled_Passion) { return; }
            if (!hasRecoveredBreak)
            {
                unit.RecoverBreakExceptForcedIncludeNextTurn();
                hasRecoveredBreak = true;
            }
        }

        [HarmonyPatch(typeof(BuffModel), nameof(BuffModel.OnRoundEnd))]
        [HarmonyPrefix]
        private static void LoseBurnTurn(BattleUnitModel unit, BATTLE_EVENT_TIMING timing)
        {
            if (unit.GetActivatedBuf(Rekindled_Passion) == null) { return; }
            var burn = unit.GetActivatedBuf(BUFF_UNIQUE_KEYWORD.Combustion);
            if (burn == null)
            {
                unit.DestroyBuff(Rekindled_Passion, timing);
            }
        }

    }
}
