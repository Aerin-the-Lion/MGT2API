using System;

namespace MGT2API.Difficulty.Adjustments
{
    /// <summary>
    /// Defines the contract for a singleton instance of a class that applies custom multipliers to various aspects of the game.
    /// [JP] ゲームの様々な側面にカスタム乗数を適用するシングルトンインスタンスのクラスの契約を定義します。
    /// </summary>
    internal interface ICustomMultiplier
    {
        /// <summary>
        /// Applies a custom multiplier to a given value.
        /// [JP] 与えられた値にカスタム乗数を適用します。
        /// </summary>
        /// <param name="baseValue">The base value to be adjusted.</param>
        /// <returns>The adjusted value after applying the custom multiplier.</returns>
        float ApplyMultiplier(float baseValue);
    }
}