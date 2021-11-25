using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System;

using System.Windows.Forms;
using ServisTest.ServiceReference1;
using System.IO;
using System.Diagnostics;

namespace ServisTest
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        BasicHttpBinding binding = new BasicHttpBinding();

     

        private ServiceReference1.EFaturaEntegrasyon2SoapClient client;

        private string token;
        public long efaturaCredit = 0;
        public long earsivCredit = 0;
        public long eirsaliyeCredit = 0;
        private void Form1_Load(object sender, EventArgs e)
        {
            textBox1.Text = "efatura1";
            textBox2.Text = "Efatura123!";

            binding.Name = "IntegrationServiceSoap";
            binding.CloseTimeout = TimeSpan.Parse("00:01:00");
            binding.OpenTimeout = TimeSpan.Parse("00:01:00");
            binding.ReceiveTimeout = TimeSpan.Parse("00:01:00");
            binding.SendTimeout = TimeSpan.Parse("00:01:00");
            binding.AllowCookies = false;
            binding.BypassProxyOnLocal = false;
            binding.HostNameComparisonMode = HostNameComparisonMode.StrongWildcard;
            binding.MaxBufferSize = int.MaxValue; // 65536;
            binding.MaxBufferPoolSize = int.MaxValue;
            binding.MaxReceivedMessageSize = int.MaxValue; // 65536;
            binding.MessageEncoding = WSMessageEncoding.Text;
            binding.TextEncoding = Encoding.UTF8;
            binding.TransferMode = TransferMode.Buffered;
            binding.UseDefaultWebProxy = true;
            binding.ReaderQuotas.MaxArrayLength = int.MaxValue; // 16384;
            binding.ReaderQuotas.MaxBytesPerRead = 4096;
            binding.ReaderQuotas.MaxNameTableCharCount = int.MaxValue; // 16384;

            client = new EFaturaEntegrasyon2SoapClient(binding, new EndpointAddress("https://efintws.turkkep.com.tr/EFaturaEntegrasyon2.asmx?wsdl"));
        }
        

        private void btnGirisYap_Click(object sender, EventArgs e)
        {
            token = client.OturumAc(textBox1.Text,textBox2.Text);
            MessageBox.Show("Oturum Açıldı");

        }

        private void btnBakiyeSorgula_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(token))
            {
                efaturaCredit = client.EFaturaKalanKontorSorgula(token);
                earsivCredit = client.EArsivKalanKontorSorgula(token);
                eirsaliyeCredit = client.EIrsaliyeKalanKontorSorgula(token);

                label1.Text = "E-Fatura Bakiye: " + efaturaCredit +
                "\nE-Arşiv Bakiye: " + earsivCredit +
                "\nE-İrsaliye Bakiye: " + eirsaliyeCredit;
            }
        }

        private void btnFaturaGoster_Click(object sender, EventArgs e)
        {
            dataGridView1.DataSource = client.YeniGelenFaturalariListele(token);
        }

        private void dataGridView1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            var invoice = dataGridView1.CurrentRow.Cells[3].Value.ToString();

            var bytes = client.GelenFaturaPdfAl(token, Convert.ToInt32(invoice));
            if (bytes != null && bytes.Length > 0)
            {
                File.WriteAllBytes(@"C:\TEMP\" + invoice + ".pdf", bytes);
                Process.Start(@"C:\TEMP\" + invoice + ".pdf");
            }
            else
            {
                MessageBox.Show("Fatura bulunamadı");
            }
        }
    }
}
