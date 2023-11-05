using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class UIMainMenu : MonoBehaviour
{
    private UIDocument _document;

    private VisualElement _credits;
    private VisualElement _menu;

    private void Awake()
    {
        _document = GetComponent<UIDocument>();

        _menu = _document.rootVisualElement.Q("Main");
        _credits = _document.rootVisualElement.Q("Credits");

        _menu.Q<Button>("Play").clickable.clicked += () => SceneManager.LoadScene("Game");
        
        _credits.Q<Button>("Back").clickable.clicked += () =>
        {
            _credits.style.display = DisplayStyle.None;
            _menu.style.display = DisplayStyle.Flex;
        };
        
        _menu.Q<Button>("CreditsBtn").clickable.clicked += () =>
        {
            _credits.style.display = DisplayStyle.Flex;
            _menu.style.display = DisplayStyle.None;
        };
    }
}
