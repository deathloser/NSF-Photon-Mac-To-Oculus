using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;
using Photon.Pun;
using System;


public class CaretakerPlayer : MonoBehaviour
{
    //mouse look
    public enum RotationAxes {
    MouseXAndY = 0,
    MouseX = 1,
    MouseY = 2
    }
    public RotationAxes axes = RotationAxes.MouseXAndY;

    public float minimumVert = -45.0f;
    public float maximumVert = 45.0f;
    public float sensitivityHor = 9.0f;
    public float sensitivityVert = 9.0f;
    private float rotationX = 0;

    public GameObject canvas;
    public GameObject uiTextPrefab;

    //move direction

    public Vector3 moveDirection;
    public int speed = 6;
    public float gravity = -9.8f;
    public CharacterController controller;
    public float MoveSpeed;
    public float RotateSpeed;

    PhotonView photonView;
    public Camera cam;
    public GameObject patientPrefab;
    public GameObject patientCamera;
    public GameObject buttonManager;
    public AudioSource track1;

    Vector3 playerTransform;
    Vector3 cameraTransform;
    Vector3 offset;

    //end 
    void Awake() {
    controller = GetComponent<CharacterController>();   
    photonView = GetComponent<PhotonView>();
    // track1 = GetComponent<AudioSource>();

    // track1.clip = Resources.Load<AudioClip>("237_jojohnson");
    // track1.Play();
    }


    [PunRPC]
    void ChatMessage(String audioName)
    {
        Debug.Log("task on click called");
        // AudioSource track1 = GetComponent<AudioSource>();
        // track1.clip = Resources.Load<AudioClip>(audioName);
        // track1.Play();
    }

    private void Start() {
    patientCamera = GameObject.Find("PatientCamera");


    //camera mouse look
    Rigidbody body = GetComponent<Rigidbody>();
    if (body != null) {
            body.freezeRotation = true;
        }
    if(!photonView.IsMine) {
        Debug.Log("Destroying");
        cam.enabled = false;

    }

    canvas = GameObject.Find("Canvas");
    buttonManager = GameObject.Find("ButtonManager(Clone)");
    buttonManager.SetActive(false);
    // patientPrefab = GameObject.Find("PatientPrefab(Clone)");
    // patientCamera = patientPrefab.transform.GetChild(0).gameObject;
    // patientCamera.SetActive(false);


    }



    void Update() {

        // if(patientCamera) {
        //     Debug.Log("I found the patient camera");
        // } else {
        //     Debug.Log("I didnt find it");
        // }

        if(!photonView.IsMine) {
            return;
            
        }

        
        MoveThePlayer();
        CameraFollow();
        
        
        
    }
    void MoveThePlayer() {
        if (axes==RotationAxes.MouseX) {
        transform.Rotate(0,Input.GetAxis("Mouse X") * sensitivityHor, 0);
        
    }
        else if (axes==RotationAxes.MouseY) {
            rotationX -= Input.GetAxis("Mouse Y") * sensitivityVert;
            rotationX = Mathf.Clamp(rotationX, minimumVert, maximumVert);

            float rotationY = transform.localEulerAngles.y;
            transform.localEulerAngles = new Vector3(rotationX,rotationY,0);


        }

        else {

            rotationX -= Input.GetAxis("Mouse Y") * sensitivityVert;
            rotationX = Mathf.Clamp(rotationX,minimumVert,maximumVert);

            float delta = Input.GetAxis("Mouse X") * sensitivityHor;
            float rotationY = transform.localEulerAngles.y + delta;

            transform.localEulerAngles = new Vector3(rotationX, rotationY,0);
        }

        moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical"));

        moveDirection = transform.TransformDirection(moveDirection);
        moveDirection *= speed * Time.deltaTime;
        moveDirection.y = gravity;

        controller.Move(moveDirection);


    }

    void CameraFollow() {
        cameraTransform = playerTransform + offset;
    }
}
