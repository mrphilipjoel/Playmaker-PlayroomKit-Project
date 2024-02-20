namespace GooglyEyesGames.PlaymakerIntegrations.PlayroomKit.Actions
{
    using UnityEngine;
    using HutongGames.PlayMaker;
	using Tooltip = HutongGames.PlayMaker.TooltipAttribute;
    using HutongGames.PlayMaker.Actions;
    using static HutongGames.PlayMaker.FsmEventTarget;

    [ActionCategory("PlayroomKit")]
	public class PlayroomKitSetGameStateBool : FsmStateAction
	{
        [RequiredField]
        public FsmString stateKey;

        [RequiredField]
        public FsmBool boolValue;

        public FsmBool reliable;

        public override void OnEnter()
        {

            Playroom.PlayroomKit.SetState(stateKey.Value, boolValue.Value, reliable.Value);
            Finish();
        }

    }

}
