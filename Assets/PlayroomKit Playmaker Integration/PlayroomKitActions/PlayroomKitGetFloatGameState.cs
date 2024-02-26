namespace GooglyEyesGames.PlaymakerIntegrations.PlayroomKit.Actions
{
    using UnityEngine;
    using HutongGames.PlayMaker;
	using Tooltip = HutongGames.PlayMaker.TooltipAttribute;
    using HutongGames.PlayMaker.Actions;
    using static HutongGames.PlayMaker.FsmEventTarget;
    using Unity.VisualScripting;
    using HutongGames.Utility;

    [ActionCategory("PlayroomKit")]
	public class PlayroomKitGetFloatGameState : FsmStateAction
	{
		public FsmString stateKey;

        [UIHint(UIHint.Variable)]
        [RequiredField]
        public FsmFloat result;

        public FsmBool everyFrame;

        private void GetGameState()
        {
            result.Value = Playroom.PlayroomKit.GetState<float>(stateKey.Value);
        }



        public override void OnUpdate()
        {
            if (everyFrame.Value)
            {
                GetGameState();
            }
        }

        public override void OnEnter()
        {
            GetGameState();
            if (!everyFrame.Value)
            {
                Finish();
            }
        }

    }

}
