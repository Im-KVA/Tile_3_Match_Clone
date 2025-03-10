using System.Collections.Generic;
using DG.Tweening;
using KVA.SoundManager;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("UI Panels")]
    [SerializeField] private CanvasGroup _playPanel;
    [SerializeField] private CanvasGroup _winPanel;
    [SerializeField] private CanvasGroup _losePanel;
    [SerializeField] private CanvasGroup _pausePanel;
    [SerializeField] private CanvasGroup _shopPanel;
    [SerializeField] private CanvasGroup _levelPanel;
    [SerializeField] private float _fadeDuration = 0.5f;
    [SerializeField] private List<RectTransform> _winStars;
    [SerializeField] private Button _replayButton;
    [SerializeField] private Button _nextButton;

    public bool isGameEnd = true;
    public bool isGamePause;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        Application.targetFrameRate = 60;
        QualitySettings.vSyncCount = 0;
    }

    private void Start()
    {
        ShowPlayPanel();
    }

    private void ShowPanel(CanvasGroup panel)
    {
        _playPanel.gameObject.SetActive(panel == _playPanel);
        _winPanel.gameObject.SetActive(panel == _winPanel);
        _losePanel.gameObject.SetActive(panel == _losePanel);
        _pausePanel.gameObject.SetActive(panel == _pausePanel);
        _shopPanel.gameObject.SetActive(panel == _shopPanel);
        _levelPanel.gameObject.SetActive(panel == _levelPanel);

        panel.alpha = 0;
        panel.DOFade(1, _fadeDuration);
    }

    public void ShowPlayPanel()
    {
        ShowPanel(_playPanel);
        isGameEnd = false;
        isGamePause = false;
    }

    public void ShowWinPanel()
    {
        isGameEnd = true;
        isGamePause = true;
        ShowPanel(_winPanel);
        AnimateStars();
        AnimateButtons();
        SoundManager.PlaySound(SoundType.WIN);
        SoundManager.PlaySound(SoundType.FIREWORK);
    }

    public void ShowLosePanel()
    {
        isGameEnd = true;
        isGamePause = true;
        ShowPanel(_losePanel);
        AnimateStars();
        AnimateButtons();
        SoundManager.PlaySound(SoundType.LOSE);
    }

    public void ShowPausePanel()
    {
        isGamePause = true;
        ShowPanel(_pausePanel);
    }

    public void ShowShopPanel()
    {
        isGamePause = true;
        ShowPanel(_shopPanel);
    }

    public void ShowLevelPanel()
    {
        isGamePause = true;
        ShowPanel(_levelPanel);
    }

    private void AnimateStars()
    {
        foreach (RectTransform star in _winStars)
        {
            star.localScale = Vector3.zero;
            star.anchoredPosition += new Vector2(0, 300);

            star.DOAnchorPosY(star.anchoredPosition.y - 300, 0.8f)
                .SetEase(Ease.OutBounce)
                .OnComplete(() => {
                    star.DOScale(1.2f, 0.3f).SetEase(Ease.OutBack)
                        .OnComplete(() => star.DOScale(1f, 0.2f));
                });

            star.DORotate(Vector3.forward * 15, 0.6f).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutSine);
        }
    }

    private void AnimateButtons()
    {
        _replayButton.transform.localScale = Vector3.zero;
        _nextButton.transform.localScale = Vector3.zero;

        _replayButton.transform.DOScale(1.2f, 0.3f).SetEase(Ease.OutBack)
            .OnComplete(() => _replayButton.transform.DOScale(1f, 0.2f));

        _nextButton.transform.DOScale(1.2f, 0.3f).SetEase(Ease.OutBack)
            .OnComplete(() => _nextButton.transform.DOScale(1f, 0.2f));
    }

    public void PlayButtonSound()
    {
        SoundManager.PlaySound(SoundType.BUTTON);
    }
}
