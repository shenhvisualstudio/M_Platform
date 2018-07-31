//
//文件名：    Login.aspx.cs
//功能描述：  用户登陆
//创建时间：  2016/03/05
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
using ServiceInterface.Common;
using YGSoft.IPort.Data;

namespace M_Platform.Entrance
{
    public partial class Login : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //身份校验
            if (!InterfaceTool.IdentityVerify(Request))
            {
                Json = JsonConvert.SerializeObject(new DicPackage(false, null, "身份认证错误！").DicInfo());
                return;
            }

            //账号（手机号或Iport账号）
            string strAccount = Request.Form["Account"];
            //密码
            string strPassword = Request.Form["Password"];

            ////账号（手机号或Iport账号）
            //string strAccount = Request.Params["Account"];
            ////密码
            //string strPassword = Request.Params["Password"];

            AppLog log = new AppLog(Request);
            log.strAccount = strAccount;
            log.strBehavior = "用户登陆";
            log.strBehaviorURL = "/Entrance/Login.aspx";

            try
            {
                if (strAccount == null || strPassword == null)
                {
                    Json = JsonConvert.SerializeObject(new DicPackage(false, null, "参数错误，登陆失败！").DicInfo());
                    return;
                }

                ILogin iLogin = new ILogin(log);
             
                //手机号登录&&Iport账号登录
                if (TokenTool.VerifyMobile(strAccount) == "ture")
                {
                    Json = iLogin.GetInfoByMobileLogin(strAccount, strPassword);
                }//手机号登陆
                else
                {
                    Json = iLogin.GetInfoByUserNameLogin(strAccount, strPassword);
                }//Iport账号登录        
            }
            catch (Exception ex)
            {
                Json = JsonConvert.SerializeObject(new DicPackage(false, null, string.Format("{0}：用户登陆数据发生异常。{1}", ex.Source, ex.Message)).DicInfo());
                log.LogCatalogFailure(string.Format("{0}：用户登陆数据发生异常。{1}", ex.Source, ex.Message));
            }
        }
        protected string Json;
    }
}