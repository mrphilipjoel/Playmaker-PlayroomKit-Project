using UnityEngine;
using UnityEngine.UI;
using HutongGames.PlayMaker;
using Unity.VectorGraphics;

namespace GooglyEyesGames.PlayMaker.Actions // Replace YourNamespace with your actual namespace
{
    public class SetSVGImageSprite : FsmStateAction
    {
        [RequiredField]
        [CheckForComponent(typeof(SVGImage))]
        public FsmOwnerDefault gameObject;

        [ObjectType(typeof(Sprite))]
        public FsmObject sprite;

        public override void Reset()
        {
            gameObject = null;
            sprite = null;
        }

        public override void OnEnter()
        {
            var go = Fsm.GetOwnerDefaultTarget(gameObject);
            if (go == null)
            {
                Finish();
                return;
            }

            var svgImage = go.GetComponent<SVGImage>();
            if (svgImage != null && sprite.Value != null && sprite.Value is Sprite)
            {
                svgImage.sprite = sprite.Value as Sprite;
            }

            Finish();
        }
    }
}
