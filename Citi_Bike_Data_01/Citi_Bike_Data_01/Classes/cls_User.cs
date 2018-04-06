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
        public userType UserType
        {
            get { return userType; }
            set { userType = value; }
        }

        public gender GenderType
        {
            get { return genderType; }
            set { genderType = value; }
        }

        public string BirthYear
        {
            get { return birthYear; }
            set { birthYear = value; }
        }

        private gender genderType;
        private string birthYear;
        private userType userType;
        #endregion

        public cls_User()
        {
        }

        public cls_User(gender genderType, string birthYear, userType userType)
        {
            this.genderType = genderType;
            this.birthYear = birthYear;
            this.userType = userType;
        }
    }
}
