using Azure;
using BncPayments.Models;
using BncPayments.Utils;
using ClassLibrary.BncModels;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Reflection;
using System.Security.Cryptography.Xml;
using System.Text;

namespace BncPayments.Services
{
    public interface IBncServices
    {
        Task<HttpResponseMessage> Banks();
        Task<HttpResponseMessage> BcvRates();
        Task<HttpResponseMessage> Current(Current model);
        Task<string> GetExternalDataAsync();
        Task<HttpResponseMessage> History(History model);
        Task<HttpResponseMessage> HistoryByDate(HistoryByDate model);
        Task<HttpResponseMessage> LogOn();
        Task<ModelExample> PostExternalData(ModelExample model);
        Task<HttpResponseMessage> ReverseC2P(ReverseC2P model);
        Task<HttpResponseMessage> ReverseC2PALT(ReverseC2Palt model);
        Task<HttpResponseMessage> Send(Send model);
        Task<HttpResponseMessage> SendC2P(SendC2P model);
        Task<HttpResponseMessage> SendP2P(SendP2P model);
        Task<HttpResponseMessage> TransactionPos(TransactionsPos model);
        Task<HttpResponseMessage> Validate(Validate model);
        Task<HttpResponseMessage> ValidateExistence(ValidateExistence model);
        Task<HttpResponseMessage> ValidateP2P(ValidateP2P model);
    }
    public class BncServices : IBncServices
    {
        private readonly HttpClient _httpClient;
        private readonly IEncryptionServices _encryptServices;
        private readonly IHashService _hashServices;
        private readonly WorkingKeyServices _workingKeyServices;
        private readonly string _workingKeyTests;
        private readonly IRequestServices _requestServices;
        private readonly IResponseServices _responseServices;
        private readonly ApiBncSettings _apiBncSettings;
        public BncServices(IEncryptionServices encryptService,
            IHashService hashServices,
            ApiBncSettings apiBncSettings,
            WorkingKeyServices workingKeyServices,
            IRequestServices requestServices,
            IResponseServices responseServices)
        {
            _apiBncSettings = apiBncSettings;
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri(_apiBncSettings.BaseAddress);
            _encryptServices = encryptService;
            _hashServices = hashServices;
            _workingKeyServices = workingKeyServices;
            _workingKeyTests = "dd8321dc18579eed46015ab3fa518a88";
            _requestServices = requestServices;
            _responseServices = responseServices;
        }

        /// <summary>
        /// Metodo para pruebas basicas de configuracion de la API
        /// consumiendo JSONplaceholder
        /// </summary>
        /// <returns></returns>
        public async Task<string> GetExternalDataAsync()
        {
            var response = await _httpClient.GetAsync("character/?page=10");
            response.EnsureSuccessStatusCode();

            var data = await response.Content.ReadAsStringAsync();
            return data;
        }

        /// <summary>
        /// Metodo para pruebas basicas de configuracion de la API
        /// consumiendo JSONplaceholder
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<ModelExample> PostExternalData(ModelExample model)
        {
            string url = "posts";

            var httpContent = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(url, httpContent);

            if (response.IsSuccessStatusCode)
            {
                var jsonRespuesta = await response.Content.ReadAsStringAsync();
                var resultado = JsonConvert.DeserializeObject<ModelExample>(jsonRespuesta);

                return resultado;
            }

            return model;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<HttpResponseMessage> LogOn()
        {
            string url = "Auth/LogOn";
            var jsonConvert = JsonConvert.SerializeObject(
                new LogOn()
                {
                    ClientGuid = _apiBncSettings.ClientGUID
                });

            return await SendRequest(_apiBncSettings.MasterKey, jsonConvert, url, 0);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<HttpResponseMessage> SendP2P(SendP2P model)
        {
            string url = "MobPayment/SendP2P";
            var jsonConvert = JsonConvert.SerializeObject(model);

            var modelRequest = new RequestDb()
            {
                Method = Methods.SendP2P,
                Url = url
            };
            // CREAR REQUEST EN BBDD
            var resultCreate = await CreateRequest(modelRequest, 
                await _workingKeyServices.GetWorkingKeyObject(), 
                jsonConvert);

            return await SendRequest(await _workingKeyServices.GetWorkingKey() ?? "", 
                jsonConvert, 
                url, 
                resultCreate);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<HttpResponseMessage> SendC2P(SendC2P model)
        {
            string url = "MobPayment/SendC2P";
            var jsonConvert = JsonConvert.SerializeObject(model);

            var modelRequest = new RequestDb()
            {
                Method = Methods.SendC2P,
                Url = url
            };
            // CREAR REQUEST EN BBDD
            var resultCreate = await CreateRequest(modelRequest,
                await _workingKeyServices.GetWorkingKeyObject(),
                jsonConvert);

            return await SendRequest(await _workingKeyServices.GetWorkingKey() ?? "",
                jsonConvert,
                url,
                resultCreate);
        }

        public async Task<HttpResponseMessage> ReverseC2P(ReverseC2P model)
        {
            string url = "MobPayment/ReverseC2P";
            var jsonConvert = JsonConvert.SerializeObject(model);
            var modelRequest = new RequestDb()
            {
                Method = Methods.ReverseC2P,
                Url = url
            };
            // CREAR REQUEST EN BBDD
            var resultCreate = await CreateRequest(modelRequest,
                await _workingKeyServices.GetWorkingKeyObject(),
                jsonConvert);

            return await SendRequest(await _workingKeyServices.GetWorkingKey() ?? "",
                jsonConvert,
                url,
                resultCreate);
        }

        public async Task<HttpResponseMessage> ReverseC2PALT(ReverseC2Palt model)
        {
            string url = "MobPayment/ReverseC2PALT";
            var jsonConvert = JsonConvert.SerializeObject(model);
            var modelRequest = new RequestDb()
            {
                Method = Methods.ReverseC2PALT,
                Url = url
            };
            // CREAR REQUEST EN BBDD
            var resultCreate = await CreateRequest(modelRequest,
                await _workingKeyServices.GetWorkingKeyObject(),
                jsonConvert);

            return await SendRequest(await _workingKeyServices.GetWorkingKey() ?? "",
                jsonConvert,
                url,
                resultCreate);
        }

        public async Task<HttpResponseMessage> Send(Send model)
        {
            string url = "Transaction/Send";
            var jsonConvert = JsonConvert.SerializeObject(model);
            var modelRequest = new RequestDb()
            {
                Method = Methods.Send,
                Url = url
            };
            // CREAR REQUEST EN BBDD
            var resultCreate = await CreateRequest(modelRequest,
                await _workingKeyServices.GetWorkingKeyObject(),
                jsonConvert);

            return await SendRequest(await _workingKeyServices.GetWorkingKey() ?? "",
                jsonConvert,
                url,
                resultCreate);
        }

        public async Task<HttpResponseMessage> Banks()
        {
            string url = "Services/Banks";
            var jsonConvert = JsonConvert.SerializeObject(new { });
            var modelRequest = new RequestDb()
            {
                Method = Methods.Banks,
                Url = url
            };
            // CREAR REQUEST EN BBDD
            var resultCreate = await CreateRequest(modelRequest,
                await _workingKeyServices.GetWorkingKeyObject(),
                jsonConvert);

            return await SendRequest(await _workingKeyServices.GetWorkingKey() ?? "",
                jsonConvert,
                url,
                resultCreate);
        }

        public async Task<HttpResponseMessage> Current(Current model)
        {
            string url = "Position/Current";
            var jsonConvert = JsonConvert.SerializeObject(model);
            var modelRequest = new RequestDb()
            {
                Method = Methods.Current,
                Url = url
            };
            // CREAR REQUEST EN BBDD
            var resultCreate = await CreateRequest(modelRequest,
                await _workingKeyServices.GetWorkingKeyObject(),
                jsonConvert);

            return await SendRequest(await _workingKeyServices.GetWorkingKey() ?? "",
                jsonConvert,
                url,
                resultCreate);
        }


        public async Task<HttpResponseMessage> History(History model)
        {
            string url = "Position/History";
            var jsonConvert = JsonConvert.SerializeObject(model);
            var modelRequest = new RequestDb()
            {
                Method = Methods.History,
                Url = url
            };
            // CREAR REQUEST EN BBDD
            var resultCreate = await CreateRequest(modelRequest,
                await _workingKeyServices.GetWorkingKeyObject(),
                jsonConvert);

            return await SendRequest(await _workingKeyServices.GetWorkingKey() ?? "",
                jsonConvert,
                url,
                resultCreate);
        }

        public async Task<HttpResponseMessage> HistoryByDate(HistoryByDate model)
        {
            string url = "Position/HistoryByDate";
            var jsonConvert = JsonConvert.SerializeObject(model);
            var modelRequest = new RequestDb()
            {
                Method = Methods.HistoryByDate,
                Url = url
            };
            // CREAR REQUEST EN BBDD
            var resultCreate = await CreateRequest(modelRequest,
                await _workingKeyServices.GetWorkingKeyObject(),
                jsonConvert);

            return await SendRequest(await _workingKeyServices.GetWorkingKey() ?? "",
                jsonConvert,
                url,
                resultCreate);
        }

        public async Task<HttpResponseMessage> Validate(Validate model)
        {
            string url = "Position/Validate";
            var jsonConvert = JsonConvert.SerializeObject(model);
            var modelRequest = new RequestDb()
            {
                Method = Methods.Validate,
                Url = url
            };
            // CREAR REQUEST EN BBDD
            var resultCreate = await CreateRequest(modelRequest,
                await _workingKeyServices.GetWorkingKeyObject(),
                jsonConvert);

            return await SendRequest(await _workingKeyServices.GetWorkingKey() ?? "",
                jsonConvert,
                url,
                resultCreate);
        }

        public async Task<HttpResponseMessage> ValidateExistence(ValidateExistence model)
        {
            string url = "Position/ValidateExistence";
            var jsonConvert = JsonConvert.SerializeObject(model);
            var modelRequest = new RequestDb()
            {
                Method = Methods.ValidateExistence,
                Url = url
            };
            // CREAR REQUEST EN BBDD
            var resultCreate = await CreateRequest(modelRequest,
                await _workingKeyServices.GetWorkingKeyObject(),
                jsonConvert);

            return await SendRequest(await _workingKeyServices.GetWorkingKey() ?? "",
                jsonConvert,
                url,
                resultCreate);
        }

        public async Task<HttpResponseMessage> ValidateP2P(ValidateP2P model)
        {
            string url = "Position/ValidateP2P";
            var jsonConvert = JsonConvert.SerializeObject(model);
            var modelRequest = new RequestDb()
            {
                Method = Methods.ValidateP2P,
                Url = url
            };
            // CREAR REQUEST EN BBDD
            var resultCreate = await CreateRequest(modelRequest,
                await _workingKeyServices.GetWorkingKeyObject(),
                jsonConvert);

            return await SendRequest(await _workingKeyServices.GetWorkingKey() ?? "",
                jsonConvert,
                url,
                resultCreate);
        }

        public async Task<HttpResponseMessage> TransactionPos(TransactionsPos model)
        {
            string url = "Position/TransactionPOS";
            var jsonConvert = JsonConvert.SerializeObject(model);
            var modelRequest = new RequestDb()
            {
                Method = Methods.TransactionPos,
                Url = url
            };
            // CREAR REQUEST EN BBDD
            var resultCreate = await CreateRequest(modelRequest,
                await _workingKeyServices.GetWorkingKeyObject(),
                jsonConvert);

            return await SendRequest(await _workingKeyServices.GetWorkingKey() ?? "",
                jsonConvert,
                url,
                resultCreate);
        }

        public async Task<HttpResponseMessage> BcvRates()
        {
            string url = "Services/BCVRates";
            var jsonConvert = JsonConvert.SerializeObject(new { });
            var modelRequest = new RequestDb()
            {
                Method = Methods.BcvRates,
                Url = url
            };
            // CREAR REQUEST EN BBDD
            var resultCreate = await CreateRequest(modelRequest,
                await _workingKeyServices.GetWorkingKeyObject(),
                jsonConvert);

            return await SendRequest(await _workingKeyServices.GetWorkingKey() ?? "",
                jsonConvert,
                url,
                resultCreate);
        }        
        
        private async Task<HttpResponseMessage> SendRequest(string encryptKey, string jsonObject, string url, long idRequest)
        {
            // TO DO:
            // Manejar errores en HASH y ENCRYPT
            /// Del objeto que sera el Value de el request
            /// calcular Hash
            var hashModel = _hashServices.CreateSHA256Hash(jsonObject);
            /// Calcular encriptacion ASE
            var encryptModel = _encryptServices.EncryptBnc(jsonObject, encryptKey ?? "");

            var request = new Request()
            {
                ClientGuid = _apiBncSettings.ClientGUID,
                Reference = "",
                Value = encryptModel,
                Validation = hashModel,
                SwTestOperation = false
            };

            var httpContent = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(url, httpContent);

            await CreateResponse(response, idRequest);
            
            return response;
        }

        private async Task<long> CreateRequest(RequestDb model, WorkingKey workingKey, string jsonObject)
        {
            var encryptModel = _encryptServices.EncryptBnc(jsonObject, workingKey.Key ?? "");

            model.RequestBody = encryptModel;
            model.IdWorkingKey = workingKey.Id;

            var idRequest = await _requestServices.Create(model);

            return idRequest;
        }

        private async Task CreateResponse(HttpResponseMessage responseApi, long idRequest)
        {
            if (responseApi != null && idRequest != 0)
            {
                var jsonResponse = await responseApi.Content.ReadAsStringAsync();

                var model = new ResponseDb()
                {
                    IdRequest = idRequest,
                    StatusCode = responseApi.StatusCode.ToString(),
                    ResponseBody = jsonResponse
                };

                await _responseServices.Create(model);
            }
        }
    }
}
