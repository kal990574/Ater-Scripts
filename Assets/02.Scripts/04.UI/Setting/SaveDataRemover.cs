using UnityEngine;
using System;
using System.IO;
using Cysharp.Threading.Tasks;
using _02.Scripts._03.Outgame.SaveSystem.Repository;
using UnityEngine.SceneManagement;
using Sirenix.OdinInspector;


public class SaveDataRemover : MonoBehaviour
{
    [Button]
    public void TryRemoveSaveData()
    {
        TryRemoveSaveDataAsync().Forget();
    }
    
    private async UniTaskVoid TryRemoveSaveDataAsync()
    {
        SaveDataRemovalSnapshot snapshot = await SaveDataRemovalSnapshot.CaptureAsync();

        try
        {
            await snapshot.DeleteAllAsync();
            snapshot.VerifyDeleted();
            RefreshRuntimeStateAfterDelete();
            Debug.Log("[SaveDataRemover] Save data removal committed.");
            RestartCurrentScene();
        }
        catch (Exception exception)
        {
            Debug.LogError($"[SaveDataRemover] Save data removal failed. Rolling back. {exception}");

            try
            {
                await snapshot.RestoreAllAsync();
                RefreshRuntimeStateFromStorage();
                Debug.Log("[SaveDataRemover] Rollback completed.");
            }
            catch (Exception rollbackException)
            {
                Debug.LogError($"[SaveDataRemover] Rollback failed. Manual recovery may be required. {rollbackException}");
            }
        }
    }

    private static void RefreshRuntimeStateAfterDelete()
    {
        StatisticsManager statisticsManager = StatisticsManager.Instance;
        if (statisticsManager != null)
        {
            statisticsManager.ResetCurrentRun();
            statisticsManager.ReloadPersistentFromStorage();
        }

        AchievementManager achievementManager = AchievementManager.Instance;
        if (achievementManager != null)
        {
            achievementManager.ReloadAllDataFromStorage();
        }
    }

    private static void RefreshRuntimeStateFromStorage()
    {
        StatisticsManager statisticsManager = StatisticsManager.Instance;
        if (statisticsManager != null)
        {
            statisticsManager.ResetCurrentRun();
            statisticsManager.ReloadPersistentFromStorage();
        }

        AchievementManager achievementManager = AchievementManager.Instance;
        if (achievementManager != null)
        {
            achievementManager.ReloadAllDataFromStorage();
        }
    }

    private static void RestartCurrentScene()
    {
        Scene activeScene = SceneManager.GetActiveScene();
        if (activeScene.IsValid() == false)
        {
            Debug.LogError("[SaveDataRemover] Cannot restart scene because the active scene is invalid.");
            return;
        }

        SceneManager.LoadScene(activeScene.buildIndex);
    }

    private sealed class SaveDataRemovalSnapshot
    {
        private readonly string _savePath;
        private readonly byte[] _saveFileBytes;
        private readonly string _statisticsJson;
        private readonly string _achievementJson;

        private SaveDataRemovalSnapshot(
            string savePath,
            byte[] saveFileBytes,
            string statisticsJson,
            string achievementJson)
        {
            _savePath = savePath;
            _saveFileBytes = saveFileBytes;
            _statisticsJson = statisticsJson;
            _achievementJson = achievementJson;
        }

        public static async UniTask<SaveDataRemovalSnapshot> CaptureAsync()
        {
            string savePath = LocalSaveRepository.GetSavePath();
            byte[] saveFileBytes = null;

            if (File.Exists(savePath))
            {
                saveFileBytes = await UniTask.RunOnThreadPool(() => File.ReadAllBytes(savePath));
            }

            string statisticsJson = PlayerPrefs.HasKey(StatisticsPlayerPrefsRepository.SaveKey)
                ? PlayerPrefs.GetString(StatisticsPlayerPrefsRepository.SaveKey, string.Empty)
                : null;

            string achievementJson = PlayerPrefs.HasKey(AchievementStatePlayerPrefsRepository.SaveKey)
                ? PlayerPrefs.GetString(AchievementStatePlayerPrefsRepository.SaveKey, string.Empty)
                : null;

            return new SaveDataRemovalSnapshot(savePath, saveFileBytes, statisticsJson, achievementJson);
        }

        public async UniTask DeleteAllAsync()
        {
            if (File.Exists(_savePath))
            {
                await UniTask.RunOnThreadPool(() => File.Delete(_savePath));
            }

            PlayerPrefs.DeleteKey(StatisticsPlayerPrefsRepository.SaveKey);
            PlayerPrefs.DeleteKey(AchievementStatePlayerPrefsRepository.SaveKey);
            PlayerPrefs.Save();
        }

        public void VerifyDeleted()
        {
            if (File.Exists(_savePath))
            {
                throw new IOException($"Save file still exists: {_savePath}");
            }

            if (PlayerPrefs.HasKey(StatisticsPlayerPrefsRepository.SaveKey))
            {
                throw new InvalidOperationException("Statistics data was not deleted.");
            }

            if (PlayerPrefs.HasKey(AchievementStatePlayerPrefsRepository.SaveKey))
            {
                throw new InvalidOperationException("Achievement data was not deleted.");
            }
        }

        public async UniTask RestoreAllAsync()
        {
            if (_saveFileBytes != null)
            {
                await UniTask.RunOnThreadPool(() => File.WriteAllBytes(_savePath, _saveFileBytes));
            }
            else if (File.Exists(_savePath))
            {
                await UniTask.RunOnThreadPool(() => File.Delete(_savePath));
            }

            RestorePlayerPrefsValue(StatisticsPlayerPrefsRepository.SaveKey, _statisticsJson);
            RestorePlayerPrefsValue(AchievementStatePlayerPrefsRepository.SaveKey, _achievementJson);
            PlayerPrefs.Save();
        }

        private static void RestorePlayerPrefsValue(string key, string value)
        {
            if (value == null)
            {
                PlayerPrefs.DeleteKey(key);
                return;
            }

            PlayerPrefs.SetString(key, value);
        }
    }
}
