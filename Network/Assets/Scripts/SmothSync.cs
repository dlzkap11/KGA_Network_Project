using UnityEngine;
using Photon.Pun;

public class SmothSync : MonoBehaviourPun, IPunObservable
{
    [SerializeField] private float moveSpeed;
    [SerializeField] private float lerpSpeed;
    [SerializeField] private float snapDistance; // 중간에 전송이 많이 빌 경우 순간이동

    private Vector3 networkPosition;
    private Quaternion networkRotation;

    private void Awake()
    {
        networkPosition = transform.position;
        networkRotation = transform.rotation;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
            SetRate(30, 30);
        if (Input.GetKeyDown(KeyCode.Alpha2))
            SetRate(5, 5);
        if (Input.GetKeyDown(KeyCode.Alpha3))
            SetRate(30, 5);

        if (photonView.IsMine)
            return;

        if(Vector3.Distance(transform.position,networkPosition) > snapDistance)
        {
            transform.position = networkPosition;
            transform.rotation = networkRotation;
        }


        transform.position = Vector3.Lerp(transform.position, networkPosition, lerpSpeed * Time.deltaTime);
        transform.rotation = Quaternion.Slerp(transform.rotation, networkRotation, lerpSpeed * Time.deltaTime);
    }

    private void SetRate(int sendRate, int serializeRate)
    {
        PhotonNetwork.SendRate = sendRate;
        PhotonNetwork.SerializationRate = serializeRate;
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if(stream.IsWriting)
        {
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
        }
        else
        {
            networkPosition = (Vector3)stream.ReceiveNext();
            networkRotation = (Quaternion)stream.ReceiveNext();
        }
    }
}
