using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MGT2API.Difficulty
{
    internal class DifficultyAPI
    {
        private static bool _hooksEnabled = false;
        internal static void SetHooks()
        {
            if (_hooksEnabled)
            {
                return;
            }

            //HarmonyPatchのフックポイント

            //On.RoR2.DifficultyCatalog.GetDifficultyDef += GetExtendedDifficultyDef;
            //On.RoR2.RuleDef.FromDifficulty += InitialiseRuleBookAndFinalizeList;
        }
    }
}
