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

        public UniTask Save(SaveData saveData)
        {
            var json = JsonUtility.ToJson(saveData);
            File.WriteAllText(_savePath, json);
            return UniTask.CompletedTask;
        }

        public UniTask<SaveData> Load()
        {
            if(!File.Exists(_savePath)) 
                throw new FileNotFoundException("저장 데이터 없음");
            
            var json = File.ReadAllText(_savePath);
            var data = JsonUtility.FromJson<SaveData>(json);
            return UniTask.FromResult(data);
        }

        public UniTask Delete()
        {
            if(File.Exists(_savePath)) 
                File.Delete(_savePath);
            return UniTask.CompletedTask;
        }

        public UniTask<bool> HasSave()
        {
            return UniTask.FromResult(File.Exists(_savePath));
        }
    }
}
