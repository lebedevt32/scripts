using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class SpellInfo : MonoBehaviour
{
    public float damage;
    private Rigidbody _spellCore;

    public bool delayedForming = false;
    public string PrefabName;// используетс€ дл€ создани€ обьекта после каста
    public bool useGravity = false;
    public bool isFixed = false;
    public bool isFreezed = false;
    public float speed = 100;
    public Vector3 cast_vector;


    private void Start()
    {
        _spellCore = GetComponent<Rigidbody>();
    }
    private void Update()
    {
        //if (transform.parent == null)
        //{
        //    _spellbody.AddForce(cast_vector);
        //}
    }
    public void Acting()
    {
        if (PrefabName != null) 
        { 
            //GameObject temp = PhotonNetwork.Instantiate(PrefabName, transform.position, transform.rotation);
            //temp.transform.parent = transform;
            PrefabName = null;
        }

        if (!isFreezed)
        {
            _spellCore.constraints = RigidbodyConstraints.None;
            _spellCore.AddForce(cast_vector * speed);
        }
        else
        {
            _spellCore.constraints = RigidbodyConstraints.FreezeAll;
        }
    }
}
