using UnityEngine;
using System.Collections;

public class GuideController : MonoBehaviour
{
    [SerializeField]
    AudioClip[] vocals;
    [SerializeField]
    string[] texts;

    [SerializeField]
    AudioSource vocalSource;
    [SerializeField]
    UnityEngine.UI.Text textSource;

    int index = 0;

    public bool Next()
    {
        if (index < vocals.Length)
        {
            vocalSource.Stop();
            vocalSource.PlayOneShot(vocals[index],2f);
            textSource.text = texts[index];
            index++;
            return true;
        }
        else
        {
            return false;
        }
    }
}
