using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Behavior : MonoBehaviour
{
    public Animator animator;
    public bool lookRight = true;
    public GameObject player;
    int attack;
    public int speed;
    //GameObject sprite;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        bool isMoving = false;
        #region GetHit
        if (Input.GetKeyDown(KeyCode.P))
        {
            animator.SetTrigger("hit");
           

        }

        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Zero_hit"))
        {
            return;
        }
        #endregion
        #region GetHit
        if (Input.GetKeyDown(KeyCode.M))
        {
            animator.SetTrigger("death");
           

        }

        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Zero_Death"))
        {
            return;
        }
        #endregion
        #region Attack
        if (Input.GetKeyDown(KeyCode.Space))
        {
            attack++;
            animator.SetInteger("attack", attack);
            //Debug.Log(attack);
        }
        else
        {
            attack = 0;
            animator.SetInteger("attack", attack);
        }

        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Zero_Attack1"))
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                attack = 2;
                animator.SetInteger("attack", attack);
            }
            return;
        }
        else if (animator.GetCurrentAnimatorStateInfo(0).IsName("Zero_Attack2"))
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                attack = 3;
                animator.SetInteger("attack", attack);
            }
            return;
        }
        else if (animator.GetCurrentAnimatorStateInfo(0).IsName("Zero_Attack3"))
        {
            return;
        }
        #endregion
        #region Move

        if (Input.GetKey(KeyCode.S))
        {
            transform.Translate(Vector3.back * speed * Time.deltaTime);
            isMoving = true;
        }
        else if (Input.GetKey(KeyCode.W))
        {
            transform.Translate(Vector3.forward * speed * Time.deltaTime);
            isMoving = true;
        }


        if (Input.GetKey(KeyCode.A))
        {
            transform.Translate(Vector3.left * speed * Time.deltaTime);
            isMoving = true;
            if (lookRight)
            {
                lookRight = false;
                player.transform.localScale = new Vector3(player.transform.localScale.x * -1, player.transform.localScale.y, player.transform.localScale.z);
            }
        }
        else if (Input.GetKey(KeyCode.D))
        {
            isMoving = true;
            transform.Translate(Vector3.right * speed * Time.deltaTime);
            if (!lookRight)
            {
                lookRight = true;
                player.transform.localScale = new Vector3(player.transform.localScale.x * -1, player.transform.localScale.y, player.transform.localScale.z);
            }
        }

        animator.SetBool("run", isMoving);
    }
}
#endregion