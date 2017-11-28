using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Payout.CoreAPI.Models;

namespace Payout.CoreAPI
{
    public class OktaRequests
    {
        public static Dictionary<string, string> getGroupsFromUserOkta(string OktaURL, string OktaUserId, string APiKey)
        {
            Dictionary<string, string> UserRoles = new Dictionary<string, string>();
            
            string URL = $"/api/v1/users/{OktaUserId}/groups";
            var obj = GetJsonObj(OktaURL, APiKey, URL);

            try
            {
                IEnumerable<dynamic> dynVar1 = ((Newtonsoft.Json.Linq.JArray)obj).AsEnumerable().Where(f => f.ToString().Contains("OKTA_GROUP") && f.ToString().Contains("Payout")).ToList();
                foreach (IEnumerable<dynamic> itemDynvar in dynVar1)
                {
                    //Get Group Name
                    dynVar1 = itemDynvar.Where(f => f.ToString().Contains("profile")).FirstOrDefault();
                    string RoleName = ((Newtonsoft.Json.Linq.JProperty)((Newtonsoft.Json.Linq.JProperty)dynVar1).Value.First).Value.ToString();

                    //Get Group Id
                    dynVar1 = itemDynvar.Where(f => f.ToString().Contains("id")).FirstOrDefault();
                    string RoleId = ((Newtonsoft.Json.Linq.JProperty)dynVar1).Value.ToString();
                    UserRoles.Add(RoleName, RoleId);
                }
            }
            catch { }


            return UserRoles;
        }

        //Get user by Id or login name
        public static Dictionary<string, string> GetUserFromOkta(string OktaURL, string APiKey, string UserInput)
        {
            Dictionary<string, string> OktaUser = new Dictionary<string, string>();
            string URL = $"/api/v1/users/{UserInput}";            
            var obj = GetJsonObj(OktaURL, APiKey, URL);
            
            //CODE TO RETRIEVE USER AND ID FROM OKTA JSON STRING
            try
            {                
                string OktaUserId = ((Newtonsoft.Json.Linq.JValue)((Newtonsoft.Json.Linq.JProperty)((Newtonsoft.Json.Linq.JContainer)obj).First).Value).Value.ToString();
                
                IEnumerable<dynamic> dynVar = ((Newtonsoft.Json.Linq.JContainer)obj).Children().AsEnumerable();
                dynVar = dynVar.Where(f => f.ToString().Contains("profile")).FirstOrDefault();
                dynVar = ((Newtonsoft.Json.Linq.JProperty)dynVar).Value.AsEnumerable().Where(f => f.ToString().Contains("email")).FirstOrDefault();
                string UserName = ((Newtonsoft.Json.Linq.JValue)((Newtonsoft.Json.Linq.JProperty)dynVar).Value).Value.ToString();

                OktaUser.Add(UserName, OktaUserId);                
            }
            catch (Exception ex)
            {

            }

            return OktaUser;
        }
        
        public static string GetGroupIdFromOkta(string OktaURL, string APiKey, string GroupName)
        {
            string OktaRoleId = "";
            string URL = $"/api/v1/groups?filter=type eq \"OKTA_GROUP\"";
            var obj = GetJsonObj(OktaURL, APiKey, URL);
                        
            try
            {
                IEnumerable<dynamic> dynVar = ((Newtonsoft.Json.Linq.JArray)obj).AsEnumerable().Where(f => f.ToString().Contains("Payout")).ToList();

                foreach (IEnumerable<dynamic> itemDynvar in dynVar)
                {
                    //Get Group Name
                    dynamic dynGroupParameter = itemDynvar.Where(f => f.ToString().Contains("profile")).FirstOrDefault();
                    if (((Newtonsoft.Json.Linq.JProperty)((Newtonsoft.Json.Linq.JProperty)dynGroupParameter).Value.First).Value.ToString() == GroupName)
                    {
                        dynGroupParameter = itemDynvar.Where(f => f.ToString().Contains("id")).FirstOrDefault();
                        return ((Newtonsoft.Json.Linq.JValue)((Newtonsoft.Json.Linq.JProperty)dynGroupParameter).Value).Value.ToString();
                    }
                }
            }
            catch (Exception ex)
            {

            }

            return OktaRoleId;
        }
        
        public static bool AddGroup(string OktaURL, string APiKey, string GroupName)
        {
            try
            {
                string URL = $"/api/v1/groups";
                string JsonContent = string.Format(@"{{
                                                      ""profile"": {{
                                                        ""name"": ""{0}"",
                                                        ""description"": ""RS Group""
                                                      }}
                                                    }}", GroupName);


                var response = PostOktaAPI(OktaURL, APiKey, URL, JsonContent);
            }
            catch(Exception ex)
            {
                return false;
            }
            return true;
        }

        public static bool AssignGroupToApp(string OktaURL, string APiKey, string groupId, string appId)
        {
            try
            {
                string URL = $"/api/v1/apps/{appId}/groups/{groupId}";
                var response = PutOktaAPI(OktaURL, APiKey, URL);
            }
            catch (Exception ex)
            {
                return false;
            }
            return true;
        }

        public static bool UpdateGroupName(string OktaURL, string APiKey, string OldGroup, string NewGroup)
        {
            try
            {
                OldGroup = GetGroupIdFromOkta(OktaURL, APiKey, OldGroup);
                string URL = $"/api/v1/groups/{OldGroup}";
                string JsonContent = string.Format(@"{
                                                      ""profile"": {
                                                        ""name"": ""{0}"",
                                                        ""description"": ""RS Group""
                                                      }
                                                    }", NewGroup);
                var response = PutOktaAPI(OktaURL, APiKey, URL, JsonContent);
            }
            catch
            {
                return false;
            }
            return true;
        }

        public static bool DeleteGroup(string OktaURL, string APiKey, string GroupName)
        {
            try
            {
                string GroupId = GetGroupIdFromOkta(OktaURL, APiKey, GroupName);
                string URL = $"/api/v1/groups/{GroupId}";
                var response = DeleteOktaAPI(OktaURL, APiKey, URL);
            }
            catch
            {
                return false;
            }
            return true;
        }
        
        public static bool RemoveUserFromGroup(string OktaURL, string APiKey, string GroupId, string UserId)
        {
            try
            {
                string URL = $"/api/v1/groups/{GroupId}/users/{UserId}";
                var response = DeleteOktaAPI(OktaURL, APiKey, URL);
            }
            catch
            {
                return false;
            }
            return true;
        }

        public static bool AddUserToGroup(string OktaURL, string APiKey, string GroupId, string UserId)
        {
            try
            {
                string URL = $"/api/v1/groups/{GroupId}/users/{UserId}";
                var response = PutOktaAPI(OktaURL, APiKey, URL);
                if(response.StatusCode.ToString() == "NotFound")
                    return false;
            }
            catch
            {
                return false;
            }
            return true;
        }

        public static bool CreateUserInOkta(string OktaURL, string APiKey, PayoutUser User)
        {
            try
            {
                string URL = $"/api/v1/users?activate=true";
                string JsonContent = string.Format(@"{{
                                                      ""profile"": {{
                                                        ""firstName"": ""{0}"",
                                                        ""lastName"": ""{1}"",
                                                        ""email"": ""{2}"",
                                                        ""login"": ""{3}""
                                                      }}
                                                    }}", User.FirstName, User.LastName, User.Email, User.Email);


                var response = PostOktaAPI(OktaURL, APiKey, URL, JsonContent);
            }
            catch (Exception ex)
            {
                return false;
            }
            return true;
        }

        private static object GetJsonObj(string OktaURL, string APiKey, string URL)
        {            
            var response = GetOktaAPI(OktaURL, APiKey, URL);
            string res = "";
            using (HttpContent content = response.Content)
            {
                Task<string> result = content.ReadAsStringAsync();
                res = result.Result;
            }

            var obj = JsonConvert.DeserializeObject(res, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto
            });

            return obj;
        }

        private static HttpResponseMessage GetOktaAPI(string OktaURL, string APiKey, string URL)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(OktaURL);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Add("Authorization", "SSWS " + APiKey);

                return client.GetAsync(URL).Result;
            }
        }

        private static HttpResponseMessage PostOktaAPI(string OktaURL, string APiKey, string URL, string JSONBodyContent = "")
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(OktaURL);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Add("Authorization", "SSWS " + APiKey);
                                
                using (HttpContent httpContent = new StringContent(JSONBodyContent, Encoding.UTF8, "application/json"))
                {
                    httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                    return client.PostAsync(URL, httpContent).Result;
                }
            }
        }

        private static HttpResponseMessage DeleteOktaAPI(string OktaURL, string APiKey, string URL)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(OktaURL);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Add("Authorization", "SSWS " + APiKey);

                return client.DeleteAsync(URL).Result;
            }
        }

        private static HttpResponseMessage PutOktaAPI(string OktaURL, string APiKey, string URL, string JSONBodyContent = "")
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(OktaURL);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Add("Authorization", "SSWS " + APiKey);

                using (HttpContent httpContent = new StringContent(JSONBodyContent, Encoding.UTF8, "application/json"))
                {
                    httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                    return client.PutAsync(URL, httpContent).Result;
                }
            }
        }
    }
}
