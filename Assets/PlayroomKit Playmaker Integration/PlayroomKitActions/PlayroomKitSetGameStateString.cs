namespace GooglyEyesGames.PlaymakerIntegrations.PlayroomKit.Actions
{
    using UnityEngine;
    using HutongGames.PlayMaker;
	using Tooltip = HutongGames.PlayMaker.TooltipAttribute;
    using HutongGames.PlayMaker.Actions;
    using static HutongGames.PlayMaker.FsmEventTarget;

    [ActionCategory("PlayroomKit")]
	public class PlayroomKitSetGameStateString : FsmStateAction
	{
        [RequiredField]
        public FsmString stateKey;

        [RequiredField]
        public FsmString stringValue;

        public FsmBool reliable;

        public override void OnEnter()
        {

            Playroom.PlayroomKit.SetState(stateKey.Value, stringValue.Value, reliable.Value);
            Finish();
        }

    }

}
