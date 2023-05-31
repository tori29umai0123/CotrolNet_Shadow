using UnityEngine;
using System.IO;
using System.Collections;
using System;

public class Camera_Capture : MonoBehaviour
{
    public GameObject ModelCamera;
    public GameObject LightCamera;
    public GameObject DirectionalLight;

    public Vector3 Pos;
    public ModelLoader ModelLoader;
    public string outpath;
    public string modelpath;
    public int count;
    public int max_count;
    public int model_count;
    public int load_count;
    public bool load;

    private void Start()
    {
        count = -1;
        model_count = Directory.GetFiles(modelpath, "*", SearchOption.TopDirectoryOnly).Length;
        StartCoroutine(RandomCapture());

        string targetFolderPath = Path.Combine(outpath, "target");
        if (!Directory.Exists(targetFolderPath))
        {
            Directory.CreateDirectory(targetFolderPath);
        }

        string lightFolderPath = Path.Combine(outpath, "light");
        if (!Directory.Exists(lightFolderPath))
        {
            Directory.CreateDirectory(lightFolderPath);
        }
    }

    private void ModelLoad(string model_name)
    {
        ModelLoader.FileLoad(modelpath + "/" + model_name + ".obj");
        Debug.Log("Start Heavy Method: " + System.Threading.Thread.CurrentThread.ManagedThreadId);
        System.Threading.Thread.Sleep(1000);
        Debug.Log("End Heavy Method: " + System.Threading.Thread.CurrentThread.ManagedThreadId);
    }

    private IEnumerator RandomCapture()
    {
        while (true)
        {
            count += 1;
            int model_rnd = UnityEngine.Random.Range(0, model_count);
            float rot_rnd = UnityEngine.Random.Range(-360, 360);
            string model_name = model_rnd.ToString("0000");
            ModelLoad(model_name);
            ModelCamera.transform.RotateAround(Pos, Vector3.up, rot_rnd);
            DirectionalLight.transform.eulerAngles = new Vector3(rot_rnd, 90, 90);
            yield return new WaitForSeconds(1);

            if (count > 0)
            {
                if (load)
                {
                    count = load_count;
                    load = false;
                }
                CameraSavePng_Model(count.ToString("00000"));
                CameraSavePng_Light(count.ToString("00000"));
            }

            if (count == max_count + 1)
            {
                break;
            }
        }
    }

    private void Update()
    {
        if (count == max_count + 1)
        {
            UnityEditor.EditorApplication.isPlaying = false;
        }
    }

    private void CameraSavePng_Model(string count)
    {
        int captureSizeX = 512;
        int captureSizeY = 512;

        Camera camera = ModelCamera.GetComponent<Camera>();

        var rt = new RenderTexture(captureSizeX, captureSizeY, 24);
        var prev = camera.targetTexture;
        camera.targetTexture = rt;
        camera.Render();
        camera.targetTexture = prev;
        RenderTexture.active = rt;

        var capture = new Texture2D(captureSizeX, captureSizeY, TextureFormat.ARGB32, false);
        capture.ReadPixels(new Rect(0, 0, capture.width, capture.height), 0, 0);
        capture.Apply();
        var bytes = capture.EncodeToPNG();
        var path = Path.Combine(outpath, "target", count + ".png");
        if (string.IsNullOrEmpty(path))
        {
            return;
        }
        File.WriteAllBytes(path, bytes);

        RenderTexture.active = null;
        camera.targetTexture = null;
        Destroy(rt);
    }

    private void CameraSavePng_Light(string count)
    {
        int captureSizeX = 512;
        int captureSizeY = 512;

        Camera camera = LightCamera.GetComponent<Camera>();

        var rt = new RenderTexture(captureSizeX, captureSizeY, 24);
        var prev = camera.targetTexture;
        camera.targetTexture = rt;
        camera.Render();
        camera.targetTexture = prev;
        RenderTexture.active = rt;

        var capture = new Texture2D(captureSizeX, captureSizeY, TextureFormat.ARGB32, false);
        capture.ReadPixels(new Rect(0, 0, capture.width, capture.height), 0, 0);
        capture.Apply();
        var bytes = capture.EncodeToPNG();
        var path = Path.Combine(outpath, "light", count + ".png");
        if (string.IsNullOrEmpty(path))
        {
            return;
        }
        File.WriteAllBytes(path, bytes);

        RenderTexture.active = null;
        camera.targetTexture = null;
        Destroy(rt);
    }
}