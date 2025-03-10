using System.Collections.Generic;
using DG.Tweening;
using KVA.SoundManager;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public Transform levelParent;

    [SerializeField] private List<GameObject> _levelPrefabs;
    [SerializeField] private TileDatabase _tileDatabase;

    private GameObject _currentLevel;
    private int _currentLevelIndex = -1;

    void Start()
    {
        LoadLevel(0);
    }

    public void LoadLevel(int levelIndex)
    {
        ClearLevel();
        _currentLevelIndex = levelIndex;

        if (levelIndex < 0 || levelIndex >= _levelPrefabs.Count)
        {
            return;
        }

        _currentLevel = Instantiate(_levelPrefabs[levelIndex], levelParent);
        AdjustTileZPositionAndAssignSprites(_currentLevel);
        SoundManager.PlaySound(SoundType.SPAWNBLOCK);
    }

    public void LoadNextLevel()
    {
        _currentLevelIndex++;
        if (_currentLevelIndex >= _levelPrefabs.Count)
        {
            return;
        }

        LoadLevel(_currentLevelIndex);
    }

    public void ClearLevel()
    {
        if (_currentLevel != null)
        {
            TileManager.Instance.ClearTray();
            Destroy(_currentLevel);
            _currentLevel = null;
        }
    }

    public void ReplayLevel()
    {
        if (_currentLevelIndex < 0 || _currentLevelIndex >= _levelPrefabs.Count)
        {
            return;
        }

        LoadLevel(_currentLevelIndex);
    }

    private void AdjustTileZPositionAndAssignSprites(GameObject level)
    {
        Tile[] tiles = level.GetComponentsInChildren<Tile>();
        float zOffset = 0f;

        List<Tile> tileList = new List<Tile>(tiles);

        while (tileList.Count >= 3)
        {
            List<Tile> selectedTiles = new List<Tile>();

            for (int i = 0; i < 3; i++)
            {
                int randomIndex = Random.Range(0, tileList.Count);
                selectedTiles.Add(tileList[randomIndex]);
                tileList.RemoveAt(randomIndex);
            }

            TileData randomTileData = _tileDatabase.GetRandomTile();

            foreach (Tile tile in selectedTiles)
            {
                tile.SetTileData(randomTileData);
            }
        }

        float delay = 0f;
        float delayBetweenTiles = 0.05f;

        foreach (Tile tile in tiles)
        {
            Transform tileTransform = tile.transform;
            Vector3 currentScale = tileTransform.localScale;
            tileTransform.localScale = Vector3.zero;

            tileTransform.DOScale(currentScale, 0.5f)
                .SetEase(Ease.OutBack)
                .SetDelay(delay);

            SpriteRenderer spriteRenderer = tile.GetComponent<SpriteRenderer>();
            if (spriteRenderer != null)
            {
                Color originalColor = spriteRenderer.color;
                spriteRenderer.color = new Color(originalColor.r, originalColor.g, originalColor.b, 0);
                spriteRenderer.DOFade(1, 0.5f).SetDelay(delay);
            }

            delay += delayBetweenTiles;
        }

        foreach (Tile tile in tiles)
        {
            tile.DetectOverlappingTiles();
            tile.transform.position -= new Vector3(0, 0, zOffset);
            zOffset += 0.01f;
        }

        foreach (Tile remainingTile in tileList)
        {
            Destroy(remainingTile.gameObject);
        }
    }
}
