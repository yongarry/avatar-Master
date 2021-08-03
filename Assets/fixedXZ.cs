using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class fixedXZ : MonoBehaviour
{
    public Transform Transformation;
    private Vector3 newposition;
    void Update()
    {
        newposition = new Vector3();
        newposition.y = Transformation.position.y;
        Transformation.position = newposition;
    }
}
