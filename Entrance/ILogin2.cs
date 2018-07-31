//
//文件名：    ILogin.aspx.cs
//功能描述：  登陆接口
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
using YGSoft.IPort.Data;

namespace M_Platform.Entrance
{
    public class ILogin2
    {
        /// <summary>
        /// 日志对象
        /// </summary>
        private SysLog log;

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="registerE"></param>
        public ILogin2(SysLog log)
        {
            this.log = log;
        }

        #region 获取通过手机号登陆
        /// <summary>
        /// 获取通过手机号登陆
        /// </summary>
        /// <param name="account"手机号></param>
        /// <param name="password">密码</param>
        /// <returns>返回结果</returns>
        public string GetInfoByMobileLogin(string strAccount, string strPassword)
        {
            Dictionary<string, string> info = new Dictionary<string, string>();
            string strJson = string.Empty;

            string strSql =
                string.Format(@"select b.code_user,a.password 
                                from tb_sys_user a, tb_sys_userinfo b 
                                where a.code_user=b.code_user and b.mobile='{0}'",
                                strAccount);
            //判断用户（手机号）是否存在
            var dt = new Leo.Oracle.DataAccess(RegistryKey.KeyPathIport).ExecuteTable(strSql);
            if (dt.Rows.Count <= 0)
            {
                strJson = JsonConvert.SerializeObject(new DicPackage(false, null, "用户名或密码错误！").DicInfo());
                log.LogCatalogFailure("用户名或密码错误！");
                return strJson;
            }

            if (!Identity.VerifyText(Format.Trim(strPassword), dt.Rows[0]["password"] as string))
            {
                strJson = JsonConvert.SerializeObject(new DicPackage(false, null, "用户名或密码错误！").DicInfo());
                log.LogCatalogFailure("用户名或密码错误！");
                return strJson;
            }

            strJson = JsonConvert.SerializeObject(new DicPackage(true, Convert.ToString(dt.Rows[0]["code_user"]), null).DicInfo());
            log.LogCatalogSuccess();
            return strJson;
        }
        #endregion

        #region 获取通过用户名(Iport账号)登陆
        /// <summary>
        /// 获取通过用户名(Iport账号)登陆
        /// </summary>
        /// <param name="strAccount">用户名(Iport账号)</param>
        /// <param name="strPassword">密码</param>
        /// <returns>返回结果</returns>
        public string GetInfoByUserNameLogin(string strAccount, string strPassword)
        {
            Dictionary<string, string> info = new Dictionary<string, string>();
            string strJson = string.Empty;

            string strSql =
                string.Format(@"select code_user,password 
                                from tb_sys_user 
                                where upper(logogram)='{0}'",
                                strAccount.ToUpper());
            //判断用户（Iport账号）是否存在
            var dt0 = new Leo.Oracle.DataAccess(RegistryKey.KeyPathIport).ExecuteTable(strSql);
            if (dt0.Rows.Count == 0)
            {
                strJson = JsonConvert.SerializeObject(new DicPackage(false, null, "用户名或密码错误！").DicInfo());
                log.LogCatalogFailure("用户名或密码错误！");
                return strJson;
            }

            if (!Identity.VerifyText(Format.Trim(strPassword), dt0.Rows[0]["password"] as string))
            {
                strJson = JsonConvert.SerializeObject(new DicPackage(false, null, "用户名或密码错误！").DicInfo());
                log.LogCatalogFailure("用户名或密码错误！");
                return strJson;
            }

            //            sql =
            //                string.Format(@"select mobile 
            //                                from tb_sys_userinfo 
            //                                where code_user='{0}'",
            //                                Convert.ToString(dt0.Rows[0]["code_user"]));
            //            //判断用户（Iport账号）是否存在
            //            var dt1 = new Leo.Oracle.DataAccess(RegistryKey.KeyPathIport).ExecuteTable(sql);
            //            if ((dt1.Rows.Count == 0) || string.IsNullOrWhiteSpace(Convert.ToString(dt1.Rows[0]["mobile"])))
            //            {
            //                Json = JsonConvert.SerializeObject(new DicPackage(false, Convert.ToString(dt0.Rows[0]["code_user"]), "此账号未绑定手机号！").DicInfo());
            //            }
            //            else
            //            {
            //                Json = JsonConvert.SerializeObject(new DicPackage(true, Convert.ToString(dt0.Rows[0]["code_user"]), "登陆成功！").DicInfo());
            //            }

            strJson = JsonConvert.SerializeObject(new DicPackage(true, Convert.ToString(dt0.Rows[0]["code_user"]), null).DicInfo());
            log.LogCatalogSuccess();
            return strJson;
        }
        #endregion
    }

}




