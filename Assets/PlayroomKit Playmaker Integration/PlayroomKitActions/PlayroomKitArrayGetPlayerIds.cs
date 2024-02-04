namespace GooglyEyesGames.PlaymakerIntegrations.PlayroomKit.Actions
{
    using UnityEngine;
    using HutongGames.PlayMaker;
    using Tooltip = HutongGames.PlayMaker.TooltipAttribute;

    [ActionCategory("PlayroomKit")]
	public class PlayroomKitArrayGetPlayerIds : FsmStateAction
	{
        [RequiredField]
        [UIHint(UIHint.Variable)]
        [ArrayEditor(VariableType.String)]
        [Tooltip("The Array Variable to use.")]
        public FsmArray array;

        public override void OnEnter()
		{
            foreach (var id in Playroom.PlayroomKit.GetPlayers().Keys)
            {
                DoAddValue(id);
            }
			Finish();
		}

        private void DoAddValue(string playerId)
        {
            array.Resize(array.Length + 1);
            array.Set(array.Length - 1, playerId);
        }

    }

}
