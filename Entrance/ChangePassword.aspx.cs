//
//文件名：    ChangePassword.aspx.cs
//功能描述：  修改密码
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
using YGSoft.IPort.MobileManage.Interface;

namespace M_Platform.Entrance
{
    public partial class ChangePassword : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //身份校验
            if (!InterfaceTool.IdentityVerify(Request))
            {
                Json = JsonConvert.SerializeObject(new DicPackage(false, null, "身份认证错误！").DicInfo());
                return;
            }

            //用户编码
            string strCodeUser = Request.Form["CodeUser"];
            //旧密码
            string strOldPassword = Request.Form["OldPassword"];
            //新密码1
            string strNewPassword1 = Request.Form["NewPassword1"];
            //新密码2
            string strNewPassword2 = Request.Form["NewPassword2"];

            ////用户编码
            //string strCodeUser = Request.Params["CodeUser"];
            ////旧密码
            //string strOldPassword = Request.Params["OldPassword"];
            ////新密码1
            //string strNewPassword1 = Request.Params["NewPassword1"];
            ////新密码2
            //string strNewPassword2 = Request.Params["NewPassword2"];

            AppLog log = new AppLog(Request);
            log.strBehavior = "修改密码";
            log.strBehaviorURL = "/Entrance/ChangePassword.aspx";

            try
            {
                if (strCodeUser == null || strOldPassword == null || strNewPassword1 == null || strNewPassword2 == null)
                {
                    Json = JsonConvert.SerializeObject(new DicPackage(false, null, "参数错误，修改密码失败！").DicInfo());
                    return;

                }

                string strSql =
                    string.Format("select password from TB_SYS_USER t where code_user ='{0}'", strCodeUser);
                var dt = new Leo.Oracle.DataAccess(RegistryKey.KeyPathIport).ExecuteTable(strSql);
                if (dt.Rows.Count <= 0)
                {
                    Json = JsonConvert.SerializeObject(new DicPackage(false, null, "无效用户！").DicInfo());
                    log.LogCatalogFailure("无效用户！");
                    return;
                }

                //校验旧密码
                if (!string.IsNullOrWhiteSpace(dt.Rows[0]["PASSWORD"] as string))
                {
                    if (!Identity.VerifyText(strOldPassword, dt.Rows[0]["PASSWORD"] as string))
                    {
                        Json = JsonConvert.SerializeObject(new DicPackage(false, null, "当前密码错误，请重新输入！").DicInfo());
                        log.LogCatalogFailure("当前密码错误，请重新输入！");
                        return;
                    }
                }
                else if (!(string.IsNullOrWhiteSpace(strOldPassword) && string.IsNullOrWhiteSpace(dt.Rows[0]["PASSWORD"] as string)))
                {
                    Json = JsonConvert.SerializeObject(new DicPackage(false, null, "当前密码错误，请重新输入！").DicInfo());
                    log.LogCatalogFailure("当前密码错误，请重新输入！");
                    return;
                }
                //校验新密码
                if (strNewPassword1.Length < 6 || strNewPassword1.Length > 32 || strNewPassword2.Length < 6 || strNewPassword2.Length > 32)
                {
                    Json = JsonConvert.SerializeObject(new DicPackage(false, null, "密码应为6~32个字符！").DicInfo());
                    log.LogCatalogFailure("密码应为6~32个字符！");
                    return;
                }
                if (!strNewPassword1.Equals(strNewPassword2))
                {
                    Json = JsonConvert.SerializeObject(new DicPackage(false, null, "新密码不一致！").DicInfo());
                    log.LogCatalogFailure("新密码不一致！");
                    return;
                }

                string strXmlParams =
                         string.Format("<params><usercode>{0}</usercode><oldpassword>{1}</oldpassword><newpassword>{2}</newpassword></params>",
                                        strCodeUser, strOldPassword, strNewPassword1);

                bool isSuccess = YGSoft.IPort.MobileManage.Interface.Core.ServiceUser.UpdatePassword(strXmlParams).Value;
                if (!isSuccess)
                {             
                    Json = JsonConvert.SerializeObject(new DicPackage(false, null, YGSoft.IPort.MobileManage.Interface.Core.ServiceUser.UpdatePassword(strXmlParams).Message).DicInfo());
                    log.LogCatalogFailure(YGSoft.IPort.MobileManage.Interface.Core.ServiceUser.UpdatePassword(strXmlParams).Message);
                }
                else
                {
                    Json = JsonConvert.SerializeObject(new DicPackage(true, null, "密码修改成功！").DicInfo());
                    log.LogCatalogSuccess();
                }              
            }
            catch (Exception ex)
            {
                Json = JsonConvert.SerializeObject(new DicPackage(false, null, string.Format("{0}：修改密码数据发生异常。{1}", ex.Source, ex.Message)).DicInfo());
            }
        }
        protected string Json;
    }
}

//strCodeUser = "173ff7d8702a4919876608562a39d216";
//strOldPassword = "111111";
//strNewPassword1 = "123456";
//strNewPassword2 = "123456";