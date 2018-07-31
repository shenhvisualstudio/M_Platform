using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;

namespace M_Platform.Common.SMS
{
    public class ALiSmsSender
    {
        public static Tuple<bool, string> PostRequest(string paramData)
        {

            string strURL = "http://168.100.1.123/cloudsms/sender";
            System.Net.HttpWebRequest request;
                
            System.Net.ServicePointManager.DefaultConnectionLimit = 200;

            request = (System.Net.HttpWebRequest)System.Net.HttpWebRequest.Create(strURL);
            request.ContentType = "application/json; charset=UTF-8";
            request.Method = "POST";
            request.Headers.Add("Authorization", "Basic " + Convert.ToBase64String(Encoding.GetEncoding("utf-8").GetBytes("portapp:w321lXfEtVv0FTN4")));
            byte[] LoadData;
            //将URL编码后的字符串转化为字节
            LoadData = System.Text.Encoding.UTF8.GetBytes(paramData);
            //设置请求的 ContentLength 
            request.ContentLength = LoadData.Length;
            //获得请求流


            Stream newStream = request.GetRequestStream();
            newStream.Write(LoadData, 0, LoadData.Length);
            newStream.Close();


            HttpWebResponse response = null;
            try
            {
                response = (HttpWebResponse)request.GetResponse();
                if (response.StatusDescription != "OK")
                {
                    newStream = request.GetRequestStream();
                    newStream.Write(LoadData, 0, LoadData.Length);
                    newStream.Close();
                    return new Tuple<bool, string>(false, LoadData.ToString());
                }

                return new Tuple<bool, string>(true, string.Empty);
            }
            catch (Exception ex)
            {
                return new Tuple<bool, string>(false, ex.Message);
            }
            finally {
                if (response != null) {

                    response.Close();
                }
                
            }
        }
    }
}