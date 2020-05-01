using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Android.Util;
using Javax.Net.Ssl;
using ListWithJson.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace ListWithJson
{
    public class RestService
    {
        HttpClient _client;
        User _user;

        public RestService()
        {
            _client = new HttpClient(GetInsecureHandler());
            Debug.AutoFlush = true;
        }

        public RestService(User user) : this()
        {
            string authData = $"{user.Email}:{user.Password}";
            string base64AuthData = Convert.ToBase64String(Encoding.UTF8.GetBytes(authData));
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", base64AuthData);
            _user = user;
        }

        public async Task<User> Authenticate(User user, bool isCreate)
        {
            var uri = new Uri(string.Format(Constants.ApiAuthenticationUrl, isCreate ? "register" : "login"));
            var json = JsonConvert.SerializeObject(user);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            try
            {
                var response = await _client.PostAsync(uri, content);
                if (response.IsSuccessStatusCode)
                {
                    Log.Debug("RestService", "Succesfully authenticated. " + (isCreate ? "New account created." : ""));
                    string contentString = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<User>(contentString);
                }
                else
                {
                    Log.Error("RestService", $"Authentication ({(isCreate ? "creating new account" : "signing in")}) failed" +
                        $" {response.StatusCode}: {response.ReasonPhrase}");
                    return null;
                }
            }
            catch (HttpRequestException e)
            {
                Log.Error("RestService", e.Message);
                return null;
            }
            catch (Exception e)
            {
                Log.Error("RestService", e.Message);
                return null;
            }
        }

        public async Task<List<Product>> Get()
        {
            var products = new List<Product>();
            var uri = new Uri(string.Format(Constants.ApiProductsUrl, ""));

            try
            {
                var response = await _client.GetAsync(uri);
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    products = new List<Product>(JsonConvert.DeserializeObject<Product[]>(content));
                }
                else
                {
                    Log.Error("RestService", $"Get response status code {response.StatusCode}: {response.ReasonPhrase}");
                }
            }
            catch (HttpRequestException e)
            {
                Log.Error("RestService", e.Message);
                return null;
            }
            catch (Exception e)
            {
                Log.Error("RestService", e.Message);
            }

            return products;
        }

        public async Task<Product> Post(Product product)
        {
            var uri = new Uri(string.Format(Constants.ApiProductsUrl, string.Empty));

            var json = JsonConvert.SerializeObject(product);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            try
            {
                var response = await _client.PostAsync(uri, content);
                if (response.IsSuccessStatusCode)
                {
                    Log.Debug("RestService", "Item succesfully saved.");
                    string contentString = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<Product>(contentString);
                }
                else
                {
                    Log.Error("RestService", $"Post response status code {response.StatusCode}: {response.ReasonPhrase}");
                    return null;
                }
            }
            catch (HttpRequestException e)
            {
                Log.Error("RestService", e.Message);
                return null;
            }
            catch (Exception e)
            {
                Log.Error("RestService", e.Message);
                return null;
            }
        }

        public async Task Delete(int id)
        {
            var uri = new Uri(string.Format(Constants.ApiProductsUrl, id.ToString()));
            
            try
            {
                var response = await _client.DeleteAsync(uri);
                if (response.IsSuccessStatusCode)
                    Log.Debug("RestService", "Item succesfully deleted.");
            }
            catch (HttpRequestException e)
            {
                Log.Error("RestService", e.Message);
            }
            catch (Exception e)
            {
                Log.Error("RestService", e.Message);
            }
        }

        public HttpClientHandler GetInsecureHandler()
        {
            var handler = new HttpClientHandler();
            handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) =>
            {
                if (cert.Issuer.Equals("CN=localhost"))
                    return true;
                return errors == System.Net.Security.SslPolicyErrors.None;
            };
            return handler;
        }
    }
}