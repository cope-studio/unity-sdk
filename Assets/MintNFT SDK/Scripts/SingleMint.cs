using System;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

using WalletConnectSharp.Unity;

using MintNFTSDK.SingleMint;


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

    private string apiKey = ""; // API key from website
    private string jwt = ""; // API key from website

    private string mintData; // Stores stringified Json for Mint request body

    private List<IMultipartFormSection> formData; // Holds form data for file upload

    // Web request initialization
    private UnityWebRequest loadPreview;
    private UnityWebRequest loadAsset;

    private void Start()
    {
        formData = new List<IMultipartFormSection>(); // Initialize form data
    }

    // Starts the Upload to IPFS against the connected Wallet and the metadata
    public void UploadToIPFS()
    {
        uploadRoot.name = "WIN Badge";
        uploadRoot.description = "Badge for winning this FPS level"; 

        string data = JsonUtility.ToJson(uploadRoot);

        formData.Add(new MultipartFormDataSection("metadata", Encoding.ASCII.GetBytes(data)));

        StartCoroutine(GetPreviewFiles());
    }

    // Starts the Minting process by preparing the request body
    public void Mint()
    {
        root.wallet = WalletConnect.ActiveSession.Accounts[0];
        root.type = "ERC721";
        root.tokenCategory = null;
        root.amount = 1;
        root.network = "mainnet";
        root.tokenUri = ipfsRoot.data.url;

        mintData = JsonUtility.ToJson(root);

        StartMint();
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

        StartUpload();
    }

    // Upload request to endpoint - IPFS Upload
    private async void StartUpload()
    {
        string result = await MintNFT.Upload(apiKey, formData);

        if (result.Contains("ERROR"))
        {
            Debug.LogError("Error in Uploading!");
            Debug.Log(result);
            uploadFail.Invoke();

            formData.Clear();
        }
        else
        {
            Debug.Log(result);

            ipfsRoot = JsonUtility.FromJson<IPFSRoot>(result);

            postUpload.Invoke();

            Mint();
        }
    }

    // Mint request to endpoint - Post upload success
    private async void StartMint()
    {
        string result = await MintNFT.Mint(apiKey, jwt, mintData);

        if (result.Contains("ERROR"))
        {
            Debug.LogError("Error in Minting!");
            Debug.Log(result);

            mintFail.Invoke();

            formData.Clear();
        }
        else
        {
            Debug.Log(result);

            postMint.Invoke();
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
public class UploadRoot
{
    public string name;
    public string description;
}

[Serializable]
public class DataRoot
{
    public string wallet;
    public string type;
    public string tokenCategory;
    public int amount;
    public string network;
    public string tokenUri;
}
