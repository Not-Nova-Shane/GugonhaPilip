using HarmonyLib;

namespace GugoPilip
{
    internal class Buff_Overheat
    {
        public static BUFF_UNIQUE_KEYWORD Overheat = Lethe.Patches.CustomBuffs.ParseBuffUniqueKeyword("ENTER_BUFF_ID_HERE");
        public static int ExtraBurnInflictedThisTurn = 0;
        public static int ExtraVibrationInflictedThisTurn = 0;

        [HarmonyPatch(typeof(BattleObjectManager), nameof(BattleObjectManager.OnRoundStart_Model))]
        [HarmonyPostfix]
        private static void resetadders()
        {
            ExtraVibrationInflictedThisTurn = 0;
            ExtraBurnInflictedThisTurn = 0;
        }

        [HarmonyPatch(typeof(BuffModel), nameof(BuffModel.GetAttackDmgMultiplier))]
        [HarmonyPostfix]
        private static void IncreaseDamageMul(BattleActionModel action, ref float __result, BuffModel __instance)
        {
            if (__instance.GetMainKeyword() != Overheat) { return; }
            __result += 30 * 0.01f;
        }

        [HarmonyPatch(typeof(SkillModel), nameof(SkillModel.GetAttackDmgMultiplier))] // patching Class SkillModel, targeting the specific method GetAttackDmgMultiplier
        [HarmonyPrefix]
        private static void IncreaseDamageTaken(BattleActionModel action, CoinModel coin, BattleUnitModel target, ref float __result, SkillModel __instance)
        {
            if (target.GetActivatedBuf(Overheat) == null) { return; }
            __result += 80 * 0.01f;
        }


        [HarmonyPatch(typeof(BuffModel), nameof(BuffModel.OnRoundEnd))] // patching Class SkillModel, targeting the specific method GetAttackDmgMultiplier
        [HarmonyPrefix]
        private static void LoseBurnTurn(BattleUnitModel unit, BATTLE_EVENT_TIMING timing)
        {
            if (unit.GetActivatedBuf(Overheat) == null) { return; }
            unit.LoseBuffTurn(BUFF_UNIQUE_KEYWORD.Combustion, 1, timing);

            var burn = unit.GetActivatedBuf(BUFF_UNIQUE_KEYWORD.Combustion);
            if (burn == null)
            {
                unit.DestroyBuff(Overheat, timing);
            }
        }

        [HarmonyPatch(typeof(BattleUnitModel), nameof(BattleUnitModel.RightAfterAnyUnitGetAnyBuff))] // patching Class BattleUnitModel, targeting the specific method RightAfterAnyUnitGetAnyBuff
        [HarmonyPostfix]
        private static void InflictExtraBullshit(BUFF_UNIQUE_KEYWORD bufKeyword, int stack, int activeRound, BattleUnitModel giverOrNull, ABILITY_SOURCE_TYPE srcType, BATTLE_EVENT_TIMING timing, int overStack, int overTurn)
        {
            if (giverOrNull.GetActivatedBuf(Overheat) == null) { return; } // return if they dont have buff
            if (bufKeyword != BUFF_UNIQUE_KEYWORD.Combustion && bufKeyword != BUFF_UNIQUE_KEYWORD.Vibration) { return; } // return if not inflicting burn and vibration
            if (ExtraBurnInflictedThisTurn < 1)
            {
                giverOrNull.AddBuff_Giver(BUFF_UNIQUE_KEYWORD.Combustion, 1, giverOrNull, timing, 0, 0, srcType, null, out _, out _, out _, out _);
                ExtraBurnInflictedThisTurn += 1;
            }

            if (ExtraVibrationInflictedThisTurn < 1)
            {
                giverOrNull.AddBuff_Giver(BUFF_UNIQUE_KEYWORD.Vibration, 1, giverOrNull, timing, 0, 0, srcType, null, out _, out _, out _, out _);
                ExtraVibrationInflictedThisTurn += 1;
            }
        }


    }
}
