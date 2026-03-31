using System.Collections.Generic;
using _02.Scripts.Core.Domain;

namespace _02.Scripts.Core.Infrastructure
{
    public class GameStateProviderAdapter : IGameStateProvider
    {
        private readonly IGameManager _gameManager;
        
        // gameManager에서 받아옴
        public GameStateProviderAdapter(IGameManager gameManager)
        {
            _gameManager = gameManager;
        }
        
        public int CurrentChapter => _gameManager.CurrentChapter;
        
        // Inventory Manager에서 받아옴
        public IReadOnlyList<string> GetInventory()
        {
            return new List<string> ();
        }
        
        // 해결 퍼즐 아이템 관리소에서 받아옴
        public IReadOnlyList<string> GetSolvedPuzzles()
        {
            return new List<string> ();
        }
    }
}