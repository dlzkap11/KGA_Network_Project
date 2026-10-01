using UnityEngine;
using TMPro;
using Photon.Pun;

public class NameTag : MonoBehaviourPun
{
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private Transform nameCanvas;

    void Start()
    {
        nameText.text = photonView.Owner.NickName;

        if(photonView.IsMine)
            nameText.color = Color.blue;
        else
            nameText.color = Color.red;
    }

    void LateUpdate()
    {
        nameCanvas.rotation = Camera.main.transform.rotation;
    }
}