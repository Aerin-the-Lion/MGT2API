using System;

namespace MGT2API.Difficulty.Adjustments
{
    /// <summary>
    /// Manages custom multipliers applied to the game sales based on the difficulty level.
    /// [JP] 難易度レベルに基づいてゲーム売上に適用されるカスタム乗数を管理します。
    /// </summary>
    public sealed class GameSalesBasedOnDifficultyMultiplier : ICustomMultiplier
    {
        internal static GameSalesBasedOnDifficultyMultiplier instance;

        private GameSalesBasedOnDifficultyMultiplier()
        {
            // プライベートコンストラクターでインスタンス化を防ぐ
        }

        /// <summary>
        /// Gets the singleton instance of GameSalesBasedOnDifficultyMultiplier.
        /// [JP] GameSalesBasedOnDifficultyMultiplierのシングルトンインスタンスを取得します。
        /// </summary>
        public static GameSalesBasedOnDifficultyMultiplier Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new GameSalesBasedOnDifficultyMultiplier();
                }
                return instance;
            }
        }

        /// <summary>
        /// Event for applying a custom multiplier to the game sales based on the difficulty level.
        /// This event allows for dynamic adjustment of game sales multipliers within the Modding environment.
        /// [JP] 難易度レベルに基づいてゲーム売上にカスタム乗数を適用するためのイベント。
        /// このイベントを通じて、Modding環境内でゲーム売上の乗数を動的に調整することが可能になります。
        /// </summary>
        public static event Func<float, float> OnGameSalesMultiplierApplied;

        /// <summary>
        /// Applies a custom multiplier to the base game sales value based on the difficulty level.
        /// This method is triggered by the OnDifficultySalesMultiplierApplied event and allows for customization of game sales multipliers.
        /// [JP] 難易度レベルに基づいて基本ゲーム売上値にカスタム乗数を適用します。
        /// このメソッドはOnDifficultySalesMultiplierAppliedイベントによってトリガーされ、ゲーム売上の乗数のカスタマイズを可能にします。
        /// </summary>
        /// <param name="baseSales">The base game sales value to be adjusted.</param>
        /// <returns>The adjusted game sales value after applying the custom modifications.</returns>
        public float ApplyMultiplier(float baseSales)
        {
            var handler = OnGameSalesMultiplierApplied;
            float salesMultiplier = baseSales;

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