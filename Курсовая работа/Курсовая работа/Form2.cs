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

namespace Курсовая_работа
{
    public partial class Form2 : Form
    {
        public SqlConnection conn = new SqlConnection("Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=C:\\Users\\User\\Desktop\\курсач\\Курсовая работа\\Курсовая работа\\DB.mdf;Integrated Security=True");
        public Form2()
        {
            InitializeComponent();
        }

        private void orderBindingNavigatorSaveItem_Click(object sender, EventArgs e)
        {
            this.Validate();
            this.orderBindingSource.EndEdit();
            this.tableAdapterManager.UpdateAll(this.dBDataSet);

        }

        private void Form2_Load(object sender, EventArgs e)
        {
            this.orderTableAdapter4.Fill(this.dBDataSet4.Order);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            conn.Open();

            string query = $"INSERT INTO [Order] (clientName, goodsType, goodsTransport, date, price) VALUES (@clientName, @goodsType, @goodsTransport, @date, @price)";
            SqlCommand com = new SqlCommand(query, conn);

            com.Parameters.Add("@clientName", System.Data.SqlDbType.NVarChar);
            com.Parameters.Add("@goodsType", System.Data.SqlDbType.NVarChar);
            com.Parameters.Add("@goodsTransport", System.Data.SqlDbType.NVarChar);
            com.Parameters.Add("@date", System.Data.SqlDbType.Date);
            com.Parameters.Add("@price", System.Data.SqlDbType.Decimal);

            com.Parameters["@clientName"].Value = clientNameTextBox.Text;
            com.Parameters["@goodsType"].Value = goodsTypeTextBox.Text;
            com.Parameters["@goodsTransport"].Value = goodsObjectTextBox.Text;
            com.Parameters["@date"].Value = dateDateTimePicker.Value;
            com.Parameters["@price"].Value = Convert.ToDecimal(priceTextBox.Text);

            com.ExecuteNonQuery();

            conn.Close();
        }
    }
}
