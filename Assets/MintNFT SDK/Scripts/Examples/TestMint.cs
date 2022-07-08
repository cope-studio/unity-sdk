using System;

using UnityEngine;
using MintNFTSDK.SingleMint;

public class TestMint : MonoBehaviour
{
    [Header("Request Body Fields")]
    [Space(5f)]
    // Define fields for the request body
    [Tooltip("Wallet address used for getting the API Key")]
    public string wallet;
    [Tooltip("Type of NFT - ERC721 or ERC1155")]
    public string type;
    [Tooltip("'soulbound' if you want it to be ECR721-Soulbound. Leave empty for normal ERC721 and ERC1155")]
    public string tokenCategory;
    [Tooltip("Total supply for ERC1155. Default is 1 for ERC721")]
    public int amount = 1;
    [Tooltip("'mainnet' - Polygon Mainnet")]
    public string network;
    [Tooltip("IPFS Uri that you got as a respose while uploading")]
    public string tokenUri;

    // API Key from the platform
    private string APIKEY = "ad072490-b517-4010-a052-377b88fa7188";

    // Mint request body object
    private MintBody body = new MintBody();
    private string bodyJson;

    // Start is called before the first frame update
    void Start()
    {
        body.wallet = wallet;
        body.type = type;
        body.tokenCategory = tokenCategory; // ['soulbound' if you want it to be ECR721-Soulbound. Leave empty for normal ERC721 and ERC1155]
        body.amount = amount;
        body.network = network;
        body.tokenUri = tokenUri;

        bodyJson = JsonUtility.ToJson(body);
        Debug.Log(bodyJson);
    }

    // Mint the NFT (async operation)
    public async void MintFile()
    {
        string result = await MintNFT.Mint(APIKEY, bodyJson);
        Debug.Log(result);
    }
}

[Serializable]
public class MintBody
{
    public string wallet;
    public string type;
    public string tokenCategory;
    public int amount;
    public string network;
    public string tokenUri;
}

