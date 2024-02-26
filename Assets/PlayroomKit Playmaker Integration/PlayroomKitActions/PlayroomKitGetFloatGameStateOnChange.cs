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
	public class PlayroomKitGetFloatGameStateOnChange : FsmStateAction
	{
		public FsmString stateKey;

        [UIHint(UIHint.Variable)]
        [RequiredField]
        public FsmFloat result;

        public FsmBool reliable;
        //public bool everyFrame;
        public FsmBool hasChangedBool;

        public FsmEvent onChangedEvent;


        private bool firstCheck = true;
        private float firstResult = 0;


        public override void OnUpdate()
        {
            if (!firstCheck)
            {
                result.Value = Playroom.PlayroomKit.GetState<float>(stateKey.Value);
                if (result.Value != firstResult)
                {
                    hasChangedBool.Value = true;
                    Fsm.Event(onChangedEvent);
                }
                else
                {
                    hasChangedBool.Value = false;
                }
            }
        }

        public override void OnEnter()
        {
            hasChangedBool.Value = false;
            firstCheck = true;
            result.Value = Playroom.PlayroomKit.GetState<float>(stateKey.Value);
            firstResult = result.Value;
            firstCheck = false;
        }

    }

}
