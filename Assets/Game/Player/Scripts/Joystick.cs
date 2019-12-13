using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Joystick : MonoBehaviour
{
    public Canvas father;
    public float radius;
    Vector3 initialPos;
    Vector2 newPos;
    Vector2 pos;

    private Vector2 axis;
    public Vector2 axisPublic
    {
        get
        {
            return axis;
        }
    }
    public float horizontal
    {
        get
        {
            return axis.x;
        }
    }
    public float vectical
    {
        get
        {
            return axis.y;
        }
    }
    public bool isMoving
    {
        get
        {
            return axis != Vector2.zero;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        initialPos = transform.position;
    }

    public void Drag()
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(father.transform as RectTransform, Input.mousePosition, father.worldCamera, out pos);
        transform.position = pos;
        newPos= father.transform.TransformPoint(pos) - initialPos;
        newPos.x = Mathf.Clamp(newPos.x, -radius, radius);
        newPos.y = Mathf.Clamp(newPos.y, -radius, radius);
        transform.localPosition = newPos;

        axis = newPos / radius;
    }

    public void Drop()
    {
        transform.position = initialPos;
        axis = Vector2.zero;
    }
}
