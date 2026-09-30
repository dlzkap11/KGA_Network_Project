using Photon.Realtime;
using System;
using UnityEngine;
using UnityEngine.UI;

public class JoinRoom : MonoBehaviour
{
    private Button button;
    private string roomName;
    private Action<string> onJoin;

    private void Start()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(OnClick);
    }

    public void Setup(RoomInfo info, Action<string> joinCallback)
    {
        roomName = info.Name;
        onJoin = joinCallback;

    }

    private void OnClick()
    {
        onJoin(roomName);
    }

}
