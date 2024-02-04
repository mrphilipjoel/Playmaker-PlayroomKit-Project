namespace GooglyEyesGames.PlaymakerIntegrations.PlayroomKit.Actions
{
    using UnityEngine;
    using HutongGames.PlayMaker;

    [ActionCategory("PlayroomKit")]
	public class PlayroomKitIsHostTest : FsmStateAction
	{
		[ActionSection("Results")]

		[UIHint(UIHint.Variable)]
		public FsmBool isHost;

		[ActionSection("Events")]
		public FsmEvent isHostEvent;
		public FsmEvent notHostEvent;
		public override void OnEnter()
		{
			isHost.Value = Playroom.PlayroomKit.IsHost();

			if (isHost.Value)
			{
				Fsm.Event(isHostEvent);
			}
			else
			{
				Fsm.Event(notHostEvent);
			}
			Finish();
		}


	}

}
