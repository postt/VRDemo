using UnityEngine;
using System.Collections;

public class UI_SetParent : MonoBehaviour
{
    [SerializeField]
    Transform parent;

    void Awake()
    {
        this.transform.SetParent(parent);
    }
}
