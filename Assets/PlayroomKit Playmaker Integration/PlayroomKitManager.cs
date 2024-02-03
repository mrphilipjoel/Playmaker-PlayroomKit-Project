namespace GooglyEyesGames.PlaymakerIntegrations.PlayroomKit
{
    using UnityEngine;
    using Playroom;
    using System.Collections.Generic;
    using HutongGames.PlayMaker;
    using static Playroom.PlayroomKit;
    using AOT;
    using System;

    public class PlayroomKitManager : MonoBehaviour
    {
        public static PlayroomKitManager Instance { get; private set; }

        private static Dictionary<string, GameObject> PlayerObjectDictionary = new();
        private static Dictionary<string, PlayroomKit.Player> PlayerReferenceDictionary = new();

        [Header("PlayroomKit Options")]
        public bool streamMode = false;
        public bool allowGamepads = false;
        public int maxPlayersPerRoom = 4;
        public bool skipLobby = false;
        public int reconnectGracePeriodInSeconds = 30;
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }


#if UNITY_EDITOR
            streamMode = true;
            int randomInt = UnityEngine.Random.Range(1111, 9999);
#endif
            PlayroomKit.InitOptions options = new PlayroomKit.InitOptions
            {
#if UNITY_EDITOR
                roomCode = randomInt.ToString(),
#endif
                streamMode = streamMode,
                allowGamepads = allowGamepads,
                maxPlayersPerRoom = maxPlayersPerRoom,
                skipLobby = skipLobby,
                reconnectGracePeriod = reconnectGracePeriodInSeconds * 1000,
            };

#if !UNITY_EDITOR && UNITY_WEBGL
WebGLInput.captureAllKeyboardInput = false;

#endif
            PlayroomKit.InsertCoin(() =>
            {
                PlayroomKit.OnPlayerJoin(PlaymakerOnPlayerJoin);
                PlayMakerFSM.BroadcastEvent("PlayroomKit/InsertCoin");


#if !UNITY_EDITOR && UNITY_WEBGL
            WebGLInput.captureAllKeyboardInput = true;
#endif
            });


        }

        private void PlaymakerOnPlayerJoin(PlayroomKit.Player player)
        {
            player.OnQuit(RemovePlayer);
            PlayerReferenceDictionary.Add(player.id, player);
            Fsm.EventData.StringData = player.id;
            PlayMakerFSM.BroadcastEvent("PlayroomKit/OnPlayerJoin");
        }

        public void AddPlayer(string playerId, GameObject playerObject)
        {
            PlayerObjectDictionary.Add(playerId, playerObject);
            
        }

        [MonoPInvokeCallback(typeof(Action<string>))]
        private static void RemovePlayer(string playerID)
        {
            if (PlayerObjectDictionary.TryGetValue(playerID, out GameObject player))
            {
                Destroy(player);
            }
            else
            {
                Debug.LogWarning("Player not in dictionary!");
            }

        }

        public PlayroomKit.Player GetPlayer(string playerID)
        {
            if (PlayerReferenceDictionary.TryGetValue(playerID, out PlayroomKit.Player player))
            {
                return player;
            }
            else
            {
                Debug.LogWarning("PlayerID not in dictionary!");
                return null;
            }
        }
    }
}

