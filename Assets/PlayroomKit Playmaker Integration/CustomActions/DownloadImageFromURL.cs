namespace GooglyEyesGames.PlayMaker.Actions
{
    using UnityEngine;
    using UnityEngine.Networking;
    using HutongGames.PlayMaker;
    using Tooltip = HutongGames.PlayMaker.TooltipAttribute;
    using System.Collections;

    [ActionCategory(ActionCategory.ScriptControl)]
    [Tooltip("Downloads an image from the specified URL and stores it as an FsmTexture.")]
    public class DownloadImageFromURL : FsmStateAction
    {
        [RequiredField]
        [Tooltip("The URL of the image to download.")]
        public FsmString imageUrl;

        [UIHint(UIHint.Variable)]
        [Tooltip("Store the downloaded image as a texture.")]
        public FsmTexture downloadedTexture;

        [UIHint(UIHint.Variable)]
        [Tooltip("The progress of the download as a percentage (0 to 100).")]
        public FsmFloat progress;

        [ActionSection("Events")]
        public FsmEvent complete;
        public FsmEvent error;
        public FsmString errorMessage;

        public override void Reset()
        {
            imageUrl = null;
            downloadedTexture = null;
            progress = null;
        }

        public override void OnEnter()
        {
            if (string.IsNullOrEmpty(imageUrl.Value))
            {
                Debug.LogError("Image URL is not specified.");
                Finish();
                return;
            }

            // Start the download coroutine
            StartCoroutine(DownloadImage(imageUrl.Value));
        }

        private IEnumerator DownloadImage(string url)
        {
            using (UnityWebRequest www = UnityWebRequestTexture.GetTexture(url))
            {
                // Set download handler to get the texture
                DownloadHandlerTexture downloadHandler = new DownloadHandlerTexture();
                www.downloadHandler = downloadHandler;

                // Send the request and track the progress
                AsyncOperation operation = www.SendWebRequest();
                while (!operation.isDone)
                {
                    // Update progress
                    progress.Value = operation.progress * 100f;

                    yield return null;
                }

                // Check for errors
                if (www.result != UnityWebRequest.Result.Success)
                {
                    errorMessage.Value = www.error;
                    Fsm.Event(error);
                    Finish();
                }
                else
                {
                    // Get the downloaded texture
                    Texture2D texture = downloadHandler.texture;

                    // Store the texture in the FsmTexture variable
                    downloadedTexture.Value = texture;

                    // Finish the action
                    Fsm.Event(complete);
                    Finish();
                }
            }
        }
    }
}
