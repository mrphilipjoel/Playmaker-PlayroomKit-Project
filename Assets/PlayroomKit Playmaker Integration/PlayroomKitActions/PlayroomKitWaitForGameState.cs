namespace GooglyEyesGames.PlaymakerIntegrations.PlayroomKit.Actions
{
    using UnityEngine;
    using HutongGames.PlayMaker;
	using Tooltip = HutongGames.PlayMaker.TooltipAttribute;
    using HutongGames.PlayMaker.Actions;
    using static HutongGames.PlayMaker.FsmEventTarget;

    [ActionCategory("PlayroomKit")]
	public class PlayroomKitWaitForGameState : FsmStateAction
	{
		public FsmString stateKey;

        [ActionSection("Results")]
        public FsmBool hasStateBool;
        public FsmEvent hasStateEvent;

        public override void OnEnter()
        {
            Playroom.PlayroomKit.WaitForState(stateKey.Value, FoundState);

        }

        private void FoundState()
        {
            hasStateBool.Value = true;
            Fsm.Event(hasStateEvent);
            Finish();

        }


    }

}
