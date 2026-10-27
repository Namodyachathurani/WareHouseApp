using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WareHouseApp.People
{
    public class ShippingOperator : Person
    {
        
        public override bool Login(string username, string password)
        {
            return false;
        }
        public override void ChangePassword(string newPassword)
        {
            this.Password = newPassword;
        }
        public override void Logout(int userID)
        {

        }

        public int LoadStocks()
        {
            int stocks = 0;
            return stocks;
        }
        public int ShipStocks()
        {
            int stocks = 0;
            return stocks;
        }
    }
}
