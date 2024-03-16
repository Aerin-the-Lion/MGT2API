using HarmonyLib;
using JetBrains.Annotations;
using MGT2API.Difficulty.Adjustments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace MGT2API.Difficulty.Hooks
{
    /// <summary>
    /// Patch for distributing game pass subscriptions to servers.
    /// [JP] サーバーへのGame Pass定期購読の分配に関するリファクタリング兼カスタム乗数設定するパッチです。
    /// </summary>
    [HarmonyPatch]
    internal class DistributeGamePassSubscriptionsOnServerHooks
    {
        [HarmonyPatch(typeof(gamepassScript), "VerteileAbosAufServer")]
        internal static bool Prefix(ref gamepassScript __instance)
        {
            //------------------------------------------
            //Todo : ここで、オリジナルコードを使うか、使わないかの判定処理
            //return true; //オリジナルコードを使う
            //------------------------------------------

            //Debug.Log("================================================");
            //Debug.Log("gamepassScript.VerteileAbosAufServer.Prefix");
            //Debug.Log("================================================");

            try
            {
                __instance = GamePassSubscriptionManager.DistributeSubscriptionsOnServer(__instance);
            }
            catch (Exception ex)
            {
                Debug.LogError("An error occurred while distributing game pass subscriptions: " + ex.Message);
            }

            return false;
        }

        /// <summary>
        /// このクラスは、ゲームパスの定期購読をサーバーに配布するためのメソッドを提供します。
        /// 元のメソッドは、gamepassScript.VertailAbosAufServerです。
        /// 毎週、ゲームパスの定期購読数を計算し、サーバーに配布します。
        /// Moddingのため、リファクタリングを実施しました。
        /// This class provides methods for distributing game pass subscriptions to servers.
        /// The original method is gamepassScript.VertailAbosAufServer.
        /// Every week, calculate the number of game pass subscriptions and distribute them to servers.
        /// Refactored for modding purposes.
        /// </summary>
        private static class GamePassSubscriptionManager
        {
            private static gamepassScript instance;

            public static gamepassScript DistributeSubscriptionsOnServer(gamepassScript instance)
            {
                GamePassSubscriptionManager.instance = instance;

                // ==================================================
                if (!IsGamePassActive)
                {
                    GamePassSubscriptions = 0L; //gamePassSubscriptions
                    return instance;
                }

                long initialSubscriptions = GamePassSubscriptions;
                GamePassSubscriptions -= CalculateSubscriptionAdjustment(initialSubscriptions);

                ApplySubscriptionPriceChangeEffects();
                GamePassSubscriptions += GamePassSubscriptionLastWeek;
                AdjustSubscriptionForUserLimits();

                DistributeSubscriptionsToServers();

                // Calculate the difference in subscriptions from last week
                GamePassSubscriptionLastWeek = GamePassSubscriptions - initialSubscriptions;
                return GamePassSubscriptionManager.instance;
            }

            private static long CalculateSubscriptionAdjustment(long subscriptions)
            {
                float adjustment = subscriptions;
                adjustment = ApplyDifficultyMultiplier(adjustment);
                adjustment = ApplySubscriptionThresholdMultiplier(adjustment);

                return Mathf.RoundToInt(adjustment);
            }

            /// <summary>
            /// ゲーム難易度の直接的な影響を適用するメソッド
            /// </summary>
            /// <param name="adjustment"></param>
            /// <returns></returns>
            private static float ApplyDifficultyMultiplier(float adjustment)
            {
                float[] difficultyMultipliers = { 0.01f, 0.015f, 0.02f, 0.025f, 0.03f, 0.035f };
                int[][] randomRanges = {
                    new[] {1, 20},
                    new[] {5, 20},
                    new[] {10, 20},
                    new[] {15, 20},
                    new[] {18, 25},
                    new[] {20, 25}
                };
                int gameDifficulty = Traverse.Create(instance).Field("difficulty").GetValue<int>();
                float baseMultiplier = difficultyMultipliers[gameDifficulty];
                float customMultiplier = GamePassSubsDistributionMultiplier.Instance.ApplyMultiplier(baseMultiplier);
                int[] selectedRange = randomRanges[gameDifficulty];
                int randomAddition = UnityEngine.Random.Range(selectedRange[0], selectedRange[1] + 1); // Adjust the range as needed

                return adjustment * customMultiplier + randomAddition;
            }

            /// <summary>
            /// サブスクリプションの登録者数のしきい値による影響を適用するメソッド
            /// それぞれの設定のしきい値を超えた際、サブスクリプションの登録者数に1.2倍の影響を与えます。
            /// </summary>
            /// <param name="adjustment"></param>
            /// <returns></returns>
            private static float ApplySubscriptionThresholdMultiplier(float adjustment)
            {
                long[] thresholds = { 10000000, 50000000, 100000000, 150000000, 200000000, 250000000, 300000000 };
                foreach (long threshold in thresholds)
                {
                    if (GamePassSubscriptions > threshold)
                    {
                        adjustment *= 1.2f;
                    }
                }
                return adjustment;
            }


            /// <summary>
            /// サブスクリプションの価格変更による影響を適用するメソッド
            /// （実際、自分で使ったことがないので、なんとも言えない。）
            /// （ただ、オリジナル実装内容としては、1から10の範囲でSwitch文を回しているので、何かしらあるんじゃないかとは踏んでいるが。）
            /// </summary>
            private static void ApplySubscriptionPriceChangeEffects()
            {
                int priceChange = GamePassSubscriptionPrice - GamePassSubscriptionPriceOld;

                // priceChangeが1以上10以下の範囲である場合に処理を行う
                if (priceChange >= 1 && priceChange <= 10)
                {
                    // priceChangeが9または10の場合は、GamePassSubscriptionsを半分にする
                    if (priceChange == 9 || priceChange == 10)
                    {
                        GamePassSubscriptions /= 2L;
                    }
                    else
                    {
                        //// それ以外の場合は、priceChangeに応じた計算を行う
                        //GamePassSubscriptions -= GamePassSubscriptions / (11L - priceChange);
                    }
                }
                GamePassSubscriptionPriceOld = GamePassSubscriptionPrice;
            }

            private static void AdjustSubscriptionForUserLimits()
            {
                if (GamePassSubscriptions > TotalGamePassUsers)
                {
                    GamePassSubscriptions = TotalGamePassUsers;
                }
            }

            private static void DistributeSubscriptionsToServers()
            {
                long remainingSubscriptions = GamePassSubscriptions;
                mainScript mS_ = Traverse.Create(instance).Field("mS_").GetValue<mainScript>();
                foreach (var roomScript in mS_.arrayRoomScripts)
                {
                    if (roomScript && roomScript.typ == 15 && (roomScript.serverReservieren == 0 || roomScript.serverReservieren == 3))
                    {
                        remainingSubscriptions = roomScript.SetAbos(remainingSubscriptions);
                        if (remainingSubscriptions <= 0L)
                        {
                            break;
                        }
                    }
                }
                GamePassSubscriptions -= remainingSubscriptions;
                if (GamePassSubscriptions < 0L)
                {
                    GamePassSubscriptions = 0L;
                }
            }

            private static bool IsGamePassActive
            {
                get => instance.gamePass_aktiv;
                set => instance.gamePass_aktiv = value;
            }

            private static long GamePassSubscriptions
            {
                get => instance.gamePass_Abos;
                set => instance.gamePass_Abos = value;
            }

            private static int GamePassSubscriptionPrice
            {
                get => instance.gamePass_AboPreis;
                set => instance.gamePass_AboPreis = value;
            }

            private static int GamePassSubscriptionPriceOld
            {
                get => instance.gamePass_AboPreisOld;
                set => instance.gamePass_AboPreisOld = value;
            }

            private static long GamePassSubscriptionLastWeek
            {
                get => instance.gamePass_AbosLetzteWoche;
                set => instance.gamePass_AbosLetzteWoche = value;
            }

            private static long TotalGamePassUsers
            {
                get => instance.gamePass_userGesamt;
                set => instance.gamePass_userGesamt = value;
            }
        }
    }
}
