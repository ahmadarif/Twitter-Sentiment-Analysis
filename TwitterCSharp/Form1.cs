using System;
using System.Net;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using Tweetinvi;
using Tweetinvi.Core.Enum;
using MySql.Data.MySqlClient;
using System.Windows.Forms.DataVisualization.Charting;
using Tweetinvi.WebLogic.Utils;
using System.Net.Http;
using System.Threading;
using Tweetinvi.Core.Interfaces.Models.Parameters;
using NaiveBayesianClasisifier;
using System.Collections.Generic;

namespace TwitterCSharp
{
    public partial class Form1 : Form
    {
        private const string userKey = "382934293-x2kmISFXel7FerxWlVg4xf217ISeL2TcekNFs11B";
        private const string userSecret = "qFaGWqjdg8MRpxIQuzyV0U6aRAvJmR9z7QA6zEqntc7Vx";
        private const string consumerKey = "4Z76kiHWpdTiAdtjoxFlS2arf";
        private const string consumerSecret = "DJGNGu51HRL3CRaPaodyFO0vAEBOB1T8vrrNzr8m511Du5Dn66";
        private int nbTweetDetected = 0;
        private bool isProcessRunning = false;
        private bool simpanStream = true;
        private bool Stream = true;

        public Form1()
        {
            InitializeComponent();
            //WebRequest.DefaultWebProxy.Credentials = new NetworkCredential("josuasipayung", "manchesterunited");
            TwitterCredentials.SetCredentials(userKey, userSecret, consumerKey, consumerSecret);
            var user = User.GetLoggedUser();
            textBox1.Text = user.ScreenName;
            btnAddTri.Enabled = false;
            btnClass.Enabled = false;
            btnSmpnKlas.Enabled = false;
            
        }
     
        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox4.Text == "" || textBox2.Text=="")
            {
               if (textBox4.Text == "") MessageBox.Show("Fill Twitter ID.....");
               if (textBox2.Text == "") MessageBox.Show("Fill number of tweet.....");
            }
            else
            {
                nbTweetDetected = 0;
                var searchParameter = Search.CreateTweetSearchParameter(textBox4.Text);
                searchParameter.MaximumNumberOfResults = Convert.ToInt32(textBox2.Text);
                //searchParameter.TweetSearchType = TweetSearchType.OriginalTweetsOnly;
                
                DataTable d = new DataTable();
                d.Columns.Add("ID");
                d.Columns.Add("Name");
                d.Columns.Add("Date");
                d.Columns.Add("Text");
                
                if (isProcessRunning)
                {
                    MessageBox.Show("A process is already running.");
                    return;
                }

                // Initialize the dialog that will contain the progress bar
                ProgressDialog progressDialog = new ProgressDialog();

                // Initialize the thread that will handle the background process
                Thread backgroundThread = new Thread(
                    new ThreadStart(() =>
                    {
                        // Set the flag that indicates if a process is currently running
                        isProcessRunning = true;

                        // Set the dialog to operate in indeterminate mode
                        progressDialog.SetIndeterminate(true);
                        try
                        {
                            var tweets = Search.SearchTweets(searchParameter);
                            // Pause the thread for five seconds
                            foreach (var tweet in tweets)
                            {
                                nbTweetDetected = nbTweetDetected + 1;
                                if (tweet.Retweeted == false)
                                {
                                    var tanggal = String.Format("{0}-{1}-{2} {3}:{4}:{5}", tweet.CreatedAt.Year,
                                        tweet.CreatedAt.Month, tweet.CreatedAt.Day, tweet.CreatedAt.Hour,
                                        tweet.CreatedAt.Minute, tweet.CreatedAt.Second);
                                    try
                                    {
                                        d.Rows.Add(tweet.IdStr.ToString(), tweet.Creator.ScreenName.ToString(), tanggal.ToString(), tweet.Text);
                                    }
                                    catch (Exception ex)
                                    {
                                        MessageBox.Show(ex.ToString());
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.ToString());
                        }

                        // Close the dialog if it hasn't been already
                        if (progressDialog.InvokeRequired)
                            progressDialog.BeginInvoke(new Action(() => progressDialog.Close()));

                        // Reset the flag that indicates if a process is currently running
                        isProcessRunning = false;
                    }
                ));

                // Start the background process thread
                backgroundThread.Start();
                progressDialog.Text = "Please wait..... " + textBox4.Text;
                // Open the dialog
                progressDialog.ShowDialog();
                dataGridView1.DataSource = d;
                textBox3.Text = nbTweetDetected.ToString();      
                dataGridView1.Columns[0].Width = 100;
                dataGridView1.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
                dataGridView1.Columns[1].Width = 100;
                dataGridView1.Columns[2].Width = 100;
                dataGridView1.Columns[3].Width = 500;
                dataGridView1.Columns[3].DefaultCellStyle.WrapMode = DataGridViewTriState.True;
                dataGridView1.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            }
            
        }
        
        private void btnSave_Click(object sender, EventArgs e)
        {
            String config = "server=localhost; userid = root; database = twitter";
            MySqlConnection con = new MySqlConnection(config);
            string query = "INSERT INTO twitter.crawling(ID, nama, tanggal, text) VALUES (@ID, @nama, @tanggal, @text)";
            string errmsg = "";
            int berhasil = 0;
            int gagal = 0;
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
                    for (int row = 0; row < dataGridView1.Rows.Count - 1; row++)
                    {
                        try
                        {
                            MySqlCommand cmd = new MySqlCommand(query, con);
                            con.Open();
                            cmd.Parameters.AddWithValue("@ID", dataGridView1.Rows[row].Cells[0].Value.ToString());
                            cmd.Parameters.AddWithValue("@nama", dataGridView1.Rows[row].Cells[1].Value.ToString());
                            cmd.Parameters.AddWithValue("@tanggal", dataGridView1.Rows[row].Cells[2].Value.ToString());
                            cmd.Parameters.AddWithValue("@text", dataGridView1.Rows[row].Cells[3].Value.ToString());

                            cmd.ExecuteNonQuery();
                            cmd.Parameters.Clear();
                            berhasil++;
                            con.Close();
                        }
                        catch (Exception)
                        {
                            con.Close();
                            gagal++;
                        }
                    }
                    errmsg = String.Format(">{0} records successfully saved \r\n>{1} records not saved", berhasil.ToString(), gagal.ToString());
                  
                    MessageBox.Show(errmsg);
                    if (progressDialog.InvokeRequired)
                        progressDialog.BeginInvoke(new Action(() => progressDialog.Close()));
                    isProcessRunning = false;
                }
            ));
            backgroundThread.Start();
            progressDialog.Text="Please wait.....";  
            progressDialog.ShowDialog();     
        }

        private void cmdAmbil_Click(object sender, EventArgs e)
        {
            tampilData();
            btnAddTri.Enabled = true;
            btnClass.Enabled = true;
            btnSmpnKlas.Enabled = true;
        }

        private void PreCasefolding()
        {
            for (int row = 0; row < dataGridView2.Rows.Count - 1; row++)
            {
                dataGridView2.Rows[row].Cells[3].Value = dataGridView2.Rows[row].Cells[2].Value;
                dataGridView2.Rows[row].Cells[3].Value = dataGridView2.Rows[row].Cells[3].Value.ToString().ToLower();
              
            }
        }
        private void PreNormalisasi()
        {
            for (int row = 0; row < dataGridView2.Rows.Count - 1; row++)
            {  
                normalisasi norm = new normalisasi();
                dataGridView2.Rows[row].Cells[3].Value = norm.normal(dataGridView2.Rows[row].Cells[3].Value.ToString());
            }
        }
        private void PreConvert()
        {
            for (int row = 0; row < dataGridView2.Rows.Count - 1; row++)
            {
                convert conv = new convert();
                dataGridView2.Rows[row].Cells[3].Value = conv.convr(dataGridView2.Rows[row].Cells[3].Value.ToString());
            }
        }
        private void PreTokenizing()
        {
            for (int row = 0; row < dataGridView2.Rows.Count - 1; row++)
            {
                tokenizing token = new tokenizing();
                dataGridView2.Rows[row].Cells[3].Value = token.token(dataGridView2.Rows[row].Cells[3].Value.ToString());
            }
        }
        private void PreStopword()
        {
            for (int row = 0; row < dataGridView2.Rows.Count - 1; row++)
            {
                stopword stop = new stopword();
                dataGridView2.Rows[row].Cells[3].Value = stop.stopWord(dataGridView2.Rows[row].Cells[3].Value.ToString());
            }
        }
        private void PreSteaming()
        {
            stemming stem = new stemming();
            stem.listKamus();
            
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
                    try
                    {
                        for (int row = 0; row < (dataGridView2.Rows.Count - 1); row++)
                        {
                            String text = dataGridView2.Rows[row].Cells[3].Value.ToString();
                            String hasil = "";
                        
                                string[] words = text.Split(' ');
                                for (int i = 0; i < words.Length; i++)
                                {
                                    words[i] = stem.Stemming(words[i]);
                                }
                                hasil = String.Join(" ", words);
                            dataGridView2.Rows[row].Cells[3].Value = hasil.ToString();
                        }
                        Stream = false;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.ToString());
                        Stream = true;
                    }
   
                    if (progressDialog.InvokeRequired)
                        progressDialog.BeginInvoke(new Action(() => progressDialog.Close()));
                    isProcessRunning = false;
                }
            ));
            backgroundThread.Start();
            progressDialog.Text="Proses Steamming.....";
            progressDialog.ShowDialog();
            
        }
        private void simpanSteaming()
        {
            String config = "server=localhost; userid = root; database = twitter";
            MySqlConnection con = new MySqlConnection(config);
            string query = "update twitter.crawling set normal=@text where ID=@ID;";
            string query2 = "delete from twitter.crawling where ID=@ID;";
            string errmsg = "";
            int berhasil = 0; int dihapus = 0; int gagal = 0;
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
                    for (int row = 0; row < dataGridView2.Rows.Count - 1; row++)
                    {
                        try
                        {
                            con.Open();
                            string text = dataGridView2.Rows[row].Cells[3].Value.ToString();
                            if (text == "")
                            {
                                MySqlCommand cmd = new MySqlCommand(query2, con);
                                cmd.Parameters.AddWithValue("@ID", dataGridView2.Rows[row].Cells[0].Value.ToString());
                                cmd.ExecuteNonQuery();
                                cmd.Parameters.Clear();
                                dihapus++;
                            }
                            else
                            {
                                MySqlCommand cmd = new MySqlCommand(query, con);
                                cmd.Parameters.AddWithValue("@ID", dataGridView2.Rows[row].Cells[0].Value.ToString());
                                cmd.Parameters.AddWithValue("@text", dataGridView2.Rows[row].Cells[3].Value.ToString());
                                cmd.ExecuteNonQuery();
                                cmd.Parameters.Clear();
                                berhasil++;
                            }
                            
                            con.Close();
                        }
                        catch (Exception)
                        {
                            con.Close();
                            gagal++;
                        }
                    }
                    errmsg = String.Format(">{0} records saved successfully \r\n>{1} records not saved \r\n>{2} records deleted", berhasil.ToString(), gagal.ToString(),dihapus.ToString());
                    
                    MessageBox.Show(errmsg);
                    if (progressDialog.InvokeRequired)
                        progressDialog.BeginInvoke(new Action(() => progressDialog.Close()));
                    isProcessRunning = false;
                }
            ));
            backgroundThread.Start();
            progressDialog.Text = "Saving data.....";
            progressDialog.ShowDialog();
            simpanStream = false;
        }  
        private void tampilData()
        {
            dataGridView2.Columns.Clear();
            dataGridView2.DataSource = null;

            String config = "server=localhost; userid = root; database = twitter";
            MySqlConnection con = new MySqlConnection(config);
            string query = "select ID,nama,text,normal from crawling";
            MySqlDataAdapter MyDA = new MySqlDataAdapter(query, con);
            DataTable table = new DataTable();
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
                    try
                    {
                        con.Open();
                        table.Columns.Add("ID");
                        table.Columns.Add("Nama");
                        table.Columns.Add("Text");
                        MyDA.Fill(table);
                    }
                    catch (MySqlException ex)
                    {
                        MessageBox.Show(ex.ToString());
                    }
                    finally
                    {
                        con.Close();
                    }
                    if (progressDialog.InvokeRequired)
                        progressDialog.BeginInvoke(new Action(() => progressDialog.Close()));
                    isProcessRunning = false;
                }
            ));
            backgroundThread.Start();
            progressDialog.Text="Getting data.....";
            progressDialog.ShowDialog();
            BindingSource bSource = new BindingSource();
            bSource.DataSource = table;
            dataGridView2.DataSource = bSource;
            dataGridView2.Columns[0].Width = 100;
            dataGridView2.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
            dataGridView2.Columns[1].Width = 100;
            dataGridView2.Columns[2].Width = 600;
            dataGridView2.Columns[3].Visible=false;
            dataGridView2.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            textBox5.Text = (dataGridView2.RowCount - 1).ToString();
        }
        private void ProKlasifikasi()
        {
            if (dataGridView2.ColumnCount == 4) dataGridView2.Columns.Add("Sentiment", "Sentiment");
            dataGridView2.Columns[0].Width = 100;
            dataGridView2.Columns[1].Width = 100;
            dataGridView2.Columns[2].Width = 500;
            dataGridView2.Columns[4].Width = 100;
            string connectionSQL = "server=localhost;database=twitter;uid=root;password=;";
            MySqlConnection db = new MySqlConnection(connectionSQL);
            db.Open();
            MySqlCommand dbcmd = db.CreateCommand();
            string sql = "SELECT normal,sentiment FROM trining";
            dbcmd.CommandText = sql;
            MySqlDataReader reader = dbcmd.ExecuteReader();
            List<Document> _trainCorpus = new List<Document>();
            while (reader.Read())
            {
                var dokumen = new Document(reader.GetString(1).ToString(), reader.GetString(0).ToString());
                _trainCorpus.Add(dokumen);
            }

            db.Close();
            
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
                    try
                    {
                        for (int row = 0; row < (dataGridView2.Rows.Count- 1); row++)
                        {
                            String Hasil="";
                            String teks=dataGridView2.Rows[row].Cells[3].Value.ToString();
                                var c = new Classifier(_trainCorpus);
                                var pos = c.IsInClassProbability("Positive", teks);
                                var neg = c.IsInClassProbability("Negative", teks);                            
                                if (pos > (neg)) Hasil = "Positive";
                                else  Hasil = "Negative";                                         
                            dataGridView2.Rows[row].Cells[4].Value = Hasil.ToString();
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.ToString());
                    }
                    if (progressDialog.InvokeRequired)
                        progressDialog.BeginInvoke(new Action(() => progressDialog.Close()));
                    isProcessRunning = false;
                }
            ));

            backgroundThread.Start();
            progressDialog.Text = "Classification process.....";
            progressDialog.ShowDialog();          

        }
        
        
        private void btnClass_Click(object sender, EventArgs e)
        {
            PreCasefolding(); 
            PreNormalisasi(); 
            PreConvert(); 
            PreTokenizing(); 
            PreStopword();
            PreSteaming();
            if (simpanStream) simpanSteaming();
            ProKlasifikasi();
        }
        
        private void btnAddTri_Click(object sender, EventArgs e)
        {
            
            PreCasefolding();
            PreNormalisasi();
            PreConvert();
            PreTokenizing();
            PreStopword();
            if (Stream) PreSteaming();
            if (simpanStream) simpanSteaming();
            
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
                    Form frm2 = new FrmDataTrining();
                    frm2.ShowDialog();
                    if (progressDialog.InvokeRequired)
                        progressDialog.BeginInvoke(new Action(() => progressDialog.Close()));
                    isProcessRunning = false;
                }
            ));

            backgroundThread.Start();
            progressDialog.Text = "Please wait..... ";
            progressDialog.ShowDialog();
           
        }

        private void btnSmpnKlas_Click(object sender, EventArgs e)
        {
            String config = "server=localhost; userid = root; database = twitter";
            MySqlConnection con = new MySqlConnection(config);
            string query = "update twitter.crawling set sentiment=@sentiment where ID=@ID;";
            string errmsg = "";
            int berhasil = 0;
            int gagal = 0;
            
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
                    for (int row = 0; row < dataGridView2.Rows.Count - 1; row++)
                    {
                        try
                        {
                            MySqlCommand cmd = new MySqlCommand(query, con);
                            con.Open();
                            cmd.Parameters.AddWithValue("@ID", dataGridView2.Rows[row].Cells[0].Value.ToString());
                            cmd.Parameters.AddWithValue("@sentiment", dataGridView2.Rows[row].Cells[4].Value.ToString());

                            cmd.ExecuteNonQuery();
                            cmd.Parameters.Clear();
                            berhasil++;
                            con.Close();
                        }
                        catch (Exception)
                        {
                            con.Close();
                            gagal++;
                        }
                    }
                    errmsg = String.Format(">{0} records sucessfully saved \r\n>{1} records not saved ", berhasil.ToString(), gagal.ToString());
                    MessageBox.Show(errmsg);
                    if (progressDialog.InvokeRequired)
                        progressDialog.BeginInvoke(new Action(() => progressDialog.Close()));
                    isProcessRunning = false;
                }
            ));
            backgroundThread.Start();
            progressDialog.ShowDialog(); 
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch ((sender as TabControl).SelectedIndex)
            {
                case 0:
                    break;
                case 1:
                    break;
                case 2:
                    String config = "server=localhost; userid = root; database = twitter";
                    MySqlConnection con = new MySqlConnection(config);
                    string query = "SELECT COUNT( ID ) AS ID, MIN( sentiment ) AS sentimen FROM crawling GROUP BY sentiment order by sentiment desc";
                    MySqlDataAdapter da = new MySqlDataAdapter(query, con);
                    DataSet ds = new DataSet();
                    da.Fill(ds,"Salary");
                    chart1.DataSource = ds.Tables["Salary"];

                    chart1.Series["Series1"].XValueMember = "sentimen";
                    chart1.Series["Series1"].YValueMembers = "ID";
      
                  
                    chart1.Series["Series1"].ChartType = SeriesChartType.Pie;  // Set chart type like Bar chart, Pie chart
                    chart1.Series["Series1"].IsValueShownAsLabel = true;  // To show chart value 
                    break;
            }
        }

        private void button2_Click(object sender, EventArgs e)
               {
                   this.chart1.SaveImage("D:\\chart1.Jpeg", ChartImageFormat.Jpeg);
                   MessageBox.Show("Chart Saved Successfully", Application.ProductName,
                   MessageBoxButtons.OK, MessageBoxIcon.Information);
               }

        private void btnGrafik_Click(object sender, EventArgs e)
        {
            String config = "server=localhost; userid = root; database = twitter";
            MySqlConnection con = new MySqlConnection(config);
            string query = "SELECT COUNT( ID ) AS ID, MIN( sentiment ) AS sentimen FROM crawling GROUP BY sentiment order by sentiment desc";
            MySqlDataAdapter da = new MySqlDataAdapter(query, con);
            DataSet ds = new DataSet();
            da.Fill(ds, "Salary");
            chart1.DataSource = ds.Tables["Salary"];

            chart1.Series["Series1"].XValueMember = "sentimen";
            chart1.Series["Series1"].YValueMembers = "ID";
            
            
            
            chart1.Series["Series1"].ChartType = SeriesChartType.Pie;  // Set chart type like Bar chart, Pie chart
            chart1.Series["Series1"].IsValueShownAsLabel = true;  // To show chart value 
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void chart1_Click(object sender, EventArgs e)
        {

        }

        private void label6_Click(object sender, EventArgs e)
        {

        }

        private void label7_Click(object sender, EventArgs e)
        {

        }

        private void label8_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {

        }

        private void tabPage3_Click(object sender, EventArgs e)
        {

        }
        
    }
}
