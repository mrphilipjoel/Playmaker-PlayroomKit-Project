namespace GooglyEyesGames.PlaymakerIntegrations.PlayroomKit
{
    using UnityEngine;
    using Playroom;

    public class PlayroomKitManager : MonoBehaviour
    {
        public static PlayroomKitManager Instance { get; private set; }

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
            int randomInt = Random.Range(1111, 9999);
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
                PlayMakerFSM.BroadcastEvent("PlayroomKit/InsertCoin");


#if !UNITY_EDITOR && UNITY_WEBGL
            WebGLInput.captureAllKeyboardInput = true;
#endif
            });
/*            void Start()
            {
                PlayroomKit.InsertCoin(() =>
                {
                    PlayMakerFSM.BroadcastEvent("PlayroomKit/InsertCoin");


#if !UNITY_EDITOR && UNITY_WEBGL
            WebGLInput.captureAllKeyboardInput = true;
#endif
                });
            }*/

        }
    }
}

