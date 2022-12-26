using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SessionTimetable
{
    [Serializable]
    public class Timetable
    {   
        // список состоит из другого списка, содержащего расписание на день. Сессия расчитывается на 2 недели
        public List<List<(SessionItem, string)>> FirstWeek { get; }
        public List<List<(SessionItem, string)>> SecondWeek { get; }

        // 
        public static int MaxCountOfLess = 1;

        public static int CountOfWeeks = 1;

        //название группы/фио преподавателя для которого составлено расписание
        public string Name;
        public bool IsGroup;

        //дисциплины которые есть у группы/преподавателя
        public List<SessionItem> SessionItems { get; }

        public Timetable(string name, bool isgroup, bool memory = false)
        {
            if (name == "")
                throw new ArgumentException();
            IsGroup = isgroup;
            SecondWeek= new List<List<(SessionItem, string)>>();
            FirstWeek = new List<List<(SessionItem, string)>>();
            SessionItems = new List<SessionItem>();

            Name = name;

            //выделяем память

            if(memory)
            {
                for (int i = 0; i < 6; i++)
                {
                    List<(SessionItem, string)> list1 = new List<(SessionItem, string)>();
                    List<(SessionItem, string)> list2 = new List<(SessionItem, string)>();
                    for (int j = 0; j < MaxCountOfLess; j++)
                    {
                        list1.Add((null, ""));
                        list2.Add((null, ""));
                    }
                    SecondWeek.Add(list1);
                    FirstWeek.Add(list2);   
                }
            }
        }
        /*
        public bool Check()
        {
            for(int i = 0; i < SessionItems.Count(); i++)
            {
                int hours = 0;

                for(int j = 0; j < SecondWeek.Count(); j++)
                {
                    for(int k = 0; k < SecondWeek[j].Count(); k++)
                    {
                        if (SessionItems[i] == SecondWeek[j][k].Item1)
                            hours += 2;
                    }
                }
                for (int j = 0; j < FirstWeek.Count(); j++)
                {
                    for (int k = 0; k < FirstWeek[j].Count(); k++)
                    {
                        if (SessionItems[i] == FirstWeek[j][k].Item1)
                            hours += 2;
                    }
                }
                /*
                hours *= 9;
                if (hours == SessionItems[i].Item2)
                    continue;
                else return false;
            }
            return true;
        }*/
        public string GetItemInfo(bool secondweek, int day, int num, bool groups)
        {
            if (day < 0 || day > 5 || num < 0 || num > 5)
                throw new ArgumentException();
            string res;
            if(!groups)
            {
                if (secondweek)
                {
                    if (SecondWeek[day][num].Item1 == null)
                        res = "";
                    else
                        res = SecondWeek[day][num].Item1.GetInfo() + " " + SecondWeek[day][num].Item2;
                }

                else
                {
                    if (FirstWeek[day][num].Item1 == null)
                        res = "";
                    else
                        res = FirstWeek[day][num].Item1.GetInfo() + " " + FirstWeek[day][num].Item2;
                }

            }

            else
            {
                if (secondweek)
                {
                    if (SecondWeek[day][num].Item1 == null)
                        res = "";
                    else
                        res = SecondWeek[day][num].Item1.GetInfo() + " " + SecondWeek[day][num].Item1.GetGroup() + " " + SecondWeek[day][num].Item2;
                }

                else
                {
                    if (FirstWeek[day][num].Item1 == null)
                        res = "";
                    else
                        res = FirstWeek[day][num].Item1.GetInfo() + " " + FirstWeek[day][num].Item1.GetGroup() + " " + FirstWeek[day][num].Item2;
                }

            }
            return res;
        }


        //назначаем экзамен или консультацию
        public bool AddToTimetable(bool secondweek, int day, int num, SessionItem les, string auditorium = "")
        {
            int aud;
            if(day < 0 || day > 5 || num < 0 || num > 5 || les == null || !int.TryParse(auditorium, out aud))
                throw new ArgumentException();

            if(secondweek)
            {
                if (SecondWeek[day][num].Item1 == null)
                    SecondWeek[day][num] = (les, auditorium);
                else
                    return false;
            }

            else
            {
                if (FirstWeek[day][num].Item1 == null)
                    FirstWeek[day][num] = (les, auditorium);
                else
                    return false;
            }
            return true;
        }
        //удаляем ненужный предмет
        public void RemoveFromTable(bool evenweek, int day, int num)
        {
            if (day < 0 || day > 5 || num < 0 || num > 5)
                throw new ArgumentException();

            if (evenweek)
                SecondWeek[day][num] = (null, "");

            else
                FirstWeek[day][num] = (null, "");
        }

        //заполняем список дисциплин группы
        public void SetGroupItems(string filetext)
        {
            if (filetext == "")
                throw new ArgumentException();

            //на входе содержимое текстового файла с расписанием сессии
            string[] strings = filetext.Split('\n');

            string[] words;
            for (int i = 1; i < strings.Length; i++)
            {
                if (strings[i] == "")
                    continue;
                words = strings[i].Split(';');
                for(int j = 0; j < words.Length; j++)
                {
                    if (words[j].First() == ' ')
                        words[j].Remove(0, 1);
                }

                if (words[2].Trim().ToLower() == "консультация")
                {
                    Consultation l = new Consultation(words[0], words[3], words[1]);
                    SessionItems.Add((l));
                }
                else
                {
                    Exam s = new Exam(words[0], Name, words[1]);
                    SessionItems.Add((s));
                }
            }
        }

    }
}
