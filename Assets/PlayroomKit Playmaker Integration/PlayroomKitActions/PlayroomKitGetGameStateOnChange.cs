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
	public class PlayroomKitGetGameStateOnChange : FsmStateAction
	{
		public FsmString stateKey;

        [UIHint(UIHint.Variable)]
        [RequiredField]
        [Tooltip("Variable type. Must be bool, float, int, or string.")]
        public FsmVar variable;

        public FsmBool reliable;
        //public bool everyFrame;
        public FsmBool hasChangedBool;

        public FsmEvent onChangedEvent;

        private Fsm fromFsm;
        private float floatCache;
        private int intCache;
        private bool boolCache;
        private string stringCache;

        private bool firstCheck = true;

        private void GetTheState()
        {
            switch (variable.Type)
            {

                case VariableType.Float:
                    FsmFloat _floatTarget = fromFsm.Variables.GetFsmFloat(variable.variableName);
                    _floatTarget.Value = Playroom.PlayroomKit.GetState<float>(stateKey.Value);
                    if (firstCheck)
                    {
                        firstCheck = false;
                        floatCache = _floatTarget.Value;
                    }
                    else
                    {
                        if (_floatTarget.Value != floatCache)
                        {
                            hasChangedBool.Value = true;
                            Fsm.Event(onChangedEvent);
                        }
                    }
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

        }

        public override void OnUpdate()
        {
            GetTheState();
        }

        public override void OnEnter()
        {
            firstCheck = true;
            hasChangedBool.Value = false;
            fromFsm = Fsm;
            GetTheState();
            /*if (!everyFrame)
            {
                Finish();
            }*/
            
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
