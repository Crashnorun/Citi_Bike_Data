using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Citi_Bike_Data_01.Classes
{
    public class cls_User
    {
        #region PROPERTIES
        public UserType UserType
        {
            get { return userType; }
            set { userType = value; }
        }

        public Gender GenderType
        {
            get { return genderType; }
            set { genderType = value; }
        }

        public string BirthYear
        {
            get { return birthYear; }
            set { birthYear = value; }
        }

        private Gender genderType;
        private string birthYear;
        private UserType userType;
        #endregion

        public cls_User()
        {
        }

        public cls_User(Gender genderType, string birthYear, UserType userType)
        {
            this.genderType = genderType;
            this.birthYear = birthYear;
            this.userType = userType;
        }
    }
}
