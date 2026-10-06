using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GugoPilip
{
    internal class Passive_CooledHeart
    {
        public static int pilipUnitId = 67128; // gugo, edit this when you recieve the git

        [HarmonyPatch(typeof(BattleUnitModel), nameof(BattleUnitModel.RightAfterAnyUnitGetAnyBuff))] // patching Class BattleUnitModel, targeting the specific method RightAfterAnyUnitGetAnyBuff
        [HarmonyPrefix]
        private static void CapBurnPotency(BattleUnitModel receiver, BUFF_UNIQUE_KEYWORD bufKeyword, int activeRound, BattleUnitModel giverOrNull, ABILITY_SOURCE_TYPE srcType, BATTLE_EVENT_TIMING timing, int overStack, int overTurn)
        {
            if (receiver.GetUnitID() != pilipUnitId) { return; } // simple unit check
            BuffInfo burn = receiver.GetActivatedBuf(BUFF_UNIQUE_KEYWORD.Combustion);
            if (burn == null) return; // if pilip has no burn, retreat!!

            int stack = burn.GetStack();
            if (stack > 50)
                receiver.LoseBuffStack(bufKeyword, (stack - 50), timing);
        }

        [HarmonyPatch(typeof(BattleUnitModel), nameof(BattleUnitModel.ChangeTakeDamage))] // patching Class BattleUnitModel, targeting the specific method ChangeTakeDamage
        [HarmonyPostfix]
        private static void ChangeBurnDamage(BUFF_UNIQUE_KEYWORD keyword, BATTLE_EVENT_TIMING timing, DAMAGE_SOURCE_TYPE dmgSrcType, BattleUnitModel __instance, ref int __result)
        {
            // an __instance is a harmony overload that grabs the instance of the class this patch is running on
            // so, in this case, the __instance points to a live battle unit. We then check if it was burn damage and was also pilip.
            // the ref int result is attempting to grab the damage result that exists in the method, and set it here

            if (__instance.GetUnitID() != pilipUnitId) { return; } // simple unit check
            if (dmgSrcType != DAMAGE_SOURCE_TYPE.BUFF) { return; } // must be a buff dealing damage
            if (keyword != BUFF_UNIQUE_KEYWORD.Combustion) { return; } // must be burn

            int SP = __instance.Mp;
            int dmgToTake = __result;
            int room = Math.Min(dmgToTake, SP - (-40)); // calculate how much SP can be lost before hitting -40
            __instance.TakeMpDamage(null, room, timing, dmgSrcType, out int _);
            __result = 0;
        }

    }
}
