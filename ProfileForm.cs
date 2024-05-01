using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace appFrench
{
    public partial class ProfileForm : Form
    {

        int id = 0;
        public ProfileForm(int id_us)
        {
            id = id_us;
            InitializeComponent();

            labelSet();

        }
        void labelSet() { 
        Db db = new Db();
            SqlConnection connection = db.getConnection();
            using (connection)
            {
                connection.Open();
                string quary = "SELECT Score FROM Games WHERE UserID = @id AND GameType = 1";
                SqlCommand command = new SqlCommand(quary, connection);
                command.Parameters.Add("@id", id);
                labelFirstRecord.Text = command.ExecuteScalar().ToString();
                string quary2 = "SELECT Score FROM Games WHERE UserID = @id AND GameType = 2";
                SqlCommand command2 = new SqlCommand(quary2, connection);
                command2.Parameters.Add("@id", id);
                labelFirstSecond.Text = command2.ExecuteScalar().ToString();
            }
        }
        private void buttonExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
