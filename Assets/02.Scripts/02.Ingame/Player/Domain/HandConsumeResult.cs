namespace _02.Scripts.Player
{
    public readonly struct HandConsumeResult
    {
        public HandConsumeResult(bool consumed, bool inventoryEmptyAfterConsume, int nextRecommendedSlotIndex)
        {
            Consumed = consumed;
            InventoryEmptyAfterConsume = inventoryEmptyAfterConsume;
            NextRecommendedSlotIndex = nextRecommendedSlotIndex;
        }

        public bool Consumed { get; }
        public bool InventoryEmptyAfterConsume { get; }
        public int NextRecommendedSlotIndex { get; }
    }
}
