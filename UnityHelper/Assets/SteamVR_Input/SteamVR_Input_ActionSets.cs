// ------------------------------------------------------------------------------
//  <autogenerated>
//      This code was generated by a tool.
//      Mono Runtime Version: 2.0.50727.1433
// 
//      Changes to this file may cause incorrect behavior and will be lost if 
//      the code is regenerated.
//  </autogenerated>
// ------------------------------------------------------------------------------

namespace Valve.VR
{
    using System;
    using UnityEngine;
    
    
    public partial class SteamVR_Actions
    {
        
        private static SteamVR_Input_ActionSet_perhand p_perhand;
        
        private static SteamVR_Input_ActionSet_DominantHand p_DominantHand;
        
        private static SteamVR_Input_ActionSet_NonDominantHand p_NonDominantHand;
        
        private static SteamVR_Input_ActionSet_MovementHand p_MovementHand;
        
        private static SteamVR_Input_ActionSet_RotationHand p_RotationHand;
        
        public static SteamVR_Input_ActionSet_perhand perhand
        {
            get
            {
                return SteamVR_Actions.p_perhand.GetCopy <SteamVR_Input_ActionSet_perhand>();
            }
        }
        
        public static SteamVR_Input_ActionSet_DominantHand DominantHand
        {
            get
            {
                return SteamVR_Actions.p_DominantHand.GetCopy <SteamVR_Input_ActionSet_DominantHand>();
            }
        }
        
        public static SteamVR_Input_ActionSet_NonDominantHand NonDominantHand
        {
            get
            {
                return SteamVR_Actions.p_NonDominantHand.GetCopy <SteamVR_Input_ActionSet_NonDominantHand>();
            }
        }
        
        public static SteamVR_Input_ActionSet_MovementHand MovementHand
        {
            get
            {
                return SteamVR_Actions.p_MovementHand.GetCopy <SteamVR_Input_ActionSet_MovementHand>();
            }
        }
        
        public static SteamVR_Input_ActionSet_RotationHand RotationHand
        {
            get
            {
                return SteamVR_Actions.p_RotationHand.GetCopy <SteamVR_Input_ActionSet_RotationHand>();
            }
        }
        
        private static void StartPreInitActionSets()
        {
            SteamVR_Actions.p_perhand = ((SteamVR_Input_ActionSet_perhand)(SteamVR_ActionSet.Create <SteamVR_Input_ActionSet_perhand>("/actions/perhand")));
            SteamVR_Actions.p_DominantHand = ((SteamVR_Input_ActionSet_DominantHand)(SteamVR_ActionSet.Create <SteamVR_Input_ActionSet_DominantHand>("/actions/DominantHand")));
            SteamVR_Actions.p_NonDominantHand = ((SteamVR_Input_ActionSet_NonDominantHand)(SteamVR_ActionSet.Create <SteamVR_Input_ActionSet_NonDominantHand>("/actions/NonDominantHand")));
            SteamVR_Actions.p_MovementHand = ((SteamVR_Input_ActionSet_MovementHand)(SteamVR_ActionSet.Create <SteamVR_Input_ActionSet_MovementHand>("/actions/MovementHand")));
            SteamVR_Actions.p_RotationHand = ((SteamVR_Input_ActionSet_RotationHand)(SteamVR_ActionSet.Create <SteamVR_Input_ActionSet_RotationHand>("/actions/RotationHand")));
            Valve.VR.SteamVR_Input.actionSets = new Valve.VR.SteamVR_ActionSet[]
            {
                    SteamVR_Actions.perhand,
                    SteamVR_Actions.DominantHand,
                    SteamVR_Actions.NonDominantHand,
                    SteamVR_Actions.MovementHand,
                    SteamVR_Actions.RotationHand};
        }
    }
}
