using UnityEngine;
using System.Collections;

public class Rotater : MonoBehaviour
{
    [SerializeField]
    Vector3 vec;

	void Update ()
    {
        this.transform.Rotate(vec);
	}
}
