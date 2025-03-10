using UnityEngine;
using UnityEngine.UI;

public class ButtonToggle : MonoBehaviour
{
    [SerializeField] private Image _iconImage;
    [SerializeField] private Sprite _onSprite;
    [SerializeField] private Sprite _offSprite;
    private bool _isOn = true;

    private void Start()
    {
        UpdateIcon();
    }

    public void Toggle()
    {
        _isOn = !_isOn;
        UpdateIcon();
    }

    private void UpdateIcon()
    {
        _iconImage.sprite = _isOn ? _onSprite : _offSprite;
    }
}
