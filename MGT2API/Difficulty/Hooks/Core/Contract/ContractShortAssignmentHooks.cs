using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using UnityEngine;
using MGT2API.Difficulty.Adjustments;

namespace MGT2API.Difficulty.Hooks
{
    /// <summary>
    /// mainScript.difficultyにまつわる数値を変更するためのAPI
    /// this.mS_.difficultyというインスタンス変数として使われることもある。
    /// 経緯 : 難易度にまつわるものは、すべてハードコードされているため、それらを柔軟に変えるために作成された。
    /// </summary>
    /// 

    [HarmonyPatch]
    internal static class ContractShortAssignmentHooks
    {

        /// <summary>
        /// Patch for the UpdateContractWork method in the contractWorkMain class.
        /// [JP] contractWorkMainクラスのUpdateContractWorkメソッドに対するリファクタリング兼カスタムカスタム乗数をパッチします。
        /// この項目では、一部のコードを書き換えて、メソッドをインジェクトしなければならない。
        /// ILコードを書き換えを検討したが、どうにも面倒なため、Prefixで完全書き換えを行う。
        /// もしほかに書き換えが必要な部分があったら、適宜コードのファイル構造を変更すること。
        /// 該当部分 : CommittedWorkの委託料及びペナルティ金額に該当する部分。
        /// なぜか知らないが、Contract部分へのコード適用はされなかった。意味不明。
        /// In this item, you need to rewrite some of the code and inject the method.
        /// I considered rewriting the IL code, but it was too much trouble, so I completely rewrote it with Prefix.
        /// If there are other parts that need to be rewritten, change the file structure of the code as needed.
        /// Relevant part : The commission fee and penalty amount for the CommittedWork.
        /// For some reason, the code was not applied to the Contract part. I don't know why.
        /// </summary>
        /// <returns></returns>
        [HarmonyPrefix, HarmonyPatch(typeof(contractWorkMain), "UpdateContractWork")]
        internal static bool PrePatchIfNeed(ref contractWorkMain __instance, ref bool forceNewContract, ref GUI_Main ___guiMain_,
            ref mainScript ___mS_, ref textScript ___tS_, ref roomDataScript ___rdS_, ref forschungSonstiges ___forschungSonstiges_, ref unlockScript ___unlock_)
        {
            //------------------------------------------
            //Todo : ここで、オリジナルコードを使うか、使わないかの判定処理
            //return true; //オリジナルコードを使う
            //------------------------------------------

            //contractWork.gehalt = Mathf.RoundToInt(contractWork.points * (6f - (float)___mS_.difficulty)) * 13;
            //contractWork.strafe = Mathf.RoundToInt(UnityEngine.Random.Range((float)contractWork.gehalt * 0.1f, (float)contractWork.gehalt * 0.3f));

            ///Debug.Log("================================================");
            ///Debug.Log("contractWorkMain_UpdateContractWork_Patch");
            ///Debug.Log("================================================");

            // 以下、完全に書き換えたコード
            Traverse.Create(__instance).Method("FindScripts").GetValue();
            GameObject[] array = GameObject.FindGameObjectsWithTag("ContractWork");
            if (array.Length != 0)
            {
                for (int i = 0; i < array.Length; i++)
                {
                    if (array[i])
                    {
                        contractWork component = array[i].GetComponent<contractWork>();
                        if (component)
                        {
                            if (component.IsAngenommen())
                            {
                                component.zeitInWochen--;
                                if (component.zeitInWochen < 0)
                                {
                                    ___guiMain_.UpdateAuftragsansehen(-component.GetAuftragsansehen());
                                    ___mS_.Pay((long)Mathf.RoundToInt((float)component.GetStrafe()), 14);
                                    string text = ___tS_.GetText(610);
                                    text = text.Replace("<NAME>", "<b><color=blue>" + component.GetName() + "</color></b>");
                                    text = text.Replace("<NUM>", "<b><color=red>" + ___mS_.GetMoney((long)component.GetStrafe(), true) + "</color></b>");
                                    switch (component.art)
                                    {
                                        case 0:
                                            ___guiMain_.CreateLeftNews(-1, ___guiMain_.uiSprites[12], text, ___rdS_.roomData_SPRITE[1]);
                                            break;
                                        case 1:
                                            ___guiMain_.CreateLeftNews(-1, ___guiMain_.uiSprites[12], text, ___rdS_.roomData_SPRITE[3]);
                                            break;
                                        case 2:
                                            ___guiMain_.CreateLeftNews(-1, ___guiMain_.uiSprites[12], text, ___rdS_.roomData_SPRITE[4]);
                                            break;
                                        case 3:
                                            ___guiMain_.CreateLeftNews(-1, ___guiMain_.uiSprites[12], text, ___rdS_.roomData_SPRITE[5]);
                                            break;
                                        case 4:
                                            ___guiMain_.CreateLeftNews(-1, ___guiMain_.uiSprites[12], text, ___rdS_.roomData_SPRITE[10]);
                                            break;
                                        case 5:
                                            ___guiMain_.CreateLeftNews(-1, ___guiMain_.uiSprites[12], text, ___rdS_.roomData_SPRITE[14]);
                                            break;
                                        case 6:
                                            ___guiMain_.CreateLeftNews(-1, ___guiMain_.uiSprites[12], text, ___rdS_.roomData_SPRITE[17]);
                                            break;
                                        case 7:
                                            ___guiMain_.CreateLeftNews(-1, ___guiMain_.uiSprites[12], text, ___rdS_.roomData_SPRITE[8]);
                                            break;
                                    }
                                    UnityEngine.Object.Destroy(array[i]);
                                }
                            }
                            else
                            {
                                component.wochenAlsAngebot++;
                                if (component.wochenAlsAngebot > 16 && UnityEngine.Random.Range(0, 100) > 90)
                                {
                                    UnityEngine.Object.Destroy(array[i]);
                                }
                            }
                        }
                    }
                }
            }
            if (___mS_.globalEvent != 2 && ((array.Length < 20 && (float)UnityEngine.Random.Range(0, 100) > 80f - ___mS_.auftragsAnsehen * 0.5f) || forceNewContract))
            {
                contractWork contractWork = __instance.CreateContractWork();
                contractWork.myID = UnityEngine.Random.Range(1, 999999999);
                switch (UnityEngine.Random.Range(0, 8))
                {
                    case 0:
                        contractWork.art = 0;
                        break;
                    case 1:
                        if (___forschungSonstiges_.IsErforscht(28))
                        {
                            contractWork.art = 1;
                        }
                        break;
                    case 2:
                        if (___forschungSonstiges_.IsErforscht(31))
                        {
                            contractWork.art = 2;
                        }
                        break;
                    case 3:
                        if (___forschungSonstiges_.IsErforscht(32))
                        {
                            contractWork.art = 3;
                        }
                        break;
                    case 4:
                        if (___unlock_.Get(8))
                        {
                            contractWork.art = 4;
                        }
                        break;
                    case 5:
                        if (___forschungSonstiges_.IsErforscht(33))
                        {
                            contractWork.art = 5;
                        }
                        break;
                    case 6:
                        if (___forschungSonstiges_.IsErforscht(38))
                        {
                            contractWork.art = 6;
                        }
                        break;
                    case 7:
                        if (___forschungSonstiges_.IsErforscht(39))
                        {
                            contractWork.art = 7;
                        }
                        break;
                }
                contractWork.angenommen = false;
                contractWork.wochenAlsAngebot = 0;
                if (contractWork.art != 5 && contractWork.art != 6)
                {
                    contractWork.typ = ___tS_.GetRandomContractNumber(contractWork.art);
                }
                contractWork.points = (float)(20 * UnityEngine.Random.Range(10, 30 + Mathf.RoundToInt(___mS_.auftragsAnsehen * 5f)));
                try
                {
                    SetRewardAndPenalty(contractWork, ___mS_);
                }
                catch (Exception ex)
                {
                    // Handle the exception
                    Debug.LogError("An error occurred on SetRewardAndPenalty : " + ex.Message);
                }
                //contractWork.gehalt = Mathf.RoundToInt(contractWork.points * (6f - (float)___mS_.difficulty)) * 13;
                //contractWork.strafe = Mathf.RoundToInt(UnityEngine.Random.Range((float)contractWork.gehalt * 0.1f, (float)contractWork.gehalt * 0.3f));

                var GetRandomPublisherID = Traverse.Create(__instance).Method("GetRandomPublisherID");
                contractWork.auftraggeberID = GetRandomPublisherID.GetValue<int>();

                if (contractWork.typ == 25 && !___unlock_.Get(31))
                {
                    contractWork.typ = 6;
                }
                switch (contractWork.art)
                {
                    case 1:
                        contractWork.points *= 0.8f;
                        break;
                    case 2:
                        contractWork.points *= 0.6f;
                        break;
                    case 3:
                        contractWork.points *= 0.4f;
                        break;
                    case 4:
                        contractWork.points *= 0.3f;
                        break;
                    case 5:
                        {
                            contractWork.points *= 1000f;
                            int num = Mathf.RoundToInt(contractWork.points) / 100 * 100;
                            contractWork.points = (float)num;
                            break;
                        }
                    case 6:
                        contractWork.points *= 0.8f;
                        break;
                    case 7:
                        contractWork.points *= 0.8f;
                        break;
                }
                contractWork.points *= __instance.pointsRegulator;
                if (contractWork.art != 5)
                {
                    contractWork.zeitInWochen = Mathf.RoundToInt(contractWork.points / 50f + (float)UnityEngine.Random.Range(5, 10));
                }
                else
                {
                    contractWork.zeitInWochen = Mathf.RoundToInt(contractWork.points / 20000f + (float)UnityEngine.Random.Range(5, 10));
                }
                if (contractWork.auftraggeberID != -1)
                {
                    contractWork.Init();
                }
                else
                {
                    contractWork.gameObject.SetActive(false);
                    UnityEngine.Object.Destroy(contractWork.gameObject);
                }
            }
            __instance.UpdateGUI();
            return false;
        }

        // --------------------------------------------------------------------------------

        /// <summary>
        /// Set the reward and penalty for the ShortAssignment of the Contract.
        /// in the future, if you want to change the code, you should change it here.
        /// Currently, I'm just refactoring and writing the original code.
        /// [JP]ContractのShortAssignmentの報酬とペナルティを設定する。
        /// [JP]今後、報酬やペナルティの計算ロジックを調整する場合は、この部分を変更してください。なお、現在はリファクタリングしているだけで、オリジナルコードを記述している。
        /// </summary>
        /// <param name="contractWork"></param>
        /// <param name="mainScript"></param>
        private static void SetRewardAndPenalty(contractWork contractWork, mainScript mainScript)
        {
            float baseDifficultyMultiplier = 6f; //default value is 6f
            int rewardFinalMultiplier = 13;             //default value is 13
            float minPenaltyRate = 0.1f;           //default value is 0.1f
            float maxPenaltyRate = 0.3f;           //default value is 0.3f

            float rewardMultiplier = 1f; // 基本の乗数は1（変更なし）
            ICustomMultiplier customMultiplier = ContractShortAssignmentMultiplier.Instance;
            rewardMultiplier = customMultiplier.ApplyMultiplier(rewardMultiplier);

            float difficultyAdjustment = baseDifficultyMultiplier - (float)mainScript.difficulty;
            contractWork.gehalt = Mathf.RoundToInt(contractWork.points * difficultyAdjustment * rewardMultiplier) * rewardFinalMultiplier;

            float minPenalty = (float)contractWork.gehalt * minPenaltyRate;
            float maxPenalty = (float)contractWork.gehalt * maxPenaltyRate;
            contractWork.strafe = Mathf.RoundToInt(UnityEngine.Random.Range(minPenalty, maxPenalty));
        }
    }
}
