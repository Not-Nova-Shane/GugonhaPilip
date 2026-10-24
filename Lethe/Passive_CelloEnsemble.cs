using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static AbilityEffect_DamageBuffWithDisappear;

namespace GugoPilip
{
    internal class Passive_CelloEnsemble
    {
        public static int pilipUnitId = 67128; // gugo, edit this when you recieve the git
        public static bool hasInflictInst = false;
        public static bool gugoTestLog = true; // set to true for logging stock, left to default for you to test
        public static bool CheckSinRequirement_Cello()
        {
            SinManager sinMgr = Singleton<SinManager>.Instance;
            var sinDict = sinMgr.GetAttributeStock(UNIT_FACTION.PLAYER);
            int slothStock = sinDict[ATTRIBUTE_TYPE.AMBER].GetDecrypted();
            int wrathStock = sinDict[ATTRIBUTE_TYPE.CRIMSON].GetDecrypted();
            int gloomStock = sinDict[ATTRIBUTE_TYPE.AZURE].GetDecrypted();

            if (gugoTestLog)
            {
                Main.log.LogInfo($"Current stocks: Sloth: {slothStock}, Wrath: {wrathStock}, Gloom: {gloomStock}");
            }

            if (slothStock >=2 && wrathStock >=1 && gloomStock >= 2) { return true; }

            return false;
        }

        [HarmonyPatch(typeof(BattleUnitModel), nameof(BattleUnitModel.RightAfterAnyUnitGetAnyBuff))] // patching Class BattleUnitModel, targeting the specific method RightAfterAnyUnitGetAnyBuff
        [HarmonyPrefix]
        private static void InflictCombustionByVibration(BattleUnitModel receiver, BUFF_UNIQUE_KEYWORD bufKeyword, int stack, int activeRound, BattleUnitModel giverOrNull, ABILITY_SOURCE_TYPE srcType, BATTLE_EVENT_TIMING timing, int overStack, int overTurn)
        {
            // tip, to add and know which overloads (parameters) to use, hover the method to see what it takes.
            // this patch adds the "int stack" parameter, so I know how much vibration is being inflicted
            // so I can mirror that onto burn.
            if (!CheckSinRequirement_Cello()) { return; }
            if (giverOrNull.GetUnitID() != pilipUnitId) { return; } // simple unit check to see if giver is pilip
            if (bufKeyword != BUFF_UNIQUE_KEYWORD.Vibration) { return; } // return if not inflicting tremor
            if (receiver.IsFaction(UNIT_FACTION.PLAYER)) { return; }
            if (hasInflictInst)
            {
                hasInflictInst = false;
                return;
            }
            else
            {
                hasInflictInst = true;  
                giverOrNull.AddBuff_Giver(BUFF_UNIQUE_KEYWORD.Combustion, stack/2, giverOrNull, timing, 0, 0, srcType, null, out _, out _, out _, out _); // out _ means out nothing, you can replace if you need the excess for some reason
            }
        }

        [HarmonyPatch(typeof(BattleUnitModel), nameof(BattleUnitModel.GetMinSpeedAdder))] // patching Class BattleUnitModel, targeting the specific method RightAfterAnyUnitGetAnyBuff
        [HarmonyPrefix]
        private static void IncreaseMinSpeed(BattleUnitModel __instance, ref int __result)
        {
            // the reason that __result and __instance can be used even if this method has no params is because they are unique overloads
            // as __instance is managed by harmony, and ref is able to just grab a var that exists within the method itself
            if (!CheckSinRequirement_Cello()) { return; }
            if (__instance.GetUnitID() != pilipUnitId) { return; } // simple unit check to see if giver is pilip
            BuffInfo burn = __instance.GetActivatedBuf(BUFF_UNIQUE_KEYWORD.Combustion);
            if (burn == null) { return; }
            if (__instance.GetActivatedBuf(BUFF_UNIQUE_KEYWORD.Combustion).GetStack() < 36) { return; }
            __result += 1; // add 1 min speed
        }

        [HarmonyPatch(typeof(BattleUnitModel), nameof(BattleUnitModel.GetMaxSpeedAdder))] // patching Class BattleUnitModel, targeting the specific method RightAfterAnyUnitGetAnyBuff
        [HarmonyPrefix]
        private static void IncreaseMaxSpeed(BattleUnitModel __instance, ref int __result)
        {
            // the reason that __result and __instance can be used even if this method has no params is because they are unique overloads
            // as __instance is managed by harmony, and ref is able to just grab a var that exists within the method itself
            if (!CheckSinRequirement_Cello()) { return; }
            if (__instance.GetUnitID() != pilipUnitId) { return; } // simple unit check to see if giver is pilip
            BuffInfo burn = __instance.GetActivatedBuf(BUFF_UNIQUE_KEYWORD.Combustion);
            if (burn == null) { return; }
            if (__instance.GetActivatedBuf(BUFF_UNIQUE_KEYWORD.Combustion).GetStack() < 36) { return; }
            __result += 1; // add 1 min speed
        }


        [HarmonyPatch(typeof(SkillModel), nameof(SkillModel.GetAttackDmgMultiplier))] // patching Class SkillModel, targeting the specific method GetAttackDmgMultiplier
        [HarmonyPrefix]
        private static void IncreaseDamagePilipBurn(BattleActionModel action, CoinModel coin, ref float __result, SkillModel __instance)
        {
            if (!CheckSinRequirement_Cello()) { return; }
            if (action.Model.GetUnitID() != pilipUnitId) { return; } // model is battleunitmodel
            BuffInfo burn = action.Model.GetActivatedBuf(BUFF_UNIQUE_KEYWORD.Combustion);
            if (burn == null) { return; }
            if (action.Model.GetActivatedBuf(BUFF_UNIQUE_KEYWORD.Combustion).GetStack() < 24) { return; }
            __result += 10 * 0.01f; // * 0.01f is due to how limbabs does mults
            // 10 * 0.01f = 0.1, which is 10% extra damage. you can fact check via dumpeddata probably
        }

        [HarmonyPatch(typeof(SkillModel), nameof(SkillModel.GetSkillPowerAdder))] // patching Class SkillModel, targeting the specific method GetSkillPowerAdder
        [HarmonyPrefix]
        private static void IncreaseBasePowerPilipBurn(BattleActionModel action, ref int __result, SkillModel __instance)
        {
            if (!CheckSinRequirement_Cello()) { return; }
            if (action.Model.GetUnitID() != pilipUnitId) { return; } // model is battleunitmodel
            BuffInfo burn = action.Model.GetActivatedBuf(BUFF_UNIQUE_KEYWORD.Combustion);
            if (burn == null) { return; }
            if (action.Model.GetActivatedBuf(BUFF_UNIQUE_KEYWORD.Combustion).GetStack() < 12) { return; }
            __result += 1; // add 1 base power
        }

        [HarmonyPatch(typeof(SkillModel), nameof(SkillModel.GetSkillPowerResultAdder))] // patching Class SkillModel, targeting the specific method GetSkillPowerResultAdder
        [HarmonyPrefix]
        private static void IncreaseFinalPowerPilipBurn(BattleActionModel action, BATTLE_EVENT_TIMING timing, CoinModel coinOrNull, ref int __result, SkillModel __instance)
        {
            if (!CheckSinRequirement_Cello()) { return; }
            if (action.Model.GetUnitID() != pilipUnitId) { return; } // model is battleunitmodel
            BuffInfo burn = action.Model.GetActivatedBuf(BUFF_UNIQUE_KEYWORD.Combustion);
            if (burn == null) { return; }
            if (action.Model.GetActivatedBuf(BUFF_UNIQUE_KEYWORD.Combustion).GetStack() < 48) { return; }
            __result += 1; // add 1 final power
        }

    }
}
