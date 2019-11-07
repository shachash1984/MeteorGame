using UnityEngine;
using UnityEngine.UI;

public class LocalizedText : MonoBehaviour
{
    public string LocalizationKey;
    public bool ChangeAlignment;

    void Start()
    {
        Text text = GetComponent<Text>();
        text.text = LocalizationManager.GetLocalizedText(LocalizationKey);
        if (ChangeAlignment && LocalizationManager.IsRTL())
        {
            switch (text.alignment)
            {
                case TextAnchor.LowerLeft: text.alignment = TextAnchor.LowerRight; break;
                case TextAnchor.LowerRight: text.alignment = TextAnchor.LowerLeft; break;

                case TextAnchor.MiddleLeft: text.alignment = TextAnchor.MiddleRight; break;
                case TextAnchor.MiddleRight: text.alignment = TextAnchor.MiddleLeft; break;

                case TextAnchor.UpperLeft: text.alignment = TextAnchor.UpperRight; break;
                case TextAnchor.UpperRight: text.alignment = TextAnchor.UpperLeft; break;
            }
        }
    }
}