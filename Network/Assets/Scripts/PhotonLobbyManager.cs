using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PhotonLobbyManager : MonoBehaviourPunCallbacks
{
    public static PhotonLobbyManager Instance;

    [SerializeField] private TMP_Text statusText;
    [SerializeField] private TMP_Text roomInfoText;
    [SerializeField] private TMP_Text lobbyStatsText;
    [SerializeField] private TMP_Text nickNameText;
    [SerializeField] private TMP_Text gameVersionText;
    [SerializeField] private TMP_Text playerText;
    [SerializeField] private TMP_InputField nickNameInputField;

    [SerializeField] private Button connectButton;
    [SerializeField] private Button disconnectButton;


    private string lastStatus;

    private string nickName;
    private string gameVersion;

    void Start()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
        //nickNameInputField.gameObject.SetActive(false);
        //nickNameInputField.onValueChanged.AddListener(OnChanged); // 글자 입력할 때마다
        //nickNameInputField.onEndEdit.AddListener(OnEndEdit);      // 엔터 또는 포커스 해제 시

        RefreshButton();
        SetGameVersion("1.0");
        SetStatus("Disconnect");
    }

    

    public void OnClickConnect()
    {
        if (PhotonNetwork.IsConnected) // 이미 연결 중이면 리턴
            return;
        SetNickName();


        
        PhotonNetwork.ConnectUsingSettings(); // 연결 중
        PhotonNetwork.GameVersion = gameVersion;
        SetStatus("Connecting");
        RefreshButton();

    }

    void OnChanged(string text) => Debug.Log("변경: " + text);
    void OnEndEdit(string text) => Debug.Log("입력 완료: " + text);

    public void OnClickDisconnect()
    {
        if (!PhotonNetwork.IsConnected) // 이미 연결 중이 아니라면 리턴
            return;

        if (PhotonNetwork.InLobby)
        {
            Debug.Log("로비 나가기");
            PhotonNetwork.LeaveLobby(); // 만약 로비에 있으면 로비만 나가기
        }
        else if (PhotonNetwork.InRoom)
        {
            PhotonNetwork.LeaveRoom();
        }
        else
        {
            Debug.Log("연결 끊기");
            PhotonNetwork.Disconnect(); // 연결 끊는 중
        }

    }

    public override void OnConnectedToMaster()
    {
        SetStatus("Connect To Master");
        

        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        base.OnJoinedLobby();
        SetStatus("InLobby");
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        SetStatus($"Disconnected - {cause}");
       
        if(cause == DisconnectCause.ServerTimeout || cause == DisconnectCause.ClientTimeout)
        {
            if (PhotonNetwork.Reconnect())
            {
                SetStatus($"Reconnecting ({cause})");
            }
        }

        RefreshButton();
    }

    public override void OnLeftLobby()
    {
        SetStatus("Connect");
    }


    public void SetNickName()
    {
        nickNameInputField.gameObject.SetActive(true);
        nickName = nickNameInputField.text;

        PhotonNetwork.NickName = nickName;
        nickNameText.text = nickName;
    }

    public void SetStatus(string status)
    {
        if (status == lastStatus)
            return;

        lastStatus = status;

        if (statusText != null)
        {
            statusText.text = status;
        }
    }

    private void RefreshButton()
    {
        ClientState state = PhotonNetwork.NetworkClientState;

        bool offline = (state == ClientState.PeerCreated) || (state == ClientState.Disconnected);
        connectButton.interactable = offline;
        disconnectButton.interactable = !offline;

    }

    public void RefreshLobbyStats()
    {
        if(PhotonNetwork.InLobby)
            lobbyStatsText.text = $"Lobby Players : {PhotonNetwork.CountOfPlayersOnMaster} \nLobby Counts : {PhotonNetwork.CountOfRooms} \nPlayer? {PhotonNetwork.CountOfPlayers}";
        else
        {
            lobbyStatsText.text = "No Lobby Here";
        }
    }

    private void SetGameVersion(string version)
    {
        gameVersion = version;
        gameVersionText.text = version;
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        RefreshPlayerList();
    }

    private void RefreshPlayerList()
    {
        string text = "No Room";

        if (PhotonNetwork.InRoom)
        {
            StringBuilder sb = new StringBuilder();

            foreach(Player p in PhotonNetwork.PlayerList)
            {
                sb.AppendLine($"{p.ActorNumber}. {p.NickName}{(p.IsMasterClient ? " +" : " ")}");
                if (IsReady(p))
                    sb.Append("[Ready!]");
                
                
            }
            text = sb.ToString();
            playerText.text = text;
        }
        playerText.text = text;
    }

    [SerializeField] private Button readyButton;
    [SerializeField] private Button startButton;

    public void OnClickReadyToggle()
    {
        if (!PhotonNetwork.InRoom)
            return;


        Debug.Log("준비!");

        bool ready = IsReady(PhotonNetwork.LocalPlayer);

        Hashtable props = new Hashtable { { "ready", ready } };
        PhotonNetwork.LocalPlayer.SetCustomProperties(props);
    }

    public bool IsReady(Player p)
    {
        if (p.CustomProperties.TryGetValue("ready", out object value))
            return (bool)value;
        
        return false;
    }

    private bool AllReady()
    {
        foreach(Player p in PhotonNetwork.PlayerList)
        {
            if(!IsReady(p))
                return false;
        }
        return true;
    }

    public void OnClickStart()
    {
        if (!PhotonNetwork.IsMasterClient || !AllReady())
            return;

        Debug.Log("게임 시작");
        PhotonNetwork.CurrentRoom.IsOpen = false;
        PhotonNetwork.LoadLevel("GameScene");
        //SceneManager.LoadSceneAsync("GameScene", LoadSceneMode.Single);
        //SceneManager.LoadScene("GameScene");
    }

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        Debug.Log($"뉴 방장 : {newMasterClient.NickName}");
        RefreshPlayerList();
    }

    public void OnClickTransferMaster()
    {
        if (!PhotonNetwork.InRoom || !PhotonNetwork.IsMasterClient)
            return;

        foreach (Player p in PhotonNetwork.PlayerList)
        {
            if (!p.IsMasterClient)
            {
                PhotonNetwork.SetMasterClient(p);
                return;
            }
                
        }

    }


}