using UnityEngine;
using TMPro;
using Photon.Pun;

public class NameTag : MonoBehaviourPun
{
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private Transform nameCanvas;

    private PlayerController player;

    void Start()
    {

        player = GetComponent<PlayerController>();

        if (photonView.IsMine)
            nameText.color = Color.blue;
        else
            nameText.color = Color.red;
    }
    private void Update()
    {
        
        nameText.text = $"Player {player.hp}";
    }

    void LateUpdate()
    {
        nameCanvas.rotation = Camera.main.transform.rotation;
    }
}
//[TopScoreLoader 의사코드]

//OnClickLoadTop3:
//scoresRef나 topScoreText가 비어 있으면 → 종료          (가드)  
//  scoresRef에서 "score 기준으로 정렬한 뒤 뒤에서 3개"를 한 번 읽어온다  
//  스냅샷이 없으면 → "아직 기록이 없습니다" 표시, 종료  
//  여기부터는 여러분 몫입니다.  
//  목표: 화면에 1위부터 3위까지 순서대로, "N위  이름 : 점수" 형식으로 보여준다  
//  주의: 방금 읽어온 순서가 그대로 1위부터인지, 위의 예측과 비교해 확인하고 시작하세요.