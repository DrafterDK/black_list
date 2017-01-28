using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Xml.Serialization;
using System.IO;

namespace testconnectbd
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // TODO: данная строка кода позволяет загрузить данные в таблицу "dataDataSet.Users". При необходимости она может быть перемещена или удалена.
            this.usersTableAdapter.Fill(this.dataDataSet.Users);

            string file_name = "setting.xml";
           // читаем файл настроек
            try
            {
                XmlSerializer ser = new XmlSerializer(typeof(program_setting));
                using (FileStream fs = new FileStream(file_name, FileMode.OpenOrCreate))
                {
                    program_setting PS = (program_setting)ser.Deserialize(fs);
                    Set_program_setting(PS);
                }
            }
            catch
            {
                // используем размеры по умолчанию
            }
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            int indexUser = Convert.ToInt32(dataGridView1[0, dataGridView1.CurrentRow.Index].Value);
            string[] user = new string[2];
            user = Get_User_to_index(indexUser);
            textBox_name.Text = user[0];
            textBox_name.Enabled = false;
            textBox_sur_name.Text = user[1];
            textBox_sur_name.Enabled = false;
            textBox_id.Text = indexUser.ToString();
            button_change.Enabled = true;
            button_save.Enabled = false;

        }


        string[] Get_User_to_index(int index) // получение данных пользователя по его индексу
        {
            string[] user = new string[2];
            try
            {
                string connectString = "Data Source=VLADIMIR-PC;Initial Catalog=data;Integrated Security=True";
                SqlConnection connetcted = new SqlConnection();
                connetcted.ConnectionString = connectString;
                connetcted.Open();

                SqlCommand command = new SqlCommand("SELECT Usename, UserSerName   FROM Users WHERE iduser = " + index, connetcted);
                using (SqlDataReader dr = command.ExecuteReader())
                {
                    dr.Read();
                    user[0] = dr.GetString(0);
                    user[1] = dr.GetString(1);

                }
                connetcted.Close();

                return user;
            }
            catch
            {
                MessageBox.Show("Ошибка при подключении к БД", "Ошибка подключения", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return user;
            }
        }


        private void button_change_Click(object sender, EventArgs e)
        {
            if (textBox_name.Text != "" & textBox_sur_name.Text != "")
            {
                textBox_name.Enabled = true;
                textBox_sur_name.Enabled = true;
                button_save.Enabled = true;
                button_change.Enabled = false;
            }

        }
        void delete_user(string index) //удаление пользователя
        {
            string connectString = "Data Source=VLADIMIR-PC;Initial Catalog=data;Integrated Security=True";
            try
            {
                SqlConnection connetcted = new SqlConnection();
                connetcted.ConnectionString = connectString;
                connetcted.Open();
                SqlCommand command = new SqlCommand("DELETE users WHERE iduser = " + index, connetcted);
                command.ExecuteNonQuery();
                connetcted.Close();
                MessageBox.Show("Пользователь под номером " + index + " удален.", "Пользователь удален.", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch
            {
                MessageBox.Show("Ошибка при подключении к БД", "Ошибка подключения", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        void change_user(string name, string sur_name, string index) //изменение данных пользователя
        {
            try
            {
                string connectString = "Data Source=VLADIMIR-PC;Initial Catalog=data;Integrated Security=True";
                SqlConnection connetcted = new SqlConnection();
                connetcted.ConnectionString = connectString;
                connetcted.Open();
                SqlCommand command = new SqlCommand("UPDATE users SET UseName = '" + name + "', UserSerName = '" + sur_name + "' WHERE iduser = " + index, connetcted);
                command.ExecuteNonQuery();
                connetcted.Close();
                MessageBox.Show("Пользователь под номером " + index + " изменен.", "Изменения сохранены", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch
            {
                MessageBox.Show("Ошибка при подключении к БД", "Ошибка подключения", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        void insert_user(string name, string sur_name) //добавление нового пользователя
        {
            try
            {
                string connectString = "Data Source=VLADIMIR-PC;Initial Catalog=data;Integrated Security=True";
                SqlConnection connetcted = new SqlConnection();
                connetcted.ConnectionString = connectString;
                connetcted.Open();
                SqlCommand command = new SqlCommand("INSERT INTO users VALUES ('" + name + "','" + sur_name + "')", connetcted);
                command.ExecuteNonQuery();
                connetcted.Close();
                MessageBox.Show("Новый пользователь " + name + " " + sur_name + " добавлен.", "Пользователь добавлен.", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch
            {
                MessageBox.Show("Ошибка при подключении к БД", "Ошибка подключения", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button_save_Click(object sender, EventArgs e)
        {

            string name = textBox_name.Text.Trim();
            string sur_name = textBox_sur_name.Text.Trim();
            string index = textBox_id.Text;

            if (name != "" & sur_name != "")
            {
                change_user(name, sur_name, index);
                clear_textbox();

                button_change.Enabled = true;
                button_save.Enabled = false;
                textBox_name.Enabled = false;
                textBox_sur_name.Enabled = false;

                refresh_Data_Grid_View();
            }
            else
            {
                MessageBox.Show("Внесите данные для сохранения", "Не хватает данных", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button_delete_Click(object sender, EventArgs e)
        {
            if (textBox_name.Text != "" & textBox_sur_name.Text != "")
            {
                string index = textBox_id.Text;
                delete_user(index);
                clear_textbox();

                refresh_Data_Grid_View();
            }
        }

        void clear_textbox()
        {
            textBox_id.Text = "";
            textBox_name.Text = "";
            textBox_sur_name.Text = "";
        }

        private void button_add_Click_1(object sender, EventArgs e)
        {
            string name = textBox_add_name.Text.Trim();
            string sur_name = textBox_add_sur_name.Text.Trim();

            if (name != "" & sur_name != "")
            {
                insert_user(name, sur_name);

                textBox_add_name.Text = "";
                textBox_add_sur_name.Text = "";

                refresh_Data_Grid_View();
            }
            else
            {
                MessageBox.Show("Внесите все необходимые данные для добавления пользователя.", "Не хватает данных", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            int indent = 8;
            int w = this.Size.Width - 40;
            int h = this.Size.Height - 265;
            dataGridView1.Size = new Size(w, h);

            Point pos_GB_edit = new Point();
            pos_GB_edit.X = dataGridView1.Location.X + dataGridView1.Size.Width / 2 - groupBox_edit.Size.Width / 2;
            pos_GB_edit.Y = dataGridView1.Location.Y + dataGridView1.Size.Height + indent;
            groupBox_edit.Location = pos_GB_edit;

            Point pos_GB_add = new Point();
            pos_GB_add.X = pos_GB_edit.X;
            pos_GB_add.Y = indent + pos_GB_edit.Y + groupBox_edit.Size.Height;
            groupBox_add.Location = pos_GB_add;

        }

       public class program_setting
        {
            public int form_H;
            public int form_W;
            public int GroupBox_add_location_X;
            public int GroupBox_add_location_Y;
            public int GroupBox_edit_location_X;
            public int GroupBox_edit_location_Y;
            public int DataGridView_H;
            public int DataGridView_W;
        } // Класс хранящий данные настроек необходимых для сохранения
     
        void Set_program_setting (program_setting PS)
       {
           this.Size = new Size(PS.form_W, PS.form_H);
           dataGridView1.Size = new Size(PS.DataGridView_W, PS.DataGridView_H);           
          
           Point GB_add_loc = new Point();
           GB_add_loc.X = PS.GroupBox_add_location_X;
           GB_add_loc.Y = PS.GroupBox_add_location_Y;

           Point GB_edit_loc = new Point();
           GB_edit_loc.X = PS.GroupBox_edit_location_X;
           GB_edit_loc.Y = PS.GroupBox_edit_location_Y;
       } // метод установки размеров и позиций элементов

        program_setting Get_program_setting()
        {
            program_setting PS = new program_setting();
            PS.form_H = this.Size.Height;
            PS.form_W = this.Size.Width;
            PS.GroupBox_add_location_X = groupBox_add.Location.X;
            PS.GroupBox_add_location_Y = groupBox_add.Location.Y;
            PS.GroupBox_edit_location_X = groupBox_edit.Location.X;
            PS.GroupBox_edit_location_Y = groupBox_edit.Location.Y;
            PS.DataGridView_H = dataGridView1.Size.Height;
            PS.DataGridView_W = dataGridView1.Size.Width;

            return PS;
        }  // метод получения всех размеров и позиций элементов в программе

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            // получаем текущие размеры и позиции элементов
            program_setting PS = new program_setting();
            PS = Get_program_setting();
            //сохраняем их в файл
            string file_name = "setting.xml";
            XmlSerializer ser = new XmlSerializer(typeof(program_setting));
            TextWriter writer = new StreamWriter(file_name);
            ser.Serialize(writer, PS);
            writer.Close();
        }

        private void textBox_name_TextChanged(object sender, EventArgs e)
        {
            if (textBox_name.Text.Trim() != "")
            {
                button_change.Enabled = true;
                button_delete.Enabled = true;
            }
            else
            {
                button_change.Enabled = false;
                button_delete.Enabled = false;
            }
        }

        private void textBox_sur_name_TextChanged(object sender, EventArgs e)
        {
            if (textBox_sur_name.Text.Trim() != "")
            {
                button_change.Enabled = true;
                button_delete.Enabled = true;
            }
            else
            {
                button_change.Enabled = false;
                button_delete.Enabled = false;
            }
        }

        void refresh_Data_Grid_View()
        {
            dataDataSet.Clear();
            this.usersTableAdapter.Fill(this.dataDataSet.Users);
            dataGridView1.Refresh();
        } // обновление данных в DataGridView

    }
}
