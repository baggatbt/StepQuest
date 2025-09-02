using System;

public static class GameEvents
{
    public static event Action<string> StageCleared;   // stageID
    public static event Action<Enemy>  EnemyDefeated;
    public static event Action<int>    StepsSpent;     // steps

    public static void RaiseStageCleared(string id) => StageCleared?.Invoke(id);
    public static void RaiseEnemyDefeated(Enemy e)    => EnemyDefeated?.Invoke(e);
    public static void RaiseStepsSpent(int steps)     => StepsSpent?.Invoke(steps);
}
