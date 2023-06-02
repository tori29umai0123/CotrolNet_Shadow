using UnityEngine;
using System.IO;
using System.Collections;

public class Camera_Capture : MonoBehaviour
{
    public GameObject ModelCamera;
    public GameObject LightCamera;
    public GameObject DirectionalLight;
    public GameObject model;
    public int captureW;
    public int captureH;

    public bool IsLoading;
    public Vector3 Pos;
    public string outpath;
    public string modelpath;
    public int count;
    public int max_count;
    public int model_count;

    private bool isCapturing; // 撮影中かどうかを管理するフラグ

    private void Start()
    {
        count = 0;
        model_count = Directory.GetFiles(modelpath, "*", SearchOption.TopDirectoryOnly).Length/2;
        isCapturing = false; // 撮影フラグを初期化

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
        StartCapture();
    }

    private void StartCapture()
    {
        if (!isCapturing)
        {
            StartCoroutine(RandomCapture());
        }
    }

    bool ModelLoad(string model_name)
    {
        GameObject modelPrefab = Resources.Load<GameObject>(model_name);

        model = Instantiate(modelPrefab);

        //modelの座標がズレる時の修正用。位置がおかしくなったらコメントアウトしてみて
        foreach (Transform childTransform in model.transform)
        {
            if (childTransform.gameObject.name != "default.default")
            {
                var child = childTransform.gameObject;
                Pos = child.GetComponent<Renderer>().bounds.center;
                float x = -Pos.x;
                float y = -Pos.y;
                float z = -Pos.z;
                model.transform.Translate(x, y, z);
            }
            if (model.transform.childCount == 1)
            {
                var child = childTransform.gameObject;
                Pos = child.GetComponent<Renderer>().bounds.center;
                float x = -Pos.x;
                float y = -Pos.y;
                float z = -Pos.z;
                model.transform.Translate(x, y, z);
            }
        }
        IsLoading = true ;
        return IsLoading;
    }

    private IEnumerator RandomCapture()
    {
        isCapturing = true; // 撮影フラグを設定

        while (count < max_count)
        {
            count++; // count の値をインクリメント

            int model_rnd = UnityEngine.Random.Range(0, model_count);
            string model_name = model_rnd.ToString("0000");

            yield return new WaitUntil(() => ModelLoad(model_name) == true); // モデルのロードが完了するまで待機
                                                                              // モデルのロードが完了した後に行う処理をここに記述する
                                                                              // ランダム光源及びランダムカメラ配置
            float rot_rnd = UnityEngine.Random.Range(-360, 360);
            ModelCamera.transform.RotateAround(Pos, Vector3.up, rot_rnd);
            DirectionalLight.transform.eulerAngles = new Vector3(rot_rnd, 90, 90);


            // ここで撮影を行う
            CameraSavePng_Model(count.ToString("00000"));
            CameraSavePng_Light(count.ToString("00000"));

            // モデルの撮影が完了したらGameObjectを破棄する
            Destroy(model);

            IsLoading = false;
            if (count >= max_count)
            {
                UnityEditor.EditorApplication.isPlaying = false;
            }
        }

        isCapturing = false; // 撮影フラグを解除
    }

    private void CameraSavePng_Model(string count)
    {
        Camera camera = ModelCamera.GetComponent<Camera>();

        RenderTexture rt = new RenderTexture(captureW, captureH, 24);
        RenderTexture prev = camera.targetTexture;
        camera.targetTexture = rt;
        camera.Render();
        camera.targetTexture = prev;
        RenderTexture.active = rt;

        Texture2D capture = new Texture2D(captureW, captureH, TextureFormat.ARGB32, false);
        capture.ReadPixels(new Rect(0, 0, capture.width, capture.height), 0, 0);
        capture.Apply();
        byte[] bytes = capture.EncodeToPNG();
        string path = Path.Combine(outpath, "target", count + ".png");
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
        Camera camera = LightCamera.GetComponent<Camera>();

        RenderTexture rt = new RenderTexture(captureW, captureH, 24);
        RenderTexture prev = camera.targetTexture;
        camera.targetTexture = rt;
        camera.Render();
        camera.targetTexture = prev;
        RenderTexture.active = rt;

        Texture2D capture = new Texture2D(captureW, captureH, TextureFormat.ARGB32, false);
        capture.ReadPixels(new Rect(0, 0, capture.width, capture.height), 0, 0);
        capture.Apply();
        byte[] bytes = capture.EncodeToPNG();
        string path = Path.Combine(outpath, "light", count + ".png");
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