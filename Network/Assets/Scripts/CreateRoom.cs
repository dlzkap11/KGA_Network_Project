using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;


//[CreateRoom 설계도]
//
//CreateRoomButton을 누르면:  
//  로비 안이 아니면->그냥 종료                (가드 절)  
//  방 옵션(정원 4)을 만든다  
//  이름 없이(=랜덤 이름) 방 생성을 요청한다
//
//방 입장이 완료되면 (Photon이 자동 호출):  
//  상태 표시를 "InRoom"으로 바꾼다  
//  방 정보를 화면에 갱신한다
//
//누군가 방에 들어오면 / 나가면 (Photon이 각각 자동 호출):  
//  방 정보를 화면에 다시 갱신한다  
//      // 혼자서는 절대 호출되지 않는 콜백이다 — 5절에서 짝이 들어와야 처음 울린다
//
//방 정보 갱신:  
//  현재 방이 없으면->그냥 종료               (가드 절)  
//  "방 이름 (인원/정원)" 형태로 콘솔과 화면에 표시

public class CreateRoom : MonoBehaviourPunCallbacks
{
    public static CreateRoom Instance;


    [SerializeField] PhotonLobbyManager manager;
    [SerializeField] private string roomName = "Room";
    [SerializeField] private TMP_Text roomInfoText;
    private int count = 100;

    private void Awake()
    {
        Instance = this;
    }

    public void OnCreateRoom()
    {
        if (!PhotonNetwork.InLobby && PhotonNetwork.InRoom)
            return;

        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 4;
        roomOptions.IsOpen = true;


        PhotonNetwork.CreateRoom(roomName + $"{Random.Range(0, count)}", roomOptions);
    }

    public void OnJoinRoom(string name)
    {
        if (PhotonNetwork.InRoom && !PhotonNetwork.InLobby)
            return;

        PhotonNetwork.JoinRoom(name);
        
    }

    public override void OnCreatedRoom()
    {
        
        PhotonNetwork.JoinRoom(roomName);
        manager.SetStatus("InRoom");
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.Log($"Code : {returnCode}" + message);
    }

    public override void OnJoinedRoom()
    {
        Debug.Log($"{PhotonNetwork.NickName}이 {roomName}에 입장하였습니다");
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log($"Code : {returnCode}" + message);
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.Log($"Code : {returnCode}" + message);
        PhotonNetwork.JoinRandomRoom();
    }

    public override void OnLeftRoom()
    {
        Debug.Log("룸에서 나가기");
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        base.OnPlayerEnteredRoom(newPlayer);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        base.OnPlayerLeftRoom(otherPlayer);
    }

    public void RefreshRoomInfo()
    {
        Room room = PhotonNetwork.CurrentRoom;


        if(PhotonNetwork.InRoom && room != null)
        {
            roomInfoText.text = $"Current RoomName : {room.Name} ({room.PlayerCount} / {room.MaxPlayers})";
        }
        else
        {
            roomInfoText.text = "Not Found Room";
        }

    }

    [SerializeField] private GameObject root;
    [SerializeField] private TMP_Text roomText;
    [SerializeField] private GameObject prefabButton;
    
    Dictionary<string, RoomInfo> roomDic = new Dictionary<string, RoomInfo>();


    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        RoomListView(roomList);
        RefreshRoomButtons();
    }
    
    public void RoomListView(List<RoomInfo> roomList)
    {
        foreach(RoomInfo room in roomList)
        {
            if (room.RemovedFromList)
                roomDic.Remove(room.Name);
            else
                roomDic[room.Name] = room;
        }
    }

    private void ClearRoomList()
    {
        roomDic.Clear();
        RefreshRoomButtons();
    }

    public void RefreshRoomButtons()
    {
        foreach(Transform child in root.transform)
        {
            Destroy(child.gameObject);
        }

        if (!PhotonNetwork.InLobby)
            return;

        foreach(RoomInfo info in roomDic.Values)
        {
            GameObject item = Instantiate(prefabButton, root.transform);
            item.name = info.Name;
            roomText = item.GetComponentInChildren<TMP_Text>();
            roomText.text = $"{info.Name} ({info.PlayerCount}/{info.MaxPlayers})";
            item.GetComponent<JoinRoom>().Setup(info, OnJoinRoom);
        }

    }
}
