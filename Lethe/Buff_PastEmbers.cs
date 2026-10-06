using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GugoPilip
{
    internal class Buff_PastEmbers
    {
        public static BUFF_UNIQUE_KEYWORD Past_Embers = Lethe.Patches.CustomBuffs.ParseBuffUniqueKeyword("ENTER_BUFF_ID_HERE");
        public static int ExtraBurnInflictedThisTurn = 0;

        [HarmonyPatch(typeof(BattleObjectManager), nameof(BattleObjectManager.OnRoundStart_Model))]
        [HarmonyPostfix]
        private static void resetadders()
        {
            ExtraBurnInflictedThisTurn = 0;
        }

        [HarmonyPatch(typeof(BuffModel), nameof(BuffModel.OnRoundStart_After_Event))] // patching Class BuffModel, targeting the specific method GetMinSpeedAdder
        [HarmonyPostfix]
        private static void Lose10SPRoundStart(BattleUnitModel unit, BATTLE_EVENT_TIMING timing, BuffModel __instance)
        {
            if (__instance.GetMainKeyword() != Past_Embers) { return; }
            unit.TakeMpDamage(null, 10, timing, DAMAGE_SOURCE_TYPE.BUFF, out int _);
        }

        [HarmonyPatch(typeof(BuffModel), nameof(BuffModel.GetMinSpeedAdder))] // patching Class BuffModel, targeting the specific method GetMinSpeedAdder
        [HarmonyPostfix]
        private static void IncreaseMinSpeed(BattleUnitModel unit, ref int __result, BuffModel __instance)
        {
            if (__instance.GetMainKeyword() != Past_Embers) { return; }
            __result += 2;
        }

        [HarmonyPatch(typeof(BuffModel), nameof(BuffModel.GetAttackDmgMultiplier))] // patching Class BuffModel, targeting the specific method GetAttackDmgMultiplier
        [HarmonyPostfix]
        private static void IncreaseDamageMul(BattleActionModel action, ref float __result, BuffModel __instance)
        {
            if (__instance.GetMainKeyword() != Past_Embers) { return; }
            if (!action.Model.HasSkillAsDefaultSkill(action.Skill)) { return; }
            __result += 10 * 0.01f; 
        }

        [HarmonyPatch(typeof(BuffModel), nameof(BuffModel.GetAttackLevelAdder))] // patching Class BuffModel, targeting the specific method GetAttackLevelAdder
        [HarmonyPostfix]
        private static void IncreaseOffenseLevel(ref int __result, BuffModel __instance) // NOTE: test this one more in game for me gugo, i'm not sure if this is exactly offense level up? it should be, but its diff from v1 decompile
        {
            if (__instance.GetMainKeyword() != Past_Embers) { return; }
            __result += 2; 
        }

        [HarmonyPatch(typeof(BattleUnitModel), nameof(BattleUnitModel.RightAfterAnyUnitGetAnyBuff))] // patching Class BattleUnitModel, targeting the specific method RightAfterAnyUnitGetAnyBuff
        [HarmonyPostfix]
        private static void InflictExtraBurn(BUFF_UNIQUE_KEYWORD bufKeyword, int stack, int activeRound, BattleUnitModel giverOrNull, ABILITY_SOURCE_TYPE srcType, BATTLE_EVENT_TIMING timing, int overStack, int overTurn)
        {
            if (giverOrNull.GetUnitID() != Passive_CooledHeart.pilipUnitId) { return; } // simple unit check to see if giver is pilip
            if (giverOrNull.GetActivatedBuf(Past_Embers) == null) { return; } // return if they dont have buff
            if (bufKeyword != BUFF_UNIQUE_KEYWORD.Combustion) { return; } // return if not inflicting burn
            if (ExtraBurnInflictedThisTurn < 2)
            {
                giverOrNull.AddBuff_Giver(BUFF_UNIQUE_KEYWORD.Combustion, 1, giverOrNull, timing, 0, 0, srcType, null, out _, out _, out _, out _);
                ExtraBurnInflictedThisTurn += 1;
            }
        }

    }
}
