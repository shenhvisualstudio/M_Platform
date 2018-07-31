//
//文件名：    DES_Decrypt.aspx.cs
//功能描述：  DES解密
//创建时间：  2016/03/22
//作者：      
//修改时间：  暂无
//修改描述：  暂无
//
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Newtonsoft.Json;
using Leo;
using YGSoft.IPort.Data;
using ServiceInterface.Common;
using System.Security.Cryptography;
using System.IO;
using System.Text;


namespace M_Platform.Encryption
{
    public partial class DES_Decrypt : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //身份校验
            if (!InterfaceTool.IdentityVerify(Request))
            {
                Json = JsonConvert.SerializeObject(new DicPackage(false, null, "身份认证错误！").DicInfo());
                return;
            }
            
            string strKey = Request.Params["Key"];
            string strCipherText = HttpUtility.UrlDecode(Request.Params["Value"]);
            string password2 = DES_IV.IV;
            string cipher = string.Empty;

            try
            {
                if (strKey == null || strCipherText == null)
                {
                    Json = JsonConvert.SerializeObject(new DicPackage(false, null, "参数错误，DES解密失败！").DicInfo());
                    return;
                }

                char[] key = new char[8];
                if (strKey.Length > 8)
                {
                    strKey = strKey.Remove(8);
                }
                strKey.CopyTo(0, key, 0, strKey.Length);

                char[] iv = new char[8];
                if (password2.Length > 8)
                {
                    password2 = password2.Remove(8);
                }
                password2.CopyTo(0, iv, 0, password2.Length);

                strCipherText = strCipherText.Replace(" ", "+");
                SymmetricAlgorithm serviceProvider = new DESCryptoServiceProvider();
                serviceProvider.Key = Encoding.ASCII.GetBytes(key);
                serviceProvider.IV = Encoding.ASCII.GetBytes(iv);

                byte[] contentArray = Convert.FromBase64String(strCipherText);
                MemoryStream memoryStream = new MemoryStream(contentArray);
                CryptoStream cryptoStream = new CryptoStream(memoryStream, serviceProvider.CreateDecryptor(), CryptoStreamMode.Read);
                StreamReader streamReader = new StreamReader(cryptoStream);

                Json = JsonConvert.SerializeObject(new DicPackage(true, streamReader.ReadToEnd(), null).DicInfo());

                streamReader.Dispose();
                cryptoStream.Dispose();
                memoryStream.Dispose();
                serviceProvider.Clear();         
            }
            catch (Exception ex)
            {
                Json = JsonConvert.SerializeObject(new DicPackage(false, null, string.Format("{0}：DES解密数据发生异常。{1}", ex.Source, ex.Message)).DicInfo());
            }
        }
        protected string Json;
    }
}