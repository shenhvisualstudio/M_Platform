//
//文件名：    RetrievePassword.aspx.cs
//功能描述：  找回密码
//创建时间：  2016/03/25
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
namespace M_Platform.Entrance
{
    public partial class RetrievePassword : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //身份校验
            if (!InterfaceTool.IdentityVerify(Request))
            {
                Json = JsonConvert.SerializeObject(new DicPackage(false, null, "身份认证错误！").DicInfo());
                return;
            }

            //手机号码
            string strMobile = Request.Form["Mobile"];
            //密码1
            string strPassword1 = Request.Form["Password1"];
            //密码2
            string strPassword2 = Request.Form["Password2"];

            AppLog log = new AppLog(Request);
            log.strBehavior = "找回密码";
            log.strBehaviorURL = "/Entrance/RetrievePassword.aspx";

            try
            {
                if (strMobile == null || strPassword1 == null || strPassword2 == null)
                {
                    Json = JsonConvert.SerializeObject(new DicPackage(false, null, "参数错误，修改密码失败！").DicInfo());
                    return;

                }

                string strSql =
                    string.Format("select code_user from TB_SYS_USERINFO where mobile ='{0}'", strMobile);
                var dt = new Leo.Oracle.DataAccess(RegistryKey.KeyPathIport).ExecuteTable(strSql);
                if (dt.Rows.Count <= 0)
                {
                    Json = JsonConvert.SerializeObject(new DicPackage(false, null, "用户名错误！").DicInfo());
                    return;
                }
        
                //校验新密码
                if (strPassword1.Length < 6 || strPassword1.Length > 32 || strPassword2.Length < 6 || strPassword2.Length > 32)
                {
                    Json = JsonConvert.SerializeObject(new DicPackage(false, null, "密码应为6~32个字符！").DicInfo());
                    return;
                }
                if (!strPassword1.Equals(strPassword2))
                {
                    Json = JsonConvert.SerializeObject(new DicPackage(false, null, "新密码不一致！").DicInfo());
                    return;
                }

                //string strXmlParams =
                //         string.Format("<params><usercode>{0}</usercode><oldpassword>{1}</oldpassword><newpassword>{2}</newpassword></params>",
                //                        strCodeUser, strOldPassword, strNewPassword1);

                //ServiceIportUser.WebServiceUserSoapClient service = new ServiceIportUser.WebServiceUserSoapClient();
                //string strReturnMessage = service.UpdatePassword(strXmlParams, 1);
                //RetureMessage retureMessage = JsonConvert.DeserializeObject<RetureMessage>(strReturnMessage);

                //if ((bool)retureMessage.Value == false)
                //{
                //    Json = JsonConvert.SerializeObject(new DicPackage(false, null, retureMessage.Message).DicInfo());
                //}
                //else
                //{
                //    Json = JsonConvert.SerializeObject(new DicPackage(true, null, "密码重置成功！").DicInfo());
                //}

                Json = JsonConvert.SerializeObject(new DicPackage(true, null, "接口调式中！").DicInfo());

                log.LogCatalogSuccess();
            }
            catch (Exception ex)
            {
                Json = JsonConvert.SerializeObject(new DicPackage(false, null, string.Format("{0}：找回密码数据发生异常。{1}", ex.Source, ex.Message)).DicInfo());
                log.LogCatalogFailure(string.Format("{0}：找回密码数据发生异常。{1}", ex.Source, ex.Message));
            }
        }
        protected string Json;
    }
}
//strMobile = "18036600293";
//strPassword1 = "123456";
//strPassword2 = "123456";