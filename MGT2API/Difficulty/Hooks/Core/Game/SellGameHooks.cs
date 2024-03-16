using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using HarmonyLib;
using System.Runtime.CompilerServices;
using MGT2API.Difficulty.Adjustments;

namespace MGT2API.Difficulty.Hooks
{
    [HarmonyPatch]
    internal static class SellGameHooks
    {
        private enum AchivementType
        {
            PcAndConsoleGamesSold = 0,
            ArcadeGamesSold = 1,
            MobileGamesSold = 2,
            AddonsSold = 3,
            SecondaryMarketingSold = 4,
            InAppPurchases = 5,
            OwnConsolesSold = 6,
            FansAddedWhenGamesSold = 7,
            TrainingSpeedUp = 8,
            WorkRateUp = 9
        }

        [HarmonyPatch(typeof(gameScript), "SellGame")]
        public static bool Prefix(ref gameScript __instance)
        {
            if (!__instance.isOnMarket)
            {
                if (!__instance.inDevelopment && __instance.freigabeBudget > 0)
                {
                    __instance.freigabeBudget--;
                }
                return false;
            }
            if (__instance.publisherID != -1 && !__instance.pS_)
            {
                __instance.FindMyPublisher();
            }
            if (__instance.releaseDate <= 0)
            {
                __instance.weeksOnMarket++;
            }
            else
            {
                __instance.releaseDate--;
            }
            // num 売上の基本倍率、売上計算に使う基本係数？ -> salesBaseRate
            float salesBaseRate = 0.07f;
            // num2 各レビュースコアに基づく売上予測値 -> estimatedSales
            float estimatedSales = 0f;
            // num3 最終的な売上調整係数、追加の売上調整に使われる -> finalAdjustmentFactor
            float finalAdjustmentFactor = 0f;
            // num4 現在の総売上 -> currentTotalSales
            long currentTotalSales = __instance.sellsTotal;
            // num5 ゲームの総レビュースコア -> totalReviewScore
            int totalReviewScore = __instance.reviewTotal;
            // num6 ユーザーレビューに基づく売上係数 -> userReviewSalesFactor
            float userReviewSalesFactor = 1f + __instance.GetUserReviewPercent() * 0.01f;
            if (totalReviewScore < 3)
            {
                estimatedSales = 5f * userReviewSalesFactor;
            }
            if (totalReviewScore >= 3 && totalReviewScore < 5)
            {
                estimatedSales = 30f * userReviewSalesFactor;
            }
            if (totalReviewScore >= 5 && totalReviewScore < 10)
            {
                estimatedSales = 200f * userReviewSalesFactor;
            }
            if (totalReviewScore >= 10 && totalReviewScore < 20)
            {
                estimatedSales = 300f * userReviewSalesFactor;
            }
            if (totalReviewScore >= 20 && totalReviewScore < 30)
            {
                estimatedSales = 500f * userReviewSalesFactor;
            }
            if (totalReviewScore >= 30 && totalReviewScore < 40)
            {
                estimatedSales = 1000f * userReviewSalesFactor;
            }
            if (totalReviewScore >= 40 && totalReviewScore < 50)
            {
                estimatedSales = 2000f * userReviewSalesFactor;
            }
            if (totalReviewScore >= 50 && totalReviewScore < 60)
            {
                estimatedSales = 5000f * userReviewSalesFactor;
            }
            if (totalReviewScore >= 60 && totalReviewScore < 70)
            {
                estimatedSales = 8000f * userReviewSalesFactor;
            }
            if (totalReviewScore >= 70 && totalReviewScore < 80)
            {
                estimatedSales = 13000f * userReviewSalesFactor;
            }
            if (totalReviewScore >= 80 && totalReviewScore < 90)
            {
                estimatedSales = 19000f * userReviewSalesFactor;
            }
            if (totalReviewScore >= 90 && totalReviewScore < 95)
            {
                estimatedSales = 25000f * userReviewSalesFactor;
            }
            if (totalReviewScore >= 95 && totalReviewScore < 100)
            {
                estimatedSales = 40000f * userReviewSalesFactor;
            }
            if (totalReviewScore >= 100)
            {
                estimatedSales = 100000f * userReviewSalesFactor;
            }
            estimatedSales *= 0.5f;
            if (__instance.debug)
            {
                Debug.Log("GAME " + __instance.myName + " A " + estimatedSales.ToString());
            }
            // num7 総レビュースコアをfloatにキャストしたもの、計算で使用 -> totalReviewScoreFloat
            float totalReviewScoreFloat = (float)__instance.reviewTotal;
            estimatedSales += __instance.points_gameplay * (totalReviewScoreFloat * 0.01f);
            estimatedSales += __instance.points_grafik * (totalReviewScoreFloat * 0.01f);
            estimatedSales += __instance.points_sound * (totalReviewScoreFloat * 0.01f);
            estimatedSales += __instance.points_technik * (totalReviewScoreFloat * 0.01f);
            // -------------------------------------------------------------------------------------

            /*
    (input)
    >> GetIpBekanntheit

    (output)
    Translation:
    - English: "Retrieve IP popularity"
    - Japanese: "IPの知名度を取得する"

    For programming purposes, use:
    - "retrieveIppopularity"
*/
            /*
            if (__instance.mainIP != -1)
            {
                if (!__instance.script_mainIP)
                {
                    __instance.FindMainIpScript();
                }
                if (__instance.script_mainIP)
                {
                    switch (__instance.mS_.difficulty)
                    {
                        case 0:
                            estimatedSales += estimatedSales * __instance.GetIpBekanntheit() * 0.2f;
                            break;
                        case 1:
                            estimatedSales += estimatedSales * __instance.GetIpBekanntheit() * 0.2f * 0.8f;
                            break;
                        case 2:
                            estimatedSales += estimatedSales * __instance.GetIpBekanntheit() * 0.2f * 0.5f;
                            break;
                        case 3:
                            estimatedSales += estimatedSales * __instance.GetIpBekanntheit() * 0.2f * 0.4f;
                            break;
                        case 4:
                            estimatedSales += estimatedSales * __instance.GetIpBekanntheit() * 0.2f * 0.37f;
                            break;
                        case 5:
                            estimatedSales += estimatedSales * __instance.GetIpBekanntheit() * 0.2f * 0.35f;
                            break;
                        default:
                            estimatedSales += estimatedSales * __instance.GetIpBekanntheit() * 0.2f * 0.5f;
                            break;
                    }
                    if (__instance.script_mainIP.merchGesamtReviewPoints < (float)__instance.reviewTotal)
                    {
                        __instance.script_mainIP.merchGesamtReviewPoints = (float)__instance.reviewTotal;
                    }
                }
            }
            */

            // IPの可視性を取得して、売上推定値に応じた倍率を掛けるメソッド
            float CalculateSalesBasedOnIpPopularityMultiplier(int difficulty, float ipPopularity)
            {
                // 難易度に応じた倍率を格納する辞書
                Dictionary<int, float> difficultyMultipliers = new Dictionary<int, float>
                    {
                        {0, 1.0f},
                        {1, 0.8f},
                        {2, 0.5f},
                        {3, 0.4f},
                        {4, 0.37f},
                        {5, 0.35f},
                    };

                // デフォルトの倍率は0.5とする
                float defaultMultiplier = 0.5f;

                // 辞書から倍率を取得、キーがなければデフォルト値を使用
                float multiplier = difficultyMultipliers.ContainsKey(difficulty) ? difficultyMultipliers[difficulty] : defaultMultiplier;

                // IPの可視性に基づいて売上推定値の倍率を計算
                return ipPopularity * 0.2f * multiplier;
            }

            if (__instance.mainIP != -1)
            {
                if (!__instance.script_mainIP)
                {
                    __instance.FindMainIpScript();
                }
                if (__instance.script_mainIP)
                {
                    //CalculateSalesBasedOnIpPopularity
                    float ipPopularity = __instance.GetIpBekanntheit(); // IPの可視性を取得
                    float baseSalesMultiplier = CalculateSalesBasedOnIpPopularityMultiplier(__instance.mS_.difficulty, ipPopularity); // 売上倍率を計算
                    float customSalesMultiplier = GameSalesBasedOnIpPopularityMultiplier.Instance.ApplyMultiplier(baseSalesMultiplier);
                    estimatedSales += estimatedSales * customSalesMultiplier; // 推定売上に倍率を適用

                    if (__instance.script_mainIP.merchGesamtReviewPoints < (float)__instance.reviewTotal)
                    {
                        __instance.script_mainIP.merchGesamtReviewPoints = (float)__instance.reviewTotal;
                    }
                }
            }
            // -------------------------------------------------------------------------------------

            if (__instance.ExistAutomatenspiel())
            {
                estimatedSales += estimatedSales * 0.2f;
            }
            if (__instance.nachfolger_created)
            {
                if (__instance.debug)
                {
                    Debug.Log("GAME " + __instance.myName + " B " + estimatedSales.ToString());
                }
                if (!__instance.script_nachfolger)
                {
                    __instance.FindNachfolgerScript();
                }
                if (__instance.script_nachfolger && __instance.script_nachfolger.isOnMarket)
                {
                    estimatedSales *= 0.6f;
                }
            }
            if (__instance.typ_nachfolger)
            {
                if (!__instance.script_vorgaenger)
                {
                    __instance.FindVorgaengerScript();
                }
                if (__instance.script_vorgaenger)
                {
                    if (__instance.weeksOnMarket > 2)
                    {
                        if (__instance.script_vorgaenger.reviewTotal < 5)
                        {
                            estimatedSales -= 1000f;
                        }
                        if (__instance.script_vorgaenger.reviewTotal >= 5 && __instance.script_vorgaenger.reviewTotal < 10)
                        {
                            estimatedSales -= 500f;
                        }
                        if (__instance.script_vorgaenger.reviewTotal >= 10 && __instance.script_vorgaenger.reviewTotal < 20)
                        {
                            estimatedSales -= 300f;
                        }
                        if (__instance.script_vorgaenger.reviewTotal >= 20 && __instance.script_vorgaenger.reviewTotal < 30)
                        {
                            estimatedSales -= 100f;
                        }
                        if (__instance.script_vorgaenger.reviewTotal >= 30 && __instance.script_vorgaenger.reviewTotal < 40)
                        {
                            estimatedSales -= 50f;
                        }
                    }
                    if (__instance.script_vorgaenger.reviewTotal >= 40 && __instance.script_vorgaenger.reviewTotal < 50)
                    {
                        estimatedSales += 50f;
                    }
                    if (__instance.script_vorgaenger.reviewTotal >= 50 && __instance.script_vorgaenger.reviewTotal < 60)
                    {
                        estimatedSales += 1000f;
                    }
                    if (__instance.script_vorgaenger.reviewTotal >= 60 && __instance.script_vorgaenger.reviewTotal < 70)
                    {
                        estimatedSales += 2000f;
                    }
                    if (__instance.script_vorgaenger.reviewTotal >= 70 && __instance.script_vorgaenger.reviewTotal < 80)
                    {
                        estimatedSales += 3000f;
                    }
                    if (__instance.script_vorgaenger.reviewTotal >= 80 && __instance.script_vorgaenger.reviewTotal < 90)
                    {
                        estimatedSales += 4000f;
                    }
                    if (__instance.script_vorgaenger.reviewTotal >= 90 && __instance.script_vorgaenger.reviewTotal < 95)
                    {
                        estimatedSales += 5000f;
                    }
                    if (__instance.script_vorgaenger.reviewTotal >= 95 && __instance.script_vorgaenger.reviewTotal < 100)
                    {
                        estimatedSales += 10000f;
                    }
                    if (__instance.script_vorgaenger.reviewTotal >= 100)
                    {
                        estimatedSales += 15000f;
                    }
                }
            }
            if (__instance.IsMyGame())
            {
                // num8 レビュースコアに基づく売上係数 -> reviewScoreMultiplier
                // num9 ジャンルファンによる売上ボーナス -> genreFansSalesBonus
                // num10 メインジャンルファンによる売上ボーナス -> mainGenreFansSalesBonus
                // num11 サブジャンルが存在する場合のサブジャンルファンによる売上ボーナス -> subGenreFansSalesBonus
                float reviewScoreMultiplier = (float)__instance.reviewTotal * 0.01f;
                float genreFansSalesBonus = reviewScoreMultiplier * (float)__instance.genres_.GetAmountFans() * 0.005f;
                float mainGenreFansSalesBonus = reviewScoreMultiplier * (float)__instance.genres_.genres_FANS[__instance.maingenre] * 0.05f;
                float subGenreFansSalesBonus = 0f;
                if (__instance.subgenre != -1)
                {
                    subGenreFansSalesBonus = reviewScoreMultiplier * (float)__instance.genres_.genres_FANS[__instance.subgenre] * 0.01f;
                }
                estimatedSales += genreFansSalesBonus + mainGenreFansSalesBonus + subGenreFansSalesBonus;
            }
            else
            {
                //num12 1975年からの経過年数 -> yearsSince1975
                //num13 レビュースコアに基づく売上係数 -> reviewScoreMultiplier2
                //num14 レビュースコアに基づく売上増加1 -> reviewScoreBasedSales1
                //num15 レビュースコアに基づく売上増加2 -> reviewScoreBasedSales2
                int yearsSince1975 = __instance.mS_.year - 1975;
                float reviewScoreMultiplier2 = (float)__instance.reviewTotal * 0.01f;
                float reviewScoreBasedSales1 = reviewScoreMultiplier2 * (float)(50000 * yearsSince1975) * 0.001f;
                float reviewScoreBasedSales2 = reviewScoreMultiplier2 * (float)(5000 * yearsSince1975) * 0.01f;
                estimatedSales += reviewScoreBasedSales1 + reviewScoreBasedSales2;
            }
            switch (__instance.gameTyp)
            {
                case 0:
                    if (!__instance.arcade)
                    {
                        // num16 週ごとの売上減衰係数 -> weeklySalesDecayFactor
                        float weeklySalesDecayFactor = 1f - (float)__instance.weeksOnMarket * 0.01f;
                        if (weeklySalesDecayFactor >= 0f)
                        {
                            estimatedSales *= weeklySalesDecayFactor;
                        }
                        else
                        {
                            estimatedSales *= 0f;
                        }
                        estimatedSales -= (float)(__instance.weeksOnMarket * __instance.weeksOnMarket * 3);
                    }
                    else
                    {
                        // num17 アーケードの週ごとの売上減衰係数 -> arcadeWeeklySalesDecayFactor
                        float arcadeWeeklySalesDecayFactor = 1f - (float)__instance.weeksOnMarket * 0.003f;
                        if (arcadeWeeklySalesDecayFactor >= 0f)
                        {
                            estimatedSales *= arcadeWeeklySalesDecayFactor;
                        }
                        else
                        {
                            estimatedSales *= 0f;
                        }
                        estimatedSales -= (float)(__instance.weeksOnMarket * __instance.weeksOnMarket);
                    }
                    break;
                case 1:
                    estimatedSales *= __instance.mmoInteresse * 0.01f;
                    if (__instance.IsMyGame())
                    {
                        __instance.AddMMOInteresse(-UnityEngine.Random.Range(0.3f, 0.5f));
                    }
                    else
                    {
                        __instance.AddMMOInteresse(-UnityEngine.Random.Range(0.1f, 0.3f));
                    }
                    break;
                case 2:
                    estimatedSales *= __instance.f2pInteresse * 0.01f;
                    if (__instance.IsMyGame())
                    {
                        __instance.AddF2PInteresse(-UnityEngine.Random.Range(0.3f, 0.5f));
                    }
                    else
                    {
                        __instance.AddF2PInteresse(-UnityEngine.Random.Range(0.1f, 0.3f));
                    }
                    break;
            }
            if (estimatedSales < 0f)
            {
                estimatedSales = 0f;
            }
            if (!__instance.arcade)
            {
                estimatedSales *= __instance.games_.GetSells();
            }
            else
            {
                estimatedSales *= __instance.games_.GetSellsArcade();
            }
            if (__instance.gameTyp == 1 || __instance.gameTyp == 2 || __instance.typ_mmoaddon)
            {
                estimatedSales *= __instance.games_.GetInternetUser();
            }
            switch (__instance.weeksOnMarket)
            {
                case 1:
                    estimatedSales *= 1.3f;
                    break;
                case 2:
                    estimatedSales *= 1.4f;
                    break;
                case 3:
                    estimatedSales *= 1.5f;
                    break;
                case 4:
                    estimatedSales *= 1.4f;
                    break;
                case 5:
                    estimatedSales *= 1.1f;
                    break;
            }
            if (__instance.IsMyGame())
            {
                if (__instance.typ_nachfolger || __instance.typ_remaster || __instance.typ_spinoff || __instance.typ_standard)
                {
                    if (!__instance.handy && !__instance.arcade)
                    {
                        //num18 PCとコンソールのゲームの売上に応じたAchivementsボーナスの追加売上係数 -> PcAndConsoleAchivementsBonus
                        float PcAndConsoleAchivementsBonus = (float)__instance.mS_.GetAchivementBonus(0);
                        PcAndConsoleAchivementsBonus *= 0.01f;
                        finalAdjustmentFactor += PcAndConsoleAchivementsBonus;
                    }
                    if (__instance.arcade)
                    {
                        //num19 アーケードゲームの売上に応じたAchivementsボーナスの追加売上係数 -> ArcadeAchivementsBonus
                        float ArcadeAchivementsBonus = (float)__instance.mS_.GetAchivementBonus(1);
                        ArcadeAchivementsBonus *= 0.01f;
                        finalAdjustmentFactor += ArcadeAchivementsBonus;
                    }
                    if (__instance.handy)
                    {
                        //num20 携帯ゲームの売上に応じたAchivementsボーナスの追加売上係数 -> MobileGameAchivementsBonus
                        float MobileGameAchivementsBonus = (float)__instance.mS_.GetAchivementBonus(2);
                        MobileGameAchivementsBonus *= 0.01f;
                        finalAdjustmentFactor += MobileGameAchivementsBonus;
                    }
                }
                if (__instance.typ_addon || __instance.typ_addonStandalone || __instance.typ_mmoaddon)
                {
                    //num21 アドオンゲームの売上に応じたAchivementsボーナスの追加売上係数 -> AddonAchivementsBonus
                    float AddonAchivementsBonus = (float)__instance.mS_.GetAchivementBonus(3);
                    AddonAchivementsBonus *= 0.01f;
                    finalAdjustmentFactor += AddonAchivementsBonus;
                }
                if (__instance.typ_budget || __instance.typ_bundle || __instance.typ_bundleAddon || __instance.typ_goty)
                {
                    //num22 セカンダリマーケティングの売上に応じたAchivementsボーナスの追加売上係数 -> SecondaryMarketingAchivementsBonus
                    float SecondaryMarketingAchivementsBonus = (float)__instance.mS_.GetAchivementBonus(4);
                    SecondaryMarketingAchivementsBonus *= 0.01f;
                    finalAdjustmentFactor += SecondaryMarketingAchivementsBonus;
                }
            }
            switch (__instance.usk)
            {
                case 0:
                    finalAdjustmentFactor += 0.1f;
                    break;
                case 1:
                    finalAdjustmentFactor += 0.1f;
                    break;
                case 2:
                    finalAdjustmentFactor += 0.05f;
                    break;
                case 3:
                    finalAdjustmentFactor += 0f;
                    break;
                case 4:
                    finalAdjustmentFactor -= 0.05f;
                    break;
                case 5:
                    finalAdjustmentFactor -= 0.2f;
                    break;
            }
            if (!__instance.gameLanguage[0])
            {
                finalAdjustmentFactor -= 0.05f;
            }
            if (!__instance.gameLanguage[1])
            {
                finalAdjustmentFactor -= 0.03f;
            }
            if (!__instance.gameLanguage[2])
            {
                finalAdjustmentFactor -= 0.03f;
            }
            if (!__instance.gameLanguage[3])
            {
                finalAdjustmentFactor -= 0.02f;
            }
            if (!__instance.gameLanguage[4])
            {
                finalAdjustmentFactor -= 0.02f;
            }
            if (!__instance.gameLanguage[5])
            {
                finalAdjustmentFactor -= 0.02f;
            }
            if (!__instance.gameLanguage[6])
            {
                finalAdjustmentFactor -= 0.01f;
            }
            if (!__instance.gameLanguage[7])
            {
                finalAdjustmentFactor -= 0.02f;
            }
            if (!__instance.gameLanguage[8])
            {
                finalAdjustmentFactor -= 0.02f;
            }
            if (!__instance.gameLanguage[9])
            {
                finalAdjustmentFactor -= 0.03f;
            }
            if (!__instance.gameLanguage[10])
            {
                finalAdjustmentFactor -= 0.04f;
            }
            if (!__instance.typ_bundle)
            {
                if (__instance.mS_.trendGenre == __instance.maingenre)
                {
                    finalAdjustmentFactor += 0.33f;
                }
                if (__instance.mS_.trendTheme == __instance.gameMainTheme)
                {
                    finalAdjustmentFactor += 0.15f;
                }
                if (__instance.mS_.trendAntiGenre == __instance.maingenre)
                {
                    finalAdjustmentFactor -= 0.33f;
                }
                if (__instance.mS_.trendAntiTheme == __instance.gameMainTheme)
                {
                    finalAdjustmentFactor -= 0.15f;
                }
                if (__instance.mS_.trendGenre == __instance.subgenre)
                {
                    finalAdjustmentFactor += 0.1f;
                }
                if (__instance.mS_.trendTheme == __instance.gameSubTheme)
                {
                    finalAdjustmentFactor += 0.05f;
                }
                if (__instance.mS_.trendAntiGenre == __instance.subgenre)
                {
                    finalAdjustmentFactor -= 0.1f;
                }
                if (__instance.mS_.trendAntiTheme == __instance.gameSubTheme)
                {
                    finalAdjustmentFactor -= 0.05f;
                }
            }
            finalAdjustmentFactor += __instance.GetHype() * 0.01f;
            if (!__instance.typ_bundle && !__instance.arcade)
            {
                if (__instance.gameCopyProtect != -1)
                {
                    if (!__instance.gameCopyProtectScript_)
                    {
                        GameObject gameObject = GameObject.Find("COPYPROTECT_" + __instance.gameCopyProtect.ToString());
                        if (gameObject)
                        {
                            __instance.gameCopyProtectScript_ = gameObject.GetComponent<copyProtectScript>();
                        }
                        else
                        {
                            __instance.gameCopyProtect = -1;
                        }
                    }
                    if (__instance.gameCopyProtectScript_)
                    {
                        finalAdjustmentFactor += __instance.gameCopyProtectScript_.effekt * 0.002f;
                    }
                }
                if (__instance.gameAntiCheat != -1 && (__instance.gameplayFeatures_DevDone[21] || __instance.gameplayFeatures_DevDone[23]))
                {
                    if (!__instance.gameAntiCheatScript_)
                    {
                        GameObject gameObject2 = GameObject.Find("ANTICHEAT_" + __instance.gameAntiCheat.ToString());
                        if (gameObject2)
                        {
                            __instance.gameAntiCheatScript_ = gameObject2.GetComponent<antiCheatScript>();
                        }
                        else
                        {
                            __instance.gameAntiCheat = -1;
                        }
                    }
                    if (__instance.gameAntiCheatScript_)
                    {
                        finalAdjustmentFactor += __instance.gameAntiCheatScript_.effekt * 0.003f;
                    }
                }
            }
            if (__instance.publisherID != __instance.mS_.myID && __instance.pS_)
            {
                if (__instance.maingenre == __instance.pS_.fanGenre)
                {
                    finalAdjustmentFactor += 0.2f;
                }
                finalAdjustmentFactor += __instance.pS_.stars * 0.01f;
            }
            if (!__instance.arcade)
            {
                if (__instance.mS_.month == 12 || __instance.mS_.month == 1)
                {
                    finalAdjustmentFactor += 0.5f;
                }
                if (__instance.mS_.month == 6 || __instance.mS_.month == 7)
                {
                    finalAdjustmentFactor -= 0.3f;
                }
            }
            if (__instance.IsMyGame() && __instance.mS_.awardBonus > 0 && __instance.mS_.awardBonusAmount > 0f)
            {
                finalAdjustmentFactor += __instance.mS_.awardBonusAmount;
            }
            if (!__instance.arcade)
            {
                finalAdjustmentFactor += __instance.bonusSellsUpdates;
            }
            else
            {
                finalAdjustmentFactor += __instance.bonusSellsUpdates * 0.2f;
            }
            finalAdjustmentFactor += __instance.bonusSellsAddons;
            finalAdjustmentFactor += __instance.addonQuality;
            if (finalAdjustmentFactor < -0.5f)
            {
                finalAdjustmentFactor = -0.5f;
            }
            estimatedSales *= 1f + finalAdjustmentFactor;
            if (!__instance.arcade)
            {
                // num23 これはゲームの基本的な売上予測係数を表しています。ゲームが独占的である場合には値が増加します。baseSalesCoefficient。
                float baseSalesCoefficient = 10f;
                if (__instance.exklusiv)
                {
                    baseSalesCoefficient = 25f;
                }
                for (int i = 0; i < __instance.gamePlatform.Length; i++)
                {
                    if (__instance.gamePlatform[i] != -1)
                    {
                        if (!__instance.gamePlatformScript[i])
                        {
                            __instance.FindMyPlatforms();
                        }
                        if (__instance.gamePlatformScript[i])
                        {
                            baseSalesCoefficient += __instance.gamePlatformScript[i].GetMarktanteil() * __instance.genres_.GetFloatPlatformSells(__instance.maingenre, __instance.gamePlatformScript[i].typ);
                        }
                    }
                }
                baseSalesCoefficient *= 0.007f;
                estimatedSales *= baseSalesCoefficient;
            }
            else
            {
                estimatedSales *= __instance.genres_.GetFloatPlatformSells(__instance.maingenre, 4);
            }
            if (__instance.IsMyGame())
            {
                if (!__instance.typ_bundle && !__instance.arcade && __instance.mS_.gelangweiltGenre != -1)
                {
                    if (__instance.maingenre == __instance.mS_.gelangweiltGenre)
                    {
                        estimatedSales *= 0.5f;
                    }
                    else if (__instance.subgenre == __instance.mS_.gelangweiltGenre)
                    {
                        estimatedSales *= 0.85f;
                    }
                }
                if (__instance.mS_.sauerBugs > 0)
                {
                    estimatedSales *= 0.7f;
                }
                if (__instance.mS_.schlechteSpiele > 0)
                {
                    estimatedSales *= 0.6f;
                }
                if (!__instance.typ_bundle && !__instance.typ_addon && !__instance.typ_mmoaddon)
                {
                    Vector4 amountGamesWithGenreAndTopic = __instance.games_.GetAmountGamesWithGenreAndTopic(__instance);
                    // num24 ジャンルのレベルに基づいた競争の影響を計算するために使われます。 -> genreCompetitionFactor
                    // 1/1800に近い数値
                    float genreCompetitionFactor = 0.00055555557f * (float)__instance.genres_.genres_LEVEL.Length;
                    // num25 テーマのレベルに基づいた競争の影響を計算するために使われます。 -> themeCompetitionFactor
                    //  1/30000に近い数値
                    float themeCompetitionFactor = 3.3333334E-05f * (float)__instance.themes_.themes_LEVEL.Length;
                    //num26 これは最終的な競争の影響を表しており、ゲームの売上予測にマイナスの影響を与えます。 -> totalCompetitionImpact
                    float totalCompetitionImpact = amountGamesWithGenreAndTopic.x * genreCompetitionFactor + amountGamesWithGenreAndTopic.y * themeCompetitionFactor + amountGamesWithGenreAndTopic.z * genreCompetitionFactor * 2.5f + amountGamesWithGenreAndTopic.w * themeCompetitionFactor * 2.5f;
                    switch (__instance.mS_.settings_competition)
                    {
                        case 1:
                            totalCompetitionImpact *= 2f;
                            break;
                        case 2:
                            totalCompetitionImpact *= 3f;
                            break;
                    }
                    /*
                    switch (__instance.mS_.difficulty)
                    {
                        case 0:
                            if (totalCompetitionImpact > 0.5f)
                            {
                                totalCompetitionImpact = 0.5f;
                            }
                            break;
                        case 1:
                            if (totalCompetitionImpact > 0.55f)
                            {
                                totalCompetitionImpact = 0.55f;
                            }
                            break;
                        case 2:
                            if (totalCompetitionImpact > 0.6f)
                            {
                                totalCompetitionImpact = 0.6f;
                            }
                            break;
                        case 3:
                            if (totalCompetitionImpact > 0.65f)
                            {
                                totalCompetitionImpact = 0.65f;
                            }
                            break;
                        case 4:
                            if (totalCompetitionImpact > 0.7f)
                            {
                                totalCompetitionImpact = 0.7f;
                            }
                            break;
                        case 5:
                            if (totalCompetitionImpact > 0.75f)
                            {
                                totalCompetitionImpact = 0.75f;
                            }
                            break;
                    }
                    estimatedSales *= 1f - totalCompetitionImpact;
                    */

                    estimatedSales *= SetGameSalesCompetitionImpact(__instance.mS_.difficulty, totalCompetitionImpact);

                }
            }
            if (__instance.gameLicence != -1)
            {
                if (__instance.licences_.licence_GENREGOOD[__instance.gameLicence] == __instance.maingenre)
                {
                    estimatedSales += estimatedSales * (__instance.licences_.licence_QUALITY[__instance.gameLicence] * 0.01f * 0.8f);
                }
                if (__instance.licences_.licence_GENREGOOD[__instance.gameLicence] != __instance.maingenre && __instance.licences_.licence_GENREBAD[__instance.gameLicence] != __instance.maingenre)
                {
                    estimatedSales += estimatedSales * (__instance.licences_.licence_QUALITY[__instance.gameLicence] * 0.01f * 0.3f);
                }
                if (__instance.licences_.licence_GENREBAD[__instance.gameLicence] == __instance.maingenre)
                {
                    estimatedSales -= estimatedSales * (__instance.licences_.licence_QUALITY[__instance.gameLicence] * 0.01f * 0.3f);
                }
                if (__instance.subgenre != -1)
                {
                    if (__instance.licences_.licence_GENREGOOD[__instance.gameLicence] == __instance.subgenre)
                    {
                        estimatedSales += estimatedSales * (__instance.licences_.licence_QUALITY[__instance.gameLicence] * 0.01f * 0.2f);
                    }
                    if (__instance.licences_.licence_GENREGOOD[__instance.gameLicence] != __instance.subgenre && __instance.licences_.licence_GENREBAD[__instance.gameLicence] != __instance.subgenre)
                    {
                        estimatedSales += estimatedSales * (__instance.licences_.licence_QUALITY[__instance.gameLicence] * 0.01f * 0.05f);
                    }
                    if (__instance.licences_.licence_GENREBAD[__instance.gameLicence] == __instance.subgenre)
                    {
                        estimatedSales -= estimatedSales * (__instance.licences_.licence_QUALITY[__instance.gameLicence] * 0.01f * 0.1f);
                    }
                }
            }
            //num27: これはメインジャンルの人気度を表しています。トレンドによってこの値が変化するため、mainGenrePopularityと名付けるのが適切です。
            float mainGenrePopularity = __instance.genres_.genres_BELIEBTHEIT[__instance.maingenre];
            if (__instance.maingenre == __instance.mS_.trendGenre)
            {
                mainGenrePopularity = 100f;
            }
            if (__instance.maingenre == __instance.mS_.trendAntiGenre)
            {
                mainGenrePopularity = 20f;
            }
            //num28: これは人気度に基づいて調整された売上の一部を表します。popularityAdjustedSalesという名前にリファクタリングします。
            float popularityAdjustedSales = estimatedSales * 0.5f * (mainGenrePopularity * 0.01f);
            estimatedSales *= 0.8f;
            estimatedSales += popularityAdjustedSales;
            if (!__instance.mS_.settings_sabotageOff && __instance.mS_.sabotage_erwischt > 0)
            {
                estimatedSales *= 0.5f;
            }
            if (!__instance.mS_.settings_sabotageOff && __instance.mS_.sabotage_geruecht > 0)
            {
                estimatedSales *= 0.8f;
            }
            if (__instance.mS_.globalEvent == 0)
            {
                estimatedSales *= 0.5f;
            }
            if (__instance.mS_.globalEvent == 1)
            {
                estimatedSales *= 1.5f;
            }
            if (__instance.newGenreCombination)
            {
                estimatedSales *= 1.1f;
            }
            if (__instance.newTopicCombination)
            {
                estimatedSales *= 1.03f;
            }
            if (__instance.commercialFlop)
            {
                if (__instance.reviewTotal >= 70 && !__instance.typ_bundle && !__instance.typ_bundleAddon && __instance.mS_.trendGenre != __instance.maingenre)
                {
                    estimatedSales *= 0.1f;
                }
                else
                {
                    __instance.commercialFlop = false;
                }
            }
            if (__instance.commercialHit)
            {
                if (__instance.reviewTotal >= 70 && __instance.reviewTotal < 90)
                {
                    estimatedSales *= 2f;
                }
                else
                {
                    __instance.commercialHit = false;
                }
            }
            //-------------------------------------------------------------------------------------
            // ここから下は、ゲームの難易度による売上の減少係数を計算している。
            /*
            switch (__instance.mS_.difficulty)
            {
                case 0:
                    estimatedSales *= 2f;
                    break;
                case 1:
                    estimatedSales *= 1.5f;
                    break;
                case 2:
                    estimatedSales *= 1f;
                    break;
                case 3:
                    estimatedSales *= 0.5f;
                    break;
                case 4:
                    estimatedSales *= 0.35f;
                    break;
                case 5:
                    estimatedSales *= 0.25f;
                    break;
            }
            */
            estimatedSales *= SetGameSalesBasedOnDifficulty(__instance.mS_.difficulty);

            if (__instance.IsMyGame() && __instance.publisherID == __instance.mS_.myID)
            {
                switch (__instance.mS_.GetStudioLevel(__instance.mS_.studioPoints))
                {
                    case 0:
                        estimatedSales *= 0.5f;
                        break;
                    case 1:
                        estimatedSales *= 0.55f;
                        break;
                    case 2:
                        estimatedSales *= 0.6f;
                        break;
                    case 3:
                        estimatedSales *= 0.65f;
                        break;
                    case 4:
                        estimatedSales *= 0.7f;
                        break;
                    case 5:
                        estimatedSales *= 0.75f;
                        break;
                    case 6:
                        estimatedSales *= 0.8f;
                        break;
                    case 7:
                        estimatedSales *= 0.85f;
                        break;
                    case 8:
                        estimatedSales *= 0.9f;
                        break;
                    case 9:
                        estimatedSales *= 0.95f;
                        break;
                }
            }
            if (__instance.mS_.settings_sandbox && __instance.IsMyGame() && __instance.mS_.sandbox_gameSells > 0f)
            {
                estimatedSales *= __instance.mS_.sandbox_gameSells;
            }
            estimatedSales *= salesBaseRate;
            if (__instance.typ_addon)
            {
                estimatedSales *= 0.4f;
                if (!__instance.script_vorgaenger)
                {
                    __instance.FindVorgaengerScript();
                }
                if (__instance.script_vorgaenger && estimatedSales > 0f)
                {
                    if (__instance.script_vorgaenger.amountAddons > 0)
                    {
                        estimatedSales /= (float)__instance.script_vorgaenger.amountAddons;
                    }
                    if (!__instance.script_vorgaenger.isOnMarket && __instance.script_vorgaenger.publisherID != __instance.mS_.myID)
                    {
                        estimatedSales *= 0.8f;
                    }
                    if ((float)__instance.sellsTotal + estimatedSales + (float)__instance.vorbestellungen > (float)__instance.script_vorgaenger.sellsTotal)
                    {
                        estimatedSales = (float)(__instance.script_vorgaenger.sellsTotal - (__instance.sellsTotal + (long)__instance.vorbestellungen));
                    }
                    if (estimatedSales <= 0f)
                    {
                        estimatedSales = 1f;
                    }
                    if (!__instance.script_vorgaenger.isOnMarket && (float)__instance.sellsTotal + estimatedSales > (float)__instance.script_vorgaenger.sellsTotal)
                    {
                        estimatedSales = 0f;
                    }
                }
            }
            if (__instance.typ_addonStandalone)
            {
                estimatedSales *= 0.5f;
                if (!__instance.script_vorgaenger)
                {
                    __instance.FindVorgaengerScript();
                }
                if (__instance.script_vorgaenger && __instance.script_vorgaenger.amountAddons > 0)
                {
                    estimatedSales /= (float)__instance.script_vorgaenger.amountAddons;
                }
            }
            if (__instance.typ_mmoaddon)
            {
                estimatedSales *= 0.65f;
                if (!__instance.script_vorgaenger)
                {
                    __instance.FindVorgaengerScript();
                }
                if (__instance.script_vorgaenger && estimatedSales > 0f)
                {
                    __instance.script_vorgaenger.abosAddons = Mathf.RoundToInt(estimatedSales);
                    if ((float)__instance.sellsTotal + estimatedSales + (float)__instance.vorbestellungen > (float)__instance.script_vorgaenger.sellsTotal)
                    {
                        estimatedSales = (float)(__instance.script_vorgaenger.sellsTotal - (__instance.sellsTotal + (long)__instance.vorbestellungen));
                    }
                    if (estimatedSales <= 0f)
                    {
                        estimatedSales = 1f;
                    }
                }
            }
            if (__instance.gameTyp == 1)
            {
                if (__instance.IsMyGame() && __instance.games_.freeServerPlatz <= 0L)
                {
                    estimatedSales *= 0.05f;
                }
                int amountOfMMOs = __instance.games_.GetAmountOfMMOs();
                // num29: これはMMOの数に基づいた売上の減少係数を表しています。amountOfMMOsSalesDecreaseFactorと名付けるのが適切です。
                float amountOfMMOsSalesDecreaseFactor = 1f + (float)amountOfMMOs * 0.1f;
                if (amountOfMMOs > 0)
                {
                    estimatedSales /= amountOfMMOsSalesDecreaseFactor;
                }
                //num30: 新たに獲得されるアボナメント（サブスクリプション）の予測数です。projectedNewSubscriptions
                float projectedNewSubscriptions = UnityEngine.Random.Range((estimatedSales + (float)__instance.abosAddons) * 0.5f, (estimatedSales + (float)__instance.abosAddons) * 0.7f);
                __instance.abosAddons = 0;
                float baseSubscriptionLoss = (float)__instance.abonnements - (float)__instance.abonnements / 102f * (float)__instance.reviewTotal;
                //num31: これはアボナメント（サブスクリプション）の減少係数を表しています。baseSubscriptionLossと名付けるのが適切です。
                baseSubscriptionLoss *= 0.25f;
                baseSubscriptionLoss += (float)__instance.weeksOnMarket;
                if (__instance.IsMyGame())
                {
                    baseSubscriptionLoss += baseSubscriptionLoss * ((100f - __instance.hype) * 0.01f);
                }
                switch (__instance.aboPreis)
                {
                    case 1:
                        projectedNewSubscriptions *= 1f;
                        break;
                    case 2:
                        projectedNewSubscriptions *= 0.95f;
                        break;
                    case 3:
                        projectedNewSubscriptions *= 0.9f;
                        break;
                    case 4:
                        projectedNewSubscriptions *= 0.8f;
                        break;
                    case 5:
                        projectedNewSubscriptions *= 0.7f;
                        break;
                    case 6:
                        projectedNewSubscriptions *= 0.65f;
                        break;
                    case 7:
                        projectedNewSubscriptions *= 0.6f;
                        break;
                    case 8:
                        projectedNewSubscriptions *= 0.5f;
                        break;
                    case 9:
                        projectedNewSubscriptions *= 0.4f;
                        break;
                    case 10:
                        projectedNewSubscriptions *= 0.2f;
                        break;
                }
                if (amountOfMMOs > 0)
                {
                    projectedNewSubscriptions /= amountOfMMOsSalesDecreaseFactor;
                }
                if (__instance.IsMyGame())
                {
                    projectedNewSubscriptions *= 0.7f;
                }
                else
                {
                    projectedNewSubscriptions *= 1.1f;
                }
                __instance.abonnementsWoche = __instance.abonnements;
                __instance.abonnements -= (long)Mathf.RoundToInt(baseSubscriptionLoss);
                switch (__instance.aboPreis - __instance.aboPreisOld)
                {
                    case 1:
                        __instance.abonnements -= __instance.abonnements / 10L;
                        break;
                    case 2:
                        __instance.abonnements -= __instance.abonnements / 9L;
                        break;
                    case 3:
                        __instance.abonnements -= __instance.abonnements / 8L;
                        break;
                    case 4:
                        __instance.abonnements -= __instance.abonnements / 7L;
                        break;
                    case 5:
                        __instance.abonnements -= __instance.abonnements / 6L;
                        break;
                    case 6:
                        __instance.abonnements -= __instance.abonnements / 5L;
                        break;
                    case 7:
                        __instance.abonnements -= __instance.abonnements / 4L;
                        break;
                    case 8:
                        __instance.abonnements -= __instance.abonnements / 3L;
                        break;
                    case 9:
                        __instance.abonnements -= __instance.abonnements / 2L;
                        break;
                    case 10:
                        __instance.abonnements -= __instance.abonnements / 2L;
                        break;
                }
                __instance.aboPreisOld = __instance.aboPreis;
                __instance.abonnements += (long)Mathf.RoundToInt(projectedNewSubscriptions);
                if (__instance.abonnements > __instance.sellsTotal)
                {
                    __instance.abonnements = __instance.sellsTotal;
                }
                if (__instance.IsMyGame())
                {
                    //num32: アボナメント（サブスクリプション） -> subscriptions
                    long subscriptions = __instance.abonnements;
                    for (int j = 0; j < __instance.mS_.arrayRoomScripts.Length; j++)
                    {
                        if (__instance.mS_.arrayRoomScripts[j] && __instance.mS_.arrayRoomScripts[j].typ == 15 && (__instance.mS_.arrayRoomScripts[j].serverReservieren == 0 || __instance.mS_.arrayRoomScripts[j].serverReservieren == 1))
                        {
                            subscriptions = __instance.mS_.arrayRoomScripts[j].SetAbos(subscriptions);
                            if (subscriptions <= 0L)
                            {
                                break;
                            }
                        }
                    }
                    __instance.abonnements -= subscriptions;
                    __instance.mS_.AddAboverlauf(__instance.abonnements);
                }
                __instance.abonnementsWoche = __instance.abonnements - __instance.abonnementsWoche;
                if (__instance.abonnements < 0L)
                {
                    __instance.abonnements = 0L;
                }
                if (__instance.bestAbonnements < __instance.abonnements)
                {
                    __instance.bestAbonnements = __instance.abonnements;
                }
            }
            if (__instance.handy && __instance.gameTyp == 0)
            {
                estimatedSales *= 2.5f;
            }
            if (__instance.arcade)
            {
                if (__instance.IsMyGame())
                {
                    //num33: これはアーケードゲームの売上予測係数を表しています。arcadeQualitySalesFactorと名付けるのが適切です。
                    float arcadeQualitySalesFactor = (float)(__instance.arcadeCase + __instance.arcadeMonitor + __instance.arcadeJoystick + __instance.arcadeSound);
                    arcadeQualitySalesFactor = 1f + arcadeQualitySalesFactor * 0.05f;
                    estimatedSales *= arcadeQualitySalesFactor;
                }
                else
                {
                    //num34: これはアーケードゲームの売上予測係数を表しています。npcArcadeQualitySalesFactorと名付けるのが適切です。
                    float npcArcadeQualitySalesFactor = (float)(__instance.arcadeCase + __instance.arcadeMonitor + __instance.arcadeJoystick + __instance.arcadeSound);
                    npcArcadeQualitySalesFactor = 1f + npcArcadeQualitySalesFactor * 0.05f;
                    estimatedSales *= npcArcadeQualitySalesFactor;
                    estimatedSales *= 0.3f;
                }
                estimatedSales *= 0.005f;
                if (estimatedSales < 1f && !__instance.IsMyGame() && __instance.weeksOnMarket < 2)
                {
                    estimatedSales = (float)UnityEngine.Random.Range(1, 4);
                }
            }
            if (__instance.gameTyp == 2)
            {
                int amountOfF2Ps = __instance.games_.GetAmountOfF2Ps();
                //num35:  F2P（Free to Play）ゲームの場合、市場に存在するF2Pゲームの数に基づいた調整係数。 -> f2pMarketAdjustmentFactor
                //f2pMarketAdjustmentFactorと名付けるのが適切です。
                float f2pMarketAdjustmentFactor = 1f + (float)amountOfF2Ps * 0.1f;
                estimatedSales *= 4f;
                if (amountOfF2Ps > 0)
                {
                    estimatedSales /= f2pMarketAdjustmentFactor;
                }
                //num36: 新たに獲得されるアボナメント（サブスクリプション）の予測数で、F2Pゲーム用です。
                //projectedNewF2PSubscriptionsと名付けるのが適切です。
                float projectedNewF2PSubscriptions = UnityEngine.Random.Range((estimatedSales + (float)__instance.abosAddons) * 0.3f, (estimatedSales + (float)__instance.abosAddons) * 0.5f);
                __instance.abosAddons = 0;
                //num37: F2Pゲームでのアボナメント（サブスクリプション）が失われる割合を計算するための基礎数値です。baseF2PSubscriptionLossと命名します。
                float baseF2PSubscriptionLoss = (float)__instance.abonnements - (float)__instance.abonnements / 102f * (float)__instance.reviewTotal;
                baseF2PSubscriptionLoss *= 0.25f;
                baseF2PSubscriptionLoss += (float)__instance.weeksOnMarket;
                if (amountOfF2Ps > 0)
                {
                    projectedNewF2PSubscriptions /= (float)amountOfF2Ps;
                }
                __instance.abonnementsWoche = __instance.abonnements;
                __instance.abonnements -= (long)Mathf.RoundToInt(baseF2PSubscriptionLoss);
                __instance.abonnements += (long)Mathf.RoundToInt(projectedNewF2PSubscriptions);
                if (__instance.abonnements > __instance.sellsTotal)
                {
                    __instance.abonnements = __instance.sellsTotal;
                }
                if (__instance.IsMyGame())
                {
                    //num38: アボナメント（サブスクリプション） -> subscriptions
                    long subscriptions = __instance.abonnements;
                    for (int k = 0; k < __instance.mS_.arrayRoomScripts.Length; k++)
                    {
                        if (__instance.mS_.arrayRoomScripts[k] && __instance.mS_.arrayRoomScripts[k] && __instance.mS_.arrayRoomScripts[k].typ == 15 && (__instance.mS_.arrayRoomScripts[k].serverReservieren == 0 || __instance.mS_.arrayRoomScripts[k].serverReservieren == 2))
                        {
                            subscriptions = __instance.mS_.arrayRoomScripts[k].SetAbos(subscriptions);
                            if (subscriptions <= 0L)
                            {
                                break;
                            }
                        }
                    }
                    __instance.abonnements -= subscriptions;
                }
                __instance.abonnementsWoche = __instance.abonnements - __instance.abonnementsWoche;
                if (__instance.abonnements < 0L)
                {
                    __instance.abonnements = 0L;
                    __instance.abonnementsWoche = 0L;
                }
                if (__instance.bestAbonnements < __instance.abonnements)
                {
                    __instance.bestAbonnements = __instance.abonnements;
                }
            }
            if (__instance.typ_budget)
            {
                estimatedSales *= 0.5f;
                //num39: 予算タイトルの場合、ゲームのリリースが過去からどれだけ離れているかに基づく売上の減少係数です。budgetTitleAgingFactorと命名します。
                float budgetTitleAgingFactor = (float)(__instance.mS_.year * (__instance.date_start_year - 1));
                budgetTitleAgingFactor *= 0.05f;
                if (budgetTitleAgingFactor > 0.7f)
                {
                    budgetTitleAgingFactor = 0.7f;
                }
                budgetTitleAgingFactor = 1f - budgetTitleAgingFactor;
                estimatedSales *= budgetTitleAgingFactor;
            }
            if (__instance.typ_remaster)
            {
                estimatedSales *= 0.7f;
            }
            if (__instance.typ_goty)
            {
                estimatedSales *= 0.5f;
                //num40: GOTY版の場合、ゲームのリリースが過去からどれだけ離れているかに基づく売上の減少係数です。gotyAgingFactorと命名します。
                float gotyAgingFactor = (float)(__instance.mS_.year * (__instance.date_start_year - 1));
                gotyAgingFactor *= 0.03f;
                if (gotyAgingFactor > 0.7f)
                {
                    gotyAgingFactor = 0.7f;
                }
                gotyAgingFactor = 1f - gotyAgingFactor;
                estimatedSales *= gotyAgingFactor;
            }
            if (__instance.typ_bundle)
            {
                estimatedSales *= 0.2f;
                // num41 バンドルゲームの売上増加係数 -> bundleSalesIncreaseFactor
                float bundleSalesIncreaseFactor = 0.4f;
                if (__instance.bundleID[0] != -1)
                {
                    bundleSalesIncreaseFactor += 0.1f;
                }
                if (__instance.bundleID[1] != -1)
                {
                    bundleSalesIncreaseFactor += 0.1f;
                }
                if (__instance.bundleID[2] != -1)
                {
                    bundleSalesIncreaseFactor += 0.1f;
                }
                if (__instance.bundleID[3] != -1)
                {
                    bundleSalesIncreaseFactor += 0.1f;
                }
                if (__instance.bundleID[4] != -1)
                {
                    bundleSalesIncreaseFactor += 0.1f;
                }
                estimatedSales *= bundleSalesIncreaseFactor;
            }
            if (__instance.typ_bundleAddon)
            {
                // num42 バンドルアドオンの古さによる売上の減少係数 -> bundleAddonAgingSaleDecreaseFactor
                // ゲームのリリースからの経過年数に基づいて、売上の減少係数を計算
                float bundleAddonAgingSaleDecreaseFactor = (float)(__instance.mS_.year * (__instance.date_start_year - 1));
                bundleAddonAgingSaleDecreaseFactor *= 0.05f;
                if (bundleAddonAgingSaleDecreaseFactor > 0.7f)
                {
                    bundleAddonAgingSaleDecreaseFactor = 0.7f;
                }
                bundleAddonAgingSaleDecreaseFactor = 1f - bundleAddonAgingSaleDecreaseFactor;
                estimatedSales *= bundleAddonAgingSaleDecreaseFactor;
                // num43 バンドルアドオンの売上増加係数 -> bundleAddonSalesIncreaseFactor
                float bundleAddonSalesIncreaseFactor = 0.5f;
                if (__instance.bundleID[0] != -1)
                {
                    bundleAddonSalesIncreaseFactor += 0.05f;
                }
                if (__instance.bundleID[1] != -1)
                {
                    bundleAddonSalesIncreaseFactor += 0.05f;
                }
                if (__instance.bundleID[2] != -1)
                {
                    bundleAddonSalesIncreaseFactor += 0.05f;
                }
                if (__instance.bundleID[3] != -1)
                {
                    bundleAddonSalesIncreaseFactor += 0.05f;
                }
                if (__instance.bundleID[4] != -1)
                {
                    bundleAddonSalesIncreaseFactor += 0.05f;
                }
                estimatedSales *= bundleAddonSalesIncreaseFactor;
            }
            if (!__instance.arcade)
            {
                // num44 ゲームプラットフォームの総ユニット数 -> totalPlatformUnits
                long totalPlatformUnits = 0L;
                if (estimatedSales > 0f)
                {
                    for (int l = 0; l < __instance.gamePlatform.Length; l++)
                    {
                        if (__instance.gamePlatform[l] != -1)
                        {
                            if (!__instance.gamePlatformScript[l])
                            {
                                __instance.FindMyPlatforms();
                            }
                            if (__instance.gamePlatformScript[l])
                            {
                                if (__instance.exklusiv && __instance.gamePlatformScript[l].OwnerIsNPC() && !__instance.gamePlatformScript[l].vomMarktGenommen)
                                {
                                    if (__instance.gameTyp != 2)
                                    {
                                        // num45 プラットフォームごとのエクスクルーシブ販売ボーナス -> exclusivePlatformSaleBonus
                                        // エクスクルーシブ契約に基づいて、各プラットフォームでの販売ボーナスを計算
                                        int exclusivePlatformSaleBonus = Mathf.RoundToInt(UnityEngine.Random.Range((float)__instance.sellsPerWeek[0] * 0.2f, (float)__instance.sellsPerWeek[0] * 0.3f));
                                        exclusivePlatformSaleBonus = exclusivePlatformSaleBonus / 100 * (130 - (int)__instance.gamePlatformScript[l].GetMarktanteil());
                                        if (exclusivePlatformSaleBonus > __instance.sellsPerWeek[0])
                                        {
                                            exclusivePlatformSaleBonus = __instance.sellsPerWeek[0];
                                        }
                                        __instance.exklusivKonsolenSells += (long)exclusivePlatformSaleBonus;
                                        __instance.gamePlatformScript[l].BonusSellsExklusiv(exclusivePlatformSaleBonus);
                                    }
                                    else
                                    {
                                        // num46 MMOゲームにおけるプラットフォームごとのエクスクルーシブ販売ボーナス -> exclusiveMmoPlatformSaleBonus
                                        // MMOゲームの場合、プラットフォームごとのエクスクルーシブ販売ボーナスを計算
                                        int exclusiveMmoPlatformSaleBonus = Mathf.RoundToInt(UnityEngine.Random.Range((float)__instance.sellsPerWeek[0] * 0.2f, (float)__instance.sellsPerWeek[0] * 0.3f));
                                        exclusiveMmoPlatformSaleBonus = exclusiveMmoPlatformSaleBonus / 100 * (130 - (int)__instance.gamePlatformScript[l].GetMarktanteil());
                                        if (exclusiveMmoPlatformSaleBonus > __instance.sellsPerWeek[0])
                                        {
                                            exclusiveMmoPlatformSaleBonus = __instance.sellsPerWeek[0];
                                        }
                                        __instance.exklusivKonsolenSells += (long)(exclusiveMmoPlatformSaleBonus / 5);
                                        __instance.gamePlatformScript[l].BonusSellsExklusiv(exclusiveMmoPlatformSaleBonus);
                                    }
                                }
                                totalPlatformUnits += (long)__instance.gamePlatformScript[l].units;
                            }
                        }
                    }
                    if ((float)(__instance.sellsTotal + (long)__instance.vorbestellungen) + estimatedSales > (float)(totalPlatformUnits / 2L))
                    {
                        //num47: 利用可能なプラットフォームユニットと予測される売上との差分 -> availableUnitsAfterSales
                        long availableUnitsAfterSales = totalPlatformUnits - (__instance.sellsTotal + (long)__instance.vorbestellungen + (long)Mathf.RoundToInt(estimatedSales));
                        if (availableUnitsAfterSales / 10L > __instance.sellsTotal)
                        {
                            __instance.sellsTotal = availableUnitsAfterSales / 10L;
                        }
                    }
                    if ((float)(__instance.sellsTotal + (long)__instance.vorbestellungen) + estimatedSales > (float)totalPlatformUnits)
                    {
                        estimatedSales = (float)(totalPlatformUnits - (__instance.sellsTotal + (long)__instance.vorbestellungen));
                        if (estimatedSales <= 0f)
                        {
                            estimatedSales = 1f;
                        }
                    }
                }
            }
            if (__instance.gameTyp == 0 && __instance.sellsPerWeek[0] > 0 && estimatedSales > 0f)
            {
                // num48 世間のゲームの現実的な評価の係数 -> realismSalesDecreaseImpactFactor
                float realismSalesDecreaseImpactFactor = 0.06f;
                /*
                if (__instance.reviewTotal >= 0 && __instance.reviewTotal < 5 && __instance.sellsTotal > (long)(10000 / (__instance.mS_.difficulty + 1)))
                {
                    __instance.realsticPower += realismSalesDecreaseImpactFactor;
                }
                if (__instance.reviewTotal >= 5 && __instance.reviewTotal < 10 && __instance.sellsTotal > (long)(15000 / (__instance.mS_.difficulty + 1)))
                {
                    __instance.realsticPower += realismSalesDecreaseImpactFactor;
                }
                if (__instance.reviewTotal >= 10 && __instance.reviewTotal < 15 && __instance.sellsTotal > (long)(20000 / (__instance.mS_.difficulty + 1)))
                {
                    __instance.realsticPower += realismSalesDecreaseImpactFactor;
                }
                if (__instance.reviewTotal >= 15 && __instance.reviewTotal < 20 && __instance.sellsTotal > (long)(25000 / (__instance.mS_.difficulty + 1)))
                {
                    __instance.realsticPower += realismSalesDecreaseImpactFactor;
                }
                if (__instance.reviewTotal >= 20 && __instance.reviewTotal < 25 && __instance.sellsTotal > (long)(30000 / (__instance.mS_.difficulty + 1)))
                {
                    __instance.realsticPower += realismSalesDecreaseImpactFactor;
                }
                if (__instance.reviewTotal >= 25 && __instance.reviewTotal < 30 && __instance.sellsTotal > (long)(35000 / (__instance.mS_.difficulty + 1)))
                {
                    __instance.realsticPower += realismSalesDecreaseImpactFactor;
                }
                if (__instance.reviewTotal >= 35 && __instance.reviewTotal < 40 && __instance.sellsTotal > (long)(50000 / (__instance.mS_.difficulty + 1)))
                {
                    __instance.realsticPower += realismSalesDecreaseImpactFactor;
                }
                if (__instance.reviewTotal >= 40 && __instance.reviewTotal < 45 && __instance.sellsTotal > (long)(60000 / (__instance.mS_.difficulty + 1)))
                {
                    __instance.realsticPower += realismSalesDecreaseImpactFactor;
                }
                if (__instance.reviewTotal >= 45 && __instance.reviewTotal < 50 && __instance.sellsTotal > (long)(80000 / (__instance.mS_.difficulty + 1)))
                {
                    __instance.realsticPower += realismSalesDecreaseImpactFactor;
                }
                if (__instance.reviewTotal >= 50 && __instance.reviewTotal < 55 && __instance.sellsTotal > (long)(150000 / (__instance.mS_.difficulty + 1)))
                {
                    __instance.realsticPower += realismSalesDecreaseImpactFactor;
                }
                if (__instance.reviewTotal >= 55 && __instance.reviewTotal < 60 && __instance.sellsTotal > (long)(200000 / (__instance.mS_.difficulty + 1)))
                {
                    __instance.realsticPower += realismSalesDecreaseImpactFactor;
                }
                if (__instance.reviewTotal >= 60 && __instance.reviewTotal < 65 && __instance.sellsTotal > (long)(300000 / (__instance.mS_.difficulty + 1)))
                {
                    __instance.realsticPower += realismSalesDecreaseImpactFactor;
                }
                if (__instance.reviewTotal >= 65 && __instance.reviewTotal < 70 && __instance.sellsTotal > (long)(500000 / (__instance.mS_.difficulty + 1)))
                {
                    __instance.realsticPower += realismSalesDecreaseImpactFactor;
                }
                if (__instance.reviewTotal >= 70 && __instance.reviewTotal < 75 && __instance.sellsTotal > (long)(1000000 / (__instance.mS_.difficulty + 1)))
                {
                    __instance.realsticPower += realismSalesDecreaseImpactFactor;
                }
                if (__instance.reviewTotal >= 75 && __instance.reviewTotal < 80 && __instance.sellsTotal > (long)(2000000 / (__instance.mS_.difficulty + 1)))
                {
                    __instance.realsticPower += realismSalesDecreaseImpactFactor;
                }
                if (__instance.reviewTotal >= 80 && __instance.reviewTotal < 85 && __instance.sellsTotal > (long)(4000000 / (__instance.mS_.difficulty + 1)))
                {
                    __instance.realsticPower += realismSalesDecreaseImpactFactor;
                }
                if (__instance.realsticPower > 0f)
                {
                    if (__instance.realsticPower > 0.98f)
                    {
                        __instance.realsticPower = 0.98f;
                    }
                    estimatedSales *= 1f - __instance.realsticPower;
                }
                if (estimatedSales < 0f)
                {
                    estimatedSales = 0f;
                }
                */

                estimatedSales *= SetGameSalesBasedOnRealisticPowerAndSalesImpact(__instance, realismSalesDecreaseImpactFactor);
                if (estimatedSales < 0f)
                {
                    estimatedSales = 0f;
                }
            }

            if (__instance.HasInAppPurchases())
            {
                if (__instance.gameTyp == 0 && __instance.releaseDate <= 0)
                {
                    // num49 ゲーム内アプリ購入に対する反感による売上の減少量 -> inAppPurchaseHateSalesDecrease
                    float inAppPurchaseHateSalesDecrease = estimatedSales * (__instance.GetInAppPurchaseHate() * 0.01f) * 0.3f;
                    estimatedSales -= inAppPurchaseHateSalesDecrease;
                    float inAppPurchaseMoneyPerWeek = __instance.GetInAppPurchaseMoneyPerWeek();
                    // num50 ゲーム内アプリ購入からの週間収入予測値 -> inAppPurchaseWeeklyRevenueEstimate
                    float inAppPurchaseWeeklyRevenueEstimate = UnityEngine.Random.Range((float)(__instance.sellsTotal / 100L * 2L), (float)(__instance.sellsTotal / 100L * 3L));
                    if (__instance.IsMyGame())
                    {
                        // num51 アチーブメントボーナスによる収入の増加率 -> achievementBonusIncomeIncrease
                        float achievementBonusIncomeIncrease = (float)__instance.mS_.GetAchivementBonus(5);
                        achievementBonusIncomeIncrease *= 0.01f;
                        inAppPurchaseWeeklyRevenueEstimate += inAppPurchaseWeeklyRevenueEstimate * achievementBonusIncomeIncrease;
                    }
                    if (estimatedSales <= 0f)
                    {
                        inAppPurchaseWeeklyRevenueEstimate *= 0.6f;
                    }
                    // num52 市場における週数による収益の減少率 -> marketDurationRevenueDecreaseFactor
                    float marketDurationRevenueDecreaseFactor = 1f - (float)__instance.weeksOnMarket * 0.01f;
                    if (marketDurationRevenueDecreaseFactor < 0.1f)
                    {
                        marketDurationRevenueDecreaseFactor = 0.1f;
                    }
                    inAppPurchaseWeeklyRevenueEstimate *= marketDurationRevenueDecreaseFactor;
                    if (__instance.weeksOnMarket > 5)
                    {
                        inAppPurchaseWeeklyRevenueEstimate -= (float)(__instance.weeksOnMarket * 30);
                    }
                    if (inAppPurchaseWeeklyRevenueEstimate < 0f)
                    {
                        inAppPurchaseWeeklyRevenueEstimate = 0f;
                    }
                    if (inAppPurchaseWeeklyRevenueEstimate > 2E+09f)
                    {
                        inAppPurchaseWeeklyRevenueEstimate = (float)(2000000000 - UnityEngine.Random.Range(0, 100000));
                    }
                    __instance.inAppPurchaseWeek = Mathf.RoundToInt(inAppPurchaseWeeklyRevenueEstimate);
                    // num53 ゲーム内アプリ購入からの実際の週間収益 -> actualInAppPurchaseWeeklyRevenue
                    long actualInAppPurchaseWeeklyRevenue = (long)(inAppPurchaseMoneyPerWeek * (float)Mathf.RoundToInt(inAppPurchaseWeeklyRevenueEstimate));

                    // 総売上とゲーム内アプリ購入による売上に週間収益を追加
                    __instance.umsatzTotal += actualInAppPurchaseWeeklyRevenue;
                    __instance.umsatzInApp += actualInAppPurchaseWeeklyRevenue;
                    if (__instance.IsMyGame())
                    {
                        __instance.mS_.Earn(actualInAppPurchaseWeeklyRevenue, 8);
                    }
                    if (__instance.IsMyGame())
                    {
                        __instance.PayGewinnbeteiligung(actualInAppPurchaseWeeklyRevenue);
                    }
                    if (!__instance.IsMyGame())
                    {
                        __instance.AddTochterfirmaUmsatz(actualInAppPurchaseWeeklyRevenue);
                    }
                }
                if (__instance.gameTyp == 1 && __instance.releaseDate <= 0)
                {
                    // num54 サブスクリプションベースのゲームでのゲーム内アプリ購入に対する反感による売上減少量 -> subscriptionGameInAppPurchaseDislikeSalesDecrease
                    // サブスクリプションベースのゲームにおけるゲーム内アプリ購入への反感が売上に与える減少量。
                    float subscriptionGameInAppPurchaseDislikeSalesDecrease = estimatedSales * (__instance.GetInAppPurchaseHate() * 0.01f) * 0.3f;
                    estimatedSales -= subscriptionGameInAppPurchaseDislikeSalesDecrease;
                    float inAppPurchaseMoneyPerWeek2 = __instance.GetInAppPurchaseMoneyPerWeek();
                    // num55 サブスクリプションからの週間収入予測値 -> subscriptionWeeklyRevenueEstimate
                    float subscriptionWeeklyRevenueEstimate = UnityEngine.Random.Range((float)(__instance.abonnements / 100L * 4L), (float)(__instance.abonnements / 100L * 5L));
                    if (__instance.IsMyGame())
                    {
                        // num56 アチーブメントボーナスによるサブスクリプション収入の増加率 -> subscriptionAchievementBonusIncomeIncrease
                        float subscriptionAchievementBonusIncomeIncrease = (float)__instance.mS_.GetAchivementBonus(5);
                        subscriptionAchievementBonusIncomeIncrease *= 0.01f;
                        subscriptionWeeklyRevenueEstimate += subscriptionWeeklyRevenueEstimate * subscriptionAchievementBonusIncomeIncrease;
                    }
                    if (estimatedSales <= 0f)
                    {
                        subscriptionWeeklyRevenueEstimate *= 0.8f;
                    }
                    if (subscriptionWeeklyRevenueEstimate < 0f)
                    {
                        subscriptionWeeklyRevenueEstimate = 0f;
                    }
                    if (subscriptionWeeklyRevenueEstimate > 2E+09f)
                    {
                        subscriptionWeeklyRevenueEstimate = (float)(2000000000 - UnityEngine.Random.Range(0, 100000));
                    }
                    __instance.inAppPurchaseWeek = Mathf.RoundToInt(subscriptionWeeklyRevenueEstimate);
                    // num57 サブスクリプション型ゲームの実際の週間収益 -> actualSubscriptionWeeklyRevenue
                    long actualSubscriptionWeeklyRevenue = (long)(inAppPurchaseMoneyPerWeek2 * (float)Mathf.RoundToInt(subscriptionWeeklyRevenueEstimate));
                    __instance.umsatzTotal += actualSubscriptionWeeklyRevenue;
                    __instance.umsatzInApp += actualSubscriptionWeeklyRevenue;
                    if (__instance.IsMyGame())
                    {
                        __instance.mS_.Earn(actualSubscriptionWeeklyRevenue, 8);
                    }
                    if (__instance.IsMyGame())
                    {
                        __instance.PayGewinnbeteiligung(actualSubscriptionWeeklyRevenue);
                    }
                    if (!__instance.IsMyGame())
                    {
                        __instance.AddTochterfirmaUmsatz(actualSubscriptionWeeklyRevenue);
                    }
                }
                if (__instance.gameTyp == 2 && __instance.releaseDate <= 0)
                {
                    // num58 ゲームパス型ゲームのインアップ購入に対する反感による売上減少量 -> gamePassInAppPurchaseDislikeSalesDecrease
                    // ゲームパス型ゲームにおけるインアップ購入への反感が売上に与える減少量。
                    float gamePassInAppPurchaseDislikeSalesDecrease = estimatedSales * (__instance.GetInAppPurchaseHate() * 0.01f) * 0.3f;
                    estimatedSales -= gamePassInAppPurchaseDislikeSalesDecrease;
                    float inAppPurchaseMoneyPerWeek3 = __instance.GetInAppPurchaseMoneyPerWeek();
                    // num59 ゲームパス型ゲームからの週間収入予測値 -> gamePassWeeklyRevenueEstimate
                    float gamePassWeeklyRevenueEstimate = UnityEngine.Random.Range(((float)__instance.abonnements + estimatedSales) / 100f * 150f, ((float)__instance.abonnements + estimatedSales) / 100f * 200f);
                    if (__instance.IsMyGame())
                    {
                        // num60 アチーブメントボーナスによるゲームパス収入の増加率 -> gamePassAchievementBonusIncomeIncrease
                        float gamePassAchievementBonusIncomeIncrease = (float)__instance.mS_.GetAchivementBonus(5);
                        gamePassAchievementBonusIncomeIncrease *= 0.01f;
                        gamePassWeeklyRevenueEstimate += gamePassWeeklyRevenueEstimate * gamePassAchievementBonusIncomeIncrease;
                    }
                    if (estimatedSales <= 0f)
                    {
                        gamePassWeeklyRevenueEstimate *= 0.8f;
                    }
                    if (gamePassWeeklyRevenueEstimate < 0f)
                    {
                        gamePassWeeklyRevenueEstimate = 0f;
                    }
                    if (gamePassWeeklyRevenueEstimate > 2E+09f)
                    {
                        gamePassWeeklyRevenueEstimate = (float)(2000000000 - UnityEngine.Random.Range(0, 100000));
                    }
                    __instance.inAppPurchaseWeek = Mathf.RoundToInt(gamePassWeeklyRevenueEstimate);
                    // num61 ゲームパス型ゲームの実際の週間収益 -> actualGamePassWeeklyRevenue
                    long actualGamePassWeeklyRevenue = (long)(inAppPurchaseMoneyPerWeek3 * (float)Mathf.RoundToInt(gamePassWeeklyRevenueEstimate));
                    __instance.umsatzTotal += actualGamePassWeeklyRevenue;
                    __instance.umsatzInApp += actualGamePassWeeklyRevenue;
                    if (__instance.IsMyGame())
                    {
                        __instance.mS_.Earn(actualGamePassWeeklyRevenue, 8);
                    }
                    if (__instance.IsMyGame())
                    {
                        __instance.PayGewinnbeteiligung(actualGamePassWeeklyRevenue);
                    }
                    if (!__instance.IsMyGame())
                    {
                        __instance.AddTochterfirmaUmsatz(actualGamePassWeeklyRevenue);
                    }
                }
            }
            if (__instance.inGamePass)
            {
                // num62 ゲームパスによる売上調整量 -> gamePassSalesAdjustment
                float gamePassSalesAdjustment = 0f;
                if (estimatedSales > 0f)
                {
                    // num63 アクティブなプラットフォームユーザー総数 -> totalActivePlatformUsers
                    long totalActivePlatformUsers = 0L;
                    // num64 ゲームパスを持つプラットフォームのアクティブユーザー数 -> activeGamePassPlatformUsers
                    long activeGamePassPlatformUsers = 0L;
                    for (int m = 0; m < __instance.gamePlatformScript.Length; m++)
                    {
                        if (__instance.gamePlatformScript[m])
                        {
                            totalActivePlatformUsers += (long)__instance.gamePlatformScript[m].GetAktiveNutzer();
                            if (__instance.gamePlatformScript[m].inGamePass || __instance.gamePlatformScript[m].inGamePassPassiv)
                            {
                                activeGamePassPlatformUsers += (long)__instance.gamePlatformScript[m].GetAktiveNutzer();
                            }
                        }
                    }
                    if (totalActivePlatformUsers > 100L)
                    {
                        // num65 ゲームパスプラットフォームユーザーによる売上調整量 -> gamePassUserRatio
                        long gamePassUserRatio = totalActivePlatformUsers / 100L;
                        gamePassUserRatio = activeGamePassPlatformUsers / gamePassUserRatio;
                        gamePassSalesAdjustment = estimatedSales * ((float)gamePassUserRatio * 0.01f);
                        gamePassSalesAdjustment *= __instance.games_.GetGamePassInteressted();
                        gamePassSalesAdjustment *= 0.33f;
                        switch (__instance.gpS_.gamePass_AboPreis)
                        {
                            case 2:
                                gamePassSalesAdjustment *= 0.95f;
                                break;
                            case 3:
                                gamePassSalesAdjustment *= 0.9f;
                                break;
                            case 4:
                                gamePassSalesAdjustment *= 0.85f;
                                break;
                            case 5:
                                gamePassSalesAdjustment *= 0.75f;
                                break;
                            case 6:
                                gamePassSalesAdjustment *= 0.65f;
                                break;
                            case 7:
                                gamePassSalesAdjustment *= 0.5f;
                                break;
                            case 8:
                                gamePassSalesAdjustment *= 0.4f;
                                break;
                            case 9:
                                gamePassSalesAdjustment *= 0.3f;
                                break;
                            case 10:
                                gamePassSalesAdjustment *= 0.2f;
                                break;
                        }
                        estimatedSales -= gamePassSalesAdjustment;
                    }
                }
                if (__instance.GetVorbestellungen() > 500)
                {
                    // num66 予約注文からランダムに減少させる数 -> randomPreorderReduction
                    int randomPreorderReduction = UnityEngine.Random.Range(1, __instance.GetVorbestellungen() / 33);
                    __instance.vorbestellungen -= randomPreorderReduction;
                    gamePassSalesAdjustment += (float)randomPreorderReduction;
                }
                __instance.gamePassPlayer = Mathf.RoundToInt(gamePassSalesAdjustment) + UnityEngine.Random.Range(1, 10);
                __instance.gpS_.gamePass_AbosLetzteWoche += (long)Mathf.RoundToInt(gamePassSalesAdjustment);
            }
            // num67 実際の販売数（負の値を0に調整）-> actualSales
            int actualSales = Mathf.RoundToInt(estimatedSales);
            if (actualSales < 0)
            {
                actualSales = 0;
            }
            if ((__instance.IsMyGame() || (__instance.typ_contractGame && __instance.developerID == __instance.mS_.myID)) && __instance.releaseDate <= 0 && !__instance.typ_bundle)
            {
                __instance.AddFans(Mathf.RoundToInt(estimatedSales));
            }
            // 物理版とデジタル版の売上処理
            // num68 物理版基本売上 -> physicalBaseSales
            // num69 デラックス版物理売上 -> physicalDeluxeSales
            // num70 コレクターズ版物理売上 -> physicalCollectorsSales
            // num71 デジタル版売上 -> digitalSales
            float physicalBaseSales = 0f;
            float physicalDeluxeSales = 0f;
            float physicalCollectorsSales = 0f;
            float digitalSales = 0f;
            if (__instance.releaseDate <= 0)
            {
                for (int n = __instance.sellsPerWeek.Length - 1; n >= 1; n--)
                {
                    __instance.sellsPerWeek[n] = __instance.sellsPerWeek[n - 1];
                    __instance.sellsPerWeekOnline[n] = __instance.sellsPerWeekOnline[n - 1];
                }
                if (__instance.publisherID != __instance.mS_.myID)
                {
                    actualSales = Mathf.RoundToInt((float)actualSales * __instance.GetPreisAbzug(0));
                    __instance.sellsPerWeek[0] = actualSales;
                    __instance.sellsPerWeekOnline[0] = __instance.games_.GetDigitalSells() * UnityEngine.Random.Range(80f, 100f);
                    __instance.sellsTotal += (long)actualSales;
                }
                else if (__instance.IsMyGame())
                {
                    if (__instance.gameTyp != 2)
                    {
                        if (!__instance.arcade)
                        {
                            float digitalSells = __instance.games_.GetDigitalSells();
                            if (__instance.digitalVersion)
                            {
                                digitalSales = (float)actualSales * digitalSells * __instance.GetPreisAbzug(3);
                                if (!__instance.retailVersion)
                                {
                                    digitalSales += (float)actualSales * 0.2f * __instance.GetPreisAbzug(3);
                                }
                            }
                            if (__instance.retailVersion)
                            {
                                physicalBaseSales = (float)actualSales * (1f - digitalSells) * __instance.GetPreisAbzug(0);
                                physicalBaseSales += physicalBaseSales * __instance.GetEditionQualiaet(0);
                                physicalDeluxeSales = (float)actualSales * __instance.games_.GetDeluxeCurve() * __instance.GetPreisAbzug(1) * __instance.GetEditionQualiaet(1);
                                physicalBaseSales -= physicalDeluxeSales;
                                physicalCollectorsSales = (float)actualSales * __instance.games_.GetCollectorsCurve() * __instance.GetPreisAbzug(2) * __instance.GetEditionQualiaet(2);
                                physicalCollectorsSales *= 0.5f;
                                physicalBaseSales -= physicalCollectorsSales;
                                if (!__instance.digitalVersion)
                                {
                                    physicalBaseSales += (float)actualSales * 0.2f * __instance.GetPreisAbzug(0);
                                }
                                if (__instance.lagerbestand[1] <= 0L)
                                {
                                    physicalBaseSales += physicalDeluxeSales;
                                    physicalDeluxeSales = 0f;
                                }
                                if (__instance.lagerbestand[2] <= 0L)
                                {
                                    physicalBaseSales += physicalCollectorsSales;
                                    physicalCollectorsSales = 0f;
                                }
                                if (physicalBaseSales < 0f)
                                {
                                    physicalBaseSales = 0f;
                                }
                            }
                            __instance.sellsStandard_forProduction = (long)Mathf.RoundToInt(physicalBaseSales);
                            physicalBaseSales += (float)__instance.vorbestellungen;
                            __instance.vorbestellungen = 0;
                            if (__instance.retailVersion)
                            {
                                digitalSales = (float)Mathf.RoundToInt(digitalSales);
                                physicalBaseSales = (float)Mathf.RoundToInt(physicalBaseSales);
                                physicalDeluxeSales = (float)Mathf.RoundToInt(physicalDeluxeSales);
                                physicalCollectorsSales = (float)Mathf.RoundToInt(physicalCollectorsSales);
                                if ((float)__instance.lagerbestand[0] < physicalBaseSales)
                                {
                                    __instance.vorbestellungen += Mathf.RoundToInt(physicalBaseSales - (float)__instance.lagerbestand[0]);
                                    physicalBaseSales = (float)__instance.lagerbestand[0];
                                }
                                __instance.lagerbestand[0] -= (long)Mathf.RoundToInt(physicalBaseSales);
                                if ((float)__instance.lagerbestand[1] < physicalDeluxeSales)
                                {
                                    physicalDeluxeSales = (float)__instance.lagerbestand[1];
                                }
                                __instance.lagerbestand[1] -= (long)Mathf.RoundToInt(physicalDeluxeSales);
                                if ((float)__instance.lagerbestand[2] < physicalCollectorsSales)
                                {
                                    physicalCollectorsSales = (float)__instance.lagerbestand[2];
                                }
                                __instance.lagerbestand[2] -= (long)Mathf.RoundToInt(physicalCollectorsSales);
                            }
                            __instance.sellsPerWeek[0] = Mathf.RoundToInt(digitalSales + physicalBaseSales + physicalDeluxeSales + physicalCollectorsSales);
                            __instance.sellsPerWeekOnline[0] = 100f / (digitalSales + physicalBaseSales + physicalDeluxeSales + physicalCollectorsSales) * digitalSales;
                            __instance.sellsTotal += (long)Mathf.RoundToInt(digitalSales + physicalBaseSales + physicalDeluxeSales + physicalCollectorsSales);
                            __instance.sellsTotalStandard += (long)Mathf.RoundToInt(physicalBaseSales);
                            __instance.sellsTotalDeluxe += (long)Mathf.RoundToInt(physicalDeluxeSales);
                            __instance.sellsTotalCollectors += (long)Mathf.RoundToInt(physicalCollectorsSales);
                            __instance.sellsTotalOnline += (long)Mathf.RoundToInt(digitalSales);
                            if (__instance.vorbestellungen > 0 && __instance.releaseDate <= 0 && physicalBaseSales <= 0f)
                            {
                                __instance.vorbestellungen -= UnityEngine.Random.Range(0, __instance.vorbestellungen / 50 + 3);
                                __instance.vorbestellungen -= __instance.weeksOnMarket * 2;
                                if (__instance.vorbestellungen < 0)
                                {
                                    __instance.vorbestellungen = 0;
                                }
                            }
                        }
                        else
                        {
                            physicalBaseSales = (float)actualSales * __instance.GetPreisAbzug(0);
                            __instance.sellsPerWeek[0] = Mathf.RoundToInt(physicalBaseSales);
                            __instance.sellsPerWeekOnline[0] = 0f;
                            __instance.vorbestellungen += Mathf.RoundToInt(physicalBaseSales);
                            if (__instance.vorbestellungen > 50)
                            {
                                __instance.stornierungen = UnityEngine.Random.Range(0, __instance.vorbestellungen / 50 + 3);
                                __instance.vorbestellungen -= __instance.stornierungen;
                            }
                            else
                            {
                                __instance.stornierungen = 0;
                                if (__instance.weeksOnMarket > 20 && __instance.vorbestellungen > 0)
                                {
                                    __instance.stornierungen = 1;
                                    __instance.vorbestellungen--;
                                }
                            }
                        }
                    }
                    else
                    {
                        digitalSales = (float)actualSales;
                        __instance.sellsPerWeek[0] = Mathf.RoundToInt(digitalSales);
                        __instance.sellsPerWeekOnline[0] = 100f;
                        __instance.sellsTotal += (long)Mathf.RoundToInt(digitalSales);
                        __instance.sellsTotalOnline += (long)Mathf.RoundToInt(digitalSales);
                    }
                }
            }
            else if (__instance.retailVersion)
            {
                __instance.vorbestellungen += Mathf.RoundToInt((float)actualSales * __instance.GetPreisAbzug(0) / (float)(__instance.releaseDate + 1));
            }
            if (__instance.IsMyGame())
            {
                if (__instance.hype > 0f && __instance.releaseDate <= 0)
                {
                    __instance.AddHype(-UnityEngine.Random.Range(0.1f, 1f));
                }
            }
            else
            {
                __instance.hype = 100f;
            }
            if (__instance.releaseDate <= 0 && ((__instance.sellsPerWeek[0] > 100 && !__instance.arcade) || __instance.sellsTotal > 100L || (__instance.arcade && __instance.sellsTotal > 0L)) && !__instance.typ_budget && !__instance.typ_goty)
            {
                // num72 ユーザーレビューのランダムな変動 -> userReviewRandomVariance
                float userReviewRandomVariance;
                if (!__instance.arcade)
                {
                    userReviewRandomVariance = (float)__instance.sellsPerWeek[0];
                    userReviewRandomVariance = UnityEngine.Random.Range(userReviewRandomVariance * 0.01f, userReviewRandomVariance * 0.02f);
                }
                else
                {
                    userReviewRandomVariance = (float)__instance.sellsTotal;
                    userReviewRandomVariance = UnityEngine.Random.Range(userReviewRandomVariance * 0.01f, userReviewRandomVariance * 0.02f) + (float)UnityEngine.Random.Range(0, 5);
                }
                // num73 ユーザーレビューの最終スコア -> userReviewFinalScore
                float userReviewFinalScore = 0f;
                switch (UnityEngine.Random.Range(0, 5))
                {
                    case 0:
                        userReviewFinalScore = userReviewRandomVariance * (float)__instance.reviewGameplay / 100f;
                        break;
                    case 1:
                        userReviewFinalScore = userReviewRandomVariance * (float)__instance.reviewGrafik / 100f;
                        break;
                    case 2:
                        userReviewFinalScore = userReviewRandomVariance * (float)__instance.reviewSound / 100f;
                        break;
                    case 3:
                        userReviewFinalScore = userReviewRandomVariance * (float)__instance.reviewSteuerung / 100f;
                        break;
                    case 4:
                        userReviewFinalScore = userReviewRandomVariance * (float)__instance.reviewTotal / 100f;
                        break;
                }
                userReviewFinalScore -= UnityEngine.Random.Range(0f, __instance.points_bugs);
                if (userReviewFinalScore < 0f)
                {
                    userReviewFinalScore = 0f;
                }
                if (!__instance.mS_.settings_sabotageOff && __instance.mS_.sabotage_reviews > 0)
                {
                    userReviewFinalScore /= 2f;
                }
                __instance.userPositiv += Mathf.RoundToInt(userReviewFinalScore);
                __instance.userNegativ += Mathf.RoundToInt(userReviewRandomVariance - userReviewFinalScore);
            }
            if (__instance.gameTyp != 2 && !__instance.arcade && !__instance.typ_addon && !__instance.typ_addonStandalone && !__instance.typ_mmoaddon && __instance.releaseDate <= 0)
            {
                if (!__instance.devS_)
                {
                    __instance.FindMyDeveloper();
                }
                if (!__instance.pS_)
                {
                    __instance.FindMyPublisher();
                }
                if (currentTotalSales < 1000000L && __instance.sellsTotal >= 1000000L)
                {
                    __instance.mS_.AddAwards(7, __instance.devS_);
                    if (__instance.publisherID != __instance.developerID)
                    {
                        __instance.mS_.AddAwards(7, __instance.pS_);
                    }
                    if (__instance.IsMyGame() || __instance.developerID == __instance.mS_.myID)
                    {
                        __instance.guiMain_.CreateTopNewsGoldeneSchallplatte(__instance.GetNameWithTag());
                        __instance.mS_.goldeneSchallplatten++;
                    }
                }
                if (currentTotalSales < 5000000L && __instance.sellsTotal >= 5000000L)
                {
                    __instance.mS_.AddAwards(10, __instance.devS_);
                    if (__instance.publisherID != __instance.developerID)
                    {
                        __instance.mS_.AddAwards(10, __instance.pS_);
                    }
                    if (__instance.IsMyGame() || __instance.developerID == __instance.mS_.myID)
                    {
                        __instance.guiMain_.CreateTopNewsPlatinSchallplatte(__instance.GetNameWithTag());
                        __instance.mS_.platinSchallplatten++;
                    }
                }
                if (currentTotalSales < 10000000L && __instance.sellsTotal >= 10000000L)
                {
                    __instance.mS_.AddAwards(11, __instance.devS_);
                    if (__instance.publisherID != __instance.developerID)
                    {
                        __instance.mS_.AddAwards(11, __instance.pS_);
                    }
                    if (__instance.IsMyGame() || __instance.developerID == __instance.mS_.myID)
                    {
                        __instance.guiMain_.CreateTopNewsDiamantSchallplatte(__instance.GetNameWithTag());
                        __instance.mS_.diamantSchallplatten++;
                    }
                }
            }
            if (__instance.IsMyGame())
            {
                if (__instance.releaseDate <= 0 && __instance.mS_.achScript_ && __instance.gameTyp != 2)
                {
                    if (__instance.sellsTotal >= 1000000L)
                    {
                        __instance.mS_.achScript_.SetAchivement(48);
                    }
                    if (__instance.sellsTotal >= 10000000L)
                    {
                        __instance.mS_.achScript_.SetAchivement(49);
                    }
                    if (__instance.sellsTotal >= 50000000L)
                    {
                        __instance.mS_.achScript_.SetAchivement(50);
                    }
                }
                Traverse.Create(__instance).Method("UpdateFanletter").GetValue();
                //__instance.UpdateFanletter();
                if (!__instance.typ_addon && !__instance.typ_mmoaddon && !__instance.arcade)
                {
                    //num74 サポートコールの予測数 -> predictedSupportCalls
                    float predictedSupportCalls = (float)actualSales * 0.001f + __instance.points_bugs;
                    predictedSupportCalls = UnityEngine.Random.Range(0f, predictedSupportCalls);
                    if (__instance.gameTyp == 2)
                    {
                        predictedSupportCalls /= 5f;
                    }
                    __instance.mS_.AddAnrufe(Mathf.RoundToInt(predictedSupportCalls));
                }
                if (__instance.publisherID != __instance.mS_.myID)
                {
                    if (__instance.pS_)
                    {
                        __instance.mS_.AddVerkaufsverlauf((long)actualSales);
                        float f;
                        if (__instance.mS_.exklusivVertrag_ID == __instance.publisherID)
                        {
                            f = (float)(actualSales * __instance.pS_.GetShareExklusiv());
                        }
                        else
                        {
                            f = (float)(actualSales * __instance.pS_.GetShare());
                        }
                        // num75 パブリッシャー収益 -> publisherRevenue
                        int publisherRevenue = Mathf.RoundToInt(f);
                        __instance.umsatzTotal += (long)publisherRevenue;
                        __instance.mS_.Earn((long)publisherRevenue, 3);
                        __instance.PayGewinnbeteiligung((long)publisherRevenue);
                        // num76 サブスクリプション収入 -> subscriptionIncome
                        long subscriptionIncome = 0L;
                        if (__instance.gameTyp == 1 && __instance.mS_.week == 5)
                        {
                            subscriptionIncome = __instance.abonnements * (long)__instance.aboPreis;
                            __instance.umsatzTotal += subscriptionIncome;
                            __instance.umsatzAbos += subscriptionIncome;
                            __instance.mS_.Earn(subscriptionIncome, 7);
                            __instance.PayGewinnbeteiligung(subscriptionIncome);
                            __instance.costs_server += __instance.abonnements / 10L;
                        }
                        __instance.PlayerPayEngineLicence((long)publisherRevenue + subscriptionIncome);
                        if (__instance.hype < 50f && (UnityEngine.Random.Range(0f, 100f + __instance.pS_.stars) > 90f || __instance.weeksOnMarket <= 1))
                        {
                            __instance.AddHype(UnityEngine.Random.Range(15f, __instance.pS_.stars + 15f));
                            if (__instance.hype < 0f)
                            {
                                __instance.hype = 0f;
                            }
                            if (__instance.hype > 100f)
                            {
                                __instance.hype = 100f;
                            }
                            string text = __instance.tS_.GetText(495);
                            text = text.Replace("<NAME1>", __instance.GetNameWithTag());
                            __instance.guiMain_.CreateTopNewsInfo(text);
                        }
                    }
                }
                else if (!__instance.arcade)
                {
                    __instance.mS_.AddVerkaufsverlauf((long)Mathf.RoundToInt(digitalSales + physicalBaseSales + physicalDeluxeSales + physicalCollectorsSales));
                    if (digitalSales > 0f)
                    {
                        __instance.mS_.AddDownloadverlauf((long)Mathf.RoundToInt(digitalSales));
                    }
                    // num77 総収益 -> totalRevenue
                    long totalRevenue = 0L;
                    if (__instance.gameTyp != 2)
                    {
                        totalRevenue = Convert.ToInt64(digitalSales * (float)__instance.verkaufspreis[3]) + Convert.ToInt64(physicalBaseSales * (float)__instance.verkaufspreis[0]) + Convert.ToInt64(physicalDeluxeSales * (float)__instance.verkaufspreis[1]) + Convert.ToInt64(physicalCollectorsSales * (float)__instance.verkaufspreis[2]);
                        __instance.umsatzTotal += totalRevenue;
                        __instance.mS_.Earn(totalRevenue, 3);
                        __instance.PayGewinnbeteiligung(totalRevenue);
                    }
                    long subscriptionIncome;
                    if (__instance.gameTyp == 1 && __instance.mS_.week == 5)
                    {
                        subscriptionIncome = __instance.abonnements * (long)__instance.aboPreis;
                        __instance.umsatzTotal += subscriptionIncome;
                        __instance.umsatzAbos += subscriptionIncome;
                        __instance.mS_.Earn(subscriptionIncome, 7);
                        __instance.PayGewinnbeteiligung(subscriptionIncome);
                        __instance.costs_server += __instance.abonnements / 10L;
                    }
                    // num78 サブスクリプション収入 -> subscriptionIncome
                    subscriptionIncome = 0L;
                    if (__instance.gameTyp == 2 && __instance.mS_.week == 5)
                    {
                        __instance.costs_server += __instance.abonnements / 10L;
                    }
                    __instance.PlayerPayEngineLicence(totalRevenue + subscriptionIncome);
                    if (__instance.autoPreis && !__instance.arcade && !__instance.handy)
                    {
                        __instance.UpdateAutoPreis();
                    }
                }
                if (__instance.gameTab_)
                {
                    __instance.gameTab_.UpdateData();
                }
                if ((__instance.publisherID != __instance.mS_.myID && actualSales <= 0 && __instance.abonnements <= 0L) || (__instance.publisherID == __instance.mS_.myID && __instance.mS_.automatic_RemoveGameFormMarket && __instance.releaseDate <= 0 && __instance.weeksOnMarket > 4 && __instance.vorbestellungen <= 0 && __instance.sellsPerWeek[0] <= __instance.mS_.automatic_RemoveGameFormMarket_Amount && __instance.abonnements <= 0L))
                {
                    if (!__instance.arcade || (__instance.arcade && __instance.vorbestellungen <= 0))
                    {
                        __instance.guiMain_.ActivateMenu(__instance.guiMain_.uiObjects[82]);
                        __instance.guiMain_.uiObjects[82].GetComponent<Menu_GameFromMarket>().Init(__instance, false);
                        __instance.guiMain_.OpenMenu(false);
                        __instance.RemoveFromMarket();
                    }
                }
                else
                {
                    if (__instance.sellsTotal > 0L && __instance.weeksOnMarket < 24 && !__instance.guiMain_.menuOpen && __instance.reviewTotal > 90 && !__instance.trendsetter && __instance.releaseDate <= 0 && __instance.mS_.trendGenre != __instance.maingenre && !__instance.typ_mmoaddon && !__instance.typ_addon && !__instance.typ_budget && !__instance.typ_bundle && !__instance.typ_addonStandalone && !__instance.typ_goty && !__instance.typ_bundleAddon && UnityEngine.Random.Range(0, 200) == 1)
                    {
                        __instance.guiMain_.ActivateMenu(__instance.guiMain_.uiObjects[142]);
                        __instance.guiMain_.uiObjects[142].GetComponent<Menu_Trendsetter>().Init(__instance);
                        __instance.guiMain_.OpenMenu(false);
                        __instance.mS_.award_Trendsetter++;
                        __instance.AddHype(30f);
                        __instance.AddIpPoints(70f);
                        if (__instance.mS_.achScript_)
                        {
                            __instance.mS_.achScript_.SetAchivement(34);
                        }
                    }
                    if (__instance.sellsTotal > 0L && __instance.commercialFlop && __instance.weeksOnMarket == 4 && !__instance.guiMain_.menuOpen)
                    {
                        __instance.guiMain_.ActivateMenu(__instance.guiMain_.uiObjects[352]);
                        __instance.guiMain_.uiObjects[352].GetComponent<Menu_RandomEventCommercialFlop>().Init(__instance);
                    }
                    if (__instance.sellsTotal > 0L && __instance.commercialHit && __instance.weeksOnMarket == 4 && !__instance.guiMain_.menuOpen)
                    {
                        __instance.guiMain_.ActivateMenu(__instance.guiMain_.uiObjects[354]);
                        __instance.guiMain_.uiObjects[354].GetComponent<Menu_RandomEventCommercialHit>().Init(__instance);
                    }
                    if (__instance.sellsTotal > 0L && __instance.points_bugsInvis > 0f && __instance.mS_.difficulty >= 2 && __instance.weeksOnMarket >= 4 && __instance.weeksOnMarket <= 20 && UnityEngine.Random.Range(0, 200) <= __instance.mS_.difficulty && !__instance.guiMain_.menuOpen)
                    {
                        __instance.guiMain_.ActivateMenu(__instance.guiMain_.uiObjects[353]);
                        __instance.guiMain_.uiObjects[353].GetComponent<Menu_RandomEventBugs>().Init(__instance);
                    }
                }
            }
            else
            {
                if (__instance.gameTyp != 2)
                {
                    // f2 売上総収益 -> totalRevenue2
                    float totalRevenue2 = 0f;
                    if (!__instance.handy && !__instance.arcade)
                    {
                        if (__instance.publisherID != __instance.developerID)
                        {
                            if (__instance.pS_)
                            {
                                totalRevenue2 = (float)actualSales * __instance.pS_.share;
                            }
                        }
                        else
                        {
                            totalRevenue2 = (float)(actualSales * __instance.verkaufspreis[0]);
                        }
                    }
                    if (__instance.handy)
                    {
                        totalRevenue2 = (float)(actualSales * 3);
                    }
                    if (__instance.arcade)
                    {
                        totalRevenue2 = (float)(actualSales * __instance.verkaufspreis[0]);
                    }
                    //num79 アーケードゲームの売上収益 -> arcadeGameRevenue
                    int arcadeGameRevenue = Mathf.RoundToInt(totalRevenue2);
                    __instance.umsatzTotal += (long)arcadeGameRevenue;
                    __instance.AddTochterfirmaUmsatz((long)arcadeGameRevenue);
                }
                if (__instance.gameTyp == 1 && __instance.mS_.week == 5)
                {
                    // num80 : 定期購読による売上収益 -> subscriptionRevenue
                    long subscriptionRevenue = __instance.abonnements * (long)__instance.aboPreis;
                    __instance.umsatzTotal += subscriptionRevenue;
                    __instance.umsatzAbos += subscriptionRevenue;
                    __instance.AddTochterfirmaUmsatz(subscriptionRevenue);
                    __instance.costs_server += __instance.abonnements / 10L;
                }
                if (__instance.gameTyp == 2 && __instance.mS_.week == 5)
                {
                    __instance.costs_server += __instance.abonnements / 10L;
                }
                if (!__instance.engineS_)
                {
                    __instance.FindMyEngineNew();
                }
                if (__instance.engineS_ && __instance.engineS_.ownerID == __instance.mS_.myID && __instance.reviewTotal > 50)
                {
                    __instance.AddFans(actualSales);
                }
                if ((actualSales <= 0 && __instance.abonnements < 10L) || (__instance.gameTyp == 2 && __instance.abonnements < 10L && __instance.weeksOnMarket > 5))
                {
                    if (!__instance.typ_bundle)
                    {
                        __instance.FindMyEngineNew();
                        if (__instance.engineS_)
                        {
                            if (__instance.engineS_.ownerID == __instance.mS_.myID)
                            {
                                if (__instance.guiMain_)
                                {
                                    __instance.guiMain_.OpenEngineAbrechnung(__instance);
                                }
                            }
                            else if (__instance.mS_.multiplayer && __instance.engineS_.EngineFromMitspieler() && __instance.mS_.mpCalls_.isServer)
                            {
                                __instance.mS_.mpCalls_.SERVER_Send_EngineAbrechnung(__instance.engineS_.ownerID, __instance.myID);
                            }
                            if (__instance.GetPublisherOrDeveloperIsTochterfirma() && __instance.guiMain_)
                            {
                                __instance.guiMain_.OpenTochterfirmaAbrechnung(__instance);
                            }
                        }
                    }
                    __instance.RemoveFromMarket();
                }
            }
            if (__instance.typ_mmoaddon)
            {
                gameScript gameScript = __instance.FindVorgaengerScript();
                if (gameScript && !gameScript.isOnMarket)
                {
                    __instance.RemoveFromMarket();
                }
            }
            if (__instance.mS_.multiplayer)
            {
                if (__instance.mS_.mpCalls_.isServer && (__instance.IsMyGame() || __instance.typ_contractGame || (__instance.DeveloperIsNPC() && __instance.PublisherIsNPC() && __instance.OwnerIsNPC())))
                {
                    __instance.mS_.mpCalls_.SERVER_Send_GameSell(__instance);
                }
                if (__instance.mS_.mpCalls_.isClient && __instance.IsMyGame())
                {
                    __instance.mS_.mpCalls_.CLIENT_Send_GameSell(__instance);
                }
            }
            return false;
        }

        //--------------------------------------------------------------------------------
        /// <summary>
        /// 
        /// </summary>
        /// <param name="difficulty"></param>
        /// <returns></returns>
        static float SetGameSalesBasedOnDifficulty(int difficulty)
        {
            Dictionary<int, float> difficultyMultipliers = new Dictionary<int, float>
                {
                    {0, 2.0f},
                    {1, 1.5f},
                    {2, 1.0f},
                    {3, 0.5f},
                    {4, 0.35f},
                    {5, 0.25f}
                };

            float AdjustGameSalesBasedOnDifficulty(int _difficulty)
            {
                float defaultMultiplier = 1.0f;
                float multiplier = difficultyMultipliers.ContainsKey(_difficulty) ? difficultyMultipliers[_difficulty] : defaultMultiplier;
                return multiplier;
            }

            // Modifierイベントハンドラーを定義して、値の変更をカスタマイズ
            float CustomModifier(float baseValue)
            {
                return GameSalesBasedOnDifficultyMultiplier.Instance.ApplyMultiplier(baseValue); // 最終的な値を返す
            }

            // 使用例
            float baseDifficultyValue = AdjustGameSalesBasedOnDifficulty(difficulty);
            float result = CustomModifier(baseDifficultyValue); // Modifierを適用
            return result;
        }

        //--------------------------------------------------------------------------------
        /// <summary>
        /// ゲーム開発会社が多ければ多いほど、競争が激しくなり、ゲームが売れなくなるロジックのメソッド
        /// </summary>
        /// <param name="difficulty"></param>
        /// <param name="totalCompetitionImpact"></param>
        /// <returns></returns>
        static float SetGameSalesCompetitionImpact(int difficulty, float totalCompetitionImpact)
        {
            // Difficultyに応じたCompetitionImpactの最大値を定義
            Dictionary<int, float> competitionImpactLimits = new Dictionary<int, float>
                {
                    {0, 0.5f},
                    {1, 0.55f},
                    {2, 0.6f},
                    {3, 0.65f},
                    {4, 0.7f},
                    {5, 0.75f},
                };

            // Difficultyに基づいてtotalCompetitionImpactの値を調整するメソッド
            float AdjustCompetitionImpact(int _difficulty, float currentImpact)
            {
                // デフォルトの最大値を設定
                float defaultLimit = 0.5f;

                // Dictionaryから適切な値を取得、見つからなければデフォルト値を使用
                float limit = competitionImpactLimits.ContainsKey(_difficulty) ? competitionImpactLimits[_difficulty] : defaultLimit;

                // 現在の影響値が限界値より大きければ、限界値を使用
                return currentImpact > limit ? limit : currentImpact;
            }

            // Modifierイベントハンドラーを定義して、値の変更をカスタマイズ
            float CustomModifier(float baseValue)
            {
                return GameSalesCompetitionImpactMultiplier.Instance.ApplyMultiplier(baseValue); // 最終的な値を返す
            }

            // 使用例
            totalCompetitionImpact = CustomModifier(totalCompetitionImpact); // Modifierを適用
            totalCompetitionImpact = AdjustCompetitionImpact(difficulty, totalCompetitionImpact);
            return 1f - totalCompetitionImpact;
        }

        //--------------------------------------------------------------------------------
        /// <summary>
        /// 
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="realismImpactFactor"></param>
        /// <returns></returns>
        static float SetGameSalesBasedOnRealisticPowerAndSalesImpact(gameScript instance, float realismImpactFactor)
        {
            /*
                レビュースコアと売上の条件: 86未満のレビュースコアを持ち、「レビュースコア〇〇以上、〇〇未満」と「難易度に応じてスケールした売上のしきい値」に達した場合、
                instance.realsticPowerにrealismImpactFactor（0.06）を加算する。
                realsticPowerの上限: instance.realsticPowerは最大で0.98までしか上がらない。これはゲームの現実的な魅力が一定の閾値を超えないように制限するためだ。
                売上への影響: estimatedSales *= 1f - instance.realsticPower;この計算により、realsticPowerが高くなるほど売上予測が減少する。
                つまり、ゲームの現実感が強すぎると、逆に売上が落ちることを示している。
                最低売上の保証: instance.realsticPowerの影響により、売上予測が最低でも2%は保持される。
                これは、ゲームの現実的な魅力が非常に高くても、売上が完全にゼロにはならないことを保証する。
                realsticPowerを操作するテスト: instance.realsticPowerを意図的に0に設定すると、売上が一定の数値を保持する。
                これは、realsticPowerの値が売上に直接影響することを確認するための良いテストだな。
            */

            Dictionary<int, int> reviewToSalesThreshold = new Dictionary<int, int>
                {
                    {5, 10000},
                    {10, 15000},
                    {15, 20000},
                    {20, 25000},
                    {25, 30000},
                    {30, 35000},
                    {40, 50000},
                    {45, 60000},
                    {50, 80000},
                    {55, 150000},
                    {60, 200000},
                    {65, 300000},
                    {70, 500000},
                    {75, 1000000},
                    {80, 2000000},
                    {85, 4000000}
                };

            void AdjustRealisticPowerAndSales(float _realismImpactFactor)
            {
                foreach (var threshold in reviewToSalesThreshold)
                {
                    if (instance.reviewTotal >= threshold.Key - 5 && instance.reviewTotal < threshold.Key)
                    {
                        if (instance.sellsTotal > (long)(threshold.Value / (instance.mS_.difficulty + 1)))
                        {
                            //Debug.Log("SetGameSalesBasedOnRealisticPowerAndSalesImpact : " + instance.myName);
                            //Debug.Log("Review Total : " + instance.reviewTotal);
                            //Debug.Log("Sells Total : " + instance.sellsTotal);
                            //Debug.Log("Realistic Power : " + instance.realsticPower);
                            //Debug.Log("Threshold : " + (long)(threshold.Value / (instance.mS_.difficulty + 1)));
                            //Debug.Log("---------------------------------------------------------------------------");
                            instance.realsticPower += CustomModifier(_realismImpactFactor); // Modifierを適用
                        }
                        break; // マッチしたらループを抜ける
                    }
                }
            }

            // Modifierイベントハンドラーを定義して、値の変更をカスタマイズ
            float CustomModifier(float baseValue)
            {
                return GameSalesBasedOnRealisticPowerAndSalesMultiplier.
                    Instance.ApplyMultiplier(baseValue); // 最終的な値を返す
            }

            AdjustRealisticPowerAndSales(realismImpactFactor);
            instance.realsticPower = Math.Min(instance.realsticPower, 0.98f); // 最大値を0.98に制限
            return 1f - instance.realsticPower;
        }
    }
}

