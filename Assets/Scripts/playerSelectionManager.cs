using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using TMPro;

public class playerSelectionManager : MonoBehaviour
{
    public Transform playerSwitcherTransform;

    public Button nextButton;
    public Button prevButton;
    public GameObject[] playerTopModels;
    public int playerSelectionNumber;

    [Header("UI")]
    public TextMeshProUGUI playerModelType_Text;
    public GameObject ui_Selection;
    public GameObject ui_AfterSelection;


    #region Unity Methods
    // Start is called before the first frame update
    void Start()
    {
        ui_Selection.SetActive(true);
        ui_AfterSelection.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    #endregion

    #region UI Callback Methods

    public void nextPlayer()
    {
        playerSelectionNumber += 1;
        if(playerSelectionNumber >= playerTopModels.Length)
        {
            playerSelectionNumber = 0;
        }

        nextButton.enabled = false;
        prevButton.enabled = false;

        StartCoroutine(Rotate(Vector3.up, playerSwitcherTransform, 90, 1f));

        if(playerSelectionNumber == 0 || playerSelectionNumber == 1)
        {
            // This means player model type is attack
            playerModelType_Text.text = "Attack";
        }
        else
        {
            // This means player model type is defend
            playerModelType_Text.text = "Defend";
        }
    }

    public void prevPlayer()
    {
        playerSelectionNumber -= 1;
        if(playerSelectionNumber < 0)
        {
            playerSelectionNumber = playerTopModels.Length - 1;
        }

        nextButton.enabled = false;
        prevButton.enabled = false;
        StartCoroutine(Rotate(Vector3.up, playerSwitcherTransform, -90, 1f));
    }

    public void onSelectButtonClicked()
    {
        ui_Selection.SetActive(false);
        ui_AfterSelection.SetActive(true);

        // this is how we set a custom properties to player 
        ExitGames.Client.Photon.Hashtable playerSelectionProp = new ExitGames.Client.Photon.Hashtable { { MultiplayerARSpiinnerTopGame.PLAYER_SELECTION_NUMBER, playerSelectionNumber} };
        PhotonNetwork.LocalPlayer.SetCustomProperties(playerSelectionProp);
        
    }

    public void onReselectClicked()
    {
        ui_Selection.SetActive(true);
        ui_AfterSelection.SetActive(false);
    }

    public void onBattleButtonClicked()
    {
        SceneLoader.Instance.LoadScene("Scene_Gameplay");
    }

    public void onBackButtonClicked()
    {
        SceneLoader.Instance.LoadScene("Scene_Lobby");
    }
    #endregion
    #region Private Methods
    IEnumerator Rotate(Vector3 axis, Transform transformToRotate, float angle, float duration = 1.0f)
    {
        Quaternion orignalRotation = transformToRotate.rotation;
        Quaternion finalRotation = transformToRotate.rotation * Quaternion.Euler(axis * angle); // this is how we rotate a vector by another vector in unity

        float elapsedTime = 0.0f;
        while(elapsedTime  < duration)
        {
            transformToRotate.rotation = Quaternion.Slerp(orignalRotation, finalRotation, elapsedTime / duration);  // slerp will slowly rotate the object from orignal to final by a amount
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        // when the time is up it will be really close to final rotation but not exactly at final rotation and below line will make it to final rotation 
        transformToRotate.rotation = finalRotation;

        nextButton.enabled = true;
        prevButton.enabled = true;
    }
    #endregion
}
