using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class OwnableBall : MonoBehaviourPun
{

    private float moveSpeed = 5;
    private Renderer ren;
    void Start()
    {
        ren = GetComponent<Renderer>();
    }

    // Update is called once per frame
    void Update()
    {

        if (photonView.IsMine)
        {
            float h = 0f;
            float v = 0f;
            if (Input.GetKeyDown(KeyCode.I)) v = 1f;
            if (Input.GetKeyDown(KeyCode.K)) v = -1f;
            if (Input.GetKeyDown(KeyCode.J)) h = -1f;
            if (Input.GetKeyDown(KeyCode.L)) h = 1f;

            Vector3 dir = new Vector3(h, 0, v);
            transform.Translate(dir * moveSpeed * Time.deltaTime);
            ren.material.color = Color.blue;

        }

        if (!photonView.IsMine && Input.GetKeyDown(KeyCode.Z))
        {
            // 소유권을 가져오게끔
            photonView.RequestOwnership();
        }

        if (!photonView.IsMine)
            ren.material.color = Color.yellow;
    }




}
