using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SimpleHealthBar_SpaceshipExample;
using Photon;
public class animacionmonito : PunBehaviour
{
    public int velocidad;
    // Start is called before the first frame update
   
    bool LookRight = true;

    public GameObject jugador;
    public Animator animator;
    int Ataque=0;
    bool vivo = false;
    public FloatingJoystick theJoystick;
    //BUTTONS
    enum Botones { RANGED, MELEE };
    public Button[] theButtons;

    bool opcion = false;

    [SerializeField]
    PlayerHealth p;

    private void Start()
    {
        theJoystick = FindObjectOfType<FloatingJoystick>();
        theButtons = FindObjectsOfType<Button>();
    }

    // Update is called once per frame
    void Update()
    {
        if(photonView.isMine)
        {
            #region GetHit
            if (animator.GetCurrentAnimatorStateInfo(0).IsName("Hurt"))
            {

                if (p.jugadorsin.m_HP <= 0)
                {
                    vivo = true;
                    //animator.SetBool("Morir",true);
                }
                else
                {
                    return;

                }

            }
            #endregion
            #region AtaqueMelee
            if (opcion == false)
            {
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    AttackMelee();

                }
                else
                {
                    Ataque = 0;
                    animator.SetInteger("Ataque", Ataque);
                }
            }
            if (p.jugadorsin.m_HP <= 0)
            {
                animator.SetBool("Morir", true);
            }

            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                opcion = true;
            }
            if (opcion != false)
            {
                if (Input.GetKeyDown(KeyCode.Q))
                {

                    AttackRanged();

                }
                else
                {
                    Ataque = 3;
                    animator.SetInteger("Ataque", Ataque);
                    animator.SetBool("Arma", opcion);
                }
            }

            theButtons[(int)Botones.MELEE].onClick.AddListener(Melee);
            theButtons[(int)Botones.RANGED].onClick.AddListener(Ranged);





            #endregion



            #region Move
            bool isMoving = false;

            if (theJoystick.Horizontal != 0 || theJoystick.Vertical != 0)
            {
                isMoving = true;
            }
            else if (Input.GetKey(KeyCode.S))
            {
                transform.Translate(Vector3.back * velocidad * Time.deltaTime);
                isMoving = true;
            }
            else if (Input.GetKey(KeyCode.W))
            {
                transform.Translate(Vector3.forward * velocidad * Time.deltaTime);
                isMoving = true;
            }


            if (Input.GetKey(KeyCode.A))
            {
                transform.Translate(Vector3.left * velocidad * Time.deltaTime);
                isMoving = true;
                /*if (LookRight)
                {
                    LookRight = false;
                    jugador.transform.localScale = new Vector3(jugador.transform.localScale.x * -1, jugador.transform.localScale.y, jugador.transform.localScale.z);
                }*/
            }
            else if (Input.GetKey(KeyCode.D))
            {
                isMoving = true;
                transform.Translate(Vector3.right * velocidad * Time.deltaTime);
                /*if (!LookRight)
                {
                    LookRight = true;
                    jugador.transform.localScale = new Vector3(jugador.transform.localScale.x * -1, jugador.transform.localScale.y, jugador.transform.localScale.z);
                }*/
            }

            animator.SetBool("run", isMoving);
            #endregion
        }

        
    }
    void Melee()
    {
        animator.SetInteger("Ataque", 1);
    }

    void Ranged()
    {
        animator.SetInteger("Ataque", 2);
    }

    void AttackMelee()
    {
        Ataque++;
        if (Ataque == 1)
        {
            animator.SetInteger("Ataque", Ataque);
        }
    }

    void AttackRanged()
    {
        Ataque = 2;

        if (Ataque == 2)
        {
            animator.SetInteger("Ataque", Ataque);
            animator.SetBool("Arma", opcion);
        }
    }

}
