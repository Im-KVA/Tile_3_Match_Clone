using UnityEngine;
using UnityEngine.UI;

public class LanguageSelector : MonoBehaviour
{
    [SerializeField] private GameObject _popupLanguage;
    [SerializeField] private Button _btnLanguage;
    [SerializeField] private Button[] _flagButtons;
    [SerializeField] private Image _selectedFlag;

    private void Start()
    {
        _popupLanguage.SetActive(false);

        _btnLanguage.onClick.AddListener(TogglePopup);

        foreach (Button flag in _flagButtons)
        {
            flag.onClick.AddListener(() => SelectFlag(flag));
        }
    }

    private void TogglePopup()
    {
        _popupLanguage.SetActive(!_popupLanguage.activeSelf);
    }

    private void SelectFlag(Button flag)
    {
        _selectedFlag.sprite = flag.GetComponent<Image>().sprite;
        _popupLanguage.SetActive(false);
    }
}
