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
    internal static class GetDevCostsForEngineHooks
    {
        /// <summary>
        /// Patches the custom multiplier for the development cost of engines.
        /// [JP] ゲームエンジンの開発コストに適用するカスタム乗数をパッチします。
        /// </summary>
        /// <param name="__instance"></param>
        /// <param name="__result"></param>
        [HarmonyPatch(typeof(engineFeatures), "GetDevCostsForEngine")]
        internal static void Postfix(ref engineFeatures __instance, ref int __result)
        {
            //------------------------------------------
            //Todo : ここで、オリジナルコードを使うか、使わないかの判定処理
            //return true; //オリジナルコードを使う
            //------------------------------------------

            //Debug.Log("================================================");
            //Debug.Log(nameof(GetDevCostsForEngineHooks.Postfix));
            //Debug.Log("================================================");

            __result = Mathf.RoundToInt(EngineDevCostMultiplier.Instance.ApplyMultiplier(__result));
        }
    }

    /*
     * オリジナルコード、リファクタリング案
        public int GetDevCostsForEngine(int engineIndex)
        {
            float difficultyMultiplier = 1.25f + this.mS_.difficulty * 0.2f;
            int baseDevCost = this.engineFeatures_DEV_COSTS[engineIndex];
            int adjustedDevCost = Mathf.RoundToInt(baseDevCost * difficultyMultiplier);

            // 小数点以下を切り捨てて、200の倍数に調整
            int roundedDevCost = Mathf.RoundToInt((float)adjustedDevCost / 200) * 200;

            return roundedDevCost;
        }
    */
}
