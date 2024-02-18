using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.Transform)]
    [Tooltip("Gets the direction from a specified GameObject to a world point.")]
    public class GetDirectionToPosition : FsmStateAction
    {
        [RequiredField]
        [Tooltip("The GameObject from which to measure the direction.")]
        public FsmOwnerDefault gameObject;

        [RequiredField]
        [Tooltip("The world point to which the direction is measured.")]
        public FsmVector3 targetPosition;

        [UIHint(UIHint.Variable)]
        [Tooltip("Store the resulting direction in a Vector3 variable.")]
        public FsmVector3 storeResult;

        public override void Reset()
        {
            gameObject = null;
            targetPosition = null;
            storeResult = null;
        }

        public override void OnEnter()
        {
            if (gameObject.OwnerOption == OwnerDefaultOption.UseOwner)
            {
                DoGetDirectionToPosition(Owner.transform.position);
            }
            else
            {
                DoGetDirectionToPosition(Fsm.GetOwnerDefaultTarget(gameObject).transform.position);
            }

            Finish();
        }

        void DoGetDirectionToPosition(Vector3 fromPosition)
        {
            Vector3 direction = (targetPosition.Value - fromPosition).normalized;

            storeResult.Value = direction;
        }
    }
}
