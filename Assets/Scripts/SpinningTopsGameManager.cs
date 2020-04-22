using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System;
using TMPro;

public class SpinningTopsGameManager : MonoBehaviourPunCallbacks
{
    [Header("UI")]
    public GameObject ui_InformPanelGameobject;
    public TextMeshProUGUI ui_InformText;
    public GameObject searchForGamesButton;

    // Start is called before the first frame update
    void Start()
    {
        ui_InformPanelGameobject.SetActive(true);
        ui_InformText.text = " Search for game to begin Battle! ";
    }

    // Update is called once per frame
    void Update()
    {

    }

    #region UI Callback Methods
    public void JoinRandomRoom()
    {
        ui_InformText.text = " Searching for available rooms..... ";
        PhotonNetwork.JoinRandomRoom();
        searchForGamesButton.SetActive(false);
    }

    public void onQuitMatchButtonClicked()
    {
        if (PhotonNetwork.InRoom)
        {
            PhotonNetwork.LeaveRoom();
        }
        else
        {
            SceneLoader.Instance.LoadScene("Scene_Lobby");
        }
    }
    #endregion

    #region Photon Callback Methods
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log(message);
        ui_InformText.text = message;
        CreateAndJoinRoom();
    }

    public override void OnLeftRoom()
    {
        SceneLoader.Instance.LoadScene("Scene_Lobby");
    }

    public override void OnJoinedRoom()
    {
        if(PhotonNetwork.CurrentRoom.PlayerCount == 1)
        {
            ui_InformText.text = " Joined to " + PhotonNetwork.CurrentRoom.Name + " waiting for other players";
        }
        else
        {
            ui_InformText.text = " Joined to " + PhotonNetwork.CurrentRoom.Name;
            StartCoroutine(DeactivateAfterSeconds(ui_InformPanelGameobject, 2f));
        }
        Debug.Log(PhotonNetwork.NickName + " joined to " + PhotonNetwork.CurrentRoom.Name);
    }

    public override void OnPlayerEnteredRoom(Player newPlayer) // this method is called when a remote player joins the room that we are in
    {
        Debug.Log(newPlayer.NickName + " joined to " + PhotonNetwork.CurrentRoom.Name + "Player count " + PhotonNetwork.CurrentRoom.PlayerCount);

        ui_InformText.text = newPlayer.NickName + " joined to " + PhotonNetwork.CurrentRoom.Name + "Player count " + PhotonNetwork.CurrentRoom.PlayerCount;

        StartCoroutine(DeactivateAfterSeconds(ui_InformPanelGameobject, 2f));
    }
    #endregion
    #region private methods
    private void CreateAndJoinRoom()
    {
        string randomRoomName = "Room" + UnityEngine.Random.Range(0, 1000);

        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 2;

        // creating a room
        PhotonNetwork.CreateRoom(randomRoomName, roomOptions);
    }

    IEnumerator DeactivateAfterSeconds(GameObject _gameobject, float seconds)
    {
        yield return new WaitForSeconds(seconds);
        _gameobject.SetActive(false);
    }
    #endregion
}
