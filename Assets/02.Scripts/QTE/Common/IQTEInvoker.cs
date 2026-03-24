//해당 오브젝트는 QTE를 발생시킴
public interface IQTEInvoker
{
    void ApplyQTEFailure(); //QTE에 실패한경우
    void ApplyQTESuccess(); //QTE에 성공한 경우
    void ApplyQTEGreatSuccess();    //QTE에 대성공한경우
}
