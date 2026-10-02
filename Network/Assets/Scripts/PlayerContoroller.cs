using UnityEngine;
using Photon.Pun;

public class PlayerContoroller : MonoBehaviourPun, IPunObservable
{
    [SerializeField] private float speed;

    [SerializeField] private Renderer render;
    private Animator anim;
    private bool isMove;

    public int hp = 100;

    private void Start()
    {
        anim = GetComponent<Animator>();
        render = gameObject.GetComponentInChildren<Renderer>();

        if (photonView.IsMine)
        {
            render.material.color = Color.white;
        }
        else
        {
            render.material.color = Color.black;
        }
    }

    void Update()
    {
        if (!photonView.IsMine)
            return;

        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        Vector3 dir = new Vector3(h, 0f, v);

        transform.Translate(dir * speed * Time.deltaTime, Space.World);

        if(dir.sqrMagnitude > 0.01f)
        {
            transform.rotation = Quaternion.LookRotation(dir);
        }

        if(dir.magnitude > 0.01f)
            isMove = true;
        else
            isMove = false;

        anim.SetBool("IsMove", isMove);

        if (Input.GetKeyDown(KeyCode.Space))
        {
            photonView.RPC("PlayJump", RpcTarget.All);
        }

        if (Input.GetKeyDown(KeyCode.Z))
        {
            OnAttack();
            
        }

        if (Input.GetKeyDown(KeyCode.M))
        {
            hp -= 10;
        }

    }

    private void OnAttack()
    {
        photonView.RPC("PlayAttack", RpcTarget.All, transform.position, transform.forward);
    }

    [PunRPC]
    private void PlayJump()
    {
        anim.SetTrigger("Jump");
        Debug.Log($"{photonView.Owner.ActorNumber}플레이어 점프");
    }

    [PunRPC]
    private void PlayAttack(Vector3 origin, Vector3 direction)
    {
        Debug.DrawRay(origin, direction * 10f, Color.red, 5f);
        anim.SetTrigger("Attack");
        Debug.Log($"{photonView.Owner.ActorNumber}플레이어 공격");
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsReading) //특정 데이터의 값을 읽을 때
        {
            hp = (int)stream.ReceiveNext();
        }
        else if (stream.IsWriting) //특정 데이터를 보낼 때
        {
            stream.SendNext(hp);
        }
    }
}
