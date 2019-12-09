using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;
public class projectile : PunBehaviour
{
    public int owner;
    Vector3 angle;
    bool facingright;
    float timer;
    public float torque = 10f;
    Rigidbody rigi;

    [SerializeField]
    Transform target;

    private void Start()
    {
        rigi = GetComponent<Rigidbody>();
    }

    void OnTriggerEnter(Collider other)
    {
        
        if (other.gameObject.CompareTag("Player"))
        {
            if (other.GetComponent<Player>().ID != owner)
            {
                StartCoroutine(DeathTime());
                rigi.velocity = Vector3.zero;
                rigi.angularVelocity = Vector3.zero;
                target = null;
                this.gameObject.SetActive(false);
            }

        }

        if (other.gameObject.CompareTag("ENEMIES"))
        {
            StartCoroutine(DeathTime());
            rigi.velocity = Vector3.zero;
            rigi.angularVelocity = Vector3.zero;
            target = null;
            this.gameObject.SetActive(false);
        }
    }

    public void OnEnable()
    {
        if (GetComponent<Attack>())
        {
            Attack attack = GetComponent<Attack>();
            if (attack.effect1 == Items.Modifier.BULLETPOINT || attack.effect2 == Items.Modifier.BULLETPOINT)
                timer = 2.25f;
            else
                timer = 1.5f;

            if (attack.effect1 == Items.Modifier.HOMING)
                target = GetHomingTarget(transform.position, !facingright);
        }
    }

    IEnumerator DeathTime()
    {
        yield return new WaitForEndOfFrame();
        gameObject.SetActive(false);
    }


    public void moveProjectile(bool direction, float force)
    {
        if (direction)
            rigi.AddForce(Vector3.right * force);
        else
            rigi.AddForce(Vector3.left * force);

    }
    public void Update()
    {
        if (target != null)
        {
            float step = torque * Time.deltaTime;
            Vector3 relativePos = target.position - transform.position;
            relativePos.Normalize();
            var targetRotation = Quaternion.LookRotation(relativePos);
            targetRotation = Quaternion.Euler(targetRotation.eulerAngles.x, targetRotation.eulerAngles.y - 90f, targetRotation.eulerAngles.z);
            var rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, step);
            rigi.MoveRotation(rotation);

            if (facingright)
                rigi.velocity = transform.forward * -7;
            else
                rigi.velocity = transform.forward * 7;
        }

        timer -= Time.deltaTime;
        if (timer <= 0)
        {
            rigi.velocity = Vector3.zero;
            rigi.angularVelocity = Vector3.zero;
            target = null;
            gameObject.SetActive(false);
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
        if (GetComponent<Attack>())
                if (GetComponent<Attack>().effect1 == Items.Modifier.LOWDRAG || GetComponent<Attack>().effect2 == Items.Modifier.LOWDRAG)
                    moveProjectile(_facingRight, 525);
                else
                    moveProjectile(_facingRight, 350);

        if (GetComponent<Attack>().effect1 == Items.Modifier.HOMING)
            target = GetHomingTarget(transform.position, facingright);
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

    public Transform GetHomingTarget(Vector3 position, bool facingRight)
    {
        Debug.Log("Getting Target");
        int m = PhotonConnection.GetInstance().playerList.Count;
        int n = m + PhotonConnection.GetInstance().enemiesList.Count;
        float d = Mathf.Infinity;
        Transform temp = null;
        Player currentP;
        GameObject currentE;

        if (facingRight)
        {
            Debug.Log("derecha");
            for (int i = 0; i < n ; i++)
            {
                if (i < m)
                {
                    Debug.Log("Checando Jugador " + i);
                    currentP = PhotonConnection.GetInstance().playerList[i];
                    if (transform.position.x < currentP.transform.position.x && Vector3.Distance(transform.position, currentP.transform.position) < d &&
                        currentP.photonView.viewID != owner)
                    {
                        temp = currentP.transform;
                        d = Vector3.Distance(transform.position, currentP.transform.position);
                    }
                }
                else
                {
                    int ii = i - m;
                    Debug.Log("Checando Enemigo " + ii);
                    currentE = PhotonConnection.GetInstance().enemiesList[ii];
                    if (transform.position.x < currentE.transform.position.x && Vector3.Distance(transform.position, currentE.transform.position) < d)
                    {
                        temp = currentE.transform;
                        d = Vector3.Distance(transform.position, currentE.transform.position);
                        Debug.Log(ii + " " + temp.position, temp.gameObject);
                    }
                }
            }
            return temp;
        }
        else
        {
            for (int i = 0; i < n ; i++)
            {
                Debug.Log("izquierda");
                if (i < m)
                {
                    Debug.Log("Checando Jugador " + i);
                    currentP = PhotonConnection.GetInstance().playerList[i];
                    if (transform.position.x > currentP.transform.position.x && Vector3.Distance(transform.position, currentP.transform.position) < d &&
                    currentP.photonView.viewID != owner)
                    {
                        temp = currentP.transform;
                        d = Vector3.Distance(transform.position, currentP.transform.position);
                    }
                }
                else
                {
                    int ii = i - m;
                    Debug.Log("Checando Enemigo " + ii);
                    currentE = PhotonConnection.GetInstance().enemiesList[ii];
                    if (transform.position.x > currentE.transform.position.x && Vector3.Distance(transform.position, currentE.transform.position) < d)
                    {
                        temp = currentE.transform;
                        d = Vector3.Distance(transform.position, currentE.transform.position);
                        Debug.Log(ii + " " + temp.position, temp.gameObject);
                    }
                }
            }
            return temp;
        }
    }

    public void ReactivarFlecha(object[] _parameters)
    {
        gameObject.SetActive(true);
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

        gameObject.transform.position = position;
        facingright = _facingRight;
        gameObject.SetActive(shouldIActivate);
        if (GetComponent<Attack>())
            if (GetComponent<Attack>().effect1 == Items.Modifier.LOWDRAG || GetComponent<Attack>().effect2 == Items.Modifier.LOWDRAG)
                moveProjectile(_facingRight, 525);
            else
                moveProjectile(_facingRight, 350);
    }
}