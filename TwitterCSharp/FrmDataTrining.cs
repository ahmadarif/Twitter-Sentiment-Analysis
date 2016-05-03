using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using System.Threading;


namespace TwitterCSharp
{
    public partial class FrmDataTrining : Form
    {
        private bool isProcessRunning = false;
        public FrmDataTrining()
        {
            InitializeComponent();
            TampilData();
            dataGridView1.Columns[0].Width = 100;        
            dataGridView1.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
            dataGridView1.Columns[1].Width = 80;
            dataGridView1.Columns[1].HeaderText = "Name";
            dataGridView1.Columns[2].Width = 470;
            dataGridView1.Columns[2].DefaultCellStyle.WrapMode = DataGridViewTriState.True;
            dataGridView1.Columns[2].HeaderText = "Text";
            dataGridView1.Columns[3].Visible = false;

            dataGridView1.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            DataGridViewCheckBoxColumn chk = new DataGridViewCheckBoxColumn();
            dataGridView1.Columns.Add(chk);
            chk.HeaderText = "Positive";
            chk.Name = "pos";
            chk.Width = 50;
            DataGridViewCheckBoxColumn chk1 = new DataGridViewCheckBoxColumn();
            dataGridView1.Columns.Add(chk1);
            chk1.HeaderText = "Negative";
            chk1.Name = "neg";
            chk1.Width = 50;
             
        }
        private void TampilData()
        {
            String config = "server=localhost; userid = root; database = twitter";
            MySqlConnection con = new MySqlConnection(config);
            string query = "SELECT ID,nama,text,normal FROM crawling WHERE ID NOT IN ( SELECT ID FROM trining)";
            try
            {
                con.Open();
                MySqlDataAdapter MyDA = new MySqlDataAdapter(query, con);
                DataTable table = new DataTable();
                MyDA.Fill(table);
                BindingSource bSource = new BindingSource();
                bSource.DataSource = table;
                dataGridView1.DataSource = bSource;
                                       
            }
            catch (MySqlException ex)
            {
                MessageBox.Show(ex.ToString());
            }
            finally
            {
                con.Close();
            }
            label1.Text = "Data: " + (dataGridView1.RowCount - 1).ToString();
        }
        
        private void button1_Click_1(object sender, EventArgs e)
        {
            int i = 0;
            List<int> ChkedRowPos = new List<int>();
            List<int> ChkedRowNeg = new List<int>();
            List<int> ChkedRowNet = new List<int>();
            String config = "server=localhost; userid = root; database = twitter";
            MySqlConnection con = new MySqlConnection(config);
            string query = "INSERT INTO twitter.trining(ID,nama,text,normal,sentiment) VALUES (@ID, @nama, @text,@normal,@sentiment)";

            for (i = 0; i <= dataGridView1.RowCount - 1; i++)
            {
                if (Convert.ToBoolean(dataGridView1.Rows[i].Cells["pos"].Value) == true)
                {
                    ChkedRowPos.Add(i);
                }
                if (Convert.ToBoolean(dataGridView1.Rows[i].Cells["neg"].Value) == true)
                {
                    ChkedRowNeg.Add(i);
                }
                
            }

            if (ChkedRowPos.Count == 0 && ChkedRowNeg.Count == 0 && ChkedRowNet.Count == 0)
            {
                MessageBox.Show("Please choose positive or negative.....");
                return;
            }
            if (isProcessRunning)
            {
                MessageBox.Show("A process is already running.");
                return;
            }
            ProgressDialog progressDialog = new ProgressDialog();

            Thread backgroundThread = new Thread(
                new ThreadStart(() =>
                {
                    isProcessRunning = true;
                    progressDialog.SetIndeterminate(true);
                    foreach (int j in ChkedRowPos)
                    {
                        try
                        {
                            MySqlCommand cmd = new MySqlCommand(query, con);
                            con.Open();
                            cmd.Parameters.AddWithValue("@ID", dataGridView1.Rows[j].Cells[2].Value.ToString());
                            cmd.Parameters.AddWithValue("@nama", dataGridView1.Rows[j].Cells[3].Value.ToString());
                            cmd.Parameters.AddWithValue("@text", dataGridView1.Rows[j].Cells[4].Value.ToString());
                            cmd.Parameters.AddWithValue("@normal", dataGridView1.Rows[j].Cells[5].Value.ToString());
                            cmd.Parameters.AddWithValue("@sentiment", "Positive");
                            cmd.ExecuteNonQuery();
                            cmd.Parameters.Clear();
                            con.Close();
                        }
                        catch (Exception ex)
                        {
                            con.Close();
                            MessageBox.Show(ex.ToString() + dataGridView1.Rows[j].Cells[0].Value.ToString());
                        }

                    }
                    foreach (int j in ChkedRowNeg)
                    {
                        try
                        {
                            MySqlCommand cmd = new MySqlCommand(query, con);
                            con.Open();
                            cmd.Parameters.AddWithValue("@ID", dataGridView1.Rows[j].Cells[2].Value.ToString());
                            cmd.Parameters.AddWithValue("@nama", dataGridView1.Rows[j].Cells[3].Value.ToString());
                            cmd.Parameters.AddWithValue("@text", dataGridView1.Rows[j].Cells[4].Value.ToString());
                            cmd.Parameters.AddWithValue("@normal", dataGridView1.Rows[j].Cells[5].Value.ToString());
                            cmd.Parameters.AddWithValue("@sentiment", "Negative");
                            cmd.ExecuteNonQuery();
                            cmd.Parameters.Clear();
                            con.Close();
                        }
                        catch (Exception ex)
                        {
                            con.Close();
                            MessageBox.Show(ex.ToString() );
                        }
                    }
                   
                    
                    if (progressDialog.InvokeRequired)
                        progressDialog.BeginInvoke(new Action(() => progressDialog.Close()));
                    isProcessRunning = false;
                }
            ));

            backgroundThread.Start();
            progressDialog.ShowDialog();
            TampilData();
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == dataGridView1.Columns["pos"].Index)
            {
                dataGridView1[1, e.RowIndex].Value = "false";
                            }
            if (e.ColumnIndex == dataGridView1.Columns["neg"].Index)
            {
                dataGridView1[0, e.RowIndex].Value = "false";
                
            }
            
        }
        private void TampilDeleteData()
        {
            dataGridView2.Columns.Clear();
            dataGridView2.DataSource = null;
            String config = "server=localhost; userid = root; database = twitter";
            MySqlConnection con = new MySqlConnection(config);
            string query = "SELECT * FROM trining ";
            try
            {
                con.Open();
                MySqlDataAdapter MyDA = new MySqlDataAdapter(query, con);
                DataTable table = new DataTable();
                MyDA.Fill(table);
                BindingSource bSource = new BindingSource();
                bSource.DataSource = table;
                dataGridView2.DataSource = bSource;

            }
            catch (MySqlException ex)
            {
                MessageBox.Show(ex.ToString());
            }
            finally
            {
                con.Close();
            }
            dataGridView2.Columns[0].Width = 100;
            dataGridView2.Columns[1].Width = 80;
            dataGridView2.Columns[1].HeaderText = "Name";
            dataGridView2.Columns[2].Width = 300;
            dataGridView2.Columns[2].DefaultCellStyle.WrapMode = DataGridViewTriState.True;
            dataGridView2.Columns[2].HeaderText = "Text";
            dataGridView2.Columns[3].Width = 200;
            dataGridView2.Columns[3].DefaultCellStyle.WrapMode = DataGridViewTriState.True;
            dataGridView2.Columns[3].HeaderText = "Preprocessing";
            dataGridView2.Columns[4].Width = 70;
            dataGridView2.Columns[4].HeaderText = "Sentiment";
            DataGridViewButtonColumn btn = new DataGridViewButtonColumn();
            dataGridView2.Columns.Add(btn);
            btn.HeaderText = "Delete Data";
            btn.Text = "Delete";
            btn.Name = "btn";
            btn.Width = 60;
            btn.UseColumnTextForButtonValue = true;
            dataGridView2.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
        }
        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch ((sender as TabControl).SelectedIndex)
            {
                case 0:
                    TampilData();
                    break;
                case 1:
                    TampilDeleteData();
                    break;
            } 
        }

        private void dataGridView2_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == dataGridView2.Columns["btn"].Index)
            {
                MessageBoxManager.Yes = "Yes";
                MessageBoxManager.No = "No";
                MessageBoxManager.Register();
                DialogResult hasil = MessageBox.Show("Delete data? \"" + dataGridView2[2, e.RowIndex].Value.ToString() + "\"", "Question",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);
                MessageBoxManager.Unregister();
                if (hasil == DialogResult.Yes)
                {
                    String config = "server=localhost; userid = root; database = twitter";
                    MySqlConnection con = new MySqlConnection(config);
                    string query = "DELETE FROM twitter.trining WHERE ID =@ID";
                    try
                    {
                        MySqlCommand cmd = new MySqlCommand(query, con);
                        con.Open();
                        var errro = dataGridView2[1, e.RowIndex].Value.ToString();
                        cmd.Parameters.AddWithValue("@ID", dataGridView2[0, e.RowIndex].Value.ToString());
                        cmd.ExecuteNonQuery();
                        cmd.Parameters.Clear();
                        con.Close();
                        MessageBox.Show(String.Format("ID {0} Successfully deleted ", dataGridView2[0, e.RowIndex].Value.ToString()));
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message.ToString());
                        con.Close();
                    }
                    TampilDeleteData();
                }
                else if (hasil == DialogResult.No)
                {

                }
            }
        }

        private void FrmDataTrining_Load(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        
    }
}
