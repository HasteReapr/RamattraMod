using RoR2;
using RoR2.Projectile;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

namespace RamattraMod.Modules.Misc
{
    internal class VortexProjectileController : NetworkBehaviour
    {
        private float groundingTimer;
        private float groundingInterval = 0.5f;
        public float width;
        public float height;
        private TeamFilter teamFilter;

        private void Awake()
        {
            //set our current team
            this.teamFilter = base.GetComponent<TeamFilter>();
            Chat.AddMessage($"{this.teamFilter}");
            //set our height and width
            if (base.GetComponent<BoxCollider>() != null)
            {
                width = base.GetComponent<BoxCollider>().size.x;
                height = base.GetComponent<BoxCollider>().size.y;
            }
        }

        private void Start()
        {
            if (NetworkServer.active)
            {
                this.Ground(TeamComponent.GetTeamMembers(this.teamFilter.teamIndex), base.transform.position);
            }
        }

        private void FixedUpdate()
        {
            if (NetworkServer.active)
            {
                this.groundingTimer -= Time.fixedDeltaTime;
                if (this.groundingTimer <= 0f)
                {
                    for (TeamIndex teamIndex = TeamIndex.Neutral; teamIndex < TeamIndex.Count; teamIndex += 1)
                    {
                        if (teamIndex != this.teamFilter.teamIndex)
                        {
                            this.Ground(TeamComponent.GetTeamMembers(teamIndex), base.transform.position);
                        }
                    }

                    this.groundingTimer = this.groundingInterval;
                    return;
                }
            }
        }

        private void Ground(IEnumerable<TeamComponent> victims, Vector3 curPos)
        {
            if (!NetworkServer.active) return;

            float checkX = this.width / 2;//curPos.x - (this.width / 2);
            float checkY = this.height / 2; //curPos.y - (this.height / 2);
            float checkZ = this.width / 2;//curPos.x - (this.width / 2);

            foreach (TeamComponent teamComponent in victims)
            {
                Vector3 checkPos = teamComponent.transform.position - curPos;
                //Chat.AddMessage($"X proj pos {checkX} X victim pos {checkPos.x}");
                //Chat.AddMessage($"Y proj pos {checkY} Y victim pos {checkPos.y}");
                //Chat.AddMessage($"Z proj pos {checkZ} Z victim pos {checkPos.z}");
                if (checkPos.x <= checkX && checkPos.y <= checkY && checkPos.z <= checkZ)
                {
                    Chat.AddMessage("Victim within the volume of the cube.");
                    CharacterBody victim = teamComponent.body;
                    if (victim)
                    {
                        victim.AddTimedBuff(Modules.Buffs.vortexDebuff, 0.5f);
                        if (victim.GetComponent<Misc.VortexGrounding>() == null) victim.gameObject.AddComponent<Misc.VortexGrounding>().body = victim;
                    }
                }
            }
        }
    }
}
