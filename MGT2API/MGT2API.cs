using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using UnityEngine;


namespace MGT2API
{
    [BepInPlugin(PluginGuid, PluginName, PluginVersion)]
    [BepInProcess("Mad Games Tycoon 2.exe")]
    internal class MGT2API : BaseUnityPlugin
    {
        public const string PluginGuid = "MGT2API";
        public const string PluginName = "MGT2API";
        public const string PluginVersion = "0.0.0.1";
        public Harmony harmony = new Harmony(PluginGuid);

        void Awake()
        {
            //ConfigManager configManager = new ConfigManager(Config);
            LoadHooks();
        }

        void LoadHooks()
        {
            Logger.LogInfo(nameof(LoadHooks));
            harmony.PatchAll(); //←うまく行きます。当面はこれでやります…。
        }
    }
}
