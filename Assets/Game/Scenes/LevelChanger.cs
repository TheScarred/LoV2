using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelChanger : MonoBehaviour
{
    public Animator animator;
    void Start()
    {
    }
    public void FadeToLevel()
    {
        animator.SetTrigger("FadeOut");
    }
}
