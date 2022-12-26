using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SessionTimetable
{
    [Serializable]
    public class Consultation: SessionItem
    {

        public Consultation(string name, string groups, string professorName)
            :base(name, professorName)
        {
            //проверяем входные данные
            if (groups == "" )
                throw new ArgumentException();
            
            base.Group = groups;
        }
        public override string GetInfo()
        {
            return base.GetInfo() + " Консультация\n";
        }

    }
}
