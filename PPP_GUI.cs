namespace PcPartPrices
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.IO;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Windows.Forms;
    using HtmlAgilityPack;
    using static System.Windows.Forms.ListViewItem;

    public partial class PPP_GUI : Form
    {
        private static void Log(
            string message,
            [CallerFilePath] string file = null,
            [CallerLineNumber] int line = 0)
        {
            Console.WriteLine("{0} ({1}): {2}", Path.GetFileName(file), line, message);
        }

        public static string DictionaryToString(Dictionary<string, string> dictionary)
        {
            string dictionaryString = "{";
            foreach (KeyValuePair<string, string> keyValues in dictionary)
            {
                dictionaryString += keyValues.Key + " : " + keyValues.Value + ", \n";
            }

            return dictionaryString.TrimEnd(',', ' ') + "}";
        }

        public static double AvgPrice(string query)
        {
            try
            {
                HtmlWeb web = new HtmlWeb();
                HtmlAgilityPack.HtmlDocument document = web.Load("https://www.ebay.com/sch/i.html?_from=R40&_nkw=" + query);
                HtmlNode[] nodes = document.DocumentNode.SelectNodes("//span[@class='s-item__price']").ToArray();
                int count = 0;
                float average = 0;
                var regex = new Regex(@"\d{1,3}(?:[.,]\d{3})*(?:[.,]\d{2})");
                foreach (HtmlNode item in nodes)
                {
                    if (item.InnerHtml.Substring(0, 1).Equals("$") && count < 4)
                    {
                        average += float.Parse(regex.Match(item.InnerHtml).Value);
                    }
                    else if (count >= 4)
                    {
                        break;
                    }

                    count++;
                }

                return average / (count - 1);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return -1;
            }
        }

        private static List<string> GetParts(SystemInfo si)
        {
            List<string> myParts = new List<string>();
            myParts.Add("Operating System:");
            myParts.Add(si.myComputer["os_name"]);
            myParts.Add("CPU:");
            myParts.Add(si.myComputer["cpu_name"]);
            myParts.Add("Motherboard:");
            myParts.Add(si.myComputer["mb_name"]);
            int ramspeed = Convert.ToInt32(si.myComputer["some_ram_speed"]);
            int closest = si.ramSpeeds.Aggregate((x, y) => Math.Abs(x - ramspeed) < Math.Abs(y - ramspeed) ? x : y);
            myParts.Add("Memory:");
            myParts.Add(si.myComputer["total_ram_capacity"] + " " + closest + "MHZ");

            myParts.Add("Storage:");
            foreach (StorageDevice store in si.storageDevices)
            {
                myParts.Add(store.model + " " + store.size);
            }

            myParts.Add("GPU:");
            foreach (GPU gpu in si.gpus)
            {
                if (!gpu.name.Contains("Intel"))
                {
                    myParts.Add(gpu.name);
                }
            }

            myParts.Add("Displays:");
            foreach (string display in si.displays)
            {
                myParts.Add(display);
            }

            myParts.Add("Sound Devices:");
            foreach (SoundDevice device in si.soundDevices)
            {
                if (!myParts.Contains(device.name))
                {
                    myParts.Add(device.name);
                }
            }

            return myParts;
        }

        private ProgressBar pBar1 = new ProgressBar();
        private double total = 0.0;

        public Dictionary<string, double> GetPrices(List<string> myParts)
        {
            this.total = 0;
            Dictionary<string, double> myPartsPrices = new Dictionary<string, double>();

            string query = string.Empty;
            int i = 0;
            foreach (string part in myParts)
            {
                if (part != null && !part.Equals(string.Empty))
                {
                    if (part.Substring(part.Length - 1).Equals(":"))
                    {
                        myPartsPrices.Add(part, -69);
                        this.listView1.Items[i].SubItems.Add(string.Empty);
                    }
                    else
                    {
                        query = part.Replace(" ", "+");
                        double avg = AvgPrice(query);
                        Console.WriteLine(string.Empty + part + " - $" + avg);
                        try
                        {
                            myPartsPrices.Add(part, avg);
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine("extra part " + e);
                        }

                        this.total += avg;
                        string val2 = string.Format("${0:N2}", avg);
                        this.listView1.Items[i].SubItems.Add(val2);
                    }
                }

                this.pBar1.PerformStep();

                // pBar1.Refresh();
                i++;
            }

            return myPartsPrices;
        }

        private List<string> myParts1 = new List<string>();
        private Dictionary<string, double> myPartsPrices1 = new Dictionary<string, double>();
        private ListView listView1 = new ListView();
        private Button button1 = new Button();
        private Button button2 = new Button();
        private Label label1 = new Label();

        public PPP_GUI()
        {
            try
            {
                this.Controls.Add(this.listView1);
                SystemInfo si = new SystemInfo();

                this.myParts1 = GetParts(si);

                this.InitializeComponent();

                this.listView1.Location = new System.Drawing.Point(12, 12);
                this.Controls.Add(this.listView1);
                this.listView1.Name = "ListView1";
                this.listView1.Size = new System.Drawing.Size(445, 600);
                this.listView1.View = View.Details;
                this.listView1.GridLines = true;

                try
                {
                    foreach (string p in this.myParts1)
                    {
                        if (p != null && !p.Equals(string.Empty))
                        {
                            if (p.Substring(p.Length - 1).Equals(":"))
                            {
                                ListViewItem item5 = new ListViewItem(p, 0)
                                {
                                    Font = new Font(this.listView1.Font, FontStyle.Bold),
                                };
                                this.listView1.Items.Add(item5);
                            }
                            else
                            {
                                ListViewItem item5 = new ListViewItem(p, 0)
                                {
                                    Checked = true,
                                };
                                this.listView1.Items.Add(item5);
                            }
                        }
                        else
                        {
                            ListViewItem item5 = new ListViewItem(p, 0)
                            {
                                Checked = true,
                            };
                            this.listView1.Items.Add(item5);
                        }
                    }
                }
                catch (Exception e)
                {
                    Log(e.Message);
                }

                ListViewItem item = new ListViewItem(string.Empty, 0)
                {
                    Font = new Font(this.listView1.Font, FontStyle.Bold),
                };
                this.listView1.Items.Add(item);

                ListViewItem itemf = new ListViewItem("Total Cost:", 0)
                {
                    Font = new Font(this.listView1.Font, FontStyle.Bold),
                };
                this.listView1.Items.Add(itemf);

                this.listView1.Columns.Add("Item ", -2, HorizontalAlignment.Left);
                this.listView1.Columns.Add("Estimated Price", -2, HorizontalAlignment.Left);

                this.button1.Location = new Point(475, 62);
                this.button1.Height = 50;
                this.button1.Width = 100;
                this.button1.Text = "get prices";

                this.button2.Location = new Point(475, 182);
                this.button2.Height = 50;
                this.button2.Width = 100;
                this.button2.Text = "export to .txt";

                this.pBar1.Location = new Point(475, 12);
                this.pBar1.Visible = true;
                this.pBar1.Minimum = 1;
                this.pBar1.Maximum = this.myParts1.Count();
                this.pBar1.Value = 1;
                this.pBar1.Step = 1;

                this.label1.Location = new Point(475, 122);
                this.label1.UseMnemonic = true;

                // Set the text of the control and specify a mnemonic character.
                this.label1.Size = new Size(100, 50);
                this.label1.Text = "*prices aggregated from top ebay results";

                this.Controls.Add(this.label1);
                this.Controls.Add(this.pBar1);
                this.Controls.Add(this.button1);
                this.Controls.Add(this.button2);
                this.button1.Click += new EventHandler(this.Button1_Click);
                this.button2.Click += new EventHandler(this.Button2_Click);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;
            this.button2.Text = "export to .txt";
            this.total = 0;
            this.pBar1.Value = 1;
            this.button1.Text = "Fetching Prices...";
            ListViewItem item1 = new ListViewItem("item1", 0);
            this.myPartsPrices1 = this.GetPrices(this.myParts1);
            string val3 = string.Format("${0:N2}", this.total);
            ListViewSubItem item3 = new ListViewSubItem();
            item3.Text = val3;
            item3.Font = new Font(this.listView1.Font, FontStyle.Bold);
            this.listView1.Items[this.myParts1.Count() + 1].SubItems.Add(item3);
            Cursor.Current = Cursors.Default;
            this.button1.Text = "Get Prices";
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            this.button2.Text = "exporting to .txt";
            this.pBar1.Value = 1;
            Cursor.Current = Cursors.WaitCursor;
            string cd = Directory.GetCurrentDirectory();
            string localDate = DateTime.Now.ToString("MM-dd-yyyy");
            int count = this.listView1.Items.Count;
            using (TextWriter tw = new StreamWriter(cd + @"\\PPP_results_" + localDate + ".txt"))
            {
                foreach (ListViewItem item in this.listView1.Items)
                {
                    if (this.myPartsPrices1.ContainsKey(item.Text))
                    {
                        if (this.myPartsPrices1[item.Text] == -69)
                        {
                            tw.WriteLine(item.Text);
                        }
                        else
                        {
                            tw.WriteLine(item.Text + "," + this.myPartsPrices1[item.Text]);
                        }
                    }
                    else if (item.Text.Equals(string.Empty))
                    {
                        tw.WriteLine(string.Empty);
                    }
                    else if (count <= 1)
                    {
                        tw.WriteLine(item.Text + "," + this.total);
                    }
                    else
                    {
                        tw.WriteLine(item.Text);
                    }

                    count--;
                }
            }

            this.pBar1.Value = this.myParts1.Count();
            Cursor.Current = Cursors.Default;
            this.button2.Text = "exported";
        }
    }
}
