using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using UnityEngine.SceneManagement;

public class lobbyManager : MonoBehaviourPunCallbacks
{
    [Header("Login UI")]
    public InputField playerNameInputField;
    public GameObject ui_LoginGameobject;


    [Header("Lobby UI")]
    public GameObject ui_LobbyGameObject;
    public GameObject UI_3DgameObject;

    [Header("Connection Status Gameobject")]
    public GameObject UI_ConnectionStatusGameObject;
    public Text connectionStatusText;
    public bool showConnectionStatus;


    #region UNITY Methods

    // Start is called before the first frame update
    void Start()
    {
        if (PhotonNetwork.IsConnected)
        {
            // activating only lobby UI since we already connected to photon
            ui_LobbyGameObject.SetActive(true);
            UI_3DgameObject.SetActive(true);
            UI_ConnectionStatusGameObject.SetActive(false);

            ui_LoginGameobject.SetActive(false);
        }
        else
        {
            // Activating only Login UI since we did not connect to photon yet
            ui_LobbyGameObject.SetActive(false);
            UI_3DgameObject.SetActive(false);
            UI_ConnectionStatusGameObject.SetActive(false);

            ui_LoginGameobject.SetActive(true);
        }

    }

    // Update is called once per frame
    void Update()
    {
        if (showConnectionStatus)
        {
            connectionStatusText.text = "Connection Status: " + PhotonNetwork.NetworkClientState;

        }
    }
    #endregion
    #region UI Callback Methods 

    public void OnEnterGameButtonClicked()
    {
        string playerName = playerNameInputField.text;

        if (!string.IsNullOrEmpty(playerName))
        {
            ui_LobbyGameObject.SetActive(false);
            UI_3DgameObject.SetActive(false);
            ui_LoginGameobject.SetActive(false);

            showConnectionStatus = true;
            UI_ConnectionStatusGameObject.SetActive(true);

            if (!PhotonNetwork.IsConnected)
            {
                PhotonNetwork.LocalPlayer.NickName = playerName;
                PhotonNetwork.ConnectUsingSettings();
            }
        }
        else
        {
            Debug.Log("Invalid Player Name");
        }
    }

    public void OnQuickMatchButtonClicked()
    {
        SceneLoader.Instance.LoadScene("Scene_PlayerSelection");
        //SceneManager.LoadScene("Scene_Loading");
    }

    #endregion

    #region PHOTON Callback Methods

    public override void OnConnected()  // called when internet connection is established, i.e first method called when we press enter game button
    {
        Debug.Log("Internet connection established!");
    }

    public override void OnConnectedToMaster() // called when connection to photon server is successfully established
    {
        Debug.Log(PhotonNetwork.LocalPlayer.NickName + " is connected to Photon Server");

        ui_LobbyGameObject.SetActive(true);
        UI_3DgameObject.SetActive(true);
        ui_LoginGameobject.SetActive(false);

        UI_ConnectionStatusGameObject.SetActive(false);
    }

    #endregion
}
