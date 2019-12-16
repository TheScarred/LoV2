using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon;
using Items;
using SimpleHealthBar_SpaceshipExample;
using Custom.Indicators;

public class Player : PunBehaviour
{
    //Audio
    AudioSource audio;
    [SerializeField]
    AudioClip fightingmelee, fightingranged, fightingmeleecrit;

 
    public CharacterController player_controller;
    public Weapon melee, ranged;
    public Image meleeButton, rangedButton;
    public Sprite attackOrGet;
    public Sprite[] meleeSprites;
    public Sprite[] rangedSprites;
    public State myState;
    public PlayerStats _myPlayerStats;
    List<GameObject> range_attack_Objects = new List<GameObject>();
    public GameObject prefab_range_attack;
    public TerrainGenerator terreno;
    float delayMovement;
    float gravity;
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
    public SpriteRenderer mySprite;
    // Melee attack hitbox & stat script
    public GameObject BasicHitBox;
    public Attack meleeAttack, rangedAttack;
    Collider pickup = null;
    float meleeCooldown;
    float rangedCooldown;
    int damageCooldown;

    public OffscreenIndicator indicators;
    //JOYSTICK
     FloatingJoystick theJoystick;

    //BUTTONS
    enum Botones { RANGED, MELEE };
    public Button[] theButtons;
    Text ammoLeft;

    //Particles
    TypesAvailable.particleType particleDeath;
    public TypesAvailable.particleType particleHit;
    TypesAvailable.particleType particleHeal;
    TypesAvailable.particleType particleGrab;
    TypesAvailable.particleType particleSpawn;

    void Awake()
    {
        audio = gameObject.GetComponent<AudioSource>();
    }




    void Start()
    {
        mySprite = GetComponent<SpriteRenderer>();
        myState = State.NORMAL;
        attackFrame = new WaitForSeconds(0.1f);
        second = new WaitForSeconds(1f);
        melee = ScriptableObject.CreateInstance<Weapon>();
        ranged = ScriptableObject.CreateInstance<Weapon>();
        rangedAmmo = 10;
        gravity = 15f;

        //SE ASIGNAN EL JOYSTICK Y LOS BOTONES
        theJoystick = FindObjectOfType<FloatingJoystick>();
        theButtons = FindObjectsOfType<Button>();
        ammoLeft = FindObjectOfType<Text>();


        PhotonConnection.GetInstance().playerList.Add(this);
        if (photonView.isMine)
        {
            //gameObject.AddComponent<AudioListener>();
            meleeAttack = BasicHitBox.GetComponent<Attack>();
            rangedAttack = prefab_range_attack.GetComponent<Attack>();
            health = GetComponent<PlayerHealth>();

            Camera.main.transform.parent = transform;

            Camera.main.transform.localPosition = new Vector3(0, 8, -16);
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

        _myPlayerStats = GetComponent<PlayerStats>();
        ID = this.gameObject.GetComponent<PhotonView>().viewID;
        //hit box is deactivated unless the player hits
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


        ParticleManager.GetInstance().ActivateParticle(this.transform, particleSpawn);

        mySprite = gameObject.GetComponent<SpriteRenderer>();
        meleeButton = theButtons[1].transform.GetChild(0).GetComponent<Image>();
        rangedButton = theButtons[0].transform.GetChild(0).GetComponent<Image>();

        if(health==null)
        {
            health = GetComponent<PlayerHealth>();
        }
        _myPlayerStats.UpdateScoreboard();
        health.ActivarAudioListener();
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

                        Attack arrowAttack = range_attack_Objects[i].GetComponent<Attack>();

                        arrowAttack.damage = ranged.stats.damage;
                        arrowAttack.armourPen = ranged.stats.armourPen;

                        if (ranged.stats.mod1 != Modifier.SPEEDLOAD && ranged.stats.mod1 != Modifier.BOTTOMLESS)
                            arrowAttack.effect1 = ranged.stats.mod1;
                        else
                            arrowAttack.effect1 = Modifier.NONE;

                        if (ranged.stats.mod2 != Modifier.SPEEDLOAD && ranged.stats.mod2 != Modifier.BOTTOMLESS)
                            arrowAttack.effect2 = ranged.stats.mod2;
                        else
                            arrowAttack.effect2 = Modifier.NONE;


                        if (Random.Range(1, 101) >= (100 - (100 * ranged.stats.critChance)))
                            arrowAttack.isCrit = true;
                        else
                            arrowAttack.isCrit = false;

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
        go.GetComponent<projectile>().PrepareRPC(parameters);
        range_attack_Objects.Add(go);
        return go;
        
    }
    void Movement()
    {
        //Checar que lado esta mirando para cambiar su la escala (voltear)
        if (Input.GetAxis("Horizontal") > 0 && !facingRight || Input.GetAxis("Horizontal") < 0 && facingRight || theJoystick.Horizontal > 0 && !facingRight || theJoystick.Horizontal < 0 && facingRight)
        {
            facingRight = !facingRight;
            Vector3 scale  = BasicHitBox.transform.localPosition;
            scale.x *= -1;
            BasicHitBox.transform.localPosition = scale;
            mySprite.flipX = !facingRight;
        }

        float h = 0;
        float v = 0;

        //CHECA SI HAY MOVIMIENTO EN EL JOYSTICK PARA VER SI UTILIZA ESTE MISMO O LAS TECLAS
        if (theJoystick.Horizontal == 0 && theJoystick.Vertical == 0)
        {
            h = _myPlayerStats.m_Speed * Input.GetAxis("Horizontal");
            v = _myPlayerStats.m_Speed * Input.GetAxis("Vertical");
        }
        else
        {
            //MOVIMIENTO DE JOYSTICK
            h = _myPlayerStats.m_Speed * theJoystick.Horizontal;
            v = _myPlayerStats.m_Speed * theJoystick.Vertical;
        }


        Vector3 movement = new Vector3(h, 0.0f, v);
        movement.y = -gravity * Time.deltaTime;
        if (melee.stats.mod1 == Modifier.SWIFTNESS || melee.stats.mod2 == Modifier.SWIFTNESS)
            player_controller.Move(movement * 1.15f * Time.deltaTime);
        //player_rigidbody.velocity = movement * (_myPlayerStats.m_Speed * 1.15f);
        else
            player_controller.Move(movement * Time.deltaTime);
        //player_rigidbody.velocity = movement * _myPlayerStats.m_Speed;


    }

    public void MeleeAttack()
    {
        if (Random.Range(1, 101) >= (100 - (100 * melee.stats.critChance)))
        {
            meleeAttack.isCrit = true;
            audio.PlayOneShot(fightingmeleecrit);
        }
        else
        {
            meleeAttack.isCrit = false;
        }
        if (meleeCooldown <= Time.time)
        {
            imAttacking = true;
            audio.PlayOneShot(fightingmelee);
            PhotonNetwork.RPC(photonView, "ToggleHitBox", PhotonTargets.AllBuffered, false);
            meleeCooldown = Time.time + melee.stats.rOF;
        }
    }

    public void RangedAttack()
    {
        if (rangedCooldown <= Time.time && rangedAmmo > 0)
        {
            imAttacking = true;

            audio.PlayOneShot(fightingranged);
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

    void AttackInput()
    {
        //Primary Attack
        if (Input.GetKeyDown(KeyCode.Space))
        {
            MeleeAttack();
        }

        //Secondary attack
        if (Input.GetKeyDown(KeyCode.Q))
        {
            RangedAttack();
        }

        if (weaponTrigger && pickup != null)
        {
            if (meleeButton.sprite == attackOrGet)
                theButtons[(int)Botones.MELEE].onClick.AddListener(PickUpWeapon);
                
            else if (rangedButton.sprite == attackOrGet)
                theButtons[(int)Botones.RANGED].onClick.AddListener(PickUpWeapon);
        }
        else
        {
            theButtons[(int)Botones.MELEE].onClick.AddListener(MeleeAttack);
            theButtons[(int)Botones.RANGED].onClick.AddListener(RangedAttack);
        }
    }

    void UpdateVariables()
    {
        posicionJugador = transform.position;

        if (meleeCooldown < melee.stats.rOF)
            meleeCooldown += Time.time;
        if (rangedCooldown < ranged.stats.rOF)
            rangedCooldown += Time.time;

        if(delayMovement > 0)
        {
            delayMovement -= Time.deltaTime;
            if(delayMovement <= 0)
            {
                imAttacking = false;
            }
        }

        ammoLeft.text = rangedAmmo.ToString();
    }

    void PickUpWeapon()
    {
        WeaponPickup weapon = pickup.GetComponent<WeaponPickup>();
        ChangeWeapon(ref weapon.type, ref weapon.rarity, PhotonConnection.GetInstance().randomSeed, ref weapon.ID, weapon.lastWear);
        if (weapon.type == WeaponType.MELEE)
            meleeButton.sprite = meleeSprites[(int)melee.rarity];
        else
            rangedButton.sprite = rangedSprites[(int)ranged.rarity];

        StartCoroutine(PhotonConnection.GetInstance().WaitFrame());
    }

    void OnTriggerEnter(Collider col)
    {
        
        if (col.CompareTag("HitMelee") || (col.CompareTag("Proyectile") && (col.GetComponent<projectile>().owner == photonView.ownerId)))
        {
            Attack attack = col.GetComponent<Attack>();
            _myPlayerStats.ReceiveDamage(col.GetComponent<Attack>().armourPen, col.GetComponent<Attack>().damage);
            PhotonNetwork.RPC(photonView, "TakeDamage", PhotonTargets.All, false, ID);

            if (transform.position.x > col.transform.position.x)
                if (attack.effect1 == Modifier.KNOCKBACK || attack.effect2 == Modifier.KNOCKBACK)
                    KnockBack(Vector3.right, 1f);
                else
                    KnockBack(Vector3.right, 0.5f);
            else
                if (attack.effect1 == Modifier.KNOCKBACK || attack.effect2 == Modifier.KNOCKBACK)
                    KnockBack(Vector3.left, 1f);
                else
                    KnockBack(Vector3.left, 0.5f);

            if ((attack.effect1 == Modifier.BLEEDING || attack.effect1 == Modifier.POISON) && myState == State.NORMAL)
            {
                myState = State.DAMAGE;
                StartCoroutine(TakeDamagePSecond(3));
            }
        }
        if (col.CompareTag("Food") && _myPlayerStats.m_HP < _myPlayerStats.base_HP)
        {
            HealPlayer((int)col.GetComponent<Food>().type);
            object[] ToSend = new object[2];
            ToSend[0] = col.GetComponent<Consumable>().id;
            ToSend[1] = ID;
            PhotonNetwork.RPC(photonView, "DissappearConsumable", PhotonTargets.All, false, ToSend);
        }
        if (col.CompareTag("Armour") && _myPlayerStats.m_Shield < _myPlayerStats.base_Shield)
        {
            ArmourUp((int)col.GetComponent<Armour>().type);
            object[] ToSend = new object[2];
            ToSend[0] = col.GetComponent<Consumable>().id;
            ToSend[1] = ID;
            PhotonNetwork.RPC(photonView, "DissappearConsumable", PhotonTargets.All, false, ToSend);
        }
        if (col.CompareTag("Ammo") && rangedAmmo < 30)
        {
            Resuply((int)col.GetComponent<Ammo>().type);
            object[] ToSend = new object[2];
            ToSend[0] = col.GetComponent<Consumable>().id;
            ToSend[1] = ID;
            PhotonNetwork.RPC(photonView, "DissappearConsumable", PhotonTargets.All, false, ToSend);
        }
        if (col.CompareTag("Melee") || col.CompareTag("Rango"))
        {
            if (col.CompareTag("Melee"))
                meleeButton.sprite = attackOrGet;
            else
                rangedButton.sprite = attackOrGet;

            weaponTrigger = true;
            pickup = col;
        }
    }

    private void OnTriggerExit(Collider col)
    {
        if (col.CompareTag("Melee") || col.CompareTag("Rango"))
        {
            if (col.CompareTag("Melee"))
                meleeButton.sprite = meleeSprites[(int)melee.rarity];
            else
                rangedButton.sprite = rangedSprites[(int)ranged.rarity];

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
        melee.type = WeaponType.MELEE;
        melee.rarity = WeaponRarity.COMMON;
        melee.sprite = meleeSprites[(int)melee.rarity];
        melee.stats = WeaponStats.SetStats(melee.stats, PhotonConnection.GetInstance().randomSeed, melee.type, melee.rarity, -1, -1);
        meleeAttack.damage = melee.stats.damage;
        meleeAttack.armourPen = melee.stats.armourPen;

        ranged.type = WeaponType.RANGED;
        ranged.rarity = WeaponRarity.COMMON;
        ranged.sprite = meleeSprites[(int)ranged.rarity];
        ranged.stats = WeaponStats.SetStats(ranged.stats, PhotonConnection.GetInstance().randomSeed, ranged.type, ranged.rarity, -2, -1);
        rangedAttack.damage = ranged.stats.damage;
        rangedAttack.armourPen = ranged.stats.armourPen;

    }

    [PunRPC]
    void ResetMeleeWeapon()
    {
        melee.type = WeaponType.MELEE;
        melee.rarity = WeaponRarity.COMMON;
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
        meleeButton.sprite = meleeSprites[0];
    }

    public void BreakRangedWeapon()
    {
        PhotonNetwork.RPC(photonView, "ResetRangedWeapon", PhotonTargets.All, false);
        rangedButton.sprite = rangedSprites[0];
    }

    [PunRPC]
    public void DissappearWeapon(object[] received)
    {
        for (int i = 0; i < PhotonConnection.GetInstance().weaponList.Count; i++)
        {
            if (PhotonConnection.GetInstance().weaponList[i].ID == (int)received[0])
            {
                PhotonConnection.GetInstance().weaponList[i].gameObject.SetActive(false);
                ParticleManager.GetInstance().ActivateParticle(PhotonConnection.GetInstance().GetPlayerById((int)received[1]).transform, particleGrab);
            }
        }
    }

    [PunRPC]
    public void DissappearConsumable(object[] received)
    {
        for (int i = 0; i < PhotonConnection.GetInstance().consumables.Count; i++)
        {
            if (PhotonConnection.GetInstance().consumables[i].id == (int)received[0])
            {
                PhotonConnection.GetInstance().consumables[i].gameObject.SetActive(false);

                if (PhotonConnection.GetInstance().consumables[i].CompareTag("Food"))
                    ParticleManager.GetInstance().ActivateParticle(PhotonConnection.GetInstance().GetPlayerById((int)received[1]).transform, particleHeal);
                else
                    ParticleManager.GetInstance().ActivateParticle(PhotonConnection.GetInstance().GetPlayerById((int)received[1]).transform, particleGrab);

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
                object[] ToSend = new object[2];
                ToSend[0] = objects[4];
                ToSend[1] = ID;
                PhotonNetwork.RPC(photonView, "DissappearWeapon", PhotonTargets.All, false, ToSend);
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
            object[] ToSend = new object[2];
            ToSend[0] = objects[4];
            ToSend[1] = ID;
            PhotonNetwork.RPC(photonView, "DissappearWeapon", PhotonTargets.All, false, ToSend);
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
                meleeAttack.effect1 = melee.stats.mod1;

            else
                meleeAttack.effect1 = Modifier.NONE;

            ShowParticle(meleeAttack.effect1);
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

            if (ranged.stats.mod1 != Modifier.SPEEDLOAD && ranged.stats.mod1 != Modifier.BOTTOMLESS)
                rangedAttack.effect1 = ranged.stats.mod1;
            else
                rangedAttack.effect1 = Modifier.NONE;

            if (ranged.stats.mod2 != Modifier.SPEEDLOAD && ranged.stats.mod2 != Modifier.BOTTOMLESS)
                rangedAttack.effect2 = ranged.stats.mod1;
            else
                rangedAttack.effect1 = Modifier.NONE;

            ShowParticle(rangedAttack.effect1);
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
                meleeAttack.effect1 = melee.stats.mod1;

            else
                meleeAttack.effect1 = Modifier.NONE;

            ShowParticle(meleeAttack.effect1);
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

        if (ranged.stats.mod1 != Modifier.SPEEDLOAD && ranged.stats.mod1 != Modifier.BOTTOMLESS)
            rangedAttack.effect1 = ranged.stats.mod1;
        else
            rangedAttack.effect1 = Modifier.NONE;

        if (ranged.stats.mod2 != Modifier.SPEEDLOAD && ranged.stats.mod2 != Modifier.BOTTOMLESS)
            rangedAttack.effect2 = ranged.stats.mod1;
        else
            rangedAttack.effect2 = Modifier.NONE;
    }

    [PunRPC]
    public void UpdatePickupWeapon(object[] objects)
    {
        WeaponPickup current;
        for (int i = 0; i < PhotonConnection.GetInstance().weaponList.Count; i++)
        {
            current = PhotonConnection.GetInstance().weaponList[i];
            if (current.ID == (int)objects[2])
            {
                current.ID = (int)objects[0];
                current.rarity = (WeaponRarity)objects[1];
                current.lastWear = (int)objects[4];

                if ((WeaponType)objects[3] == WeaponType.MELEE)
                    current.GetComponent<SpriteRenderer>().sprite = meleeSprites[(int)objects[1]];
                else
                    current.GetComponent<SpriteRenderer>().sprite = rangedSprites[(int)objects[1]];
            }
        }
    }

    void ShowParticle(Modifier mod)
    {
        TypesAvailable.particleType particle;
        particle = TypesAvailable.particleType.NONE;
        switch (mod)
        {
            case Modifier.FRENZY:
                particle = TypesAvailable.particleType.MOD_ATTACK;
                break;
            case Modifier.ARMOURSLAYER:
                particle = TypesAvailable.particleType.MOD_BLOODTHIRST;
                break;
            case Modifier.FOCUS:
                particle = TypesAvailable.particleType.MOD_BLOODTHIRST;
                break;
            case Modifier.SWIFTNESS:
                particle = TypesAvailable.particleType.MOD_SPEED;
                break;
            case Modifier.BLEEDING:
                particle = TypesAvailable.particleType.MOD_BLEED;
                break;
            case Modifier.BLOODTHIRST:
                particle = TypesAvailable.particleType.MOD_BLOODTHIRST;
                break;
            case Modifier.KNOCKBACK:
                particle = TypesAvailable.particleType.MOD_KNOCKBACK;
                break;
            case Modifier.POISON:
                particle = TypesAvailable.particleType.MOD_BLEED;
                break;
            case Modifier.NONE:
                ParticleManager.GetInstance().StopParticle(this.transform);
                break;

        }
        if(particle != TypesAvailable.particleType.NONE)
            ParticleManager.GetInstance().ActivateParticle(transform, particle);
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
                SendStream(stream, mySprite.flipX);

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
                float received_HP = (float)stream.ReceiveNext();
                if(received_HP < _myPlayerStats.m_HP)
                {
                    ParticleManager.GetInstance().ActivateParticle(transform, particleHit);
                }
                _myPlayerStats.m_HP = received_HP;

                _myPlayerStats.m_DamageRange = (float)stream.ReceiveNext();
                _myPlayerStats.m_DamageMelee = (float)stream.ReceiveNext();
                mySprite.flipX = (bool)stream.ReceiveNext();
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
            }
            else if (vivo == true)
            {
                PhotonNetwork.RPC(photonView, "KillPlayer", PhotonTargets.AllBuffered, false, ID);
            }
            else if (Input.GetKey(KeyCode.Space))
            {
                PhotonNetwork.RPC(photonView, "RevivePlayer", PhotonTargets.AllBuffered, false, ID);
            }

        }
            
             
        if (PhotonNetwork.connected)
        {
            _myPlayerStats.UpdateScoreboard();
            health.ActivarAudioListener();
        }
        if (PhotonNetwork.player.NickName == "")
            PhotonNetwork.player.NickName = "Jugador #" + Random.Range(1.00f, 9.00f);
    }

    public void SendStream(PhotonStream stream, object value)
    {
        stream.SendNext(value);
    }

    [PunRPC]
    IEnumerator ToggleHitBox()
    {
        
        BasicHitBox.GetComponent<Collider>().enabled = true;
        print(BasicHitBox.GetComponent<Collider>().enabled);
        yield return attackFrame;
        BasicHitBox.GetComponent<Collider>().enabled = false;   //will go back to waiting if another object is hit after detecting one with space. Will need counter for animation
        imAttacking = false;
        print(BasicHitBox.GetComponent<Collider>().enabled);
    }

    public void HealPlayer(int amount)
    {
        if (_myPlayerStats.m_HP + amount > _myPlayerStats.base_HP)
            _myPlayerStats.m_HP = _myPlayerStats.base_HP;
        else
            _myPlayerStats.m_HP += amount;

        health.healthBar.UpdateBar(_myPlayerStats.m_HP, _myPlayerStats.base_HP);
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
    public void TakeDamage(int id)
    {
        ParticleManager.GetInstance().ActivateParticle(PhotonConnection.GetInstance().GetPlayerById(id).transform, particleHit);
    }

    [PunRPC]
    public void RevivePlayer(int id)
    {
        gameObject.GetComponent<BoxCollider>().enabled = true;
        gameObject.GetComponent<SpriteRenderer>().enabled = true;
        gameObject.GetComponent<Rigidbody>().useGravity = true;
        gameObject.GetComponent<PlayerStats>().HP_bar.fillAmount = 1;
        _myPlayerStats.ResetStats();

        int randSpawn = Random.Range(0, terreno.PlayerSpawners.Count);

        transform.position = terreno.PlayerSpawners[randSpawn].transform.position;
        ParticleManager.GetInstance().ActivateParticle(PhotonConnection.GetInstance().GetPlayerById(id).transform, particleSpawn);
        vivo = true;
    }

    [PunRPC]
    public void KillPlayer(int id)
    {
        
        vivo = false;
        if(photonView.isMine)
        {
            
            PhotonNetwork.Disconnect();
        }

    }

    private void OnDisable()
    {
        Debug.Log("Call particle Player Death");
        ParticleManager.GetInstance().ActivateParticle(transform, particleDeath, false);
    }
}
