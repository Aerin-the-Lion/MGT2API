using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MGT2API.Difficulty.Adjustments
{
    /// <summary>
    /// Manages the custom multiplier for IP points awarded when selling games weekly.
    /// [JP] 週毎、ゲームを販売した際に変動するIPポイントのカスタム乗数を管理します。
    /// </summary>
    public sealed class IpPointsAdditionOnSellPerWeekMultiplier
    {
        private static IpPointsAdditionOnSellPerWeekMultiplier instance;

        private IpPointsAdditionOnSellPerWeekMultiplier()
        {
            // Private constructor to prevent instantiation
        }

        /// <summary>
        /// Manages the custom multiplier for IP points awarded when selling games weekly.
        /// [JP] 週毎、ゲームを販売した際に変動するIPポイントのカスタム乗数を管理します。
        /// </summary>
        public static IpPointsAdditionOnSellPerWeekMultiplier Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new IpPointsAdditionOnSellPerWeekMultiplier();
                }
                return instance;
            }
        }

        /// <summary>
        /// Event registration point for applying a custom multiplier to IP points awarded from weekly game sales.
        /// The default source code of MGT2 has a setting where the multiplier of IP points changes every 250 weeks.
        /// [JP] 週毎のゲーム販売から得られるIPポイントにカスタム乗数を適用するためのイベント登録ポイントです。
        /// なお、MGT2デフォルトのソースコードでは、250週毎にIPポイントの乗数が変動する設定になっています。
        /// </summary>
        public static event Func<float, float> OnIpPointsCustomMultiplierApplied;

        /// <summary>
        /// Applies a custom multiplier to the base multiplier for IP points awarded from weekly game sales.
        /// This method is triggered by the OnIpPointsCustomMultiplierApplied event and facilitates the customization of the IP point reward system.
        /// [JP] 週毎のゲーム販売から得られるIPポイントの基本乗数にカスタム乗数を適用します。
        /// このメソッドはOnIpPointsCustomMultiplierAppliedイベントによってトリガーされ、IPポイント報酬システムのカスタマイズを促進します。
        /// </summary>
        internal float ApplyCustomMultiplier(float baseMultiplier)
        {
            var handler = OnIpPointsCustomMultiplierApplied;
            float ipPointsMultiplier = baseMultiplier;

            try
            {
                if (handler != null)
                {
                    foreach (Func<float, float> modifier in handler.GetInvocationList())
                    {
                        ipPointsMultiplier = modifier(ipPointsMultiplier);
                    }
                }
            }
            catch (Exception ex)
            {
                UnityEngine.Debug.LogException(ex);
            }

            return ipPointsMultiplier;
        }
    }
}
