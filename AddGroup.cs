using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace SessionTimetable
{
    public partial class AddGroup : Form
    {
        private Form1 form1;
        string filePath = "";
        public AddGroup(Form1 form1)
        {
            InitializeComponent();

            this.form1 = form1;
        }

        private void Cancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void Save_Click(object sender, EventArgs e)
        {
            if(GroupNameTB.Text == "") 
            {
                MessageBox.Show("Номер группы не заполнен!", "Ошибка:", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            foreach(Timetable s in form1.SessionTimetable)
            {
                if(s.Name.ToLower().Trim() == GroupNameTB.Text.ToLower().Trim())
                {
                    MessageBox.Show("Группа с таким номером уже существует!", "Ошибка:", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }

            if(filePath == "")
            {
                MessageBox.Show("Перечень предметов не выбран!", "Ошибка:", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            
            try 
            {
                Timetable table = new Timetable(GroupNameTB.Text, true, true);
                string text;
                using (StreamReader sr = new StreamReader(openFileDialog1.FileName))
                    text = sr.ReadToEnd();
                table.SetGroupItems(text);

                //добавляем расписание группы(пока пустое) в массив расписаний
                form1.SessionTimetable.Add(table);
                List<Timetable> professors = new List<Timetable>();
                bool flag = true;

                //смотрим всех преподавателей новой группы и если для них еще не создано расписание, то создаем объект и добавляем в коллекцию
                for(int i = 0; i < table.SessionItems.Count(); i++)
                {
                    //смотрим уже созданных преподавателей
                    for (int j = 0; j < form1.SessionTimetable.Count(); j++)
                    {
                        if (!form1.SessionTimetable[j].IsGroup)
                        {
                            if (form1.SessionTimetable[j].Name == table.SessionItems[i].ProfessorName)
                            {
                                flag = false;
                                break;
                            }

                        }
                    }
                    if (!flag)
                        continue;

                    if (flag)
                    {
                        bool add = true;
                        //добавляем нового преподавателя
                        Timetable professor = new Timetable(table.SessionItems[i].ProfessorName, false, true);

                        if (professors.Count() != 0)
                        {
                            for (int k = 0; k < professors.Count(); k++)
                            {
                                if (professor.Name == professors[k].Name)
                                {
                                    add = false;
                                }
                            }
                            if (add)
                            {
                                professors.Add(professor);
                            }
                        } else
                        {
                            professors.Add(professor);
                        }
                        
                    }

                    /*
                    //смотрим преподавателей, которых добавили только что и еще не сохранили в общий список
                    for (int k = 0; k < professors.Count(); k++)
                    {
                        if (professors[k].Name == table.SessionItems[i].ProfessorName)
                        {
                            flag = false;
                            break;
                        }
                        flag = true;
                    }
                    */

                }
                //выгружаем список добавленных преподавателей в общий список расписаний
                foreach(Timetable schedule1 in professors)
                {
                    form1.SessionTimetable.Add(schedule1);
                }
            }
            catch
            {
                MessageBox.Show("Что-то не так, попробуйте снова :( ", "Ошибка:", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //получаем имя выбранного файла с учебным планом
            openFileDialog1.ShowDialog();
            if (openFileDialog1.FileName != openFileDialog1.InitialDirectory)
                filePath = openFileDialog1.FileName;    
        }
    }
}
