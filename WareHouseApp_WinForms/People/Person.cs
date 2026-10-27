using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WareHouseApp.People
{
    abstract public class Person
    {
        public int empID;
        public string userName;
        private string password;
        public string Password
        {
            set
            {
                this.password = value;
            }
        }
        abstract public bool Login(string username,string password);
        abstract public void ChangePassword(string newPassword);
        abstract public void Logout(int userID);
    }
}
