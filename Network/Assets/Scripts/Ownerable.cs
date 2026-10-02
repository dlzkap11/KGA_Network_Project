using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Unity.VisualScripting;
public class Ownerable : MonoBehaviourPun //, IPunOwnershipCallbacks
{

    private float moveSpeed = 5f;

    private int count = 0;
    float[] arr = { 1f, -1f };
    Renderer ren;
    
    void Start()
    {
        ren = GetComponent<Renderer>();   
    }

    void Update()
    {
        if (photonView.IsMine)
        {
            float h = 0f;
            float v = 0f;
            if (Input.GetKeyDown(KeyCode.I))
            {
                v = arr[count % 2];
                count++;
            }

            if (Input.GetKeyDown(KeyCode.U))
            {
                h = arr[count % 2];
                count++;
            }

            Vector3 dir = new Vector3(h, 0, v);
            transform.Translate(dir * moveSpeed * Time.deltaTime);
            ren.material.color = Color.yellow;
        }

        if(!photonView.IsMine && Input.GetKeyDown(KeyCode.B))
        {
            photonView.RequestOwnership();
            ren.material.color = Color.red;
        }
        
    }

    //public void OnOwnershipRequest(PhotonView targetView, Player requestingPlayer)
    //{
    //    photonView.TransferOwnership(requestingPlayer);
    //}

    //public void OnOwnershipTransfered(PhotonView targetView, Player previousOwner)
    //{
        
    //}

    //public void OnOwnershipTransferFailed(PhotonView targetView, Player senderOfFailedRequest)
    //{
        
    //}

}
