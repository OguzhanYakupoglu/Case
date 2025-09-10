using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : SoftSingleton<UIManager>
{
    [SerializeField] private Panel gamePanel;
    [SerializeField] private Panel successPanel;
    [SerializeField] private Panel failPanel;
    private void Start()
    {
        GameManager.Instance.OnGameStateChanged += (newState) =>
        {
            UpdateUI(newState);
        };
    }
    private void UpdateUI(GameManager.GameState newState)
    {
        gamePanel.SetActive(newState == GameManager.GameState.Play);
        successPanel.SetActive(newState == GameManager.GameState.Success);
        failPanel.SetActive(newState == GameManager.GameState.Fail);
    }
}
