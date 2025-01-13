using UnityEngine;

public class HYJ_CharacterImage : MonoBehaviour
{
    [SerializeField] public int curPos;    // 현재 캐릭터 위치
    [SerializeField] public int unitIndex; // 캐릭터 고유 번호

    // 해당 스크립트는 프리팹에만 적용한다.
    // 생성 / 삭제만 외부에서 지정.
    // 캐릭터의 위치가 변경되면 위치를 참조하여 배치하기
}
