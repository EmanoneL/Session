using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SessionTimetable
{
    //базовый класс, содержащий основные свойста экзаменов и консультаций
    [Serializable]
    public class SessionItem
    {
        //название предмета
        public string Name { get; }
        public string Group;

        //преподаватель
        public string ProfessorName { get; }

        public SessionItem(string name, string professorName)
        {
            //проверяем входные данные
            if(name == "" || professorName == "")
                throw new ArgumentException();

            Name = name;
            ProfessorName = professorName;
        }

        public virtual string GetInfo()
        {
            return Name + '\n' + ProfessorName + '\n';
        }
        public string GetGroup()
        {
            return Group;
        }
    }
}
