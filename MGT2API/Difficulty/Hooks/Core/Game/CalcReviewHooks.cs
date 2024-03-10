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
    internal static class CalcReviewHooks
    {
        private static float GetBaseScoreMultiplier(int difficulty)
        {
            switch (difficulty)
            {
                case 0:
                    return 7000f;
                case 1:
                    return 10000f;
                case 2:
                    return 15000f;
                case 3:
                    return 18000f;
                case 4:
                    return 22000f;
                case 5:
                    return 30000f;
                default:
                    throw new ArgumentOutOfRangeException(nameof(difficulty), difficulty, "Invalid difficulty level.");
            }
        }

        private static float GetRetroBaseScoreMultiplier(int difficulty)
        {
            switch (difficulty)
            {
                case 0:
                    return 2500f;
                case 1:
                    return 3000f;
                case 2:
                    return 3500f;
                case 3:
                    return 4000f;
                case 4:
                    return 4200f;
                case 5:
                    return 4500f;
                default:
                    throw new ArgumentOutOfRangeException(nameof(difficulty), difficulty, "Invalid difficulty level.");
            }
        }

        /// <summary>
        /// Patches the custom multiplier for the review (rating) at the time of game release.
        /// [JP] ゲームリリース時のレビュー（評価）に適用するカスタム乗数をパッチします。
        /// </summary>
        /// <param name="__instance"></param>
        /// <param name="entwicklungsbericht"></param>
        /// <returns></returns>
        [HarmonyPatch(typeof(gameScript), "CalcReview")]
        internal static bool Prefix(ref gameScript __instance, bool entwicklungsbericht)
        {
            if (__instance.reviewTotal > 0)
            {
                return false;
            }
            __instance.date_month = __instance.mS_.month;
            __instance.date_year = __instance.mS_.year;
            //num: これは、難易度に応じた基本スコアに GetReviewCurve の結果を乗じたものを保持しているようだ。
            //そのため、baseScoreMultiplier などが適切かもしれん。
            //numをリファクタリング
            float baseScoreMultiplier = 0f;
            if (__instance.developerID == __instance.mS_.myID)
            {
                if (!__instance.retro)
                {
                    baseScoreMultiplier = GetBaseScoreMultiplier(__instance.mS_.difficulty) * __instance.games_.GetReviewCurve();
                }
                else
                {
                    baseScoreMultiplier = GetRetroBaseScoreMultiplier(__instance.mS_.difficulty) * __instance.games_.GetReviewCurve();
                }
            }
            else if (!__instance.retro)
            {
                baseScoreMultiplier = 14000f * __instance.games_.GetReviewCurve();
            }
            else
            {
                baseScoreMultiplier = 4000f * __instance.games_.GetReviewCurve();
            }
        //num2: gameplayScore という名前がいいかもしれん。
        //__instance.points_gameplay に何らかの計算を施した結果を保持しているからな。
        float gameplayScore;
            if (__instance.mS_.year >= 1979)
            {
                gameplayScore = __instance.points_gameplay / (baseScoreMultiplier / 100f);
            }
            else
            {
                gameplayScore = __instance.points_gameplay / (baseScoreMultiplier / 90f);
            }
            //num3: これはグラフィックに関するスコアを保持しているようだから、graphicsScore と名付けるのが適切だろう。
            float graphicsScore;
            if (__instance.mS_.year >= 1982)
            {
                graphicsScore = __instance.points_grafik / (baseScoreMultiplier / 100f);
            }
            else
            {
                graphicsScore = __instance.points_grafik / (baseScoreMultiplier / 90f);
            }
            //num4: これはサウンドに関するスコアを保持しているので、soundScore と呼ぶのがいい。
            float soundScore;
            if (__instance.mS_.year >= 1985)
            {
                soundScore = __instance.points_sound / (baseScoreMultiplier / 100f);
            }
            else
            {
                soundScore = __instance.points_sound / (baseScoreMultiplier / 90f);
            }
            //num5: これは技術的なスコアか何かを保持しているようだから、technicalScore と名付けるのが適切だろう。
            float technicalScore;
            if (__instance.unlock_.Get(8))
            {
                technicalScore = __instance.points_technik / (baseScoreMultiplier / 100f);
            }
            else
            {
                technicalScore = __instance.points_technik / (baseScoreMultiplier / 80f);
            }
            //num6: これは最終的なゲームのレビュースコアを計算するために使われているようだ。
            //そのため、finalReviewScore という名前が適切かもしれん。
            float finalReviewScore = 0f;
            if (gameplayScore > 99f)
            {
                gameplayScore = 99f;
            }
            if (graphicsScore > 99f)
            {
                graphicsScore = 99f;
            }
            if (soundScore > 99f)
            {
                soundScore = 99f;
            }
            if (technicalScore > 99f)
            {
                technicalScore = 99f;
            }
            if (__instance.developerID != __instance.mS_.myID && __instance.retro)
            {
                graphicsScore *= 0.7f;
                soundScore *= 0.7f;
                technicalScore *= 0.7f;
            }
            if (__instance.developerID == __instance.mS_.myID)
            {
                //num7: この変数は、特定のプラットフォームの経験値を合計してプラットフォームの数で割ったものを保持しているようだ。
                //これは平均のプラットフォーム経験値を示しているので、averagePlatformExperience という名前がいいだろう。
                float averagePlatformExperience = 0f;
                //num8: これは使用されているプラットフォームの数を数えるために使われている。
                //したがって、platformCount という名前が適切だ。
                int platformCount = 0;
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
                            platformCount++;
                            averagePlatformExperience += (float)__instance.gamePlatformScript[i].erfahrung;
                        }
                    }
                }
                averagePlatformExperience /= (float)platformCount;
                graphicsScore -= (5f - averagePlatformExperience) * 2f;
                soundScore -= (5f - averagePlatformExperience) * 2f;
                technicalScore -= (5f - averagePlatformExperience) * 2f;
            }
            else if (!entwicklungsbericht)
            {
                graphicsScore -= UnityEngine.Random.Range(0f, 5f);
                soundScore -= UnityEngine.Random.Range(0f, 5f);
                technicalScore -= UnityEngine.Random.Range(0f, 5f);
            }
            else
            {
                graphicsScore -= 2.5f;
                soundScore -= 2.5f;
                technicalScore -= 2.5f;
            }
            if (__instance.developerID == __instance.mS_.myID && __instance.mS_.year >= 1979)
            {
                if (!__instance.gameplayStudio[0])
                {
                    gameplayScore -= 1f;
                }
                if (!__instance.gameplayStudio[1])
                {
                    gameplayScore -= 1f;
                }
                if (!__instance.gameplayStudio[2])
                {
                    gameplayScore -= 1f;
                }
                if (!__instance.gameplayStudio[3])
                {
                    gameplayScore -= 1f;
                }
                if (!__instance.gameplayStudio[4])
                {
                    gameplayScore -= 1f;
                }
                if (!__instance.gameplayStudio[5])
                {
                    gameplayScore -= 1f;
                }
            }
            if (__instance.mS_.year >= 1982)
            {
                if (!__instance.grafikStudio[0])
                {
                    graphicsScore -= 1f;
                }
                if (!__instance.grafikStudio[1])
                {
                    graphicsScore -= 1f;
                }
                if (!__instance.grafikStudio[2])
                {
                    graphicsScore -= 1f;
                }
                if (!__instance.grafikStudio[3])
                {
                    graphicsScore -= 1f;
                }
                if (!__instance.grafikStudio[4])
                {
                    graphicsScore -= 1f;
                }
                if (!__instance.grafikStudio[5])
                {
                    graphicsScore -= 1f;
                }
            }
            if (__instance.mS_.year >= 1985)
            {
                if (!__instance.soundStudio[0])
                {
                    soundScore -= 2f;
                }
                if (!__instance.soundStudio[1])
                {
                    soundScore -= 2f;
                }
                if (!__instance.soundStudio[2])
                {
                    soundScore -= 2f;
                }
                if (!__instance.soundStudio[3])
                {
                    soundScore -= 2f;
                }
                if (!__instance.soundStudio[4])
                {
                    soundScore -= 2f;
                }
                if (!__instance.soundStudio[5])
                {
                    soundScore -= 2f;
                }
            }
            if (__instance.unlock_.Get(8))
            {
                if (!__instance.motionCaptureStudio[0])
                {
                    graphicsScore -= 1f;
                }
                if (!__instance.motionCaptureStudio[1])
                {
                    graphicsScore -= 1f;
                }
                if (!__instance.motionCaptureStudio[2])
                {
                    graphicsScore -= 1f;
                }
                if (!__instance.motionCaptureStudio[3])
                {
                    graphicsScore -= 1f;
                }
                if (!__instance.motionCaptureStudio[4])
                {
                    graphicsScore -= 1f;
                }
                if (!__instance.motionCaptureStudio[5])
                {
                    graphicsScore -= 1f;
                }
            }
            if (!__instance.handy && !__instance.retro && __instance.developerID == __instance.mS_.myID)
            {
                int outdatetAmount = __instance.eF_.GetOutdatetAmount(__instance.gameEngineFeature[0]);
                int outdatetAmount2 = __instance.eF_.GetOutdatetAmount(__instance.gameEngineFeature[1]);
                int outdatetAmount3 = __instance.eF_.GetOutdatetAmount(__instance.gameEngineFeature[2]);
                int outdatetAmount4 = __instance.eF_.GetOutdatetAmount(__instance.gameEngineFeature[3]);
                if (outdatetAmount > 0)
                {
                    graphicsScore -= (float)(outdatetAmount * 2);
                }
                if (outdatetAmount2 > 0)
                {
                    soundScore -= (float)(outdatetAmount2 * 2);
                }
                if (outdatetAmount3 > 0)
                {
                    gameplayScore -= (float)outdatetAmount3;
                }
                if (outdatetAmount4 > 0)
                {
                    gameplayScore -= (float)outdatetAmount4;
                }
            }
            if (__instance.developerID == __instance.mS_.myID)
            {
                if (__instance.mS_.year >= 1983 && __instance.mS_.year < 1988 && __instance.gameSize == 0)
                {
                    gameplayScore -= 1f;
                    graphicsScore -= 1f;
                    soundScore -= 1f;
                    technicalScore -= 1f;
                }
                if (__instance.mS_.year >= 1988 && __instance.mS_.year < 1995)
                {
                    if (__instance.gameSize == 0)
                    {
                        gameplayScore -= 2f;
                        graphicsScore -= 2f;
                        soundScore -= 2f;
                        technicalScore -= 2f;
                    }
                    if (__instance.gameSize == 1)
                    {
                        gameplayScore -= 1f;
                        graphicsScore -= 1f;
                        soundScore -= 1f;
                        technicalScore -= 1f;
                    }
                }
                if (__instance.mS_.year >= 1995 && __instance.mS_.year < 2004)
                {
                    if (__instance.gameSize == 0)
                    {
                        gameplayScore -= 3f;
                        graphicsScore -= 3f;
                        soundScore -= 3f;
                        technicalScore -= 3f;
                    }
                    if (__instance.gameSize == 1)
                    {
                        gameplayScore -= 2f;
                        graphicsScore -= 2f;
                        soundScore -= 2f;
                        technicalScore -= 2f;
                    }
                    if (__instance.gameSize == 2)
                    {
                        gameplayScore -= 1f;
                        graphicsScore -= 1f;
                        soundScore -= 1f;
                        technicalScore -= 1f;
                    }
                }
                if (__instance.mS_.year >= 2004 && __instance.mS_.year < 2020)
                {
                    if (__instance.gameSize == 0)
                    {
                        gameplayScore -= 4f;
                        graphicsScore -= 4f;
                        soundScore -= 4f;
                        technicalScore -= 4f;
                    }
                    if (__instance.gameSize == 1)
                    {
                        gameplayScore -= 3f;
                        graphicsScore -= 3f;
                        soundScore -= 3f;
                        technicalScore -= 3f;
                    }
                    if (__instance.gameSize == 2)
                    {
                        gameplayScore -= 2f;
                        graphicsScore -= 2f;
                        soundScore -= 2f;
                        technicalScore -= 2f;
                    }
                    if (__instance.gameSize == 3)
                    {
                        gameplayScore -= 1f;
                        graphicsScore -= 1f;
                        soundScore -= 1f;
                        technicalScore -= 1f;
                    }
                }
                if (__instance.mS_.year >= 2020)
                {
                    if (__instance.gameSize == 0)
                    {
                        gameplayScore -= 5f;
                        graphicsScore -= 5f;
                        soundScore -= 5f;
                        technicalScore -= 5f;
                    }
                    if (__instance.gameSize == 1)
                    {
                        gameplayScore -= 4f;
                        graphicsScore -= 4f;
                        soundScore -= 4f;
                        technicalScore -= 4f;
                    }
                    if (__instance.gameSize == 2)
                    {
                        gameplayScore -= 3f;
                        graphicsScore -= 3f;
                        soundScore -= 3f;
                        technicalScore -= 3f;
                    }
                    if (__instance.gameSize == 3)
                    {
                        gameplayScore -= 2f;
                        graphicsScore -= 2f;
                        soundScore -= 2f;
                        technicalScore -= 2f;
                    }
                    if (__instance.gameSize == 4)
                    {
                        gameplayScore -= 1f;
                        graphicsScore -= 1f;
                        soundScore -= 1f;
                        technicalScore -= 1f;
                    }
                }
            }
            if (__instance.gameLicence != -1)
            {
                if (__instance.licences_.licence_GENREGOOD[__instance.gameLicence] == __instance.maingenre)
                {
                    gameplayScore += __instance.licences_.licence_QUALITY[__instance.gameLicence] * 0.01f * 3f;
                }
                if (__instance.licences_.licence_GENREGOOD[__instance.gameLicence] != __instance.maingenre && __instance.licences_.licence_GENREBAD[__instance.gameLicence] != __instance.maingenre)
                {
                    gameplayScore += __instance.licences_.licence_QUALITY[__instance.gameLicence] * 0.01f * 1f;
                }
                if (__instance.licences_.licence_GENREBAD[__instance.gameLicence] == __instance.maingenre)
                {
                    gameplayScore -= __instance.licences_.licence_QUALITY[__instance.gameLicence] * 0.01f * 5f;
                }
                if (__instance.subgenre != -1)
                {
                    if (__instance.licences_.licence_GENREGOOD[__instance.gameLicence] == __instance.subgenre)
                    {
                        gameplayScore += __instance.licences_.licence_QUALITY[__instance.gameLicence] * 0.01f * 2f;
                    }
                    if (__instance.licences_.licence_GENREGOOD[__instance.gameLicence] != __instance.subgenre && __instance.licences_.licence_GENREBAD[__instance.gameLicence] != __instance.subgenre)
                    {
                        gameplayScore += __instance.licences_.licence_QUALITY[__instance.gameLicence] * 0.01f * 1f;
                    }
                    if (__instance.licences_.licence_GENREBAD[__instance.gameLicence] == __instance.subgenre)
                    {
                        gameplayScore -= __instance.licences_.licence_QUALITY[__instance.gameLicence] * 0.01f * 3f;
                    }
                }
            }
            if (__instance.developerID != __instance.mS_.myID)
            {
                if (!entwicklungsbericht)
                {
                    gameplayScore -= UnityEngine.Random.Range(0f, 2f);
                    graphicsScore -= UnityEngine.Random.Range(0f, 2f);
                    soundScore -= UnityEngine.Random.Range(0f, 2f);
                    technicalScore -= UnityEngine.Random.Range(0f, 2f);
                }
                else
                {
                    gameplayScore -= 1f;
                    graphicsScore -= 1f;
                    soundScore -= 1f;
                    technicalScore -= 1f;
                }
            }
            finalReviewScore += gameplayScore * 0.01f * __instance.genres_.genres_GAMEPLAY[__instance.maingenre];
            finalReviewScore += graphicsScore * 0.01f * __instance.genres_.genres_GRAPHIC[__instance.maingenre];
            finalReviewScore += soundScore * 0.01f * __instance.genres_.genres_SOUND[__instance.maingenre];
            finalReviewScore += technicalScore * 0.01f * __instance.genres_.genres_CONTROL[__instance.maingenre];
            graphicsScore -= __instance.points_bugs * 0.1f;
            soundScore -= __instance.points_bugs * 0.1f;
            technicalScore -= __instance.points_bugs * 0.1f;
            gameplayScore -= __instance.points_bugs * 0.2f;
            finalReviewScore -= __instance.points_bugs * 0.1f;
            if (!__instance.genres_.IsTargetGroup(__instance.maingenre, __instance.gameZielgruppe))
            {
                gameplayScore -= 4f;
                finalReviewScore -= 3f;
            }
            if (!__instance.genres_.IsGenreCombination(__instance.maingenre, __instance.subgenre))
            {
                gameplayScore -= 4f;
                finalReviewScore -= 3f;
            }
            if (!__instance.themes_.IsThemesFitWithGenre(__instance.gameMainTheme, __instance.maingenre))
            {
                gameplayScore -= 4f;
                finalReviewScore -= 3f;
            }
            if (!__instance.themes_.IsThemesFitWithGenre(__instance.gameSubTheme, __instance.maingenre))
            {
                gameplayScore -= 2f;
                finalReviewScore -= 1.5f;
            }
            for (int j = 0; j < __instance.Designschwerpunkt.Length; j++)
            {
                if (__instance.Designschwerpunkt[j] < __instance.genres_.GetFocus(j, __instance.maingenre, __instance.subgenre))
                {
                    //num9: この変数は特定のデザインの重点（Designschwerpunkt）がジャンルにおける期待値からどれだけ逸脱しているかを示している。
                    //したがって、designFocusDeviation という名前が適切かもしれない。
                    float designFocusDeviation = (float)(__instance.genres_.GetFocus(j, __instance.maingenre, __instance.subgenre) - __instance.Designschwerpunkt[j]);
                    switch (__instance.mS_.difficulty)
                    {
                        case 0:
                            gameplayScore -= designFocusDeviation * 0.3f;
                            finalReviewScore -= designFocusDeviation * 0.2f;
                            break;
                        case 1:
                            gameplayScore -= designFocusDeviation * 0.4f;
                            finalReviewScore -= designFocusDeviation * 0.3f;
                            break;
                        case 2:
                            gameplayScore -= designFocusDeviation * 0.5f;
                            finalReviewScore -= designFocusDeviation * 0.4f;
                            break;
                        case 3:
                            gameplayScore -= designFocusDeviation * 0.8f;
                            finalReviewScore -= designFocusDeviation * 0.5f;
                            break;
                        case 4:
                            gameplayScore -= designFocusDeviation * 0.9f;
                            finalReviewScore -= designFocusDeviation * 0.6f;
                            break;
                        case 5:
                            gameplayScore -= designFocusDeviation * 1f;
                            finalReviewScore -= designFocusDeviation * 0.7f;
                            break;
                    }
                }
            }
            for (int k = 0; k < __instance.Designausrichtung.Length; k++)
            {
                //num10: この変数は特定のデザインの方向性（Designausrichtung）がジャンルにおける期待値からどれだけ逸脱しているかを示している。
                //designAlignmentDeviation という名前が適切だ。
                float designAlignmentDeviation = (float)Mathf.Abs(__instance.Designausrichtung[k] - __instance.genres_.GetAlign(k, __instance.maingenre, __instance.subgenre));
                switch (__instance.mS_.difficulty)
                {
                    case 0:
                        gameplayScore -= designAlignmentDeviation * 0.3f;
                        finalReviewScore -= designAlignmentDeviation * 0.2f;
                        break;
                    case 1:
                        gameplayScore -= designAlignmentDeviation * 0.4f;
                        finalReviewScore -= designAlignmentDeviation * 0.3f;
                        break;
                    case 2:
                        gameplayScore -= designAlignmentDeviation * 0.5f;
                        finalReviewScore -= designAlignmentDeviation * 0.4f;
                        break;
                    case 3:
                        gameplayScore -= designAlignmentDeviation * 0.8f;
                        finalReviewScore -= designAlignmentDeviation * 0.5f;
                        break;
                    case 4:
                        gameplayScore -= designAlignmentDeviation * 0.9f;
                        finalReviewScore -= designAlignmentDeviation * 0.6f;
                        break;
                    case 5:
                        gameplayScore -= designAlignmentDeviation * 1f;
                        finalReviewScore -= designAlignmentDeviation * 0.7f;
                        break;
                }
            }
            if (__instance.developerID == __instance.mS_.myID)
            {
                float num10 = (5f - (float)__instance.genres_.genres_LEVEL[__instance.maingenre]) * 0.4f;
                gameplayScore -= num10 * 2f;
                finalReviewScore -= num10 * 1.5f;
            }
            if (__instance.developerID == __instance.mS_.myID)
            {
                if (__instance.subgenre >= 0)
                {
                    float num10 = (5f - (float)__instance.genres_.genres_LEVEL[__instance.subgenre]) * 0.2f;
                    gameplayScore -= num10 * 2f;
                    finalReviewScore -= num10 * 1.5f;
                }
                else
                {
                    gameplayScore -= 2f;
                    finalReviewScore -= 1.5f;
                }
            }
            if (__instance.developerID == __instance.mS_.myID)
            {
                float num10 = (5f - (float)__instance.themes_.themes_LEVEL[__instance.gameMainTheme]) * 0.4f;
                gameplayScore -= num10 * 2f;
                finalReviewScore -= num10 * 1.5f;
            }
            if (__instance.developerID == __instance.mS_.myID)
            {
                if (__instance.gameSubTheme >= 0)
                {
                    float num10 = (5f - (float)__instance.themes_.themes_LEVEL[__instance.gameSubTheme]) * 0.2f;
                    gameplayScore -= num10 * 2f;
                    finalReviewScore -= num10 * 1.5f;
                }
                else
                {
                    gameplayScore -= 2f;
                    finalReviewScore -= 1.5f;
                }
            }
            if (__instance.developerID == __instance.mS_.myID)
            {
                if (__instance.typ_addon || __instance.typ_addonStandalone)
                {
                    //num11: この変数はアドオンの品質に応じたスコアの減少を保持しているようだ。
                    float addonQualityAdjustment = 0.4f;
                    addonQualityAdjustment -= __instance.addonQuality;
                    gameplayScore -= gameplayScore * addonQualityAdjustment;
                    finalReviewScore -= finalReviewScore * addonQualityAdjustment;
                }
                if (__instance.typ_mmoaddon)
                {
                    //num12: この変数はMMOアドオンの品質に応じたスコアの減少を保持しているようだ。
                    float mmoAddonQualityAdjustment = 0.4f;
                    mmoAddonQualityAdjustment -= __instance.addonQuality;
                    gameplayScore -= gameplayScore * mmoAddonQualityAdjustment;
                    finalReviewScore -= finalReviewScore * mmoAddonQualityAdjustment;
                }
            }
            if (__instance.developerID == __instance.mS_.myID && __instance.finanzierung_Grundkosten < 100)
            {
                //num13: この変数は、開発費に応じたスコアの減少を保持しているようだ。
                float financingBaseCostAdjustment = (float)__instance.finanzierung_Grundkosten;
                financingBaseCostAdjustment *= 0.01f;

                //num14: この変数は、開発費に応じたスコアの減少を保持しているようだ。
                float anotherAdjustment = finalReviewScore - finalReviewScore * financingBaseCostAdjustment;
                finalReviewScore -= anotherAdjustment * 0.5f;
                anotherAdjustment = finalReviewScore - gameplayScore * financingBaseCostAdjustment;
                gameplayScore -= anotherAdjustment * 0.5f;
                anotherAdjustment = graphicsScore - graphicsScore * financingBaseCostAdjustment;
                graphicsScore -= anotherAdjustment * 0.2f;
                anotherAdjustment = soundScore - soundScore * financingBaseCostAdjustment;
                soundScore -= anotherAdjustment * 0.3f;
                anotherAdjustment = technicalScore - technicalScore * financingBaseCostAdjustment;
                technicalScore -= anotherAdjustment * 0.2f;
            }
            if (!entwicklungsbericht && __instance.specialMarketing[1] != 0)
            {
                gameplayScore += (float)__instance.specialMarketing[1];
                graphicsScore += (float)__instance.specialMarketing[1];
                soundScore += (float)__instance.specialMarketing[1];
                technicalScore += (float)__instance.specialMarketing[1];
                finalReviewScore += (float)__instance.specialMarketing[1];
            }
            if (__instance.developerID == __instance.mS_.myID && __instance.mS_.GetFanGenreID() == __instance.maingenre)
            {
                finalReviewScore += 3f;
            }
            if (__instance.developerID == __instance.mS_.myID)
            {
                switch (__instance.mS_.difficulty)
                {
                    case 0:
                        gameplayScore += 4f;
                        graphicsScore += 4f;
                        soundScore += 4f;
                        technicalScore += 4f;
                        finalReviewScore += 4f;
                        break;
                    case 1:
                        gameplayScore += 3f;
                        graphicsScore += 3f;
                        soundScore += 3f;
                        technicalScore += 3f;
                        finalReviewScore += 3f;
                        break;
                    case 2:
                        gameplayScore += 2f;
                        graphicsScore += 2f;
                        soundScore += 2f;
                        technicalScore += 2f;
                        finalReviewScore += 2f;
                        break;
                    case 3:
                        gameplayScore += 1f;
                        graphicsScore += 1f;
                        soundScore += 1f;
                        technicalScore += 1f;
                        finalReviewScore += 1f;
                        break;
                    case 4:
                        gameplayScore += 0.5f;
                        graphicsScore += 0.5f;
                        soundScore += 0.5f;
                        technicalScore += 0.5f;
                        finalReviewScore += 0.5f;
                        break;
                    case 5:
                        gameplayScore += 0f;
                        graphicsScore += 0f;
                        soundScore += 0f;
                        technicalScore += 0f;
                        finalReviewScore += 0f;
                        break;
                }
            }
            if (!entwicklungsbericht && __instance.mS_.settings_RandomReviews)
            {
                gameplayScore += (float)UnityEngine.Random.Range(-(__instance.mS_.settings_RandomReviewsNum * 3), __instance.mS_.settings_RandomReviewsNum * 3);
                graphicsScore += (float)UnityEngine.Random.Range(-(__instance.mS_.settings_RandomReviewsNum * 3), __instance.mS_.settings_RandomReviewsNum * 3);
                soundScore += (float)UnityEngine.Random.Range(-(__instance.mS_.settings_RandomReviewsNum * 3), __instance.mS_.settings_RandomReviewsNum * 3);
                technicalScore += (float)UnityEngine.Random.Range(-(__instance.mS_.settings_RandomReviewsNum * 3), __instance.mS_.settings_RandomReviewsNum * 3);
                finalReviewScore += (float)UnityEngine.Random.Range(-(__instance.mS_.settings_RandomReviewsNum * 3), __instance.mS_.settings_RandomReviewsNum * 3);
            }
            if (__instance.developerID != __instance.mS_.myID)
            {
                if (!__instance.devS_)
                {
                    __instance.FindMyDeveloper();
                }
                if (__instance.devS_ && !__instance.devS_.IsTochterfirma())
                {
                    switch (__instance.mS_.difficulty)
                    {
                        case 0:
                            if (!entwicklungsbericht)
                            {
                                graphicsScore -= (float)UnityEngine.Random.Range(5, 10);
                                soundScore -= (float)UnityEngine.Random.Range(5, 10);
                                technicalScore -= (float)UnityEngine.Random.Range(5, 10);
                                finalReviewScore -= (float)UnityEngine.Random.Range(5, 10);
                            }
                            else
                            {
                                graphicsScore -= 7.5f;
                                soundScore -= 7.5f;
                                technicalScore -= 7.5f;
                                finalReviewScore -= 7.5f;
                            }
                            break;
                        case 1:
                            if (!entwicklungsbericht)
                            {
                                graphicsScore -= (float)UnityEngine.Random.Range(0, 5);
                                soundScore -= (float)UnityEngine.Random.Range(0, 5);
                                technicalScore -= (float)UnityEngine.Random.Range(0, 5);
                                finalReviewScore -= (float)UnityEngine.Random.Range(0, 5);
                            }
                            else
                            {
                                graphicsScore -= 2.5f;
                                soundScore -= 2.5f;
                                technicalScore -= 2.5f;
                                finalReviewScore -= 2.5f;
                            }
                            break;
                        case 2:
                            if (!entwicklungsbericht)
                            {
                                graphicsScore -= (float)UnityEngine.Random.Range(0, 3);
                                soundScore -= (float)UnityEngine.Random.Range(0, 3);
                                technicalScore -= (float)UnityEngine.Random.Range(0, 3);
                                finalReviewScore -= (float)UnityEngine.Random.Range(0, 3);
                            }
                            else
                            {
                                graphicsScore -= 1.5f;
                                soundScore -= 1.5f;
                                technicalScore -= 1.5f;
                                finalReviewScore -= 1.5f;
                            }
                            break;
                        case 3:
                            if (!entwicklungsbericht)
                            {
                                gameplayScore += (float)UnityEngine.Random.Range(0, 8);
                                graphicsScore += (float)UnityEngine.Random.Range(0, 2);
                                soundScore += (float)UnityEngine.Random.Range(0, 2);
                                technicalScore += (float)UnityEngine.Random.Range(0, 2);
                                finalReviewScore += (float)UnityEngine.Random.Range(0, 4);
                            }
                            else
                            {
                                gameplayScore += 4f;
                                graphicsScore += 1f;
                                soundScore += 1f;
                                technicalScore += 1f;
                                finalReviewScore += 2f;
                            }
                            break;
                        case 4:
                            if (!entwicklungsbericht)
                            {
                                gameplayScore += (float)UnityEngine.Random.Range(0, 12);
                                graphicsScore += (float)UnityEngine.Random.Range(0, 4);
                                soundScore += (float)UnityEngine.Random.Range(0, 4);
                                technicalScore += (float)UnityEngine.Random.Range(0, 4);
                                finalReviewScore += (float)UnityEngine.Random.Range(0, 8);
                            }
                            else
                            {
                                gameplayScore += 6f;
                                graphicsScore += 2f;
                                soundScore += 2f;
                                technicalScore += 2f;
                                finalReviewScore += 4f;
                            }
                            break;
                        case 5:
                            if (!entwicklungsbericht)
                            {
                                gameplayScore += (float)UnityEngine.Random.Range(0, 16);
                                graphicsScore += (float)UnityEngine.Random.Range(0, 6);
                                soundScore += (float)UnityEngine.Random.Range(0, 6);
                                technicalScore += (float)UnityEngine.Random.Range(0, 6);
                                finalReviewScore += (float)UnityEngine.Random.Range(0, 12);
                            }
                            else
                            {
                                gameplayScore += 8f;
                                graphicsScore += 3f;
                                soundScore += 3f;
                                technicalScore += 3f;
                                finalReviewScore += 6f;
                            }
                            break;
                    }
                }
                if (__instance.sonderIP)
                {
                    if (!entwicklungsbericht)
                    {
                        gameplayScore = (float)UnityEngine.Random.Range(__instance.sonderIPMindestreview - 5, __instance.sonderIPMindestreview + 5);
                        graphicsScore = (float)UnityEngine.Random.Range(__instance.sonderIPMindestreview - 5, __instance.sonderIPMindestreview + 5);
                        soundScore = (float)UnityEngine.Random.Range(__instance.sonderIPMindestreview - 5, __instance.sonderIPMindestreview + 5);
                        technicalScore = (float)UnityEngine.Random.Range(__instance.sonderIPMindestreview - 5, __instance.sonderIPMindestreview + 5);
                        finalReviewScore = (float)UnityEngine.Random.Range(__instance.sonderIPMindestreview - 5, __instance.sonderIPMindestreview + 5);
                    }
                    else
                    {
                        gameplayScore = (float)__instance.sonderIPMindestreview;
                        graphicsScore = (float)__instance.sonderIPMindestreview;
                        soundScore = (float)__instance.sonderIPMindestreview;
                        technicalScore = (float)__instance.sonderIPMindestreview;
                        finalReviewScore = (float)__instance.sonderIPMindestreview;
                    }
                    if (!__instance.devS_)
                    {
                        __instance.FindMyDeveloper();
                    }
                    if (__instance.devS_ && __instance.devS_.IsTochterfirma())
                    {
                        switch (__instance.devS_.tf_entwicklungsdauer)
                        {
                            case 0:
                                gameplayScore -= 10f;
                                graphicsScore -= 10f;
                                soundScore -= 10f;
                                technicalScore -= 10f;
                                finalReviewScore -= 10f;
                                break;
                            case 1:
                                gameplayScore -= 5f;
                                graphicsScore -= 5f;
                                soundScore -= 5f;
                                technicalScore -= 5f;
                                finalReviewScore -= 5f;
                                break;
                        }
                    }
                }
                if (!__instance.sonderIP && __instance.mS_.settings_sandbox)
                {
                    gameplayScore *= __instance.mS_.sandbox_npcGameQuality;
                    graphicsScore *= __instance.mS_.sandbox_npcGameQuality;
                    soundScore *= __instance.mS_.sandbox_npcGameQuality;
                    technicalScore *= __instance.mS_.sandbox_npcGameQuality;
                    finalReviewScore *= __instance.mS_.sandbox_npcGameQuality;
                }
            }
            if (!entwicklungsbericht)
            {
                if (finalReviewScore >= 98f)
                {
                    finalReviewScore = 98f;
                    if (UnityEngine.Random.Range(0, 25) == 1)
                    {
                        finalReviewScore = 99f;
                    }
                    if (UnityEngine.Random.Range(0, 50) == 1)
                    {
                        finalReviewScore = 100f;
                    }
                }
                if (gameplayScore >= 98f)
                {
                    gameplayScore = 98f;
                    if (UnityEngine.Random.Range(0, 10) == 1)
                    {
                        gameplayScore = 99f;
                    }
                    if (UnityEngine.Random.Range(0, 25) == 1)
                    {
                        gameplayScore = 100f;
                    }
                }
                if (graphicsScore >= 98f)
                {
                    graphicsScore = 98f;
                    if (UnityEngine.Random.Range(0, 10) == 1)
                    {
                        graphicsScore = 99f;
                    }
                    if (UnityEngine.Random.Range(0, 25) == 1)
                    {
                        graphicsScore = 100f;
                    }
                }
                if (soundScore >= 98f)
                {
                    soundScore = 98f;
                    if (UnityEngine.Random.Range(0, 10) == 1)
                    {
                        soundScore = 99f;
                    }
                    if (UnityEngine.Random.Range(0, 25) == 1)
                    {
                        soundScore = 100f;
                    }
                }
                if (technicalScore >= 98f)
                {
                    technicalScore = 98f;
                    if (UnityEngine.Random.Range(0, 10) == 1)
                    {
                        technicalScore = 99f;
                    }
                    if (UnityEngine.Random.Range(0, 25) == 1)
                    {
                        technicalScore = 100f;
                    }
                }
            }

            try
            {
                float scoreMultiplier = 1f; // 基本の乗数は1（変更なし）
                ICustomMultiplier customMultiplier = GameReviewCalcMultiplier.Instance;

                float gameplayScoreMultiplier = scoreMultiplier * gameplayScore;
                float gameplayScoreAdjusted = customMultiplier.ApplyMultiplier(gameplayScoreMultiplier);

                float graphicsScoreMultiplier = scoreMultiplier * graphicsScore;
                float graphicsScoreAdjusted = customMultiplier.ApplyMultiplier(graphicsScoreMultiplier);

                float soundScoreMultiplier = scoreMultiplier * soundScore;
                float soundScoreAdjusted = customMultiplier.ApplyMultiplier(soundScoreMultiplier);

                float technicalScoreMultiplier = scoreMultiplier * technicalScore;
                float technicalScoreAdjusted = customMultiplier.ApplyMultiplier(technicalScoreMultiplier);

                float finalReviewScoreMultiplier = scoreMultiplier * finalReviewScore;
                float finalReviewScoreAdjusted = customMultiplier.ApplyMultiplier(finalReviewScoreMultiplier);

                __instance.reviewGameplay = Mathf.RoundToInt(gameplayScoreAdjusted);
                __instance.reviewGrafik = Mathf.RoundToInt(graphicsScoreAdjusted);
                __instance.reviewSound = Mathf.RoundToInt(soundScoreAdjusted);
                __instance.reviewSteuerung = Mathf.RoundToInt(technicalScoreAdjusted);
                __instance.reviewTotal = Mathf.RoundToInt(finalReviewScoreAdjusted);
            }
            catch (Exception ex)
            {
                Debug.LogError("An error occurred on CalcReviewHooks : " + ex.Message);
            }

            if (__instance.reviewGameplay < 1)
            {
                __instance.reviewGameplay = 1;
            }
            if (__instance.reviewGrafik < 1)
            {
                __instance.reviewGrafik = 1;
            }
            if (__instance.reviewSound < 1)
            {
                __instance.reviewSound = 1;
            }
            if (__instance.reviewSteuerung < 1)
            {
                __instance.reviewSteuerung = 1;
            }
            if (__instance.reviewTotal < 1)
            {
                __instance.reviewTotal = 1;
            }
            if (__instance.reviewGameplay > 100)
            {
                __instance.reviewGameplay = 100;
            }
            if (__instance.reviewGrafik > 100)
            {
                __instance.reviewGrafik = 100;
            }
            if (__instance.reviewSound > 100)
            {
                __instance.reviewSound = 100;
            }
            if (__instance.reviewSteuerung > 100)
            {
                __instance.reviewSteuerung = 100;
            }
            if (__instance.reviewTotal > 100)
            {
                __instance.reviewTotal = 100;
            }
            if (__instance.developerID != __instance.mS_.myID)
            {
                if (!entwicklungsbericht)
                {
                    if (__instance.reviewGameplay <= 1)
                    {
                        __instance.reviewGameplay = UnityEngine.Random.Range(2, 10);
                    }
                    if (__instance.reviewGrafik <= 1)
                    {
                        __instance.reviewGrafik = UnityEngine.Random.Range(2, 10);
                    }
                    if (__instance.reviewSound <= 1)
                    {
                        __instance.reviewSound = UnityEngine.Random.Range(2, 10);
                    }
                    if (__instance.reviewSteuerung <= 1)
                    {
                        __instance.reviewSteuerung = UnityEngine.Random.Range(2, 10);
                    }
                    if (__instance.reviewTotal <= 1)
                    {
                        __instance.reviewTotal = UnityEngine.Random.Range(2, 10);
                    }
                }
                else
                {
                    if (__instance.reviewGameplay <= 1)
                    {
                        __instance.reviewGameplay = 5;
                    }
                    if (__instance.reviewGrafik <= 1)
                    {
                        __instance.reviewGrafik = 5;
                    }
                    if (__instance.reviewSound <= 1)
                    {
                        __instance.reviewSound = 5;
                    }
                    if (__instance.reviewSteuerung <= 1)
                    {
                        __instance.reviewSteuerung = 5;
                    }
                    if (__instance.reviewTotal <= 1)
                    {
                        __instance.reviewTotal = 5;
                    }
                }
            }
            if (!entwicklungsbericht && !__instance.typ_addon && !__instance.typ_addonStandalone && !__instance.typ_budget && !__instance.typ_bundle && !__instance.typ_mmoaddon && !__instance.typ_goty && !__instance.typ_bundleAddon)
            {
                if (!__instance.devS_)
                {
                    __instance.FindMyDeveloper();
                }
                if (__instance.reviewTotal >= 80)
                {
                    __instance.mS_.AddAwards(8, __instance.devS_);
                }
                if (__instance.reviewTotal < 30)
                {
                    __instance.mS_.AddAwards(9, __instance.devS_);
                }
            }

            return false;
        }
    }
}

/*
    コードをざっと見た感じ、以下のことが行われているようだ：

    既にレビューが存在するかのチェック：もし __instance.reviewTotal > 0 なら、このメソッドは何もせずに終了する。
    現在の日付の設定：__instance.date_month と __instance.date_year を現在の月と年に設定している。
    レビュー基準値の計算：ゲームがレトロかどうか、開発者のIDがプレイヤーのものかどうか、難易度設定に基づいて、レビューの基準値（num）を計算している。
    各カテゴリーのスコア計算：ゲームプレイ、グラフィック、サウンド、技術の各カテゴリーに対してスコア（num2, num3, num4, num5）を計算している。
    これらの計算には、ゲームの特徴や年代、プラットフォームの経験値など様々な要素が考慮されている。
    最終スコアの計算：最後に、上記の各カテゴリーのスコアを加算して、ゲームのトータルスコア（num6）を計算している。

    問題点や改善点を挙げるとすれば：

    - コードの複雑さ：このメソッドは非常に長く、多くの分岐がある。コードの可読性やメンテナンス性を向上させるために、いくつかの部分を別のメソッドに分割することを検討した方がいいかもしれない。
    - マジックナンバーの使用：コード内に直接数値（例：7000f、18000f など）が多用されている。
    - これらを意味のある名前の定数や設定値に置き換えることで、コードの理解と変更がしやすくなる。
    - ランダム性の使用：UnityEngine.Random.Range を使っている部分がいくつかあるが、これによりスコア計算に一貫性がなくなり、デバッグが難しくなる可能性がある。
    
    ゲームデザイン上このランダム性が意図的であれば問題ないが、そうでなければ再検討した方がいい。
    全体として、このコードはゲームのレビュースコアを計算する複雑なロジックを含んでおり、様々な条件や要素を考慮しているようだ。
    ただ、その複雑さゆえに、バグが発生しやすくなっている可能性があるぞ。

    頑張ってリファクタリングしてみるんだな。
*/
