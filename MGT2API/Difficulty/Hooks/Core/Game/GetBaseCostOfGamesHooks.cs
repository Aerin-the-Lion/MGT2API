using HarmonyLib;
using MGT2API.Difficulty.Adjustments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace MGT2API.Difficulty.Hooks
{
    [HarmonyPatch]
    internal static class GetBaseCostOfGamesHooks
    {
        /// <summary>
        /// Patches the custom multiplier for the base cost of games.
        /// [JP] ゲームの基本コストに適用するカスタム乗数をパッチします。
        /// </summary>
        /// <param name="__instance"></param>
        /// <param name="__result"></param>
        [HarmonyPatch(typeof(games), "GetGrundkosten")]
        internal static void Postfix(ref games __instance, ref float __result)
        {
            //------------------------------------------
            //Todo : ここで、オリジナルコードを使うか、使わないかの判定処理
            //return true; //オリジナルコードを使う
            //------------------------------------------

            //Debug.Log("================================================");
            //Debug.Log("games.GetGrundkosten.Postfix");
            //Debug.Log("================================================");

            __result =　GameBaseCostMultiplier.Instance.ApplyMultiplier(__result);
        }





        /*
        //リファクタリング後のコード
        public float GetBaseCost()
        {
            float difficultyFactor = (float)this.mS_.difficulty * 0.5f;
            const float baseCost = 2.5f; // 基本コスト

            return baseCost + difficultyFactor;
        }
        */
    }
}
