using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class mySynchronization : MonoBehaviour, IPunObservable
{
    Rigidbody rb;
    PhotonView photonview;

    Vector3 networkedPosition;
    Quaternion networkedRotation;

    public bool syncVelocity = true;
    public bool syncAngularVelocity = true;
    public bool isTeleportEnabled = true;

    public float teleportIfDistanceGreaterThan = 1.0f;

    private float distance;
    private float angle;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        photonview = GetComponent<PhotonView>();
    }
    // Start is called before the first frame update
    void Start()
    {
        //PhotonNetwork.SerializationRate = 50;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        if (!photonview.IsMine)
        {
            rb.position = Vector3.MoveTowards(rb.position, networkedPosition, distance * (1.0f/ PhotonNetwork.SerializationRate)); 
            // Serialization rate is the number of times per second that this photon serialize view is called, default is 10 but can try different values(commented statement in start method)
            rb.rotation = Quaternion.RotateTowards(rb.rotation, networkedRotation, angle * (1.0f/ PhotonNetwork.SerializationRate)); // mul by 100 because sometimes rotation change is quick and small and change is not obvious that's why.

        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            // Then photonview is mine and i am the one who controls this player.
            // should send positoin, velocity,etc. data to other players.
            stream.SendNext(rb.position);
            stream.SendNext(rb.rotation);

            if (syncVelocity)
            {
                stream.SendNext(rb.velocity);
            }
            if (syncAngularVelocity)
            {
                stream.SendNext(rb.angularVelocity);
            }


        }
        else
        {
            // if it is not wrting then it is reading
            // called on my player gameobject that exist in remote player's game
            networkedPosition = (Vector3)stream.ReceiveNext();
            networkedRotation = (Quaternion)stream.ReceiveNext();

            if (isTeleportEnabled)
            {
                if(Vector3.Distance(rb.position, networkedPosition) > teleportIfDistanceGreaterThan)
                {
                    rb.position = networkedPosition;
                }
            }

            if(syncVelocity || syncAngularVelocity)
            {
                float lag = Mathf.Abs((float)(PhotonNetwork.Time - info.SentServerTime));
                // photonNetwork.time is usede to sync time across all players, it is actually the server time thus it will be same across each client
                // info.servertime is the time at which the data sends
                // thus we get delay time between sending and recieving the data i.e ping

                if (syncVelocity)
                {
                    rb.velocity = (Vector3)stream.ReceiveNext();
                    networkedPosition += rb.velocity * lag;
                    distance = Vector3.Distance(rb.position, networkedPosition);
                }

                if (syncAngularVelocity)
                {
                    rb.angularVelocity = (Vector3)stream.ReceiveNext();
                    networkedRotation = Quaternion.Euler(rb.angularVelocity * lag) * networkedRotation;

                    angle = Quaternion.Angle(rb.rotation, networkedRotation);
                }

            }


        }
    }
}
