using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System.IO;
using System.Text.RegularExpressions;

namespace ebay_exporter
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            btnParse.Enabled = false;
            btnExport.Enabled = false;
        }

        ChromeDriver driver;
        List<eBayItem> items = new List<eBayItem>();

        private void btnLaunch_Click(object sender, EventArgs e)
        {
            try
            {
                driver = new ChromeDriver();
                driver.Navigate().GoToUrl("http://my.ebay.co.uk/ws/eBayISAPI.dll?MyEbay&gbh=1");
                btnLaunch.Enabled = false;
                btnParse.Enabled = true;
            }
            catch (Exception ex)
            {
                File.WriteAllText("exception.txt", ex.ToString());
                MessageBox.Show("Could not load Chrome WebDriver: do you have Chrome installed?", "Could not load Chrome", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnParse_Click(object sender, EventArgs e)
        {
            var els = driver.FindElementsByCssSelector("div.order-r");

            if (!els.Any())
            {
                MessageBox.Show("No elements with the class 'order-r' were not found.", "Order elements not found", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            foreach (var el in els)
            {
                // id
                var id = "UNKNOWN ID";
                var idEl = el.FindElement(By.CssSelector(".display-item-id"));
                if (idEl != null)
                {
                    id = Regex.Replace(idEl.Text, "[^0-9]", string.Empty);
                }

                // name
                var name = "UNKNOWN NAME";
                var nameEl = el.FindElement(By.CssSelector(".item-title"));
                if (nameEl != null)
                {
                    name = nameEl.Text;
                }

                // date
                var purchaseDate = DateTime.MinValue;
                var dateEl = el.FindElement(By.CssSelector(".row-date"));
                if (dateEl != null)
                {
                    string dateString = dateEl.Text;
                    purchaseDate = DateTime.Parse(dateString);
                }

                // price
                var price = 0M;
                var priceEl = el.FindElement(By.CssSelector(".cost-label"));
                if (priceEl != null)
                {
                    var priceText = priceEl.Text;
                    price = decimal.Parse(Regex.Replace(priceText, "[^.0-9]", string.Empty));
                }
                
                // postage
                var postage = 0M;
                var postageEls = el.FindElements(By.CssSelector(".ship-label div"));
                if (postageEls != null && postageEls.Any())
                {
                    var postageText = Regex.Replace(postageEls.First().Text, "[^.0-9]", string.Empty);
                    if (string.IsNullOrWhiteSpace(postageText))
                    {
                        postage = 0;
                    }
                    else
                    {
                        postage = decimal.Parse(postageText);
                    }
                }

                // seller
                var seller = "UNKNOWN SELLER";
                var sellerEl = el.FindElement(By.CssSelector(".order-item-count a"));
                if (sellerEl != null)
                {
                    seller = sellerEl.Text;
                }

                items.Add(new eBayItem
                {
                    Name = name,
                    PurchaseDate = purchaseDate,
                    Price = price,
                    Postage = postage
                });
            }

            MessageBox.Show(items.Count + " total items parsed", items.Count + " items", MessageBoxButtons.OK, MessageBoxIcon.Information);
            btnExport.Enabled = true;
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            StringBuilder sb = new StringBuilder();
            foreach(var item in items)
            {
                sb.AppendFormat("{0},{1},{2},{3}{4}",
                    item.PurchaseDate,
                    item.Name.Replace(",", ""),
                    item.Price,
                    item.Postage,
                    Environment.NewLine);
            }

            var folder = System.Configuration.ConfigurationSettings.AppSettings["writeTo"];
            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }

            File.WriteAllText(folder + "/export.csv", sb.ToString());
            MessageBox.Show("CSV file exported to 'export.csv'", "CSV exported", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }

    public class eBayItem
    {
        public string Name;
        public DateTime PurchaseDate;
        public decimal Price;
        public decimal Postage;
    }
}
