using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FIMSpace.BonesStimulation
{
    public partial class BonesStimulator : UnityEngine.EventSystems.IDropHandler, IFHierarchyIcon
    {
        
        #region Hierarchy Icon

        public string EditorIconPath { get { if (PlayerPrefs.GetInt("AnimsH", 1) == 0) return ""; else return "Bones Stimulator/BonesStimulator"; } }
        public void OnDrop(UnityEngine.EventSystems.PointerEventData data) { }

        #endregion


        #region Editor Helpers

        [HideInInspector] public string _editor_Title = " Bones Stimulator";

        [HideInInspector] public bool _editor_DrawSetup = true;
        [HideInInspector] public bool _editor_DrawTweaking = false;

        [HideInInspector] public int _editor_DisplayedPreset = 0;
        [HideInInspector] public bool _editor_DrawGizmos = true;

        public bool DrawGizmos = true;
        //[HideInInspector] public Type _editor_ViewCategory;
        //[HideInInspector] public EMD_SetupCategory _editor_SetupCategory = EMD_SetupCategory.Movement;

        #endregion

        public enum EStimulationMode
        { Muscles, Vibrate, Squeezing, Collisions }
        public EStimulationMode _editor_SelCategory = EStimulationMode.Muscles;
    }
}