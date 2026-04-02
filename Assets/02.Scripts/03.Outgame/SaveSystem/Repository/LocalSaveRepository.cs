using System;
using System.IO;
using _02.Scripts._03.Outgame.SaveSystem.Domain;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _02.Scripts._03.Outgame.SaveSystem.Repository
{
    public class LocalSaveRepository : ISaveRepository
    {
        private readonly string _savePath;

        public LocalSaveRepository()
        {
            _savePath = Path.Combine(Application.persistentDataPath, "save.json");
        }

        public async UniTask Save(SaveData saveData)
        {
            var json = JsonUtility.ToJson(saveData);
            await UniTask.RunOnThreadPool(() => File.WriteAllText(_savePath, json));
        }

        public async UniTask<SaveData> Load()
        {
            if (!File.Exists(_savePath))
                throw new FileNotFoundException("저장 데이터 없음");

            var json = await UniTask.RunOnThreadPool(() => File.ReadAllText(_savePath));
            return JsonUtility.FromJson<SaveData>(json);
        }

        public async UniTask Delete()
        {
            await UniTask.RunOnThreadPool(() =>
            {
                if (File.Exists(_savePath))
                    File.Delete(_savePath);
            });
        }

        public UniTask<bool> HasSave()
        {
            return UniTask.FromResult(File.Exists(_savePath));
        }
    }
}
