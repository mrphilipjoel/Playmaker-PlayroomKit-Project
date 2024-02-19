namespace GooglyEyesGames.PlaymakerIntegrations.PlayroomKit.Actions
{
    using UnityEngine;
    using HutongGames.PlayMaker;
	using Tooltip = HutongGames.PlayMaker.TooltipAttribute;
    using HutongGames.PlayMaker.Actions;
    using static HutongGames.PlayMaker.FsmEventTarget;

    [ActionCategory("PlayroomKit")]
	public class PlayroomKitSetGameStateFloat : FsmStateAction
	{
		public FsmString stateKey;

        [RequiredField]
        public FsmFloat floatValue;

        public FsmBool reliable;

        public override void OnEnter()
		{

            Playroom.PlayroomKit.SetState(stateKey.Value, floatValue.Value, reliable.Value);
            Finish();
        }

    }

}
