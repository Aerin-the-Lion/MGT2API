using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using HarmonyLib;
using System.Runtime.CompilerServices;
using MGT2API.Difficulty.Adjustments;

namespace MGT2API.Difficulty.Hooksaaaaa
{
    [HarmonyPatch]
    internal static class SellGameHooksaaaaaa
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
        internal static bool Prefix(ref gameScript __instance)
        {
            //Debug.Log("================================================");
            //Debug.Log("SellGameHooks.SellGame.Prefix");
            //Debug.Log("================================================");

            try
            {
                SellGameManager.SellGameRefactored(ref __instance);
            }
            catch (Exception ex)
            {
                Debug.LogError("An error occurred : " + ex.Message);
            }
            return false;
        }

        private static class SellGameManager
        {

            public static void SellGameRefactored(ref gameScript instance)
            {

                // ==================================================
                if (!instance.isOnMarket)
                {
                    if (!instance.inDevelopment && instance.freigabeBudget > 0)
                    {
                        instance.freigabeBudget--;
                    }
                    return;
                }
                if (instance.publisherID != -1 && !instance.pS_)
                {
                    instance.FindMyPublisher();
                }
                if (instance.releaseDate <= 0)
                {
                    instance.weeksOnMarket++;
                }
                else
                {
                    instance.releaseDate--;
                }
                // num 売上の基本倍率、売上計算に使う基本係数？ -> salesBaseRate
                float salesBaseRate = 0.07f;
                // num2 各レビュースコアに基づく売上予測値 -> estimatedSales
                float estimatedSales = 0f;
                // num3 最終的な売上調整係数、追加の売上調整に使われる -> finalAdjustmentFactor
                float finalAdjustmentFactor = 0f;
                // num4 現在の総売上 -> currentTotalSales
                long currentTotalSales = instance.sellsTotal;
                // num5 ゲームの総レビュースコア -> totalReviewScore
                int totalReviewScore = instance.reviewTotal;
                // num6 ユーザーレビューに基づく売上係数 -> userReviewSalesFactor
                float userReviewSalesFactor = 1f + instance.GetUserReviewPercent() * 0.01f;
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
                if (instance.debug)
                {
                    Debug.Log("GAME " + instance.myName + " A " + estimatedSales.ToString());
                }
                // num7 総レビュースコアをfloatにキャストしたもの、計算で使用 -> totalReviewScoreFloat
                float totalReviewScoreFloat = (float)instance.reviewTotal;
                estimatedSales += instance.points_gameplay * (totalReviewScoreFloat * 0.01f);
                estimatedSales += instance.points_grafik * (totalReviewScoreFloat * 0.01f);
                estimatedSales += instance.points_sound * (totalReviewScoreFloat * 0.01f);
                estimatedSales += instance.points_technik * (totalReviewScoreFloat * 0.01f);

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
                if (instance.mainIP != -1)
                {
                    if (!instance.script_mainIP)
                    {
                        instance.FindMainIpScript();
                    }
                    if (instance.script_mainIP)
                    {
                        switch (instance.mS_.difficulty)
                        {
                            case 0:
                                estimatedSales += estimatedSales * instance.GetIpBekanntheit() * 0.2f;
                                break;
                            case 1:
                                estimatedSales += estimatedSales * instance.GetIpBekanntheit() * 0.2f * 0.8f;
                                break;
                            case 2:
                                estimatedSales += estimatedSales * instance.GetIpBekanntheit() * 0.2f * 0.5f;
                                break;
                            case 3:
                                estimatedSales += estimatedSales * instance.GetIpBekanntheit() * 0.2f * 0.4f;
                                break;
                            case 4:
                                estimatedSales += estimatedSales * instance.GetIpBekanntheit() * 0.2f * 0.37f;
                                break;
                            case 5:
                                estimatedSales += estimatedSales * instance.GetIpBekanntheit() * 0.2f * 0.35f;
                                break;
                            default:
                                estimatedSales += estimatedSales * instance.GetIpBekanntheit() * 0.2f * 0.5f;
                                break;
                        }
                        if (instance.script_mainIP.merchGesamtReviewPoints < (float)instance.reviewTotal)
                        {
                            instance.script_mainIP.merchGesamtReviewPoints = (float)instance.reviewTotal;
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

                if (instance.mainIP != -1)
                {
                    if (!instance.script_mainIP)
                    {
                        instance.FindMainIpScript();
                    }
                    if (instance.script_mainIP)
                    {
                        //CalculateSalesBasedOnIpPopularity
                        float ipPopularity = instance.GetIpBekanntheit(); // IPの可視性を取得
                        float baseSalesMultiplier = CalculateSalesBasedOnIpPopularityMultiplier(instance.mS_.difficulty, ipPopularity); // 売上倍率を計算
                        float customSalesMultiplier = GameSalesBasedOnIpPopularityMultiplier.Instance.ApplyMultiplier(baseSalesMultiplier);
                        estimatedSales += estimatedSales * customSalesMultiplier; // 推定売上に倍率を適用

                        if (instance.script_mainIP.merchGesamtReviewPoints < (float)instance.reviewTotal)
                        {
                            instance.script_mainIP.merchGesamtReviewPoints = (float)instance.reviewTotal;
                        }
                    }
                }
                // -------------------------------------------------------------------------------------
                if (instance.ExistAutomatenspiel())
                {
                    estimatedSales += estimatedSales * 0.2f;
                }
                if (instance.nachfolger_created)
                {
                    if (instance.debug)
                    {
                        Debug.Log("GAME " + instance.myName + " B " + estimatedSales.ToString());
                    }
                    if (!instance.script_nachfolger)
                    {
                        instance.FindNachfolgerScript();
                    }
                    if (instance.script_nachfolger && instance.script_nachfolger.isOnMarket)
                    {
                        estimatedSales *= 0.6f;
                    }
                }
                if (instance.typ_nachfolger)
                {
                    if (!instance.script_vorgaenger)
                    {
                        instance.FindVorgaengerScript();
                    }
                    if (instance.script_vorgaenger)
                    {
                        if (instance.weeksOnMarket > 2)
                        {
                            if (instance.script_vorgaenger.reviewTotal < 5)
                            {
                                estimatedSales -= 1000f;
                            }
                            if (instance.script_vorgaenger.reviewTotal >= 5 && instance.script_vorgaenger.reviewTotal < 10)
                            {
                                estimatedSales -= 500f;
                            }
                            if (instance.script_vorgaenger.reviewTotal >= 10 && instance.script_vorgaenger.reviewTotal < 20)
                            {
                                estimatedSales -= 300f;
                            }
                            if (instance.script_vorgaenger.reviewTotal >= 20 && instance.script_vorgaenger.reviewTotal < 30)
                            {
                                estimatedSales -= 100f;
                            }
                            if (instance.script_vorgaenger.reviewTotal >= 30 && instance.script_vorgaenger.reviewTotal < 40)
                            {
                                estimatedSales -= 50f;
                            }
                        }
                        if (instance.script_vorgaenger.reviewTotal >= 40 && instance.script_vorgaenger.reviewTotal < 50)
                        {
                            estimatedSales += 50f;
                        }
                        if (instance.script_vorgaenger.reviewTotal >= 50 && instance.script_vorgaenger.reviewTotal < 60)
                        {
                            estimatedSales += 1000f;
                        }
                        if (instance.script_vorgaenger.reviewTotal >= 60 && instance.script_vorgaenger.reviewTotal < 70)
                        {
                            estimatedSales += 2000f;
                        }
                        if (instance.script_vorgaenger.reviewTotal >= 70 && instance.script_vorgaenger.reviewTotal < 80)
                        {
                            estimatedSales += 3000f;
                        }
                        if (instance.script_vorgaenger.reviewTotal >= 80 && instance.script_vorgaenger.reviewTotal < 90)
                        {
                            estimatedSales += 4000f;
                        }
                        if (instance.script_vorgaenger.reviewTotal >= 90 && instance.script_vorgaenger.reviewTotal < 95)
                        {
                            estimatedSales += 5000f;
                        }
                        if (instance.script_vorgaenger.reviewTotal >= 95 && instance.script_vorgaenger.reviewTotal < 100)
                        {
                            estimatedSales += 10000f;
                        }
                        if (instance.script_vorgaenger.reviewTotal >= 100)
                        {
                            estimatedSales += 15000f;
                        }
                    }
                }
                if (instance.IsMyGame())
                {
                    // num8 レビュースコアに基づく売上係数 -> reviewScoreMultiplier
                    float reviewScoreMultiplier = (float)instance.reviewTotal * 0.01f;
                    // num9 ジャンルファンによる売上ボーナス -> genreFansSalesBonus
                    float genreFansSalesBonus = reviewScoreMultiplier * (float)instance.genres_.GetAmountFans() * 0.005f;
                    // num10 メインジャンルファンによる売上ボーナス -> mainGenreFansSalesBonus
                    float mainGenreFansSalesBonus = reviewScoreMultiplier * (float)instance.genres_.genres_FANS[instance.maingenre] * 0.05f;
                    // num11 サブジャンルが存在する場合のサブジャンルファンによる売上ボーナス -> subGenreFansSalesBonus
                    float subGenreFansSalesBonus = 0f;
                    if (instance.subgenre != -1)
                    {
                        subGenreFansSalesBonus = reviewScoreMultiplier * (float)instance.genres_.genres_FANS[instance.subgenre] * 0.01f;
                    }
                    estimatedSales += genreFansSalesBonus + mainGenreFansSalesBonus + subGenreFansSalesBonus;
                }
                else
                {
                    //num12 1975年からの経過年数 -> yearsSince1975
                    int yearsSince1975 = instance.mS_.year - 1975;
                    //num13 レビュースコアに基づく売上係数 -> reviewScoreMultiplier2
                    float reviewScoreMultiplier2 = (float)instance.reviewTotal * 0.01f;
                    //num14 レビュースコアに基づく売上増加1 -> reviewScoreBasedSales1
                    float reviewScoreBasedSales1 = reviewScoreMultiplier2 * (float)(50000 * yearsSince1975) * 0.001f;
                    //num15 レビュースコアに基づく売上増加2 -> reviewScoreBasedSales2
                    float reviewScoreBasedSales2 = reviewScoreMultiplier2 * (float)(5000 * yearsSince1975) * 0.01f;
                    estimatedSales += reviewScoreBasedSales1 + reviewScoreBasedSales2;
                }
                // -------------------------------------------------------------------------------------
                switch (instance.gameTyp)
                {
                    case 0:
                        if (!instance.arcade)
                        {
                            // num16 週ごとの売上減衰係数 -> weeklySalesDecayFactor
                            float weeklySalesDecayFactor = 1f - (float)instance.weeksOnMarket * 0.01f;
                            if (weeklySalesDecayFactor >= 0f)
                            {
                                estimatedSales *= weeklySalesDecayFactor;
                            }
                            else
                            {
                                estimatedSales *= 0f;
                            }
                            estimatedSales -= (float)(instance.weeksOnMarket * instance.weeksOnMarket * 3);
                        }
                        else
                        {
                            // num17 アーケードの週ごとの売上減衰係数 -> arcadeWeeklySalesDecayFactor
                            float arcadeWeeklySalesDecayFactor = 1f - (float)instance.weeksOnMarket * 0.003f;
                            if (arcadeWeeklySalesDecayFactor >= 0f)
                            {
                                estimatedSales *= arcadeWeeklySalesDecayFactor;
                            }
                            else
                            {
                                estimatedSales *= 0f;
                            }
                            estimatedSales -= (float)(instance.weeksOnMarket * instance.weeksOnMarket);
                        }
                        break;
                    case 1:
                        estimatedSales *= instance.mmoInteresse * 0.01f;
                        if (instance.IsMyGame())
                        {
                            instance.AddMMOInteresse(-UnityEngine.Random.Range(0.3f, 0.5f));
                        }
                        else
                        {
                            instance.AddMMOInteresse(-UnityEngine.Random.Range(0.1f, 0.3f));
                        }
                        break;
                    case 2:
                        estimatedSales *= instance.f2pInteresse * 0.01f;
                        if (instance.IsMyGame())
                        {
                            instance.AddF2PInteresse(-UnityEngine.Random.Range(0.3f, 0.5f));
                        }
                        else
                        {
                            instance.AddF2PInteresse(-UnityEngine.Random.Range(0.1f, 0.3f));
                        }
                        break;
                }
                if (estimatedSales < 0f)
                {
                    estimatedSales = 0f;
                }
                if (!instance.arcade)
                {
                    estimatedSales *= instance.games_.GetSells();
                }
                else
                {
                    estimatedSales *= instance.games_.GetSellsArcade();
                }
                if (instance.gameTyp == 1 || instance.gameTyp == 2 || instance.typ_mmoaddon)
                {
                    estimatedSales *= instance.games_.GetInternetUser();
                }
                switch (instance.weeksOnMarket)
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
                // -------------------------------------------------------------------------------------
                if (instance.IsMyGame())
                {
                    if (instance.typ_nachfolger || instance.typ_remaster || instance.typ_spinoff || instance.typ_standard)
                    {
                        //クソほどわかりにくいけど、Achivementsという要素があり、15個の要素がある。
                        //それぞれの要素には、それぞれに応じたボーナスが設定されている。
                        //instance.mS_.GetAchivementBonus(0) は amountAchivementsBonus[0]とおなじです。
                        //例えば、instance.mS_.GetAchivementBonus(0)は「PC and console games sold」で、これはPCとコンソールのゲームの売上に応じたボーナスが設定されている。
                        //Achivementsのボーナスは、例えば「パズルゲーム」を70%以上のレビュースコアでリリースすると、その該当のボーナス1%がもらえるといった具合だ。
                        //最終的に、各分野で7-8%のボーナスがもらえるようになる。
                        //……はい。ここもマジックナンバーでハードコーディングされてて、しかもあっちゃこっちゃにこういうのを点在しているせいで、再利用可能性もなければ、保守性もない。
                        if (!instance.handy && !instance.arcade)
                        {
                            //num18 PCとコンソールのゲームの売上に応じたAchivementsボーナスの追加売上係数 -> PcAndConsoleAchivementsBonus
                            float PcAndConsoleAchivementsBonus = (float)instance.mS_.GetAchivementBonus((int)AchivementType.PcAndConsoleGamesSold);
                            PcAndConsoleAchivementsBonus *= 0.01f;
                            finalAdjustmentFactor += PcAndConsoleAchivementsBonus;
                        }
                        if (instance.arcade)
                        {
                            //num19 アーケードゲームの売上に応じたAchivementsボーナスの追加売上係数 -> ArcadeAchivementsBonus
                            float ArcadeAchivementsBonus = (float)instance.mS_.GetAchivementBonus((int)AchivementType.ArcadeGamesSold);
                            ArcadeAchivementsBonus *= 0.01f;
                            finalAdjustmentFactor += ArcadeAchivementsBonus;
                        }
                        if (instance.handy)
                        {
                            //num20 携帯ゲームの売上に応じたAchivementsボーナスの追加売上係数 -> MobileGameAchivementsBonus
                            float MobileGameAchivementsBonus = (float)instance.mS_.GetAchivementBonus((int)AchivementType.MobileGamesSold);
                            MobileGameAchivementsBonus *= 0.01f;
                            finalAdjustmentFactor += MobileGameAchivementsBonus;
                        }
                    }
                    if (instance.typ_addon || instance.typ_addonStandalone || instance.typ_mmoaddon)
                    {
                        //num21 アドオンゲームの売上に応じたAchivementsボーナスの追加売上係数 -> AddonAchivementsBonus
                        float AddonAchivementsBonus = (float)instance.mS_.GetAchivementBonus((int)AchivementType.AddonsSold);
                        AddonAchivementsBonus *= 0.01f;
                        finalAdjustmentFactor += AddonAchivementsBonus;
                    }
                    if (instance.typ_budget || instance.typ_bundle || instance.typ_bundleAddon || instance.typ_goty)
                    {
                        //num22 セカンダリマーケティングの売上に応じたAchivementsボーナスの追加売上係数 -> SecondaryMarketingAchivementsBonus
                        float SecondaryMarketingAchivementsBonus = (float)instance.mS_.GetAchivementBonus((int)AchivementType.SecondaryMarketingSold);
                        SecondaryMarketingAchivementsBonus *= 0.01f;
                        finalAdjustmentFactor += SecondaryMarketingAchivementsBonus;
                    }
                }
                switch (instance.usk)
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
                if (!instance.gameLanguage[0])
                {
                    finalAdjustmentFactor -= 0.05f;
                }
                if (!instance.gameLanguage[1])
                {
                    finalAdjustmentFactor -= 0.03f;
                }
                if (!instance.gameLanguage[2])
                {
                    finalAdjustmentFactor -= 0.03f;
                }
                if (!instance.gameLanguage[3])
                {
                    finalAdjustmentFactor -= 0.02f;
                }
                if (!instance.gameLanguage[4])
                {
                    finalAdjustmentFactor -= 0.02f;
                }
                if (!instance.gameLanguage[5])
                {
                    finalAdjustmentFactor -= 0.02f;
                }
                if (!instance.gameLanguage[6])
                {
                    finalAdjustmentFactor -= 0.01f;
                }
                if (!instance.gameLanguage[7])
                {
                    finalAdjustmentFactor -= 0.02f;
                }
                if (!instance.gameLanguage[8])
                {
                    finalAdjustmentFactor -= 0.02f;
                }
                if (!instance.gameLanguage[9])
                {
                    finalAdjustmentFactor -= 0.03f;
                }
                if (!instance.gameLanguage[10])
                {
                    finalAdjustmentFactor -= 0.04f;
                }
                if (!instance.typ_bundle)
                {
                    if (instance.mS_.trendGenre == instance.maingenre)
                    {
                        finalAdjustmentFactor += 0.33f;
                    }
                    if (instance.mS_.trendTheme == instance.gameMainTheme)
                    {
                        finalAdjustmentFactor += 0.15f;
                    }
                    if (instance.mS_.trendAntiGenre == instance.maingenre)
                    {
                        finalAdjustmentFactor -= 0.33f;
                    }
                    if (instance.mS_.trendAntiTheme == instance.gameMainTheme)
                    {
                        finalAdjustmentFactor -= 0.15f;
                    }
                    if (instance.mS_.trendGenre == instance.subgenre)
                    {
                        finalAdjustmentFactor += 0.1f;
                    }
                    if (instance.mS_.trendTheme == instance.gameSubTheme)
                    {
                        finalAdjustmentFactor += 0.05f;
                    }
                    if (instance.mS_.trendAntiGenre == instance.subgenre)
                    {
                        finalAdjustmentFactor -= 0.1f;
                    }
                    if (instance.mS_.trendAntiTheme == instance.gameSubTheme)
                    {
                        finalAdjustmentFactor -= 0.05f;
                    }
                }
                finalAdjustmentFactor += instance.GetHype() * 0.01f;
                if (!instance.typ_bundle && !instance.arcade)
                {
                    if (instance.gameCopyProtect != -1)
                    {
                        if (!instance.gameCopyProtectScript_)
                        {
                            GameObject gameObject = GameObject.Find("COPYPROTECT_" + instance.gameCopyProtect.ToString());
                            if (gameObject)
                            {
                                instance.gameCopyProtectScript_ = gameObject.GetComponent<copyProtectScript>();
                            }
                            else
                            {
                                instance.gameCopyProtect = -1;
                            }
                        }
                        if (instance.gameCopyProtectScript_)
                        {
                            finalAdjustmentFactor += instance.gameCopyProtectScript_.effekt * 0.002f;
                        }
                    }
                    if (instance.gameAntiCheat != -1 && (instance.gameplayFeatures_DevDone[21] || instance.gameplayFeatures_DevDone[23]))
                    {
                        if (!instance.gameAntiCheatScript_)
                        {
                            GameObject gameObject2 = GameObject.Find("ANTICHEAT_" + instance.gameAntiCheat.ToString());
                            if (gameObject2)
                            {
                                instance.gameAntiCheatScript_ = gameObject2.GetComponent<antiCheatScript>();
                            }
                            else
                            {
                                instance.gameAntiCheat = -1;
                            }
                        }
                        if (instance.gameAntiCheatScript_)
                        {
                            finalAdjustmentFactor += instance.gameAntiCheatScript_.effekt * 0.003f;
                        }
                    }
                }
                if (instance.publisherID != instance.mS_.myID && instance.pS_)
                {
                    if (instance.maingenre == instance.pS_.fanGenre)
                    {
                        finalAdjustmentFactor += 0.2f;
                    }
                    finalAdjustmentFactor += instance.pS_.stars * 0.01f;
                }
                if (!instance.arcade)
                {
                    if (instance.mS_.month == 12 || instance.mS_.month == 1)
                    {
                        finalAdjustmentFactor += 0.5f;
                    }
                    if (instance.mS_.month == 6 || instance.mS_.month == 7)
                    {
                        finalAdjustmentFactor -= 0.3f;
                    }
                }
                if (instance.IsMyGame() && instance.mS_.awardBonus > 0 && instance.mS_.awardBonusAmount > 0f)
                {
                    finalAdjustmentFactor += instance.mS_.awardBonusAmount;
                }
                if (!instance.arcade)
                {
                    finalAdjustmentFactor += instance.bonusSellsUpdates;
                }
                else
                {
                    finalAdjustmentFactor += instance.bonusSellsUpdates * 0.2f;
                }
                finalAdjustmentFactor += instance.bonusSellsAddons;
                finalAdjustmentFactor += instance.addonQuality;
                if (finalAdjustmentFactor < -0.5f)
                {
                    finalAdjustmentFactor = -0.5f;
                }
                estimatedSales *= 1f + finalAdjustmentFactor;
                if (!instance.arcade)
                {
                    // num23 これはゲームの基本的な売上予測係数を表しています。ゲームが独占的である場合には値が増加します。baseSalesCoefficient。
                    float baseSalesFactor = 10f;
                    if (instance.exklusiv)
                    {
                        baseSalesFactor = 25f;
                    }
                    for (int i = 0; i < instance.gamePlatform.Length; i++)
                    {
                        if (instance.gamePlatform[i] != -1)
                        {
                            if (!instance.gamePlatformScript[i])
                            {
                                instance.FindMyPlatforms();
                            }
                            if (instance.gamePlatformScript[i])
                            {
                                baseSalesFactor += instance.gamePlatformScript[i].GetMarktanteil()
                                    * instance.genres_.GetFloatPlatformSells(instance.maingenre, instance.gamePlatformScript[i].typ);
                            }
                        }
                    }
                    baseSalesFactor *= 0.007f;
                    estimatedSales *= baseSalesFactor;
                }
                else
                {
                    estimatedSales *= instance.genres_.GetFloatPlatformSells(instance.maingenre, 4);
                }
                if (instance.IsMyGame())
                {
                    if (!instance.typ_bundle && !instance.arcade && instance.mS_.gelangweiltGenre != -1)
                    {
                        if (instance.maingenre == instance.mS_.gelangweiltGenre)
                        {
                            estimatedSales *= 0.5f;
                        }
                        else if (instance.subgenre == instance.mS_.gelangweiltGenre)
                        {
                            estimatedSales *= 0.85f;
                        }
                    }
                    if (instance.mS_.sauerBugs > 0)
                    {
                        estimatedSales *= 0.7f;
                    }
                    if (instance.mS_.schlechteSpiele > 0)
                    {
                        estimatedSales *= 0.6f;
                    }

                    if (!instance.typ_bundle && !instance.typ_addon && !instance.typ_mmoaddon)
                    {
                        Vector4 amountGamesWithGenreAndTopic = instance.games_.GetAmountGamesWithGenreAndTopic(instance);
                        // num24 ジャンルのレベルに基づいた競争の影響を計算するために使われます。 -> genreCompetitionFactor
                        // 1/1800に近い数値
                        float genreCompetitionFactor = 0.00055555557f * (float)instance.genres_.genres_LEVEL.Length;
                        // num25 テーマのレベルに基づいた競争の影響を計算するために使われます。 -> themeCompetitionFactor
                        //  1/30000に近い数値
                        float themeCompetitionFactor = 3.3333334E-05f * (float)instance.themes_.themes_LEVEL.Length;
                        //num26 これは最終的な競争の影響を表しており、ゲームの売上予測にマイナスの影響を与えます。
                        float totalCompetitionImpact = amountGamesWithGenreAndTopic.x * genreCompetitionFactor + amountGamesWithGenreAndTopic.y * themeCompetitionFactor + amountGamesWithGenreAndTopic.z * genreCompetitionFactor * 2.5f + amountGamesWithGenreAndTopic.w * themeCompetitionFactor * 2.5f;
                        switch (instance.mS_.settings_competition)
                        {
                            case 1:
                                totalCompetitionImpact *= 2f;
                                break;
                            case 2:
                                totalCompetitionImpact *= 3f;
                                break;
                        }
                        /*
                        switch (instance.mS_.difficulty)
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

                        estimatedSales *= SetGameSalesCompetitionImpact(instance.mS_.difficulty, totalCompetitionImpact);

                    }
                }
                if (instance.gameLicence != -1)
                {
                    if (instance.licences_.licence_GENREGOOD[instance.gameLicence] == instance.maingenre)
                    {
                        estimatedSales += estimatedSales * (instance.licences_.licence_QUALITY[instance.gameLicence] * 0.01f * 0.8f);
                    }
                    if (instance.licences_.licence_GENREGOOD[instance.gameLicence] != instance.maingenre && instance.licences_.licence_GENREBAD[instance.gameLicence] != instance.maingenre)
                    {
                        estimatedSales += estimatedSales * (instance.licences_.licence_QUALITY[instance.gameLicence] * 0.01f * 0.3f);
                    }
                    if (instance.licences_.licence_GENREBAD[instance.gameLicence] == instance.maingenre)
                    {
                        estimatedSales -= estimatedSales * (instance.licences_.licence_QUALITY[instance.gameLicence] * 0.01f * 0.3f);
                    }
                    if (instance.subgenre != -1)
                    {
                        if (instance.licences_.licence_GENREGOOD[instance.gameLicence] == instance.subgenre)
                        {
                            estimatedSales += estimatedSales * (instance.licences_.licence_QUALITY[instance.gameLicence] * 0.01f * 0.2f);
                        }
                        if (instance.licences_.licence_GENREGOOD[instance.gameLicence] != instance.subgenre && instance.licences_.licence_GENREBAD[instance.gameLicence] != instance.subgenre)
                        {
                            estimatedSales += estimatedSales * (instance.licences_.licence_QUALITY[instance.gameLicence] * 0.01f * 0.05f);
                        }
                        if (instance.licences_.licence_GENREBAD[instance.gameLicence] == instance.subgenre)
                        {
                            estimatedSales -= estimatedSales * (instance.licences_.licence_QUALITY[instance.gameLicence] * 0.01f * 0.1f);
                        }
                    }
                }
                //num27: これはメインジャンルの人気度を表しています。トレンドによってこの値が変化するため、mainGenrePopularityと名付けるのが適切です。
                float mainGenrePopularity = instance.genres_.genres_BELIEBTHEIT[instance.maingenre];
                if (instance.maingenre == instance.mS_.trendGenre)
                {
                    mainGenrePopularity = 100f;
                }
                if (instance.maingenre == instance.mS_.trendAntiGenre)
                {
                    mainGenrePopularity = 20f;
                }
                //num28: これは人気度に基づいて調整された売上の一部を表します。popularityAdjustedSalesという名前にリファクタリングします。
                float popularityAdjustedSales = estimatedSales * 0.5f * (mainGenrePopularity * 0.01f);
                estimatedSales *= 0.8f;
                estimatedSales += popularityAdjustedSales;
                if (!instance.mS_.settings_sabotageOff && instance.mS_.sabotage_erwischt > 0)
                {
                    estimatedSales *= 0.5f;
                }
                if (!instance.mS_.settings_sabotageOff && instance.mS_.sabotage_geruecht > 0)
                {
                    estimatedSales *= 0.8f;
                }
                if (instance.mS_.globalEvent == 0)
                {
                    estimatedSales *= 0.5f;
                }
                if (instance.mS_.globalEvent == 1)
                {
                    estimatedSales *= 1.5f;
                }
                if (instance.newGenreCombination)
                {
                    estimatedSales *= 1.1f;
                }
                if (instance.newTopicCombination)
                {
                    estimatedSales *= 1.03f;
                }
                if (instance.commercialFlop)
                {
                    if (instance.reviewTotal >= 70 && !instance.typ_bundle && !instance.typ_bundleAddon && instance.mS_.trendGenre != instance.maingenre)
                    {
                        estimatedSales *= 0.1f;
                    }
                    else
                    {
                        instance.commercialFlop = false;
                    }
                }
                if (instance.commercialHit)
                {
                    if (instance.reviewTotal >= 70 && instance.reviewTotal < 90)
                    {
                        estimatedSales *= 2f;
                    }
                    else
                    {
                        instance.commercialHit = false;
                    }
                }

                //-------------------------------------------------------------------------------------
                // ここから下は、ゲームの難易度による売上の減少係数を計算している。
                /*
                switch (instance.mS_.difficulty)
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

                estimatedSales *= SetGameSalesBasedOnDifficulty(instance.mS_.difficulty);

                if (instance.IsMyGame() && instance.publisherID == instance.mS_.myID)
                {
                    switch (instance.mS_.GetStudioLevel(instance.mS_.studioPoints))
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
                if (instance.mS_.settings_sandbox && instance.IsMyGame() && instance.mS_.sandbox_gameSells > 0f)
                {
                    estimatedSales *= instance.mS_.sandbox_gameSells;
                }
                estimatedSales *= salesBaseRate;
                if (instance.typ_addon)
                {
                    estimatedSales *= 0.4f;
                    if (!instance.script_vorgaenger)
                    {
                        instance.FindVorgaengerScript();
                    }
                    if (instance.script_vorgaenger && estimatedSales > 0f)
                    {
                        if (instance.script_vorgaenger.amountAddons > 0)
                        {
                            estimatedSales /= (float)instance.script_vorgaenger.amountAddons;
                        }
                        if (!instance.script_vorgaenger.isOnMarket && instance.script_vorgaenger.publisherID != instance.mS_.myID)
                        {
                            estimatedSales *= 0.8f;
                        }
                        if ((float)instance.sellsTotal + estimatedSales + (float)instance.vorbestellungen > (float)instance.script_vorgaenger.sellsTotal)
                        {
                            estimatedSales = (float)(instance.script_vorgaenger.sellsTotal - (instance.sellsTotal + (long)instance.vorbestellungen));
                        }
                        if (estimatedSales <= 0f)
                        {
                            estimatedSales = 1f;
                        }
                        if (!instance.script_vorgaenger.isOnMarket && (float)instance.sellsTotal + estimatedSales > (float)instance.script_vorgaenger.sellsTotal)
                        {
                            estimatedSales = 0f;
                        }
                    }
                }
                if (instance.typ_addonStandalone)
                {
                    estimatedSales *= 0.5f;
                    if (!instance.script_vorgaenger)
                    {
                        instance.FindVorgaengerScript();
                    }
                    if (instance.script_vorgaenger && instance.script_vorgaenger.amountAddons > 0)
                    {
                        estimatedSales /= (float)instance.script_vorgaenger.amountAddons;
                    }
                }
                if (instance.typ_mmoaddon)
                {
                    estimatedSales *= 0.65f;
                    if (!instance.script_vorgaenger)
                    {
                        instance.FindVorgaengerScript();
                    }
                    if (instance.script_vorgaenger && estimatedSales > 0f)
                    {
                        instance.script_vorgaenger.abosAddons = Mathf.RoundToInt(estimatedSales);
                        if ((float)instance.sellsTotal + estimatedSales + (float)instance.vorbestellungen > (float)instance.script_vorgaenger.sellsTotal)
                        {
                            estimatedSales = (float)(instance.script_vorgaenger.sellsTotal - (instance.sellsTotal + (long)instance.vorbestellungen));
                        }
                        if (estimatedSales <= 0f)
                        {
                            estimatedSales = 1f;
                        }
                    }
                }
                if (instance.gameTyp == 1)
                {
                    if (instance.IsMyGame() && instance.games_.freeServerPlatz <= 0L)
                    {
                        estimatedSales *= 0.05f;
                    }
                    int amountOfMMOs = instance.games_.GetAmountOfMMOs();
                    // num29: これはMMOの数に基づいた売上の減少係数を表しています。amountOfMMOsSalesDecreaseFactorと名付けるのが適切です。
                    float amountOfMMOsSalesDecreaseFactor = 1f + (float)amountOfMMOs * 0.1f;
                    if (amountOfMMOs > 0)
                    {
                        estimatedSales /= amountOfMMOsSalesDecreaseFactor;
                    }
                    //num30: 新たに獲得されるアボナメント（サブスクリプション）の予測数です。projectedNewSubscriptions
                    float projectedNewSubscriptions = UnityEngine.Random.Range((estimatedSales + (float)instance.abosAddons) * 0.5f, (estimatedSales + (float)instance.abosAddons) * 0.7f);
                    instance.abosAddons = 0;
                    //num31: これはアボナメント（サブスクリプション）の減少係数を表しています。baseSubscriptionLossと名付けるのが適切です。
                    float baseSubscriptionLoss = (float)instance.abonnements - (float)instance.abonnements / 102f * (float)instance.reviewTotal;
                    baseSubscriptionLoss *= 0.25f;
                    baseSubscriptionLoss += (float)instance.weeksOnMarket;
                    if (instance.IsMyGame())
                    {
                        baseSubscriptionLoss += baseSubscriptionLoss * ((100f - instance.hype) * 0.01f);
                    }
                    switch (instance.aboPreis)
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
                    if (instance.IsMyGame())
                    {
                        projectedNewSubscriptions *= 0.7f;
                    }
                    else
                    {
                        projectedNewSubscriptions *= 1.1f;
                    }
                    instance.abonnementsWoche = instance.abonnements;
                    instance.abonnements -= (long)Mathf.RoundToInt(baseSubscriptionLoss);
                    switch (instance.aboPreis - instance.aboPreisOld)
                    {
                        case 1:
                            instance.abonnements -= instance.abonnements / 10L;
                            break;
                        case 2:
                            instance.abonnements -= instance.abonnements / 9L;
                            break;
                        case 3:
                            instance.abonnements -= instance.abonnements / 8L;
                            break;
                        case 4:
                            instance.abonnements -= instance.abonnements / 7L;
                            break;
                        case 5:
                            instance.abonnements -= instance.abonnements / 6L;
                            break;
                        case 6:
                            instance.abonnements -= instance.abonnements / 5L;
                            break;
                        case 7:
                            instance.abonnements -= instance.abonnements / 4L;
                            break;
                        case 8:
                            instance.abonnements -= instance.abonnements / 3L;
                            break;
                        case 9:
                            instance.abonnements -= instance.abonnements / 2L;
                            break;
                        case 10:
                            instance.abonnements -= instance.abonnements / 2L;
                            break;
                    }
                    instance.aboPreisOld = instance.aboPreis;
                    //abonnements -> subscriptions
                    instance.abonnements += (long)Mathf.RoundToInt(projectedNewSubscriptions);
                    if (instance.abonnements > instance.sellsTotal)
                    {
                        instance.abonnements = instance.sellsTotal;
                    }
                    if (instance.IsMyGame())
                    {
                        //num32: アボナメント（サブスクリプション）
                        long subscriptions = instance.abonnements;
                        for (int j = 0; j < instance.mS_.arrayRoomScripts.Length; j++)
                        {
                            if (instance.mS_.arrayRoomScripts[j] && instance.mS_.arrayRoomScripts[j].typ == 15 && (instance.mS_.arrayRoomScripts[j].serverReservieren == 0 || instance.mS_.arrayRoomScripts[j].serverReservieren == 1))
                            {
                                subscriptions = instance.mS_.arrayRoomScripts[j].SetAbos(subscriptions);
                                if (subscriptions <= 0L)
                                {
                                    break;
                                }
                            }
                        }
                        instance.abonnements -= subscriptions;
                        instance.mS_.AddAboverlauf(instance.abonnements);
                    }
                    instance.abonnementsWoche = instance.abonnements - instance.abonnementsWoche;
                    if (instance.abonnements < 0L)
                    {
                        instance.abonnements = 0L;
                    }
                    if (instance.bestAbonnements < instance.abonnements)
                    {
                        instance.bestAbonnements = instance.abonnements;
                    }
                }
                if (instance.handy && instance.gameTyp == 0)
                {
                    estimatedSales *= 2.5f;
                }
                if (instance.arcade)
                {
                    if (instance.IsMyGame())
                    {
                        //num33: これはアーケードゲームの売上予測係数を表しています。arcadeQualitySalesFactorと名付けるのが適切です。
                        float arcadeQualitySalesFactor = (float)(instance.arcadeCase + instance.arcadeMonitor + instance.arcadeJoystick + instance.arcadeSound);
                        arcadeQualitySalesFactor = 1f + arcadeQualitySalesFactor * 0.05f;
                        estimatedSales *= arcadeQualitySalesFactor;
                    }
                    else
                    {
                        //num34: これはアーケードゲームの売上予測係数を表しています。npcArcadeQualitySalesFactorと名付けるのが適切です。
                        float npcArcadeQualitySalesFactor = (float)(instance.arcadeCase + instance.arcadeMonitor + instance.arcadeJoystick + instance.arcadeSound);
                        npcArcadeQualitySalesFactor = 1f + npcArcadeQualitySalesFactor * 0.05f;
                        estimatedSales *= npcArcadeQualitySalesFactor;
                        estimatedSales *= 0.3f;
                    }
                    estimatedSales *= 0.005f;
                    if (estimatedSales < 1f && !instance.IsMyGame() && instance.weeksOnMarket < 2)
                    {
                        estimatedSales = (float)UnityEngine.Random.Range(1, 4);
                    }
                }
                if (instance.gameTyp == 2)
                {
                    int amountOfF2Ps = instance.games_.GetAmountOfF2Ps();
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
                    float projectedNewF2PSubscriptions = UnityEngine.Random.Range((estimatedSales + (float)instance.abosAddons) * 0.3f, (estimatedSales + (float)instance.abosAddons) * 0.5f);
                    instance.abosAddons = 0;
                    //num37: F2Pゲームでのアボナメント（サブスクリプション）が失われる割合を計算するための基礎数値です。baseF2PSubscriptionLossと命名します。
                    float baseF2PSubscriptionLoss = (float)instance.abonnements - (float)instance.abonnements / 102f * (float)instance.reviewTotal;
                    baseF2PSubscriptionLoss *= 0.25f;
                    baseF2PSubscriptionLoss += (float)instance.weeksOnMarket;
                    if (amountOfF2Ps > 0)
                    {
                        projectedNewF2PSubscriptions /= (float)amountOfF2Ps;
                    }
                    instance.abonnementsWoche = instance.abonnements;
                    instance.abonnements -= (long)Mathf.RoundToInt(baseF2PSubscriptionLoss);
                    instance.abonnements += (long)Mathf.RoundToInt(projectedNewF2PSubscriptions);
                    if (instance.abonnements > instance.sellsTotal)
                    {
                        instance.abonnements = instance.sellsTotal;
                    }
                    if (instance.IsMyGame())
                    {
                        //num38: アボナメント（サブスクリプション）
                        long subscriptions2 = instance.abonnements;
                        for (int k = 0; k < instance.mS_.arrayRoomScripts.Length; k++)
                        {
                            if (instance.mS_.arrayRoomScripts[k] && instance.mS_.arrayRoomScripts[k] && instance.mS_.arrayRoomScripts[k].typ == 15 && (instance.mS_.arrayRoomScripts[k].serverReservieren == 0 || instance.mS_.arrayRoomScripts[k].serverReservieren == 2))
                            {
                                subscriptions2 = instance.mS_.arrayRoomScripts[k].SetAbos(subscriptions2);
                                if (subscriptions2 <= 0L)
                                {
                                    break;
                                }
                            }
                        }
                        instance.abonnements -= subscriptions2;
                    }
                    instance.abonnementsWoche = instance.abonnements - instance.abonnementsWoche;
                    if (instance.abonnements < 0L)
                    {
                        instance.abonnements = 0L;
                        instance.abonnementsWoche = 0L;
                    }
                    if (instance.bestAbonnements < instance.abonnements)
                    {
                        instance.bestAbonnements = instance.abonnements;
                    }
                }
                if (instance.typ_budget)
                {
                    estimatedSales *= 0.5f;
                    //num39: 予算タイトルの場合、ゲームのリリースが過去からどれだけ離れているかに基づく売上の減少係数です。budgetTitleAgingFactorと命名します。
                    float budgetTitleAgingFactor = (float)(instance.mS_.year * (instance.date_start_year - 1));
                    budgetTitleAgingFactor *= 0.05f;
                    if (budgetTitleAgingFactor > 0.7f)
                    {
                        budgetTitleAgingFactor = 0.7f;
                    }
                    budgetTitleAgingFactor = 1f - budgetTitleAgingFactor;
                    estimatedSales *= budgetTitleAgingFactor;
                }
                if (instance.typ_remaster)
                {
                    estimatedSales *= 0.7f;
                }
                if (instance.typ_goty)
                {
                    estimatedSales *= 0.5f;
                    //num40: GOTY版の場合、ゲームのリリースが過去からどれだけ離れているかに基づく売上の減少係数です。gotyAgingFactorと命名します。
                    float gotyAgingFactor = (float)(instance.mS_.year * (instance.date_start_year - 1));
                    gotyAgingFactor *= 0.03f;
                    if (gotyAgingFactor > 0.7f)
                    {
                        gotyAgingFactor = 0.7f;
                    }
                    gotyAgingFactor = 1f - gotyAgingFactor;
                    estimatedSales *= gotyAgingFactor;
                }
                if (instance.typ_bundle)
                {
                    estimatedSales *= 0.2f;
                    // num41 バンドルゲームの売上増加係数 -> bundleSalesIncreaseFactor
                    float bundleSalesIncreaseFactor = 0.4f; // バンドルに含まれるゲームの数に基づいて、売上の増加係数を計算
                    if (instance.bundleID[0] != -1)
                    {
                        bundleSalesIncreaseFactor += 0.1f;
                    }
                    if (instance.bundleID[1] != -1)
                    {
                        bundleSalesIncreaseFactor += 0.1f;
                    }
                    if (instance.bundleID[2] != -1)
                    {
                        bundleSalesIncreaseFactor += 0.1f;
                    }
                    if (instance.bundleID[3] != -1)
                    {
                        bundleSalesIncreaseFactor += 0.1f;
                    }
                    if (instance.bundleID[4] != -1)
                    {
                        bundleSalesIncreaseFactor += 0.1f;
                    }
                    estimatedSales *= bundleSalesIncreaseFactor;
                }
                if (instance.typ_bundleAddon)
                {
                    // num42 バンドルアドオンの古さによる売上の減少係数 -> bundleAddonAgingSaleDecreaseFactor
                    // ゲームのリリースからの経過年数に基づいて、売上の減少係数を計算
                    float bundleAddonAgingSaleDecreaseFactor = (float)(instance.mS_.year * (instance.date_start_year - 1));
                    bundleAddonAgingSaleDecreaseFactor *= 0.05f;
                    if (bundleAddonAgingSaleDecreaseFactor > 0.7f)
                    {
                        bundleAddonAgingSaleDecreaseFactor = 0.7f;
                    }
                    bundleAddonAgingSaleDecreaseFactor = 1f - bundleAddonAgingSaleDecreaseFactor;
                    estimatedSales *= bundleAddonAgingSaleDecreaseFactor;
                    // num43 バンドルアドオンの売上増加係数 -> bundleAddonSalesIncreaseFactor
                    float bundleAddonSalesIncreaseFactor = 0.5f; // バンドルに含まれるアドオンの数に基づいて、売上の増加係数を計算
                    if (instance.bundleID[0] != -1)
                    {
                        bundleAddonSalesIncreaseFactor += 0.05f;
                    }
                    if (instance.bundleID[1] != -1)
                    {
                        bundleAddonSalesIncreaseFactor += 0.05f;
                    }
                    if (instance.bundleID[2] != -1)
                    {
                        bundleAddonSalesIncreaseFactor += 0.05f;
                    }
                    if (instance.bundleID[3] != -1)
                    {
                        bundleAddonSalesIncreaseFactor += 0.05f;
                    }
                    if (instance.bundleID[4] != -1)
                    {
                        bundleAddonSalesIncreaseFactor += 0.05f;
                    }
                    estimatedSales *= bundleAddonSalesIncreaseFactor;
                }
                if (!instance.arcade)
                {
                    // num44 ゲームプラットフォームの総ユニット数 -> totalPlatformUnits
                    long totalPlatformUnits = 0L;    // バンドルやアドオンがリリースされているプラットフォームの総ユニット数を計算
                    if (estimatedSales > 0f)
                    {
                        for (int l = 0; l < instance.gamePlatform.Length; l++)
                        {
                            if (instance.gamePlatform[l] != -1)
                            {
                                if (!instance.gamePlatformScript[l])
                                {
                                    instance.FindMyPlatforms();
                                }
                                if (instance.gamePlatformScript[l])
                                {
                                    if (instance.exklusiv && instance.gamePlatformScript[l].OwnerIsNPC() && !instance.gamePlatformScript[l].vomMarktGenommen)
                                    {
                                        if (instance.gameTyp != 2)
                                        {
                                            // num45 プラットフォームごとのエクスクルーシブ販売ボーナス -> exclusivePlatformSaleBonus
                                            // エクスクルーシブ契約に基づいて、各プラットフォームでの販売ボーナスを計算
                                            int exclusivePlatformSaleBonus = Mathf.RoundToInt(UnityEngine.Random.Range((float)instance.sellsPerWeek[0] 
                                                * 0.2f, (float)instance.sellsPerWeek[0] * 0.3f));
                                            exclusivePlatformSaleBonus = exclusivePlatformSaleBonus / 100 
                                                * (130 - (int)instance.gamePlatformScript[l].GetMarktanteil());
                                            if (exclusivePlatformSaleBonus > instance.sellsPerWeek[0])
                                            {
                                                exclusivePlatformSaleBonus = instance.sellsPerWeek[0];
                                            }
                                            instance.exklusivKonsolenSells += (long)exclusivePlatformSaleBonus;
                                            instance.gamePlatformScript[l].BonusSellsExklusiv(exclusivePlatformSaleBonus);
                                        }
                                        else
                                        {
                                            // num46 MMOゲームにおけるプラットフォームごとのエクスクルーシブ販売ボーナス -> exclusiveMmoPlatformSaleBonus
                                            // MMOゲームの場合、プラットフォームごとのエクスクルーシブ販売ボーナスを計算
                                            int exclusiveMmoPlatformSaleBonus = Mathf.RoundToInt(UnityEngine.Random.Range((float)instance.sellsPerWeek[0] * 0.2f, (float)instance.sellsPerWeek[0] * 0.3f));
                                            exclusiveMmoPlatformSaleBonus = exclusiveMmoPlatformSaleBonus / 100 * (130 - (int)instance.gamePlatformScript[l].GetMarktanteil());
                                            if (exclusiveMmoPlatformSaleBonus > instance.sellsPerWeek[0])
                                            {
                                                exclusiveMmoPlatformSaleBonus = instance.sellsPerWeek[0];
                                            }
                                            instance.exklusivKonsolenSells += (long)(exclusiveMmoPlatformSaleBonus / 5);
                                            instance.gamePlatformScript[l].BonusSellsExklusiv(exclusiveMmoPlatformSaleBonus);
                                        }
                                    }
                                    totalPlatformUnits += (long)instance.gamePlatformScript[l].units;
                                }
                            }
                        }
                        if ((float)(instance.sellsTotal + (long)instance.vorbestellungen) + estimatedSales > (float)(totalPlatformUnits / 2L))
                        {
                            //num47: 利用可能なプラットフォームユニットと予測される売上との差分 -> availableUnitsAfterSales
                            long availableUnitsAfterSales = totalPlatformUnits - (instance.sellsTotal + (long)instance.vorbestellungen + (long)Mathf.RoundToInt(estimatedSales));
                            if (availableUnitsAfterSales / 10L > instance.sellsTotal)
                            {
                                instance.sellsTotal = availableUnitsAfterSales / 10L;
                            }
                        }
                        if ((float)(instance.sellsTotal + (long)instance.vorbestellungen) + estimatedSales > (float)totalPlatformUnits)
                        {
                            estimatedSales = (float)(totalPlatformUnits - (instance.sellsTotal + (long)instance.vorbestellungen));
                            if (estimatedSales <= 0f)
                            {
                                estimatedSales = 1f;
                            }
                        }
                    }
                }

                if (instance.gameTyp == 0 && instance.sellsPerWeek[0] > 0 && estimatedSales > 0f)
                //if (instance.gameTyp == 0 && estimatedSales > 0f)
                {
                    // num48 世間のゲームの現実的な評価の係数 -> realismSalesDecreaseImpactFactor
                    float realismSalesDecreaseImpactFactor = 0.06f;
                    /*
                    if (instance.reviewTotal >= 0 && instance.reviewTotal < 5 && instance.sellsTotal > (long)(10000 / (instance.mS_.difficulty + 1)))
                    {
                        instance.realsticPower += gameRealismIncreaseFactor;
                    }
                    if (instance.reviewTotal >= 5 && instance.reviewTotal < 10 && instance.sellsTotal > (long)(15000 / (instance.mS_.difficulty + 1)))
                    {
                        instance.realsticPower += gameRealismIncreaseFactor;
                    }
                    if (instance.reviewTotal >= 10 && instance.reviewTotal < 15 && instance.sellsTotal > (long)(20000 / (instance.mS_.difficulty + 1)))
                    {
                        instance.realsticPower += gameRealismIncreaseFactor;
                    }
                    if (instance.reviewTotal >= 15 && instance.reviewTotal < 20 && instance.sellsTotal > (long)(25000 / (instance.mS_.difficulty + 1)))
                    {
                        instance.realsticPower += gameRealismIncreaseFactor;
                    }
                    if (instance.reviewTotal >= 20 && instance.reviewTotal < 25 && instance.sellsTotal > (long)(30000 / (instance.mS_.difficulty + 1)))
                    {
                        instance.realsticPower += gameRealismIncreaseFactor;
                    }
                    if (instance.reviewTotal >= 25 && instance.reviewTotal < 30 && instance.sellsTotal > (long)(35000 / (instance.mS_.difficulty + 1)))
                    {
                        instance.realsticPower += gameRealismIncreaseFactor;
                    }
                    if (instance.reviewTotal >= 35 && instance.reviewTotal < 40 && instance.sellsTotal > (long)(50000 / (instance.mS_.difficulty + 1)))
                    {
                        instance.realsticPower += gameRealismIncreaseFactor;
                    }
                    if (instance.reviewTotal >= 40 && instance.reviewTotal < 45 && instance.sellsTotal > (long)(60000 / (instance.mS_.difficulty + 1)))
                    {
                        instance.realsticPower += gameRealismIncreaseFactor;
                    }
                    if (instance.reviewTotal >= 45 && instance.reviewTotal < 50 && instance.sellsTotal > (long)(80000 / (instance.mS_.difficulty + 1)))
                    {
                        instance.realsticPower += gameRealismIncreaseFactor;
                    }
                    if (instance.reviewTotal >= 50 && instance.reviewTotal < 55 && instance.sellsTotal > (long)(150000 / (instance.mS_.difficulty + 1)))
                    {
                        instance.realsticPower += gameRealismIncreaseFactor;
                    }
                    if (instance.reviewTotal >= 55 && instance.reviewTotal < 60 && instance.sellsTotal > (long)(200000 / (instance.mS_.difficulty + 1)))
                    {
                        instance.realsticPower += gameRealismIncreaseFactor;
                    }
                    if (instance.reviewTotal >= 60 && instance.reviewTotal < 65 && instance.sellsTotal > (long)(300000 / (instance.mS_.difficulty + 1)))
                    {
                        instance.realsticPower += gameRealismIncreaseFactor;
                    }
                    if (instance.reviewTotal >= 65 && instance.reviewTotal < 70 && instance.sellsTotal > (long)(500000 / (instance.mS_.difficulty + 1)))
                    {
                        instance.realsticPower += gameRealismIncreaseFactor;
                    }
                    if (instance.reviewTotal >= 70 && instance.reviewTotal < 75 && instance.sellsTotal > (long)(1000000 / (instance.mS_.difficulty + 1)))
                    {
                        instance.realsticPower += gameRealismIncreaseFactor;
                    }
                    if (instance.reviewTotal >= 75 && instance.reviewTotal < 80 && instance.sellsTotal > (long)(2000000 / (instance.mS_.difficulty + 1)))
                    {
                        instance.realsticPower += gameRealismIncreaseFactor;
                    }
                    if (instance.reviewTotal >= 80 && instance.reviewTotal < 85 && instance.sellsTotal > (long)(4000000 / (instance.mS_.difficulty + 1)))
                    {
                        instance.realsticPower += gameRealismIncreaseFactor;
                    }
                    if (instance.realsticPower > 0f)
                    {
                        if (instance.realsticPower > 0.98f)
                        {
                            instance.realsticPower = 0.98f;
                        }
                        estimatedSales *= 1f - instance.realsticPower;
                    }
                    if (estimatedSales < 0f)
                    {
                        estimatedSales = 0f;
                    }
                }
                */

                    estimatedSales *= SetGameSalesBasedOnRealisticPowerAndSalesImpact(instance, realismSalesDecreaseImpactFactor);
                    if (estimatedSales < 0f)
                    {
                        estimatedSales = 0f;
                    }

                    if (instance.HasInAppPurchases())
                    {
                        if (instance.gameTyp == 0 && instance.releaseDate <= 0)
                        {
                            // num49 ゲーム内アプリ購入に対する反感による売上の減少量 -> inAppPurchaseHateSalesDecrease
                            float inAppPurchaseHateSalesDecrease = estimatedSales * (instance.GetInAppPurchaseHate() * 0.01f) * 0.3f; // ゲーム内アプリ購入に対するプレイヤーの反感により売上がどれだけ減少するか。
                            estimatedSales -= inAppPurchaseHateSalesDecrease;
                            float inAppPurchaseMoneyPerWeek = instance.GetInAppPurchaseMoneyPerWeek();

                            // num50 ゲーム内アプリ購入からの週間収入予測値 -> inAppPurchaseWeeklyRevenueEstimate
                            float inAppPurchaseWeeklyRevenueEstimate = UnityEngine.Random.Range((float)(instance.sellsTotal / 100L * 2L), (float)(instance.sellsTotal / 100L * 3L)); // ゲーム内アプリ購入から期待される週間収入の予測値。
                            if (instance.IsMyGame())
                            {
                                // num51 アチーブメントボーナスによる収入の増加率 -> achievementBonusIncomeIncrease
                                float achievementBonusIncomeIncrease = (float)instance.mS_.GetAchivementBonus((int)AchivementType.InAppPurchases);
                                achievementBonusIncomeIncrease *= 0.01f;    // ゲームのアチーブメントボーナスが収入に与える増加率。
                                inAppPurchaseWeeklyRevenueEstimate += inAppPurchaseWeeklyRevenueEstimate * achievementBonusIncomeIncrease;
                            }
                            if (estimatedSales <= 0f)
                            {
                                inAppPurchaseWeeklyRevenueEstimate *= 0.6f;
                            }
                            // num52 市場における週数による収益の減少率 -> marketDurationRevenueDecreaseFactor
                            float marketDurationRevenueDecreaseFactor = 1f - (float)instance.weeksOnMarket * 0.01f; // ゲームが市場に出てからの週数が収益に与える減少率。
                            if (marketDurationRevenueDecreaseFactor < 0.1f)
                            {
                                marketDurationRevenueDecreaseFactor = 0.1f;
                            }
                            inAppPurchaseWeeklyRevenueEstimate *= marketDurationRevenueDecreaseFactor;
                            if (instance.weeksOnMarket > 5)
                            {
                                inAppPurchaseWeeklyRevenueEstimate -= (float)(instance.weeksOnMarket * 30);
                            }
                            if (inAppPurchaseWeeklyRevenueEstimate < 0f)
                            {
                                inAppPurchaseWeeklyRevenueEstimate = 0f;
                            }
                            if (inAppPurchaseWeeklyRevenueEstimate > 2E+09f)
                            {
                                inAppPurchaseWeeklyRevenueEstimate = (float)(2000000000 - UnityEngine.Random.Range(0, 100000));
                            }
                            instance.inAppPurchaseWeek = Mathf.RoundToInt(inAppPurchaseWeeklyRevenueEstimate);
                            // num53 ゲーム内アプリ購入からの実際の週間収益 -> actualInAppPurchaseWeeklyRevenue
                            long actualInAppPurchaseWeeklyRevenue = (long)(inAppPurchaseMoneyPerWeek * (float)Mathf.RoundToInt(inAppPurchaseWeeklyRevenueEstimate)); // ゲーム内アプリ購入から得られる実際の週間収益。

                            // 総売上とゲーム内アプリ購入による売上に週間収益を追加
                            instance.umsatzTotal += actualInAppPurchaseWeeklyRevenue;
                            instance.umsatzInApp += actualInAppPurchaseWeeklyRevenue;
                            if (instance.IsMyGame())
                            {
                                instance.mS_.Earn(actualInAppPurchaseWeeklyRevenue, 8);
                            }
                            if (instance.IsMyGame())
                            {
                                instance.PayGewinnbeteiligung(actualInAppPurchaseWeeklyRevenue);
                            }
                            if (!instance.IsMyGame())
                            {
                                instance.AddTochterfirmaUmsatz(actualInAppPurchaseWeeklyRevenue);
                            }
                        }
                        if (instance.gameTyp == 1 && instance.releaseDate <= 0)
                        {
                            // num54 サブスクリプションベースのゲームでのゲーム内アプリ購入に対する反感による売上減少量 -> subscriptionGameInAppPurchaseDislikeSalesDecrease
                            // サブスクリプションベースのゲームにおけるゲーム内アプリ購入への反感が売上に与える減少量。
                            float subscriptionGameInAppPurchaseHateSalesDecrease = estimatedSales * (instance.GetInAppPurchaseHate() * 0.01f) * 0.3f; // サブスクリプションベースのゲームにおけるゲーム内アプリ購入への反感が売上に与える減少量。
                            estimatedSales -= subscriptionGameInAppPurchaseHateSalesDecrease;
                            float inAppPurchaseMoneyPerWeek2 = instance.GetInAppPurchaseMoneyPerWeek();

                            // num55 サブスクリプションからの週間収入予測値 -> subscriptionWeeklyRevenueEstimate
                            float subscriptionWeeklyRevenueEstimate = UnityEngine.Random.Range((float)(instance.abonnements / 100L * 4L), (float)(instance.abonnements / 100L * 5L)); // サブスクリプションから期待される週間収入の予測値。
                            if (instance.IsMyGame())
                            {
                                // num56 アチーブメントボーナスによるサブスクリプション収入の増加率 -> subscriptionAchievementBonusIncomeIncrease
                                float subscriptionAchievementBonusIncomeIncrease = (float)instance.mS_.GetAchivementBonus((int)AchivementType.InAppPurchases);
                                subscriptionAchievementBonusIncomeIncrease *= 0.01f; // サブスクリプション収入にアチーブメントボーナスが与える増加率。
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
                            instance.inAppPurchaseWeek = Mathf.RoundToInt(subscriptionWeeklyRevenueEstimate);

                            // ------------------------------------------------------------------------------------
                            // num57 サブスクリプション型ゲームの実際の週間収益 -> actualSubscriptionWeeklyRevenue
                            long actualSubscriptionWeeklyRevenue = (long)(inAppPurchaseMoneyPerWeek2 * (float)Mathf.RoundToInt(subscriptionWeeklyRevenueEstimate)); // サブスクリプション型ゲームから得られる実際の週間収益。
                            instance.umsatzTotal += actualSubscriptionWeeklyRevenue;
                            instance.umsatzInApp += actualSubscriptionWeeklyRevenue;
                            if (instance.IsMyGame())
                            {
                                instance.mS_.Earn(actualSubscriptionWeeklyRevenue, 8);
                            }
                            if (instance.IsMyGame())
                            {
                                instance.PayGewinnbeteiligung(actualSubscriptionWeeklyRevenue);
                            }
                            if (!instance.IsMyGame())
                            {
                                instance.AddTochterfirmaUmsatz(actualSubscriptionWeeklyRevenue);
                            }
                        }

                        if (instance.gameTyp == 2 && instance.releaseDate <= 0)
                        {
                            // num58 ゲームパス型ゲームのインアップ購入に対する反感による売上減少量 -> gamePassInAppPurchaseDislikeSalesDecrease
                            // ゲームパス型ゲームにおけるインアップ購入への反感が売上に与える減少量。
                            float gamePassInAppPurchaseHateSalesDecrease = estimatedSales * (instance.GetInAppPurchaseHate() * 0.01f) * 0.3f;
                            estimatedSales -= gamePassInAppPurchaseHateSalesDecrease;
                            float inAppPurchaseMoneyPerWeek3 = instance.GetInAppPurchaseMoneyPerWeek();

                            // num59 ゲームパス型ゲームからの週間収入予測値 -> gamePassWeeklyRevenueEstimate
                            float gamePassWeeklyRevenueEstimate = UnityEngine.Random.Range(((float)instance.abonnements + estimatedSales) / 100f * 150f, ((float)instance.abonnements + estimatedSales) / 100f * 200f); // ゲームパス型ゲームから期待される週間収入の予測値。

                            if (instance.IsMyGame())
                            {
                                // num60 アチーブメントボーナスによるゲームパス収入の増加率 -> gamePassAchievementBonusIncomeIncrease
                                float gamePassAchievementBonusIncomeIncrease = (float)instance.mS_.GetAchivementBonus((int)AchivementType.InAppPurchases);
                                gamePassAchievementBonusIncomeIncrease *= 0.01f; // ゲームパス収入にアチーブメントボーナスが与える増加率。
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
                            instance.inAppPurchaseWeek = Mathf.RoundToInt(gamePassWeeklyRevenueEstimate);

                            // num61 ゲームパス型ゲームの実際の週間収益 -> actualGamePassWeeklyRevenue
                            long actualGamePassWeeklyRevenue = (long)(inAppPurchaseMoneyPerWeek3 * (float)Mathf.RoundToInt(gamePassWeeklyRevenueEstimate)); // ゲームパス型ゲームから得られる実際の週間収益。
                            instance.umsatzTotal += actualGamePassWeeklyRevenue;
                            instance.umsatzInApp += actualGamePassWeeklyRevenue;
                            if (instance.IsMyGame())
                            {
                                instance.mS_.Earn(actualGamePassWeeklyRevenue, 8);
                            }
                            if (instance.IsMyGame())
                            {
                                instance.PayGewinnbeteiligung(actualGamePassWeeklyRevenue);
                            }
                            if (!instance.IsMyGame())
                            {
                                instance.AddTochterfirmaUmsatz(actualGamePassWeeklyRevenue);
                            }
                        }
                    }
                    if (instance.inGamePass)
                    {
                        // num62 ゲームパスによる売上調整量 -> gamePassSalesAdjustment
                        float gamePassSalesAdjustment = 0f;
                        if (estimatedSales > 0f)
                        {
                            // num63 アクティブなプラットフォームユーザー総数 -> totalActivePlatformUsers
                            long totalActivePlatformUsers = 0L;
                            // num64 ゲームパスを持つプラットフォームのアクティブユーザー数 -> activeGamePassPlatformUsers
                            long activeGamePassPlatformUsers = 0L;

                            for (int m = 0; m < instance.gamePlatformScript.Length; m++)
                            {
                                if (instance.gamePlatformScript[m])
                                {
                                    totalActivePlatformUsers += (long)instance.gamePlatformScript[m].GetAktiveNutzer();
                                    if (instance.gamePlatformScript[m].inGamePass || instance.gamePlatformScript[m].inGamePassPassiv)
                                    {
                                        activeGamePassPlatformUsers += (long)instance.gamePlatformScript[m].GetAktiveNutzer();
                                    }
                                }
                            }
                            if (totalActivePlatformUsers > 100L)
                            {
                                // num65 ゲームパスプラットフォームユーザーによる売上調整量 -> gamePassUserRatio
                                long gamePassUserRatio = totalActivePlatformUsers / 100L;
                                gamePassUserRatio = activeGamePassPlatformUsers / gamePassUserRatio;
                                gamePassSalesAdjustment = estimatedSales * ((float)gamePassUserRatio * 0.01f);
                                gamePassSalesAdjustment *= instance.games_.GetGamePassInteressted();
                                gamePassSalesAdjustment *= 0.33f;
                                switch (instance.gpS_.gamePass_AboPreis)
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
                        if (instance.GetVorbestellungen() > 500)
                        {
                            // num66 予約注文からランダムに減少させる数 -> randomPreorderReduction
                            int randomPreorderReduction = UnityEngine.Random.Range(1, instance.GetVorbestellungen() / 33);
                            instance.vorbestellungen -= randomPreorderReduction;
                            gamePassSalesAdjustment += (float)randomPreorderReduction;
                        }
                        instance.gamePassPlayer = Mathf.RoundToInt(gamePassSalesAdjustment) + UnityEngine.Random.Range(1, 10);
                        instance.gpS_.gamePass_AbosLetzteWoche += (long)Mathf.RoundToInt(gamePassSalesAdjustment);
                    }

                    // num67 実際の販売数（負の値を0に調整）-> actualSales
                    int actualSales = Mathf.RoundToInt(estimatedSales);
                    if (actualSales < 0)
                    {
                        actualSales = 0;
                    }
                    if ((instance.IsMyGame() || (instance.typ_contractGame && instance.developerID == instance.mS_.myID)) && instance.releaseDate <= 0 && !instance.typ_bundle)
                    {
                        instance.AddFans(Mathf.RoundToInt(estimatedSales));
                    }
                    // 物理版とデジタル版の売上処理
                    // num68 物理版基本売上 -> physicalBaseSales
                    float physicalBaseSales = 0f;
                    // num69 デラックス版物理売上 -> physicalDeluxeSales
                    float physicalDeluxeSales = 0f;
                    // num70 コレクターズ版物理売上 -> physicalCollectorsSales
                    float physicalCollectorsSales = 0f;
                    // num71 デジタル版売上 -> digitalSales
                    float digitalSales = 0f;

                    if (instance.releaseDate <= 0)
                    {
                        for (int n = instance.sellsPerWeek.Length - 1; n >= 1; n--)
                        {
                            instance.sellsPerWeek[n] = instance.sellsPerWeek[n - 1];
                            instance.sellsPerWeekOnline[n] = instance.sellsPerWeekOnline[n - 1];
                        }
                        if (instance.publisherID != instance.mS_.myID)
                        {
                            actualSales = Mathf.RoundToInt((float)actualSales * instance.GetPreisAbzug(0));
                            instance.sellsPerWeek[0] = actualSales;
                            instance.sellsPerWeekOnline[0] = instance.games_.GetDigitalSells() * UnityEngine.Random.Range(80f, 100f);
                            instance.sellsTotal += (long)actualSales;
                        }
                        else if (instance.IsMyGame())
                        {
                            if (instance.gameTyp != 2)
                            {
                                if (!instance.arcade)
                                {
                                    float digitalSells = instance.games_.GetDigitalSells();
                                    if (instance.digitalVersion)
                                    {
                                        digitalSales = (float)actualSales * digitalSells * instance.GetPreisAbzug(3);
                                        if (!instance.retailVersion)
                                        {
                                            digitalSales += (float)actualSales * 0.2f * instance.GetPreisAbzug(3);
                                        }
                                    }
                                    if (instance.retailVersion)
                                    {
                                        physicalBaseSales = (float)actualSales * (1f - digitalSells) * instance.GetPreisAbzug(0);
                                        physicalBaseSales += physicalBaseSales * instance.GetEditionQualiaet(0);
                                        physicalDeluxeSales = (float)actualSales * instance.games_.GetDeluxeCurve() * instance.GetPreisAbzug(1) * instance.GetEditionQualiaet(1);
                                        physicalBaseSales -= physicalDeluxeSales;
                                        physicalCollectorsSales = (float)actualSales * instance.games_.GetCollectorsCurve() * instance.GetPreisAbzug(2) * instance.GetEditionQualiaet(2);
                                        physicalCollectorsSales *= 0.5f;
                                        physicalBaseSales -= physicalCollectorsSales;
                                        if (!instance.digitalVersion)
                                        {
                                            physicalBaseSales += (float)actualSales * 0.2f * instance.GetPreisAbzug(0);
                                        }
                                        if (instance.lagerbestand[1] <= 0L)
                                        {
                                            physicalBaseSales += physicalDeluxeSales;
                                            physicalDeluxeSales = 0f;
                                        }
                                        if (instance.lagerbestand[2] <= 0L)
                                        {
                                            physicalBaseSales += physicalCollectorsSales;
                                            physicalCollectorsSales = 0f;
                                        }
                                        if (physicalBaseSales < 0f)
                                        {
                                            physicalBaseSales = 0f;
                                        }
                                    }
                                    instance.sellsStandard_forProduction = (long)Mathf.RoundToInt(physicalBaseSales);
                                    physicalBaseSales += (float)instance.vorbestellungen;
                                    instance.vorbestellungen = 0;
                                    if (instance.retailVersion)
                                    {
                                        digitalSales = (float)Mathf.RoundToInt(digitalSales);
                                        physicalBaseSales = (float)Mathf.RoundToInt(physicalBaseSales);
                                        physicalDeluxeSales = (float)Mathf.RoundToInt(physicalDeluxeSales);
                                        physicalCollectorsSales = (float)Mathf.RoundToInt(physicalCollectorsSales);
                                        if ((float)instance.lagerbestand[0] < physicalBaseSales)
                                        {
                                            instance.vorbestellungen += Mathf.RoundToInt(physicalBaseSales - (float)instance.lagerbestand[0]);
                                            physicalBaseSales = (float)instance.lagerbestand[0];
                                        }
                                        instance.lagerbestand[0] -= (long)Mathf.RoundToInt(physicalBaseSales);
                                        if ((float)instance.lagerbestand[1] < physicalDeluxeSales)
                                        {
                                            physicalDeluxeSales = (float)instance.lagerbestand[1];
                                        }
                                        instance.lagerbestand[1] -= (long)Mathf.RoundToInt(physicalDeluxeSales);
                                        if ((float)instance.lagerbestand[2] < physicalCollectorsSales)
                                        {
                                            physicalCollectorsSales = (float)instance.lagerbestand[2];
                                        }
                                        instance.lagerbestand[2] -= (long)Mathf.RoundToInt(physicalCollectorsSales);
                                    }
                                    instance.sellsPerWeek[0] = Mathf.RoundToInt(digitalSales + physicalBaseSales + physicalDeluxeSales + physicalCollectorsSales);
                                    instance.sellsPerWeekOnline[0] = 100f / (digitalSales + physicalBaseSales + physicalDeluxeSales + physicalCollectorsSales) * digitalSales;
                                    instance.sellsTotal += (long)Mathf.RoundToInt(digitalSales + physicalBaseSales + physicalDeluxeSales + physicalCollectorsSales);
                                    instance.sellsTotalStandard += (long)Mathf.RoundToInt(physicalBaseSales);
                                    instance.sellsTotalDeluxe += (long)Mathf.RoundToInt(physicalDeluxeSales);
                                    instance.sellsTotalCollectors += (long)Mathf.RoundToInt(physicalCollectorsSales);
                                    instance.sellsTotalOnline += (long)Mathf.RoundToInt(digitalSales);
                                    if (instance.vorbestellungen > 0 && instance.releaseDate <= 0 && physicalBaseSales <= 0f)
                                    {
                                        instance.vorbestellungen -= UnityEngine.Random.Range(0, instance.vorbestellungen / 50 + 3);
                                        instance.vorbestellungen -= instance.weeksOnMarket * 2;
                                        if (instance.vorbestellungen < 0)
                                        {
                                            instance.vorbestellungen = 0;
                                        }
                                    }
                                }
                                else
                                {
                                    physicalBaseSales = (float)actualSales * instance.GetPreisAbzug(0);
                                    instance.sellsPerWeek[0] = Mathf.RoundToInt(physicalBaseSales);
                                    instance.sellsPerWeekOnline[0] = 0f;
                                    instance.vorbestellungen += Mathf.RoundToInt(physicalBaseSales);
                                    if (instance.vorbestellungen > 50)
                                    {
                                        instance.stornierungen = UnityEngine.Random.Range(0, instance.vorbestellungen / 50 + 3);
                                        instance.vorbestellungen -= instance.stornierungen;
                                    }
                                    else
                                    {
                                        instance.stornierungen = 0;
                                        if (instance.weeksOnMarket > 20 && instance.vorbestellungen > 0)
                                        {
                                            instance.stornierungen = 1;
                                            instance.vorbestellungen--;
                                        }
                                    }
                                }
                            }
                            else
                            {
                                digitalSales = (float)actualSales;
                                instance.sellsPerWeek[0] = Mathf.RoundToInt(digitalSales);
                                instance.sellsPerWeekOnline[0] = 100f;
                                instance.sellsTotal += (long)Mathf.RoundToInt(digitalSales);
                                instance.sellsTotalOnline += (long)Mathf.RoundToInt(digitalSales);
                            }
                        }
                    }
                    else if (instance.retailVersion)
                    {
                        instance.vorbestellungen += Mathf.RoundToInt((float)actualSales * instance.GetPreisAbzug(0) / (float)(instance.releaseDate + 1));
                    }
                    if (instance.IsMyGame())
                    {
                        if (instance.hype > 0f && instance.releaseDate <= 0)
                        {
                            instance.AddHype(-UnityEngine.Random.Range(0.1f, 1f));
                        }
                    }
                    else
                    {
                        instance.hype = 100f;
                    }

                    // ===================================================================================================
                    if (instance.releaseDate <= 0 && ((instance.sellsPerWeek[0] > 100 && !instance.arcade) || instance.sellsTotal > 100L || (instance.arcade && instance.sellsTotal > 0L)) && !instance.typ_budget && !instance.typ_goty)
                    {
                        // num72 ユーザーレビューのランダムな変動 -> userReviewRandomVariance
                        float userReviewRandomVariance;
                        if (!instance.arcade)
                        {
                            userReviewRandomVariance = (float)instance.sellsPerWeek[0];
                            userReviewRandomVariance = UnityEngine.Random.Range(userReviewRandomVariance * 0.01f, userReviewRandomVariance * 0.02f);
                        }
                        else
                        {
                            userReviewRandomVariance = (float)instance.sellsTotal;
                            userReviewRandomVariance = UnityEngine.Random.Range(userReviewRandomVariance * 0.01f, userReviewRandomVariance * 0.02f) + (float)UnityEngine.Random.Range(0, 5);
                        }
                        // num73 ユーザーレビューの最終スコア -> userReviewFinalScore
                        float userReviewFinalScore = 0f;
                        switch (UnityEngine.Random.Range(0, 5))
                        {
                            case 0:
                                userReviewFinalScore = userReviewRandomVariance * (float)instance.reviewGameplay / 100f;
                                break;
                            case 1:
                                userReviewFinalScore = userReviewRandomVariance * (float)instance.reviewGrafik / 100f;
                                break;
                            case 2:
                                userReviewFinalScore = userReviewRandomVariance * (float)instance.reviewSound / 100f;
                                break;
                            case 3:
                                userReviewFinalScore = userReviewRandomVariance * (float)instance.reviewSteuerung / 100f;
                                break;
                            case 4:
                                userReviewFinalScore = userReviewRandomVariance * (float)instance.reviewTotal / 100f;
                                break;
                        }
                        userReviewFinalScore -= UnityEngine.Random.Range(0f, instance.points_bugs);
                        if (userReviewFinalScore < 0f)
                        {
                            userReviewFinalScore = 0f;
                        }
                        if (!instance.mS_.settings_sabotageOff && instance.mS_.sabotage_reviews > 0)
                        {
                            userReviewFinalScore /= 2f;
                        }
                        instance.userPositiv += Mathf.RoundToInt(userReviewFinalScore);
                        instance.userNegativ += Mathf.RoundToInt(userReviewRandomVariance - userReviewFinalScore);
                    }
                    if (instance.gameTyp != 2 && !instance.arcade && !instance.typ_addon && !instance.typ_addonStandalone && !instance.typ_mmoaddon && instance.releaseDate <= 0)
                    {
                        if (!instance.devS_)
                        {
                            instance.FindMyDeveloper();
                        }
                        if (!instance.pS_)
                        {
                            instance.FindMyPublisher();
                        }
                        if (currentTotalSales < 1000000L && instance.sellsTotal >= 1000000L)
                        {
                            instance.mS_.AddAwards(7, instance.devS_);
                            if (instance.publisherID != instance.developerID)
                            {
                                instance.mS_.AddAwards(7, instance.pS_);
                            }
                            if (instance.IsMyGame() || instance.developerID == instance.mS_.myID)
                            {
                                instance.guiMain_.CreateTopNewsGoldeneSchallplatte(instance.GetNameWithTag());
                                instance.mS_.goldeneSchallplatten++;
                            }
                        }
                        if (currentTotalSales < 5000000L && instance.sellsTotal >= 5000000L)
                        {
                            instance.mS_.AddAwards(10, instance.devS_);
                            if (instance.publisherID != instance.developerID)
                            {
                                instance.mS_.AddAwards(10, instance.pS_);
                            }
                            if (instance.IsMyGame() || instance.developerID == instance.mS_.myID)
                            {
                                instance.guiMain_.CreateTopNewsPlatinSchallplatte(instance.GetNameWithTag());
                                instance.mS_.platinSchallplatten++;
                            }
                        }
                        if (currentTotalSales < 10000000L && instance.sellsTotal >= 10000000L)
                        {
                            instance.mS_.AddAwards(11, instance.devS_);
                            if (instance.publisherID != instance.developerID)
                            {
                                instance.mS_.AddAwards(11, instance.pS_);
                            }
                            if (instance.IsMyGame() || instance.developerID == instance.mS_.myID)
                            {
                                instance.guiMain_.CreateTopNewsDiamantSchallplatte(instance.GetNameWithTag());
                                instance.mS_.diamantSchallplatten++;
                            }
                        }
                    }
                    if (instance.IsMyGame())
                    {
                        if (instance.releaseDate <= 0 && instance.mS_.achScript_ && instance.gameTyp != 2)
                        {
                            if (instance.sellsTotal >= 1000000L)
                            {
                                instance.mS_.achScript_.SetAchivement(48);
                            }
                            if (instance.sellsTotal >= 10000000L)
                            {
                                instance.mS_.achScript_.SetAchivement(49);
                            }
                            if (instance.sellsTotal >= 50000000L)
                            {
                                instance.mS_.achScript_.SetAchivement(50);
                            }
                        }

                        // ===================================================================================================
                        Traverse.Create(instance).Method("UpdateFanletter").GetValue();
                        if (!instance.typ_addon && !instance.typ_mmoaddon && !instance.arcade)
                        {
                            // num74 サポートコールの予測数 -> predictedSupportCalls
                            float predictedSupportCalls = (float)actualSales * 0.001f + instance.points_bugs;
                            predictedSupportCalls = UnityEngine.Random.Range(0f, predictedSupportCalls);
                            if (instance.gameTyp == 2)
                            {
                                predictedSupportCalls /= 5f;
                            }
                            instance.mS_.AddAnrufe(Mathf.RoundToInt(predictedSupportCalls));
                        }
                        if (instance.publisherID != instance.mS_.myID)
                        {
                            if (instance.pS_)
                            {
                                instance.mS_.AddVerkaufsverlauf((long)actualSales);
                                float f;
                                if (instance.mS_.exklusivVertrag_ID == instance.publisherID)
                                {
                                    f = (float)(actualSales * instance.pS_.GetShareExklusiv());
                                }
                                else
                                {
                                    f = (float)(actualSales * instance.pS_.GetShare());
                                }
                                // num75 パブリッシャー収益 -> publisherRevenue
                                int publisherRevenue = Mathf.RoundToInt(f);
                                instance.umsatzTotal += (long)publisherRevenue;
                                instance.mS_.Earn((long)publisherRevenue, 3);
                                instance.PayGewinnbeteiligung((long)publisherRevenue);
                                long subscriptionIncome = 0L;
                                if (instance.gameTyp == 1 && instance.mS_.week == 5)
                                {
                                    // num76 サブスクリプション収入 -> subscriptionIncome
                                    subscriptionIncome = instance.abonnements * (long)instance.aboPreis;
                                    instance.umsatzTotal += subscriptionIncome;
                                    instance.umsatzAbos += subscriptionIncome;
                                    instance.mS_.Earn(subscriptionIncome, 7);
                                    instance.PayGewinnbeteiligung(subscriptionIncome);
                                    instance.costs_server += instance.abonnements / 10L;
                                }
                                instance.PlayerPayEngineLicence((long)publisherRevenue + subscriptionIncome);
                                if (instance.hype < 50f && (UnityEngine.Random.Range(0f, 100f + instance.pS_.stars) > 90f || instance.weeksOnMarket <= 1))
                                {
                                    instance.AddHype(UnityEngine.Random.Range(15f, instance.pS_.stars + 15f));
                                    if (instance.hype < 0f)
                                    {
                                        instance.hype = 0f;
                                    }
                                    if (instance.hype > 100f)
                                    {
                                        instance.hype = 100f;
                                    }
                                    string text = instance.tS_.GetText(495);
                                    text = text.Replace("<NAME1>", instance.GetNameWithTag());
                                    instance.guiMain_.CreateTopNewsInfo(text);
                                }
                            }
                        }
                        else if (!instance.arcade)
                        {
                            instance.mS_.AddVerkaufsverlauf((long)Mathf.RoundToInt(digitalSales + physicalBaseSales + physicalDeluxeSales + physicalCollectorsSales));
                            if (digitalSales > 0f)
                            {
                                instance.mS_.AddDownloadverlauf((long)Mathf.RoundToInt(digitalSales));
                            }
                            // num77 総収益 -> totalRevenue
                            long totalRevenue = 0L;
                            if (instance.gameTyp != 2)
                            {
                                totalRevenue = Convert.ToInt64(digitalSales * (float)instance.verkaufspreis[3]) + Convert.ToInt64(physicalBaseSales * (float)instance.verkaufspreis[0]) + Convert.ToInt64(physicalDeluxeSales * (float)instance.verkaufspreis[1]) + Convert.ToInt64(physicalCollectorsSales * (float)instance.verkaufspreis[2]);
                                instance.umsatzTotal += totalRevenue;
                                instance.mS_.Earn(totalRevenue, 3);
                                instance.PayGewinnbeteiligung(totalRevenue);
                            }
                            // num78 サブスクリプション収入 -> subscriptionIncome
                            long subscriptionIncome;
                            if (instance.gameTyp == 1 && instance.mS_.week == 5)
                            {
                                subscriptionIncome = instance.abonnements * (long)instance.aboPreis;
                                instance.umsatzTotal += subscriptionIncome;
                                instance.umsatzAbos += subscriptionIncome;
                                instance.mS_.Earn(subscriptionIncome, 7);
                                instance.PayGewinnbeteiligung(subscriptionIncome);
                                instance.costs_server += instance.abonnements / 10L;
                            }
                            subscriptionIncome = 0L;
                            if (instance.gameTyp == 2 && instance.mS_.week == 5)
                            {
                                instance.costs_server += instance.abonnements / 10L;
                            }
                            instance.PlayerPayEngineLicence(totalRevenue + subscriptionIncome);
                            if (instance.autoPreis && !instance.arcade && !instance.handy)
                            {
                                instance.UpdateAutoPreis();
                            }
                        }
                        if (instance.gameTab_)
                        {
                            instance.gameTab_.UpdateData();
                        }
                        if ((instance.publisherID != instance.mS_.myID && actualSales <= 0 && instance.abonnements <= 0L) || (instance.publisherID == instance.mS_.myID && instance.mS_.automatic_RemoveGameFormMarket && instance.releaseDate <= 0 && instance.weeksOnMarket > 4 && instance.vorbestellungen <= 0 && instance.sellsPerWeek[0] <= instance.mS_.automatic_RemoveGameFormMarket_Amount && instance.abonnements <= 0L))
                        {
                            if (!instance.arcade || (instance.arcade && instance.vorbestellungen <= 0))
                            {
                                instance.guiMain_.ActivateMenu(instance.guiMain_.uiObjects[82]);
                                instance.guiMain_.uiObjects[82].GetComponent<Menu_GameFromMarket>().Init(instance, false);
                                instance.guiMain_.OpenMenu(false);
                                instance.RemoveFromMarket();
                            }
                        }
                        else
                        {
                            if (instance.sellsTotal > 0L && instance.weeksOnMarket < 24 && !instance.guiMain_.menuOpen && instance.reviewTotal > 90 && !instance.trendsetter && instance.releaseDate <= 0 && instance.mS_.trendGenre != instance.maingenre && !instance.typ_mmoaddon && !instance.typ_addon && !instance.typ_budget && !instance.typ_bundle && !instance.typ_addonStandalone && !instance.typ_goty && !instance.typ_bundleAddon && UnityEngine.Random.Range(0, 200) == 1)
                            {
                                instance.guiMain_.ActivateMenu(instance.guiMain_.uiObjects[142]);
                                instance.guiMain_.uiObjects[142].GetComponent<Menu_Trendsetter>().Init(instance);
                                instance.guiMain_.OpenMenu(false);
                                instance.mS_.award_Trendsetter++;
                                instance.AddHype(30f);
                                instance.AddIpPoints(70f);
                                if (instance.mS_.achScript_)
                                {
                                    instance.mS_.achScript_.SetAchivement(34);
                                }
                            }
                            if (instance.sellsTotal > 0L && instance.commercialFlop && instance.weeksOnMarket == 4 && !instance.guiMain_.menuOpen)
                            {
                                instance.guiMain_.ActivateMenu(instance.guiMain_.uiObjects[352]);
                                instance.guiMain_.uiObjects[352].GetComponent<Menu_RandomEventCommercialFlop>().Init(instance);
                            }
                            if (instance.sellsTotal > 0L && instance.commercialHit && instance.weeksOnMarket == 4 && !instance.guiMain_.menuOpen)
                            {
                                instance.guiMain_.ActivateMenu(instance.guiMain_.uiObjects[354]);
                                instance.guiMain_.uiObjects[354].GetComponent<Menu_RandomEventCommercialHit>().Init(instance);
                            }
                            if (instance.sellsTotal > 0L && instance.points_bugsInvis > 0f && instance.mS_.difficulty >= 2 && instance.weeksOnMarket >= 4 && instance.weeksOnMarket <= 20 && UnityEngine.Random.Range(0, 200) <= instance.mS_.difficulty && !instance.guiMain_.menuOpen)
                            {
                                instance.guiMain_.ActivateMenu(instance.guiMain_.uiObjects[353]);
                                instance.guiMain_.uiObjects[353].GetComponent<Menu_RandomEventBugs>().Init(instance);
                            }
                        }
                    }
                    else
                    {
                        if (instance.gameTyp != 2)
                        {
                            // 売上総収益 -> totalRevenue2
                            float totalRevenue2 = 0f;
                            if (!instance.handy && !instance.arcade)
                            {
                                if (instance.publisherID != instance.developerID)
                                {
                                    if (instance.pS_)
                                    {
                                        totalRevenue2 = (float)actualSales * instance.pS_.share;
                                    }
                                }
                                else
                                {
                                    totalRevenue2 = (float)(actualSales * instance.verkaufspreis[0]);
                                }
                            }
                            if (instance.handy)
                            {
                                totalRevenue2 = (float)(actualSales * 3);
                            }
                            if (instance.arcade)
                            {
                                totalRevenue2 = (float)(actualSales * instance.verkaufspreis[0]);
                            }
                            // num79 アーケードゲームの売上収益 -> arcadeGameRevenue
                            int arcadeGameRevenue = Mathf.RoundToInt(totalRevenue2);
                            instance.umsatzTotal += (long)arcadeGameRevenue;
                            instance.AddTochterfirmaUmsatz((long)arcadeGameRevenue);
                        }
                        if (instance.gameTyp == 1 && instance.mS_.week == 5)
                        {
                            // num80 : 定期購読による売上収益 -> subscriptionRevenue
                            long subscriptionRevenue = instance.abonnements * (long)instance.aboPreis;
                            instance.umsatzTotal += subscriptionRevenue;
                            instance.umsatzAbos += subscriptionRevenue;
                            instance.AddTochterfirmaUmsatz(subscriptionRevenue);
                            instance.costs_server += instance.abonnements / 10L;
                        }
                        if (instance.gameTyp == 2 && instance.mS_.week == 5)
                        {
                            instance.costs_server += instance.abonnements / 10L;
                        }
                        if (!instance.engineS_)
                        {
                            instance.FindMyEngineNew();
                        }
                        if (instance.engineS_ && instance.engineS_.ownerID == instance.mS_.myID && instance.reviewTotal > 50)
                        {
                            instance.AddFans(actualSales);
                        }
                        if ((actualSales <= 0 && instance.abonnements < 10L) || (instance.gameTyp == 2 && instance.abonnements < 10L && instance.weeksOnMarket > 5))
                        {
                            if (!instance.typ_bundle)
                            {
                                instance.FindMyEngineNew();
                                if (instance.engineS_)
                                {
                                    if (instance.engineS_.ownerID == instance.mS_.myID)
                                    {
                                        if (instance.guiMain_)
                                        {
                                            instance.guiMain_.OpenEngineAbrechnung(instance);
                                        }
                                    }
                                    else if (instance.mS_.multiplayer && instance.engineS_.EngineFromMitspieler() && instance.mS_.mpCalls_.isServer)
                                    {
                                        instance.mS_.mpCalls_.SERVER_Send_EngineAbrechnung(instance.engineS_.ownerID, instance.myID);
                                    }
                                    if (instance.GetPublisherOrDeveloperIsTochterfirma() && instance.guiMain_)
                                    {
                                        instance.guiMain_.OpenTochterfirmaAbrechnung(instance);
                                    }
                                }
                            }
                            instance.RemoveFromMarket();
                        }
                    }
                    if (instance.typ_mmoaddon)
                    {
                        gameScript gameScript = instance.FindVorgaengerScript();
                        if (gameScript && !gameScript.isOnMarket)
                        {
                            instance.RemoveFromMarket();
                        }
                    }
                    if (instance.mS_.multiplayer)
                    {
                        if (instance.mS_.mpCalls_.isServer && (instance.IsMyGame() || instance.typ_contractGame || (instance.DeveloperIsNPC() && instance.PublisherIsNPC() && instance.OwnerIsNPC())))
                        {
                            instance.mS_.mpCalls_.SERVER_Send_GameSell(instance);
                        }
                        if (instance.mS_.mpCalls_.isClient && instance.IsMyGame())
                        {
                            instance.mS_.mpCalls_.CLIENT_Send_GameSell(instance);
                        }
                    }
                }
            }
            //--------------------------------------------------------------------------------
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
}
