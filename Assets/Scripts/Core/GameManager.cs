using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace EmpireRush.Core
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }
        
        [Header("Game Settings")]
        public bool debugMode = false;
        public float gameVersion = 1.0f;
        
        [Header("Game State")]
        public GameState currentState = GameState.MainMenu;
        
        public static event Action<GameState> OnGameStateChanged;
        public static event Action OnApplicationPausedChanged;
        
        private DateTime lastPauseTime;
        private bool wasApplicationPaused;
        
        public enum GameState
        {
            MainMenu,
            Playing,
            Paused,
            Settings,
            Store
        }
        
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                InitializeGame();
            }
            else
            {
                Destroy(gameObject);
            }
        }
        
        private void InitializeGame()
        {
            Application.targetFrameRate = 60;
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
            
            // Initialize core systems
            if (SaveSystem.Instance != null)
                SaveSystem.Instance.LoadGame();
                
            if (EconomyManager.Instance != null)
                EconomyManager.Instance.Initialize();
                
            if (AnalyticsManager.Instance != null)
                AnalyticsManager.Instance.Initialize();
        }
        
        public void ChangeGameState(GameState newState)
        {
            if (currentState == newState) return;
            
            GameState previousState = currentState;
            currentState = newState;
            
            OnGameStateChanged?.Invoke(newState);
            
            if (debugMode)
                Debug.Log($"Game state changed from {previousState} to {newState}");
        }
        
        public void LoadScene(string sceneName)
        {
            SceneManager.LoadScene(sceneName);
        }
        
        public void LoadSceneAsync(string sceneName)
        {
            SceneManager.LoadSceneAsync(sceneName);
        }
        
        private void OnApplicationPause(bool pauseStatus)
        {
            wasApplicationPaused = pauseStatus;
            
            if (pauseStatus)
            {
                lastPauseTime = DateTime.Now;
                SaveSystem.Instance?.SaveGame();
            }
            else
            {
                HandleApplicationResumed();
            }
            
            OnApplicationPausedChanged?.Invoke();
        }
        
        private void OnApplicationFocus(bool hasFocus)
        {
            OnApplicationPause(!hasFocus);
        }
        
        private void HandleApplicationResumed()
        {
            if (SaveSystem.Instance != null && EconomyManager.Instance != null)
            {
                TimeSpan offlineTime = DateTime.Now - lastPauseTime;
                EconomyManager.Instance.CalculateOfflineEarnings(offlineTime);
            }
        }
        
        private void OnDestroy()
        {
            if (Instance == this)
            {
                SaveSystem.Instance?.SaveGame();
            }
        }
        
        private void OnApplicationQuit()
        {
            SaveSystem.Instance?.SaveGame();
        }
    }
}