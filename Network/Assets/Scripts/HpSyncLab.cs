using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;

public enum Mode
{
    Stream,
    RPC,
    Property
}

public class HpSyncLab : MonoBehaviourPunCallbacks, IPunObservable
{
    [SerializeField] private Mode mode;
    [SerializeField] private TMP_Text hpText;
    [SerializeField] private int hp = 100;

    void Start()
    {
        if(mode == Mode.Property)
        {
            if (photonView.Owner.CustomProperties.TryGetValue("hp", out object value))
            {
                hp = (int)value;
            }
            
        }
        RefreshText();
    }

    void Update()
    {
        if (!photonView.IsMine)
            return;

        if (Input.GetKeyDown(KeyCode.H))
        {
            ChangeHp(10);
        }
    }

    public void ChangeHp(int amout)
    {
        hp -= amout;
        RefreshText();

        switch (mode)
        {
            case Mode.RPC:
                photonView.RPC(nameof(RpcSetHp), RpcTarget.All, hp);
                break;
            case Mode.Property:
                Hashtable props = new Hashtable();
                props["hp"] = hp;
                photonView.Owner.SetCustomProperties(props);
                break;
            case Mode.Stream:
                break;
        }
    }

    public void RefreshText()
    {
        if (hpText != null)
            hpText.text = $"HP {hp}";

        Debug.Log($"{mode}");
    }

    [PunRPC]
    public void RpcSetHp(int newHp)
    {
        hp = newHp;
        RefreshText();
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        if (mode != Mode.Property)
            return;

        if (targetPlayer != photonView.Owner)
            return;

        //if (changedProps.ContainsKey("hp"))
            //hp = (int)changedProps["hp"];

        if (changedProps.TryGetValue("hp", out object value))
        {
            hp = (int)value;
            RefreshText();
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (mode != Mode.Stream)
            return;

        if (stream.IsReading)
        {
            hp = (int)stream.ReceiveNext();
            RefreshText();
        }
        else if (stream.IsWriting)
        {
            stream.SendNext(hp);
        }
    }
}
