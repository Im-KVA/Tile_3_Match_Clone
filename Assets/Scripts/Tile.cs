using UnityEngine;
using System.Collections.Generic;
using DG.Tweening;
using KVA.SoundManager;

public class Tile : MonoBehaviour
{
    [Header("Tile Components")]
    [SerializeField] private SpriteRenderer _tileRenderer;
    [SerializeField] private SpriteRenderer _tileTypeRenderer;
    [SerializeField] private BoxCollider2D _collider;
    [SerializeField] private ParticleSystem _killParticle;


    [Header("Tile Data")]
    [SerializeField] private TileData _tileData;
    [SerializeField] private List<Tile> _underTilesList = new();
    [SerializeField] private List<Tile> _overTilesList = new();

    private HashSet<Tile> _underTiles = new();
    private HashSet<Tile> _overTiles = new();

    private Vector3 _originalScale;

    public int TileID => _tileData.tileID;
    public float GetWidth => _collider.bounds.size.x;

    private void Awake()
    {
        _originalScale = transform.localScale;
    }

    private void Start()
    {
        _tileTypeRenderer.sprite = _tileData.tileTypeSprite;
    }

    private void OnMouseDown()
    {
        if (IsCovered()) return;
        if (GameManager.Instance.isGamePause) return;

        SelectTile();
        SoundManager.PlaySound(SoundType.TILE);
    }

    private void OnMouseUp()
    {
        if (IsCovered()) return;
        if (GameManager.Instance.isGamePause) return;
        OnTileDestroyed();
        DeselectTile();
        TileManager.Instance.AddTileToTray(this);
    }

    public bool IsCovered()
    {
        if (_overTilesList.Count > 0)
        {
            _tileRenderer.sprite = _tileData.tileUnderSprite;
            return true;
        }
        else
        {
            _tileRenderer.sprite = _tileData.tileSprite;
            return false;
        }
    }

    public void DetectOverlappingTiles()
    {
        _underTiles.Clear();
        _overTiles.Clear();

        Vector2 size = _collider.bounds.size;
        Vector2 center = (Vector2)_collider.bounds.center;

        Collider2D[] colliders = Physics2D.OverlapBoxAll(center, size, 0);

        foreach (Collider2D col in colliders)
        {
            if (col.gameObject == gameObject) continue;
            if (!col.TryGetComponent<Tile>(out var otherTile)) continue;

            if (otherTile.transform.position.z > transform.position.z)
            {
                _underTiles.Add(otherTile);
                otherTile._overTiles.Add(this);
            }
            else if (otherTile.transform.position.z < transform.position.z)
            {
                _overTiles.Add(otherTile);
                otherTile._underTiles.Add(this);
            }
        }

        _underTilesList.Clear();
        _underTilesList = new List<Tile>(_underTiles);
        _overTilesList.Clear();
        _overTilesList = new List<Tile>(_overTiles);

        IsCovered();
    }

    public void SetTileData(TileData tileData)
    {
        _tileData = tileData;
    }

    public void OnTileDestroyed()
    {
        foreach (Tile tile in _underTilesList)
        {
            if (tile != null)
            {
                tile._overTilesList.Remove(this);
                tile.IsCovered();
            }
        }
    }

    private void SelectTile()
    {
        transform.DOScale(_originalScale * 1.5f, 0.2f).SetEase(Ease.OutBack);
        transform.DOShakeRotation(0.3f, new Vector3(0, 0, 10), 10, 90, false);

        _tileRenderer.sortingOrder = 10;
        _tileTypeRenderer.sortingOrder = 10;
    }

    public void DeselectTile()
    {
        transform.DOScale(_originalScale, 0.2f).SetEase(Ease.InBack);
        transform.rotation = Quaternion.Euler(Vector3.zero);

        _tileRenderer.sortingOrder = 0;
        _tileTypeRenderer.sortingOrder = 0;
    }

    public void KillTile()
    {
        _killParticle.Play();
        transform.DOScale(_originalScale * 1.1f, 0.1f);
        transform.DOScale(_originalScale * 0f, 0.5f).OnComplete(() => Destroy(gameObject));
    }
}
