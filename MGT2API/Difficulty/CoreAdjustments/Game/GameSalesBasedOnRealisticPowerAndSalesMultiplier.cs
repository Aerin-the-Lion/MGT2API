using System;

namespace MGT2API.Difficulty.Adjustments
{
    /// <summary>
    /// Manages custom multipliers applied to game sales based on the game's realistic power and total sales.
    /// Details are in the document.
    /// [JP] ゲームのRealistic Powerと総売上に基づいてゲームの売上に適用されるカスタム乗数を管理します。
    /// 詳細は、ドキュメントを参照してください。
    /// </summary>
    public sealed class GameSalesBasedOnRealisticPowerAndSalesMultiplier : ICustomMultiplier
    {
        private static GameSalesBasedOnRealisticPowerAndSalesMultiplier instance;

        private GameSalesBasedOnRealisticPowerAndSalesMultiplier()
        {
            // Private constructor to prevent instantiation.
        }

        /// <summary>
        /// Gets the singleton instance of GameSalesBasedOnRealisticPowerAndSalesMultiplier.
        /// [JP] GameSalesBasedOnRealisticPowerAndSalesMultiplierのシングルトンインスタンスを取得します。
        /// </summary>
        internal static GameSalesBasedOnRealisticPowerAndSalesMultiplier Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new GameSalesBasedOnRealisticPowerAndSalesMultiplier();
                }
                return instance;
            }
        }

        /// <summary>
        /// Default : 0.06f
        /// Event registration point for applying a custom multiplier to game sales based on the game's realistic power and total sales.
        /// This event allows for dynamic adjustment of game sales within the Modding environment based on the game's realistic power and total sales.
        /// The Realistic Power value, when reaching these thresholds, gradually affects the decrease in game sales.
        /// [JP] ゲームのRealistic Powerと総売上に基づいてゲームの売上にカスタム乗数を適用するためのイベント登録ポイントです。
        /// Realistic Powerの数値は、これらしきい値に達することで、ゲームの売上に減少の影響を徐々に与えます。
        /// このイベントを通じて、Modding環境内でゲームのRealistic Powerと総売上に基づいたゲームの売上を動的に調整することが可能になります。
        /// </summary>
        public static event Func<float, float> OnGameSalesMultiplierApplied;

        /// <summary>
        /// Applies a custom multiplier to the base multiplier for game sales calculation based on the game's realistic power and total sales.
        /// This method is triggered by the OnSalesMultiplierApplied event and allows for the customization of game sales based on the game's realistic power and total sales.
        /// [JP] ゲームのRealistic Powerと総売上に基づいてゲームの売上計算の基本乗数にカスタム乗数を適用します。
        /// このメソッドはOnSalesMultiplierAppliedイベントによってトリガーされ、ゲームのRealistic Powerと総売上に基づいたゲームの売上のカスタマイズを可能にします。
        /// </summary>
        /// <param name="baseMultiplier">The base multiplier to be adjusted for the sales calculation.</param>
        /// <returns>The adjusted multiplier after applying the custom modifications for the sales calculation based on the game's realistic power and total sales.</returns>
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