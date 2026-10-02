using UnityEngine;
using Photon.Realtime;
using ExitGames.Client.Photon;
using Photon.Pun;
using TMPro;
public class ChatManager : MonoBehaviour, IOnEventCallback
{

    [SerializeField] private TMP_InputField inputField;
    [SerializeField] private TMP_Text chatLog;
    [SerializeField] private ReceiverGroup chatReceivers = ReceiverGroup.All;


    private void OnEnable()
    {
        PhotonNetwork.AddCallbackTarget(this);
    }
    private void OnDisable()
    {
        PhotonNetwork.RemoveCallbackTarget(this);
    }

    private void OnSubmit(string text)
    {
        if(!string.IsNullOrWhiteSpace(text))
            SendChat(text);

        inputField.text = "";
        inputField.ActivateInputField();
    }

    private void SendChat(string message)
    {
        RaiseEventOptions options = new RaiseEventOptions();
        options.Receivers = chatReceivers;

        PhotonNetwork.RaiseEvent(1, message, options, SendOptions.SendReliable);
    }

    public void OnEvent(EventData photonEvent)
    {
        if (photonEvent.Code == 1)
        {
            object data = photonEvent.CustomData; // 여러 값을 보낸다면 배열로 받기

            chatLog.text = string.Join(data.ToString(), "\n");
        }
    }
}
