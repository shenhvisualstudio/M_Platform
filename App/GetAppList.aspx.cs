﻿//
//文件名：    GetAppList.aspx.cs
//功能描述：  获取移动应用信息列表
//创建时间：  2015/09/18
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

namespace M_Platform.App
{
    public partial class GetAppList : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //设备类型
            string strDeviceType = Request.Params["DeviceType"];
            //应用类型
            string strAppType = Request.Params["AppType"];

            try
            {
                if (strDeviceType == null || strAppType == null)
                {
                    Json = JsonConvert.SerializeObject(new DicPackage(false, null, "参数错误，获取移动应用信息列表失败！").DicInfo());
                    return;

                }
                string strSql =
                    string.Format("select url,appname,name from TB_APP_UPDATE where devicetype='{0}' and apptype='{1}'", strDeviceType, strAppType);
                var dt = new Leo.Oracle.DataAccess(RegistryKey.KeyPathMa).ExecuteTable(strSql);
                if (dt.Rows.Count <= 0)
                {
                    Json = JsonConvert.SerializeObject(new DicPackage(false, null, "暂无数据！").DicInfo());
                    return;
                }

                string[,] strArray = new string[dt.Rows.Count, 3];
                for (int iRow = 0; iRow < dt.Rows.Count; iRow++)
                {
                    strArray[iRow, 0] = Convert.ToString(dt.Rows[iRow]["url"]);
                    strArray[iRow, 1] = Convert.ToString(dt.Rows[iRow]["appname"]);
                    strArray[iRow, 2] = Convert.ToString(dt.Rows[iRow]["name"]);
                }

                Json = JsonConvert.SerializeObject(new DicPackage(true, strArray, null).DicInfo());
            }
            catch (Exception ex)
            {
                Json = JsonConvert.SerializeObject(new DicPackage(false, null, string.Format("{0}：获取移动应用信息列表数据发生异常。{1}", ex.Source, ex.Message)).DicInfo());
            }
        }
        protected string Json;
    }
}