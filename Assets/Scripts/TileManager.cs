using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using KVA.SoundManager;
using UnityEngine;

public class TileManager : MonoBehaviour
{
    public static TileManager Instance { get; private set; }

    [Header("Tray's Settings")]
    [SerializeField] private Transform _trayTransform;
    [SerializeField] private float _spacing = 0.05f;
    [SerializeField] private float _durationMove = 0.3f;
    [SerializeField] private List<Tile> _tilesInTray = new();
    [SerializeField] private LevelManager _levelManager;

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
    }

    public void AddTileToTray(Tile tile)
    {
        if (_tilesInTray.Contains(tile)) return;

        int insertIndex = FindInsertIndex(tile);
        _tilesInTray.Insert(insertIndex, tile);
        tile.OnTileDestroyed();

        if (_tilesInTray.Count > 7)
        {
            _tilesInTray.Clear();
            _levelManager.ClearLevel();

            GameManager.Instance.ShowLosePanel();
            return;
        }

        UpdateTilePositions(() => {
            CheckAndRemoveMatchedTiles();
        });
    }

    private int FindInsertIndex(Tile newTile)
    {
        int index = _tilesInTray.Count;
        for (int i = 0; i < _tilesInTray.Count; i++)
        {
            if (_tilesInTray[i].TileID == newTile.TileID)
            {
                return i + 1;
            }
        }
        return index;
    }

    private void UpdateTilePositions(System.Action onComplete = null)
    {
        float tileWidth = _tilesInTray.Count > 0 ? _tilesInTray[0].GetWidth : 1f;
        int completedCount = 0;

        for (int i = 0; i < _tilesInTray.Count; i++)
        {
            Vector3 targetPos = _trayTransform.position + new Vector3(i * (tileWidth + _spacing), 0, -i);
            MoveToPosition(targetPos, _tilesInTray[i], () => {
                completedCount++;
                if (completedCount == _tilesInTray.Count)
                {
                    onComplete?.Invoke();
                }
            });
        }
    }

    private void CheckAndRemoveMatchedTiles()
    {
        Dictionary<int, List<Tile>> tileGroups = new();

        foreach (Tile tile in _tilesInTray)
        {
            if (!tileGroups.ContainsKey(tile.TileID))
            {
                tileGroups[tile.TileID] = new List<Tile>();
            }
            tileGroups[tile.TileID].Add(tile);
        }

        List<Tile> tilesToRemove = new();

        foreach (var group in tileGroups.Values)
        {
            while (group.Count >= 3)
            {
                tilesToRemove.AddRange(group.GetRange(0, 3));
                group.RemoveRange(0, 3);
            }
        }

        if (tilesToRemove.Count > 0)
        {
            StartCoroutine(RemoveTilesFromTray(tilesToRemove));
        }
    }

    private void MoveToPosition(Vector3 targetPosition, Tile tile, System.Action onComplete = null)
    {
        tile.transform.DOMove(targetPosition, _durationMove).SetEase(Ease.OutQuad)
        .OnComplete(() => {
            transform.DOShakeRotation(0.2f, new Vector3(0, 0, 5), 5, 90, false);
            onComplete?.Invoke();
        });
    }

    private IEnumerator RemoveTilesFromTray(List<Tile> tiles)
    {
        foreach (Tile tile in tiles)
        {
            tile.KillTile();
            _tilesInTray.Remove(tile);

            yield return new WaitForSeconds(0.1f);
        }
        SoundManager.PlaySound(SoundType.COMBO);

        UpdateTilePositions();
        Invoke(nameof(CheckWinCondition), 1f);
    }

    private void CheckWinCondition()
    {
        if (FindObjectsOfType<Tile>().Length == 0)
        {
            GameManager.Instance.ShowWinPanel();
        }
    }

    public void ClearTray()
    {
        _tilesInTray.Clear();
    }
}
