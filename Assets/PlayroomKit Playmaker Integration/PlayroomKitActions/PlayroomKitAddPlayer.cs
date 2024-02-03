namespace GooglyEyesGames.PlaymakerIntegrations.PlayroomKit.Actions
{
    using UnityEngine;
    using HutongGames.PlayMaker;

    [ActionCategory("PlayroomKit")]
	public class PlayroomKitAddPlayer : FsmStateAction
	{
		public FsmString playerId;
		public FsmGameObject playerPrefab;
		[UIHint(UIHint.Variable)]
		public FsmGameObject spawnedPlayer;

		public override void OnEnter()
		{
			spawnedPlayer.Value = GameObject.Instantiate(playerPrefab.Value);
			PlayroomKitManager.Instance.AddPlayer(playerId.Value, spawnedPlayer.Value);
			Finish();
		}


	}

}
