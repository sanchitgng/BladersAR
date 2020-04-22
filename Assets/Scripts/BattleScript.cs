using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using TMPro;
using System;

public class BattleScript : MonoBehaviourPun
{
    public Spinner spinnerScript;
    public GameObject ui_3d_Gameobject;
    public GameObject deathPanel_UI_Prefab;

    private GameObject deathPanel_UI_GameObject;

    private float startSpinSpeed;
    private float currentSpinSpeed;
    public Image spinSpeedBar_Image;
    public TextMeshProUGUI spinSpeedRatio_text;

    public float commonDamageCoefficient = 0.04f;

    public bool isAttacker;
    public bool isDefender;
    private bool isDead = false;

    private Rigidbody rb;

    [Header("Player Type damage Coefficients")]
    public float doDamage_Coefficient_Attacker = 10f;  // do more damage than defender- ADVANTAGE
    public float getDamaged_Coefficient_Attacker = 1.2f; // gets more damage- DISADVANTAGE

    public float doDamage_Coefficient_Deefnder = 0.75f; // Do less damage- DISADVANTAGE
    public float getDamaged_Coefficient_Defender = 0.2f; // gets less damage- ADVANTAGE

    private void Awake()
    {
        startSpinSpeed = spinnerScript.spinSpeed;
        currentSpinSpeed = spinnerScript.spinSpeed;

        spinSpeedBar_Image.fillAmount = currentSpinSpeed / startSpinSpeed;
    }

    private void checkPlayerType()
    {
        if (gameObject.name.Contains("Attacker"))
        {
            isAttacker = true;
            isDefender = false;
        }
        else if (gameObject.name.Contains("Defender"))
        {
            isDefender = true;
            isAttacker = false;

            spinnerScript.spinSpeed = 4400;

            startSpinSpeed = spinnerScript.spinSpeed;
            currentSpinSpeed = spinnerScript.spinSpeed;

            spinSpeedRatio_text.text = currentSpinSpeed + "/" + startSpinSpeed;
        }
    }

    private void Start()
    {
        checkPlayerType();
        rb = GetComponent<Rigidbody>();

    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // comparing speed of the spinner tops
            float mySpeed = gameObject.GetComponent<Rigidbody>().velocity.magnitude;
            float otherPlayerSpeed = collision.collider.gameObject.GetComponent<Rigidbody>().velocity.magnitude;

            if(mySpeed > otherPlayerSpeed)
            {
                float default_DamageAmount = gameObject.GetComponent<Rigidbody>().velocity.magnitude * 3600 * commonDamageCoefficient;

                if (isAttacker)
                {
                    default_DamageAmount *= doDamage_Coefficient_Attacker;
                }
                else if (isDefender)
                {
                    default_DamageAmount *= doDamage_Coefficient_Deefnder;
                }

                if (collision.collider.gameObject.GetComponent<PhotonView>().IsMine)
                {
                    // damage the other player
                    // dont' need else as every player will locally check his speed own speed with remote player and apply damage to the slower player
                    collision.collider.gameObject.GetComponent<PhotonView>().RPC("DoDamage", RpcTarget.AllBuffered, default_DamageAmount);
                }


            }
           
        }
    }

    [PunRPC]
    public void DoDamage(float damageAmount)
    {
        if (!isDead)
        {
            if (isAttacker)
            {
                damageAmount *= getDamaged_Coefficient_Attacker;

                if(damageAmount > 1000)
                {
                    damageAmount = 400f;
                }
            }
            else if (isDefender)
            {
                damageAmount *= getDamaged_Coefficient_Defender;
            }

            spinnerScript.spinSpeed -= damageAmount;
            currentSpinSpeed = spinnerScript.spinSpeed;

            spinSpeedBar_Image.fillAmount = currentSpinSpeed / startSpinSpeed;
            spinSpeedRatio_text.text = currentSpinSpeed.ToString("F0") + "/" + startSpinSpeed;

            if (currentSpinSpeed < 100)
            {
                // player dies
                Die();
            }
        }
        
    }

    private void Die()
    {
        isDead = true;
        GetComponent<Movement_Controller>().enabled = false;
        rb.freezeRotation = false;
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        spinnerScript.spinSpeed = 0f;

        ui_3d_Gameobject.SetActive(false);
        if (photonView.IsMine)
        {
            // countdown for respawn
            StartCoroutine(Respawn());
        }
        

    }

    IEnumerator Respawn()
    {
        GameObject canvasGameobject = GameObject.Find("Canvas");

        if(deathPanel_UI_GameObject == null)
        {
            deathPanel_UI_GameObject = Instantiate(deathPanel_UI_Prefab);
        }
        else
        {
            deathPanel_UI_GameObject.SetActive(true);
        }

        Text respawnTimeText = deathPanel_UI_GameObject.transform.Find("RespawnTimeText").GetComponent<Text>();

        float respawnTime = 8.0f;

        respawnTimeText.text = respawnTime.ToString(".00");

        while(respawnTime > 0)
        {
            yield return new WaitForSeconds(1.0f);
            respawnTime -= 1.0f;
            respawnTimeText.text = respawnTime.ToString(".00");

            GetComponent<Movement_Controller>().enabled = false; 
            //we already disabled movement control in do Damge RPC method, just want to disable again locally so that it is sure that player can't move.
        }

        deathPanel_UI_GameObject.SetActive(false);
        GetComponent<Movement_Controller>().enabled = true;

        photonView.RPC("ReBorn", RpcTarget.AllBuffered);
        
    }

    [PunRPC]
    public void ReBorn()
    {
        isDead = false;
        spinnerScript.spinSpeed = startSpinSpeed;
        currentSpinSpeed = spinnerScript.spinSpeed;

        spinSpeedBar_Image.fillAmount = currentSpinSpeed / startSpinSpeed;
        spinSpeedRatio_text.text = currentSpinSpeed + "/" + startSpinSpeed;

        rb.freezeRotation = true;
        transform.rotation = Quaternion.Euler(Vector3.zero);

        ui_3d_Gameobject.SetActive(true);

    }
}
