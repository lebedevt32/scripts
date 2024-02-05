using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Unity.VisualScripting;

public class SpawnPlayers : MonoBehaviour
{
    public GameObject player;
    public float minX, minY, minZ, maxX, maxY, maxZ;

    void Start()
    {
        Vector3 randomPosition = new Vector3(Random.Range(minX, maxX), Random.Range(minY, maxY), Random.Range(minZ, maxZ));
        PhotonNetwork.Instantiate(player.name, randomPosition, Quaternion.Euler(0,180,0));
        Camera.main.transform.position = randomPosition + new Vector3(0, 10, 0);
    }

    public void Respawn(PhotonView corpse)
    {
        //corpse.RPC("DestroyPlayer", RpcTarget.Others, corpse.ViewID);
        PhotonNetwork.Destroy(corpse.gameObject);
        

        Vector3 randomPosition = new Vector3(Random.Range(minX, maxX), Random.Range(minY, maxY), Random.Range(minZ, maxZ));
        PhotonNetwork.Instantiate(player.name, randomPosition, Quaternion.Euler(0, 180, 0));
    }

    [PunRPC]
    public void DestroyPlayer(int viewID)
    {
        PhotonNetwork.Destroy(PhotonView.Find(viewID).gameObject);
    }
}