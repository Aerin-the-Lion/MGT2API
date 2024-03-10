using System;

namespace MGT2API.Difficulty.Adjustments
{
    /// <summary>
    /// Manages custom multipliers applied to game sales based on IP popularity.
    /// [JP]IPの人気度に基づいてゲームの売上に適用されるカスタム乗数を管理します。
    /// </summary>
    public class GameSalesBasedOnIpPopularityMultiplier : ICustomMultiplier
    {
        private static GameSalesBasedOnIpPopularityMultiplier instance;

        private GameSalesBasedOnIpPopularityMultiplier()
        {
            // Private constructor to prevent instantiation.
        }

        /// <summary>
        /// Gets the singleton instance of GameSalesBasedOnIpPopularityMultiplier.
        /// </summary>
        internal static GameSalesBasedOnIpPopularityMultiplier Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new GameSalesBasedOnIpPopularityMultiplier();
                }
                return instance;
            }
        }

        /// <summary>
        /// Event registration point for applying a custom multiplier to game sales based on IP popularity.
        /// This event allows for dynamic adjustment of game sales within the Modding environment based on IP popularity.
        /// [JP] IPの人気度に基づいてゲームの売上にカスタム乗数を適用するためのイベント登録ポイントです。
        /// このイベントを通じて、Modding環境内でIPの人気度に基づいたゲームの売上を動的に調整することが可能になります。
        /// </summary>
        public static event Func<float, float> OnGameSalesMultiplierApplied;

        /// <summary>
        /// Applies a custom multiplier to the base multiplier for game sales calculation based on IP popularity.
        /// This method is triggered by the OnSalesCalcMultiplierApplied event and allows for the customization of game sales based on IP popularity.
        /// [JP] IPの人気度に基づいてゲームの売上計算の基本乗数にカスタム乗数を適用します。
        /// このメソッドはOnSalesCalcMultiplierAppliedイベントによってトリガーされ、IPの人気度に基づいたゲームの売上のカスタマイズを可能にします。
        /// </summary>
        /// <param name="baseMultiplier">The base multiplier to be adjusted for the sales calculation.</param>
        /// <returns>The adjusted multiplier after applying the custom modifications for the sales calculation based on IP popularity.</returns>
        public float ApplyMultiplier(float baseMultiplier)
        {
            var handler = OnGameSalesMultiplierApplied;
            float salesMultiplier = baseMultiplier;

            try
            {
                if (handler != null)
                {
                    foreach (Func<float, float> modifier in handler.GetInvocationList())
                    {
                        salesMultiplier = modifier(salesMultiplier);
                    }
                }
            }
            catch (Exception ex)
            {
                UnityEngine.Debug.LogException(ex);
            }

            return salesMultiplier;
        }
    }
}