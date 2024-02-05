using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.UIElements;

public class SpellCaster : MonoBehaviour
{
    [SerializeField] private string castButtonName;
    Vector3 _castPos = new Vector3(2, 2, 1);        //потом сделать так чтобы созданный шарик связывался с анимацией
    Vector3 _scaleCorrection = new Vector3();

    GameObject _camera;
    Vector3 _castVector;

    GameObject _spellContainer = null;
    GameObject _spellForm = null;
    SpellInfo _spellInfo = null; // как в этом скрипте будет написано так спелл и подействует при завершении

    Vector3 _scaleAdder = new Vector3();

    CastStatus _caststatus = CastStatus.None;
    CastStatus _castStatusShift = CastStatus.None;
    CastStatus _castStatusSpace = CastStatus.None;

    PhotonView _spellContainerView;
    PhotonView _view;
    public CastStatus GetCastStatus()
    {
        return _caststatus;
    }

    public enum CastStatus
    {
        None,
        Targeting,
        Forming,
        Acting
    }
    private void Start()
    {
        if (transform.GetComponent<PhotonView>().IsMine)
            _camera = GameObject.FindGameObjectWithTag("MainCamera");
        _view = transform.GetComponent<PhotonView>();
    }

    void Update()
    {
        if (Cursor.lockState == CursorLockMode.Locked)// проверяет также заблокированность курсора
        {
            if (_view.IsMine)
            {
                CheckCastButtons(castButtonName);
            }
        }
    }
    private void CheckCastButtons(string buttonName)// кейсы дублируются везде кроме методов GetButton
    {
        if (_caststatus == CastStatus.None && Input.GetButtonDown(buttonName)/* && Input.GetMouseButton(1)*/)
        {
            _caststatus = CastStatus.Targeting;
            Cast(PlayerPrefs.GetString(buttonName));
        }

        if (Input.GetButton(buttonName)/* && Input.GetMouseButton(1)*/)
        {
            Cast(PlayerPrefs.GetString(buttonName));
        }

        if (_caststatus == CastStatus.Forming && Input.GetButtonUp(buttonName)/* && !Input.GetMouseButtonUp(1)*/)
        {
            _caststatus = CastStatus.Acting;
        }
        else if (Input.GetButtonUp(buttonName) && _caststatus == CastStatus.Acting )// добавить сюда же активацию спелла
        {
            _caststatus = CastStatus.None;// возможно сбрасывать статус стоит не здесь, тут еще же и анимация будет

            _castVector = _camera.transform.Find("SpellTarget").position - _camera.transform.position;// ???                                   
            _spellContainer.GetComponent<SpellInfo>().cast_vector = _castVector;
            _spellContainer.GetComponent<SphereCollider>().isTrigger = false;
            if (_spellContainer.GetComponent<SpellInfo>().useGravity)
                _spellContainer.GetComponent<Rigidbody>().useGravity = true;

            if (_spellInfo.isFixed)
            {
                _spellContainer.transform.parent = null;
                GetComponent<RelativeMovement>().dashSpeed = _spellInfo.speed;
                GetComponent<RelativeMovement>().dash = true;
                _spellContainer.GetComponent<SphereCollider>().isTrigger = true;
            }
            else
            {
                if (_spellInfo.delayedForming == true)
                {
                    Vector3 tempPos = _spellForm.transform.position;
                    Quaternion tempRot = _spellForm.transform.rotation;
                    Vector3 tempScale = _spellForm.transform.localScale;
                    PhotonNetwork.Destroy(_spellForm.gameObject);
                    _spellForm = PhotonNetwork.Instantiate(_spellInfo.PrefabName, tempPos, tempRot);
                    _view.RPC("SetParentOnOthers", RpcTarget.Others, _spellContainer.GetComponent<PhotonView>().ViewID, _spellForm.GetComponent<PhotonView>().ViewID);// --------------------RPC------------------------
                    _spellForm.transform.parent = _spellContainer.transform;
                    _spellForm.transform.localScale = tempScale;
                }
                _spellContainer.transform.parent = null;
                _view.RPC("UnParentOnOthers", RpcTarget.Others, _spellContainer.GetComponent<PhotonView>().ViewID);// -------------------------------RPC---------------------------------
                _spellInfo.Acting();
            }
            _spellInfo = null;
            _spellContainer = null;
            _spellContainerView = null;
            _spellForm = null;
        }
    }
    private void Cast(string SpellCode)
    {
        switch (_caststatus)
        {
            case CastStatus.Targeting:
                if (_spellContainer == null)
                {
                    switch (SpellCode[0])
                    {
                        case '0':
                            Vector3 pos = transform.position;
                            pos.y += 2;
                            _spellContainer = PhotonNetwork.Instantiate("SpellContainer", pos, Quaternion.identity);
                            break;
                        case '1':
                            _spellContainer = PhotonNetwork.Instantiate("SpellContainer", _castPos, Quaternion.identity);
                            break;
                    }
                    _spellContainer.transform.SetParent(transform);
                    _view.RPC("SetParentOnOthers", RpcTarget.Others, _view.ViewID, _spellContainer.GetComponent<PhotonView>().ViewID);// -------------------------------RPC---------------------------------
                    _spellContainerView = _spellContainer.GetComponent<PhotonView>();
                    _spellInfo = _spellContainer.GetComponent<SpellInfo>();
                }
                _caststatus = CastStatus.Forming;
                break;
            case CastStatus.Forming:
                switch (SpellCode[1])
                {
                    case '0':
                        _spellInfo.isFixed = true;
                        _caststatus = CastStatus.Acting;  //если нужно воздействие непосредственно на обьект то создание пропускаем
                        break;
                    case '1':
                        if (_spellForm == null)
                        {
                            _spellForm = PhotonNetwork.Instantiate("Sphere", _spellContainer.transform.position, Quaternion.identity);
                            _view.RPC("SetParentOnOthers", RpcTarget.Others, _spellContainer.GetComponent<PhotonView>().ViewID, _spellForm.GetComponent<PhotonView>().ViewID);// --------------------RPC-------------------------------------
                            _spellForm.transform.SetParent(_spellContainer.transform);
                        }
                        CastScale();

                        if (SpellCode[0] == '1')// и где это должно быть?
                        {
                            CastPosCorrection();
                        }
                        break;
                    case '2':
                        if (_spellForm == null)
                        {
                            _spellForm = PhotonNetwork.Instantiate("Sphere_Solid", _spellContainer.transform.position, Quaternion.identity);
                            _view.RPC("SetParentOnOthers", RpcTarget.Others, _spellContainer.GetComponent<PhotonView>().ViewID, _spellForm.GetComponent<PhotonView>().ViewID);// --------------------RPC-----------------------
                            _spellInfo.useGravity = true;
                            _spellForm.transform.SetParent(_spellContainer.transform);
                        }
                        CastScale();

                        if (SpellCode[0] == '1')// и где это должно быть? x2
                        {
                            CastPosCorrection();
                        }
                        break;
                    case '3':
                        if (_spellForm == null)
                        {
                            _spellForm = PhotonNetwork.Instantiate("Projection_Sphere", _spellContainer.transform.position, Quaternion.identity);
                            _view.RPC("SetParentOnOthers", RpcTarget.Others, _spellContainer.GetComponent<PhotonView>().ViewID, _spellForm.GetComponent<PhotonView>().ViewID);// --------------------RPC-----------------------
                            _spellInfo.delayedForming = true;
                            _spellInfo.PrefabName = "Sphere";
                            _spellForm.transform.SetParent(_spellContainer.transform);
                        }
                        CastScale();

                        if (SpellCode[0] == '1')// и где это должно быть? x2
                        {
                            CastPosCorrection();
                        }
                        break;
                    case '4':
                        if (_spellForm == null)
                        {
                            _spellForm = PhotonNetwork.Instantiate("Projection_Sphere_Solid", _spellContainer.transform.position, Quaternion.identity);
                            _view.RPC("SetParentOnOthers", RpcTarget.Others, _spellContainer.GetComponent<PhotonView>().ViewID, _spellForm.GetComponent<PhotonView>().ViewID);// --------------------RPC-----------------------
                            _spellInfo.delayedForming = true;
                            _spellInfo.PrefabName = "Sphere_Solid";
                            _spellInfo.useGravity = true;
                            _spellForm.transform.SetParent(_spellContainer.transform);
                        }
                        CastScale();

                        if (SpellCode[0] == '1')// и где это должно быть? x2
                        {
                            CastPosCorrection();
                        }
                        break;
                }
                break;
            case CastStatus.Acting:
                switch (SpellCode[2])
                {
                    case '0':
                        _spellInfo.speed += _spellInfo.speed * Time.deltaTime;
                        break;
                    case '1':
                        _spellInfo.isFreezed = true;
                        break;
                }
            break;
        }
    }
    private void CastPosCorrection()
    {
            _scaleCorrection.x = _spellContainer.transform.localScale.x / 2;
            _scaleCorrection.y = _spellContainer.transform.localScale.y / 2;
            _spellContainer.transform.localPosition = _castPos + _scaleCorrection;
    }
    private void CastScale()
    {
            _scaleAdder.Set(2 * Time.deltaTime, 2 * Time.deltaTime, -2 * Time.deltaTime);// -1 на z это костыль из-за неправильного скейла модели
            _spellContainer.transform.localScale += _scaleAdder;
    }
}
