using UnityEngine;
using Photon.Pun;

public class PlayerController : MonoBehaviourPun//, IPunObservable
{
    [SerializeField] private float moveSpeed;
    [SerializeField] private Renderer ren;

    private Animator anim;
    private bool isWalk;
    private float lastRecieveTime;

    public int hp = 100;

    //public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    //{
    //    if(stream.IsWriting)
    //    {
    //        // 특정 데이터의 값을 보내는 상황
    //        stream.SendNext(hp);
    //    }
    //    else if(stream.IsReading)
    //    {
    //        // 보낸 특정 데이터의 값을 읽어오는 상황
    //        hp = (int)stream.ReceiveNext();

    //        float now = Time.time;
    //        Debug.Log($"[받음] hp = {hp}, 간격 = {now - lastRecieveTime:F3}초");
    //        lastRecieveTime = now;
    //    }
    //}

    private void Start()
    {
        anim = GetComponent<Animator>();

        if (photonView.IsMine)
        {
            ren.material.color = Color.blue;
        }
    }

    void Update()
    {
        if (!photonView.IsMine)
            return;

        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        Vector3 dir = new Vector3(h, 0f, v);
        transform.Translate(dir * moveSpeed * Time.deltaTime, Space.World);

        if (dir.sqrMagnitude > 0.01f)
            transform.rotation = Quaternion.LookRotation(dir);

        if (dir.sqrMagnitude > 0.01f)
            isWalk = true;
        else
            isWalk = false;

        anim.SetBool("IsWalk", isWalk);
        anim.SetFloat("Speed", dir.magnitude);

        //if(Input.GetKeyDown(KeyCode.H))
        //{
        //    hp -= 1;
        //}

        if(Input.GetKeyDown(KeyCode.Space))
        {
            photonView.RPC("PlayJump",RpcTarget.All);
        }

        if(Input.GetMouseButtonDown(0))
        {
            OnClickAttack();
        }
    }

    private void OnClickAttack()
    {
        if (!photonView.IsMine)
            return;

        photonView.RPC("RpcOnAttack", RpcTarget.All, transform.position, transform.forward);
    }

    [PunRPC]
    private void RpcOnAttack(Vector3 origin, Vector3 direction)
    {
        Debug.DrawRay(origin, direction * 10f,Color.red,5f);
        Debug.Log($"{photonView.Owner.ActorNumber}번 플레이어가 공격");
    }

    [PunRPC]
    private void PlayJumpRpc()
    {
        anim.SetTrigger("Jump");
        Debug.Log($"{photonView.Owner.ActorNumber}번 플레이어가 점프함");
    }
}
