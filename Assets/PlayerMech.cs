using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using UnityEngine.Rendering.Universal.Internal;
using DG.Tweening;
using static DOTweenExtensions;

//[RequireComponent(typeof(Rigidbody))]
public class PlayerMech : MonoBehaviour
{
    enum MovingMode { walking, driving}
    MovingMode movingMode = MovingMode.walking;

    private Action moveAction;
    private Action lookAction;

    [SerializeField]
    private Transform cameraT;
    private CameraShake camShake;

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        if(cameraT==null) cameraT = Camera.main.transform;
        lookAction = CameraLook;
        moveAction = StandardMove;

        baseFoV = Camera.main.fieldOfView;
        standHeight = transform.position.y;
        camShake = cameraT.GetComponent<CameraShake>();
    }

    Vector3[] rayDirs = new Vector3[]
    {
        new Vector3(0,0,1),
        new Vector3(.5f,0,1),
        new Vector3(-.5f,0,1)
    };

    void Update()
    {
        moveAction?.Invoke();
        lookAction?.Invoke();

        if (Input.GetKeyDown(KeyCode.CapsLock))
            ChangeMode();
    }

    private float baseFoV;
    private float standHeight;
    Tweener fovTween;
    Tweener heightTween;

    void ChangeMode()
    {
        moveAction = moveAction == StandardMove ? DriveMove : StandardMove;
        lookAction = lookAction == null ? CameraLook : null;

        switch (movingMode)
        {
            case MovingMode.walking:
                if (Input.GetKey(KeyCode.W))
                    currentGear = gearSpeeds.Length - 1;
                cameraT.DORotatePitch(-pitchAngle, .35f).SetRelative().SetEase(Ease.InOutSine);
                pitchAngle = 0;
                fovTween = Camera.main.DOFieldOfView(baseFoV + 5, .15f);
                heightTween = transform.DOMoveY(standHeight - .35f, .15f).SetEase(Ease.OutCubic);
                movingMode = MovingMode.driving;
                break;
            case MovingMode.driving:
                yawAngle = transform.localRotation.eulerAngles.y;
                fovTween = Camera.main.DOFieldOfView(baseFoV, .15f);
                heightTween = transform.DOMoveY(standHeight, .15f).SetEase(Ease.OutCubic);
                movingMode = MovingMode.walking;
                break;
        }
    }

    [SerializeField]
    private float moveSpeed = 5f;
    [SerializeField]
    private float strafeSpeed = 2f;

    void StandardMove()
    {
        transform.Translate(Input.GetAxis("Horizontal") * strafeSpeed * Time.smoothDeltaTime, 0, 0);
        transform.Translate(0, 0, Input.GetAxis("Vertical") * moveSpeed * Time.smoothDeltaTime);
    }

    [SerializeField]
    private float[] gearSpeeds = new float[] { 0, 2, 4, 6, 8 };
    private int currentGear = 0;
    private int CurrentGear { 
        get { return currentGear; }  
        set { currentGear = Mathf.Clamp(value, 0, gearSpeeds.Length - 1); } 
    }
    [SerializeField]
    private float turnSpeed = 90;

    void DriveMove()
    {
        transform.localRotation = Quaternion.Euler(0, Input.GetAxis("Horizontal")*turnSpeed*Time.smoothDeltaTime, 0) * transform.localRotation;

        if(Input.GetKeyDown(KeyCode.W)) CurrentGear++;
        if(Input.GetKeyDown(KeyCode.S)) CurrentGear--;

        Vector3 newPos = new Vector3(0, 0, gearSpeeds[CurrentGear] * Time.smoothDeltaTime);
        transform.Translate(newPos);

        RaycastHit hit;
        Ray ray;
        foreach (var dir in rayDirs)
        {
            ray = new Ray(cameraT.position, transform.TransformDirection(dir).normalized);
            if (Physics.Raycast(ray, 2f))
            {
                camShake.StartShake(.5f, .5f);
                ChangeMode();
            }
        }
    }

    const float pitchClamp = 90;
    private float yawAngle = 0;
    private float pitchAngle = 0;

    [SerializeField]
    Vector2 mouseSens = new Vector2(2f,2f);

    void CameraLook()
    {
        Cursor.lockState = CursorLockMode.Locked;

        var mouseDelta = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));
        mouseDelta = Vector2.Scale(mouseDelta, mouseSens);
        pitchAngle = Mathf.Clamp(pitchAngle - mouseDelta.y, -pitchClamp, pitchClamp);
        yawAngle += mouseDelta.x;

        cameraT.localRotation = Quaternion.Euler(pitchAngle, 0, 0);
        transform.localRotation = Quaternion.Euler(0, yawAngle, 0);
    }


}
