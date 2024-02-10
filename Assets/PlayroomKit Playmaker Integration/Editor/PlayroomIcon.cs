using UnityEngine;
using HutongGames.PlayMakerEditor;
using UnityEditor;

[InitializeOnLoad]
public class PlayroomIcon : MonoBehaviour
{
    static PlayroomIcon()
    {
        #region CategoryList

        Actions.AddCategoryIcon("PlayroomKit", PlayroomKitIconImage);

        #endregion
    }

    private static Texture sPlayroomKitIcon = null;
    internal static Texture PlayroomKitIconImage
    {
        get
        {
            if (sPlayroomKitIcon == null)
                sPlayroomKitIcon = Resources.Load<Texture>("PlayroomKitIconImage");
            ;
            if (sPlayroomKitIcon != null)
                sPlayroomKitIcon.hideFlags = HideFlags.DontSaveInEditor;
            return sPlayroomKitIcon;
        }
    }
}
