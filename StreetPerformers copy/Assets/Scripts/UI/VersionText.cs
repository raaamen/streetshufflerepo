using TMPro;
using UnityEngine;

namespace StreetPerformers
{
    public class VersionText : MonoBehaviour
    {
        private void Start()
        {
            GetComponent<TextMeshProUGUI>().text = "Version " + Application.version;
        }
    }
}
