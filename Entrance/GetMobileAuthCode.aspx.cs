//
//文件名：    GetMobileAuthCode.aspx.cs
//功能描述：  获取手机验证码
//创建时间：  2016/03/11
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
using M_Platform.Common.SMS;

namespace M_Platform.Entrance
{
    public partial class GetMobileAuthCode : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //身份校验 
            if (!InterfaceTool.IdentityVerify(Request))
            {
                Json = JsonConvert.SerializeObject(new DicPackage(false, null, "身份认证错误！").DicInfo());
                return;
            }

            //手机号       
            string strMobile = Request.Params["Mobile"];

            //strMobile = "17710556243";
            //strMobile = "18036600293";

            AppLog log = new AppLog(Request);
            log.strAccount = strMobile;
            log.strBehavior = "获取手机验证码";
            log.strBehaviorURL = "/Entrance/GetMobileAuthCode.aspx";
    
            try
            {
                if (strMobile == null)
                {                 
                    Json = JsonConvert.SerializeObject(new DicPackage(false, null, "参数错误，获取验证码失败！").DicInfo());
                    return;

                }

                //手机号验证
                string strMessage = TokenTool.VerifyMobile(strMobile);
                if (strMessage != "ture")
                {
                    Json = JsonConvert.SerializeObject(new DicPackage(false, null, strMessage).DicInfo());
                    return;
                }

                //生成随机验证码
                string strSql =
                    string.Format("select round(dbms_random.value(100000,1000000)) as authcode from dual");
                var dt0 = new Leo.Oracle.DataAccess(RegistryKey.KeyPathHmw).ExecuteTable(strSql);
                //if (dt0.Rows.Count <= 0)
                //{
                //    Json = JsonConvert.SerializeObject(new DicPackage(false, null, "网络错误，请稍后再试！").DicInfo());
                //    return;
                //}

                //获取应用中文名称
                strSql =
                    string.Format(@"select fullname 
                                    from VW_APP_TOKEN 
                                    where appname='{0}'",
                                    Request.Params["AppName"]);
                var dt1 = new Leo.Oracle.DataAccess(RegistryKey.KeyPathMa).ExecuteTable(strSql);
                //if (dt.Rows.Count <= 0)
                //{
                //    Json = JsonConvert.SerializeObject(new DicPackage(false, null, "网络错误，请稍后再试！").DicInfo());
                //    return;                 
                //}

                string paramData = "{\"template\":\"SMS_63320388\","+
                                    "\"recipients\":[\"" + strMobile + "\"],"+
                                    "\"prefix\":\"连云港港\","+
                                    "\"parameters\":{\"AUTHCODE\":\"" + Convert.ToString(dt0.Rows[0]["authcode"]) + "\",\"APPNAME\":\"" + "（" + Convert.ToString(dt1.Rows[0]["fullname"]) + "）" + "\"}}";

                var result = ALiSmsSender.PostRequest(paramData);
                if (result.Item1 == true)
                {
                    //保存动态验证码（MobileCenter）
                    //string strDynamicIntervalTime = ConfigTool.GetWebConfigKey("DynamicIntervalTime");
                    //DateTime dynamicBeginTime = DateTime.Now;
                    //DateTime dynamicEndTime = dynamicBeginTime.AddSeconds(Convert.ToDouble(strDynamicIntervalTime));
                    strSql =
                        string.Format(@"insert into TB_APP_MOBILE_AUTHCODE (mobile,dynamic_authcode,dynamic_begintime,dynamic_endtime,un_dynamic_authcode) 
                                        values({0},'{1}',sysdate,sysdate+2/24/60,{2})",
                                        strMobile, Identity.RijndaelEncode(Convert.ToString(dt0.Rows[0]["authcode"])), Convert.ToString(dt0.Rows[0]["authcode"]));              
                    new Leo.Oracle.DataAccess(RegistryKey.KeyPathMa).ExecuteNonQuery(strSql);
                    Json = JsonConvert.SerializeObject(new DicPackage(true, null, "已成功发送，请注意查看！").DicInfo());
                    log.LogCatalogSuccess();
                }
                else       
                {
                    Json = JsonConvert.SerializeObject(new DicPackage(false, null, "线路问题，请重新发送").DicInfo());
                    log.LogCatalogFailure("线路问题，请重新发送1");
                }                      
            }
            catch (Exception ex)
            {
                Json = JsonConvert.SerializeObject(new DicPackage(false, null, string.Format("{0}：获取手机验证码数据发生异常。{1}", ex.Source, ex.Message)).DicInfo());
                log.LogCatalogFailure(string.Format("{0}：获取手机验证码数据发生异常。{1}", ex.Source, ex.Message));
            }
        }
        protected string Json;
    }
}


//航贸网短信机
//ServiceSendMessage.mobileSoapClient send = new ServiceSendMessage.mobileSoapClient();
//bool isSuccess = send.sendmessage(strMobile, strMessage, "短信系统", "短信系统");
//if (isSuccess)
//{
//    //保存动态验证码（MobileCenter）
//    string strDynamicIntervalTime = ConfigTool.GetWebConfigKey("DynamicIntervalTime");
//    DateTime dynamicBeginTime = DateTime.Now;
//    DateTime dynamicEndTime = dynamicBeginTime.AddSeconds(Convert.ToDouble(strDynamicIntervalTime));
//    strSql =
//        string.Format(@"insert into TB_APP_MOBILE_AUTHCODE (mobile,dynamic_authcode,dynamic_begintime,dynamic_endtime) 
//                        values({0},'{1}',to_date('{2}','YYYY-MM-DD HH24:MI:SS'),to_date('{3}','YYYY-MM-DD HH24:MI:SS'))",
//                        strMobile, Identity.RijndaelEncode(Convert.ToString(dt0.Rows[0]["authcode"])), dynamicBeginTime, dynamicEndTime);              
//    new Leo.Oracle.DataAccess(RegistryKey.KeyPathMa).ExecuteNonQuery(strSql);
//    Json = JsonConvert.SerializeObject(new DicPackage(true, null, "已成功发送，请注意查看！").DicInfo());
//    log.LogCatalogSuccess();
//}
//else       
//{
//    Json = JsonConvert.SerializeObject(new DicPackage(false, null, "线路问题，请直接输入验证码六个1").DicInfo());
//    log.LogCatalogFailure("线路问题，请直接输入验证码六个1");
//} 


//strMessage =
//                    string.Format("验证码：{1}。如非本人使用，敬请忽略本信息。【{0}】", Convert.ToString(dt1.Rows[0]["fullname"]), Convert.ToString(dt0.Rows[0]["authcode"]));

//                发送至短信机
//                var result = MobileSmsSender.Send(strMobile, strMessage);
//                if (result.Item1 == true)
//                {
//                    保存动态验证码（MobileCenter）
//                    string strDynamicIntervalTime = ConfigTool.GetWebConfigKey("DynamicIntervalTime");
//DateTime dynamicBeginTime = DateTime.Now;
//DateTime dynamicEndTime = dynamicBeginTime.AddSeconds(Convert.ToDouble(strDynamicIntervalTime));
//strSql =
//                        string.Format(@"insert into TB_APP_MOBILE_AUTHCODE (mobile,dynamic_authcode,dynamic_begintime,dynamic_endtime,un_dynamic_authcode) 
//                                        values({0},'{1}',sysdate,sysdate+2/24/60,{2})",
//                                        strMobile, Identity.RijndaelEncode(Convert.ToString(dt0.Rows[0]["authcode"])), Convert.ToString(dt0.Rows[0]["authcode"]));              
//                    new Leo.Oracle.DataAccess(RegistryKey.KeyPathMa).ExecuteNonQuery(strSql);
//Json = JsonConvert.SerializeObject(new DicPackage(true, null, "已成功发送，请注意查看！").DicInfo());
//                    log.LogCatalogSuccess();
//                }
//                else       
//                {
//                    Json = JsonConvert.SerializeObject(new DicPackage(false, null, "线路问题，请重新发送").DicInfo());
//                    log.LogCatalogFailure("线路问题，请重新发送1");
//                }



//string paramData = "{" + "\"prefix\":\"" + "连云港港" + "\"," 
//                       +"\"template\":\"" + "SMS_63320388" + "\","
//                       + "\"parameters\":{\"authcode\":\" + Convert.ToString(dt0.Rows[0]["authcode"]) + \",\"appname\":\" + + \"}}";

//Dictionary<string, object> param = new Dictionary<string, object>();
//param.Add("title", Convert.ToString(dt0.Rows[0]["authcode"]));
//param.Add("stepname", Convert.ToString(dt1.Rows[0]["fullname"]));

//string[] m = new string[] { "18036600293" };

//Dictionary<string, object> paramData = new Dictionary<string, object>();
//paramData.Add("recipients", m);
//paramData.Add("prefix", "连云港港");
//paramData.Add("template", "SMS_63090068");
//paramData.Add("parameters", JsonConvert.SerializeObject(param));
