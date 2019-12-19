using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GetMessageToMeituan
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        public static string result_cookie = "";
        public static int ID = 0;
        private void Button1_Click(object sender, EventArgs e)
        {
            string t1 = dateTimePicker1.Value.ToString("yyyy-MM-dd")+" 00:00";
            string t2 = dateTimePicker2.Value.ToString("yyyy-MM-dd") + " 23:59";
            DateTime d1 = DateTime.Parse(t1);
            DateTime d2 = DateTime.Parse(t2);
            list.Clear();
            ID = 0;
            dataGridView1.DataSource = null;
            if (string.IsNullOrEmpty(txt_url.Text))
            {
                MessageBox.Show("请输入地址！");
            }
            else
            {
                int page = Convert.ToInt32(txt_page.Text);
                ParallelLoopResult result = Parallel.For(0, page, i =>
                {
                    RunAsync(txt_url.Text, i,d1, d2).Wait();

                    Thread.Sleep(10);
                });
                MessageBox.Show("读取完成！");
                list = list.OrderBy(m => m.Id).ToList();
                Thread.Sleep(1);
                dataGridView1.DataSource = list;
            }
        }
        public static List<Comment> list = new List<Comment>();
        static async Task RunAsync(string url, int page, DateTime state, DateTime end)
        {
            string uuid = await getcookie(url);
            string[] arr = url.Split('/');
            string id = arr[arr.Length - 2];

            int pagesize = 10 * page;
            HttpClient httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("Host", "www.meituan.com");
            httpClient.DefaultRequestHeaders.Add("Method", "Get");
            httpClient.DefaultRequestHeaders.Add("KeepAlive", "false");   // HTTP KeepAlive设为false，防止HTTP连接保持
            httpClient.DefaultRequestHeaders.Add("UserAgent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/74.0.3729.108 Safari/537.36");
            httpClient.DefaultRequestHeaders.Add("Referer", url);

            string geturl = string.Format("https://www.meituan.com/meishi/api/poi/getMerchantComment?" +
                "uuid={0}&" +
                "platform=1&" +
                "partner=126&" +
                "originUrl={1}&" +
                "riskLevel=1&" +
                "optimusCode=10&" +
                "id={2}&" +
                "userId=&" +
                "offset={3}&" +
                "pageSize=10&" +
                "sortType=0"
                , uuid, url, id, pagesize);


            var tokenResponse = httpClient.GetAsync(geturl);
            tokenResponse.Wait();
            tokenResponse.Result.EnsureSuccessStatusCode();
            string tokenRes = await tokenResponse.Result.Content.ReadAsStringAsync();
            JObject jo = (JObject)JsonConvert.DeserializeObject(tokenRes);
            int count = jo["data"]["comments"].Count();

            for (int i = 0; i < count; i++)
            {
                DateTime t1 = ConvertStringToDateTime(jo["data"]["comments"][i]["commentTime"].ToString());
                if (DateTime.Compare(state, t1) < 0 && DateTime.Compare(end, t1) > 0)
                {
                    ID++;
                    list.Add(new Comment(ID, jo["data"]["comments"][i]["userName"].ToString(), jo["data"]["comments"][i]["comment"].ToString(), t1));
                }
                else if (DateTime.Compare(state, t1) == 0 || DateTime.Compare(end, t1) == 0)
                {
                    ID++;
                    list.Add(new Comment(ID, jo["data"]["comments"][i]["userName"].ToString(), jo["data"]["comments"][i]["comment"].ToString(), t1));
                }
                else
                {

                }
            }


        }

        public static async Task<String> getcookie(string url)
        {
            try
            {
                HttpClientHandler handler = new HttpClientHandler();
                handler.UseCookies = true;
                var uri = new Uri(" https://www.meituan.com");

                HttpClient client = new HttpClient(handler);

                var result = client.GetStringAsync(uri);

                Console.WriteLine(result.Result);
                var getCookies = handler.CookieContainer.GetCookies(uri);
                string uuid = string.Empty;
                for (int i = 0; i < getCookies.Count; i++)
                {
                    if (getCookies[i].Name == "uuid")
                    {
                        return getCookies[i].Value;
                    }
                }
            }
            catch
            {
                return String.Empty;
            }
            return result_cookie;
        }

        /// <summary>        
        /// 时间戳转为C#格式时间        
        /// </summary>        
        /// <param name=”timeStamp”></param>        
        /// <returns></returns>        
        private static DateTime ConvertStringToDateTime(string timeStamp)
        {
            DateTime dtStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
            long lTime = long.Parse(timeStamp + "0000");
            TimeSpan toNow = new TimeSpan(lTime);
            return dtStart.Add(toNow);
        }

        public static string path = System.AppDomain.CurrentDomain.BaseDirectory;
        [DllImport("haha.dll")]
        static extern void QQRichEdit(string str, int i);
        [DllImport("haha.dll")]
        static extern int load();
        private HslCommunication.BasicFramework.SoftAuthorize softAuthorize = null;
        private string AuthorizeEncrypted(string arg)
        {
            return HslCommunication.BasicFramework.SoftSecurity.MD5Encrypt(arg, "4rfv%TGB");
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            softAuthorize = new HslCommunication.BasicFramework.SoftAuthorize();
            softAuthorize.FileSavePath = path + "\\Authorize.data"; // 设置存储激活码的文件，该存储是加密的
            softAuthorize.LoadByFile();

            // 检测激活码是否正确，没有文件，或激活码错误都算作激活失败
            if (!softAuthorize.IsAuthorizeSuccess(AuthorizeEncrypted))
            {
                // 显示注册窗口
                using (HslCommunication.BasicFramework.FormAuthorize form =
                    new HslCommunication.BasicFramework.FormAuthorize(
                        softAuthorize,
                        "请加QQ1748434806",
                        AuthorizeEncrypted))
                {
                    if (form.ShowDialog() != DialogResult.OK)
                    {
                        // 授权失败，退出
                        Close();
                    }
                }
            }
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            SaveFileDialog ofd = new SaveFileDialog();
            ofd.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory); //设置初始路径
            ofd.Filter = "Excel文件(*.xls)|*.xls|Csv文件(*.csv)|*.csv|所有文件(*.*)|*.*"; //设置“另存为文件类型”或“文件类型”框中出现的选择内容
            ofd.FilterIndex = 2; //设置默认显示文件类型为Csv文件(*.csv)|*.csv
            ofd.Title = "打开文件"; //获取或设置文件对话框标题
            ofd.RestoreDirectory = true;////设置对话框是否记忆上次打开的目录
            if (ofd.ShowDialog()== DialogResult.OK)
            {
                if (!string.IsNullOrEmpty(ofd.FileName))
                {
                    // DataExport(list, ofd.FileName);
                    ExportToExcel(list, "查询信息", "查询信息表", ofd.FileName);
                    MessageBox.Show("导出成功！");
                }
            }
            }

        /// <summary>
        /// 导出数据到Excel
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data">数据集合</param>
        /// <param name="head">表头(第一行数据)</param>
        /// <param name="sheetName">sheet名称</param>
        static void ExportToExcel<T>(List<T> data, string head, string sheetName,string path)
        {
            IWorkbook wb = new HSSFWorkbook();
            //设置工作簿的名称
            sheetName = string.IsNullOrEmpty(sheetName) ? "sheet1" : sheetName;
            //创建一个工作簿
            ISheet sh = wb.CreateSheet(sheetName);

            //全局索引
            int gloal_index = 0;
            System.Reflection.PropertyInfo[] oProps = null;
            foreach (T en in data)
            {
                if (oProps == null)
                {
                    oProps = ((Type)en.GetType()).GetProperties();
                }
                if (gloal_index == 0)
                {
                    #region 表头(第1行)
                    //合并单元格
                    sh.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(0, 0, 0, oProps.Length - 1));
                    //创建第1行
                    IRow row0 = sh.CreateRow(0);
                    //设置第1行高度
                    row0.Height = 20 * 20;
                    //创建第1行第1列
                    ICell icell1top0 = row0.CreateCell(0);
                    //设置第1行第1列格式
                    icell1top0.CellStyle = Getcellstyle(wb, "head");
                    //设置第1行第1列内容
                    icell1top0.SetCellValue(head);
                    #endregion

                    #region 抬头(第2行)
                    //创建第2行
                    IRow row1 = sh.CreateRow(1);
                    //设置高度
                    row1.Height = 20 * 20;
                    //columnt_index是列的索引
                    int columnt_index = 0;
                    foreach (System.Reflection.PropertyInfo item in oProps)
                    {
                        //获取T的字段名称
                        string name = item.Name;
                        //获取T的字段名称的描述
                        string des = ((DescriptionAttribute)Attribute.GetCustomAttribute(item, typeof(DescriptionAttribute))).Description;

                        //创建第2行的第columnt_index列
                        ICell icell1top = row1.CreateCell(columnt_index);
                        //设置第2行的第columnt_index列的格式
                        icell1top.CellStyle = Getcellstyle(wb, "");
                        //设置第2行的第columnt_index列的内容
                        if (!string.IsNullOrEmpty(des))
                        {
                            icell1top.SetCellValue(des);
                        }
                        else
                        {
                            icell1top.SetCellValue(name);
                        }
                        //设置第2行的第columnt_index列的宽度
                        sh.SetColumnWidth(columnt_index, (int)((15 + 0.72) * 256));
                        columnt_index++;
                    }
                    #endregion

                    gloal_index = 2;
                }

                #region 这里是List<T>具体内容
                //创建第gloal_index行
                IRow row_zs = sh.CreateRow(gloal_index);
                int column_index = 0;
                foreach (System.Reflection.PropertyInfo pi in oProps)
                {
                    //创建第gloal_index行的第columnt_index列
                    ICell icell1top = row_zs.CreateCell(column_index);
                    //设置第gloal_index行的第columnt_index列格式
                    icell1top.CellStyle = Getcellstyle(wb, "");
                    //获取en字段值
                    string v_value = pi.GetValue(en, null) == null ? "" : pi.GetValue(en, null).ToString();
                    //设置第gloal_index行的第columnt_index列的内容
                    icell1top.SetCellValue(v_value);

                    column_index++;
                }
                #endregion

                gloal_index++;
            }

            //输出内容
            using (FileStream stm = File.OpenWrite(path))
            {
                wb.Write(stm);
            }
        }
        /// <summary>
        /// 格式设置
        /// </summary>
        static ICellStyle Getcellstyle(IWorkbook wb, string type)
        {
            ICellStyle cellStyle = wb.CreateCellStyle();
            //定义字体  
            IFont font = wb.CreateFont();
            font.FontName = "微软雅黑";
            //水平对齐  
            cellStyle.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Left;
            //垂直对齐  
            cellStyle.VerticalAlignment = VerticalAlignment.Center;
            //自动换行  
            cellStyle.WrapText = true;
            //缩进
            cellStyle.Indention = 0;

            switch (type)
            {
                case "head":
                    cellStyle.SetFont(font);
                    cellStyle.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Center;
                    break;
                default:
                    cellStyle.SetFont(font);
                    break;
            }
            return cellStyle;
        }

        private static DateTime _dtStart = new DateTime(1970, 1, 1, 8, 0, 0);

        /// <summary> 
        /// 获取时间戳 
        /// </summary>  
        public static string GetTimeStamp()
        {
            DateTime dateTime = DateTime.Now;
            return Convert.ToInt64(dateTime.Subtract(_dtStart).TotalMilliseconds).ToString();
        }
    }
    }
