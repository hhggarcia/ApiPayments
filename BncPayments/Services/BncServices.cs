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
        Task<string> UpdateWorkingKey();
        Task<HttpResponseMessage> Validate(Validate model);
        Task<HttpResponseMessage> ValidateExistence(ValidateExistence model);
        Task<HttpResponseMessage> ValidateP2P(ValidateP2P model);
    }
    public class BncServices : IBncServices
    {
        private readonly HttpClient _httpClient;
        private readonly IEncryptionServices _encryptServices;
        private readonly IHashService _hashServices;
        private readonly IWorkingKeyServices _workingKeyServices;
        private readonly string _workingKeyTests;
        private readonly IRequestServices _requestServices;
        private readonly ApiBncSettings _apiBncSettings;
        public BncServices(IEncryptionServices encryptService,
            IHashService hashServices,
            ApiBncSettings apiBncSettings,
            IWorkingKeyServices workingKeyServices,
            IRequestServices requestServices)
        {
            _apiBncSettings = apiBncSettings;
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri(_apiBncSettings.BaseAddress);
            _encryptServices = encryptService;
            _hashServices = hashServices;
            _workingKeyServices = workingKeyServices;
            _workingKeyTests = "dd8321dc18579eed46015ab3fa518a88";
            _requestServices = requestServices;
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

            return await SendRequest(_apiBncSettings.MasterKey, jsonConvert, url);
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
                url);
            //return await SendRequest(_workingKeyTests, jsonConvert, url);
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
            return await SendRequest(_workingKeyServices.GetWorkingKey() ?? "", jsonConvert, url);
        }

        public async Task<HttpResponseMessage> ReverseC2P(ReverseC2P model)
        {
            string url = "MobPayment/ReverseC2P";
            var jsonConvert = JsonConvert.SerializeObject(model);
            return await SendRequest(_workingKeyServices.GetWorkingKey() ?? "", jsonConvert, url);
        }

        public async Task<HttpResponseMessage> ReverseC2PALT(ReverseC2Palt model)
        {
            string url = "MobPayment/ReverseC2PALT";
            var jsonConvert = JsonConvert.SerializeObject(model);
            return await SendRequest(_workingKeyServices.GetWorkingKey() ?? "", jsonConvert, url);
        }

        public async Task<HttpResponseMessage> Send(Send model)
        {
            string url = "Transaction/Send";
            var jsonConvert = JsonConvert.SerializeObject(model);
            return await SendRequest(_workingKeyServices.GetWorkingKey() ?? "", jsonConvert, url);
        }

        public async Task<HttpResponseMessage> Banks()
        {
            string url = "Services/Banks";
            var jsonConvert = JsonConvert.SerializeObject(new { });
            return await SendRequest(_workingKeyServices.GetWorkingKey() ?? "", jsonConvert, url);
            //return await SendRequest(_workingKeyTests, jsonConvert, url);
        }

        public async Task<HttpResponseMessage> Current(Current model)
        {
            string url = "Position/Current";
            var jsonConvert = JsonConvert.SerializeObject(model);
            return await SendRequest(_workingKeyServices.GetWorkingKey() ?? "", jsonConvert, url);
        }


        public async Task<HttpResponseMessage> History(History model)
        {
            string url = "Position/History";
            var jsonConvert = JsonConvert.SerializeObject(model);
            return await SendRequest(_workingKeyServices.GetWorkingKey() ?? "", jsonConvert, url);
        }

        public async Task<HttpResponseMessage> HistoryByDate(HistoryByDate model)
        {
            string url = "Position/HistoryByDate";
            var jsonConvert = JsonConvert.SerializeObject(model);
            return await SendRequest(_workingKeyServices.GetWorkingKey() ?? "", jsonConvert, url);
        }

        public async Task<HttpResponseMessage> Validate(Validate model)
        {
            string url = "Position/Validate";
            var jsonConvert = JsonConvert.SerializeObject(model);
            return await SendRequest(_workingKeyServices.GetWorkingKey() ?? "", jsonConvert, url);
        }

        public async Task<HttpResponseMessage> ValidateExistence(ValidateExistence model)
        {
            string url = "Position/ValidateExistence";
            var jsonConvert = JsonConvert.SerializeObject(model);
            return await SendRequest(_workingKeyServices.GetWorkingKey() ?? "", jsonConvert, url);
        }

        public async Task<HttpResponseMessage> ValidateP2P(ValidateP2P model)
        {
            string url = "Position/ValidateP2P";
            var jsonConvert = JsonConvert.SerializeObject(model);
            return await SendRequest(_workingKeyServices.GetWorkingKey() ?? "", jsonConvert, url);
        }

        public async Task<HttpResponseMessage> TransactionPos(TransactionsPos model)
        {
            string url = "Position/TransactionPOS";
            var jsonConvert = JsonConvert.SerializeObject(model);
            return await SendRequest(_workingKeyServices.GetWorkingKey() ?? "", jsonConvert, url);
        }

        public async Task<HttpResponseMessage> BcvRates()
        {
            string url = "Services/BCVRates";
            var jsonConvert = JsonConvert.SerializeObject(new { });
            return await SendRequest(_workingKeyServices.GetWorkingKey() ?? "", jsonConvert, url);
        }
        
        public async Task<string> UpdateWorkingKey()
        {
            try
            {
                var result = string.Empty;
                var response = await LogOn();
                var reLogOnjsonResponse = await response.Content.ReadAsStringAsync();
                var reLogOnResult = JsonConvert.DeserializeObject<Response>(reLogOnjsonResponse);

                if (response.IsSuccessStatusCode &&
                    reLogOnResult != null &&
                    reLogOnResult.Status.Equals("OK"))
                {
                    /// desencriptar el result.Value
                    var decryptResult = _encryptServices.DecryptText(reLogOnResult.Value, _apiBncSettings.MasterKey);

                    var logOnResponse = JsonConvert.DeserializeObject<LogOnResponse>(decryptResult);

                    if (logOnResponse != null)
                    {
                        result = logOnResponse.WorkingKey;
                    }
                }
                else if (reLogOnResult != null &&
                    reLogOnResult.Status.Equals("KO"))
                {
                    result = reLogOnResult.Status;
                }

                return result;
            }
            catch (Exception ex)
            {
                return "KO"+ex.Message;
            }

        }
        private async Task<HttpResponseMessage> SendRequest(string encryptKey, string jsonObject, string url)
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

            return response;
        }

        private async Task<bool> CreateRequest(RequestDb model, WorkingKey workingKey, string jsonObject)
        {
            var encryptModel = _encryptServices.EncryptBnc(jsonObject, workingKey.Key ?? "");

            model.RequestBody = encryptModel;
            model.WorkingKeyId = workingKey.Id;

            var idRequest = await _requestServices.Create(model);

            return idRequest != 0;
        }
    }
}
