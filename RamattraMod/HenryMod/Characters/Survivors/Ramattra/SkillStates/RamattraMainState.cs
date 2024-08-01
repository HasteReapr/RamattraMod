using BepInEx.Configuration;
using RamattraMod.Modules.Characters;
using RamattraMod.Survivors.Ramattra.Components;
using RamattraMod.Survivors.Ramattra.SkillStates;
using RamattraMod.Survivors.Ramattra.SkillStates.NemStates;
using RamattraMod.Survivors.Ramattra.Content;
using RoR2;
using RoR2.Skills;
using System;
using System.Collections.Generic;
using UnityEngine;
using RamattraMod.Survivors.Ramattra;

namespace RamattraMod.Survivors.Ramattra.SkillStates
{
    public class RamattraMainState : EntityStates.GenericCharacterMain
    {
        public bool NemForm;
        private RamattraNemesisController nemCtrl;
        public override void OnEnter()
        {
            base.OnEnter();
            nemCtrl = base.GetComponent<RamattraNemesisController>();
            NemForm = nemCtrl.isNem;
        }

        public override void Update()
        {
            base.Update();
            NemForm = nemCtrl.isNem;
            
            //this was originally used to swap between the skills between nemesis and omnic states, but is since depricated. It's really only kept here for the taunts stuff.

            if (NemForm)
            {
                
            }
            
            else
            {
               
            }
        }
        public override void OnExit()
        {
            base.OnExit();

        }
    }
}
