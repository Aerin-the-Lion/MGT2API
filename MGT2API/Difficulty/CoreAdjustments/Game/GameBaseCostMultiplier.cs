using System;

namespace MGT2API.Difficulty.Adjustments
{
    /// <summary>
    /// Manages the custom multiplier for the base cost of games.
    /// The base cost of games refers to the cost of creating games when self-publishing games.
    /// [JP] ゲームの基本コストに適用するカスタム乗数を管理します。
    /// ここでいうゲームの基本コストとは、ゲーム自社バブリッシングの際のゲーム作成コストのことです。
    /// </summary>
    public sealed class GameBaseCostMultiplier : ICustomMultiplier
    {
        private static GameBaseCostMultiplier instance;

        private GameBaseCostMultiplier()
        {
            // Private constructor to prevent instantiation.
        }

        /// <summary>
        /// Gets the singleton instance of GameBaseCostMultiplierManager.
        /// </summary>
        internal static GameBaseCostMultiplier Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new GameBaseCostMultiplier();
                }
                return instance;
            }
        }

        /// Event registration point for applying a custom multiplier to the base cost of games.
        /// This event allows for dynamic adjustment of game base costs within the Modding environment.
        /// [JP] ゲームの基本コストにカスタム乗数を適用するためのイベント登録ポイントです。
        /// このイベントを通じて、Modding環境内でゲームの基本コストを動的に調整することが可能になります。
        /// </summary>
        public static event Func<float, float> OnCostCustomMultiplierApplied;

        /// <summary>
        /// Applies a custom multiplier to the base multiplier for the game's base cost.
        /// This method is triggered by the OnCostCustomMultiplierApplied event and allows for the customization of game base costs.
        /// [JP] ゲームの基本コストの基本乗数にカスタム乗数を適用します。
        /// このメソッドはOnCostCustomMultiplierAppliedイベントによってトリガーされ、ゲームの基本コストのカスタマイズを可能にします。
        /// </summary>
        /// <param name="baseMultiplier">The base multiplier to be adjusted.</param>
        /// <returns>The adjusted multiplier after applying the custom modifications.</returns>
        public float ApplyMultiplier(float baseMultiplier)
        {
            var handler = OnCostCustomMultiplierApplied;
            float costMultiplier = baseMultiplier;

            try
            {
                if (handler != null)
                {
                    foreach (Func<float, float> modifier in handler.GetInvocationList())
                    {
                        costMultiplier = modifier(costMultiplier);
                    }
                }
            }
            catch (Exception ex)
            {
                UnityEngine.Debug.LogException(ex);
            }

            return costMultiplier;
        }
    }
}

