using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PixelScreen : MonoBehaviour
{
    public int w = 720;
    int h;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        float ratio = ((float)Camera.main.pixelHeight / (float)Camera.main.pixelWidth);
        h = Mathf.RoundToInt(w * ratio);

        if(Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        source.filterMode = FilterMode.Point;
        RenderTexture buffer = RenderTexture.GetTemporary(w, h, - 1);
        buffer.filterMode = FilterMode.Point;
        Graphics.Blit(source, buffer);
        Graphics.Blit(buffer, destination);
        RenderTexture.ReleaseTemporary(buffer);
    }
}
