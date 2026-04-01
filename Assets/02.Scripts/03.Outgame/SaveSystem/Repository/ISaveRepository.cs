using _02.Scripts._03.Outgame.SaveSystem.Domain;
using System;

namespace _02.Scripts._03.Outgame.SaveSystem.Repository
{
    public interface ISaveRepository
    {
        void Save(SaveData data, Action onSuccess, Action<string> onFailure);
        void Load(Action<SaveData> onSuccess, Action<string> onFailure);
        void Delete(Action onSuccess, Action<string> onFailure);
        bool HasSave();
    }
}