using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Extensions
{
    /// <summary>
    /// 特殊正则
    /// </summary>
    public class Regulars
    {
        /// <summary>
        /// 邮箱  EmailAddressAttribute 
        /// </summary>
        public const string Email = @"^\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$";
        /// <summary>
        /// 手机号(除开区号11位）
        /// </summary>
        public const string Phone = @"^(13[0-9]|14[5|7]|15[0|1|2|3|4|5|6|7|8|9]|18[0|1|2|3|5|6|7|8|9])\d{8}$";

        /// <summary>
        /// 域名
        /// </summary>
        public const string Domain = @"[a-zA-Z0-9][-a-zA-Z0-9]{0,62}(\.[a-zA-Z0-9][-a-zA-Z0-9]{0,62})+\.?";


        public const string Ipv4 = @"((2(5[0-5]|[0-4]\d))|[0-1]?\d{1,2})(\.((2(5[0-5]|[0-4]\d))|[0-1]?\d{1,2})){3}";


    }
}
