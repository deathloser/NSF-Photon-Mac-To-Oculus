using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    // public GameObject spawnedPlayerPrefab;
    public GameObject patientPrefab;
    public GameObject caretakerPrefab;
    public GameObject buttonManager;

    GameObject patient;

    // Start is called before the first frame update
    void Start()
    {
        ConnectToServer();
  
    }

    // Update is called once per frame
    void ConnectToServer() {
        PhotonNetwork.ConnectUsingSettings();
        Debug.Log("Try connecting to server...");
    }

    public override void OnConnectedToMaster() {
        Debug.Log("Connected to server!");
        base.OnConnectedToMaster();
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 2;
        roomOptions.IsVisible = true;
        roomOptions.IsOpen = true;
        PhotonNetwork.JoinOrCreateRoom("Room 1", roomOptions, TypedLobby.Default);
    }

    public override void OnJoinedRoom() {

        Debug.Log("Joined a room");
        base.OnJoinedRoom();

        if(PhotonNetwork.IsMasterClient) {
        Debug.Log("Im the master client! Spawning patient.");
        patient = PhotonNetwork.Instantiate(this.patientPrefab.name, new Vector3(0, 1, 1),Quaternion.identity,0);
        Debug.Log(patient);
        PhotonNetwork.Instantiate(this.buttonManager.name, new Vector3(0, 1, 1),Quaternion.identity,0);


        } else {
            Debug.Log("Im the other client. Spawning caretaker.");
            PhotonNetwork.Instantiate(this.caretakerPrefab.name, new Vector3(0, 1, 0),Quaternion.identity,0);

        }
        
    }

    public override void OnPlayerEnteredRoom(Player newPlayer) {
        Debug.Log("A new player has joined the room");
        base.OnPlayerEnteredRoom(newPlayer);
    }

    public override void OnLeftRoom() {
        base.OnLeftRoom();
        // PhotonNetwork.Destroy(spawnedPlayerPrefab);
    }
}
