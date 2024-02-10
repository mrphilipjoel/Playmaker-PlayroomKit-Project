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
	public class PlayroomKitGetGameState : FsmStateAction
	{
		public FsmString stateKey;

        [UIHint(UIHint.Variable)]
        [RequiredField]
        [Tooltip("Variable type. Must be bool, float, int, or string.")]
        public FsmVar variable;

        public FsmBool reliable;
        public bool everyFrame;

        private Fsm fromFsm;

        private void GetTheState()
        {
            switch (variable.Type)
            {

                case VariableType.Float:
                    FsmFloat _floatTarget = fromFsm.Variables.GetFsmFloat(variable.variableName);
                    _floatTarget.Value = Playroom.PlayroomKit.GetState<float>(stateKey.Value);
                    break;
                case VariableType.Int:
                    FsmInt _intTarget = fromFsm.Variables.GetFsmInt(variable.variableName);
                    _intTarget.Value = Playroom.PlayroomKit.GetState<int>(stateKey.Value);
                    break;
                case VariableType.Bool:
                    FsmBool _boolTarget = fromFsm.Variables.GetFsmBool(variable.variableName);
                    _boolTarget.Value = Playroom.PlayroomKit.GetState<bool>(stateKey.Value);
                    break;
                case VariableType.String:
                    FsmString _stringTarget = fromFsm.Variables.GetFsmString(variable.variableName);
                    _stringTarget.Value = Playroom.PlayroomKit.GetState<string>(stateKey.Value);
                    break;
                default:
                    Debug.LogError("Variable type MUST be float, int, bool, or string!");
                    break;
            }

            Debug.Log("Attempted to UpdateValue of variable: " + variable);
        }

        public override void OnUpdate()
        {
            GetTheState();
        }

        public override void OnEnter()
        {
            fromFsm = Fsm;
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
