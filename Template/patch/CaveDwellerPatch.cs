using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace LoganC.patch;

    [HarmonyPatch(typeof(CaveDwellerAI))]
    public class CaveDwellerPatch
    {
        [HarmonyPatch(nameof(CaveDwellerAI.DoAIIntervalChaseLogic))]
        [HarmonyPrefix]
        private static void DoAIIntervalChaseLogic(CaveDwellerAI caveDwellerAI)
    {   
        
        if (caveDwellerAI.inSpecialAnimation || caveDwellerAI.inKillAnimation)
            {
                return;
            }
            bool flag = caveDwellerAI.TargetClosestPlayer(4f);
            if (caveDwellerAI.currentBehaviourStateIndex == 3)
            {
                if (caveDwellerAI.searchRoutine.inProgress)
                {
                caveDwellerAI.StopSearch(caveDwellerAI.searchRoutine);
                }
                if (caveDwellerAI.TargetClosestPlayer())
                {
                if (caveDwellerAI.targetPlayer.playerClientId != caveDwellerAI.playerMemory[0].playerId) {
                    caveDwellerAI.noPlayersTimer = 0f;
                    caveDwellerAI.SetMovingTowardsTargetPlayer(caveDwellerAI.targetPlayer);
                }
                }
                else if (caveDwellerAI.noPlayersTimer > 2.5f)
                {
                caveDwellerAI.SwitchToBehaviourState(1);
                }
                else
                {
                caveDwellerAI.noPlayersTimer += caveDwellerAI.AIIntervalTime;
                }
                return;
            }
        caveDwellerAI.fakeCryTimer -= caveDwellerAI.AIIntervalTime;
            if (caveDwellerAI.isFakingBabyVoice)
            {
                if (caveDwellerAI.fakeCryTimer <= 0f)
                {
                caveDwellerAI.fakeCryTimer = Mathf.Max(UnityEngine.Random.Range(-2f, 7f), 4.2f);
                caveDwellerAI.clickingMandibles = false;
                    int num = UnityEngine.Random.Range(0, caveDwellerAI.fakeCrySFX.Length);
                caveDwellerAI.DoFakeCryLocalClient(num);
                caveDwellerAI.MakeFakeCryServerRpc(num);
                }
            }
            else if (caveDwellerAI.fakeCryTimer <= 0f)
            {
                if (UnityEngine.Random.Range(0, 100) < 65)
                {
                caveDwellerAI.fakeCryTimer = 9f;
                caveDwellerAI.clickingMandibles = true;
                caveDwellerAI.SetClickingMandiblesServerRpc();
                }
                else
                {
                caveDwellerAI.isFakingBabyVoice = true;
                caveDwellerAI.SetFakingBabyVoiceServerRpc(fakingBabyVoice: true);
                }
            }
            if (!flag)
            {
                if (caveDwellerAI.noPlayersTimer > 2.5f)
                {
                caveDwellerAI.RoamAroundCaveSpot(gotTarget: false);
                }
                else
                {
                caveDwellerAI.noPlayersTimer += caveDwellerAI.AIIntervalTime;
                }
                return;
            }
        caveDwellerAI.noPlayersTimer = 0f;
            float num2 = ((!caveDwellerAI.isOutside) ? Vector3.Distance(caveDwellerAI.targetPlayer.transform.position, caveDwellerAI.caveHidingSpot) : Vector3.Distance(caveDwellerAI.targetPlayer.transform.position, caveDwellerAI.transform.position));
            float num3 = caveDwellerAI.attackDistance;
            if (caveDwellerAI.isOutside)
            {
                num3 += 16f;
            }
            float num4 = Vector3.Distance(caveDwellerAI.targetPlayer.transform.position, caveDwellerAI.transform.position);
            if ((num4 < num3 && !Physics.Linecast(caveDwellerAI.transform.position + Vector3.up * 0.25f, caveDwellerAI.targetPlayer.transform.position + Vector3.up * 0.25f, StartOfRound.Instance.collidersAndRoomMaskAndDefault, QueryTriggerInteraction.Ignore)) || num4 < 4.5f)
            {
            caveDwellerAI.SwitchToBehaviourState(3);
            }
            else if (caveDwellerAI.pursuingPlayerInSneakMode)
            {
                if (num2 > caveDwellerAI.searchRoutine.searchWidth + 30f)
                {
                caveDwellerAI.pursuingPlayerInSneakMode = false;
                }
                if (caveDwellerAI.searchRoutine.inProgress)
                {
                caveDwellerAI.StopSearch(caveDwellerAI.searchRoutine);
                }
                if (caveDwellerAI.currentBehaviourStateIndex == 1)
                {
                caveDwellerAI.SwitchToBehaviourState(2);
                }
                if (caveDwellerAI.isFakingBabyVoice && caveDwellerAI.targetPlayer.HasLineOfSightToPosition(caveDwellerAI.transform.position + Vector3.up * 0.75f, 90f, 20, 2f))
                {
                caveDwellerAI.isFakingBabyVoice = false;
                caveDwellerAI.fakeCryTimer = 18f;
                caveDwellerAI.SetFakingBabyVoiceServerRpc(fakingBabyVoice: false);
                    if (UnityEngine.Random.Range(0, 100) < 30)
                    {
                    caveDwellerAI.clickingMandibles = true;
                    caveDwellerAI.SetClickingMandiblesServerRpc();
                    }
                }
            caveDwellerAI.ChooseClosestNodeToPlayer();
            }
            else
            {
                if (num2 < caveDwellerAI.searchRoutine.searchWidth + 15f)
                {
                caveDwellerAI.pursuingPlayerInSneakMode = true;
                }
            caveDwellerAI.RoamAroundCaveSpot(gotTarget: true);
            }
        }

}

