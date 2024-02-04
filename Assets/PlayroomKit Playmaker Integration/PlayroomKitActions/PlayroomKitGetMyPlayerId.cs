namespace GooglyEyesGames.PlaymakerIntegrations.PlayroomKit.Actions
{
    using UnityEngine;
    using HutongGames.PlayMaker;

    [ActionCategory("PlayroomKit")]
	public class PlayroomKitGetMyPlayerId : FsmStateAction
	{
		[ActionSection("Results")]

		[UIHint(UIHint.Variable)]
		public FsmString myId;

		public override void OnEnter()
		{
			myId.Value = Playroom.PlayroomKit.Me().id;
			Finish();
		}


	}

}
