using System;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEditor;
using UnityEngine.Networking;

using MintNFTSDK.SingleMint;

/// <summary>
/// Tests upload functionality
/// </summary>
public class TestUpload : MonoBehaviour
{
    [Header("Metadata")]
    [Space(5f)]
    // Define name and description for your metadata
    [Tooltip("Desired name for the token")]
    public string nftName;
    [Tooltip("Desired description for the token")]
    public string nftDescription;

    // API Key from the platform
    private string APIKEY = "ad072490-b517-4010-a052-377b88fa7188";
    // Holds form data for file upload
    private static List<IMultipartFormSection> formData = new List<IMultipartFormSection>(); 

    // Upload preview and asset from Filesystem | Enable if using filesystem
    private string[] assetTypes =
        {"Image","JPG,PNG,JPEG,SVG",
        "Gif","GIF",
        "Video","MP4,WEBM",
        "Music","MP3,WAV",
        "3D Model","GLB,GLTF"};
    private string[] previewTypes =
        {"Image","JPG,PNG,JPEG,SVG"};

    // Store paths for preview and asset if uploading from Filesystem | Enable if using filesystem
    private string assetPath;
    private string previewPath;

    // Web request initialization
    private UnityWebRequest loadPreview;
    private UnityWebRequest loadAsset;

    // Metadata Json object
    private MetadataData data = new MetadataData();
    private string metadataJson;

    private void Start()
    {
        data.name = nftName;
        data.description = nftDescription;

        metadataJson = JsonUtility.ToJson(data);
        formData.Add(new MultipartFormDataSection("metadata", Encoding.ASCII.GetBytes(metadataJson)));
        Debug.Log(metadataJson);
    }

    // Gets the file explorer to get preview file path
    public void SelectPreview()
    {
        previewPath = EditorUtility.OpenFilePanelWithFilters("Choose Preview Image", "", previewTypes);
        Debug.Log(previewPath);
        StartCoroutine(GetPreviewFiles());
    }

    private IEnumerator GetPreviewFiles()
    {
        loadPreview = UnityWebRequest.Get(previewPath);
        yield return loadPreview.SendWebRequest();
        formData.Add(new MultipartFormFileSection("image", loadPreview.downloadHandler.data, Path.GetFileName(previewPath), "image/*"));
    }

    // Gets the file explorer to get asset file path
    public void SelectAsset()
    {
        assetPath = EditorUtility.OpenFilePanelWithFilters("Choose Asset", "", assetTypes);
        Debug.Log(assetPath);
        StartCoroutine(GetAssetFile());
    }

    // Load the asset onto the form field
    private IEnumerator GetAssetFile()
    {
        loadAsset = UnityWebRequest.Get(assetPath);
        yield return loadAsset.SendWebRequest();
        formData.Add(new MultipartFormFileSection("asset", loadAsset.downloadHandler.data, Path.GetFileName(assetPath), "*/*"));
    }

    // Upload the files (async operation)
    public async void UploadFile()
    {
        string result = await MintNFT.Upload(APIKEY, formData);
        Debug.Log(result);
    }
}

[Serializable]
public class MetadataData
{
    public string name;
    public string description;
}
