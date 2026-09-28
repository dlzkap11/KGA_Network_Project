using UnityEngine;

// 상대방 위치를 보여주는 큐브(주황색, RemoteCube).
// 채널에서 도착한 값을 꺼내 목표 위치를 갱신하고,
// 즉시 반영하거나 Lerp로 부드럽게 보간해서 이동합니다.
public class RemoteCubeReceiver : MonoBehaviour
{
    public FakeNetworkChannel channel; // NetworkChannel 오브젝트를 인스펙터에서 연결
    public bool useInterpolation = true; // InterpolationToggle과 연결
    public float lerpSpeed = 10f;

    private Vector3 targetPosition;

    private void Start()
    {
        targetPosition = transform.position;
    }

    private void Update()
    {
        // 채널에 새로 도착한 값이 있으면 목표 위치 갱신
        if (channel != null && channel.TryReceive(out Vector3 receivedPosition))
        {
            targetPosition = receivedPosition;
        }

        if (useInterpolation)
        {
            // 부드럽게 목표 위치로 다가감 (튀는 느낌 완화)
            transform.position = Vector3.Lerp(transform.position, targetPosition, lerpSpeed * Time.deltaTime);
        }
        else
        {
            // 즉시 적용 (지연/손실이 그대로 눈에 보임)
            transform.position = targetPosition;
        }
    }
}