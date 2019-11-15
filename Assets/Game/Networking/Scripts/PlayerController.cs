using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private GameObject sprite;

    private int attack = 0;
    private Animator animator;
    private bool lookRight = true;
    private float speed = 3f;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            attack++;
            animator.SetInteger("attack", attack);
        }
        else
        {
            attack = 0;
            animator.SetInteger("attack", attack);
        }

        if (animator.GetCurrentAnimatorStateInfo(0).IsName("attack"))
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                attack = 2;
                animator.SetInteger("attack", attack);
            }
            return;
        }
        else if (animator.GetCurrentAnimatorStateInfo(0).IsName("attack1"))
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                attack = 3;
                animator.SetInteger("attack", attack);
            }
            return;
        }
        else if (animator.GetCurrentAnimatorStateInfo(0).IsName("attack2"))
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            animator.SetTrigger("hit");
        }

        if (animator.GetCurrentAnimatorStateInfo(0).IsName("hit"))
        {
            return;
        }

        bool isMoving = false;

        if (Input.GetKey(KeyCode.W))
        {
            transform.Translate(Vector3.forward * speed * Time.deltaTime);
            isMoving = true;
        }

        if (Input.GetKey(KeyCode.S))
        {
            transform.Translate(Vector3.back * speed * Time.deltaTime);
            isMoving = true;
        }

        if (Input.GetKey(KeyCode.A))
        {
            transform.Translate(Vector3.left * speed * Time.deltaTime);
            isMoving = true;
            if (lookRight)
            {
                lookRight = false;
                sprite.transform.localScale = new Vector3(sprite.transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
            }
        }

        if (Input.GetKey(KeyCode.D))
        {
            transform.Translate(Vector3.right * speed * Time.deltaTime);
            isMoving = true;
            if (!lookRight)
            {
                lookRight = true;
                sprite.transform.localScale = new Vector3(sprite.transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
            }
        }

        print(attack);
        animator.SetBool("run", isMoving);
    }

}
