using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using System.IO;
using Siccity.GLTFUtility;
using TMPro;
using UnityEngine.Rendering;
using UnityEditor.Experimental.AssetImporters;

public class ARController : MonoBehaviour
{
    [SerializeField] ARTrackedImageManager m_TrackedImageManager;
    public Texture2D tex2D;
    private string trackImgLink = "https://user74522.clients-cdnnow.ru/static/uploads/mrk6440mark.png";
    private string modelLink = "https://user74522.clients-cdnnow.ru/static/uploads/mrk6564obj.glb";
    private string filePath;
    [SerializeField] private GameObject trackedPrefab;

    [SerializeField] private TextMeshProUGUI text;

    private void Start()
    {
        StartCoroutine(DownloadFile(modelLink, "myModel"));
        StartCoroutine(CheckInternetAndDownload(trackImgLink));
    }

    void AddImage(Texture2D imageToAdd)
    {
        if (!(ARSession.state == ARSessionState.SessionInitializing || ARSession.state == ARSessionState.SessionTracking))
            return; // Session state is invalid

        if (m_TrackedImageManager.referenceLibrary is MutableRuntimeReferenceImageLibrary mutableLibrary)
        {
            mutableLibrary.ScheduleAddImageWithValidationJob(
                imageToAdd,
                "my new image",
                0.5f);
        }
    }

    IEnumerator CheckInternetAndDownload(string imageUrl)
    {
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            text.text = "No internet connection";
            text.color = Color.red;
            yield return null;
        }

        using (UnityWebRequest www = UnityWebRequestTexture.GetTexture(imageUrl))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                // Скачивание успешно, конвертируем данные в текстуру
                tex2D = DownloadHandlerTexture.GetContent(www);

                // Вызываем функцию добавления изображения
                AddImage(tex2D);
                text.text = "Everything is working";
            }
            else
            {
                text.text = "Error downloading image: " + www.error + ". Try check internet connection";
            }
        }
    }
    
    IEnumerator DownloadFile(string url, string fileName)
    {
        filePath = Path.Combine(Application.persistentDataPath, fileName);

        using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
        {
            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.Success)
            {
                // Сохраняем скачанный файл на диск
                File.WriteAllBytes(filePath, webRequest.downloadHandler.data);
                text.text = "File downloaded and saved to: " + filePath;
                
                // Теперь загружаем GLB-модель
                AddModel();
            }
            else
            {
                text.text = "Error downloading file: " + webRequest.error;
            }
        }
    }
    
    public void AddModel()
    {
        GameObject model = Importer.LoadFromFile(filePath, Format.GLB);
        GameObject parent = Instantiate(new GameObject());
        
        model.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
        model.transform.position = new Vector3(-100, -100, -100);
        
        parent.transform.position = model.transform.position;
        model.transform.SetParent(parent.transform);
        
        model.AddComponent<TouchInputHandler>();
        trackedPrefab = parent;
        m_TrackedImageManager.trackedImagePrefab = trackedPrefab;
    }
}