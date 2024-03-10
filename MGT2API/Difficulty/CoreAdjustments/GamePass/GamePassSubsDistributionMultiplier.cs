using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MGT2API.Difficulty.Adjustments
{
    /// <summary>
    /// Manages custom multipliers for Game Pass subscription distribution.
    /// [JP] Game Pass定期購読の分配におけるカスタム乗数を管理します。
    /// </summary>
    public sealed class GamePassSubsDistributionMultiplier : ICustomMultiplier
    {
        private static GamePassSubsDistributionMultiplier instance;

        private GamePassSubsDistributionMultiplier()
        {
            // Private constructor to prevent instantiation.
        }

        /// <summary>
        /// Gets the singleton instance of GamePassSubsDistributionMultiplier.
        /// </summary>
        internal static GamePassSubsDistributionMultiplier Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new GamePassSubsDistributionMultiplier();
                }
                return instance;
            }
        }

        /// <summary>
        /// Provides an event for customizing the multiplier applied to the distribution of Game Pass subscriptions on the server.
        /// This event is used to hook into the weekly distribution process of Game Pass subscriptions in the game.
        /// [JP] サーバー上のGame Pass定期購読の分配に適用される乗数をカスタマイズするためのイベントを提供します。
        /// [JP] このイベントは、ゲーム内で週ごとに行われるGame Pass定期購読の分配プロセスにフックするために使用されます。
        /// [JP] 乗数が高ければ高いほど、サーバーのGame Pass定期購読の分配が減少します。
        /// </summary>
        public static event Func<float, float> OnSubscriptionsCustomMultiplierApplied;

        /// <summary>
        /// Applies a custom multiplier to the base multiplier for Game Pass subscription distribution.
        /// [JP] Game Pass定期購読の分配の基本乗数にカスタム乗数を適用します。
        /// <param name="baseMultiplier">The base multiplier for Game Pass subscription distribution.</param>
        /// <returns>The modified multiplier after applying custom adjustments.</returns>
        /// <returns></returns>
        public float ApplyMultiplier(float baseMultiplier)
        {
            var handler = OnSubscriptionsCustomMultiplierApplied;
            float subscriptionsMultiplier = baseMultiplier;

            try
            {
                if (handler != null)
                {
                    foreach (Func<float, float> modifier in handler.GetInvocationList())
                    {
                        subscriptionsMultiplier = modifier(subscriptionsMultiplier);
                    }
                }
            }
            catch (Exception ex)
            {
                UnityEngine.Debug.LogException(ex);
            }

            return subscriptionsMultiplier;
        }
    }
}
