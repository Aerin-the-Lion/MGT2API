using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BepInEx;
using BepInEx.Bootstrap;
using BepInEx.Logging;
using UnityEngine;

namespace MGT2API.Difficulty.Adjustments
{
    /// <summary>
    /// Manages the custom multiplier for the rewards of Contract's ShortAssignments.
    /// Used to dynamically adjust rewards for ShortAssignments created weekly within the game.
    /// [JP] 契約の短期課題（ShortAssignment）の報酬に適用するカスタム乗数を管理します。
    /// ゲーム内で週に一度作成される短期課題の報酬を動的に調整する際に使用されます。
    /// </summary>
    public sealed class ContractShortAssignmentMultiplier : ICustomMultiplier
    {
        private static ContractShortAssignmentMultiplier instance;

        private ContractShortAssignmentMultiplier()
        {
            // Private constructor to prevent instantiation.
        }

        /// <summary>
        /// Gets the singleton instance of ContractShortAssignmentMultiplier.
        /// </summary>
        internal static ContractShortAssignmentMultiplier Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new ContractShortAssignmentMultiplier();
                }
                return instance;
            }
        }

        /// <summary>
        /// Event registration point for setting the reward of the ShortAssignment of the Contract.
        /// It is used when hooking the event of creating ShortAssignment once a week in the game.
        /// [JP]ContractのShortAssignmentの報酬を設定を行うイベント登録ポイント。
        /// ゲーム内1週間に一度のShortAssignmentの作成のイベントにフックする時に、用いられる。
        /// </summary>
        public static event Func<float, float> OnRewardCustomMultiplierApplied;


        /// <summary>
        /// Applies a custom multiplier to the base multiplier for the reward of ShortAssignments.
        /// This method is invoked by the OnRewardCustomMultiplierApplied event and allows for flexible reward adjustments.
        /// [JP] 短期課題の報酬の基本乗数にカスタム乗数を適用します。
        /// このメソッドはOnRewardCustomMultiplierAppliedイベントによって呼び出され、報酬の柔軟な調整を可能にします。
        /// </summary>
        /// <param name="baseMultiplier">The base multiplier to be adjusted.</param>
        /// <returns>The adjusted multiplier after applying the custom modifications.</returns>
        float ICustomMultiplier.ApplyMultiplier(float baseMultiplier)
        {
            var handler = OnRewardCustomMultiplierApplied;
            float rewardMultiplier = baseMultiplier;

            if (handler != null)
            {
                foreach (Func<float, float> modifier in handler.GetInvocationList())
                {
                    try
                    {
                        rewardMultiplier = modifier(rewardMultiplier);
                    }
                    catch (Exception ex)
                    {
                        UnityEngine.Debug.LogException(ex);
                    }
                }
            }

            return rewardMultiplier;
        }
    }
}
