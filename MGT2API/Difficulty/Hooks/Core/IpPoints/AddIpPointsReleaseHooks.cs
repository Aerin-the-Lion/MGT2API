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
    /// <summary>
    /// [JP] ゲームをリリース時のIPポイントを足すカスタム乗数をパッチします。
    /// また、ここでは、非常に膨大な量のコードが含まれているため、将来的にHarmonyPrefixでの完全書き換えによるリファクタリングが必要です。
    ///　現在は、最低限の機能を実装するために、HarmonyPatch用に調整し、一部のコードを追加しています。
    /// 今後、機能の拡充のためには、このクラスを分割する必要があります。
    /// </summary>
    /// </summary>
    [HarmonyPatch]
    internal class AddIpPointsReleaseHooks
    {
        [HarmonyPatch(typeof(gameScript), "AddIpPointsRelease")]
        internal static bool Prefix(ref gameScript __instance)
        {
            Debug.Log("================================================");
            Debug.Log("AddIpPointsReleaseHooks.AddIpPointsRelease.Prefix");
            Debug.Log("================================================");

            IpPointsAdditionManager.RefactoredAddIpPointsRelease(__instance);
            return false;
        }

        private static class IpPointsAdditionManager
        {
            private static gameScript instance;

            private static void Init(gameScript instance)
            {
            }

            internal static void RefactoredAddIpPointsRelease(gameScript instance)
            {
                //Init(instance);

                if (instance.mainIP != -1)
                {
                    if (!instance.script_mainIP)
                    {
                        instance.FindMainIpScript();
                    }
                    if (instance.script_mainIP && !instance.typ_bundle && !instance.typ_budget && !instance.typ_bundleAddon && !instance.typ_goty && !instance.typ_contractGame)
                    {
                        if (instance.reviewTotal < 40)
                        {
                            float reviewTotal = (float)instance.reviewTotal;
                            if (instance.portID != -1)
                            {
                                instance.AddIpPoints(-(reviewTotal * 0.4f));
                                return;
                            }
                            if (instance.typ_nachfolger)
                            {
                                instance.AddIpPoints(-reviewTotal);
                                return;
                            }
                            if (instance.typ_addon || instance.typ_addonStandalone)
                            {
                                instance.AddIpPoints(-(reviewTotal * 0.25f));
                                return;
                            }
                            if (instance.typ_mmoaddon)
                            {
                                instance.AddIpPoints(-(reviewTotal * 0.05f));
                                return;
                            }
                            if (instance.typ_remaster)
                            {
                                instance.AddIpPoints(-(reviewTotal * 0.5f));
                                return;
                            }
                            if (instance.typ_spinoff)
                            {
                                instance.AddIpPoints(-(reviewTotal * 0.9f));
                                return;
                            }
                            instance.AddIpPoints(-reviewTotal);
                            return;
                        }
                        else
                        {
                            float reviewTotalUppder40;
                            if (instance.developerID == instance.mS_.myID)
                            {
                                reviewTotalUppder40 = (float)(instance.reviewTotal * 2 / (instance.mS_.difficulty + 1));
                                reviewTotalUppder40 = IpPointsAdditionOnGameReleaseMultiplier.Instance.ApplyMultiplier(reviewTotalUppder40);
                            }
                            else
                            {
                                reviewTotalUppder40 = (float)(instance.reviewTotal * 2 / 2);
                            }
                            if (instance.script_mainIP.developerID == instance.mS_.myID)
                            {
                                if (instance.script_mainIP.ipTime < 8)
                                {
                                    reviewTotalUppder40 *= 0.2f;
                                }
                                else if ((instance.typ_nachfolger || instance.typ_remaster || instance.typ_spinoff) && instance.script_mainIP.ipTime < 30)
                                {
                                    reviewTotalUppder40 *= 0.5f;
                                }
                            }
                            if (instance.portID != -1)
                            {
                                instance.AddIpPoints(reviewTotalUppder40 * 0.4f);
                                return;
                            }
                            if (instance.typ_nachfolger)
                            {
                                instance.AddIpPoints(reviewTotalUppder40);
                                return;
                            }
                            if (instance.typ_addon || instance.typ_addonStandalone)
                            {
                                instance.AddIpPoints(reviewTotalUppder40 * 0.25f);
                                return;
                            }
                            if (instance.typ_mmoaddon)
                            {
                                instance.AddIpPoints(reviewTotalUppder40 * 0.05f);
                                return;
                            }
                            if (instance.typ_remaster)
                            {
                                instance.AddIpPoints(reviewTotalUppder40 * 0.5f);
                                return;
                            }
                            if (instance.typ_spinoff)
                            {
                                instance.AddIpPoints(reviewTotalUppder40 * 0.9f);
                                return;
                            }
                            instance.AddIpPoints(reviewTotalUppder40);
                            return;
                        }
                    }
                }
            }
            /*
            このAddIpPointsReleaseメソッドは、IPポイントを追加または減少させるロジックを含んでいて、特にレビュースコアやゲームのタイプに基づいて、どのようにIPポイントが変動するかを定義しているね。
            さらに、難易度(this.mS_.difficulty)がこのポイントの計算にどう影響するかが重要なポイントだ。
            このメソッドの主な部分を見てみると、二つの主要なブロックがあることがわかる。
            レビュースコアが40未満の場合:
            ゲームのタイプ（ポート、続編、アドオンなど）に基づいて、レビュースコアに応じた割合でIPポイントを減少させる。ここでの計算は比較的シンプルで、特定の乗数を使ってレビュースコアに基づいた値を減点している。
            レビュースコアが40以上の場合:
            この場合、計算がもう少し複雑になる。特に、ゲームの開発者がプレイヤー自身の場合、難易度がポイント計算に影響を与える。
            ここでの式num = (float) (this.reviewTotal* 2 / (this.mS_.difficulty + 1))は、難易度が高いほど、得られるIPポイントが少なくなることを意味する。
            難易度が0の場合（最も簡単）、割り算の分母は1となり、最も多くのポイントが得られる。難易度が上がると、分母が大きくなり、結果として得られるポイントが減少する。
            ここでの重要な考慮事項は、難易度がIPポイントの計算にどのように影響を与えるかを理解し、適切にバランスを取ることだ。
            難易度がゲームプレイの挑戦性を高める一方で、プレイヤーに不当に厳しいペナルティを課すことなく、公平で楽しい体験を提供する必要がある。
            お前がこのメソッドにフォーカスしたいと言ったのは理解できる。特に難易度の扱いについては、ゲームのバランスと直接関係してくるからな。
            このメソッドを調整するときは、ゲームの全体的なバランスと、プレイヤーが異なる難易度レベルでどのような体験をするかを常に念頭に置いておくことが大事だ。
            何か具体的な変更を加えたい部分があれば、もっと詳しく教えてくれ。一緒に考えてみようぜ。

            if (this.developerID == this.mS_.myID)
            ↑多分これは、自身のスタジオの場合ってことだと思われる。
        */
        }
    }
}
