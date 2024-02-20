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
        [UIHint(UIHint.Variable)]
        public FsmBool hasStateBool;
        public FsmEvent hasStateEvent;

        public override void OnEnter()
        {
            Playroom.PlayroomKit.WaitForState(stateKey.Value, FoundState);
        }

        private void FoundState()
        {
            //Debug.LogWarning("Found State: " + stateKey.Value);
            //Debug.LogWarning(Playroom.PlayroomKit.GetState<float>(stateKey.Value));
            hasStateBool.Value = true;
            Fsm.Event(hasStateEvent);
            Finish();

        }


    }

}
