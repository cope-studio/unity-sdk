using System.Collections.Generic;
using System.Threading.Tasks;

using UnityEngine.Networking;

namespace MintNFTSDK.SingleMint
{
    /// <summary>
    /// The MintNFT master class
    /// API Documentation - https://docs.0xmint.io/
    /// </summary>
    public class MintNFT
    {
        private static string apiEndPoint = "https://api.0xmint.io"; // API Endpoint

        /// <summary>
        /// Uploads the asset and preview file to IPFS
        /// </summary>
        /// <param name="apiKey"> Get you API key here - https://app.0xmint.io/create/api </param>
        /// <param name="formData"> The form data including stringified metadata json, asset file and preview image </param>
        public static async Task<string> Upload(string apiKey, List<IMultipartFormSection> formData)
        {
            string result = await UploadFile(apiKey, formData);
            return result;
        }

        private static async Task<string> UploadFile(string apiKey, List<IMultipartFormSection> formData)
        {
            UnityWebRequest req = UnityWebRequest.Post($"{apiEndPoint}/v1/upload/single", formData);

            req.SetRequestHeader("x-api-key", apiKey);
            
            req.SendWebRequest();
            while(!req.isDone)
            {
                await Task.Yield();
            }

            if (req.result == UnityWebRequest.Result.Success)
            {
                formData.Clear();
                string ipfsHash = req.downloadHandler.text;
                return ipfsHash;
            }
            else
            {
                formData.Clear();
                return $"ERROR in Upload! \n STATUS CODE - {req.responseCode} \n ERROR - {req.error}";
            }
        }

        /// <summary>
        /// Mints the NFT using data from IPFS
        /// </summary>
        /// <param name="apiKey"> Get you API key here - https://app.0xmint.today/create/api </param>
        /// <param name="body"> The request body as a stringified Json! Refer API docs for format </param>
        /// <returns></returns>
        public static async Task<string> Mint(string apiKey, string jwt, string body)
        {
            string result = await MintFile(apiKey, jwt, body);
            return result;
        }

        private static async Task<string> MintFile(string apiKey, string jwt, string body)
        {
            var uwr = new UnityWebRequest($"{apiEndPoint}/v2/collection/mint", "POST");
            byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(body);
            uwr.uploadHandler = new UploadHandlerRaw(jsonToSend);
            uwr.downloadHandler = new DownloadHandlerBuffer();

            uwr.SetRequestHeader("x-api-key", apiKey);
            uwr.SetRequestHeader("x-auth-token", jwt);
            uwr.SetRequestHeader("Content-Type", "application/json");
            uwr.SendWebRequest();

            while(!uwr.isDone)
            {
                await Task.Yield();
            }

            if (uwr.result == UnityWebRequest.Result.Success)
            {
                string res = uwr.downloadHandler.text;
                return res;
            }
            else
            {
                return $"ERROR in Minting! \n STATUS CODE - {uwr.responseCode} \n ERROR - {uwr.error}";
            }
        }

        /// <summary>
        /// Refreshes the JWT token using our refresh token
        /// </summary>
        /// <param name="apiKey"> Get you API key here - https://app.0xmint.today/create/api </param>
        /// <param name="body"> The request body as a stringified Json! Refer API docs for format </param>
        /// <returns></returns>
        public static async Task<string> RefreshJWT(string apiKey, string refreshToken, string body)
        {
            string result = await RefreshKey(apiKey, refreshToken, body);
            return result;
        }

        private static async Task<string> RefreshKey(string apiKey, string refreshToken, string body)
        {
            var uwr = new UnityWebRequest($"{apiEndPoint}/v2/collection/mint", "POST");
            byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(body);
            uwr.uploadHandler = new UploadHandlerRaw(jsonToSend);
            uwr.downloadHandler = new DownloadHandlerBuffer();

            uwr.SetRequestHeader("x-api-key", apiKey);
            uwr.SetRequestHeader("x-refresh-token", refreshToken);
            uwr.SetRequestHeader("Content-Type", "application/json");
            uwr.SendWebRequest();

            while (!uwr.isDone)
            {
                await Task.Yield();
            }

            if (uwr.result == UnityWebRequest.Result.Success)
            {
                string res = uwr.downloadHandler.text;
                return res;
            }
            else
            {
                return $"ERROR in refreshing JWT! \n STATUS CODE - {uwr.responseCode} \n ERROR - {uwr.error}";
            }
        }
    }
}
