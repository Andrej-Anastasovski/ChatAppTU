using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatAppServer
{
    public class accountModel
    {
        public string username { get; set; }
        public string password { get; set; }
        public string profPic { get; set; }
        public string status { get; set; }

        public accountModel(string username, string password, string profPic, string status)
        {
            this.username = username;
            this.password = password;
            this.profPic = profPic;
            this.status = status;
        }
    }

}
