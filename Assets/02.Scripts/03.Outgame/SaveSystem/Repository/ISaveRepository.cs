using _02.Scripts._03.Outgame.SaveSystem.Domain;
using Cysharp.Threading.Tasks;
using System;

namespace _02.Scripts._03.Outgame.SaveSystem.Repository
{
    public interface ISaveRepository
    {
        UniTask Save(SaveData saveData);
        UniTask<SaveData> Load();
        UniTask Delete();
        UniTask<bool> HasSave();
    }
}