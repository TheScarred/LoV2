using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;
using Items;
using SimpleHealthBar_SpaceshipExample;
using Custom.Indicators;

public class Player : PunBehaviour
{
    public CharacterController player_controller;
    public Weapon melee, ranged;
    public Sprite[] meleeSprites;
    public Sprite[] rangedSprites;
    public State myState;
    public PlayerStats _myPlayerStats;
    List<GameObject> range_attack_Objects = new List<GameObject>();
    public GameObject prefab_range_attack;
    public TerrainGenerator terreno;
    float delayMovement;
    public Vector3 posicionJugador;
    private bool facingRight = true;
    private bool weaponTrigger = false;
    bool vivo = true;
    public bool imAttacking;
    public int ID;
    int DamageReceived;
    public uint rangedAmmo;
    WaitForSeconds attackFrame;
    WaitForSeconds second;
    PlayerHealth health;

    // Melee attack hitbox & stat script
    public GameObject BasicHitBox;
    public Attack meleeAttack, rangedAttack;
    Collider pickup = null;
    float meleeCooldown;
    float rangedCooldown;
    int damageCooldown;

    public OffscreenIndicator indicators;
    //JOYSTICK
    public Joystick theJoystick;

    //Particles
    public ParticleManager particleManager;
    TypesAvailable.particleType particleDeath;
    public TypesAvailable.particleType particleHit;
    TypesAvailable.particleType particleHeal;
    TypesAvailable.particleType particleGrab;
    TypesAvailable.particleType particleSpawn;


    void Start()
    {
        myState = State.NORMAL;
        attackFrame = new WaitForSeconds(0.1f);
        second = new WaitForSeconds(1f);
        melee = ScriptableObject.CreateInstance<Weapon>();
        ranged = ScriptableObject.CreateInstance<Weapon>();
        rangedAmmo = 10;



        PhotonConnection.GetInstance().playerList.Add(this);
        if (photonView.isMine)
        {
            gameObject.AddComponent<AudioListener>();

            meleeAttack = BasicHitBox.GetComponent<Attack>();
            rangedAttack = prefab_range_attack.GetComponent<Attack>();

            health = GetComponent<PlayerHealth>();

            Camera.main.transform.parent = transform;

            Camera.main.transform.localPosition = new Vector3(0, 6, -12);
            indicators = GameObject.Find("Canvas").GetComponent<OffscreenIndicator>();
            PhotonNetwork.RPC(photonView, "CrearFlecha", PhotonTargets.AllBuffered, false);

            indicators.AddTarget(gameObject);

        }

        WeaponPickup[] weps = FindObjectsOfType<WeaponPickup>();
        for (int i = 0; i < weps.Length; i++)
        {
            PhotonConnection.GetInstance().weaponList.Add(weps[i]);
        }

        InitBaseWeapons(melee, ranged); // common
        //InitRandomWeapons(melee, ranged); // random

        facingRight = false;
        
        //_myPlayerStats = GetComponent<PlayerStats>();
        ID = this.gameObject.GetComponent<PhotonView>().viewID;
        //hit box is deactivated unless the player hits
        BasicHitBox.GetComponent<MeshRenderer>().enabled = false;
        BasicHitBox.GetComponent<Collider>().enabled = false;
        //BasicHitBox.GetComponent<HitBoxPlayer>().player = this;
        meleeCooldown = melee.stats.rOF;
        rangedCooldown = ranged.stats.rOF;

        imAttacking = false;


        //Particles
        particleDeath = TypesAvailable.particleType.PLAYER_DEATH;
        particleHit = TypesAvailable.particleType.HIT;
        particleHeal = TypesAvailable.particleType.HEAL;
        particleGrab = TypesAvailable.particleType.GRAB_WEAPON;
        particleSpawn = TypesAvailable.particleType.PLAYER_SPAWN;
        particleManager.ActivateParticle(this.transform, particleSpawn);

        
    }

    [PunRPC]
    public void CrearFlecha()
    {
        if(indicators == null)
        {
            indicators = GameObject.Find("Canvas").GetComponent<OffscreenIndicator>();
        }
           
    }

    GameObject SpawnRangeAttackObject(GameObject desired_prefab, Vector3 position)
    {
        delayMovement = 0.5f;
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

                        range_attack_Objects[i].GetComponent<Attack>().damage = ranged.stats.damage;
                        range_attack_Objects[i].GetComponent<Attack>().armourPen = ranged.stats.armourPen;

                        if (ranged.rarity == WeaponRarity.LEGENDARY)
                            range_attack_Objects[i].GetComponent<Attack>().effect = ranged.stats.mod1;
                        else
                            range_attack_Objects[i].GetComponent<Attack>().effect = Modifier.NONE;

                        if (Random.Range(1, 101) >= (100 - (100 * ranged.stats.critChance)))
                            range_attack_Objects[i].GetComponent<Attack>().isCrit = true;
                        else
                            range_attack_Objects[i].GetComponent<Attack>().isCrit = false;

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
   /* public IEnumerator MoveProyectile(GameObject proyectile)
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

        proyectile.GetComponent<Rigidbody>().velocity = Vector3.zero;
        proyectile.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
        proyectile.SetActive(false);

        yield return null;
    }
    */
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
        if (melee.stats.mod1 == Modifier.SWIFTNESS || melee.stats.mod2 == Modifier.SWIFTNESS)
            player_controller.Move(new Vector3(h, 0, v) * 1.15f * Time.deltaTime);
        //player_rigidbody.velocity = movement * (_myPlayerStats.m_Speed * 1.15f);
        else
            player_controller.Move(new Vector3(h, 0, v) * Time.deltaTime);
        //player_rigidbody.velocity = movement * _myPlayerStats.m_Speed;


    }

    void AttackInput()
    {
        //Primary Attack
        if (Input.GetKeyDown(KeyCode.Space))
        {
         
            if (Random.Range(1, 101) >= (100 - (100 * melee.stats.critChance)))
                meleeAttack.isCrit = true;

            else
                meleeAttack.isCrit = false;

            if (meleeCooldown <= Time.time)
            {
                imAttacking = true;
                PhotonNetwork.RPC(photonView, "ToggleHitBox", PhotonTargets.AllBuffered, false);
                meleeCooldown = Time.time + melee.stats.rOF;
            }

        }

        //Secondary attack
        if (Input.GetKeyDown(KeyCode.Q))
        {
            if (rangedCooldown <= Time.time && rangedAmmo > 0)
            {
                imAttacking = true;

                SpawnRangeAttackObject(prefab_range_attack, transform.position);

                if (ranged.stats.id >= 0)
                {
                    ranged.stats.wear--;
                    if (ranged.stats.wear <= 0)
                    {
                        BreakRangedWeapon();
                    }
                }
                rangedCooldown = Time.time + ranged.stats.rOF;

                if (ranged.stats.mod1 == Modifier.BOTTOMLESS || ranged.stats.mod2 == Modifier.BOTTOMLESS)
                {
                    if (ranged.stats.mod1 == Modifier.BOTTOMLESS)
                    {
                        if (ranged.stats.mod2 == Modifier.BOTTOMLESS)
                        {
                            if (Random.Range(0, 5) < 2)
                                rangedAmmo--;
                            else
                                Debug.Log("2 stack: Arrow recovered");
                        }
                        else if (Random.Range(0, 5) < 3)
                            rangedAmmo--;
                        else
                            Debug.Log("1 stack: Arrow recovered!");
                    }
                }
                else
                    rangedAmmo--;
            }
        }
    }

    void UpdateVariables()
    {
        posicionJugador = transform.position;

        if (meleeCooldown < melee.stats.rOF)
            meleeCooldown += Time.time;
        if (rangedCooldown < ranged.stats.rOF)
            rangedCooldown += Time.time;

        //Debug.Log(transform.position);
        
        

            


        


        if(delayMovement > 0)
        {
            delayMovement -= Time.deltaTime;
            if(delayMovement <= 0)
            {
                imAttacking = false;
            }
        }
    }

    void PickUpWeapon(Collider col)
    {
        if (Input.GetKeyDown(KeyCode.E) && weaponTrigger && (pickup != null))
        {
            WeaponPickup weapon = col.GetComponent<WeaponPickup>();
            ChangeWeapon(ref weapon.type, ref weapon.rarity, PhotonConnection.GetInstance().randomSeed, ref weapon.ID, weapon.lastWear);
            particleManager.ActivateParticle(this.transform, particleGrab);

            StartCoroutine(PhotonConnection.GetInstance().WaitFrame());
        }
    }

    void OnTriggerEnter(Collider col)
    {

        if (col.CompareTag("HitMelee") || (col.CompareTag("Proyectile") && (col.GetComponent<projectile>().owner == photonView.ownerId)))
        {
            _myPlayerStats.ReceiveDamage(col.GetComponent<Attack>().armourPen, col.GetComponent<Attack>().damage);

            if (transform.position.x > col.transform.position.x)
                if (col.GetComponent<Attack>().effect == Modifier.KNOCKBACK)
                    KnockBack(Vector3.right, 1f);
                else
                    KnockBack(Vector3.right, 0.5f);
            else
                if (col.GetComponent<Attack>().effect == Modifier.KNOCKBACK)
                    KnockBack(Vector3.left, 1f);
                else
                    KnockBack(Vector3.left, 0.5f);

            if ((col.GetComponent<Attack>().effect == Modifier.BLEEDING || col.GetComponent<Attack>().effect == Modifier.POISON) && myState == State.NORMAL)
            {
                myState = State.DAMAGE;
                StartCoroutine(TakeDamagePSecond(3));
            }
        }
        if (col.CompareTag("Food") && _myPlayerStats.m_HP < _myPlayerStats.base_HP)
        {
            HealPlayer((int)col.GetComponent<Food>().type);
            PhotonNetwork.RPC(photonView, "DissappearConsumable", PhotonTargets.All, false, col.GetComponent<Consumable>().id);
        }
        if (col.CompareTag("Armour") && _myPlayerStats.m_Shield < _myPlayerStats.base_Shield)
        {
            ArmourUp((int)col.GetComponent<Armour>().type);
            PhotonNetwork.RPC(photonView, "DissappearConsumable", PhotonTargets.All, false, col.GetComponent<Consumable>().id);
            particleManager.ActivateParticle(this.transform, particleGrab);
        }
        if (col.CompareTag("Ammo") && rangedAmmo < 30)
        {
            Resuply((int)col.GetComponent<Ammo>().type);
            PhotonNetwork.RPC(photonView, "DissappearConsumable", PhotonTargets.All, false, col.GetComponent<Consumable>().id);
            particleManager.ActivateParticle(this.transform, particleGrab);
        }
        if (col.CompareTag("Melee") || col.CompareTag("Rango"))
        {
            weaponTrigger = true;
            pickup = col;

        }
    }

    private void OnTriggerExit(Collider col)
    {
        if (col.CompareTag("Melee") || col.CompareTag("Rango"))
        {
            weaponTrigger = false;
            pickup = null;
        }
    }

    IEnumerator TakeDamagePSecond(int n)
    {
        int i = 0;
        while (i < n)
        {
            _myPlayerStats.m_HP -= 1;
            i++;
            yield return second;
        }
        myState = State.NORMAL;
    }

    void InitBaseWeapons(Weapon melee, Weapon ranged)
    {
        melee.type = Items.WeaponType.MELEE;
        melee.rarity = Items.WeaponRarity.COMMON;
        melee.sprite = meleeSprites[(int)melee.rarity];
        melee.stats = WeaponStats.SetStats(melee.stats, PhotonConnection.GetInstance().randomSeed, melee.type, melee.rarity, -1, -1);
        //meleeAttack.damage = melee.stats.damage;
        //meleeAttack.armourPen = melee.stats.armourPen;

        ranged.type = Items.WeaponType.RANGED;
        ranged.rarity = Items.WeaponRarity.COMMON;
        ranged.sprite = meleeSprites[(int)ranged.rarity];
        ranged.stats = WeaponStats.SetStats(ranged.stats, PhotonConnection.GetInstance().randomSeed, ranged.type, ranged.rarity, -2, -1);
        //rangedAttack.armourPen = ranged.stats.armourPen;
    }

    [PunRPC]
    void ResetMeleeWeapon()
    {
        melee.type = Items.WeaponType.MELEE;
        melee.rarity = Items.WeaponRarity.COMMON;
        melee.sprite = meleeSprites[(int)melee.rarity];
        melee.stats = WeaponStats.SetStats(melee.stats, PhotonConnection.GetInstance().randomSeed, melee.type, melee.rarity, -1, -1);
        meleeAttack.damage = melee.stats.damage;
        meleeAttack.armourPen = melee.stats.armourPen;

    }

    [PunRPC]
    void ResetRangedWeapon()
    {
        ranged.type = Items.WeaponType.RANGED;
        ranged.rarity = Items.WeaponRarity.COMMON;
        ranged.sprite = meleeSprites[(int)ranged.rarity];
        ranged.stats = WeaponStats.SetStats(ranged.stats, PhotonConnection.GetInstance().randomSeed, ranged.type, ranged.rarity, -2, -1);
    }

    public void BreakMeleeWeapon()
    {
        PhotonNetwork.RPC(photonView, "ResetMeleeWeapon", PhotonTargets.All, false);
    }

    public void BreakRangedWeapon()
    {
        PhotonNetwork.RPC(photonView, "ResetRangedWeapon", PhotonTargets.All, false);
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

    [PunRPC]
    public void DissappearConsumable(int id)
    {
        for (int i = 0; i < PhotonConnection.GetInstance().consumables.Count; i++)
        {
            if (PhotonConnection.GetInstance().consumables[i].id == id)
            {
                PhotonConnection.GetInstance().consumables[i].gameObject.SetActive(false);
            }
        }
    }

    public void ChangeWeapon(ref WeaponType type, ref WeaponRarity rarity, int seed, ref int weaponID, int wear)
    {
        object[] objects = new object[6];

        objects[0] = photonView.viewID;
        objects[1] = seed;
        objects[2] = type;
        objects[3] = rarity;
        objects[4] = weaponID;
        objects[5] = wear;

        if (type == WeaponType.MELEE)
            if (melee.stats.id >= 0)
            {
                int tempID = melee.stats.id;
                WeaponRarity tempR = melee.rarity;
                int tempWear = melee.stats.wear;

                PhotonNetwork.RPC(photonView, "SwapMeleeWeapon", PhotonTargets.All, false, objects);

                object[] objects2 = new object[5];
                objects2[0] = tempID; //ID DE ARMA DEL JUGADOR
                objects2[1] = tempR;
                objects2[2] = weaponID; //ID DE ARMA DEL PISO
                objects2[3] = type;
                objects2[4] = tempWear;

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
                int tempWear = ranged.stats.wear;
                PhotonNetwork.RPC(photonView, "SwapRangedWeapon", PhotonTargets.All, false, objects);

                object[] objects2 = new object[5];
                objects2[0] = tempID; //ID DE ARMA DEL JUGADOR
                objects2[1] = tempR;
                objects2[2] = weaponID; //ID DE ARMA DEL PISO
                objects2[3] = type;
                objects2[4] = tempWear;

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
            melee.stats = WeaponStats.SetStats(melee.stats, (int)objects[1], (WeaponType)objects[2], (WeaponRarity)objects[3], (int)objects[4], (int)objects[5]);
            meleeAttack.damage = melee.stats.damage;

            if (melee.rarity == WeaponRarity.LEGENDARY)
                meleeAttack.effect = melee.stats.mod1;

            else
                meleeAttack.effect = Modifier.NONE;
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
            ranged.stats = WeaponStats.SetStats(ranged.stats, (int)objects[1], (WeaponType)objects[2], (WeaponRarity)objects[3], (int)objects[4], (int)objects[5]);

            if (ranged.rarity == WeaponRarity.LEGENDARY)
                rangedAttack.effect = ranged.stats.mod1;

            else
                rangedAttack.effect = Modifier.NONE;
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
            melee.stats = WeaponStats.SetStats(melee.stats, (int)objects[1], (WeaponType)objects[2], (WeaponRarity)objects[3], (int)objects[4], (int)objects[5]);
            meleeAttack.damage = melee.stats.damage;

            if (melee.rarity == WeaponRarity.LEGENDARY)
                meleeAttack.effect = melee.stats.mod1;

            else
                meleeAttack.effect = Modifier.NONE;
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
            ranged.stats = WeaponStats.SetStats(ranged.stats, (int)objects[1], (WeaponType)objects[2], (WeaponRarity)objects[3], (int)objects[4], (int)objects[5]);
        }

    }

    [PunRPC]
    public void UpdatePickupWeapon(object[] objects)
    {
        for (int i = 0; i < PhotonConnection.GetInstance().weaponList.Count; i++)
        {
            if (PhotonConnection.GetInstance().weaponList[i].ID == (int)objects[2])
            {
                PhotonConnection.GetInstance().weaponList[i].ID = (int)objects[0];
                PhotonConnection.GetInstance().weaponList[i].rarity = (WeaponRarity)objects[1];
                PhotonConnection.GetInstance().weaponList[i].lastWear = (int)objects[4];

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
                if (imAttacking == false)
                {
                    Movement();
                }
                UpdateVariables();
                AttackInput();
                PickUpWeapon(pickup);
            }
            else if (vivo == true)
            {
                PhotonNetwork.RPC(photonView, "KillPlayer", PhotonTargets.AllBuffered, false);
                particleManager.ActivateParticle(this.transform, particleDeath);
            }

            else
                if (Input.GetKey(KeyCode.Space))
            {
                PhotonNetwork.RPC(photonView, "RevivePlayer", PhotonTargets.AllBuffered, false);
                particleManager.ActivateParticle(this.transform, particleSpawn);

            }


        }

            _myPlayerStats.UpdateScoreboard();


          
        

            // Update scoreboard

        if (PhotonNetwork.player.NickName == "")
            PhotonNetwork.player.NickName = "Jugador #" + Random.Range(1.00f, 9.00f);
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
        yield return attackFrame;
        BasicHitBox.GetComponent<Collider>().enabled = false;   //will go back to waiting if another object is hit after detecting one with space. Will need counter for animation
        BasicHitBox.GetComponent<MeshRenderer>().enabled = false;
        imAttacking = false;
    }

    public void HealPlayer(int amount)
    {
        if (_myPlayerStats.m_HP + amount > _myPlayerStats.base_HP)
            _myPlayerStats.m_HP = _myPlayerStats.base_HP;
        else
            _myPlayerStats.m_HP += amount;

        health.healthBar.UpdateBar(_myPlayerStats.m_HP, _myPlayerStats.base_HP);
        particleManager.ActivateParticle(this.transform, particleHeal);
    }

    public void ArmourUp(int amount)
    {
        if (_myPlayerStats.m_Shield + amount > _myPlayerStats.base_Shield)
            _myPlayerStats.m_Shield = _myPlayerStats.base_Shield;
        else
            _myPlayerStats.m_Shield += amount;

        health.shieldBar.UpdateBar(_myPlayerStats.m_Shield, _myPlayerStats.base_Shield);
    }

    public void Resuply(int amount)
    {
        if (rangedAmmo + amount > 30)
            rangedAmmo = 30;
        else
            rangedAmmo += (uint)amount;
    }

    public void KnockBack(Vector3 dir, float power)
    {
        transform.Translate(dir * power);
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
        //particleManager.ActivateParticle(this.transform, particleSpawn);
        vivo = true;
    }

    [PunRPC]
    public void KillPlayer()
    {
        vivo = false;
        //particleManager.ActivateParticle(this.transform, particleDeath);
        //animator.SetBool("Morir", true);
        _myPlayerStats.UpdateScoreboard();
        gameObject.GetComponent<Rigidbody>().useGravity = false;
        gameObject.GetComponent<BoxCollider>().enabled = false;
        gameObject.GetComponent<SpriteRenderer>().enabled = false;

    }
}
