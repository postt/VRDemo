using UnityEngine;
using System.Collections;

public class StartGameController : MonoBehaviour
{
    [SerializeField]
    GameObject leftControllerObject;
    [SerializeField]
    GameObject rightControllerObject;
    [SerializeField]
    UnityEngine.UI.Image curtain;

    bool rightTriggerPressed = false;
    bool touchThePickaxe = false;
    bool loading = false;

    void Update()
    {
        SteamVR_Controller.Device rightController = SteamVR_Controller.Input((int)rightControllerObject.GetComponent<SteamVR_TrackedObject>().index);
        rightTriggerPressed = rightController.GetTouchDown(SteamVR_Controller.ButtonMask.Trigger);

        if (!loading && rightTriggerPressed && touchThePickaxe)
        {
            loading = true;
            StartCoroutine(LoadGame());
        }
    }

    void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.name == "RightCollider" && leftControllerObject.activeSelf && rightControllerObject.activeSelf)
        {
            touchThePickaxe = true;
        }
    }

    void OnTriggerExit()
    {
        touchThePickaxe = false;
    }

    IEnumerator LoadGame()
    {
        while (curtain.color.a < 1)
        {
            curtain.color = new Color(0, 0, 0, curtain.color.a + 0.01f);
            yield return null;
        }
        Application.LoadLevelAsync("Jewel");
    }
}
