using System;
using _02.Scripts._03.Outgame.SaveSystem.Domain;

namespace _02.Scripts._03.Outgame.SaveSystem.Repository
{
    public class FirebaseSaveRepository : ISaveRepository
    {
        // TODO: Firebase SDK 연동 후 구현
        public void Save(SaveData data, Action onSuccess, Action<string> onFailure)
        {
            throw new NotImplementedException();
        }

        public void Load(Action<SaveData> onSuccess, Action<string> onFailure)
        {
            throw new NotImplementedException();
        }

        public void Delete(Action onSuccess, Action<string> onFailure)
        {
            throw new NotImplementedException();
        }

        public bool HasSave()
        {
            throw new NotImplementedException();
        }
    }
}