using UnityEngine;
using System.IO;
using System.Collections;
using System.Linq;
using UnityEngine.UI;

public class CameraCapture : MonoBehaviour
{
    public GameObject modelCamera;
    public GameObject lightCamera;
    public GameObject directionalLight;
    public Text Max;
    public Text Count;
    public string outpath;
    public string modelpath;
    public int maxCount;
    public int captureWidth;
    public int captureHeight;
    public float captureInterval = 0.5f;

    private bool isCapturing;
    private int currentCount;
    private int modelCount;
    public bool isMiddleLoad;
    public int middleLoadCount;
    private GameObject currentModel;
    private Vector3 modelPosition;

    private void Start()
    {
        isCapturing = false;
        currentCount = 0;
        modelCount = CountModels(modelpath);
        Max.text = maxCount.ToString();
        Count.text = "0"; // 初回のカウントは0で開始する

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

    int CountModels(string path)
    {
        string[] allowedExtensions = { ".obj", ".fbx" };
        string[] files = Directory.GetFiles(path, "*", SearchOption.AllDirectories);

        int count = 0;
        foreach (string file in files)
        {
            string extension = Path.GetExtension(file);
            if (allowedExtensions.Contains(extension))
            {
                count++;
            }
        }

        return count;
    }
    private IEnumerator RandomCapture()
    {
        isCapturing = true;

        while (currentCount < maxCount)
        {
            if (isMiddleLoad)
            {
                currentCount = middleLoadCount;
                isMiddleLoad = false;
            }
            currentCount++;
            Count.text = currentCount.ToString();


            int modelIndex = UnityEngine.Random.Range(1, modelCount);
            string modelName = modelIndex.ToString("0000");

            yield return new WaitUntil(() => ModelLoad(modelName)); // モデルのロードが完了するまで待機

            // ランダムな光源とカメラの設定
            float rotation = UnityEngine.Random.Range(-360f, 360f);
            modelCamera.transform.RotateAround(modelPosition, Vector3.up, rotation);
            directionalLight.transform.eulerAngles = new Vector3(rotation, 90f, 90f);

            // 撮影
            CameraSavePng(modelCamera.GetComponent<UnityEngine.Camera>(), "target", currentCount.ToString("00000"));
            CameraSavePng(lightCamera.GetComponent<UnityEngine.Camera>(), "light", currentCount.ToString("00000"));

            // モデルを破棄
            Destroy(currentModel);
            currentModel = null;

            // 休憩時間
            yield return new WaitForSeconds(captureInterval);

            // メモリの解放
            ReleaseMemory();

            if (currentCount >= maxCount)
            {
                UnityEditor.EditorApplication.isPlaying = false;
            }
        }

        isCapturing = false;
    }

    private bool ModelLoad(string modelName)
    {
        GameObject modelPrefab = Resources.Load<GameObject>(modelName);

        if (modelPrefab != null)
        {
            currentModel = Instantiate(modelPrefab);

            foreach (Transform childTransform in currentModel.transform)
            {
                if (childTransform.gameObject.name != "default.default")
                {
                    var child = childTransform.gameObject;
                    modelPosition = child.GetComponent<Renderer>().bounds.center;
                    float x = -modelPosition.x;
                    float y = -modelPosition.y;
                    float z = -modelPosition.z;
                    currentModel.transform.Translate(x, y, z);
                }
                if (currentModel.transform.childCount == 1)
                {
                    var child = childTransform.gameObject;
                    modelPosition = child.GetComponent<Renderer>().bounds.center;
                    float x = -modelPosition.x;
                    float y = -modelPosition.y;
                    float z = -modelPosition.z;
                    currentModel.transform.Translate(x, y, z);
                }
            }

            return true;
        }
        else
        {
            StartCoroutine(RetryModelLoad(modelName)); // リトライする
        }

        return false;
    }

    private IEnumerator RetryModelLoad(string modelName)
    {
        // リトライまでの待機時間
        yield return new WaitForSeconds(1f);

        // リトライ
        bool success = ModelLoad(modelName);
        if (!success)
        {
            Debug.LogError("モデルの読み込みに失敗しました: " + modelName);
        }
    }

    private void CameraSavePng(UnityEngine.Camera camera, string folderName, string fileName)
    {
        RenderTexture rt = new RenderTexture(captureWidth, captureHeight, 24);
        RenderTexture prev = camera.targetTexture;
        camera.targetTexture = rt;
        camera.Render();
        camera.targetTexture = prev;
        RenderTexture.active = rt;

        Texture2D capture = new Texture2D(captureWidth, captureHeight, TextureFormat.ARGB32, false);
        capture.ReadPixels(new Rect(0, 0, capture.width, capture.height), 0, 0);
        capture.Apply();
        byte[] bytes = capture.EncodeToPNG();
        string path = Path.Combine(outpath, folderName, fileName + ".png");
        if (!string.IsNullOrEmpty(path))
        {
            File.WriteAllBytes(path, bytes);
        }

        RenderTexture.active = null;
        camera.targetTexture = null;
        Destroy(rt);
    }

    private void ReleaseMemory()
    {
        Resources.UnloadUnusedAssets();
        System.GC.Collect();
    }
}