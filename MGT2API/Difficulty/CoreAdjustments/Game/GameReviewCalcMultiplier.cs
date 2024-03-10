using System;

namespace MGT2API.Difficulty.Adjustments
{
    /// <summary>
    /// Manages custom multipliers applied to the review calculation of games.
    /// [JP]ゲームのレビュー計算にカスタム乗数を適用するカスタム乗数を管理します。
    /// </summary>
    public class GameReviewCalcMultiplier : ICustomMultiplier
    {
        private static GameReviewCalcMultiplier instance;

        private GameReviewCalcMultiplier()
        {
            // Private constructor to prevent instantiation.
        }

        /// <summary>
        /// Gets the singleton instance of GameReviewCalcMultiplier.
        /// </summary>
        internal static GameReviewCalcMultiplier Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new GameReviewCalcMultiplier();
                }
                return instance;
            }
        }

        /// Event registration point for applying a custom multiplier to the review calculation of games.
        /// This event allows for dynamic adjustment of game review scores within the Modding environment.
        /// [JP] ゲームのレビュー計算にカスタム乗数を適用するためのイベント登録ポイントです。
        /// このイベントを通じて、Modding環境内でゲームのレビュースコアを動的に調整することが可能になります。
        /// </summary>
        public static event Func<float, float> OnReviewCalcMultiplierApplied;

        /// <summary>
        /// Applies a custom multiplier to the base multiplier for the game's review calculation.
        /// This method is triggered by the OnReviewCalcMultiplierApplied event and allows for the customization of game review scores.
        /// [JP] ゲームのレビュー計算の基本乗数にカスタム乗数を適用します。
        /// このメソッドはOnReviewCalcMultiplierAppliedイベントによってトリガーされ、ゲームのレビュースコアのカスタマイズを可能にします。
        /// </summary>
        /// <param name="baseMultiplier">The base multiplier to be adjusted for the review calculation.</param>
        /// <returns>The adjusted multiplier after applying the custom modifications for the review calculation.</returns>
        public float ApplyMultiplier(float baseMultiplier)
        {
            var handler = OnReviewCalcMultiplierApplied;
            float reviewMultiplier = baseMultiplier;

            try
            {
                if (handler != null)
                {
                    foreach (Func<float, float> modifier in handler.GetInvocationList())
                    {
                        reviewMultiplier = modifier(reviewMultiplier);
                    }
                }
            }
            catch (Exception ex)
            {
                UnityEngine.Debug.LogException(ex);
            }

            return reviewMultiplier;
        }
    }
}
