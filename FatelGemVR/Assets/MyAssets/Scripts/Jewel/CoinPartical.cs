using UnityEngine;
using System.Collections;

public class CoinPartical : MonoBehaviour
{
    public Transform TargetTransform;
    [SerializeField]
    GameObject destoryPartical;

    Transform modelChild;

    void Awake()
    {
        modelChild = this.transform.FindChild("Model");
    }

    void Start ()
    {
        StartCoroutine(Grow());
        StartCoroutine(SelfRotate());
        StartCoroutine(TransformLerping());
	}

    IEnumerator Grow()
    {
        float speed = 1.5f;
        this.transform.localScale = new Vector3(0, 0, 0);
        while (this.transform.localScale.x < 1)
        {
            float lerping = Mathf.Lerp(this.transform.localScale.x, 1, Time.deltaTime * speed);
            this.transform.localScale = new Vector3(lerping, lerping, lerping);
            if (this.transform.localScale.x > 0.99f)
            {
                break;
            }
            yield return null;
        }
    }

    IEnumerator SelfRotate()
    {
        while (true)
        {
            modelChild.Rotate(new Vector3(5, 0, 0), Space.Self);
            yield return null;
        }
    }

    IEnumerator TransformLerping()
    {
        float speed = Random.Range(0.025f, 0.125f);
        float lerping = 0;
        while (Vector3.Distance(this.transform.position, TargetTransform.position) > 0.05f)
        {
            lerping += speed * Time.deltaTime;
            Vector3 lerpingPos = new Vector3(
                Mathf.Lerp(this.transform.position.x, TargetTransform.position.x, lerping),
                Mathf.Lerp(this.transform.position.y, TargetTransform.position.y, lerping),
                Mathf.Lerp(this.transform.position.z, TargetTransform.position.z, lerping)
                );
            this.transform.position = lerpingPos;
            yield return null;
        }
        Instantiate(destoryPartical, this.transform.position, destoryPartical.transform.rotation);
        GameController.Instance.PlayCoinSound();
        Destroy(this.gameObject);
    }
}
