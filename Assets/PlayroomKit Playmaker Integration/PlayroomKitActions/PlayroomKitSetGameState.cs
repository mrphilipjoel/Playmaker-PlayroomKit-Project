namespace GooglyEyesGames.PlaymakerIntegrations.PlayroomKit.Actions
{
    using UnityEngine;
    using HutongGames.PlayMaker;
	using Tooltip = HutongGames.PlayMaker.TooltipAttribute;
    using HutongGames.PlayMaker.Actions;
    using static HutongGames.PlayMaker.FsmEventTarget;

    [ActionCategory("PlayroomKit")]
	public class PlayroomKitSetGameState : FsmStateAction
	{
		public FsmString stateKey;

        [RequiredField]
        [Tooltip("Variable type. Must be bool, float, int, or string.")]
        public FsmVar variable;

        public FsmBool reliable;

        public override void OnEnter()
		{

			switch (variable.Type)
			{
				case VariableType.Float:
                    Playroom.PlayroomKit.SetState(stateKey.Value, variable.floatValue, reliable.Value);
                    break;
				case VariableType.Int:
                    Playroom.PlayroomKit.SetState(stateKey.Value, variable.intValue, reliable.Value);
                    break;
				case VariableType.Bool:
                    Playroom.PlayroomKit.SetState(stateKey.Value, variable.boolValue, reliable.Value);
                    break;
				case VariableType.String:
                    Playroom.PlayroomKit.SetState(stateKey.Value, variable.stringValue, reliable.Value);
                    break;
				default:
                    Debug.LogError("Variable type MUST be float, int, bool, or string!");
					break;
			}
            Finish();
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
