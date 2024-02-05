using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using static UnityEngine.GraphicsBuffer;

public class PlayerCharacter : MonoBehaviour
{
    private PhotonView _view;
    private UIController _ui;
    [SerializeField] private float _health;
    void Start()
    {
        _view = GetComponent<PhotonView>();
        if (GetComponent<PhotonView>().IsMine)
        {
            _ui = GameObject.FindGameObjectWithTag("UIController").GetComponent<UIController>();
            _health = 10;
            _ui.UpdateHitBar(_health);
        }
    }
    /// <summary>
    /// Наносит урон и возвращает true если игрок погиб и false если выжил
    /// </summary>
    /// <param name="damage"></param>
    /// <returns></returns>
    public void Hurt(float damage)
    {
        if (_view.IsMine)
        {
            _health -= damage;
            if (_health <= 0)
                GameObject.FindGameObjectWithTag("GameManagers").GetComponent<SpawnPlayers>().Respawn(_view);
            _ui.UpdateHitBar(_health);
        }
    }

}