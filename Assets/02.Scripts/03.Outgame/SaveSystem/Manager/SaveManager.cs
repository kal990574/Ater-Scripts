using _02.Scripts._03.Outgame.SaveSystem.Domain;
using _02.Scripts._03.Outgame.SaveSystem.Repository;
using Cysharp.Threading.Tasks;

namespace _02.Scripts._03.Outgame.SaveSystem.Manager
{
    public class SaveManager
    {
        private readonly ISaveRepository _repository;

        public SaveManager(ISaveRepository repository)
        {
            _repository = repository;
        }

        public async UniTask SaveGame(SaveData data)
        {
            await _repository.Save(data);
        }
        
        public async UniTask<SaveData> LoadGame()
        {
            return await _repository.Load();
        }

        public async UniTask DeleteSave()
        {
            await _repository.Delete();
        }

        public async UniTask<bool> HasSave()
        {
            return await _repository.HasSave();
        }
    }
}