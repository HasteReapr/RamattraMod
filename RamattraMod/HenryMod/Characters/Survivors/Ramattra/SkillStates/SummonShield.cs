﻿using EntityStates;
using RoR2;
using RoR2.Projectile;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace RamattraMod.Survivors.Ramattra.SkillStates
{
    public class SummonShield : GenericProjectileBaseState
    {
        private Ray aimRay;
        public float baseDuration = 0.5f;
        public float duration;
        public float fireTime;
        private Animator animator;
        private bool hasFired;

        private TeamComponent teamComponent;

        public override void OnEnter()
        {
            base.OnEnter();
            aimRay = base.GetAimRay();
            duration = baseDuration / attackSpeedStat;
            fireTime = 0;//duration;
            hasFired = false;

            teamComponent = base.gameObject.GetComponent<TeamComponent>();

            base.characterBody.SetAimTimer(duration);

            animator = GetModelAnimator();
            animator.SetBool("attacking", true);
            animator.SetBool("inCombat", true);
            GetModelAnimator().SetFloat("ThrowKnife.playbackRate", attackSpeedStat);

            PlayCrossfade("Gesture, Override", "ThrowBall", "ThrowKnife.playbackRate", duration, 0.1f);
        }

        public override void OnExit()
        {
            base.OnExit();
            animator.SetBool("inCombat", false);
            animator.SetBool("attacking", false);
            //PlayAnimation("Gesture, Override", "BufferEmpty");
        }

        private void BlessThineShield(IEnumerable<TeamComponent> recipients, float radiusSqr, Vector3 currentPosition) //I couldn't think of a name for this method, it basically handles dealing damage to enemies
        {
            if (!NetworkServer.active) return;

            foreach (TeamComponent teamComponent in recipients)
            {
                if ((teamComponent.transform.position - currentPosition).sqrMagnitude <= radiusSqr)
                {
                    CharacterBody charBody = teamComponent.body;
                    if (charBody)
                    {
                        if(charBody.healthComponent.barrier + (charBody.healthComponent.fullHealth + charBody.healthComponent.fullShield) * RamattraStaticValues.restoreShieldPerc < charBody.healthComponent.fullCombinedHealth * 2)
                        {
                            charBody.healthComponent.barrier += (charBody.healthComponent.fullHealth + charBody.healthComponent.fullShield) * RamattraStaticValues.restoreShieldPerc;
                        }
                    }
                }
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if (fixedAge >= fireTime && !hasFired)
            {
                if (NetworkServer.active)
                {
                    float radiusSqr = 20 * 20;

                    Vector3 position = transform.position;

                    hasFired = true;
                    for (TeamIndex teamIndex = TeamIndex.Neutral; teamIndex < TeamIndex.Count; teamIndex += 1)
                    {
                        if (teamIndex == teamComponent.teamIndex)
                        {
                            BlessThineShield(TeamComponent.GetTeamMembers(teamIndex), radiusSqr, position);
                        }
                    }
                }
            }

            if (fixedAge >= duration && isAuthority)
            {
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