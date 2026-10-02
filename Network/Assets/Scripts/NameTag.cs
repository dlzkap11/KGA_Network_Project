using Photon.Pun;
using TMPro;
using UnityEngine;

public class NameTag : MonoBehaviourPun
{
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private Transform nameCanvas;

    private PlayerContoroller player;

    void Start()
    {
        player = GetComponent<PlayerContoroller>();
        nameText.text = photonView.Owner.NickName;

        if(photonView.IsMine)
            nameText.color = Color.blue;
        else
            nameText.color = Color.red;
    }

    void LateUpdate()
    {
        nameText.text = photonView.Owner.NickName;
        nameCanvas.rotation = Camera.main.transform.rotation;
    }
}

//클래스 HpSyncLab(MonoBehaviourPunCallbacks + IPunObservable):

//  설정값:
//mode = Stream / Rpc / Property 중 하나(인스펙터에서 선택)
//    hpText = 체력 표시용 텍스트  
//  변수:  
//    hp = 100


//함수 Start:  
//  mode가 Property이면  
//    → 내 오브젝트 주인(Owner)의 Custom Properties에 "hp"가 이미 있으면  
//      그 값을 hp로 가져온다 (늦게 들어온 사람도 맞는 값으로 시작)  
//  화면 텍스트를 갱신한다


//함수 Update:  
//  내 것이면서 H키를 눌렀으면  
//    → ChangeHp(-10)


//함수 ChangeHp(변화량):   (소유자만 호출됨)
//  hp에 변화량을 더한다  
//  화면 텍스트를 갱신한다

//  mode가 Rpc이면  
//    → "RpcSetHp"를 나를 제외한 모두에게 요청한다 (최종 hp 값을 담아서)  
//  mode가 Property이면  
//    → 새 Hashtable에 "hp" = hp 를 담고  
//      내 Player(Owner)에 SetCustomProperties로 저장한다  
//  mode가 Stream이면  
//    → 아무것도 하지 않는다 (OnPhotonSerializeView가 알아서 보낸다)


//함수 OnPhotonSerializeView(stream, info):   (Observed Components에 등록돼 있어야 호출됨)  
//  mode가 Stream이 아니면 → 그냥 종료

//  쓰는 쪽(소유자)이면  
//    → stream에 hp를 담는다  
//  읽는 쪽(나머지)이면  
//    → stream에서 꺼낸 값을 (int)로 바꿔 hp에 넣는다  
//    → 화면 텍스트를 갱신한다


//원격 실행 함수 RpcSetHp(새 hp):   ([PunRPC], 받는 쪽에서 실행됨)  
//  hp = 새 hp  
//  화면 텍스트를 갱신한다


//함수 OnPlayerPropertiesUpdate(대상 플레이어, 바뀐 속성):   (Property 모드에서 호출됨)  
//  mode가 Property가 아니면 → 종료  
//  대상 플레이어가 이 오브젝트의 주인이 아니면 → 종료   (남의 값으로 덮어쓰지 않기 위해)

//  바뀐 속성에 "hp"가 있으면  
//    → 그 값을 (int)로 바꿔 hp에 넣는다  
//    → 화면 텍스트를 갱신한다


//함수 RefreshText:  
//  hpText가 있으면 "HP {hp}"를 표시한다