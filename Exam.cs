using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SessionTimetable
{
    [Serializable]
    public class Exam: SessionItem
    {

        public Exam(string name, string group, string professorName)
            : base(name, professorName)
        {
            //проверяем входные данные
            if (group == "")
                throw new ArgumentException();
            
            //на экзамене одна группа
            Group = group;
        }
        public override string GetInfo()
        {
            return base.GetInfo() + " Экзамен\n";
        }
    }
}
