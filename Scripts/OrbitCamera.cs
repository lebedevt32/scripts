using UnityEngine;
using System.Collections;
using Photon.Pun;
using Unity.VisualScripting;
using static UnityEngine.GraphicsBuffer;

public class OrbitCamera : MonoBehaviour
{
    [SerializeField] public Transform target;
    public float rotSpeed = 4.5f;
    private float _rotY;
    private float _rotX;
    private Vector3 _offset;
    private Transform _viewpoint;

    private SpellCaster[] _spellCaster;//?

    public void AfterStart()
    {
        _viewpoint = target.Find("CameraFocus");
        _rotY = transform.eulerAngles.y;
        transform.position.Set(target.position.x, target.position.y + 10, target.position.z); // почему-то здесь не прибавляется z поэтому ниже делаю поправку отдельно
        _offset = target.position - transform.position;
        _offset.z -= 4;

        _spellCaster = target.GetComponents<SpellCaster>();//?
    }
    void LateUpdate()
    {
        if (transform != null)
        {
            if (Cursor.lockState == CursorLockMode.Locked)
            {
                bool activeCast = false;
                foreach (SpellCaster cast in _spellCaster)
                {
                    if (cast.GetCastStatus() != SpellCaster.CastStatus.None)
                    {
                        activeCast = true;
                        break;
                    }
                }
                if (activeCast)// изменить условие, и я не уверен что доворот из-за каста должен быть в этом скрипте
                {
                    Quaternion _castingRot = new Quaternion(transform.rotation.x, transform.rotation.y, transform.rotation.z, transform.rotation.w);
                    _castingRot.eulerAngles = new Vector3(0, _castingRot.eulerAngles.y, _castingRot.eulerAngles.z);
                    target.rotation = _castingRot;
                }

                float horInput = Input.GetAxis("Mouse X");
                float vertInput = Input.GetAxis("Mouse Y");
                //if (Input.GetMouseButton(1)) Думал вращать камеру на ПКМ это удобно но разочаровался
                //{
                _rotY += horInput * rotSpeed;
                _rotX += vertInput * rotSpeed;
                //}
            }
            Quaternion rotation = Quaternion.Euler(_rotX, _rotY, 0);
            transform.position = target.position - (rotation * _offset);
            transform.LookAt(_viewpoint);
        }
    }
}