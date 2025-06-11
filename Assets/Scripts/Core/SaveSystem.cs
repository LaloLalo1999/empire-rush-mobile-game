using System;
using System.IO;
using UnityEngine;
using EmpireRush.Economy;

namespace EmpireRush.Core
{
    public class SaveSystem : MonoBehaviour
    {
        public static SaveSystem Instance { get; private set; }
        
        [Header("Save Settings")]
        [SerializeField] private float autoSaveIntervalSeconds = 30f;
        [SerializeField] private bool enableCloudSave = true;
        
        private string saveFilePath;
        private GameSaveData currentSaveData;
        private float lastAutoSaveTime;
        
        public bool IsNewGame { get; private set; } = true;
        
        public static event Action OnGameSaved;
        public static event Action OnGameLoaded;
        
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                InitializeSaveSystem();
            }
            else
            {
                Destroy(gameObject);
            }
        }
        
        private void InitializeSaveSystem()
        {
            saveFilePath = Path.Combine(Application.persistentDataPath, "empirerush_save.json");
            
            if (GameManager.Instance != null && GameManager.Instance.debugMode)
                Debug.Log($"Save file path: {saveFilePath}");
        }
        
        private void Update()
        {
            // Auto-save every 30 seconds
            if (Time.time - lastAutoSaveTime >= autoSaveIntervalSeconds)
            {
                SaveGame();
                lastAutoSaveTime = Time.time;
            }
        }
        
        public void SaveGame()
        {
            try
            {
                currentSaveData = new GameSaveData
                {
                    gameVersion = GameManager.Instance.gameVersion,
                    saveTime = DateTime.Now.ToBinary(),
                    economyData = EconomyManager.Instance?.GetSaveData()
                };
                
                string jsonData = JsonUtility.ToJson(currentSaveData, true);
                File.WriteAllText(saveFilePath, jsonData);
                
                OnGameSaved?.Invoke();
                
                if (GameManager.Instance.debugMode)
                    Debug.Log("Game saved successfully");
                    
                // TODO: Implement cloud save with Firebase
                if (enableCloudSave)
                {
                    SaveToCloud();
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to save game: {e.Message}");
            }
        }
        
        public void LoadGame()
        {
            try
            {
                if (File.Exists(saveFilePath))
                {
                    string jsonData = File.ReadAllText(saveFilePath);
                    currentSaveData = JsonUtility.FromJson<GameSaveData>(jsonData);
                    
                    if (currentSaveData != null)
                    {
                        IsNewGame = false;
                        
                        // Load economy data
                        if (EconomyManager.Instance != null && currentSaveData.economyData != null)
                        {
                            EconomyManager.Instance.LoadSaveData(currentSaveData.economyData);
                        }
                        
                        OnGameLoaded?.Invoke();
                        
                        if (GameManager.Instance.debugMode)
                            Debug.Log("Game loaded successfully");
                    }
                }
                else
                {
                    IsNewGame = true;
                    if (GameManager.Instance.debugMode)
                        Debug.Log("No save file found, starting new game");
                }
                
                // TODO: Implement cloud load with Firebase
                if (enableCloudSave && IsNewGame)
                {
                    LoadFromCloud();
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to load game: {e.Message}");
                IsNewGame = true;
            }
        }
        
        public void DeleteSave()
        {
            try
            {
                if (File.Exists(saveFilePath))
                {
                    File.Delete(saveFilePath);
                    if (GameManager.Instance.debugMode)
                        Debug.Log("Save file deleted");
                }
                
                IsNewGame = true;
                currentSaveData = null;
                
                // TODO: Delete cloud save
                if (enableCloudSave)
                {
                    DeleteCloudSave();
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to delete save: {e.Message}");
            }
        }
        
        public bool HasSaveFile()
        {
            return File.Exists(saveFilePath);
        }
        
        public DateTime GetLastSaveTime()
        {
            if (currentSaveData != null)
                return DateTime.FromBinary(currentSaveData.saveTime);
            return DateTime.MinValue;
        }
        
        #region Cloud Save (Firebase Integration - TODO)
        
        private void SaveToCloud()
        {
            // TODO: Implement Firebase cloud save
            // This would typically involve:
            // 1. Authenticate user
            // 2. Upload save data to Firebase Realtime Database or Firestore
            // 3. Handle conflicts between local and cloud save
        }
        
        private void LoadFromCloud()
        {
            // TODO: Implement Firebase cloud load
            // This would typically involve:
            // 1. Authenticate user
            // 2. Download save data from Firebase
            // 3. Compare timestamps and merge data if needed
        }
        
        private void DeleteCloudSave()
        {
            // TODO: Implement Firebase cloud save deletion
        }
        
        #endregion
        
        #region Data Validation
        
        private bool ValidateSaveData(GameSaveData data)
        {
            if (data == null) return false;
            if (data.gameVersion <= 0) return false;
            if (data.economyData == null) return false;
            
            // Add more validation as needed
            return true;
        }
        
        #endregion
    }
    
    [System.Serializable]
    public class GameSaveData
    {
        public float gameVersion;
        public long saveTime;
        public EconomyData economyData;
        
        // Future data structures can be added here:
        // public PlayerProgressData progressData;
        // public SettingsData settingsData;
        // public AchievementData achievementData;
    }
}