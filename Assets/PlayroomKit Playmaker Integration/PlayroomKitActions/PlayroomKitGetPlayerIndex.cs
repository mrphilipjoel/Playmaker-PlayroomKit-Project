namespace GooglyEyesGames.PlaymakerIntegrations.PlayroomKit.Actions
{
    using UnityEngine;
    using HutongGames.PlayMaker;

    [ActionCategory("PlayroomKit")]
	public class PlayroomKitGetPlayerIndex : FsmStateAction
	{
		public FsmString playerId;
		[UIHint(UIHint.Variable)]
		public FsmInt playerIndex;

		public override void OnEnter()
		{
			playerIndex = PlayroomKitManager.Instance.GetPlayerIndex(playerId.Value);
			Finish();
		}


	}

}
