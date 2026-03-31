namespace _02.Scripts.Enemy
{
    public interface IEnemyAction
    {
        void Execute(EnemyController enemy);
        void Stop();
    }
}