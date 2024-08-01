using RoR2;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Events;

namespace RamattraMod.Survivors.Ramattra.Components
{
    public class RamattraAnnihilationController : NetworkBehaviour
    {
        public float radius = 40;
        public float interval = 0.2f;
        public bool shouldExpire = false;

        // These are serialized because they are vital to the behaviour, but should not be public.
        //[SerializeField]
        public float countdown = 3f;
        //[SerializeField]
        public bool resetCountdown;
        //[SerializeField]
        public bool forcedEnding;

        public TeamComponent teamComponent;
        public CharacterBody charBody;
        public float timer = 0;

        public void ResetVals()
        {
            if (NetworkServer.active)
            {
                this.countdown = 3f;
                this.resetCountdown = false;
                this.shouldExpire = false;
                this.forcedEnding = false;

                this.timer = 0f;
            }
        }

        public void ForceEnd()
        {
            if (NetworkServer.active)
            {
                this.countdown = 0;
                this.resetCountdown = false;
                this.shouldExpire = true;
                this.forcedEnding = true;
            }
        }

        private void Awake()
        {
            this.teamComponent = base.gameObject.GetComponent<TeamComponent>();
            this.charBody = base.gameObject.GetComponent<CharacterBody>();

            ResetVals();
            ForceEnd();
        }

        private void Start()
        {
            this.teamComponent = base.gameObject.GetComponent<TeamComponent>();
            this.charBody = base.gameObject.GetComponent<CharacterBody>();

            ResetVals();
            ForceEnd();
        }

        private void Update()
        {
            this.countdown -= Time.deltaTime;

            if (this.resetCountdown)
            {
                //resets the countdown to 3 if resetCooldown is true, otherwise it continues to count down
                this.countdown = 3;
            }

            if(this.countdown <= 0)
            {
                //if the countdown gets below 0 we need to end the skill early
                this.shouldExpire = true;
            }
        }

        private void FixedUpdate()
        {
            if (NetworkServer.active && !forcedEnding && hasAuthority)
            {
                //if (forcedEnding) return;

                this.timer -= Time.deltaTime;
                if (this.timer <= 0f)
                {
                    resetCountdown = false; //set this to false here, that way if its never set to true we just start the end of the skill.

                    float radiusSqr = this.radius * this.radius;

                    Vector3 position = base.transform.position;

                    for (TeamIndex teamIndex = TeamIndex.Neutral; teamIndex < TeamIndex.Count; teamIndex += 1)
                    {
                        if (teamIndex != this.teamComponent.teamIndex)
                        {
                            GraggaWagga(TeamComponent.GetTeamMembers(teamIndex), radiusSqr, position);
                        }
                    }
                    timer = interval;
                }
            }
        }

        private void GraggaWagga(IEnumerable<TeamComponent> recipients, float radiusSqr, Vector3 currentPosition) //I couldn't think of a name for this method, it basically handles dealing damage to enemies
        {
            //if (!NetworkServer.active) return;

            foreach (TeamComponent teamComponent in recipients)
            {
                if ((teamComponent.transform.position - currentPosition).sqrMagnitude <= radiusSqr)
                {
                    CharacterBody charBody = teamComponent.body;
                    if (charBody)
                    {
                        //if (charBody == this.charBody) return; //checking to make sure we arent about to damage ourself, because that occasionally happens
                        if (NetworkServer.active)
                        {
                            DamageInfo info = new DamageInfo
                            {
                                attacker = base.gameObject,
                                inflictor = base.gameObject,
                                damage = RamattraStaticValues.annihilationDamageCoef * this.charBody.damage,
                                damageColorIndex = DamageColorIndex.Void,
                                damageType = DamageType.Generic,
                                crit = false,
                                dotIndex = DotController.DotIndex.None,
                                force = Vector3.zero,
                                position = charBody.transform.position,
                                procChainMask = default(ProcChainMask),
                                procCoefficient = 0
                            };

                            charBody.healthComponent.TakeDamage(info);
                            //if we have an enemy that we can attack we shouldnt reset the cooldown because we're currently attacking *something* so
                            resetCountdown = true;
                        }
                    }
                }
            }
        }

        public override bool OnSerialize(NetworkWriter writer, bool forceAll)
        {
            if (forceAll)
            {
                writer.Write(this.countdown);
                writer.Write(this.resetCountdown);
                writer.Write(this.forcedEnding);
                writer.Write(this.shouldExpire);
                return true;
            }

            bool flag = false;

            if((base.syncVarDirtyBits & 1U) != 0U)
            {
                if (!flag)
                {
                    writer.WritePackedUInt32(base.syncVarDirtyBits);
                    flag = true;
                }

                writer.Write(this.countdown);
                writer.Write(this.resetCountdown);
                writer.Write(this.forcedEnding);
                writer.Write(this.shouldExpire);
            }

            if (!flag)
            {
                writer.WritePackedUInt32(base.syncVarDirtyBits);
            }

            return flag;
        }

        public override void OnDeserialize(NetworkReader reader, bool initialState)
        {
            if (initialState)
            {
                this.countdown = reader.ReadSingle();
                this.resetCountdown = reader.ReadBoolean();
                this.forcedEnding = reader.ReadBoolean();
                this.shouldExpire = reader.ReadBoolean();
                return;
            }

            int num = (int)reader.ReadPackedUInt32();
            if((num & 1) != 0)
            {
                this.countdown = reader.ReadSingle();
                this.resetCountdown = reader.ReadBoolean();
                this.forcedEnding = reader.ReadBoolean();
                this.shouldExpire = reader.ReadBoolean();
            }
        }
    }
}
