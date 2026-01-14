using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CameraFeed : MonoBehaviour
{
    public RawImage display;
    private WebCamTexture cam;

    IEnumerator Start()
    {
        // Ask for permission
        yield return Application.RequestUserAuthorization(UserAuthorization.WebCam);

        if (Application.HasUserAuthorization(UserAuthorization.WebCam))
        {
            var devices = WebCamTexture.devices;
            cam = new WebCamTexture(devices[0].name);

            display.texture = cam;
            cam.Play();

            // Optional: match RawImage size
            display.rectTransform.sizeDelta = new Vector2(cam.width, cam.height);
        }
    }
}
