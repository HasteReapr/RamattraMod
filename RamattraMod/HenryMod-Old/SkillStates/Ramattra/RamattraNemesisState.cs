using UnityEngine;
using RamattraMod;
using RoR2;
using BepInEx.Configuration;
using RamattraMod.Modules;
using RamattraMod.SkillStates.BaseStates;
using System;
using EntityStates;

namespace RamattraMod.SkillStates.Ramattra
{
    public class RamattraNemesisState : GenericCharacterMain
    {
        private LocalUser localUser;

        public override void OnEnter()
        {
            base.OnEnter();
            FindLocalUser();
            SetNemesisSkills();
        }

        private void SetNemesisStats()
        {

        }
        
        private void SetNemesisSkills()
        {
            if(base.skillLocator.primary != null)
                base.skillLocator.primary.SetSkillOverride(this, StaticValues.ThrowPummel, GenericSkill.SkillOverridePriority.Contextual);
        }

        private void SetOmnicSkills()
        {
            if (base.skillLocator.primary != null)
                base.skillLocator.primary.UnsetSkillOverride(this, StaticValues.FireStaff, GenericSkill.SkillOverridePriority.Contextual);
        }

        private void FindLocalUser()
        {
            if (localUser == null)
            {
                if (base.characterBody)
                {
                    foreach (LocalUser lu in LocalUserManager.readOnlyLocalUsersList)
                    {
                        if (lu.cachedBody == base.characterBody)
                        {
                            this.localUser = lu;
                            break;
                        }
                    }
                }
            }
        }

        public override void FixedUpdate()
        {
            
        }

        public override void OnExit()
        {
            base.OnExit();
            SetOmnicSkills();
        }
    }
}
