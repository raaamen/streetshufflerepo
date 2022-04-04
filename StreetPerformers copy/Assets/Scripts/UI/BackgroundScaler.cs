using UnityEngine;

namespace StreetPerformers
{
    /// <summary>
    /// Scales the size of the attached image so the width matches the height.
    /// Also scales the children of the object by the same amount the width got scaled.
    /// </summary>
    public class BackgroundScaler : MonoBehaviour
    {
        void Start()
        {
            RectTransform rect = GetComponent<RectTransform>();

#if UNITY_ANDROID || UNITY_IOS
            float origWidth = rect.rect.width;
            rect.sizeDelta = new Vector2(rect.rect.height, rect.rect.height);
            rect.offsetMax = new Vector2(rect.offsetMax.x, 0);
            rect.offsetMin = new Vector2(rect.offsetMin.x, 0);

            float scale = rect.rect.width / origWidth;
            
#elif UNITY_STANDALONE
            rect.localScale = new Vector2(rect.localScale.x, rect.localScale.x);
            rect.sizeDelta = new Vector2(rect.sizeDelta.x, rect.sizeDelta.x);

            //float scale = rect.rect.height / origHeight;
#endif
            for(int i = 0; i < transform.childCount; i++)
            {
                //transform.GetChild(i).localScale = transform.GetChild(i).localScale * scale;
            }
        }
    }
}