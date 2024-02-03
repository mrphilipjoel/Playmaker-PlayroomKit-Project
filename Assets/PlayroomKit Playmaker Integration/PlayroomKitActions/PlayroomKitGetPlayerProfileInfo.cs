namespace GooglyEyesGames.PlaymakerIntegrations.PlayroomKit.Actions
{
    using UnityEngine;
    using HutongGames.PlayMaker;
    using static Playroom.PlayroomKit;

    [ActionCategory("PlayroomKit")]
	public class PlayroomKitGetPlayerProfileInfo : FsmStateAction
	{
		public FsmString playerId;

        [ActionSection("Results")]

        [UIHint(UIHint.Variable)]
        public FsmString playerName;

        [UIHint(UIHint.Variable)]
        public FsmString playerPhotoUrl;

        [UIHint(UIHint.Variable)]
        public FsmColor playerColor;
        public override void OnEnter()
		{
            Playroom.PlayroomKit.Player player = PlayroomKitManager.Instance.GetPlayer(playerId.Value);
            if (player != null)
            {
                Player.Profile profile = player.GetProfile();
                playerName.Value = profile.name;
                playerPhotoUrl.Value = profile.photo;
                playerColor.Value = profile.color;
            }

            Finish();
		}


	}

}
