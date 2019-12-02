﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;
using UnityEngine.UI;

public class EnemyIA : PunBehaviour
{
    public enum EnemyState
    {
        Chase,
        Patrolling,
        Rage,
        Resting,
        Attacking
    }

    //EnemyStats
    float base_HP = 50;

    AudioSource audio;
    public float timeToSound = 0.5f;
    float timeCounter = 0;
    public AudioClip[] audioList;
    [SerializeField]
    AudioClip sword,death,hit;

    public float HP = 50;
    public float Damage = 10f;
    public float ArmourPen = 0;
    public int killed_points = 25;
    public float CoolDownTime = 1.5f;  //how often can it make damage

    public GameObject melee;
    public GameObject ranged;
    public GameObject consumable;
    public Sprite[] meleeSprites;
    public Sprite[] rangedSprites;
    public Sprite[] foodSprites;
    public Sprite[] armourSprites;
    public Sprite[] ammoSprites;

    public Animator animator;
    public Items.ItemType contains;
    public EnemyState status;
    public Rigidbody enemy_rigidbody;
    public LayerMask targetMask;
    public LayerMask obstacleMask;
    public GameObject playertoChase;
    public List<Transform> visibleTargets = new List<Transform>();
    public Transform patternPoint;
    public GameObject go;
    public float viewRadius;
    [Range(0, 360)]
    public float viewAngle;
    public float stoppingDistance;
    public float speed;
    public float WalkDistance;
    public float startWaitTime;
    private float waitTime;
    private bool facingRight = false;

    float minX, maxX, minY, maxY;
    float healTimer;
    WaitForSeconds delayToSearchForPlayer;

    //HitPlayer
    bool TranslatedRight;
    bool can_attack;
    WaitForSeconds CoolDown;

    //Communicate that player has been hit by enemy (Melee):
    PlayerStats player_stats;

    //Player that kills enemy
    PlayerStats killer;

    //Enemy HP Bar
    public EnemyBar script_HP;
    public Image HP_bar;
    public float damage_percentage;



    void Awake()
    {
        HP_bar.fillAmount = 1;
        audio = gameObject.GetComponent<AudioSource>();
    }
    void Start()
    {
        Random.InitState(PhotonConnection.GetInstance().randomSeed);
        contains = Items.ItemType.CONSUMABLE;
        minX = 4.75f;
        maxX = 24.75f;
        minY = 4.75f;
        maxY = 14.75f;
        //patternPoint.gameObject.GetComponent<PatternPoint>().GenerateNewPosition(minX, maxX, minY, maxY);
        waitTime = startWaitTime;
        speed = 1f;
        status = EnemyState.Patrolling;
        delayToSearchForPlayer = new WaitForSeconds(0.3f);
        StartCoroutine("FindTargets");
        healTimer = 2f;
    }
    public void OnEnable()
    {
        HP_bar.fillAmount = 1;
        minX = 4.75f;
        maxX = 24.75f;
        minY = 4.75f;
        maxY = 14.75f;
        //patternPoint.gameObject.GetComponent<PatternPoint>().GenerateNewPosition(minX, maxX, minY, maxY);
        waitTime = startWaitTime;
        speed = 1f;
        status = EnemyState.Patrolling;
        StartCoroutine("FindTargets", .2f);
        HP = base_HP;

        //HitPlayer
        TranslatedRight = true;
        can_attack = true;
        CoolDown = new WaitForSeconds(CoolDownTime);
    }

    IEnumerator FindTargets()
    {
        while (true)
        {
            //Cada cierto tiempo busco jugadores en mi rango de visión
            yield return delayToSearchForPlayer;
            FindVisibleTargets();
        }
    }
    void PlayWalking()
    {
        int audioToPlay = Random.Range(0, audioList.Length);
        audio.PlayOneShot(audioList[audioToPlay]);
    }

    //Metodo con el que saco el angúlo de visión de mi enemy
    public Vector3 DirFromAngle(float angleInDegres, bool angleIsGlobal)
    {
        if (!angleIsGlobal)
        {
            angleInDegres += transform.eulerAngles.y;
        }
        //Regresó un vector3 multiplicando el seno y coseno del triangulo que quiero sacar
        return new Vector3(Mathf.Sin(angleInDegres * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegres * Mathf.Deg2Rad));
    }

    void FindVisibleTargets()
    {

        //Limpio la lista de jugadores en vista, para evitar que se acaparé
        visibleTargets.Clear();
        Collider[] targetsInViewRadius = Physics.OverlapSphere(transform.position, viewRadius, targetMask);
        for (int i = 0; i < targetsInViewRadius.Length; i++)
        {

            Transform target = targetsInViewRadius[i].transform;
            Vector3 dirToTarget = (target.position - transform.position).normalized;
            if (Vector3.Angle(transform.forward, dirToTarget) < viewAngle / 2)
            {
                float dstToTarget = Vector3.Distance(transform.position, target.position);

                if (!Physics.Raycast(transform.position, dirToTarget, dstToTarget, obstacleMask))
                {
                    visibleTargets.Add(target);

                    status = EnemyState.Chase;
                    if (visibleTargets.Count == 1)
                    {
                        playertoChase = target.gameObject;
                        player_stats = playertoChase.GetComponent<PlayerStats>(); // connect to target script
                    }
                    else 
                    {
                        for (int x = 0; x < visibleTargets.Count; x++)
                        {
                            if ((Vector3.Distance(transform.position, visibleTargets[x].transform.position) < (Vector3.Distance(transform.position, playertoChase.transform.position))))
                            {
                                playertoChase = visibleTargets[x].gameObject;
                            }
                        }

                    }
                }
            }
        }

        if(visibleTargets.Count == 0)
        {    
                playertoChase = null;
            if(HP >= 50)
            {
                status = EnemyState.Patrolling;
            }
        }
    }


public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("HitMelee"))
        {
            if (player_stats != null)
            {
                Attack attack = other.GetComponent<Attack>();

                if (attack.isCrit)
                {
                    HP -= (attack.damage * 2);
                    //Debug.Log("CRIT! Damage Done: " + attack.damage*2);
                }
                else
                {
                    HP -= attack.damage;
                    //Debug.Log("Damage Done: " + attack.damage);
                }

                if (attack.GetComponentInParent<Player>().melee.stats.id >= 0)
                    attack.GetComponentInParent<Player>().melee.stats.wear--;

                if (attack.GetComponentInParent<Player>().melee.stats.wear <= 0 && attack.GetComponentInParent<Player>().melee.stats.id >= 0)
                    attack.GetComponentInParent<Player>().BreakMeleeWeapon();

                script_HP.ModifyHpBar(attack.damage, base_HP);
                audio.PlayOneShot(hit);
                animator.SetTrigger("hit");
                
               /*if(HP <= 20)
                {
                    int type = Random.Range(0, 10);
                    if(type >= 8)
                    {
                        float damage = (player_stats.m_DamageMelee);
                        HP -= (int)damage;
                        script_HP.ModifyHpBar(damage, base_HP);
                        audio.PlayOneShot(hit);
                        animator.SetTrigger("hit");
                    }
                    else
                    {
                        //COLOCAR ANIMACIÓN O MENSAJE DE ESQUIVE AQUÍ
                    }
                }
                else
                {
                    float damage = (player_stats.m_DamageMelee);
                    HP -= (int)damage;
                    script_HP.ModifyHpBar(damage, base_HP);
                    audio.PlayOneShot(hit);
                    animator.SetTrigger("hit");
                }*/

                if (HP <= 0)
                {
                    killer = other.gameObject.transform.parent.gameObject.GetComponent<PlayerStats>();
                    killer.KilledTarget(killed_points);
                    PhotonNetwork.player.AddScore(killed_points);
                    killer.UpdateScoreboard();

                }
            }
        }
        else if (other.gameObject.CompareTag("Proyectile"))
        {
            Attack attack = other.GetComponent<Attack>();

            if (attack.isCrit)
            {
                HP -= (attack.damage * 2);
                //Debug.Log("CRIT! Damage Done: " + attack.damage * 2);
            }
            else
            {
                HP -= attack.damage;
                //Debug.Log("Damage Done: " + attack.damage);
            }

            script_HP.ModifyHpBar(attack.damage, base_HP);
            audio.PlayOneShot(hit);
            animator.SetTrigger("hit");

            if (HP <= 0)
            {
                int col_id = other.gameObject.GetComponent<projectile>().owner;

                killer = PhotonConnection.GetInstance().GetPlayerById(col_id).GetComponent<PlayerStats>();
                killer.KilledTarget(killed_points);
                PhotonNetwork.player.AddScore(killed_points);
            }
            other.gameObject.SetActive(false);
        }
    }
    void PatrolArea(int modifier)
    {
        //Me muevo al patrollingPoint
        transform.position = Vector3.MoveTowards(transform.position, patternPoint.position, speed * modifier * Time.deltaTime);
        if (Vector3.Distance(transform.position, patternPoint.position) < WalkDistance)
        {
            if (waitTime <= 0)
            {
                //Asigno nuevo punto de patrolling después de que paso el tiempo de espera
                patternPoint.gameObject.GetComponent<PatternPoint>().GenerateNewPosition(minX, maxX, minY, maxY);

                waitTime = startWaitTime;
            }
            else
            {
                waitTime -= Time.deltaTime;
            }
        }
    }

    void ChasePlayer()
    {
        bool didMove = false;

        //Persigo a jugador
        if (Vector3.Distance(transform.position, playertoChase.transform.position) > stoppingDistance)
        {
            didMove = true;
            if (didMove)
            {
                timeCounter += Time.deltaTime;
                if (timeCounter > timeToSound)
                {
                    timeCounter = 0;
                    PlayWalking();
                }
            }
           
            if(status == EnemyState.Rage)
            {
                transform.position = Vector3.MoveTowards(transform.position, playertoChase.transform.position, speed* 2f * Time.deltaTime);
            }
            else
            {
                transform.position = Vector3.MoveTowards(transform.position, playertoChase.transform.position, speed * Time.deltaTime);
            }
           if (this.transform.position.x < playertoChase.transform.position.x && facingRight)   //player is on the left
            {
                Vector3 scale = transform.localScale;
                facingRight = false;
                scale.x *= -1;
                transform.localScale = scale;
            }
            if (this.transform.position.x > playertoChase.transform.position.x && !facingRight)  // player is on the right
            {
                Vector3 scale = transform.localScale;
                facingRight = true;
                scale.x *= -1;
                transform.localScale = scale;
            }
        }
        else
        {
            if (can_attack)
            {
                animator.SetTrigger("attak");
                StartCoroutine(Attack());
                can_attack = false;
                audio.PlayOneShot(sword);
            }

            if (animator.GetCurrentAnimatorStateInfo(0).IsName("Zero_Attack1"))
                return;
        }


        //Paso a estado patrolling si no hay jugadores cerca
        if (visibleTargets.Count == 0)
        {
            playertoChase = null;
            status = EnemyState.Patrolling;
            enemy_rigidbody.velocity = Vector3.zero;
            enemy_rigidbody.angularVelocity = Vector3.zero;
        }
    }

    IEnumerator Attack()
    {
        yield return CoolDown;
        if (playertoChase != null && Vector3.Distance(transform.position, playertoChase.transform.position) < stoppingDistance)    //puede ser muy muy pesado
        {
            if (this.transform.position.x > playertoChase.transform.position.x)   //player is on the left
            {
                //Debug.Log("Attacked Player on left!");
            }
            else  // player is on the right
            {
                //Debug.Log("Attacked player on right!");
            }
            if (player_stats != null)
            {
                player_stats.ReceiveDamage(ArmourPen, Damage);
            }
            can_attack = true;
           
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        /*if (stream.isWriting)
        {
            // stream.SendNext(transform.position);
            //stream.SendNext(transform.rotation);
            //stream.SendNext(HP_bar);

        }
        else
        {

            //HP_bar = (float)stream.ReceiveNext();
        }*/
    }

    void HealSelf()
    {
        if(playertoChase == null)
        {
            healTimer -= Time.deltaTime;
            if(healTimer <= 0)
            {
                HP += 2;
                healTimer = 2;
                script_HP.bar_HP.fillAmount += 2;
            }
        }
        else
        {
            status = EnemyState.Chase;
        }
    }

    void Update()
    {
        if (photonView.isMine)
        {
            if (status == EnemyState.Chase || status == EnemyState.Rage)
            {

                if (playertoChase != null)
                {
                    ChasePlayer();
                }
                else
                {
                    if (status == EnemyState.Rage)
                    {
                        PatrolArea(2);
                    }  
                }

            }else if(status == EnemyState.Patrolling)
            {
                PatrolArea(1);
            }
            else if (status == EnemyState.Resting)
            {
                HealSelf();
            }
          
        }
        if (HP <= 0)
        {
            animator.SetBool("death", true);

            if (animator.GetCurrentAnimatorStateInfo(0).IsName("Zero_Death") && photonView.isMine)
            {
                audio.PlayOneShot(death);
                RPCForEnemyDeath();
            }
        }
        if (HP <= base_HP / 2)
        {
            status = EnemyState.Rage;
        } else if (HP <= 20 && playertoChase == null)
        {
            status = EnemyState.Resting;
        }


    }

    public void RPCForEnemyDeath()
    {
        byte seed = (byte)Random.Range(0, 256);
        PhotonNetwork.RPC(photonView, "RemoveEnemies", PhotonTargets.AllBuffered, false, seed);
    }

    [PunRPC]
    public void RemoveEnemies(byte seed)
    {
        SpawnItem(seed);
        this.gameObject.SetActive(false);
    }

    [PunRPC]
    public void ReviveEnemy(object[] parameters)
    {
        int i = (int)parameters[0];
        Vector3 pos_terrain = (Vector3)parameters[2];

        gameObject.SetActive(true);
        gameObject.transform.position = pos_terrain;
    }

    void SpawnItem(byte seed)
    {
        Random.InitState(seed);
        int type1 = Random.Range(0, 2);
        int roll = Random.Range(1, 101);

        if (type1 == 0)
            contains = Items.ItemType.CONSUMABLE;
        else
            contains = Items.ItemType.WEAPON;

        switch (contains)
        {
            case Items.ItemType.WEAPON:
                {
                    int type2 = Random.Range(0, 2);
                    if (type2 == 0)
                    {
                        GameObject go = Instantiate(melee, transform.position, transform.rotation);
                        WeaponPickup weapon = go.GetComponent<WeaponPickup>();
                        weapon.type = Items.WeaponType.MELEE;
                        weapon.ID = PhotonConnection.GetInstance().WeaponID;
                        weapon.lastWear = 30;
                        PhotonConnection.GetInstance().weaponList.Add(go.GetComponent<WeaponPickup>());
                        PhotonConnection.GetInstance().WeaponID++;

                        if (roll <= 40)
                        {
                            weapon.rarity = Items.WeaponRarity.UNCOMMON;
                        }
                        else if (roll <= 70)
                        {
                            weapon.rarity = Items.WeaponRarity.RARE;
                        }
                        else if (roll <= 90)
                        {
                            weapon.rarity = Items.WeaponRarity.EPIC;
                        }
                        else
                        {
                            weapon.rarity = Items.WeaponRarity.LEGENDARY;
                        }

                        weapon.gameObject.GetComponent<SpriteRenderer>().sprite = meleeSprites[(int)weapon.rarity];
                    }
                    else
                    {
                        GameObject go = Instantiate(ranged, transform.position, transform.rotation);
                        WeaponPickup weapon = go.GetComponent<WeaponPickup>();
                        weapon.type = Items.WeaponType.RANGED;
                        weapon.ID = PhotonConnection.GetInstance().WeaponID;
                        weapon.lastWear = 30;
                        PhotonConnection.GetInstance().weaponList.Add(go.GetComponent<WeaponPickup>());
                        PhotonConnection.GetInstance().WeaponID++;

                        if (roll <= 40)
                            weapon.rarity = Items.WeaponRarity.UNCOMMON;
                        else if (roll <= 70)
                            weapon.rarity = Items.WeaponRarity.RARE;
                        else if (roll <= 90)
                            weapon.rarity = Items.WeaponRarity.EPIC;
                        else
                            weapon.rarity = Items.WeaponRarity.LEGENDARY;

                        weapon.gameObject.GetComponent<SpriteRenderer>().sprite = rangedSprites[(int)weapon.rarity];
                    }
                    break;
                }
            case Items.ItemType.CONSUMABLE:
                {
                    GameObject go = Instantiate(consumable, transform.position, transform.rotation);
                    int type2 = Random.Range(0, 3);
                    if (type2 == 0)
                    {
                        go.AddComponent<Food>();
                        go.tag = "Food";
                        if (roll >= 80)
                            go.GetComponent<Food>().type = Items.FoodType.MEAL;
                        else
                            go.GetComponent<Food>().type = Items.FoodType.SNACK;

                        if (go.GetComponent<Food>().type == Items.FoodType.SNACK)
                            go.GetComponent<SpriteRenderer>().sprite = foodSprites[0];
                        else
                            go.GetComponent<SpriteRenderer>().sprite = foodSprites[1];
                    }
                    else if (type2 == 1)
                    {
                        go.AddComponent<Armour>();
                        go.tag = "Armour";
                        if (roll <= 50)
                            go.GetComponent<Armour>().type = Items.ArmourType.PLATE;
                        else if (roll <= 85)
                            go.GetComponent<Armour>().type = Items.ArmourType.VEST;
                        else
                            go.GetComponent<Armour>().type = Items.ArmourType.SUIT;

                        if (go.GetComponent<Armour>().type == Items.ArmourType.PLATE)
                            go.GetComponent<SpriteRenderer>().sprite = armourSprites[0];
                        else if (go.GetComponent<Armour>().type == Items.ArmourType.VEST)
                            go.GetComponent<SpriteRenderer>().sprite = armourSprites[1];
                        else
                            go.GetComponent<SpriteRenderer>().sprite = armourSprites[2];
                    }
                    else
                    {
                        go.AddComponent<Ammo>();
                        go.tag = "Ammo";
                        if (roll <= 40)
                            go.GetComponent<Ammo>().type = Items.AmmoType.SINGLE;
                        else if (roll <= 85)
                            go.GetComponent<Ammo>().type = Items.AmmoType.BUNDLE;
                        else
                            go.GetComponent<Ammo>().type = Items.AmmoType.QUIVER;

                        if (go.GetComponent<Ammo>().type == Items.AmmoType.SINGLE)
                            go.GetComponent<SpriteRenderer>().sprite = ammoSprites[0];
                        else if (go.GetComponent<Ammo>().type == Items.AmmoType.BUNDLE)
                            go.GetComponent<SpriteRenderer>().sprite = ammoSprites[1];
                        else
                            go.GetComponent<SpriteRenderer>().sprite = ammoSprites[2];
                    }
                    go.GetComponent<Consumable>().id = PhotonConnection.GetInstance().ConsumableID;
                    PhotonConnection.GetInstance().consumables.Add(go.GetComponent<Consumable>());
                    PhotonConnection.GetInstance().ConsumableID++;
                    break;
                }
        }
    }
}
