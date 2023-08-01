using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GrblController.Models
{
    class DatabaseModel
    {
        public string Server { get; set; }
        public string DatabaseServer { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }

        public string TableName { get; set; }

        public bool State { get; set; }
    }
}
