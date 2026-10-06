using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GugoPilip
{
    internal class Passive_Sunset
    {
        public static BattleUnitModel dawnFausU = null;
        public static BattleUnitModel dawnGregU = null;
        public static BattleUnitModel pilip = null;
        public static bool fausDiedNOOO = false;
        public static bool gregDiedNOOO = false;

        [HarmonyPatch(typeof(BattleObjectManager), nameof(BattleObjectManager.OnStageStart_Model))]
        [HarmonyPostfix]
        private static void resetstate()
        {
            dawnFausU= null;
            dawnGregU= null;
            fausDiedNOOO = false;
            gregDiedNOOO= false;
            pilip = null;
        }

        [HarmonyPatch(typeof(BattleObjectManager), nameof(BattleObjectManager.OnRoundStart_Model))]
        [HarmonyPostfix]
        private static void CheckIfApplyDawnTag(BATTLE_EVENT_TIMING timing, BattleObjectManager __instance)
        {
            var alive = __instance.GetAliveList(false, UNIT_FACTION.PLAYER);
            bool dawnFaus = false;
            bool dawnGreg = false;
            foreach (BattleUnitModel target in alive)
            {
                if (target.GetUnitID() == 11216)
                {
                    dawnGreg = true;
                    dawnGregU = target;
                }

                if (target.GetUnitID() == 10216)
                {
                    dawnFaus = true;
                    dawnFausU = target;
                }

                if (target.GetUnitID() == Passive_CelloEnsemble.pilipUnitId)
                {
                    pilip = target;
                }
            }

            if (dawnFaus && dawnGreg)
            {
                pilip.AddUnitKeyword(UNIT_KEYWORD.DAWN);
                pilip.AddBuff_Giver(Buff_PastEmbers.Past_Embers, 1, pilip, timing, 0, 0, ABILITY_SOURCE_TYPE.PASSIVE, null, out _, out _, out _, out _);
            }

        }

        [HarmonyPatch(typeof(BattleUnitModel), nameof(BattleUnitModel.OnDie))]
        [HarmonyPostfix]
        private static void CheckIfApplyDawnTag(BattleUnitModel __instance, BATTLE_EVENT_TIMING timing)
        {
            if (__instance.GetUnitID() == dawnFausU.GetUnitID())
            {
                fausDiedNOOO = true;
            }

            if (__instance.GetUnitID() == dawnGregU.GetUnitID())
            {
                gregDiedNOOO = true;
            }

            if (fausDiedNOOO && gregDiedNOOO)
            {
                pilip.AddBuff_Giver(Buff_VanishingMind.Vanishing_Mind, 1, pilip, timing, 0, 0, ABILITY_SOURCE_TYPE.PASSIVE, null, out _, out _, out _, out _);
                pilip.DestroyBuff(Buff_PastEmbers.Past_Embers, timing);
            }
        }

    }
}
