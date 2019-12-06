using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;
public class projectile : PunBehaviour
{
    public int owner;
    PlayerStats creator;  //who created the projectile
    Vector3 angle;
    bool facingright;
    float timer;
    int damage;

    private void Start()
    {
       /* creator = PhotonConnection.GetInstance().GetPlayerById(owner).GetComponent<PlayerStats>();
        damage = (int)(creator.m_DamageRange);*/
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (other.GetComponent<Player>().ID != owner)
            {
                StartCoroutine(DeathTime());
                GetComponent<Rigidbody>().velocity = Vector3.zero;
                GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
                this.gameObject.SetActive(false);
            }

        }

        if (other.gameObject.CompareTag("ENEMIES"))
        {
            //DAÑO AÑ ENEMIGO
            StartCoroutine(DeathTime());
            GetComponent<Rigidbody>().velocity = Vector3.zero;
            GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
            this.gameObject.SetActive(false);
        }
    }

    /*public void OnEnable()
    {
        /*if (PhotonConnection.GetInstance().GetPlayerById(owner).GetComponent<Player>().ranged.stats.mod1 == Items.Modifier.BULLETPOINT ||
            PhotonConnection.GetInstance().GetPlayerById(owner).GetComponent<Player>().ranged.stats.mod2 == Items.Modifier.BULLETPOINT)
            timer = 2.25f;
        else
            timer = 1.5f;
    }*/

    IEnumerator DeathTime()
    {
        yield return new WaitForEndOfFrame();
        this.gameObject.SetActive(false);
    }


    public void moveProjectile(bool direction, float force)
    {
        if (!direction)
        {
            this.gameObject.GetComponent<Rigidbody>().AddForce(Vector3.right * force);
        }
        else
        {
            this.gameObject.GetComponent<Rigidbody>().AddForce(Vector3.left * force);
        }
    }
    public void Update()
    {
        timer -= Time.deltaTime;
        if (timer <= 0)
        {
            GetComponent<Rigidbody>().velocity = Vector3.zero;
            GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
            this.gameObject.SetActive(false);
        }

    }

    public void PrepareRPC(object[] parameter)
    {
        StartCoroutine(RPCForArrow(parameter));
    }

    public IEnumerator RPCForArrow(object[] parameters)
    {
        yield return new WaitForEndOfFrame();
        PhotonNetwork.RPC(GetComponent<PhotonView>(), "ArrowStart", PhotonTargets.AllBuffered, false, parameters);
    }

    [PunRPC]
    public void ArrowStart(object[] _parameters)
    {
        bool _facingRight = (bool)_parameters[1];
        int _owner = (int)_parameters[2];
        Vector3 _angular = (Vector3)_parameters[0];


        gameObject.transform.eulerAngles = _angular;
        owner = _owner;
        facingright = _facingRight;
        if (PhotonConnection.GetInstance().GetPlayerById(owner).GetComponent<Player>().ranged.stats.mod1 == Items.Modifier.LOWDRAG ||
            PhotonConnection.GetInstance().GetPlayerById(owner).GetComponent<Player>().ranged.stats.mod2 == Items.Modifier.LOWDRAG)
            moveProjectile(_facingRight, 525);
        else
            moveProjectile(_facingRight, 350);
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting)
        {
            stream.SendNext(transform.position);
            stream.SendNext(gameObject.activeSelf);
        }
        else
        {
            stream.ReceiveNext();
            stream.ReceiveNext();
        }
    }

    public void ReactivarFlecha(object[] _parameters)
    {
        this.gameObject.SetActive(true);
        StartCoroutine(RPCForReactivating(_parameters));
    }

    public IEnumerator RPCForReactivating(object[] parameters)
    {

        yield return new WaitForEndOfFrame();
        PhotonNetwork.RPC(photonView, "ReActivarFlecha", PhotonTargets.AllBuffered, false, parameters);
    }

    [PunRPC]
    public void ReActivarFlecha(object[] _parameters)
    {

        bool _facingRight = (bool)_parameters[0];
        bool shouldIActivate = (bool)_parameters[1];
        Vector3 position = (Vector3)_parameters[2];

        this.gameObject.transform.position = position;
        facingright = _facingRight;
        this.gameObject.SetActive(shouldIActivate);
        if (PhotonConnection.GetInstance().GetPlayerById(owner).GetComponent<Player>().ranged.stats.mod1 == Items.Modifier.LOWDRAG ||
            PhotonConnection.GetInstance().GetPlayerById(owner).GetComponent<Player>().ranged.stats.mod2 == Items.Modifier.LOWDRAG)
            moveProjectile(_facingRight, 525);
        else
            moveProjectile(_facingRight, 350);
    }

}