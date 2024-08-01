using RoR2;
using UnityEngine;
using UnityEngine.Networking;
using RamattraMod.Survivors.Ramattra;

namespace RamattraMod.Survivors.Ramattra.Components
{
    internal class VortexGrounding : NetworkBehaviour
    {
        //directly pulled from https://github.com/ArcPh1r3/PaladinMod/blob/master/PaladinMod/Misc/PaladinGroundController.cs & modified to be more balanced with Ravenous Vortex
        public float lifetime = 0.5f;
        private float pushForce;
        public CharacterBody body;
        private float mass;
        private Rigidbody rb;
        private CharacterMotor motor;
        private DamageInfo info;
        private float stopwatch;

        private void Start()
        {
            if (this.body)
            {
                if (this.body.HasBuff(RammyBuffs.vortexDebuff))
                {
                    if (this.body.characterMotor)
                    {
                        this.motor = this.body.characterMotor;
                        this.mass = this.motor.mass;
                    }
                    else if (this.body.rigidbody)
                    {
                        this.rb = this.body.rigidbody;
                        this.mass = this.rb.mass;
                    }

                    this.stopwatch = 0;
                    this.lifetime = 0.5f;

                    if (this.mass < 1000f) this.mass = 1000f;
                    this.pushForce = 100f * this.mass;


                    this.info = new DamageInfo
                    {
                        attacker = null,
                        inflictor = null,
                        damage = 1,
                        damageColorIndex = DamageColorIndex.Default,
                        damageType = DamageType.Generic,
                        crit = false,
                        dotIndex = DotController.DotIndex.None,
                        force = Vector3.down * this.pushForce * Time.fixedDeltaTime,
                        position = base.transform.position,
                        procChainMask = default(ProcChainMask),
                        procCoefficient = 0
                    };
                }
            }
        }

        private void PullDown()
        {
            if (NetworkServer.active)
            {
                this.info = new DamageInfo
                {
                    attacker = null,
                    inflictor = null,
                    damage = 0,
                    damageColorIndex = DamageColorIndex.Default,
                    damageType = DamageType.Generic,
                    crit = false,
                    dotIndex = DotController.DotIndex.None,
                    force = Vector3.down * this.pushForce * Time.fixedDeltaTime,
                    position = base.transform.position,
                    procChainMask = default(ProcChainMask),
                    procCoefficient = 0
                };

                if (this.motor)
                {
                    this.body.healthComponent.TakeDamageForce(this.info);
                }
                else if (this.rb)
                {
                    this.body.healthComponent.TakeDamageForce(this.info);
                }
            }

            /*if (this.body)
            {
                this.body.characterMotor.velocity += new Vector3(0, -50, 0);
            }*/
        }

        private void FixedUpdate()
        {
            stopwatch += Time.fixedDeltaTime;
            if (stopwatch >= lifetime || !this.body.HasBuff(RammyBuffs.vortexDebuff))
            {
                Destroy(this);
                return;
            }

            PullDown();
        }
    }
}
