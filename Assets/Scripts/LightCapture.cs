using UnityEngine;
using System.IO;
using System.Collections;
using System.Linq;
using UnityEngine.UI;

public class LightCapture : MonoBehaviour
{
    public GameObject modelCamera;
    public GameObject directionalLight;
    public GameObject plane;
    public Material normalMaterial;
    public Material standardMaterial; // Changed material
    public Text maxText;
    public Text countText;
    public string outpath;
    public int captureWidth;
    public int captureHeight;
    public float captureInterval = 0.5f;
    public bool randomCapture = true;
    public int maxCount;

    private string[] modelPaths;
    private bool isCapturing;
    private int currentCount;
    private int modelCount;
    private GameObject currentModel;
    private Vector3 modelPosition;
    public bool isMiddleLoad;
    public int middleLoadCount;

    private void Start()
    {
        isCapturing = false;
        currentCount = 0;

        string[] ignoreFolders = { };

        modelPaths = Directory.GetFiles("Assets/Resources", "*.obj", SearchOption.AllDirectories)
            .Where(path => !ignoreFolders.Any(folder => path.Contains(folder)))
            .ToArray();

        modelCount = modelPaths.Length;

        if (randomCapture)
        {
            maxText.text = maxCount.ToString();
            CreateDirectionFolders("Front", "Left", "Right", "Back", "Normal");
        }
        else
        {
            maxText.text = modelCount.ToString();
            CreateModelFolders("Front", "Normal");
        }

        countText.text = "0";

        StartCapture();
    }

    private void StartCapture()
    {
        if (!isCapturing)
        {
            if (randomCapture)
            {
                StartCoroutine(RandomCapture());
            }
            else
            {
                StartCoroutine(OnceCapture());
            }
        }
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
            countText.text = currentCount.ToString("00000");

            int modelIndex = Random.Range(0, modelPaths.Length);
            string modelPath = modelPaths[modelIndex];
            string modelName = Path.GetFileNameWithoutExtension(modelPath);
            string folderName = Path.GetFileName(Path.GetDirectoryName(modelPath));

            Debug.Log(modelName);

            yield return StartCoroutine(ModelLoad(folderName + "/" + modelName));

            ChangeMaterials(plane, standardMaterial);
            directionalLight.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
            CameraSavePng(modelCamera.GetComponent<Camera>(), "Front", currentCount.ToString("00000"));

            directionalLight.transform.rotation = Quaternion.Euler(0f, 90f, 0f);
            CameraSavePng(modelCamera.GetComponent<Camera>(), "Left", currentCount.ToString("00000"));

            directionalLight.transform.rotation = Quaternion.Euler(0f, -90f, 0f);
            CameraSavePng(modelCamera.GetComponent<Camera>(), "Right", currentCount.ToString("00000"));

            directionalLight.transform.rotation = Quaternion.Euler(0f, 180f, 0f);
            CameraSavePng(modelCamera.GetComponent<Camera>(), "Back", currentCount.ToString("00000"));

            ChangeMaterials(plane, normalMaterial);
            ChangeMaterials(currentModel, normalMaterial);
            CameraSavePng(modelCamera.GetComponent<Camera>(), "Normal", currentCount.ToString("00000"));

            Destroy(currentModel);
            currentModel = null;

            yield return new WaitForSeconds(captureInterval);

            ReleaseMemory();

            if (currentCount >= maxCount)
            {
                UnityEditor.EditorApplication.isPlaying = false;
            }
        }

        isCapturing = false;
    }

    private IEnumerator OnceCapture()
    {
        isCapturing = true;

        if (modelPaths.Length == 0)
        {
            Debug.LogWarning("No models found.");
            yield break;
        }

        for (int i = 0; i < modelPaths.Length; i++)
        {
            string modelPath = modelPaths[i];
            string modelName = Path.GetFileNameWithoutExtension(modelPath);
            string folderName = Path.GetFileName(Path.GetDirectoryName(modelPath));

            countText.text = i.ToString();

            yield return StartCoroutine(ModelLoad(folderName + "/" + modelName));
            ChangeMaterials(plane, standardMaterial);
            CameraSavePng(modelCamera.GetComponent<Camera>(), folderName + "/Front", modelName);

            ChangeMaterials(plane, normalMaterial);
            ChangeMaterials(currentModel, normalMaterial);
            CameraSavePng(modelCamera.GetComponent<Camera>(), folderName + "/Normal", modelName);

            Destroy(currentModel);
            currentModel = null;

            yield return new WaitForSeconds(captureInterval);

            ReleaseMemory();

            if (i >= modelPaths.Length - 1)
            {
                UnityEditor.EditorApplication.isPlaying = false;
            }
        }

        isCapturing = false;
    }

    private IEnumerator ModelLoad(string modelName)
    {
        GameObject modelPrefab = Resources.Load<GameObject>(modelName);

        if (modelPrefab != null)
        {
            currentModel = Instantiate(modelPrefab);

            if (randomCapture)
            {
                float rotation = Random.Range(-360f, 360f);
                currentModel.transform.rotation = Quaternion.identity;
                currentModel.transform.RotateAround(modelPosition, Vector3.up, rotation);
            }
            else
            {
                currentModel.transform.rotation = Quaternion.identity;
                currentModel.transform.RotateAround(modelPosition, Vector3.up, 180);
            }

            yield return null;
        }
        else
        {
            yield return StartCoroutine(RetryModelLoad(modelName));
        }
    }

    private IEnumerator RetryModelLoad(string modelName)
    {
        yield return new WaitForSeconds(1f);

        yield return ModelLoad(modelName);
    }

    private void CameraSavePng(Camera camera, string direction, string modelName)
    {
        modelPosition = currentModel.transform.position;

        RenderTexture rt = new RenderTexture(captureWidth, captureHeight, 24);
        camera.targetTexture = rt;

        Texture2D screenShot = new Texture2D(captureWidth, captureHeight, TextureFormat.RGB24, false);
        camera.Render();
        RenderTexture.active = rt;
        screenShot.ReadPixels(new Rect(0, 0, captureWidth, captureHeight), 0, 0);
        camera.targetTexture = null;
        RenderTexture.active = null;
        Destroy(rt);

        byte[] bytes = screenShot.EncodeToPNG();
        Destroy(screenShot);

        string saveFolder = outpath + "/" + direction;
        if (!Directory.Exists(saveFolder))
        {
            Directory.CreateDirectory(saveFolder);
        }

        File.WriteAllBytes(saveFolder + "/" + modelName + ".png", bytes);
    }

    private void CreateModelFolders(params string[] folderNames)
    {
        foreach (string modelPath in modelPaths)
        {
            string folderName = Path.GetFileName(Path.GetDirectoryName(modelPath));
            string saveFolder = outpath + "/" + folderName;
            if (!Directory.Exists(saveFolder))
            {
                Directory.CreateDirectory(saveFolder);

                foreach (string folderName2 in folderNames)
                {
                    string subFolder = Path.Combine(saveFolder, folderName2);
                    Directory.CreateDirectory(subFolder);
                }
            }
        }
    }

    private void CreateDirectionFolders(params string[] directions)
    {
        foreach (string direction in directions)
        {
            string saveFolder = outpath + "/" + direction;
            if (!Directory.Exists(saveFolder))
            {
                Directory.CreateDirectory(saveFolder);
            }
        }
    }

    public void ChangeMaterials(GameObject gameObject, Material material)
    {
        Renderer[] renderers = gameObject.GetComponentsInChildren<Renderer>();

        foreach (Renderer renderer in renderers)
        {
            Material[] materials = renderer.materials;

            for (int i = 0; i < materials.Length; i++)
            {
                materials[i] = material;
            }

            renderer.materials = materials;
        }
    }

    private void ReleaseMemory()
    {
        System.GC.Collect();
        Resources.UnloadUnusedAssets();
    }
}
