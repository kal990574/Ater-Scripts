/// <summary>
/// 서브 점프스케어의 공통 차단조건 검증
/// </summary>
public class SubJumpScareCommonValidator
{
    public bool TryGetBlockReason(
        SubJumpScareDatabaseSO database,
        SubJumpScareContext context,
        SubJumpScareCooldownState cooldownState,
        out string reason)
    {
        if (database == null)
        {
            reason = "SubJumpScareDatabaseSO가 연결되지 않았습니다.";
            return true;
        }

        if (context == null)
        {
            reason = "SubJumpScareContext가 null입니다.";
            return true;
        }

        if (cooldownState == null)
        {
            reason = "SubJumpScareCooldownState가 null입니다.";
            return true;
        }

        if (context.IsMainJumpScareRunning == true)
        {
            reason = "메인 점프스케어 실행 중입니다.";
            return true;
        }

        if (context.IsInMainEndGraceTime == true)
        {
            reason = "메인 점프스케어 종료 후 유예시간 중입니다.";
            return true;
        }

        if (cooldownState.IsGlobalCooldownActive() == true)
        {
            reason = "글로벌 쿨다운 중입니다.";
            return true;
        }

        reason = string.Empty;
        return false;
    }
}