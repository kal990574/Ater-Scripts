using System;                                             
using _02.Scripts._03.Outgame.SaveSystem.Domain;
using _02.Scripts._03.Outgame.SaveSystem.Repository;

namespace _02.Scripts._03.Outgame.SaveSystem.Manager
{
    public class SaveManager
    {
        private readonly ISaveRepository _repository;

        public SaveManager(ISaveRepository repository)
        {
            _repository = repository;
        }

        public void SaveGame(SaveData data, Action onSuccess, Action<string> onFailure = null)
        {
            data.SavedAt = DateTime.UtcNow.ToString("o");
            _repository.Save(data, onSuccess, onFailure);
        }
        
        public void LoadGame(Action<SaveData> onSuccess, Action<string> onFailure = null)
        {
            _repository.Load(onSuccess, onFailure);
        }

        public void DeleteSave(Action onSuccess = null, Action<string> onFailure = null)
        {                                                 
            _repository.Delete(onSuccess, onFailure);
        }

        public bool HasSave()
        {
            return _repository.HasSave();
        }
    }
}