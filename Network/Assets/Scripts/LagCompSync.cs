using UnityEngine;
using Photon.Pun;
public class LagCompSync : MonoBehaviourPun, IPunObservable
{
    public enum CompMode { Interpolate, LagCompensate, Extrapolate }

    [SerializeField] private CompMode mode;


    [SerializeField] private float lerpSpeed;

    [SerializeField] private float maxExtrapolateTime;
    [SerializeField] private float maxLag;
    [SerializeField] private float snapDistance;

    private Vector3 networkPos;
    private Quaternion networkRot;

    private Vector3 networkVelocity;

    private float timeSinceReceive;
    private float timeLogTime;

    private Vector3 lastSentPos;
    private float lastSendTime;


    private void Awake()
    {
        networkPos = transform.position;
        networkRot = transform.rotation;
        
        lastSentPos = transform.position;
        lastSendTime = Time.time;
    }

    private void Update()
    {
        if (photonView.IsMine)
            return;

        timeSinceReceive += Time.deltaTime;

        Vector3 target = networkPos;

        // 예상해서 보내기
        if(mode == CompMode.Extrapolate)
        {
            float t = Mathf.Min(timeSinceReceive, maxExtrapolateTime);
            target = networkPos + networkVelocity * t;
        }

        if(Vector3.Distance(transform.position, target) > snapDistance)
        {
            transform.position = target;
            transform.rotation = networkRot;
            return;
        }

        transform.position = Vector3.Lerp(transform.position, networkPos, lerpSpeed * Time.deltaTime);
        transform.rotation = Quaternion.Lerp(transform.rotation, networkRot, lerpSpeed * Time.deltaTime);
    }


    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsReading)
        {
            networkPos = (Vector3)stream.ReceiveNext();
            networkRot = (Quaternion)stream.ReceiveNext();
            networkVelocity = (Vector3)stream.ReceiveNext();


            float lag = (float)(PhotonNetwork.Time - info.SentServerTime);
            lag = Mathf.Clamp(lag, 0, maxLag);

            if (mode != CompMode.Interpolate)
                networkPos += networkVelocity * lag;

            if(Time.time - timeLogTime > 1f)
            {
                timeLogTime = Time.time;
                Debug.Log($"{lag * 1000f:F0}ms / 속도 {networkVelocity.magnitude:F2}");
            }
            timeSinceReceive = 0;
        }
        else if (stream.IsWriting)
        {
            Vector3 velocity = Vector3.zero;
            float dt = Time.time - lastSendTime;

            if (dt > 0.0001f)
                velocity = (transform.position - lastSentPos) / dt;

            lastSentPos = transform.position;
            lastSendTime = Time.time;

            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
            stream.SendNext(velocity);
            
        }
    }
}
