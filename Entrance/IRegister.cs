//
//文件名：    IRegister.aspx.cs
//功能描述：  注册接口
//创建时间：  2016/03/18
//作者：      
//修改时间：  暂无
//修改描述：  暂无
//
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;
using Leo;
using ServiceInterface.Common;
using YGSoft.IPort.MobileManage.Interface;

namespace M_Platform.Entrance
{
    public class IRegister
    {
        /// <summary>
        /// 日志对象
        /// </summary>
        private AppLog log;
        /// <summary>
        /// 注册数据集
        /// </summary>
        private RegisterE registerE;

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="registerE"></param>
        public IRegister(AppLog log, RegisterE registerE)
        {
            this.log = log;
            this.registerE = registerE;
        }

        #region 注册Iport用户
        public string RegisterIportUser()
        {
            string strJson = string.Empty;
            string strXmlParams =
                     string.Format("<params><loginname>{0}</loginname><password>{1}</password><username>{2}</username><companycode>{3}</companycode><departmentcode>{4}</departmentcode><usertypecode>{5}</usertypecode><workno>{6}</workno><mobile>{7}</mobile><phone>{8}</phone><qq>{9}</qq><email>{10}</email></params>",
                                     registerE.strLogogram, registerE.strPassword1, registerE.strUserName, registerE.strCompanyCode, registerE.strDepartmentCode, registerE.strUserTypeCode, registerE.strWorkNo, registerE.strMobile, registerE.strPhone, registerE.strQQ, registerE.strEmail);

            //校验手机号
            strJson = VerifyMobile(registerE.strMobile);
            if (!string.IsNullOrWhiteSpace(strJson))
            {
                return strJson;
            }

            //校验密码
            if (registerE.strPassword1.Length < 6 || registerE.strPassword1.Length > 32 || registerE.strPassword2.Length < 6 || registerE.strPassword2.Length > 32)
            {
                strJson = JsonConvert.SerializeObject(new DicPackage(false, null, "密码应为6~32个字符！").DicInfo());
                log.LogCatalogFailure("密码应为6~32个字符！");
                return strJson;
            }    
            if (registerE.strPassword1 != registerE.strPassword2)
            {
                strJson = JsonConvert.SerializeObject(new DicPackage(false, null, "密码不一致！").DicInfo());
                log.LogCatalogFailure("密码不一致！");
                return strJson;
            }

            bool isSuccess = YGSoft.IPort.MobileManage.Interface.Core.ServiceUser.Add(strXmlParams).Value;
            if (!isSuccess)
            {
                strJson = JsonConvert.SerializeObject(new DicPackage(false, null, YGSoft.IPort.MobileManage.Interface.Core.ServiceUser.Add(strXmlParams).Message).DicInfo());
                log.LogCatalogFailure(YGSoft.IPort.MobileManage.Interface.Core.ServiceUser.Add(strXmlParams).Message);
            }
            else
            {
                string strSql =
                        string.Format("select code_user from TB_SYS_USER where logogram='{0}'", registerE.strLogogram);
                var dt = new Leo.Oracle.DataAccess(RegistryKey.KeyPathIport).ExecuteTable(strSql);
                strJson = JsonConvert.SerializeObject(new DicPackage(true, Convert.ToString(dt.Rows[0]["code_user"]), null).DicInfo());
                log.LogCatalogSuccess();
            }

            return strJson;
        }
        #endregion

        #region 校验手机号
        /// <summary>
        /// 校验手机号
        /// </summary>
        /// <param name="strMobile"></param>
        /// <returns></returns>
        private string VerifyMobile(string strMobile)
        {
            string strJson = string.Empty;

            //判断是否已注册
            string sql =
                string.Format("select * from TB_SYS_USERINFO  where mobile='{0}'", strMobile);
            var dt = new Leo.Oracle.DataAccess(RegistryKey.KeyPathIport).ExecuteTable(sql);
            if (dt.Rows.Count != 0)
            {
                strJson = JsonConvert.SerializeObject(new DicPackage(false, null, "手机号已注册！").DicInfo());
                log.LogCatalogFailure("手机号已注册！");
                return strJson;
            }

            //手机号验证
            string strMessage = TokenTool.VerifyMobile(strMobile);
            if (strMessage != "ture")
            {
                strJson = JsonConvert.SerializeObject(new DicPackage(false, null, strMessage).DicInfo());
                log.LogCatalogFailure(strMessage);
                return strJson;
            }

            return strJson;
        }
        #endregion
    }

    public struct RetureMessage
    {
        //是否成功
        public bool Success { get; set; }
        //消息
        public string Message { get; set; }
        //值
        public object Value { get; set; }
    }
}




