using UnityEngine;

// 내 입력으로 직접 움직이는 큐브(파란색, LocalCube).
// 내 화면에서는 지연이 없고, 동시에 매 프레임 위치를 채널로 전송합니다.
public class LocalCubeMover : MonoBehaviour
{
    public FakeNetworkChannel channel; // NetworkChannel 오브젝트를 인스펙터에서 연결
    public float moveSpeed = 3f;

    private void Update()
    {
        // 입력 → 즉시 이동 (내 화면은 지연 없음)
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        Vector3 move = new Vector3(h, 0f, v) * moveSpeed * Time.deltaTime;
        transform.position += move;

        // 매 프레임 현재 위치를 가짜 네트워크로 전송
        if (channel != null)
        {
            channel.Send(transform.position);
        }
    }
}