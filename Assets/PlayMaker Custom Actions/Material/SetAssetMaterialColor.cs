using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.Material)]
    [Tooltip("Sets the color of a Material.")]
    public class SetAssetMaterialColor : FsmStateAction
    {
        [Tooltip("The material to set the color.")]
        public Material material;

        [RequiredField]
        [Tooltip("The color to set.")]
        public FsmColor color;

        [Tooltip("Repeat every frame.")]
        public bool everyFrame;

        public override void Reset()
        {
            material = null;
            color = Color.white;
            everyFrame = false;
        }

        public override void OnEnter()
        {
            DoSetMaterialColor();

            if (!everyFrame)
            {
                Finish();
            }
        }

        public override void OnUpdate()
        {
            DoSetMaterialColor();
        }

        void DoSetMaterialColor()
        {
            if (material == null)
            {
                Debug.LogWarning("Material is not assigned!");
                return;
            }

            material.color = color.Value;
        }
    }
}
