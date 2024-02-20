namespace GooglyEyesGames.PlaymakerIntegrations.PlayroomKit.Actions
{
    using UnityEngine;
    using HutongGames.PlayMaker;
	using Tooltip = HutongGames.PlayMaker.TooltipAttribute;
    using HutongGames.PlayMaker.Actions;
    using static HutongGames.PlayMaker.FsmEventTarget;

    [ActionCategory("PlayroomKit")]
	public class PlayroomKitSetGameStateInt : FsmStateAction
	{
        [RequiredField]
        public FsmString stateKey;

        [RequiredField]
        public FsmInt intValue;

        public FsmBool reliable;

        public override void OnEnter()
        {

            Playroom.PlayroomKit.SetState(stateKey.Value, intValue.Value, reliable.Value);
            Finish();
        }

    }

}
