using System;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;
using WalletConnectSharp.Unity;

/// <summary>
/// Demo script for uploading NFT to IPFS and Minting it in-game
/// </summary>
public class SingleMint : MonoBehaviour
{
    // For sending Metadata and wallet details
    public DataRoot root = new DataRoot();
    public UploadRoot uploadRoot = new UploadRoot();

    // For getting IPFS hash
    public IPFSRoot ipfsRoot = new IPFSRoot();

    // Unity Events
    public UnityEvent postUpload;
    public UnityEvent uploadFail;
    public UnityEvent postMint;
    public UnityEvent mintFail;

    // Upload preview and asset from Filesystem | Enable if using filesystem
    //private string[] assetTypes = 
    //    {"Image","JPG,PNG,JPEG,SVG", 
    //    "Gif","GIF", 
    //    "Video","MP4,WEBM", 
    //    "Music","MP3,WAV", 
    //    "3D Model","GLB,GLTF"}; 
    //private string[] previewTypes = 
    //    {"Image","JPG,PNG,JPEG,SVG"};

    private string apiEndPoint = "https://mintnfttoday.herokuapp.com"; // API Endpoint
    private string apiKey = "090c413d-ef55-4755-93c8-6b99151fc262"; // API key from website

    // Store paths for preview and asset if uploading from Filesystem | Enable if using filesystem
    //private string assetPath;
    //private string previewPath;

    private string mintData; // Stores stringified Json for Mint request body

    private List<IMultipartFormSection> formData; // Holds form data for file upload

    // Web request initialization
    private UnityWebRequest loadPreview;
    private UnityWebRequest loadAsset;

    private void Start()
    {
        formData = new List<IMultipartFormSection>(); // Initialize form data
    }

    #region Choose file from System | Enable if using filesystem
    //public void SelectAsset()
    //{
    //    assetPath = EditorUtility.OpenFilePanelWithFilters("Choose Asset", "", assetTypes);
    //    Debug.Log(assetPath);
    //    StartCoroutine(GetAssetFile());
    //}

    //public void SelectPreview()
    //{
    //    previewPath = EditorUtility.OpenFilePanelWithFilters("Choose Preview Image", "", previewTypes);
    //    Debug.Log(previewPath);
    //    StartCoroutine(GetPreviewFiles());
    //}
    #endregion

    // Starts the Upload to IPFS against the connected Wallet and the metadata
    public void UploadToIPFS()
    {
        uploadRoot.wallet = WalletConnect.ActiveSession.Accounts[0];
        uploadRoot.metadata.name = "WIN Badge";
        uploadRoot.metadata.description = "Badge for winning this FPS level"; 

        string data = JsonUtility.ToJson(uploadRoot);

        formData.Add(new MultipartFormDataSection("metadata", Encoding.ASCII.GetBytes(data)));

        StartCoroutine(GetPreviewFiles());
    }

    // Starts the Minting process by preparing the request body
    public void Mint()
    {
        root.wallet = WalletConnect.ActiveSession.Accounts[0];
        root.type = "ERC721";
        root.amount = "0";
        root.network = "mumbai";
        root.tokenUri = ipfsRoot.data.url;

        mintData = JsonUtility.ToJson(root);

        StartCoroutine(StartMint());
    }

    // Get all NFTs minted by this wallet through this platform
    public void GetNFT()
    {
        StartCoroutine(GetMintedNFT());
    }

    // Load the preview image onto the form field
    private IEnumerator GetPreviewFiles()
    {
        loadPreview = UnityWebRequest.Get($"{Application.streamingAssetsPath}/WinBadge.jpg");
        yield return loadPreview.SendWebRequest();
        formData.Add(new MultipartFormFileSection("image", loadPreview.downloadHandler.data, Path.GetFileName($"{Application.streamingAssetsPath}/WinBadge.jpg"), "image/png"));

        StartCoroutine(GetAssetFile());
    }
    
    // Load the asset onto the form field
    private IEnumerator GetAssetFile()
    {
        loadAsset = UnityWebRequest.Get($"{Application.streamingAssetsPath}/WinBadge.jpg");
        yield return loadAsset.SendWebRequest();
        formData.Add(new MultipartFormFileSection("asset", loadAsset.downloadHandler.data, Path.GetFileName($"{Application.streamingAssetsPath}/WinBadge.jpg"), "*/*"));

        StartCoroutine(StartUpload());
    }

    // Upload request to endpoint - IPFS Upload
    private IEnumerator StartUpload()
    {
        UnityWebRequest req = UnityWebRequest.Post($"{apiEndPoint}/v1/upload/single", formData);

        req.SetRequestHeader("x-api-key", apiKey);

        yield return req.SendWebRequest();

        if (req.result == UnityWebRequest.Result.Success)
        {
            Debug.Log(req.responseCode);
            Debug.Log(req.downloadHandler.text);

            string ipfsHash = req.downloadHandler.text;

            ipfsRoot = JsonUtility.FromJson<IPFSRoot>(ipfsHash);

            postUpload.Invoke();

            Mint();
        }
        else
        {
            Debug.LogError("Error in Uploading!");
            Debug.Log(req.error);
            Debug.Log(req.responseCode);

            uploadFail.Invoke();

            formData.Clear();
        }
    }

    // Mint request to endpoint - Post upload success
    private IEnumerator StartMint()
    {
        var uwr = new UnityWebRequest($"{apiEndPoint}/v1/mint/single", "POST");
        byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(mintData);
        uwr.uploadHandler = (UploadHandler)new UploadHandlerRaw(jsonToSend);
        uwr.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();

        uwr.SetRequestHeader("x-api-key", apiKey);
        uwr.SetRequestHeader("Content-Type", "application/json");

        yield return uwr.SendWebRequest();

        if (uwr.result == UnityWebRequest.Result.Success)
        {
            Debug.Log(uwr.responseCode);
            Debug.Log(uwr.downloadHandler.text);

            postMint.Invoke();
        }
        else
        {
            Debug.LogError("Error in Minting!");
            Debug.Log(uwr.error);
            Debug.Log(uwr.responseCode);

            mintFail.Invoke();

            formData.Clear();
        }
    }

    // Get NFT request to endpoint - Post minting atleast one NFT from this platform
    private IEnumerator GetMintedNFT()
    {
        UnityWebRequest req = UnityWebRequest.Get($"{apiEndPoint}/v1/nft/{WalletConnect.ActiveSession.Accounts[0]}");
        req.SetRequestHeader("x-api-key", apiKey);

        yield return req.SendWebRequest();

        if (req.result == UnityWebRequest.Result.Success)
        {
            Debug.Log(req.responseCode);
            Debug.Log(req.downloadHandler.text);
        }
        else
        {
            Debug.LogError("Error in getting NFTs!");
            Debug.Log(req.error);
            Debug.Log(req.responseCode);
        }
    }
}

// All serializable classes which doubles as Json Objects

[Serializable]
public class IPFSData
{
    public string ipnft;
    public string url;
}

[Serializable]
public class IPFSRoot
{
    public IPFSData data;
}

[Serializable]
public class Metadata
{
    public string name;
    public string description;
}

[Serializable]
public class UploadRoot
{
    public string wallet;
    public Metadata metadata;
}

[Serializable]
public class DataRoot
{
    public string wallet;
    public string type;
    public string amount;
    public string network;
    public string tokenUri;
}
