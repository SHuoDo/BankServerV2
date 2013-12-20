using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Threading.Tasks;
using BankServer_HTTP;

public struct GetProfileResultType
{
    public UserProfile profile;
    public ResultCodeType status;
};


namespace BankServer_HTTP
{
    class RequestWorker
    {
        /*
        * Authenticate user
        */
        public static Boolean authenticateUser(DataBaseAdapter DBHandler, string authenticationString)
        {

            int count = DBHandler.Count("*", "Client WHERE AuthString='" + authenticationString + "'");
            /*//JT:HACK Start
            if (count == 0)
            {
                DBHandler.Insert("authenticationList", "(authenticationString)", "('"+authenticationString+"')"); 
                Console.WriteLine("XXXX JT:HACK - ServerWorker::authenticateUser - Insertion successfull");
                Console.WriteLine("XXXX JT:HACK - ServerWorker::authenticateUser - User not authenticated but was added to database");
            }
            //JT:HACK End */

            if (DBHandler.Count("*", "Client WHERE AuthString='" + authenticationString + "'") == 1)
            {
                Console.WriteLine("ServerWorker::authenticateUser - User authenticated with {0}", authenticationString);
                DBHandler.Backup();
                return true;
            }
            Console.WriteLine("ServerWorker::authenticateUser - Could not authenticate user. DB Query returned count of {0}", count);
            return false;
        }

        /*
         * create new user profile
         */
        public static ResultCodeType createNewProfile(DataBaseAdapter DBHandler, UserProfile P)
        {
            if (DBHandler.Count("*", "Client WHERE Email='" + P.email + "'") == 0)
            {
                string profile = "Client";
                string items = "(UserName, Email, Password, FirstName, LastName," +
                    "address, PhoneNumber, " +
                    "accountNum, Balance, AuthString)";
                string values = "('" + P.username + "', '" + P.email + "', '" + P.password + "', '" + P.firstName + "', '" + P.lastName + "', '" +
                    P.address + "', '" +
                    P.phoneNumber + "', '" + P.accountNum + "', '" + P.acctBalance + "', '" + P.authenticationString + "')";

                DBHandler.Insert(profile, items, values);
                //DBHandler.Insert("authenticationList", "(authenticationString)", "('" + P.authenticationString + "')");

                return ResultCodeType.RESULT_CREATE_PROFILE_SUCCESS;
            }

            else
            {
                return ResultCodeType.ERROR_CREATE_PROFILE_ACCOUNT_EXISTS;
            }
        }


        /*
        * Get user profile
        */
        public static GetProfileResultType getUserProfile(DataBaseAdapter DBHandler, string userNo)
        {
            GetProfileResultType reply = new GetProfileResultType();
            reply.status = ResultCodeType.ERROR_UNKNOWN;

            List<string>[] list = DBHandler.Select("userProfile", "userNo", "" + userNo);
            if (list.Length == 1)
            {
                if (list[0].Count() == (int)UserProfileEnum.NUM_PROFILE_DATA_ITEMS)
                {
                    string String = "";
                    int Int = 1;
                    bool Bool = false;
                    double Double = 0.1;

                    object Profile = new UserProfile();
                    PropertyInfo[] properties = Profile.GetType().GetProperties();
                    int i;
                    for (i = 0; i < properties.Length; i++)
                    {
                        Console.WriteLine(properties[i].GetValue(Profile, null).ToString());
                        if (properties[i].GetType() == String.GetType())
                        {
                            properties[i].SetValue(Profile, (string)list[0][i], null);

                        }
                        else if (properties[i].GetType() == Double.GetType())
                        {
                            properties[i].SetValue(Profile, Convert.ToDouble(list[0][i]), null);
                        }
                        else if (properties[i].GetType() == Int.GetType())
                        {
                            properties[i].SetValue(Profile, Convert.ToInt32(list[0][i]), null);

                        }
                        else if (properties[i].GetType() == Bool.GetType())
                        {
                            properties[i].SetValue(Profile, Convert.ToBoolean(list[0][i]), null);
                        }
                        reply.status = ResultCodeType.UPDATE_USER_PROFILE_SUCCESS;
                        reply.profile = (UserProfile)Profile;
                    }
                }
                else
                {
                    Console.WriteLine("ServerWorker::getUserProfile - Error: Did not receive extpected number of data items from server. Received: {}, Expected: {}", list[0].Count(), (int)UserProfileEnum.NUM_PROFILE_DATA_ITEMS);
                }

            }

            else
            {
                Console.WriteLine("ServerWorker::getUserProfile - Error: Database query returned more than one record. Number Received: {}", list.Length);
            }
            return reply;

        }

        public static ResultCodeType processPaymentRequest(DataBaseAdapter DBHandler, transaction transDetails)
        {
            ResultCodeType result = new ResultCodeType();
            return result;
        }

    }
}

