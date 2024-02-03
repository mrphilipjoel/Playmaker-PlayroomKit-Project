namespace GooglyEyesGames.PlaymakerIntegrations.PlayroomKit.Actions
{
    using UnityEngine;
    using HutongGames.PlayMaker;
    using static Playroom.PlayroomKit;
    using System.Collections.Generic;
    using System.IO;
    using System.Web;
    using Unity.VectorGraphics;
    using System.Collections;
    using Tooltip = HutongGames.PlayMaker.TooltipAttribute;

    [ActionCategory("PlayroomKit")]
	public class PlayroomKitGetPlayerProfileInfo : FsmStateAction
	{
		public FsmString playerId;

        [ActionSection("Results")]

        [UIHint(UIHint.Variable)]
        public FsmString playerName;

        [Tooltip("The source sprite of the UI Image component.")]
        [ObjectType(typeof(Sprite))]
        public FsmObject playerPhotoSprite;

        [UIHint(UIHint.Variable)]
        public FsmColor playerColor;

        [ActionSection("Events")]
        public FsmEvent success;
        
        public override void OnEnter()
		{
            Playroom.PlayroomKit.Player player = PlayroomKitManager.Instance.GetPlayer(playerId.Value);
            if (player != null)
            {
                Player.Profile profile = player.GetProfile();
                playerName.Value = profile.name;
                playerColor.Value = profile.color;
                StartCoroutine(GenerateSprite(profile));
            }
            else
            {
                Debug.Log("player is null.");
                Finish();
            }

            
		}

        private IEnumerator GenerateSprite(Player.Profile profile)
        {
            yield return new WaitUntil(() => profile.photo != null);
            if (profile.photo.StartsWith("data:image/svg+xml"))
            {
                string svgBytes = profile.photo.Substring("data:image/svg+xml".Length);
                // Split the string to escape the real data
                svgBytes = svgBytes.Split(",".ToCharArray(), 2)[1];
                // Decode from the URL encoding
                svgBytes = HttpUtility.UrlDecode(svgBytes);
                VectorUtils.TessellationOptions tesselationOptions = new VectorUtils.TessellationOptions();

                using (StringReader reader = new StringReader(svgBytes))
                {
                    SVGParser.SceneInfo sceneInfo = SVGParser.ImportSVG(reader);
                    tesselationOptions.MaxCordDeviation = float.MaxValue;
                    tesselationOptions.MaxTanAngleDeviation = float.MaxValue;
                    tesselationOptions.StepDistance = 1f;
                    tesselationOptions.SamplingStepSize = 0.1f;

                    List<VectorUtils.Geometry> geoms =
                    VectorUtils.TessellateScene(sceneInfo.Scene, tesselationOptions);

                    // Build a sprite with the tessellated geometry.
                    Sprite sprite = VectorUtils.BuildSprite(geoms, 100.0f, VectorUtils.Alignment.Center, Vector2.zero, 128, true);
                    Debug.Log(sprite);
                    sprite.name = "SVGimage";
                    playerPhotoSprite.Value = sprite;
                }
            }
            else
            {
                Debug.LogError("Invalid SVG string format. Must start with 'data:image/svg+xml'");
            }
            Fsm.Event(success);
            Finish();

        }

}}
