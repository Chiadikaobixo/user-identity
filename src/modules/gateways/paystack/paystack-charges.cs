using System.Text;

namespace paystack_charge
{
    public class PaystackCharge
    {
        public const string PAYSTACK_BASE_URL = "https://api.paystack.co/transaction";
        private readonly HttpClient _httpClient;
        private readonly string _paystackSecretKey;

        public PaystackCharge(string paystackSecretKey)
        {
            _httpClient = new HttpClient();
            _paystackSecretKey = paystackSecretKey;
        }

        public async Task<string> initializeDeposit(string userEmail, int orderAmount)
        {
            try
            {
                var requestData = new
                {
                    email = userEmail,
                    amount = orderAmount * 100,
                    callback_url = "https://yourpreferdcallbackurl.com"
                };

                var requestJson = System.Text.Json.JsonSerializer.Serialize(requestData);
                var requestContent = new StringContent(requestJson, Encoding.UTF8, "application/json");

                _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _paystackSecretKey);

                var response = await _httpClient.PostAsync($"{PAYSTACK_BASE_URL}/initialize", requestContent);
                var responseContent = await response.Content.ReadAsStringAsync();
                return responseContent;
            }
            catch (System.Exception ex)
            {
                var errorMessage = ex.InnerException?.Message;
                throw;
            }
        }

        public async Task<string> verifyTranasction(string reference)
        {
            try
            {
                _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _paystackSecretKey);
                
                var response = await _httpClient.GetAsync($"{PAYSTACK_BASE_URL}/verify/{reference}");
                var responseContent = await response.Content.ReadAsStringAsync();
                return responseContent;
            }
            catch (System.Exception)
            {
                
                throw;
            }
        }
    }
}
