using RestSharp;
using System;
using System.Collections.Generic;
using System.Text;
using TenmoClient.Models;

namespace TenmoClient
{
    public class APIService
    {
        private readonly string API_URL = "";
        private readonly RestClient client = new RestClient();
        private ApiUser user = new ApiUser();

        //WTF IS THIS EVEN DOING?
        //Maybe checking to see if a user is logged or not? IDK MAN
        public bool LoggedIn { get { return !string.IsNullOrWhiteSpace(user.Token); } }


        public decimal GetBalance(int userId)
        {
            RestRequest request = new RestRequest(API_URL + "account/balance");
            IRestResponse<Account> response = client.Get<Account>(request);

            if (response.ResponseStatus != ResponseStatus.Completed)
            {
                throw new Exception($"Error unable to reach server {response.ErrorException}");
            }
            else if (!response.IsSuccessful)
            {
                throw new Exception($"Error occured - received non-success response: " + (int)response.StatusCode);
            }
            else
            {
                return response.Data.Balance;
            }
        }

      
        //CTOR
        public APIService(string api_url)
        {
            API_URL = api_url;
        }
    }
    //DANGER ZONE
}
