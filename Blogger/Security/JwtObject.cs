using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Blogger.Security
{
    public class JwtObject
    {
        public string Account { get; set; }

        public string Role { get; set; }
        //到期時間
        public string Expire { get; set; }
    }
}