using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WareHouseApp.People
{
    class Admin : Person
    {
        

        public override bool Login(string username, string password)
        {
            if(username == "Admin" && password == "admin123")
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public override void ChangePassword(string newPassword)
        {
            throw new NotImplementedException();
        }

        public override void Logout(int userID)
        {
            throw new NotImplementedException();
        }
    }
}
