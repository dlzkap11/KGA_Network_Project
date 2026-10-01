using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class GameSceneManager : MonoBehaviourPunCallbacks
{
    [SerializeField] private Vector3 spawnPos;
    [SerializeField] private Transform[] spawnPoints;

    private void Start()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
        spawnPos = new Vector3(Random.Range(-3f, 3f), 1f, 0f);
        if (PhotonNetwork.InRoom) SpawnPlayer();
        else AutoConnect();
    }

    private void AutoConnect()
    {
        PhotonNetwork.NickName = $"Player{Random.Range(0, 100)}";
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("서버 접속!");

        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("로비 입장!");
        PhotonNetwork.JoinRandomOrCreateRoom();
    }

    public override void OnCreatedRoom()
    {
        Debug.Log("방 생성");
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        PhotonNetwork.JoinRandomRoom();
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log($"{newPlayer.NickName} 입장");
    }

    public override void OnJoinedRoom()
    {
        
        SpawnPlayer();
    }

    public void SpawnPlayer()
    {
        if(!PhotonNetwork.InRoom)
            return;
        Debug.Log(PhotonNetwork.LocalPlayer.ActorNumber);
        int actorNum = PhotonNetwork.LocalPlayer.ActorNumber;

        spawnPos = spawnPoints[(actorNum - 1) % spawnPoints.Length].position;
        PhotonNetwork.Instantiate("Player", spawnPos, Quaternion.identity);
    }

}
