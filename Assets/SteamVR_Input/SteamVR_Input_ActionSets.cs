//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Valve.VR
{
    using System;
    using UnityEngine;
    
    
    public partial class SteamVR_Actions
    {
        
        private static SteamVR_Input_ActionSet_Alignment p_Alignment;
        
        private static SteamVR_Input_ActionSet_InGame p_InGame;
        
        private static SteamVR_Input_ActionSet_mixedreality p_mixedreality;
        
        private static SteamVR_Input_ActionSet_UI p_UI;
        
        public static SteamVR_Input_ActionSet_Alignment Alignment
        {
            get
            {
                return SteamVR_Actions.p_Alignment.GetCopy<SteamVR_Input_ActionSet_Alignment>();
            }
        }
        
        public static SteamVR_Input_ActionSet_InGame InGame
        {
            get
            {
                return SteamVR_Actions.p_InGame.GetCopy<SteamVR_Input_ActionSet_InGame>();
            }
        }
        
        public static SteamVR_Input_ActionSet_mixedreality mixedreality
        {
            get
            {
                return SteamVR_Actions.p_mixedreality.GetCopy<SteamVR_Input_ActionSet_mixedreality>();
            }
        }
        
        public static SteamVR_Input_ActionSet_UI UI
        {
            get
            {
                return SteamVR_Actions.p_UI.GetCopy<SteamVR_Input_ActionSet_UI>();
            }
        }
        
        private static void StartPreInitActionSets()
        {
            SteamVR_Actions.p_Alignment = ((SteamVR_Input_ActionSet_Alignment)(SteamVR_ActionSet.Create<SteamVR_Input_ActionSet_Alignment>("/actions/Alignment")));
            SteamVR_Actions.p_InGame = ((SteamVR_Input_ActionSet_InGame)(SteamVR_ActionSet.Create<SteamVR_Input_ActionSet_InGame>("/actions/InGame")));
            SteamVR_Actions.p_mixedreality = ((SteamVR_Input_ActionSet_mixedreality)(SteamVR_ActionSet.Create<SteamVR_Input_ActionSet_mixedreality>("/actions/mixedreality")));
            SteamVR_Actions.p_UI = ((SteamVR_Input_ActionSet_UI)(SteamVR_ActionSet.Create<SteamVR_Input_ActionSet_UI>("/actions/UI")));
            Valve.VR.SteamVR_Input.actionSets = new Valve.VR.SteamVR_ActionSet[] {
                    SteamVR_Actions.Alignment,
                    SteamVR_Actions.InGame,
                    SteamVR_Actions.mixedreality,
                    SteamVR_Actions.UI};
        }
    }
}
