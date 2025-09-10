using UnityEngine.SceneManagement;

public class GameManager : SoftSingleton<GameManager>
{
    public enum GameState { Play, Success, Fail }
    public GameState CurrentGameState { get; private set; }
    public System.Action<GameState> OnGameStateChanged;
    void Start()
    {
        SetGameState(GameState.Play);
    }
    public void RestartGame()
    {
        ObjectPooler.Instance.ReturnAllPools();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    public void EndGame(bool success)
    {
        SetGameState(success ? GameState.Success : GameState.Fail);
    }
    public void SetGameState(GameState newState)
    {
        CurrentGameState = newState;
        OnGameStateChanged?.Invoke(newState);
    }
}
