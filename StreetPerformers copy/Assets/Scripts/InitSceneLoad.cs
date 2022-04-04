using UnityEngine;
using UnityEngine.SceneManagement;

public class InitSceneLoad : MonoBehaviour
{
    [SerializeField] private Texture2D _cursorImage = null;

    private void Start()
    {
        //Cursor.SetCursor(_cursorImage, Vector2.zero, CursorMode.Auto);

        #if UNITY_STANDALONE
            bool init = SteamManager.Initialized;
            SceneManager.LoadScene("TitleSceneLandscape");
        #elif UNITY_IPHONE || UNITY_ANDROID
            SceneManager.LoadScene("TitleScene");
        #endif
    }
}
