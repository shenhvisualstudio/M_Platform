using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ServiceInterface.Common
{
    public class MobileSmsSender
    {
        public static Tuple<bool, string> Send(string addr, string message)
        {
            const string strApplicationId = "P000000000000111";
            const string strUrl = "http://221.178.190.165:6118/services/cmcc_mas_wbs";

            var wbs = new cmcc_mas_wbs();
            wbs.Url = strUrl;
            var uri = System.Uri.EscapeUriString("tel:" + addr);
            var request = new sendSmsRequest()
            {
                ApplicationID = strApplicationId,
                DestinationAddresses = new[] { uri },
                MessageFormat = MessageFormat.GB2312,
                SendMethod = SendMethodType.Long,
                Message = message,
                DeliveryResultRequest = true
            };
            try
            {
                var identify = wbs.sendSms(request).RequestIdentifier;
                return new Tuple<bool, string>(true, identify);
            }
            catch (Exception ex)
            {
                return new Tuple<bool, string>(false, ex.Message);
            }
        }
    }
}