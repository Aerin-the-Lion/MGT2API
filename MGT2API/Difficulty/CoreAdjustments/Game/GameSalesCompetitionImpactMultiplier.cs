using MGT2API.Difficulty.Adjustments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MGT2API.Difficulty.Adjustments
{
    /// <summary>
    /// Manages custom multipliers applied to the competition impact calculation of games.
    /// [JP]ゲームの競争影響計算にカスタム乗数を適用するマネージャー。
    /// </summary>
    public sealed class GameSalesCompetitionImpactMultiplier : ICustomMultiplier
    {
        internal static GameSalesCompetitionImpactMultiplier instance;

        private GameSalesCompetitionImpactMultiplier()
        {
            // プライベートコンストラクターでインスタンス化を防ぐ
        }

        /// <summary>
        /// Gets the singleton instance of GameCompetitionImpactMultiplier.
        /// [JP] GameCompetitionImpactMultiplierのシングルトンインスタンスを取得します。
        /// </summary>
        public static GameSalesCompetitionImpactMultiplier Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new GameSalesCompetitionImpactMultiplier();
                }
                return instance;
            }
        }

        /// <summary>
        /// Event for applying a custom multiplier to the competition impact calculation of games.
        /// This event allows for dynamic adjustment of competition impact within the Modding environment.
        /// [JP] ゲームの競争影響計算にカスタム乗数を適用するためのイベント。
        /// このイベントを通じて、Modding環境内で競争影響を動的に調整することが可能になります。
        /// </summary>
        public static event Func<float, float> OnGameSalesMultiplierApplied;

        /// <summary>
        /// Applies a custom multiplier to the base competition impact value.
        /// This method is triggered by the OnCompetitionImpactMultiplierApplied event and allows for customization of the competition impact.
        /// [JP] 基本競争影響値にカスタム乗数を適用します。
        /// このメソッドはOnCompetitionImpactMultiplierAppliedイベントによってトリガーされ、競争影響のカスタマイズを可能にします。
        /// </summary>
        /// <param name="baseImpact">The base competition impact value to be adjusted.</param>
        /// <returns>The adjusted competition impact value after applying the custom modifications.</returns>
        public float ApplyMultiplier(float baseImpact)
        {
            var handler = OnGameSalesMultiplierApplied;
            float competitionImpact = baseImpact;

            try
            {
                if (handler != null)
                {
                    foreach (Func<float, float> modifier in handler.GetInvocationList())
                    {
                        competitionImpact = modifier(competitionImpact);
                    }
                }
            }
            catch (Exception ex)
            {
                UnityEngine.Debug.LogException(ex);
            }

            return competitionImpact;
        }
    }
}
