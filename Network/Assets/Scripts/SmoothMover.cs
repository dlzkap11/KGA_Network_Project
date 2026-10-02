using Photon.Pun;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class SmoothMover : MonoBehaviourPun, IPunObservable
{
    [SerializeField] private float moveSpeed;
    [SerializeField] private float lerpSpeed;
    [SerializeField] private float snapDistance; // 중간 전송 유실시 보정값


    private Vector3 networkPos;
    private Quaternion networkRot;

    private void Awake()
    {
        networkPos = transform.position;
        networkRot = transform.rotation;
    }

    private void Update()
    {
        if (photonView.IsMine)
            return;

        if(Vector3.Distance(transform.position, networkPos) > snapDistance)
        {
            transform.position = networkPos;
            transform.rotation = networkRot;
        }
        transform.position = Vector3.Lerp(transform.position, networkPos, lerpSpeed * Time.deltaTime);
        transform.rotation = Quaternion.Lerp(transform.rotation, networkRot, lerpSpeed * Time.deltaTime);

    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        // 보내는 순서와 받는 순서가 같아야한다.
        if (stream.IsWriting)
        {
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
        }
        else if (stream.IsReading)
        {
            networkPos = (Vector3)stream.ReceiveNext();
            networkRot = (Quaternion)stream.ReceiveNext();
        }
    }
}
