using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MGT2API.Difficulty.Adjustments
{
    /// <summary>
    /// Manages custom multipliers for adding IP points on game release.
    /// [JP]ゲームをリリース時のIPポイントを加算されるカスタム乗数を管理します。
    /// </summary>
    public sealed class IpPointsAdditionOnGameReleaseMultiplier : ICustomMultiplier
    {
        private static IpPointsAdditionOnGameReleaseMultiplier instance;

        private IpPointsAdditionOnGameReleaseMultiplier()
        {
            // Private constructor to prevent instantiation.
        }

        /// <summary>
        /// Gets the singleton instance of the IpPointsAdditionOnGameReleaseMultiplierManager class.
        /// </summary>
        public static IpPointsAdditionOnGameReleaseMultiplier Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new IpPointsAdditionOnGameReleaseMultiplier();
                }
                return instance;
            }
        }

        /// <summary>
        /// Event for applying a custom multiplier to IP points added on game release.
        /// [JP] ゲームリリース時に加算されるIPポイントにカスタム乗数を適用するイベント。
        /// </summary>
        public static event Func<float, float> OnIpPointsCustomMultiplierApplied;

        /// <summary>
        /// Applies a custom multiplier to the base multiplier for IP points added on game release.
        /// [JP] ゲームリリース時に加算されるIPポイントの基本乗数にカスタム乗数を適用します。
        /// </summary>
        /// <param name="baseMultiplier">The base multiplier for IP points to be adjusted.</param>
        /// <returns>The adjusted multiplier after applying the custom modifications.</returns>
        public float ApplyMultiplier(float baseMultiplier)
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
