using HarmonyLib;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using MGT2API.Difficulty.Adjustments;


namespace MGT2API.Difficulty.Hooks
{
    /// <summary>
    /// 
    /// [JP] ゲーム販売に関するカスタム乗数をパッチします。
    /// また、ここでは、非常に膨大な量のコードが含まれているため、将来的にHarmonyPrefixでの完全書き換えによるリファクタリングが必要です。
    ///　現在は、最低限の機能を実装するために、HarmonyPatch用に調整し、一部のコードを追加しています。
    /// 今後、機能の拡充のためには、このクラスを分割する必要があります。
    /// </summary>
    [HarmonyPatch]
    internal static class SellAllGamesHooks
    {
        [HarmonyPatch(typeof(games), "SellAllGames")]
        internal static bool Prefix(ref games __instance)
        {
            Debug.Log("================================================");
            Debug.Log("SellAllGamesHooks.SellAllGames.Prefix");
            Debug.Log("================================================");

            SellAllGamesManager.RefactoredSellAllGames(__instance);
            return false;
        }

        private static class SellAllGamesManager
        {
            private static games instance;
            private static publishingOfferMain poM_;
            private static themes themes_;
            private static genres genres_;
            private static gamepassScript gpS_;
            private static mainScript mS_;
            private static GUI_Main guiMain_;
            private static textScript tS_;

            private static void Init(games instance)
            {
                SellAllGamesManager.instance = instance;
                poM_ = Traverse.Create(instance).Field("poM_").GetValue<publishingOfferMain>();
                themes_ = Traverse.Create(instance).Field("themes_").GetValue<themes>();
                genres_ = Traverse.Create(instance).Field("genres_").GetValue<genres>();
                gpS_ = Traverse.Create(instance).Field("gpS_").GetValue<gamepassScript>();
                mS_ = Traverse.Create(instance).Field("mS_").GetValue<mainScript>();
                guiMain_ = Traverse.Create(instance).Field("guiMain_").GetValue<GUI_Main>();
                tS_ = Traverse.Create(instance).Field("tS_").GetValue<textScript>();

            }

            internal static void RefactoredSellAllGames(games instance)
            {
                Init(instance);

                int num = 0;
                poM_.amountPublishingOffers = 0;
                instance.gamePassGames.Clear();
                for (int i = 0; i < themes_.themes_MARKT.Length; i++)
                {
                    themes_.themes_MARKT[i] = 0;
                }
                for (int j = 0; j < genres_.genres_MARKT.Length; j++)
                {
                    genres_.genres_MARKT[j] = 0;
                }
                gpS_.gamePass_AbosLetzteWoche = 0L;
                instance.freeServerPlatz = 0L;
                for (int k = 0; k < mS_.arrayRoomScripts.Length; k++)
                {
                    if (mS_.arrayRoomScripts[k] && mS_.arrayRoomScripts[k].typ == 15)
                    {
                        instance.freeServerPlatz += mS_.arrayRoomScripts[k].GetFreeServerplatz();
                        mS_.arrayRoomScripts[k].serverplatzUsed = 0L;
                    }
                }
                for (int l = 0; l < instance.arrayGamesScripts.Length; l++)
                {
                    if (instance.arrayGamesScripts[l])
                    {
                        if (instance.arrayGamesScripts[l].ownerID == mS_.myID && instance.arrayGamesScripts[l].ipToSell && UnityEngine.Random.Range(0, 100) < 10 && !guiMain_.uiObjects[437].activeSelf)
                        {
                            guiMain_.OpenMenu(false);
                            guiMain_.ActivateMenu(guiMain_.uiObjects[437]);
                            guiMain_.uiObjects[437].GetComponent<Menu_Result_IpVerkauf>().Init(instance.arrayGamesScripts[l]);
                        }
                        if (instance.arrayGamesScripts[l].inGamePass)
                        {
                            if (!instance.arrayGamesScripts[l].CanBeInGamePass())
                            {
                                gpS_.GAMEPASS_RemoveGame(instance.arrayGamesScripts[l], false);
                            }
                            else
                            {
                                instance.gamePassGames.Add(instance.arrayGamesScripts[l]);
                            }
                        }
                        if (instance.arrayGamesScripts[l].pubAngebot)
                        {
                            instance.arrayGamesScripts[l].pubAngebot_AngebotWoche = false;
                            if ((mS_.multiplayer && mS_.mpCalls_.isServer) || !mS_.multiplayer)
                            {
                                instance.arrayGamesScripts[l].pubAngebot_Weeks++;
                                if (instance.arrayGamesScripts[l].pubAngebot_Weeks > 25 && UnityEngine.Random.Range(0, 100) > 90)
                                {
                                    if (instance.arrayGamesScripts[l].reviewTotal <= 0)
                                    {
                                        instance.arrayGamesScripts[l].CalcReview(false);
                                        if (mS_)
                                        {
                                            mS_.reviewText_.GetReviewText(instance.arrayGamesScripts[l]);
                                        }
                                    }
                                    else
                                    {
                                        instance.arrayGamesScripts[l].date_year = mS_.year;
                                        instance.arrayGamesScripts[l].date_month = mS_.month;
                                        if (mS_)
                                        {
                                            mS_.reviewText_.GetReviewText(instance.arrayGamesScripts[l]);
                                        }
                                    }
                                    instance.arrayGamesScripts[l].FindPublisherForGame();
                                    instance.arrayGamesScripts[l].SetOnMarket();
                                    if (mS_.newsSetting[0])
                                    {
                                        string text = tS_.GetText(494);
                                        text = text.Replace("<NAME1>", instance.arrayGamesScripts[l].GetPublisherName());
                                        text = text.Replace("<NAME2>", instance.arrayGamesScripts[l].GetNameWithTag());
                                        guiMain_.CreateTopNewsInfo(text);
                                    }
                                }
                            }
                            if (instance.arrayGamesScripts[l].pubAngebot && !instance.arrayGamesScripts[l].pubAnbgebot_Inivs)
                            {
                                poM_.amountPublishingOffers++;
                            }
                        }
                        if (instance.arrayGamesScripts[l].developerID == mS_.myID && instance.arrayGamesScripts[l].inDevelopment && instance.arrayGamesScripts[l].typ_contractGame && !instance.arrayGamesScripts[l].auftragsspiel_zeitAbgelaufen)
                        {
                            instance.arrayGamesScripts[l].auftragsspiel_zeitInWochen--;
                            if (instance.arrayGamesScripts[l].auftragsspiel_zeitInWochen < 0)
                            {
                                instance.arrayGamesScripts[l].auftragsspiel_zeitInWochen = 0;
                                instance.arrayGamesScripts[l].auftragsspiel_zeitAbgelaufen = true;
                            }
                        }
                        if (((mS_.multiplayer && mS_.mpCalls_.isServer) || !mS_.multiplayer) && instance.arrayGamesScripts[l].auftragsspiel)
                        {
                            bool flag = false;
                            if (!instance.arrayGamesScripts[l].retro)
                            {
                                if (!instance.arrayGamesScripts[l].gamePlatformScript[0])
                                {
                                    instance.arrayGamesScripts[l].FindMyPlatforms();
                                }
                                if (instance.arrayGamesScripts[l].gamePlatformScript[0].vomMarktGenommen)
                                {
                                    flag = true;
                                }
                            }
                            instance.arrayGamesScripts[l].auftragsspiel_wochenAlsAngebot++;
                            if ((instance.arrayGamesScripts[l].auftragsspiel_wochenAlsAngebot > 25 || flag) && (UnityEngine.Random.Range(0, 100) > 90 || flag))
                            {
                                instance.arrayGamesScripts[l].developerID = instance.arrayGamesScripts[l].publisherID;
                                instance.arrayGamesScripts[l].FindMyDeveloper();
                                instance.arrayGamesScripts[l].FindMyPublisher();
                                if (instance.arrayGamesScripts[l].pS_)
                                {
                                    instance.arrayGamesScripts[l].pS_.ResetDataForAuftragsspiel(instance.arrayGamesScripts[l]);
                                }
                                if (instance.arrayGamesScripts[l].reviewTotal <= 0)
                                {
                                    instance.arrayGamesScripts[l].CalcReview(false);
                                    if (mS_)
                                    {
                                        mS_.reviewText_.GetReviewText(instance.arrayGamesScripts[l]);
                                    }
                                }
                                else
                                {
                                    instance.arrayGamesScripts[l].date_year = mS_.year;
                                    instance.arrayGamesScripts[l].date_month = mS_.month;
                                    if (mS_)
                                    {
                                        mS_.reviewText_.GetReviewText(instance.arrayGamesScripts[l]);
                                    }
                                }
                                instance.arrayGamesScripts[l].date_year = mS_.year;
                                instance.arrayGamesScripts[l].date_month = mS_.month;
                                instance.arrayGamesScripts[l].SetOnMarket();
                                if (mS_.newsSetting[0])
                                {
                                    string text2 = tS_.GetText(494);
                                    text2 = text2.Replace("<NAME1>", instance.arrayGamesScripts[l].GetPublisherName());
                                    text2 = text2.Replace("<NAME2>", instance.arrayGamesScripts[l].GetNameWithTag());
                                    guiMain_.CreateTopNewsInfo(text2);
                                }
                            }
                        }
                        if (num < 10 && ((mS_.multiplayer && mS_.mpCalls_.isServer) || !mS_.multiplayer) && instance.arrayGamesScripts[l].mainIP == instance.arrayGamesScripts[l].myID && instance.arrayGamesScripts[l].OwnerIsNPC())
                        {
                            if (!instance.arrayGamesScripts[l].ownerS_)
                            {
                                instance.arrayGamesScripts[l].FindMyOwner();
                            }
                            if (instance.arrayGamesScripts[l].ownerS_ && !instance.arrayGamesScripts[l].ownerS_.IsTochterfirma())
                            {
                                if (!instance.arrayGamesScripts[l].ipToSell)
                                {
                                    if (!instance.arrayGamesScripts[l].sonderIP && instance.arrayGamesScripts[l].ipPunkte > 1f && UnityEngine.Random.Range(0, 100) > 95 && (float)UnityEngine.Random.Range(0, 500) > UnityEngine.Random.Range(0f, instance.arrayGamesScripts[l].ipPunkte) && instance.IsIpFree(instance.arrayGamesScripts[l], false))
                                    {
                                        instance.arrayGamesScripts[l].ipToSell = true;
                                        num++;
                                        if (mS_.multiplayer && mS_.mpCalls_.isServer)
                                        {
                                            mS_.mpCalls_.SERVER_Send_GameIpSell(instance.arrayGamesScripts[l]);
                                        }
                                    }
                                }
                                else
                                {
                                    num++;
                                    if (UnityEngine.Random.Range(0, 100) > 95)
                                    {
                                        instance.arrayGamesScripts[l].ipToSell = false;
                                        num--;
                                        if (mS_.multiplayer && mS_.mpCalls_.isServer)
                                        {
                                            mS_.mpCalls_.SERVER_Send_GameIpSell(instance.arrayGamesScripts[l]);
                                        }
                                    }
                                }
                            }
                        }
                        if (instance.arrayGamesScripts[l].ownerID == mS_.myID && !instance.arrayGamesScripts[l].typ_contractGame && !instance.arrayGamesScripts[l].auftragsspiel && !instance.arrayGamesScripts[l].pubAngebot && instance.arrayGamesScripts[l].mainIP == instance.arrayGamesScripts[l].myID)
                        {
                            //なにこれ？ipTimeが250を超えたら、なんかしらんけど、IPポイントを減らす処理をするっぽい。
                            //ちょこっとdnSpyで調べた感じ、250回数分超えたら、AddIpPointsメソッドを呼び出し。
                            //AddIpPointsメソッドは、if (this.mainIP != -1)の時に、.ipTimeを0にする処理を行う。
                            instance.arrayGamesScripts[l].ipTime++;
                            if (instance.arrayGamesScripts[l].ipTime > 250)
                            {
                                // ----------- Difficulty API 加筆修正コード -------------------------------------------------------------------
                                float[] difficultyMultipliers = new float[] { -20f, -40f, -50f, -80f, -90f, -100f };
                                float baseMultiplier;

                                // 難易度が配列の範囲外の場合、デフォルト値を使用
                                if (mS_.difficulty < 0 || mS_.difficulty >= difficultyMultipliers.Length)
                                {
                                    baseMultiplier = -80f; // ここでデフォルト値を設定
                                }
                                else
                                {
                                    baseMultiplier = difficultyMultipliers[mS_.difficulty];
                                }

                                float customMultiplier = IpPointsAdditionOnSellPerWeekMultiplier.Instance.ApplyCustomMultiplier(baseMultiplier);
                                instance.arrayGamesScripts[l].AddIpPoints(customMultiplier);
                            }

                                // !----------- Difficulty API 加筆修正コード -------------------------------------------------------------------
                            }
                            if (mS_.week == 5)
                            {
                                instance.arrayGamesScripts[l].merchGewinnLetzterMonat = instance.arrayGamesScripts[l].merchGewinnDiesenMonat;
                                instance.arrayGamesScripts[l].merchGewinnDiesenMonat = 0L;
                                for (int m = 0; m < instance.arrayGamesScripts[l].merchLetzterMonat.Length; m++)
                                {
                                    instance.arrayGamesScripts[l].merchLetzterMonat[m] = instance.arrayGamesScripts[l].merchDiesenMonat[m];
                                    instance.arrayGamesScripts[l].merchDiesenMonat[m] = 0;
                                }
                            }
                            if (instance.arrayGamesScripts[l].merchGesamtReviewPoints > 0f)
                            {
                                instance.arrayGamesScripts[l].merchGesamtReviewPoints -= 10f;
                                if (instance.arrayGamesScripts[l].merchGesamtReviewPoints < 0f)
                                {
                                    instance.arrayGamesScripts[l].merchGesamtReviewPoints = 0f;
                                }
                            }
                        }
                        if (instance.arrayGamesScripts[l].ownerID == mS_.myID || instance.arrayGamesScripts[l].publisherID == mS_.myID || instance.arrayGamesScripts[l].developerID == mS_.myID)
                        {
                            if (instance.arrayGamesScripts[l].inDevelopment)
                            {
                                instance.arrayGamesScripts[l].weeksInDevelopment++;
                                if (instance.arrayGamesScripts[l].devFortsetzen > 1)
                                {
                                    instance.arrayGamesScripts[l].devFortsetzen--;
                                }
                            }
                            if (((instance.arrayGamesScripts[l].GetProzentGesamt() > 99f && instance.arrayGamesScripts[l].GetHype() > 0f) || instance.arrayGamesScripts[l].schublade) && UnityEngine.Random.Range(0, 100) < 10)
                            {
                                instance.arrayGamesScripts[l].AddHype(-1f);
                            }
                        }
                        int specialMarketingType = 0; // この値を動的に変更できるように変数に格納する
                        IEnumerator iWaitForSpecialMarketing = Traverse.Create(instance).Method("iWaitForSpecialMarketing", new object[] { instance.arrayGamesScripts[l], specialMarketingType }).GetValue<IEnumerator>();
                        if ((instance.arrayGamesScripts[l].ownerID == mS_.myID || instance.arrayGamesScripts[l].publisherID == mS_.myID) && (instance.arrayGamesScripts[l].isOnMarket || instance.arrayGamesScripts[l].inDevelopment || instance.arrayGamesScripts[l].schublade))
                        {
                            if (instance.arrayGamesScripts[l].specialMarketing[0] == 1)
                            {
                                specialMarketingType = 0;
                                instance.StartCoroutine(iWaitForSpecialMarketing);
                            }
                            if (instance.arrayGamesScripts[l].specialMarketing[1] == 1)
                            {
                                specialMarketingType = 1;
                                instance.StartCoroutine(iWaitForSpecialMarketing);
                            }
                            if (instance.arrayGamesScripts[l].specialMarketing[2] == 1)
                            {
                                specialMarketingType = 2;
                                instance.StartCoroutine(iWaitForSpecialMarketing);
                            }
                            if (instance.arrayGamesScripts[l].specialMarketing[3] == 1)
                            {
                                specialMarketingType = 3;
                                instance.StartCoroutine(iWaitForSpecialMarketing);
                            }
                            if (instance.arrayGamesScripts[l].specialMarketing[4] == 1)
                            {
                                specialMarketingType = 4;
                                instance.StartCoroutine(iWaitForSpecialMarketing);
                            }
                            if (instance.arrayGamesScripts[l].specialMarketing[2] == -1 && instance.arrayGamesScripts[l].hype > 100f && instance.arrayGamesScripts[l].isOnMarket && instance.arrayGamesScripts[l].reviewTotal > 0 && instance.arrayGamesScripts[l].reviewTotal < 90 && instance.arrayGamesScripts[l].weeksOnMarket > 0 && !guiMain_.uiObjects[296].activeSelf)
                            {
                                specialMarketingType = 100;
                                instance.StartCoroutine(iWaitForSpecialMarketing);
                            }
                        }
                        if (instance.arrayGamesScripts[l].isOnMarket && !instance.arrayGamesScripts[l].inDevelopment && !instance.arrayGamesScripts[l].typ_addon && !instance.arrayGamesScripts[l].typ_mmoaddon && !instance.arrayGamesScripts[l].typ_bundle)
                        {
                            themes_.themes_MARKT[instance.arrayGamesScripts[l].gameMainTheme]++;
                            if (instance.arrayGamesScripts[l].gameSubTheme != -1)
                            {
                                themes_.themes_MARKT[instance.arrayGamesScripts[l].gameSubTheme]++;
                            }
                            genres_.genres_MARKT[instance.arrayGamesScripts[l].maingenre]++;
                        }
                        if (mS_.multiplayer)
                        {
                            if (mS_.mpCalls_.isServer && (instance.arrayGamesScripts[l].IsMyGame() || instance.arrayGamesScripts[l].typ_contractGame || (instance.arrayGamesScripts[l].DeveloperIsNPC() && instance.arrayGamesScripts[l].PublisherIsNPC() && instance.arrayGamesScripts[l].OwnerIsNPC())))
                            {
                                instance.arrayGamesScripts[l].SellGame();
                                instance.arrayGamesScripts[l].AutomaticSellLagerbestand();
                            }
                            if (mS_.mpCalls_.isClient && instance.arrayGamesScripts[l].IsMyGame())
                            {
                                instance.arrayGamesScripts[l].SellGame();
                                instance.arrayGamesScripts[l].AutomaticSellLagerbestand();
                            }
                        }
                        else
                        {
                            instance.arrayGamesScripts[l].SellGame();
                            instance.arrayGamesScripts[l].AutomaticSellLagerbestand();
                        }
                        if (instance.arrayGamesScripts[l].ownerID == mS_.myID)
                        {
                            instance.arrayGamesScripts[l].SellMerchandise();
                        }
                    }
                Traverse.Create(instance).Method("UpdateAllGamePassFunctions").GetValue();
                instance.LagerplatzVerteilen();
            }
        }
    }
}