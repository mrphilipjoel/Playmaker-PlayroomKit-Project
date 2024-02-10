namespace GooglyEyesGames.PlaymakerIntegrations.PlayroomKit.Actions
{
    using UnityEngine;
    using HutongGames.PlayMaker;
	using Tooltip = HutongGames.PlayMaker.TooltipAttribute;
    using HutongGames.PlayMaker.Actions;
    using static HutongGames.PlayMaker.FsmEventTarget;
    using Unity.VisualScripting;

    [ActionCategory("PlayroomKit")]
	public class PlayroomKitGetGameState : FsmStateAction
	{
		public FsmString stateKey;

        [UIHint(UIHint.Variable)]
        [RequiredField]
        [Tooltip("Variable type. Must be bool, float, int, or string.")]
        public FsmVar variable;

        public FsmBool reliable;
        public bool everyFrame;

        private void GetTheState()
        {
            switch (variable.Type)
            {

                case VariableType.Float:
                    variable.floatValue = Playroom.PlayroomKit.GetState<float>(stateKey.Value);
                    variable.UpdateValue();
                    break;
                case VariableType.Int:
                    variable.intValue = Playroom.PlayroomKit.GetState<int>(stateKey.Value);
                    variable.UpdateValue();
                    break;
                case VariableType.Bool:
                    variable.boolValue = Playroom.PlayroomKit.GetState<bool>(stateKey.Value);
                    variable.UpdateValue();
                    break;
                case VariableType.String:
                    variable.stringValue = Playroom.PlayroomKit.GetState<string>(stateKey.Value);
                    variable.UpdateValue();
                    Debug.LogWarning(variable.stringValue);
                    break;
                default:
                    Debug.LogError("Variable type MUST be float, int, bool, or string!");
                    break;
            }




        }

        public override void OnUpdate()
        {
            GetTheState();
        }

        public override void OnEnter()
        {

            GetTheState();
            if (!everyFrame)
            {
                Finish();
            }
            
        }

#if UNITY_EDITOR

        public override string ErrorCheck()
        {
            switch (variable.Type)
            {
                case VariableType.Float:
                    return null;
                    break;
                case VariableType.Int:
                    return null;
                    break;
                case VariableType.Bool:
                    return null;
                    break;
                case VariableType.String:
                    return null;
                    break;
                default:
					return "Variable type MUST be float, int, bool, or string!";
                    break;
            }
        }
#endif

    }

}
