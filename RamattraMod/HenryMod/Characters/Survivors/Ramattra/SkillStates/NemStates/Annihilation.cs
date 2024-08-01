using EntityStates;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;
using RamattraMod.Modules;
using RamattraMod.Survivors.Ramattra.Components;

namespace RamattraMod.Survivors.Ramattra.SkillStates.NemStates
{
    internal class Annihilation : BaseState
    {
        RamattraAnnihilationController annCtrl;
        RamattraNemesisController nemCtrl;

        public static float procCoefficient = 1f;
        public static float baseDuration = 20f;

        private GameObject annRadius;

        public override void OnEnter()
        {
            base.OnEnter();
            
            annCtrl = base.GetComponent<RamattraAnnihilationController>();
            //we reset the values of the annihilation controller, so it can properly behave
            annCtrl.ResetVals();

            //we get the nemesis controller so we can exit this skill if we swap out of nemesis state
            nemCtrl = base.GetComponent<RamattraNemesisController>();

            ChildLocator childLocator = base.GetModelChildLocator();

            if (childLocator)
            {
                annRadius = childLocator.FindChild("AnnihilationArea").gameObject;
                //set to false then set to true real quick to basically restart the effect from the beggining
                annRadius.SetActive(false);
                annRadius.SetActive(true);
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if (annCtrl.shouldExpire || !nemCtrl.isNem || fixedAge > baseDuration)
            {
                if (isAuthority)
                {
                    if (annRadius)
                    {
                        annRadius.SetActive(false);
                        annRadius.GetComponentInChildren<ParticleSystem>().Stop();
                    }

                    annCtrl.ForceEnd(); //forcefully end the annihilation stuff, so it doesn't just infinitely continue

                    outer.SetNextStateToMain();
                    return;
                }
                
            }
        }

        public override void OnExit()
        {
            base.OnExit();
            if (isAuthority)
            {
                if (annRadius)
                {
                    annRadius.SetActive(false);
                    annRadius.GetComponentInChildren<ParticleSystem>().Stop();
                }
                //forcefully end annihilation in case we're frozen or smthn so it stops
                annCtrl.ForceEnd();

                outer.SetNextStateToMain();
                return;
            }
            
        }
    }
}
