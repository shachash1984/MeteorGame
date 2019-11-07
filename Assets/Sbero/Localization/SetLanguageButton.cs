using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SetLanguageButton : MonoBehaviour
{
    public SystemLanguage Language;

    void Start()
    {
        Button btn = GetComponent<Button>();
        btn.onClick.AddListener(OnClick);
    }

    void OnClick()
    {
        LocalizationManager.SetCurrentLanguage(Language);
        Scene scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.name);
    }
}
