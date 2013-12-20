using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Net.Json;
using System.Runtime.Serialization;
using Newtonsoft.Json.Linq;
using System.Net.Security;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Linq;
using System.Threading.Tasks;
using BankServer_HTTP;

public enum ResultCodeType
{
    ERROR_UNKNOWN = -1,
    RESULT_CREATE_PROFILE_SUCCESS = 0,
    ERROR_CREATE_PROFILE_USERNAME_TAKEN = 1,
    ERROR_CREATE_PROFILE_UNSUPPORTED_INSTITUTION = 2,
    ERROR_CREATE_PROFILE_INVALID_BANK_ACCT_NUM = 3,
    ERROR_CREATE_PROFILE_INVALID_BANK_CODE = 4,
    ERROR_CREATE_PROFILE_ACCOUNT_EXISTS = 5,
    UPDATE_USER_PROFILE_SUCCESS = 6,
    ERROR_UPDATE_USER_PROFILE = 7,
    //all new codes should be placed above this line
    ERROR_CREATE_PROFILE_MAX
};

/* Outgoing transaction message codes sent to mobile devices and web clients */
public enum clientOutgoingCodeEnum
{
    OUT_CODE_INVALID = -1,
    OUT_CODE_LOGIN_SUCCESS = 0,
    OUT_CODE_LOGIN_FAILURE = 1,
    OUT_CODE_SIGN_UP_SUCCESS = 2,
    OUT_CODE_SIGN_UP_FAILURE = 3,
    //OUT_CODE_SEND_USER_PROFILE_SUCCESS = 4,
    //OUT_CODE_SEND_USER_PROFILE_FAILURE = 5,
    //all new codes should be placed above this line
    OUT_CODE_MAX
};

/* Incoming transaction message codes received from mobile devices and web clients */
public enum clientIncomingCodeEnum
{
    IN_CODE_INVALID = -1,
    IN_CODE_SIGN_UP_REQ = 0,
    IN_CODE_LOGIN_REQ = 1,
    //IN_CODE_GET_USER_PROFILE = 2,
    IN_CODE_PROCESS_PAYMENT_REQ = 2,
    //all new codes should be placed above this line
    IN_CODE_MAX
};


namespace BankServer_HTTP
{

    public class BankServerRequestHandler
    {
 
        public static void handleGetRequest(BankServerConnection p)
        {
            string url = p.request.Url.OriginalString;

            string form = "";
            
            try
            {
                using (StreamReader sr = new StreamReader("WebRegistrationForm.html"))
                {
                    form = sr.ReadToEnd();
                    //Console.WriteLine(line);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("The file could not be read:");
                Console.WriteLine(e.Message);
            }
       
            byte[] formAsByte = System.Text.Encoding.UTF8.GetBytes(form);

            p.response.ContentLength64 = formAsByte.Length;
            p.response.OutputStream.Write(formAsByte, 0, formAsByte.Length);

            p.response.Close();
        }


        public static void handlePostRequest(BankServerConnection p)
        {
            DataBaseAdapter DBHandler = new DataBaseAdapter();

            //create JSON object
            JsonObjectCollection defineResponse = new JsonObjectCollection();
            JsonNumericValue messageType;         

            //parse the input data
            System.IO.Stream requestBody = p.request.InputStream;
            System.Text.Encoding encoding = p.request.ContentEncoding;
            System.IO.StreamReader reader = new System.IO.StreamReader(requestBody, encoding);
            string requestBodyInString = reader.ReadToEnd();

            JObject requestBodyInJson = JObject.Parse(requestBodyInString);
            int transactionCode = (int)requestBodyInJson.SelectToken("messageType");

            //Determine anad handle the received transaction code
            switch (transactionCode)
            {
                /*
                 * handle new user sign-up request
                 */
                case ((int)clientIncomingCodeEnum.IN_CODE_SIGN_UP_REQ):
                    UserProfile newProfile = new UserProfile();
                    JsonStringValue details;

                    //Retrieve encapsulated JSON objects from message
                    string first = (string)requestBodyInJson.SelectToken("firstname");
                    string last = (string)requestBodyInJson.SelectToken("lastname");
                    string userName = (string)requestBodyInJson.SelectToken("username");
                    string accountNum = (string)requestBodyInJson.SelectToken("accountNum");
                    string password = (string)requestBodyInJson.SelectToken("password");
                    string email = (string)requestBodyInJson.SelectToken("email");
                    string address = (string)requestBodyInJson.SelectToken("address");
                    string phone = (string)requestBodyInJson.SelectToken("phone");
                    double deposit = (double)requestBodyInJson.SelectToken("DepositAmount");
                    string authenticationString = userName + password;

                    //Populate the newProfile object with the information received from the client
                    newProfile.accountNum = accountNum;
                    newProfile.username = userName;
                    newProfile.password = password;
                    newProfile.firstName = first;
                    newProfile.lastName = last;
                    newProfile.address = address;
                    newProfile.email = email;
                    newProfile.phoneNumber = phone;
                    newProfile.authenticationString = authenticationString;

                    //pass the populated newProfile information to ServerWorker to try and create a new profile
                    //and build response message to client based on the return code receiveed from ServerWorker
                    if (RequestWorker.createNewProfile(DBHandler, newProfile) == ResultCodeType.RESULT_CREATE_PROFILE_SUCCESS)
                    {
                        messageType = new JsonNumericValue("code", (int)clientOutgoingCodeEnum.OUT_CODE_SIGN_UP_SUCCESS);
                        details = new JsonStringValue("details", "User account created");
                    }
                    else
                    {
                        messageType = new JsonNumericValue("code", (int)clientOutgoingCodeEnum.OUT_CODE_SIGN_UP_FAILURE);
                        details = new JsonStringValue("details", "Could not create profile. The email provided is already registered");
                    }

                    defineResponse.Add(messageType);
                    defineResponse.Add(details);
                    break;

                /*
                * handle get user profile request
                */
                case ((int)clientIncomingCodeEnum.IN_CODE_LOGIN_REQ):
                    //Retrieve encapsulated JSON objects from message
                    string loginUserName = (string)requestBodyInJson.SelectToken("userName");
                    string loginPassword = (string)requestBodyInJson.SelectToken("password");
                    string authString = loginUserName + loginPassword;

                    GetProfileResultType UserProf = RequestWorker.getUserProfile(DBHandler, loginUserName);
                    //if (UserProf.status == ResultCodeType.UPDATE_USER_PROFILE_SUCCESS)
                    if (RequestWorker.authenticateUser(DBHandler, authString))
                    {
                        //populate messageType fields 
                        messageType = new JsonNumericValue("code", (int)clientOutgoingCodeEnum.OUT_CODE_LOGIN_SUCCESS);
                        details = new JsonStringValue("details", "Log in success");

                        //populate User fields
                        JsonStringValue accountNumReturn = new JsonStringValue("userNo", (string)UserProf.profile.accountNum);
                        JsonNumericValue acctBalanceReturn = new JsonNumericValue("acctBalance", (double)UserProf.profile.acctBalance);
                        JsonStringValue userNameReturn = new JsonStringValue("username", (string)UserProf.profile.username);
                        JsonStringValue firstNameReturn = new JsonStringValue("firstName", (string)UserProf.profile.firstName);
                        JsonStringValue lastNameReuturn = new JsonStringValue("lastName", (string)UserProf.profile.lastName);
                        JsonStringValue emailReturn = new JsonStringValue("email", (string)UserProf.profile.email);
                        JsonStringValue addressReturn = new JsonStringValue("address1", (string)UserProf.profile.address);
                        JsonStringValue phoneNumberReturn = new JsonStringValue("phoneNumber", (string)UserProf.profile.phoneNumber);

                        JsonObjectCollection clientInfo = new JsonObjectCollection(); ;
                        clientInfo.Add(accountNumReturn);
                        clientInfo.Add(acctBalanceReturn);
                        clientInfo.Add(userNameReturn);
                        clientInfo.Add(firstNameReturn);
                        clientInfo.Add(lastNameReuturn);
                        clientInfo.Add(emailReturn);
                        clientInfo.Add(addressReturn);
                        clientInfo.Add(phoneNumberReturn);

                        defineResponse.Add(messageType);
                        defineResponse.Add(details);
                        defineResponse.Add(clientInfo);
                    }
                    else
                    {
                        messageType = new JsonNumericValue("code", (int)clientOutgoingCodeEnum.OUT_CODE_LOGIN_FAILURE);
                        details = new JsonStringValue("details", "Server error - Could not get profile data");

                        defineResponse.Add(messageType);
                        defineResponse.Add(details);
                    }
                    break;

                case ((int)clientIncomingCodeEnum.IN_CODE_PROCESS_PAYMENT_REQ):
                    break;
            }


            //finalize outgoing JSON message
            JsonObjectCollection completeResponse = new JsonObjectCollection();
            completeResponse.Add(defineResponse);

            byte[] buffer = JsonStringToByteArray(completeResponse.ToString());

            p.response.ContentLength64 = buffer.Length;
            p.response.OutputStream.Write(buffer, 0, buffer.Length);

            p.response.Close();
        }

        public static byte[] JsonStringToByteArray(string jsonString)
        {
            var encoding = new UTF8Encoding();
            return encoding.GetBytes(jsonString.Substring(1, jsonString.Length - 2));
        }

        public static JsonObjectCollection insert(JsonObjectCollection obj, JsonObject item, JsonObject newItem)
        {
            obj.Remove(item);
            obj.Add(newItem);
            return obj;
        }
    }
}


