using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TriLib の参照
using TriLibCore;

public class ModelLoader : MonoBehaviour
{
    public Camera Camera;
    public GameObject loadedGameObject;
    public Vector3 playerPos;

    public void FileLoad(string filepath)
    {
        if (loadedGameObject != null)
        {
            Destroy(loadedGameObject.gameObject);
        }
        // ロードの設定情報を作成する（今回はデフォルト設定）
        AssetLoaderOptions assetLoaderOptions = AssetLoader.CreateDefaultLoaderOptions();
        // ファイルシステムからのロードを実行する
        AssetLoader.LoadModelFromFile(filepath, OnLoad, OnMaterialsLoad, OnProgress, OnError, null, assetLoaderOptions);
    }

    /// <summary>
    /// <summary>
    /// モデルの読み込みの進行状況が変化したときに呼び出されるイベント
    /// </summary>
    /// <param name="assetLoaderContext"></param>
    /// <param name="progress"></param>
    private void OnProgress(AssetLoaderContext assetLoaderContext, float progress)
    {
    }

    /// <summary>
    /// エラー発生時に呼び出されるイベント
    /// </summary>
    /// <param name="contextualizedError"></param>
    private void OnError(IContextualizedError contextualizedError)
    {
    }

    /// <summary>
    /// 全ての GameObject が読み込まれたときに呼び出されるイベント
    /// </summary>
    /// <param name="assetLoaderContext"></param>
    private void OnLoad(AssetLoaderContext assetLoaderContext)
    {
        // ロードされたGameObjectを取得する
        GameObject loadedGameObject = assetLoaderContext.RootGameObject;

        // この時点ではマテリアルとテクスチャの読み込みが完了していない可能性があるため
        // オブジェクトを非表示にしておく
        loadedGameObject.SetActive(false);
    }

    /// <summary>
    /// マテリアルとテクスチャを含む全ての読み込みが完了したときに呼び出されるイベント
    /// </summary>
    /// <param name="assetLoaderContext"></param>
    private void OnMaterialsLoad(AssetLoaderContext assetLoaderContext)
    {
        // ロードされたGameObjectを取得する
        loadedGameObject = assetLoaderContext.RootGameObject;

        // この時点で全てのリソースの読み込みが完了しているのでオブジェクトを表示する
        loadedGameObject.SetActive(true);

        //// こちらのモデルを読み込む際、何故かモデルの座標がズレたので修正　https://www.cgtrader.com/3d-model-collections/1400-people-crowds
        foreach (Transform childTransform in loadedGameObject.transform)
        {
            Debug.Log(childTransform.gameObject.name);
            if (childTransform.gameObject.name != "default.default")
            {
                var child = childTransform.gameObject;
                playerPos = child.GetComponent<Renderer>().bounds.center;
                float x = -playerPos.x;
                float y = -playerPos.y;
                float z = -playerPos.z;
                loadedGameObject.transform.Translate(x, y, z);
            }
            if (loadedGameObject.transform.childCount == 1)
            {
                var child = childTransform.gameObject;
                playerPos = child.GetComponent<Renderer>().bounds.center;
                float x = -playerPos.x;
                float y = -playerPos.y;
                float z = -playerPos.z;
                loadedGameObject.transform.Translate(x, y, z);
            }
        }
    }
}