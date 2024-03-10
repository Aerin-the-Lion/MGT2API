using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MGT2API.Difficulty.Adjustments
{
    /// Manages the custom multiplier for the development cost of game engines.
    /// [JP] ゲームエンジンの開発コストに適用するカスタム乗数を管理します。
    /// </summary>
    public sealed class EngineDevCostMultiplier : ICustomMultiplier
    {
        private static EngineDevCostMultiplier instance;

        /// <summary>
        /// Gets the singleton instance of the EngineDevCostMultiplierManager class.
        /// </summary>
        public static EngineDevCostMultiplier Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new EngineDevCostMultiplier();
                }
                return instance;
            }
        }

        /// <summary>
        /// Event registration point for applying a custom multiplier to the development cost of game engines.
        /// This event allows for dynamic adjustment of engine development costs within the Modding environment.
        /// [JP] ゲームエンジンの開発コストにカスタム乗数を適用するためのイベント登録ポイントです。
        /// このイベントを通じて、Modding環境内でエンジンの開発コストを動的に調整することが可能になります。
        /// </summary>
        public static event Func<float, float> OnCostCustomMultiplierApplied;

        /// <summary>
        /// Applies a custom multiplier to the base multiplier for the engine's development cost.
        /// This method is triggered by the OnCostCustomMultiplierApplied event and allows for the customization of engine development costs.
        /// [JP] エンジンの開発コストの基本乗数にカスタム乗数を適用します。
        /// このメソッドはOnCostCustomMultiplierAppliedイベントによってトリガーされ、エンジンの開発コストのカスタマイズを可能にします。
        /// </summary>
        /// <param name="baseMultiplier">The base multiplier to be adjusted.</param>
        /// <returns>The adjusted multiplier after applying the custom modifications.</returns>
        public float ApplyMultiplier(float baseMultiplier)
        {
            var handler = OnCostCustomMultiplierApplied;
            float costMultiplier = baseMultiplier;

            {
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
            }

            return costMultiplier;
        }
    }
}
