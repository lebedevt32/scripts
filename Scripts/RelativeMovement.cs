using UnityEngine;
using System.Collections;
using Photon.Pun;
using System.Net.Http.Headers;

[RequireComponent(typeof(CharacterController))]
public class RelativeMovement : MonoBehaviour
{
    [SerializeField] private Transform target;
    readonly float defaultmoveSpeed = 6.0f;
    public float moveSpeed = 6.0f;
    public float rotSpeed = 15.0f;
    public bool dash = false;
    public float dashSpeed = 12.0f;
    public float dashTime = 1f;
    private CharacterController _charController;
    private Animator anim;
    private PhotonView view;


    void Start()
    {
        _charController = GetComponent<CharacterController>();
        view = GetComponent<PhotonView>();
        target = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Transform>();
        anim = GetComponent<Animator>();


        if (view.Owner.IsLocal)
        {
            Camera.main.GetComponent<OrbitCamera>().target = gameObject.GetComponent<Transform>();
            Camera.main.GetComponent<OrbitCamera>().AfterStart();
        }
        else
        {
            _charController.enabled = false;
        }
     }

    void Update()
    {
        if (view.IsMine)
        {
            if (!dash)
            {
                Vector3 movement = Vector3.zero;
                float horInput = Input.GetAxis("Horizontal");
                float vertInput = Input.GetAxis("Vertical");
                if (horInput != 0 || vertInput != 0)
                {
                    movement.x = horInput * moveSpeed;
                    movement.z = vertInput * moveSpeed;
                    movement = Vector3.ClampMagnitude(movement, moveSpeed);
                    Quaternion tmp = target.rotation;
                    target.eulerAngles = new Vector3(0, target.eulerAngles.y, 0);
                    movement = target.TransformDirection(movement);
                    target.rotation = tmp;
                    Quaternion direction = Quaternion.LookRotation(movement);
                    transform.rotation = Quaternion.Lerp(transform.rotation, direction, rotSpeed * Time.deltaTime);
                    movement *= Time.deltaTime;
                    _charController.Move(movement);
                }
                anim.SetFloat("Speed", movement.sqrMagnitude);
            }
            else if (dash) 
            {
                if(dashTime > 0)
                   Dash(dashSpeed);
                else
                {
                    dash = false;
                    dashTime = 1f;
                }
            }
        }
    }

    public void ResetMoveSpeed()
    {
        moveSpeed = defaultmoveSpeed;
    }
    private void Dash(float speed)
    {
        Vector3 movement = Vector3.zero;
        float horInput = Input.GetAxis("Horizontal");
        float vertInput = Input.GetAxis("Vertical");
        if (horInput != 0 || vertInput != 0)
        {
            movement.x = horInput * speed;
            movement.z = vertInput * speed;
            movement = Vector3.ClampMagnitude(movement, speed);
            Quaternion tmp = target.rotation;
            target.eulerAngles = new Vector3(0, target.eulerAngles.y, 0);
            movement = target.TransformDirection(movement);
            target.rotation = tmp;
            Quaternion direction = Quaternion.LookRotation(movement);
            transform.rotation = Quaternion.Lerp(transform.rotation, direction, rotSpeed * Time.deltaTime);
            movement *= Time.deltaTime;
            _charController.Move(movement);
            dashTime -= Time.deltaTime;
        }
        anim.SetFloat("Speed", movement.sqrMagnitude);
    }
}