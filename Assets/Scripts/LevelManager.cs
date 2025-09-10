using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : SoftSingleton<LevelManager>
{
    [SerializeField] private List<LevelInfo> levels;
    private const string LevelIndexKey = "LevelIndex";
    public int CurrentLevelIndex { get => PlayerPrefs.GetInt(LevelIndexKey); private set => PlayerPrefs.SetInt(LevelIndexKey, value); }
    public LevelInfo CurrentLevelInfo => levels[CurrentLevelIndex % levels.Count];

    void Start()
    {
        GameManager.Instance.OnGameStateChanged += (state) =>
        {
            if (state == GameManager.GameState.Success)
            {
                CurrentLevelIndex++;
            }
        };
    }
}
