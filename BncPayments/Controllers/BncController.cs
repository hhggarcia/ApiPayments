using BncPayments.Services;
using ClassLibrary.BncModels;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Net;
using System.Reflection;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace BncPayments.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class BncController : ControllerBase
    {
        private readonly ILogger<BncController> _logger;
        private readonly IBncServices _bncServices;
        private readonly ApiBncSettings _apiBncSettings;
        private readonly WorkingKeyServices _workingKey;
        private readonly IEncryptionServices _encryptServices;
        private readonly string _workingKeyTests;

        public BncController(ILogger<BncController> logger,
            IBncServices bncServices,
            ApiBncSettings apiBncSettings,
            WorkingKeyServices workingKey,
            IEncryptionServices encryptionServices)
        {
            _logger = logger;
            _bncServices = bncServices;
            _apiBncSettings = apiBncSettings;
            _workingKey = workingKey;
            _encryptServices = encryptionServices;
            _workingKeyTests = "dd8321dc18579eed46015ab3fa518a88";

        }

        //[HttpGet(Name = "posts")]
        //public async Task<ActionResult> GetExternalData()
        //{
        //    var data = await _bncServices.GetExternalDataAsync();
        //    return Ok(data);
        //}

        //[HttpPost("testPost")]
        //public async Task<ActionResult> PostExternalData([FromBody] ModelExample model)
        //{
        //    var rest = await _bncServices.PostExternalData(model);
        //    return Ok(rest);
        //}

        //[HttpGet("Encriptar")]
        //public IActionResult Encriptar(string textoEncriptar)
        //{
        //    var encriptar = _encryptServices.EncryptBnc(textoEncriptar, _apiBncSettings.MasterKey);
        //    return Ok(encriptar);
        //}

        //[HttpGet("Desencriptar")]
        //public IActionResult Desencriptar(string textoDesencriptar)
        //{
        //    var desencriptar = _encryptServices.DecryptText(textoDesencriptar, _apiBncSettings.MasterKey);
        //    return Ok(desencriptar);
        //}

        [HttpPost("LogOn")]
        public async Task<ActionResult> LogOn()
        {
            try
            {
                var response = await _bncServices.LogOn();
                var jsonResponse = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var result = JsonConvert.DeserializeObject<Response>(jsonResponse);

                    if (result != null &&
                        result.Status.Equals("OK"))
                    {
                        /// desencriptar el result.Value
                        var decryptResult = _encryptServices.DecryptText(result.Value, _apiBncSettings.MasterKey);

                        var logOnResponse = JsonConvert.DeserializeObject<LogOnResponse>(decryptResult);

                        if (logOnResponse != null)
                        {
                            _workingKey.SetWorkingKey(logOnResponse.WorkingKey);
                        }
                        _logger.LogInformation("Returning Ok response.");
                        return Ok(logOnResponse);
                    }
                    return Ok(jsonResponse);
                }
                else if (jsonResponse.Contains("KO"))
                {
                    var resultKo = JsonConvert.DeserializeObject<Response>(jsonResponse);

                    if (resultKo != null)
                    {
                        // Manejo específico de la excepción
                        _logger.LogError($"Working key is broked: {resultKo.Message}");

                        return StatusCode((int)HttpStatusCode.InternalServerError, new
                        {
                            resultKo.Status,
                            resultKo.Message,
                            resultKo.Value,
                            resultKo.Validation
                        });
                    }
                    return BadRequest(jsonResponse);
                }
                else
                {
                    return BadRequest(jsonResponse);
                }

            }
            catch (Exception ex)
            {
                // Manejo específico de la excepción
                _logger.LogError($"Error in LogOn: {ex.Message}");
                return StatusCode((int)HttpStatusCode.InternalServerError, new { Message = "An error occurred while processing your request." });
            }
        }

        [HttpPost("SendP2P")]
        public async Task<ActionResult> SendP2P(SendP2P model)
        {
            try
            {
                // TO DO: Verificar parametros del modelo

                var response = await _bncServices.SendP2P(model);
                var jsonResponse = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var result = JsonConvert.DeserializeObject<Response>(jsonResponse);

                    if (result != null &&
                        result.Status.Equals("OK"))
                    {
                        /// desencriptar el result.Value
                        var decryptResult = _encryptServices.DecryptText(result.Value, _workingKey.GetWorkingKey());
                        //var decryptResult = _encryptServices.DecryptText(result.Value, _workingKeyTests);

                        var logOnResponse = JsonConvert.DeserializeObject<SendP2PResponse>(decryptResult);

                        return Ok(logOnResponse);
                    }
                    return Ok(jsonResponse);

                }
                else if (jsonResponse.Contains("RWK"))
                {
                    return await LogOn();
                }
                else if (jsonResponse.Contains("KO"))
                {
                    var resultKo = JsonConvert.DeserializeObject<Response>(jsonResponse);

                    if (resultKo != null)
                    {
                        // Manejo del error
                        _logger.LogError($"Error in LogOn: {resultKo.Message}");
                        return StatusCode((int)HttpStatusCode.InternalServerError, new
                        {
                            resultKo.Status,
                            resultKo.Message,
                            resultKo.Validation,
                            resultKo.Value
                        });
                    }
                    return BadRequest(jsonResponse);
                }
                else
                {
                    return BadRequest(jsonResponse);
                }
            }
            catch (Exception ex)
            {
                // Manejo específico de la excepción
                _logger.LogError($"Error in LogOn: {ex.Message}");
                return StatusCode((int)HttpStatusCode.InternalServerError, new { Message = "An error occurred while processing your request." });
            }
        }

        [HttpPost("SendC2P")]
        public async Task<ActionResult> SendC2P(SendC2P model)
        {
            try
            {
                // TO DO: Verificar parametros del modelo

                var response = await _bncServices.SendC2P(model);
                var jsonResponse = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var result = JsonConvert.DeserializeObject<Response>(jsonResponse);

                    if (result != null &&
                        result.Status.Equals("OK"))
                    {
                        /// desencriptar el result.Value
                        var decryptResult = _encryptServices.DecryptText(result.Value, _workingKey.GetWorkingKey());
                        //var decryptResult = _encryptServices.DecryptText(result.Value, _workingKeyTests);

                        var logOnResponse = JsonConvert.DeserializeObject<SendC2PResponse>(decryptResult);

                        return Ok(logOnResponse);
                    }
                    return Ok(jsonResponse);

                }
                else if (jsonResponse.Contains("RWK"))
                {
                    return await LogOn();
                }
                else if (jsonResponse.Contains("KO"))
                {
                    var resultKo = JsonConvert.DeserializeObject<Response>(jsonResponse);

                    if (resultKo != null)
                    {
                        // Manejo del error
                        _logger.LogError($"Error in LogOn: {resultKo.Message}");
                        return StatusCode((int)HttpStatusCode.InternalServerError, new
                        {
                            resultKo.Status,
                            resultKo.Message,
                            resultKo.Validation,
                            resultKo.Value
                        });
                    }
                    return BadRequest(jsonResponse);
                }
                else
                {
                    return BadRequest(jsonResponse);
                }
            }
            catch (Exception ex)
            {
                // Manejo específico de la excepción
                _logger.LogError($"Error in LogOn: {ex.Message}");
                return StatusCode((int)HttpStatusCode.InternalServerError, new { Message = "An error occurred while processing your request." });
            }
        }

        [HttpPost("ReverseC2P")]
        public async Task<ActionResult> ReverseC2P(ReverseC2P model)
        {
            try
            {
                // TO DO: Verificar parametros del modelo

                var response = await _bncServices.ReverseC2P(model);
                var jsonResponse = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var result = JsonConvert.DeserializeObject<Response>(jsonResponse);

                    if (result != null &&
                        result.Status.Equals("OK"))
                    {
                        /// desencriptar el result.Value
                        var decryptResult = _encryptServices.DecryptText(result.Value, _workingKey.GetWorkingKey());
                        //var decryptResult = _encryptServices.DecryptText(result.Value, _workingKeyTests);

                        var logOnResponse = JsonConvert.DeserializeObject<ReverseC2PResponse>(decryptResult);

                        return Ok(logOnResponse);
                    }
                    return Ok(jsonResponse);

                }
                else if (jsonResponse.Contains("RWK"))
                {
                    return await LogOn();
                }
                else if (jsonResponse.Contains("KO"))
                {
                    var resultKo = JsonConvert.DeserializeObject<Response>(jsonResponse);

                    if (resultKo != null)
                    {
                        // Manejo del error
                        _logger.LogError($"Error in LogOn: {resultKo.Message}");
                        return StatusCode((int)HttpStatusCode.InternalServerError, new
                        {
                            resultKo.Status,
                            resultKo.Message,
                            resultKo.Validation,
                            resultKo.Value
                        });
                    }
                    return BadRequest(jsonResponse);
                }
                else
                {
                    return BadRequest(jsonResponse);
                }
            }
            catch (Exception ex)
            {
                // Manejo específico de la excepción
                _logger.LogError($"Error in LogOn: {ex.Message}");
                return StatusCode((int)HttpStatusCode.InternalServerError, new { Message = "An error occurred while processing your request." });
            }
        }

        [HttpPost("ReverseC2PALT")]
        public async Task<ActionResult> ReverseC2PALT(ReverseC2Palt model)
        {
            try
            {
                // TO DO: Verificar parametros del modelo

                var response = await _bncServices.ReverseC2PALT(model);
                var jsonResponse = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var result = JsonConvert.DeserializeObject<Response>(jsonResponse);

                    if (result != null &&
                        result.Status.Equals("OK"))
                    {
                        /// desencriptar el result.Value
                        var decryptResult = _encryptServices.DecryptText(result.Value, _workingKey.GetWorkingKey());
                        //var decryptResult = _encryptServices.DecryptText(result.Value, _workingKeyTests);

                        var logOnResponse = JsonConvert.DeserializeObject<ReverseC2PaltResponse>(decryptResult);

                        return Ok(logOnResponse);
                    }
                    return Ok(jsonResponse);

                }
                else if (jsonResponse.Contains("RWK"))
                {
                    return await LogOn();
                }
                else if (jsonResponse.Contains("KO"))
                {
                    var resultKo = JsonConvert.DeserializeObject<Response>(jsonResponse);

                    if (resultKo != null)
                    {
                        // Manejo del error
                        _logger.LogError($"Error in LogOn: {resultKo.Message}");
                        return StatusCode((int)HttpStatusCode.InternalServerError, new
                        {
                            resultKo.Status,
                            resultKo.Message,
                            resultKo.Validation,
                            resultKo.Value
                        });
                    }
                    return BadRequest(jsonResponse);
                }
                else
                {
                    return BadRequest(jsonResponse);
                }
            }
            catch (Exception ex)
            {
                // Manejo específico de la excepción
                _logger.LogError($"Error in LogOn: {ex.Message}");
                return StatusCode((int)HttpStatusCode.InternalServerError, new { Message = "An error occurred while processing your request." });
            }
        }
        [HttpPost("Send")]
        public async Task<ActionResult> Send(Send model)
        {
            try
            {
                // TO DO: Verificar parametros del modelo

                var response = await _bncServices.Send(model);
                var jsonResponse = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var result = JsonConvert.DeserializeObject<Response>(jsonResponse);

                    if (result != null &&
                        result.Status.Equals("OK"))
                    {
                        /// desencriptar el result.Value
                        var decryptResult = _encryptServices.DecryptText(result.Value, _workingKey.GetWorkingKey());
                        //var decryptResult = _encryptServices.DecryptText(result.Value, _workingKeyTests);

                        var logOnResponse = JsonConvert.DeserializeObject<SendResponse>(decryptResult);

                        return Ok(logOnResponse);
                    }
                    return Ok(jsonResponse);

                }
                else if (jsonResponse.Contains("RWK"))
                {
                    return await LogOn();
                }
                else if (jsonResponse.Contains("KO"))
                {
                    var resultKo = JsonConvert.DeserializeObject<Response>(jsonResponse);

                    if (resultKo != null)
                    {
                        // Manejo del error
                        _logger.LogError($"Error in LogOn: {resultKo.Message}");
                        return StatusCode((int)HttpStatusCode.InternalServerError, new
                        {
                            resultKo.Status,
                            resultKo.Message,
                            resultKo.Validation,
                            resultKo.Value
                        });
                    }
                    return BadRequest(jsonResponse);
                }
                else
                {
                    return BadRequest(jsonResponse);
                }
            }
            catch (Exception ex)
            {
                // Manejo específico de la excepción
                _logger.LogError($"Error in LogOn: {ex.Message}");
                return StatusCode((int)HttpStatusCode.InternalServerError, new { Message = "An error occurred while processing your request." });
            }
        }
        [HttpPost("Banks")]
        public async Task<ActionResult> Banks()
        {
            try
            {
                // TO DO: Verificar parametros del modelo

                var response = await _bncServices.Banks();
                var jsonResponse = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var result = JsonConvert.DeserializeObject<Response>(jsonResponse);

                    if (result != null &&
                        result.Status.Equals("OK"))
                    {
                        /// desencriptar el result.Value
                        var decryptResult = _encryptServices.DecryptText(result.Value, _workingKey.GetWorkingKey());
                        //var decryptResult = _encryptServices.DecryptText(result.Value, _workingKeyTests);

                        var logOnResponse = JsonConvert.DeserializeObject<List<Banks>>(decryptResult);

                        return Ok(logOnResponse);
                    }
                    return Ok(jsonResponse);

                }
                else if (jsonResponse.Contains("RWK"))
                {
                    return await LogOn();
                }
                else if (jsonResponse.Contains("KO"))
                {
                    var resultKo = JsonConvert.DeserializeObject<Response>(jsonResponse);

                    if (resultKo != null)
                    {
                        // Manejo del error
                        _logger.LogError($"Error in LogOn: {resultKo.Message}");
                        return StatusCode((int)HttpStatusCode.InternalServerError, new
                        {
                            resultKo.Status,
                            resultKo.Message,
                            resultKo.Validation,
                            resultKo.Value
                        });
                    }
                    return BadRequest(jsonResponse);
                }
                else
                {
                    return BadRequest(jsonResponse);
                }
            }
            catch (Exception ex)
            {
                // Manejo específico de la excepción
                _logger.LogError($"Error in LogOn: {ex.Message}");
                return StatusCode((int)HttpStatusCode.InternalServerError, new { Message = "An error occurred while processing your request." });
            }
        }

        [HttpPost("Current")]
        public async Task<ActionResult> Current(Current model)
        {
            try
            {
                // TO DO: Verificar parametros del modelo

                var response = await _bncServices.Current(model);
                var jsonResponse = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var result = JsonConvert.DeserializeObject<Response>(jsonResponse);

                    if (result != null &&
                        result.Status.Equals("OK"))
                    {
                        /// desencriptar el result.Value
                        var decryptResult = _encryptServices.DecryptText(result.Value, _workingKey.GetWorkingKey());
                        //var decryptResult = _encryptServices.DecryptText(result.Value, _workingKeyTests);

                        var logOnResponse = JsonConvert.DeserializeObject<Dictionary<string, CurrentItem>>(decryptResult);

                        return Ok(logOnResponse);
                    }
                    return Ok(jsonResponse);

                }
                else if (jsonResponse.Contains("RWK"))
                {
                    return await LogOn();
                }
                else if (jsonResponse.Contains("KO"))
                {
                    var resultKo = JsonConvert.DeserializeObject<Response>(jsonResponse);

                    if (resultKo != null)
                    {
                        // Manejo del error
                        _logger.LogError($"Error in LogOn: {resultKo.Message}");
                        return StatusCode((int)HttpStatusCode.InternalServerError, new
                        {
                            resultKo.Status,
                            resultKo.Message,
                            resultKo.Validation,
                            resultKo.Value
                        });
                    }
                    return BadRequest(jsonResponse);
                }
                else
                {
                    return BadRequest(jsonResponse);
                }
            }
            catch (Exception ex)
            {
                // Manejo específico de la excepción
                _logger.LogError($"Error in LogOn: {ex.Message}");
                return StatusCode((int)HttpStatusCode.InternalServerError, new { Message = "An error occurred while processing your request." });
            }
        }

        [HttpPost("History")]
        public async Task<ActionResult> History(History model)
        {
            try
            {
                // TO DO: Verificar parametros del modelo

                var response = await _bncServices.History(model);
                var jsonResponse = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var result = JsonConvert.DeserializeObject<Response>(jsonResponse);

                    if (result != null &&
                        result.Status.Equals("OK"))
                    {
                        /// desencriptar el result.Value
                        var decryptResult = _encryptServices.DecryptText(result.Value, _workingKey.GetWorkingKey());
                        //var decryptResult = _encryptServices.DecryptText(result.Value, _workingKeyTests);

                        var logOnResponse = JsonConvert.DeserializeObject<Dictionary<HistoryKey, HistoryValue>>(decryptResult);

                        return Ok(logOnResponse);
                    }
                    return Ok(jsonResponse);

                }
                else if (jsonResponse.Contains("RWK"))
                {
                    return await LogOn();
                }
                else if (jsonResponse.Contains("KO"))
                {
                    var resultKo = JsonConvert.DeserializeObject<Response>(jsonResponse);

                    if (resultKo != null)
                    {
                        // Manejo del error
                        _logger.LogError($"Error in LogOn: {resultKo.Message}");
                        return StatusCode((int)HttpStatusCode.InternalServerError, new
                        {
                            resultKo.Status,
                            resultKo.Message,
                            resultKo.Validation,
                            resultKo.Value
                        });
                    }
                    return BadRequest(jsonResponse);
                }
                else
                {
                    return BadRequest(jsonResponse);
                }
            }
            catch (Exception ex)
            {
                // Manejo específico de la excepción
                _logger.LogError($"Error in LogOn: {ex.Message}");
                return StatusCode((int)HttpStatusCode.InternalServerError, new { Message = "An error occurred while processing your request." });
            }
        }

        [HttpPost("HistoryByDate")]
        public async Task<ActionResult> HistoryByDate(HistoryByDate model)
        {
            try
            {
                //Dictionary<string, List<HistoryByDateValue>>
                // TO DO: Verificar parametros del modelo

                var response = await _bncServices.HistoryByDate(model);
                var jsonResponse = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var result = JsonConvert.DeserializeObject<Response>(jsonResponse);

                    if (result != null &&
                        result.Status.Equals("OK"))
                    {
                        /// desencriptar el result.Value
                        var decryptResult = _encryptServices.DecryptText(result.Value, _workingKey.GetWorkingKey());
                        //var decryptResult = _encryptServices.DecryptText(result.Value, _workingKeyTests);

                        var logOnResponse = JsonConvert.DeserializeObject<Dictionary<string, List<HistoryByDateValue>>>(decryptResult);

                        return Ok(logOnResponse);
                    }
                    return Ok(jsonResponse);

                }
                else if (jsonResponse.Contains("RWK"))
                {
                    return await LogOn();
                }
                else if (jsonResponse.Contains("KO"))
                {
                    var resultKo = JsonConvert.DeserializeObject<Response>(jsonResponse);

                    if (resultKo != null)
                    {
                        // Manejo del error
                        _logger.LogError($"Error in LogOn: {resultKo.Message}");
                        return StatusCode((int)HttpStatusCode.InternalServerError, new
                        {
                            resultKo.Status,
                            resultKo.Message,
                            resultKo.Validation,
                            resultKo.Value
                        });
                    }
                    return BadRequest(jsonResponse);
                }
                else
                {
                    return BadRequest(jsonResponse);
                }
            }
            catch (Exception ex)
            {
                // Manejo específico de la excepción
                _logger.LogError($"Error in LogOn: {ex.Message}");
                return StatusCode((int)HttpStatusCode.InternalServerError, new { Message = "An error occurred while processing your request." });
            }
        }

        [HttpPost("Validate")]
        public async Task<ActionResult> Validate(Validate model)
        {
            try
            {
                // TO DO: Verificar parametros del modelo

                var response = await _bncServices.Validate(model);
                var jsonResponse = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var result = JsonConvert.DeserializeObject<Response>(jsonResponse);

                    if (result != null &&
                        result.Status.Equals("OK"))
                    {
                        /// desencriptar el result.Value
                        var decryptResult = _encryptServices.DecryptText(result.Value, _workingKey.GetWorkingKey());
                        //var decryptResult = _encryptServices.DecryptText(result.Value, _workingKeyTests);

                        var logOnResponse = JsonConvert.DeserializeObject<ValidateResponse>(decryptResult);

                        return Ok(logOnResponse);
                    }
                    return Ok(jsonResponse);

                }
                else if (jsonResponse.Contains("RWK"))
                {
                    return await LogOn();
                }
                else if (jsonResponse.Contains("KO"))
                {
                    var resultKo = JsonConvert.DeserializeObject<Response>(jsonResponse);

                    if (resultKo != null)
                    {
                        // Manejo del error
                        _logger.LogError($"Error in LogOn: {resultKo.Message}");
                        return StatusCode((int)HttpStatusCode.InternalServerError, new
                        {
                            resultKo.Status,
                            resultKo.Message,
                            resultKo.Validation,
                            resultKo.Value
                        });
                    }
                    return BadRequest(jsonResponse);
                }
                else
                {
                    return BadRequest(jsonResponse);
                }
            }
            catch (Exception ex)
            {
                // Manejo específico de la excepción
                _logger.LogError($"Error in LogOn: {ex.Message}");
                return StatusCode((int)HttpStatusCode.InternalServerError, new { Message = "An error occurred while processing your request." });
            }
        }

        [HttpPost("ValidateExistence")]
        public async Task<ActionResult> ValidateExistance(ValidateExistence model)
        {
            try
            {
                // TO DO: Verificar parametros del modelo

                var response = await _bncServices.ValidateExistence(model);
                var jsonResponse = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var result = JsonConvert.DeserializeObject<Response>(jsonResponse);

                    if (result != null &&
                        result.Status.Equals("OK"))
                    {
                        /// desencriptar el result.Value
                        var decryptResult = _encryptServices.DecryptText(result.Value, _workingKey.GetWorkingKey());
                        //var decryptResult = _encryptServices.DecryptText(result.Value, _workingKeyTests);

                        var logOnResponse = JsonConvert.DeserializeObject<ValidateExistenceResponse>(decryptResult);

                        return Ok(logOnResponse);
                    }
                    return Ok(jsonResponse);

                }
                else if (jsonResponse.Contains("RWK"))
                {
                    return await LogOn();
                }
                else if (jsonResponse.Contains("KO"))
                {
                    var resultKo = JsonConvert.DeserializeObject<Response>(jsonResponse);

                    if (resultKo != null)
                    {
                        // Manejo del error
                        _logger.LogError($"Error in LogOn: {resultKo.Message}");
                        return StatusCode((int)HttpStatusCode.InternalServerError, new
                        {
                            resultKo.Status,
                            resultKo.Message,
                            resultKo.Validation,
                            resultKo.Value
                        });
                    }
                    return BadRequest(jsonResponse);
                }
                else
                {
                    return BadRequest(jsonResponse);
                }
            }
            catch (Exception ex)
            {
                // Manejo específico de la excepción
                _logger.LogError($"Error in LogOn: {ex.Message}");
                return StatusCode((int)HttpStatusCode.InternalServerError, new { Message = "An error occurred while processing your request." });
            }
        }

        [HttpPost("ValidateP2P")]
        public async Task<ActionResult> ValidateP2P(ValidateP2P model)
        {
            try
            {
                // TO DO: Verificar parametros del modelo

                var response = await _bncServices.ValidateP2P(model);
                var jsonResponse = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var result = JsonConvert.DeserializeObject<Response>(jsonResponse);

                    if (result != null &&
                        result.Status.Equals("OK"))
                    {
                        /// desencriptar el result.Value
                        var decryptResult = _encryptServices.DecryptText(result.Value, _workingKey.GetWorkingKey());
                        //var decryptResult = _encryptServices.DecryptText(result.Value, _workingKeyTests);

                        var logOnResponse = JsonConvert.DeserializeObject<ValidateP2PResponse>(decryptResult);

                        return Ok(logOnResponse);
                    }
                    return Ok(jsonResponse);

                }
                else if (jsonResponse.Contains("RWK"))
                {
                    return await LogOn();
                }
                else if (jsonResponse.Contains("KO"))
                {
                    var resultKo = JsonConvert.DeserializeObject<Response>(jsonResponse);

                    if (resultKo != null)
                    {
                        // Manejo del error
                        _logger.LogError($"Error in LogOn: {resultKo.Message}");
                        return StatusCode((int)HttpStatusCode.InternalServerError, new
                        {
                            resultKo.Status,
                            resultKo.Message,
                            resultKo.Validation,
                            resultKo.Value
                        });
                    }
                    return BadRequest(jsonResponse);
                }
                else
                {
                    return BadRequest(jsonResponse);
                }
            }
            catch (Exception ex)
            {
                // Manejo específico de la excepción
                _logger.LogError($"Error in LogOn: {ex.Message}");
                return StatusCode((int)HttpStatusCode.InternalServerError, new { Message = "An error occurred while processing your request." });
            }
        }

        [HttpPost("TransactionPos")]
        public async Task<ActionResult> TransactionPos(TransactionsPos model)
        {
            try
            {
                // TO DO: Verificar parametros del modelo

                var response = await _bncServices.TransactionPos(model);
                var jsonResponse = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var result = JsonConvert.DeserializeObject<Response>(jsonResponse);

                    if (result != null &&
                        result.Status.Equals("OK"))
                    {
                        /// desencriptar el result.Value
                        var decryptResult = _encryptServices.DecryptText(result.Value, _workingKey.GetWorkingKey());
                        //var decryptResult = _encryptServices.DecryptText(result.Value, _workingKeyTests);

                        var logOnResponse = JsonConvert.DeserializeObject<List<TransactionPosResponse>>(decryptResult);

                        return Ok(logOnResponse);
                    }
                    return Ok(jsonResponse);

                }
                else if (jsonResponse.Contains("RWK"))
                {
                    return await LogOn();
                }
                else if (jsonResponse.Contains("KO"))
                {
                    var resultKo = JsonConvert.DeserializeObject<Response>(jsonResponse);

                    if (resultKo != null)
                    {
                        // Manejo del error
                        _logger.LogError($"Error in LogOn: {resultKo.Message}");
                        return StatusCode((int)HttpStatusCode.InternalServerError, new
                        {
                            resultKo.Status,
                            resultKo.Message,
                            resultKo.Validation,
                            resultKo.Value
                        });
                    }
                    return BadRequest(jsonResponse);
                }
                else
                {
                    return BadRequest(jsonResponse);
                }
            }
            catch (Exception ex)
            {
                // Manejo específico de la excepción
                _logger.LogError($"Error in LogOn: {ex.Message}");
                return StatusCode((int)HttpStatusCode.InternalServerError, new { Message = "An error occurred while processing your request." });
            }
        }

        [HttpPost("BcvRates")]
        public async Task<ActionResult> BcvRates()
        {
            try
            {
                // TO DO: Verificar parametros del modelo

                var response = await _bncServices.BcvRates();
                var jsonResponse = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var result = JsonConvert.DeserializeObject<Response>(jsonResponse);

                    if (result != null &&
                        result.Status.Equals("OK"))
                    {
                        /// desencriptar el result.Value
                        var decryptResult = _encryptServices.DecryptText(result.Value, _workingKey.GetWorkingKey());
                        //var decryptResult = _encryptServices.DecryptText(result.Value, _workingKeyTests);

                        var logOnResponse = JsonConvert.DeserializeObject<BcvRates>(decryptResult);

                        return Ok(logOnResponse);
                    }
                    return Ok(jsonResponse);

                }
                else if (jsonResponse.Contains("RWK"))
                {
                    return await LogOn();
                }
                else if (jsonResponse.Contains("KO"))
                {
                    var resultKo = JsonConvert.DeserializeObject<Response>(jsonResponse);

                    if (resultKo != null)
                    {
                        // Manejo del error
                        _logger.LogError($"Error in LogOn: {resultKo.Message}");
                        return StatusCode((int)HttpStatusCode.InternalServerError, new
                        {
                            resultKo.Status,
                            resultKo.Message,
                            resultKo.Validation,
                            resultKo.Value
                        });
                    }
                    return BadRequest(jsonResponse);
                }
                else
                {
                    return BadRequest(jsonResponse);
                }
            }
            catch (Exception ex)
            {
                // Manejo específico de la excepción
                _logger.LogError($"Error in LogOn: {ex.Message}");
                return StatusCode((int)HttpStatusCode.InternalServerError, new { Message = "An error occurred while processing your request." });
            }
        }

        //[HttpPost("LogOn")]
        //private async Task<ActionResult> LogOn()
        //{
        //    var resultUpdate = await _bncServices.UpdateWorkingKey();

        //    if (!resultUpdate.Equals("KO"))
        //    {
        //        return Ok(resultUpdate);
        //    }
        //    else
        //    {
        //        return StatusCode((int)HttpStatusCode.InternalServerError, new { Message = "Error al intentar actulizar la working key" });
        //    }
        //}
    }
}
