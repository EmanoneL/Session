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
using Newtonsoft.Json;

namespace SessionTimetable
{
    public partial class Form1 : Form
    {
        //все существующие расписания
        public List<Timetable> SessionTimetable = new List<Timetable>();
        //флаг выбранной вкладки
        bool groups = true;
        string typeofcons;
        string typeofexam;

        public Form1()
        {
            InitializeComponent();
            SessionTable.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            SessionTable.DefaultCellStyle.WrapMode = DataGridViewTriState.True;
            ProfessorsSCH.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            ProfessorsSCH.DefaultCellStyle.WrapMode = DataGridViewTriState.True;

            Consultation l = new Consultation(" ", " ", " ");
            Exam s = new Exam(" ", " ", " ");
            typeofcons = l.GetType().ToString();
            typeofexam = s.GetType().ToString();
            ReDraw();
        }

        //обновление списка групп и преподавателей в комбобоксах
        void UpdGroupsAndProfs()
        {
            GroupNamesCMB.Items.Clear();
            ProfessorsCMB.Items.Clear();

            foreach(Timetable timetable in SessionTimetable)
            {
                if (timetable.IsGroup)
                    GroupNamesCMB.Items.Add(timetable.Name);
                else
                    ProfessorsCMB.Items.Add(timetable.Name);
            }

        }
        
        //перерисовка таблицы
        void ReDraw()
        {
            int rowsminh;

            SessionTable.Rows.Clear();
            ProfessorsSCH.Rows.Clear();
            //рисуем пустую таблицу
            for (int i = 0; i < Timetable.MaxCountOfLess; i++)
            {
                //добавление записи в таблицу
                SessionTable.Rows.Add();
                ProfessorsSCH.Rows.Add();

                SessionTable.Rows[i].Cells[0].Value = i + 1;
                ProfessorsSCH.Rows[i].Cells[0].Value = i + 1;

                for (int j = 1; j < 7; j++)
                {
                    SessionTable.Rows[i].Cells[j].Value = "";
                    ProfessorsSCH.Rows[i].Cells[j].Value = "";
                }
            }

            if(groups)
            {
                rowsminh = (SessionTable.Height - SessionTable.ColumnHeadersHeight) / Timetable.MaxCountOfLess;
                for(int i = 0; i < Timetable.MaxCountOfLess; i++)
                {
                    SessionTable.Rows[i].MinimumHeight = rowsminh;
                }

                if (GroupNamesCMB.SelectedIndex == -1)
                    return;

                int numofgr = -1;
                for(int i = 0; i < SessionTimetable.Count(); i++)
                {
                    if(SessionTimetable[i].IsGroup && SessionTimetable[i].Name == GroupNamesCMB.SelectedItem.ToString())
                    {
                        numofgr = i;
                        break;
                    }
                }
                for (int i = 0; i < Timetable.MaxCountOfLess; i++)
                {
                    for (int j = 0; j < 6; j++)
                    {
                        if (Week.SelectedIndex == 0)
                            SessionTable.Rows[i].Cells[j + 1].Value = SessionTimetable[numofgr].GetItemInfo(true, j, i, !SessionTimetable[numofgr].IsGroup);

                        if (Week.SelectedIndex == 1)
                            SessionTable.Rows[i].Cells[j + 1].Value = SessionTimetable[numofgr].GetItemInfo(false, j, i, !SessionTimetable[numofgr].IsGroup);
                    }
                }
            }
            else
            {
                rowsminh = (ProfessorsSCH.Height - ProfessorsSCH.ColumnHeadersHeight) / Timetable.MaxCountOfLess;
                for (int i = 0; i < Timetable.MaxCountOfLess; i++)
                {
                    ProfessorsSCH.Rows[i].MinimumHeight = rowsminh;
                }

                if (ProfessorsCMB.SelectedIndex == -1)
                    return;

                int numofprof = -1;
                for (int i = 0; i < SessionTimetable.Count(); i++)
                {
                    if (!SessionTimetable[i].IsGroup && SessionTimetable[i].Name == ProfessorsCMB.SelectedItem.ToString())
                    {
                        numofprof = i;
                        break;
                    }
                }
                for (int i = 0; i < Timetable.MaxCountOfLess; i++)
                {
                    for (int j = 0; j < 6; j++)
                    {
                        if (ProfWeek.SelectedIndex == 0)
                            ProfessorsSCH.Rows[i].Cells[j + 1].Value = SessionTimetable[numofprof].GetItemInfo(true, j, i, !SessionTimetable[numofprof].IsGroup);

                        if (ProfWeek.SelectedIndex == 1)
                            ProfessorsSCH.Rows[i].Cells[j + 1].Value = SessionTimetable[numofprof].GetItemInfo(false, j, i, !SessionTimetable[numofprof].IsGroup);
                    }
                }
            }
            
        }

        //чтение сохраненного рапсисания
        private void FileOpen_Click(object sender, EventArgs e)
        {
            string filePath = "";
            openFileDialog1.ShowDialog();
            if (openFileDialog1.FileName != openFileDialog1.InitialDirectory)
                filePath = openFileDialog1.FileName;

            try
            {

                if (File.Exists(filePath))
                {
                    SessionTimetable.Clear();
                    var settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto, Formatting = Formatting.Indented };
                    SessionTimetable = JsonConvert.DeserializeObject<List<Timetable>>(File.ReadAllText(filePath), settings);
                }
            }
            catch
            {
                MessageBox.Show("Файл поврежден или имет некорректный формат.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;

            }
            UpdGroupsAndProfs();
        }

        private void AddGroup_Click(object sender, EventArgs e)
        {
            AddGroup addGroup = new AddGroup(this);

            //вызываем форму для ввода данных о новой группе
            addGroup.ShowDialog();
            UpdGroupsAndProfs();

        }

        private void DeleteGroup_Click(object sender, EventArgs e)
        {

            if (GroupNamesCMB.SelectedIndex == -1)
            {
                MessageBox.Show("Группа, которую вы хотите удалить, не выбрана!", "Ошибка:", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            int index = -1;
            for (int i = 0; i < SessionTimetable.Count(); i++)
            {
                if (SessionTimetable[i].Name == GroupNamesCMB.SelectedItem.ToString())
                    index = i;
            }
            if (index > -1)
                SessionTimetable.RemoveAt(index);

            GroupNamesCMB.Text = "";

            UpdGroupsAndProfs();
            ReDraw();
        }
        private void Professors_Click(object sender, EventArgs e)
        {
            GroupsGB.Visible = false;
            ProfessorsGB.Visible = true;
            groups = false;
            ReDraw();

            GroupNamesCMB.SelectedIndex = -1;
            GroupNamesCMB.Text = "";
            Week.SelectedIndex = -1;
            Week.Text = "";
            TypeOfLessCMB.SelectedIndex = -1;
            TypeOfLessCMB.Text = "";
            LessonsCMB.Items.Clear();
            LessonsCMB.Text = "";
            AudTB.Text = "";
        }

        private void GroupMenu_Click(object sender, EventArgs e)
        {
            GroupsGB.Visible = true;
            ProfessorsGB.Visible = false;
            groups = true;
            ReDraw();

            ProfessorsCMB.SelectedIndex = -1;
            ProfessorsCMB.Text = "";
            ProfWeek.SelectedIndex = -1;
            ProfWeek.Text = "";
        }
        private void TypeOfLessCMB_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            if (GroupNamesCMB.SelectedIndex == -1)
            {
                MessageBox.Show("Группа, которой вы хотите назначить занятие, не выбрана!", "Ошибка:", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            int numofgr = -1;
            for (int i = 0; i < SessionTimetable.Count(); i++)
            {
                if (SessionTimetable[i].IsGroup && SessionTimetable[i].Name == GroupNamesCMB.SelectedItem.ToString())
                {
                    numofgr = i;
                    break;
                }
            }
            string type;
            if (TypeOfLessCMB.SelectedIndex == 0)
                type = typeofcons;
            else
                type = typeofexam;

            LessonsCMB.Items.Clear();
            LessonsCMB.Text = "";
            AudTB.Text = "";

            for (int i = 0; i < SessionTimetable[numofgr].SessionItems.Count(); i++)
            {
                if (SessionTimetable[numofgr].SessionItems[i].GetType().ToString() == type)
                    LessonsCMB.Items.Add(SessionTimetable[numofgr].SessionItems[i].Name);
            }    
        }

        void Add()
        {
            if (SessionTable.CurrentCell.ColumnIndex == 0)
            {
                MessageBox.Show("Не выбран день и время проведения занятия.\nНажмите на ячейку таблицы которая соответствует нужному дню и времени.", "Ошибка:", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (Week.SelectedIndex == -1)
            {
                MessageBox.Show("Не выбрана неделя!", "Ошибка:", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (GroupNamesCMB.SelectedIndex == -1)
            {
                MessageBox.Show("Группа, которой вы хотите назначить занятие, не выбрана!", "Ошибка:", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (TypeOfLessCMB.SelectedIndex == -1)
            {
                MessageBox.Show("Не выбран тип занятия!", "Ошибка:", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (LessonsCMB.SelectedIndex == -1)
            {
                MessageBox.Show("Дисциплина не выбрана!", "Ошибка:", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            int day = SessionTable.CurrentCell.ColumnIndex - 1;
            int les = SessionTable.CurrentCell.RowIndex;

            string type;
            if (TypeOfLessCMB.SelectedIndex == 0)
                type = typeofcons;
            else
                type = typeofexam;

            int numofgr = -1;
            for (int i = 0; i < SessionTimetable.Count(); i++)
            {
                if (SessionTimetable[i].IsGroup && SessionTimetable[i].Name == GroupNamesCMB.SelectedItem.ToString())
                {
                    numofgr = i;
                    break;
                }
            }

            if (Week.SelectedIndex == 0)
            {
                if (SessionTimetable[numofgr].SecondWeek[day][les].Item1 != null)
                {
                    MessageBox.Show("В это время уже есть занятие!", "Ошибка:", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }
            if (Week.SelectedIndex == 1)
            {
                if (SessionTimetable[numofgr].FirstWeek[day][les].Item1 != null)
                {
                    MessageBox.Show("В это время уже есть занятие!", "Ошибка:", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }
            //занятие, которое хотим добавить 
            SessionItem l;
            //в случае если день и время не заняты ни кем:
            if (type == typeofcons)
                l = new Consultation(" ", " ", " ");
            else
                l = new Exam(" ", " ", " ");
            //находим объект класса SessionItem по названию
            for (int m = 0; m < SessionTimetable[numofgr].SessionItems.Count; m++)
            {
                if (LessonsCMB.SelectedItem.ToString() == SessionTimetable[numofgr].SessionItems[m].Name && SessionTimetable[numofgr].SessionItems[m].GetType().ToString() == type)
                {
                    l = SessionTimetable[numofgr].SessionItems[m];
                    break;
                }
            }

            bool conscheck = false;
            // если экзамен, была ли назначена консультация
            if (type == typeofexam)
            {
                foreach (Timetable schedule in SessionTimetable)
                {
                    if (conscheck) break;
                    if (!schedule.IsGroup)
                        continue;

                    if (Week.SelectedIndex == 0)
                    {
                        for (int i = 0; i <= day; i++)
                        {
                            if (day > 5) break;
                            if (schedule.SecondWeek[i][0].Item1 == null) continue;
                            if (schedule.SecondWeek[i][0].Item1.GetType().ToString() == typeofcons && schedule.SecondWeek[i][0].Item1.Name == l.Name)
                            {
                                conscheck = true;
                                break;
                            } 
                            /*
                            if (schedule.SecondWeek[i][1].Item1 == null) continue;
                            if (schedule.SecondWeek[i][1].Item1.GetType().ToString() == typeofcons && schedule.SecondWeek[i][1].Item1.Name == l.Name)
                            {
                                conscheck = true;
                                break;
                            }*/

                        }
                        for (int i = 0; i < 6; i++)
                        {
                            if (conscheck) break;
                            if (schedule.FirstWeek[i][0].Item1 == null) continue;
                            if (schedule.FirstWeek[i][0].Item1.GetType().ToString() == typeofcons && schedule.FirstWeek[i][0].Item1.Name == l.Name)
                            {
                                conscheck = true;
                                break;
                            }/*
                            if (schedule.FirstWeek[i][1].Item1 == null) continue;
                            if (schedule.FirstWeek[i][1].Item1.GetType().ToString() == typeofcons && schedule.FirstWeek[i][1].Item1.Name == l.Name)
                            {
                                conscheck = true;
                                break;
                            }*/
                        }

                    } else
                    {
                        for (int i = 0; i < day; i++)
                        {
                            if (schedule.FirstWeek[i][0].Item1 == null) continue;
                            if (schedule.FirstWeek[i][0].Item1.GetType().ToString() == typeofcons && schedule.FirstWeek[i][0].Item1.Name == l.Name)
                            {
                                conscheck = true;
                                break;
                            }/*
                            if (schedule.FirstWeek[i][1].Item1 == null) continue;
                            if (schedule.FirstWeek[i][1].Item1.GetType().ToString() == typeofcons && schedule.FirstWeek[i][1].Item1.Name == l.Name)
                            {
                                conscheck = true;
                                break;
                            }*/
                        }
                    }


                }
                if (!conscheck)
                {
                    MessageBox.Show("Вы не можете назначить экзамен, не назначив консультацию", "Ошибка:", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    AudTB.Clear();
                    return;
                }
            }

            
            foreach (Timetable schedule in SessionTimetable)
            {
                if (!schedule.IsGroup)
                    continue;
                if (Week.SelectedIndex == 0)
                {
                    //если день и время у группы пустые, то нет смысла проверять 
                    if (schedule.SecondWeek[day][les].Item1 == null)
                        continue;



                    //если совпадает аудитория
                    if (schedule.SecondWeek[day][les].Item2 == AudTB.Text)
                    {
                        //если в выбранной аудитории не консультация
                        if (schedule.SecondWeek[day][les].Item1.GetType().ToString() != typeofcons || type != typeofcons)
                        {
                            MessageBox.Show("Аудитория занята!", "Ошибка:", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            AudTB.Clear();
                            return;
                        }
                        //если в выбранной аудитории консультация
                        if (schedule.SecondWeek[day][les].Item1.GetType().ToString() == typeofcons)
                        {
                            //если добавляемое занятие не консультация
                            if (type != typeofcons)
                            {
                                MessageBox.Show("Аудитория занята!", "Ошибка:", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                AudTB.Clear();
                                return;
                            }

                            //если добавляем консультацию, то смотрим, должна ли наша группа быть на ней
                            string[] groups = schedule.SecondWeek[day][les].Item1.GetGroup().Replace(",", "").Trim('\r').Split(' ');
                            //смотрим список групп, которые должны быть на этом предмете
                            for (int k = 0; k < groups.Length; k++)
                            {
                                //в списке групп нашлась выбранная
                                if (groups[k] == SessionTimetable[numofgr].Name)
                                {
                                    //находим объект класса SessionItem в списке дисциплин группы
                                    SessionItem less = new Consultation(" ", " ", " ");

                                    less = schedule.SecondWeek[day][les].Item1;

                                    try
                                    {
                                        if(l.Name == less.Name && l.ProfessorName == less.ProfessorName )
                                        {
                                            //добавляем этот объект в нужную ячейку нашего расписания
                                            SessionTimetable[numofgr].AddToTimetable(!Convert.ToBoolean(Week.SelectedIndex), day, les, less, AudTB.Text);
                                            ReDraw();
                                            TypeOfLessCMB.SelectedIndex = -1;
                                            TypeOfLessCMB.Text = "";
                                            LessonsCMB.Items.Clear();
                                            LessonsCMB.Text = "";
                                            AudTB.Text = "";
                                            return;
                                        }
                                    }
                                    catch
                                    {
                                        MessageBox.Show("Что-то не так, попробуйте изменить параметры", "Ошибка:", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                        return;
                                    }
                                }
                            }
                            MessageBox.Show("Аудитория занята!", "Ошибка:", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            AudTB.Clear();
                            return;
                        }
                    }
                    else
                        continue;

                }
                //аналогичный алгоритм для нечетной недели
                if (Week.SelectedIndex == 1)
                {
                    if (schedule.FirstWeek[day][les].Item1 == null)
                        continue;

                    if (schedule.FirstWeek[day][les].Item2 == AudTB.Text)
                    {

                        if (schedule.FirstWeek[day][les].Item1.GetType().ToString() != typeofcons || type != typeofcons)
                        {
                            MessageBox.Show("Аудитория занята!", "Ошибка:", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            AudTB.Clear();
                            return;
                        }
                        if (schedule.FirstWeek[day][les].Item1.GetType().ToString() == typeofcons)
                        {
                            if (type != typeofcons)
                            {
                                MessageBox.Show("Аудитория занята!", "Ошибка:", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                AudTB.Clear();
                                return;
                            }
                            string[] groups = schedule.FirstWeek[day][les].Item1.GetGroup().Replace(",", "").Trim('\r').Split(' ');
                            for (int k = 0; k < groups.Length; k++)
                            {
                                if (groups[k] == SessionTimetable[numofgr].Name)
                                {
                                    SessionItem less = new Consultation(" ", " ", " ");

                                    less = schedule.FirstWeek[day][les].Item1;

                                    try
                                    {
                                        if (l.Name == less.Name && l.ProfessorName == less.ProfessorName)
                                        {
                                            //добавляем этот объект в нужную ячейку нашего расписания
                                            SessionTimetable[numofgr].AddToTimetable(!Convert.ToBoolean(Week.SelectedIndex), day, les, less, AudTB.Text);
                                            ReDraw();
                                            TypeOfLessCMB.SelectedIndex = -1;
                                            TypeOfLessCMB.Text = "";
                                            LessonsCMB.Items.Clear();
                                            LessonsCMB.Text = "";
                                            AudTB.Text = "";
                                            return;
                                        }
                                    }
                                    catch
                                    {
                                        MessageBox.Show("Что-то не так, попробуйте изменить параметры", "Ошибка:", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                        return;
                                    }
                                }
                            }
                            MessageBox.Show("Аудитория занята!", "Ошибка:", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            AudTB.Clear();
                            return;
                        }
                    }
                    else
                        continue;
                }
            }

            try
            {
                
                //находим преподавателя, который должен вести предмет

                int indofprof = -1;

                for (int i = 0; i < SessionTimetable.Count(); i++)
                {
                    if (!SessionTimetable[i].IsGroup)
                    {
                        if (SessionTimetable[i].Name == l.ProfessorName)
                        {
                            indofprof = i;
                            break;
                        }
                    }
                }
                if (!SessionTimetable[indofprof].AddToTimetable(!Convert.ToBoolean(Week.SelectedIndex), day, les, l, AudTB.Text))
                {
                    MessageBox.Show("У преподавателя, который ведет этот предмет, назначено мероприятие в это время!", "Ошибка:", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                //добавляем его в расписание
                SessionTimetable[numofgr].AddToTimetable(!Convert.ToBoolean(Week.SelectedIndex), day, les, l, AudTB.Text);
            }
            catch
            {
                MessageBox.Show("Что-то не так, попробуйте изменить параметры", "Ошибка:", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
           
            ReDraw();

            TypeOfLessCMB.SelectedIndex = -1;
            TypeOfLessCMB.Text = "";
            LessonsCMB.Items.Clear();
            LessonsCMB.Text = "";
            AudTB.Text = "";
        }

        private void AddLessontoSch_Click_1(object sender, EventArgs e)
        {
            Add();
        }

        private void Week_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            TypeOfLessCMB.SelectedIndex = -1;
            TypeOfLessCMB.Text = "";
            LessonsCMB.Items.Clear();
            LessonsCMB.Text = "";
            AudTB.Text = "";
            ReDraw();
        }

        private void ProfessorsCMB_SelectedIndexChanged(object sender, EventArgs e)
        {
            ReDraw();
        }

        private void ProfWeek_SelectedIndexChanged(object sender, EventArgs e)
        {
            ReDraw();
        }

        private void GroupNamesCMB_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            //обновляем таблицу
            ReDraw();

            TypeOfLessCMB.SelectedIndex = -1;
            TypeOfLessCMB.Text = "";
            LessonsCMB.Items.Clear();
            LessonsCMB.Text = "";
            AudTB.Text = "";
        }
        void Remove()
        {
            int day = SessionTable.CurrentCell.ColumnIndex - 1;
            int les = SessionTable.CurrentCell.RowIndex;
            bool evenweek;
            string aud;
            int numofgr = -1;

            if (GroupNamesCMB.SelectedIndex == -1)
            {
                MessageBox.Show("Группа не выбрана!", "Ошибка:", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (SessionTable.CurrentCell.ColumnIndex == 0)
            {
                MessageBox.Show("Не выбран день и время проведения мероприятия.\nНажмите на ячейку таблицы которая соответствует нужному дню и времени.", "Ошибка:", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (Week.SelectedIndex == -1)
            {
                MessageBox.Show("Не указана неделя(первая/вторая)!", "Ошибка:", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (Week.SelectedIndex == 0)
                evenweek = true;
            else
                evenweek = false;

            for (int i = 0; i < SessionTimetable.Count(); i++)
            {
                if (SessionTimetable[i].IsGroup && SessionTimetable[i].Name == GroupNamesCMB.SelectedItem.ToString())
                {
                    numofgr = i;
                    break;
                }
            }
            SessionItem less = new SessionItem(" ", " ");
            //сохраняем предмет, который потом будем удалять у всех
            if (evenweek)
            {
                less = SessionTimetable[numofgr].SecondWeek[day][les].Item1;
                aud = SessionTimetable[numofgr].SecondWeek[day][les].Item2;
            }
            else
            {
                less = SessionTimetable[numofgr].FirstWeek[day][les].Item1;
                aud = SessionTimetable[numofgr].FirstWeek[day][les].Item2;
            }
            if (less == null)
                return;

            try
            {
                foreach (Timetable table in SessionTimetable)
                {
                    if (evenweek)
                    {
                        //проверяем нужно ли что-то удалять
                        if (table.SecondWeek[day][les].Item1 != null)
                            //смотрим совпадает ли пара и аудитория
                            if (table.SecondWeek[day][les].Item1.ToString() == less.ToString())  
                                if(table.SecondWeek[day][les].Item2 == aud)
                                {//удаляем из расписания конкретной группы/преподавателя
                                    table.RemoveFromTable(evenweek, day, les);
                                }
                    }
                    else
                    {//проверяем нужно ли что-то удалять
                        if (table.FirstWeek[day][les].Item1 != null)
                            //смотрим совпадает ли пара и аудитория
                            if (table.FirstWeek[day][les].Item1.ToString() == less.ToString())
                                if(table.FirstWeek[day][les].Item2 == aud)
                                {//удаляем из расписания конкретной группы/преподавателя
                                    table.RemoveFromTable(evenweek, day, les);
                                }
                    }
                }
            }
            catch
            {
                MessageBox.Show("Что-то пошло не так. попробуйте еще раз!", "Ошибка:", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            ReDraw();
        }
        private void DeleteLess_Click(object sender, EventArgs e)
        {
            Remove();
        }

        private void FileSave_Click(object sender, EventArgs e)
        {
            try
            {

                string filePath = "";
                saveFileDialog1.ShowDialog();
                if (saveFileDialog1.FileName != saveFileDialog1.InitialDirectory)
                    filePath = saveFileDialog1.FileName;

                filePath += ".json";

                if (SessionTimetable.Count != 0)//если ассортимент был удален полностью, то сохранять ничего не нужно
                {
                    var settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto, Formatting = Formatting.Indented };
                    JsonSerializer json = JsonSerializer.Create(settings);
                    using (StreamWriter sw = new StreamWriter(filePath))
                    using (JsonWriter writer = new JsonTextWriter(sw))
                    {
                        json.Serialize(writer, SessionTimetable);
                    }
                }
            }
            catch
            {
                MessageBox.Show("Что-то пошло не так. Пожалуйста попробуйте снова или обратитесь к специалистую", "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }

        private void Export_Click(object sender, EventArgs e)
        {
            string filePath = "";
            folderBrowserDialog1.ShowDialog();
            filePath = folderBrowserDialog1.SelectedPath;
            try 
            {
                string name = DateTime.Now.ToString() + " Расписания ";
                name = name.Replace(":", "-");
                name = name.Replace("/", "_");

                Directory.CreateDirectory(filePath);
                foreach (Timetable schedule in SessionTimetable)
                {
                    using (StreamWriter sw = new StreamWriter(Path.Combine(filePath, name + schedule.Name + ".xls"), false, Encoding.GetEncoding(1251)))
                    {
                        sw.WriteLine("Вторая неделя");
                        sw.WriteLine("№ пары\tПонедельник\tВторник\tСреда\tЧетверг\tПятница\tСуббота");
                        for (int i = 0; i < Timetable.MaxCountOfLess; i++)
                        {
                            string str = (i + 1).ToString() + "\t";
                            for (int j = 0; j < 6; j++)
                                str += (schedule.GetItemInfo(true, j, i, !schedule.IsGroup) + "\t");
                            str = str.Replace("\n", "");
                            sw.WriteLine(str);
                        }
                        sw.WriteLine();
                        sw.WriteLine("Первая неделя");
                        sw.WriteLine("№ пары\tПонедельник\tВторник\tСреда\tЧетверг\tПятница\tСуббота");
                        for (int i = 0; i < Timetable.MaxCountOfLess; i++)
                        {
                            string str = (i + 1).ToString() + "\t";
                            for (int j = 0; j < 6; j++)
                            {
                                if (schedule.IsGroup)
                                    str += (schedule.GetItemInfo(false, j, i, !schedule.IsGroup) + "\t");
                            }
                            str = str.Replace("\n", "");
                            sw.WriteLine(str);
                        }
                    }
                }
            }
            catch
            {
                MessageBox.Show("Что-то пошло не так. Пожалуйста попробуйте снова или обратитесь к специалистую", "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            MessageBox.Show("Расписания скопированы в папку\n " + filePath , "", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        /*private void Check_Click(object sender, EventArgs e)
        {
            if (GroupNamesCMB.SelectedIndex == -1)
            {
                MessageBox.Show("Группа не выбрана!", "Ошибка:", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            int numofgr = -1;
            for (int i = 0; i < SessionTimetable.Count(); i++)
            {
                if (SessionTimetable[i].IsGroup && SessionTimetable[i].Name == GroupNamesCMB.SelectedItem.ToString())
                {
                    numofgr = i;
                    break;
                }
            }

            if (SessionTimetable[numofgr].Check())
                MessageBox.Show("Расписание выбранной группы соответствует учебному плану по количеству часов.", "Успешно:", MessageBoxButtons.OK, MessageBoxIcon.Information);

            else MessageBox.Show("Расписание выбранной группы не соответствует учебному плану по количеству часов.", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }*/

        private void Help_Click(object sender, EventArgs e)
        {
            string path = "help.txt";
            string info;

            using (StreamReader sr = new StreamReader(path))
                info = sr.ReadToEnd();

            MessageBox.Show(info, "Справка:", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void FileBttn_Click(object sender, EventArgs e)
        {

        }
    }   
}
