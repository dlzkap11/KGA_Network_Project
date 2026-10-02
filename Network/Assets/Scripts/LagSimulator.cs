using UnityEngine;
using Photon.Pun;

public class LagSimulator : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
            SetRate(30, 30);
        if (Input.GetKeyDown(KeyCode.Alpha2))
            SetRate(5, 5);
        if (Input.GetKeyDown(KeyCode.Alpha3))
            SetRate(30, 5);

        if (Input.GetKeyDown(KeyCode.Alpha4))
            SetLag(0);
        if (Input.GetKeyDown(KeyCode.Alpha5))
            SetLag(300);
        if (Input.GetKeyDown(KeyCode.Alpha6))
            SetLag(500);
    }


    private void SetRate(int sendRate, int serializeRate)
    {
        PhotonNetwork.SendRate = sendRate;
        PhotonNetwork.SerializationRate = serializeRate;

        Debug.Log($"Rate : {sendRate}, SerializationRate : {serializeRate}");
    }

    private void SetLag(int ms)
    {
        var peer = PhotonNetwork.NetworkingClient.LoadBalancingPeer;

        if(ms == 0)
        {
            peer.IsSimulationEnabled = false;
        }
        else
        {
            peer.NetworkSimulationSettings.IncomingLag = ms;
            peer.IsSimulationEnabled = true;
        }

        Debug.Log($"{ms}ms 만큼 지연");
    }
}
