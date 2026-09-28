using System.Collections.Generic;
using UnityEngine;

// 실제 네트워크 없이 "지연"과 "패킷 손실"을 흉내 내는 가짜 통신 채널입니다.
// LocalCubeMover가 Send()로 위치를 넣고, RemoteCubeReceiver가 TryReceive()로 꺼내갑니다.
public class FakeNetworkChannel : MonoBehaviour
{
    [Header("지연 설정")]
    [Range(0f, 1f)]
    public float delaySeconds = 0.2f; // DelaySlider가 이 값을 조절합니다 (편도 지연, 초 단위)

    [Header("패킷 손실 설정 (블록 7)")]
    [Range(0f, 1f)]
    public float packetLossRate = 0f; // 0~30%. Send 단계에서 이 확률만큼 버립니다.

    // (보낸 시각, 위치)를 함께 저장하는 큐.
    // "넣은 순서대로 꺼내야 한다"는 조건 때문에 Queue(FIFO)를 사용합니다.
    private readonly Queue<(float sendTime, Vector3 position)> buffer = new Queue<(float sendTime, Vector3 position)>();

    // LocalCubeMover가 매 프레임 호출합니다.
    public void Send(Vector3 position)
    {
        // 패킷 손실 시뮬레이션: 확률적으로 그냥 버림 (버퍼에 넣지 않음)
        if (Random.value < packetLossRate)
        {
            return;
        }

        buffer.Enqueue((Time.time, position));
    }

    // RemoteCubeReceiver가 매 프레임 호출합니다.
    // 조건을 만족하는 항목이 있으면 true를 반환하고 위치를 꺼내줍니다.
    public bool TryReceive(out Vector3 position)
    {
        position = Vector3.zero;
        bool received = false;

        // "이번 프레임에 도착한 것을 전부 꺼낸다" → while로 반복해야 누적 지연이 안 생김.
        // (if로만 짜면 한 프레임에 여러 개가 도착해도 하나만 소비되어 나머지가 계속 쌓입니다.)
        while (buffer.Count > 0 && Time.time - buffer.Peek().sendTime >= delaySeconds)
        {
            position = buffer.Dequeue().position;
            received = true;
        }

        return received;
    }

    public int PendingCount => buffer.Count;
}