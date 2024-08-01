using EntityStates;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;
using RamattraMod.Modules;
using RamattraMod.Survivors.Ramattra.Components;

namespace RamattraMod.Survivors.Ramattra.SkillStates.NemStates
{
    public class NemesisBlock : BaseState
    {
        RamattraNemesisController nemCtrl;
        public override void OnEnter()
        {
            base.OnEnter();

            nemCtrl = base.GetComponent<RamattraNemesisController>();
            nemCtrl.isBlocking = true;

            if(NetworkServer.active)
                characterBody.AddBuff(RammyBuffs.blockBuff);
        }

        public override void Update()
        {
            base.Update();
            nemCtrl.isBlocking = true;
        }

        public override void OnExit()
        {
            base.OnExit();

            if (NetworkServer.active)
                characterBody.RemoveBuff(RammyBuffs.blockBuff);

            if (isAuthority)
            {
                nemCtrl.isBlocking = false;
                outer.SetNextStateToMain();

                return;
            }
            
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }
    }
}
