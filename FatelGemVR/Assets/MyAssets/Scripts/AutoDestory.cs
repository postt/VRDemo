using UnityEngine;
using System.Collections;

public class AutoDestory : MonoBehaviour
{
    [SerializeField]
    float time;

    IEnumerator Start()
    {
        yield return new WaitForSeconds(time);
        Destroy(this.gameObject);
    }
}
