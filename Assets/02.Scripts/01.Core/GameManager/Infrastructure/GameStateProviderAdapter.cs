using System.Collections.Generic;
using _02.Scripts.Core.Domain;

namespace _02.Scripts.Core.Infrastructure
{
    public class GameStateProviderAdapter : IGameStateProvider
    {
        private const string CompletedStateKey = "is_completed";
        private const string OpenStateKey = "is_open";

        private readonly IGameManager _gameManager;

        public GameStateProviderAdapter(IGameManager gameManager)
        {
            _gameManager = gameManager;
        }

        public int CurrentChapter => _gameManager.CurrentChapter;

        public IReadOnlyList<string> GetInventory()
        {
            InventoryManager inventoryManager = InventoryManager.Instance;
            RuntimeInstanceManager runtimeInstanceManager = RuntimeInstanceManager.Instance;
            if (inventoryManager == null || runtimeInstanceManager == null)
            {
                return new List<string>();
            }

            var result = new List<string>();
            IReadOnlyList<string> instanceIds = inventoryManager.ReadonlyPlayerInventoryInstanceIds;
            for (int i = 0; i < instanceIds.Count; i++)
            {
                if (runtimeInstanceManager.TryGetItemInstance(instanceIds[i], out RuntimeItemData itemData))
                {
                    result.Add(itemData.ItemName);
                }
            }

            return result;
        }

        public IReadOnlyList<string> GetCompletedTasks()
        {
            RuntimeInstanceManager runtimeInstanceManager = RuntimeInstanceManager.Instance;
            if (runtimeInstanceManager == null)
            {
                return new List<string>();
            }

            IReadOnlyDictionary<string, RuntimeData> instances = runtimeInstanceManager.RuntimeInstances;
            if (instances == null)
            {
                return new List<string>();
            }

            var result = new List<string>();
            foreach (KeyValuePair<string, RuntimeData> pair in instances)
            {
                RuntimeData data = pair.Value;
                if (string.IsNullOrEmpty(data.ObjectName))
                {
                    continue;
                }

                if (data.State == null)
                {
                    continue;
                }

                if (data.State.GetBool(CompletedStateKey) || data.State.GetBool(OpenStateKey))
                {
                    result.Add(data.ObjectName);
                }
            }

            return result;
        }
    }
}