using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;
using Items;

public enum PlayerState
{
    NORMAL,
    BLEEDING,
    POISONED
};
public class Player : PunBehaviour
{

    public Weapon melee, ranged;
    public Sprite[] meleeSprites;
    public Sprite[] rangedSprites;
    public PlayerState myState;
    PlayerStats _myPlayerStats;
    List<GameObject> range_attack_Objects = new List<GameObject>();
    public GameObject prefab_range_attack;
    public Rigidbody player_rigidbody;
    public TerrainGenerator terreno;
    WaitForSeconds proyectile_Timer = new WaitForSeconds(2.0f);
    WaitForSeconds melee_hitbox_Timer = new WaitForSeconds(0.5f);
    public Vector3 posicionJugador;
    private bool facingRight = true;
    bool vivo = true;
    public int ID;
    int DamageReceived;

    //melee attack HIT BOX
    public GameObject BasicHitBox;
    float hit_cooldown;

    //JOYSTICK
    public Joystick theJoystick;


    void Start()
    {
        melee = ScriptableObject.CreateInstance<Weapon>();
        ranged = ScriptableObject.CreateInstance<Weapon>();

        PhotonConnection.GetInstance().playerList.Add(this);
        if (photonView.isMine)
        {
            Camera.main.transform.parent = transform;
            Camera.main.transform.localPosition = new Vector3(0, 4, -7);
        }

        WeaponPickup[] weps = FindObjectsOfType<WeaponPickup>();
        for (int i = 0; i < weps.Length; i++)
        {
            PhotonConnection.GetInstance().weaponList.Add(weps[i]);
        }

        InitBaseWeapons(melee, ranged); // common
        //InitRandomWeapons(melee, ranged); // random

        facingRight = false;
        player_rigidbody = GetComponent<Rigidbody>();
        _myPlayerStats = GetComponent<PlayerStats>();
        ID = this.gameObject.GetComponent<PhotonView>().viewID;
        //hit box is deactivated unless the player hits
        BasicHitBox.GetComponent<MeshRenderer>().enabled = false;
        BasicHitBox.GetComponent<Collider>().enabled = false;
        hit_cooldown = 1.5f;
    }

    GameObject SpawnRangeAttackObject(GameObject desired_prefab, Vector3 position)
    {
        for (int i = 0; i < range_attack_Objects.Count; i++)
        {
            if (range_attack_Objects[i].activeSelf == false)
            {
                //Si vamos a meter diferente tipos de proyectiles (hachas, flechas, etc), aquí hay que colocar el nombre de los prefabas
                //Hay que copiar todo dentro del IF de "prefab.name == **** " y solo cambiarle el nombre
                if (desired_prefab.name == "Arrow")
                {
                    if (range_attack_Objects[i].gameObject.name == "Arrow(Clone)")
                    {

                        object[] parameters2 = new object[3];

                        parameters2[2] = position;
                        parameters2[1] = true;
                        parameters2[0] = facingRight;
                        // range_attack_Objects[i].GetComponent<Transform>().position = position;
                        //range_attack_Objects[i].SetActive(true);
                        // range_attack_Objects[i].GetComponent<projectile>().moveProjectile(facingRight);

                        range_attack_Objects[i].GetComponent<projectile>().ReactivarFlecha(parameters2);
                        return range_attack_Objects[i];

                    }

                }

            }
        }

        GameObject go = PhotonNetwork.Instantiate(prefab_range_attack.name.ToString(), position, Quaternion.identity, 0);
        go.GetComponent<projectile>().owner = photonView.viewID;

        //ESPERAR UN MOMENTO PARA PODER HACER ESTO O HACER UN BULLET MANAGER APARTE QUE SE ENCARGUÉ ESPECIFICAMENTE DE ESTO
        object[] parameters = new object[3];
        parameters[2] = ID;
        parameters[1] = facingRight;
        parameters[0] = new Vector3(-90, 90, 0);
        //PhotonNetwork.RPC(go.GetComponent<PhotonView>(), "ArrowStart", PhotonTargets.AllBuffered, false, parameters);
        go.GetComponent<projectile>().PrepareRPC(parameters);
        // go.transform.eulerAngles = new Vector3(-90, 90, 0);
        // go.GetComponent<projectile>().owner = ID;
        // go.GetComponent<projectile>().moveProjectile(facingRight);
        range_attack_Objects.Add(go);
        //StartCoroutine(MoveProyectile(go));
        return go;
    }
    public IEnumerator MoveProyectile(GameObject proyectile)
    {
        //Mover el disparo y luego desactivarlo para volverse a usar en el futuro
        if (!facingRight)
        {
            proyectile.GetComponent<Rigidbody>().AddForce(Vector3.right * 350f);
        }
        else
        {
            proyectile.GetComponent<Rigidbody>().AddForce(Vector3.left * 350f);
        }
        yield return proyectile_Timer;
        proyectile.GetComponent<Rigidbody>().velocity = Vector3.zero;
        proyectile.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
        proyectile.SetActive(false);

    }

    void Movement()
    {
        //Checar que lado esta mirando para cambiar su la escala (voltear)
        if (Input.GetAxis("Horizontal") > 0 && facingRight || Input.GetAxis("Horizontal") < 0 && !facingRight /*|| theJoystick.horizontal > 0 && facingRight || theJoystick.horizontal < 0 && !facingRight*/)
        {
            facingRight = !facingRight;
            Vector3 scale = transform.localScale;
            scale.x *= -1;
            transform.localScale = scale;

        }

        float h = _myPlayerStats.m_Speed * Input.GetAxis("Horizontal");
        float v = _myPlayerStats.m_Speed * Input.GetAxis("Vertical");

        //MOVIMIENTO DE JOYSTICK
        //float h = _myPlayerStats.m_Speed * theJoystick.horizontal;
        //float v = _myPlayerStats.m_Speed * theJoystick.vectical;

        Vector3 movement = new Vector3(h, 0.0f, v);
        player_rigidbody.velocity = movement * _myPlayerStats.m_Speed;


    }

  

    void AttackInput()
    {
        //Primary Attack
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (hit_cooldown <= 0)
            {
                hit_cooldown = 1.5f;
                PhotonNetwork.RPC(photonView, "ToggleHitBox", PhotonTargets.AllBuffered, false);
            }
        }

        //Secondary attack
        if (Input.GetKeyDown(KeyCode.Q))
        {
            if (_myPlayerStats.m_ShootingSpeed >= ranged.stats.rOF)
            {
                _myPlayerStats.m_ShootingSpeed = 0f;
                SpawnRangeAttackObject(prefab_range_attack, transform.position);
            }

        }
    }

    public void MeleeAttack()
    {
        StartCoroutine(ToggleHitBox());
    }

    public void RangedAttack()
    {
        if (_myPlayerStats.m_ShootingSpeed >= ranged.stats.rOF)
        {
            _myPlayerStats.m_ShootingSpeed = 0f;
            SpawnRangeAttackObject(prefab_range_attack, transform.position);
        }
    }

    void UpdateVariables()
    {
        //Debug.Log(timeBeforeAnotherShoot);
        //Todas las variables de las estádisticas se deben actualizar aquí
        posicionJugador = transform.position;
        //Debug.Log(transform.position);


        //Timer para disparar proyectiles
        if (_myPlayerStats.m_ShootingSpeed <= 1f)
        {
            _myPlayerStats.m_ShootingSpeed += Time.deltaTime;
        }
        if(hit_cooldown >= 0)
        {
            hit_cooldown -= Time.deltaTime;
        }

    }

    void OnTriggerEnter(Collider col)
    {
        if (col.CompareTag("HitMelee"))
        {
            int damage = (int)(col.gameObject.transform.parent.gameObject.GetComponent<PlayerStats>().m_DamageMelee);
            _myPlayerStats.ReceiveDamage(damage);
        }
        if (col.CompareTag("Food"))
        {
            Food food = col.GetComponent<Food>();
            if (food.type == FoodType.SMALL)
            {
                Debug.Log("Se comio una banana jajajaja xd");
                _myPlayerStats.base_HP += (int)FoodType.SMALL;
            }
        }
    }

    void OnTriggerStay(Collider objeto)
    {
        if (objeto.CompareTag("Melee") || objeto.CompareTag("Rango"))
            if (Input.GetKeyDown(KeyCode.E))
            {
                WeaponPickup weapon = objeto.GetComponent<WeaponPickup>();
                ChangeWeapon(ref weapon.type, ref weapon.rarity, PhotonConnection.GetInstance().randomSeed, ref weapon.ID);

                StartCoroutine(PhotonConnection.GetInstance().WaitFrame());
            }
    }

  
    void InitBaseWeapons(Weapon melee, Weapon ranged)
    {
        melee.type = Items.WeaponType.MELEE;
        melee.rarity = Items.WeaponRarity.COMMON;
        melee.sprite = meleeSprites[(int)melee.rarity];
        melee.stats = WeaponStats.SetStats(melee.stats, PhotonConnection.GetInstance().randomSeed, melee.type, melee.rarity, -1);

        ranged.type = Items.WeaponType.RANGED;
        ranged.rarity = Items.WeaponRarity.COMMON;
        ranged.sprite = meleeSprites[(int)ranged.rarity];
        ranged.stats = WeaponStats.SetStats(ranged.stats, PhotonConnection.GetInstance().randomSeed, ranged.type, ranged.rarity, -2);
    }

    [PunRPC]
    public void DissappearWeapon(int id)
    {
        for (int i = 0; i < PhotonConnection.GetInstance().weaponList.Count; i++)
        {
            if (PhotonConnection.GetInstance().weaponList[i].ID == id)
            {
                PhotonConnection.GetInstance().weaponList[i].gameObject.SetActive(false);
            }
        }
    }


    public void ChangeWeapon(ref WeaponType type, ref WeaponRarity rarity, int seed, ref int weaponID)
    {
        object[] objects = new object[5];

        objects[0] = photonView.viewID;
        objects[1] = seed;
        objects[2] = type;
        objects[3] = rarity;
        objects[4] = weaponID;


        if (type == WeaponType.MELEE)
            if (melee.stats.id >= 0)
            {
                int tempID = melee.stats.id;
                WeaponRarity tempR = melee.rarity;

                PhotonNetwork.RPC(photonView, "SwapMeleeWeapon", PhotonTargets.All, false, objects);

                object[] objects2 = new object[4];
                objects2[0] = tempID; //ID DE ARMA DEL JUGADOR
                objects2[1] = tempR;
                objects2[2] = weaponID; //ID DE ARMA DEL PISO
                objects2[3] = type;

                PhotonNetwork.RPC(photonView, "UpdatePickupWeapon", PhotonTargets.All, false, objects2);
            }
            else
            {
                PhotonNetwork.RPC(photonView, "DissappearWeapon", PhotonTargets.All, false, objects[4]);
                PhotonNetwork.RPC(photonView, "GetMeleeWeapon", PhotonTargets.All, false, objects);
            }

        else
            if (ranged.stats.id >= 0)
            {
                int tempID = ranged.stats.id;
                WeaponRarity tempR = ranged.rarity;
                PhotonNetwork.RPC(photonView, "SwapRangedWeapon", PhotonTargets.All, false, objects);

                object[] objects2 = new object[4];
                objects2[0] = tempID; //ID DE ARMA DEL JUGADOR
                objects2[1] = tempR;
                objects2[2] = weaponID; //ID DE ARMA DEL PISO
                objects2[3] = type;

                PhotonNetwork.RPC(photonView, "UpdatePickupWeapon", PhotonTargets.All, false, objects2);
            }

        else
        {
            PhotonNetwork.RPC(photonView, "DissappearWeapon", PhotonTargets.All, false, objects[4]);
            PhotonNetwork.RPC(photonView, "GetRangedWeapon", PhotonTargets.All, false, objects);
        }

    }


    [PunRPC]
    public void GetMeleeWeapon(object[] objects)
    {
        int playerID = (int)objects[0];
        if (playerID == photonView.viewID)
        {
            melee = ScriptableObject.CreateInstance<Weapon>();
            melee.type = (WeaponType)objects[2];
            melee.rarity = (WeaponRarity)objects[3];
            melee.stats = WeaponStats.SetStats(melee.stats, (int)objects[1], (WeaponType)objects[2], (WeaponRarity)objects[3], (int)objects[4]);
        }
    }

    [PunRPC]
    public void GetRangedWeapon(object[] objects)
    {
        int playerID = (int)objects[0];
        if (playerID == photonView.viewID)
        {
            ranged = ScriptableObject.CreateInstance<Weapon>();
            ranged.type = (WeaponType)objects[2];
            ranged.rarity = (WeaponRarity)objects[3];
            ranged.stats = WeaponStats.SetStats(ranged.stats, (int)objects[1], (WeaponType)objects[2], (WeaponRarity)objects[3], (int)objects[4]);
        }
    }

    [PunRPC]
    public void SwapMeleeWeapon(object[] objects)
    {
        int playerID = (int)objects[0];
        if (playerID == photonView.viewID)
        {
            melee = ScriptableObject.CreateInstance<Weapon>();
            melee.type = (WeaponType)objects[2];
            melee.rarity = (WeaponRarity)objects[3];
            melee.stats = WeaponStats.SetStats(melee.stats, (int)objects[1], (WeaponType)objects[2], (WeaponRarity)objects[3], (int)objects[4]);
        }

    }

    [PunRPC]
    public void SwapRangedWeapon(object[] objects)
    {
        int playerID = (int)objects[0];
        if (playerID == photonView.viewID)
        {
            ranged = ScriptableObject.CreateInstance<Weapon>();
            ranged.type = (WeaponType)objects[2];
            ranged.rarity = (WeaponRarity)objects[3];
            ranged.stats = WeaponStats.SetStats(ranged.stats, (int)objects[1], (WeaponType)objects[2], (WeaponRarity)objects[3], (int)objects[4]);
        }

    }

    [PunRPC]
    public void UpdatePickupWeapon(object[] objects)
    {
        for (int i = 0; i < PhotonConnection.GetInstance().weaponList.Count; i++)
        {
            Debug.Log(PhotonConnection.GetInstance().weaponList[i].ID);
            if (PhotonConnection.GetInstance().weaponList[i].ID == (int)objects[2])
            {
                PhotonConnection.GetInstance().weaponList[i].ID = (int)objects[0];
                PhotonConnection.GetInstance().weaponList[i].rarity = (WeaponRarity)objects[1];
                if ((WeaponType)objects[3] == WeaponType.MELEE)
                    PhotonConnection.GetInstance().weaponList[i].GetComponent<SpriteRenderer>().sprite = meleeSprites[(int)objects[1]];
                else
                    PhotonConnection.GetInstance().weaponList[i].GetComponent<SpriteRenderer>().sprite = rangedSprites[(int)objects[1]];
            }
        }
    }

    #region IPunObservable

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting)
        {

            if (_myPlayerStats == null)
            {
                Debug.Log("stats vacio writing");
            }
            else
            {
                //stream.SendNext(transform.position);
                SendStream(stream, _myPlayerStats.m_Speed);
                SendStream(stream, _myPlayerStats.m_Shield);
                SendStream(stream, _myPlayerStats.m_HP);
                SendStream(stream, _myPlayerStats.m_ShootingSpeed);
                SendStream(stream, _myPlayerStats.m_MeeleSpeed);
                SendStream(stream, _myPlayerStats.m_DamageRange);
                SendStream(stream, _myPlayerStats.m_DamageMelee);
              
            }

        }
        else
        {
            if (_myPlayerStats == null)
            {
                Debug.Log("stats vacio");
            }
            else
            {
                //transform.position = (Vector3)stream.ReceiveNext();
                _myPlayerStats.m_Speed = (float)stream.ReceiveNext();
                _myPlayerStats.m_Shield = (float)stream.ReceiveNext();
                _myPlayerStats.m_HP = (float)stream.ReceiveNext();
                _myPlayerStats.m_ShootingSpeed = (float)stream.ReceiveNext();
                _myPlayerStats.m_MeeleSpeed = (float)stream.ReceiveNext();
                _myPlayerStats.m_DamageRange = (float)stream.ReceiveNext();
                _myPlayerStats.m_DamageMelee = (float)stream.ReceiveNext();
         
            }

        }
    }
    #endregion

    void Update()
    {
        if (photonView.isMine)
        {
            if (_myPlayerStats.m_HP > 0)
            {
                Movement();
                UpdateVariables();
                AttackInput();
            }
            else if (vivo == true)
            {
                PhotonNetwork.RPC(photonView, "KillPlayer", PhotonTargets.AllBuffered, false);
            }
            else
            {
                if (Input.GetKey(KeyCode.Space))
                {
                    PhotonNetwork.RPC(photonView, "RevivePlayer", PhotonTargets.AllBuffered, false);
                }
            }

        }
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            // Show scoreboard
            _myPlayerStats.scoreboard.SetActive(true);
            // Update scoreboard
            _myPlayerStats.UpdateScoreboard();
        }

        if (Input.GetKeyUp(KeyCode.Tab))
        {
            // esconder el scoreboard
            _myPlayerStats.scoreboard.SetActive(false);
        }
        if (PhotonNetwork.player.NickName == "")
        {
            PhotonNetwork.player.NickName = "Jugador #" + Random.Range(1.00f, 9.00f);
        }

    }

    public void SendStream(PhotonStream stream, float value)
    {
        stream.SendNext(value);
    }

    [PunRPC]
    IEnumerator ToggleHitBox()
    {
        BasicHitBox.GetComponent<Collider>().enabled = true;
        BasicHitBox.GetComponent<MeshRenderer>().enabled = true;
        yield return melee_hitbox_Timer;
        BasicHitBox.GetComponent<Collider>().enabled = false;   //will go back to waiting if another object is hit after detecting one with space. Will need counter for animation
        BasicHitBox.GetComponent<MeshRenderer>().enabled = false;
    }




    [PunRPC]
    public void RevivePlayer()
    {
        gameObject.GetComponent<BoxCollider>().enabled = true;
        gameObject.GetComponent<SpriteRenderer>().enabled = true;
        gameObject.GetComponent<Rigidbody>().useGravity = true;
        gameObject.GetComponent<PlayerStats>().HP_bar.fillAmount = 1;
        _myPlayerStats.ResetStats();

        int randSpawn = Random.Range(0, terreno.PlayerSpawners.Count);
      
        transform.position = terreno.PlayerSpawners[randSpawn].transform.position;
        vivo = true;
    }

    [PunRPC]
    public void KillPlayer()
    {
        Debug.Log("MATA");
        vivo = false;
        //animator.SetBool("Morir", true);
        gameObject.GetComponent<Rigidbody>().useGravity = false;
        gameObject.GetComponent<BoxCollider>().enabled = false;
        gameObject.GetComponent<SpriteRenderer>().enabled = false;
       
    }
}
