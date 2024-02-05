using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RPC_Helper : MonoBehaviour
{
    [PunRPC]
    private void SetParentOnOthers(int parentId, int childId)
    {
        PhotonNetwork.GetPhotonView(childId).transform.parent = PhotonNetwork.GetPhotonView(parentId).transform;
    }
    [PunRPC]
    private void UnParentOnOthers(int childId)
    {
        PhotonNetwork.GetPhotonView(childId).transform.parent = null;
    }
}
