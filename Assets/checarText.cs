using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class checarText : MonoBehaviour
{
    public string textoachecar;
    public GameObject play;
    // Start is called before the first frame update
    void Start()
    {
      
    }

    // Update is called once per frame
    void Update()
    {
        textoachecar = GameObject.Find("NickName").GetComponent<Text>().text.ToString();

        if (textoachecar!="")
        {
            play.gameObject.SetActive(true);
        }
        else
        {
            play.gameObject.SetActive(false);
        }
    }
}
