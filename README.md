# MintNFT Unity SDK

Minting your NFTs over the web and plugging APIs to mint it through your own DApps are all cool. What about being able to mint in-game NFTs directly from Unity? We have got you covered! Introducing our Unity SDK

## Implementation Steps

1. Download the contents from the repo
2. Open the project on Unity, preferably Unity 2021.3
3. There are 2 scene files namely "TestUpload" and "TestMint" which you can use to test
4. The "TestUpload.cs" and "TestMint.cs" scripts can help you get started to write your own scripts
5. Main 2 methods in the "MintNFT.cs" class are "Upload" and "Mint"

### Upload
```
string result = MintNFT.Upload(string apiKey, List<IMultipartFormSection> formData);
```
### Mint
```
string result = MintNFT.Mint(string apiKey, string stringifiedJsonBody);
```

To get the API keys please go to https://mintnft.today/create/api

To check the formats of the formdata, json body, etc. please check our documentation here - https://docs.mintnft.today/
