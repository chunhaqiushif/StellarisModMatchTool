using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Text.RegularExpressions;
using Microsoft.Win32;
using System.Net;
using System.Threading;
using System.Data;

namespace StellarisModMatch
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    /// 


    public partial class MainWindow : Window
    {
        DataCollect dc = null;

        //主窗口
        public MainWindow()
        {
            InitializeComponent();
            dc = DataCollect.GetInstance();
            this.DataContext = dc;
        }

        //鼠标按下pressstart图片时的操作
        private void UserControl_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            WebBrowser wBrowser = new WebBrowser();
            wBrowser.Source = new Uri(@"steam://rungameid/281990");

            Thread thread = new Thread(new ThreadStart(() =>
            {
                
                lock (this)
                {
                    this.Dispatcher.Invoke(new Action(delegate
                    {
                        DateTime current = DateTime.Now;
                        while (current.AddMilliseconds(5000) > DateTime.Now)
                        {
                            System.Windows.Forms.Application.DoEvents();
                        }                        
                    }));
                    System.Environment.Exit(0);
                }
            }));

            InputText.IsEnabled = false;
            EnableModButton.IsEnabled = false;
            SaveModButton.IsEnabled = false;
            LoadModButton.IsEnabled = false;
            RefleshModListButton.IsEnabled = false;
            ModDataView.IsEnabled = false;
            Logo.Source = (new BitmapImage(new Uri(@"Resources\logo-click.png", UriKind.RelativeOrAbsolute)));

            thread.SetApartmentState(ApartmentState.MTA);
            thread.IsBackground = true;
            thread.Start();
        }

        //获取并选中DependencyObject中的CheckBox 树递归
        public void GetVisualChild(DependencyObject parent)
        {
            int numVisuals = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < numVisuals; i++)
            {
                DependencyObject v = (DependencyObject)VisualTreeHelper.GetChild(parent, i);
                CheckBox child = v as CheckBox;

                if (child == null)
                {
                    GetVisualChild(v);
                }
                else
                {
                    child.IsChecked = true;
                    return;
                }
            }
        }


        //全选/全不选checkbox的事件
        private void SelectedAll_Checked(object sender, RoutedEventArgs e)
        {
            CheckBox chk1 = (CheckBox)sender;
            List<ModData> intersectedList = new List<ModData>();
            //List<ModData> m_moddata = ModDataView.ItemsSource as List<ModData>;
            if (findResult.Count == 0)
            {
                intersectedList = dc.ModDataList.ToList();
            }
            else
            {
                intersectedList = dc.ModDataList.ToList().Intersect<ModData>(findResult).ToList();
            }

            intersectedList.ForEach(p => p.IsSelected = chk1.IsChecked.Value);
            LoadOrRefleshModList();
        }

        //单选某个checkbox的事件
        private void SelectedItem_Checked(object sender, RoutedEventArgs e)
        {
            var temp = this.ModDataView.SelectedItem;
            var SelectRow = temp as ModData;
            for (int i = 0; i < dc.ModDataList.Count; i++)
            {
                ModData m_modData = dc.ModDataList[i];
                if (m_modData.Id == SelectRow.Id)
                {
                    dc.ModDataList[i].IsSelected = SelectRow.IsSelected;
                    return;
                }
            }
        }

        //bool addDataGridThreadIsRun = true;
        public delegate void AddUnDownloadModDelegate();
        //加载或刷新DataGrid操作
        private void LoadOrRefleshModList()
        {
            ModDataView.Items.Refresh();
        }

        private void AddUnDownloadMod()
        {
            ICollectionView view = CollectionViewSource.GetDefaultView(dc.ModDataList);
            SortDescription sd = new SortDescription("Name", ListSortDirection.Ascending);
            view.SortDescriptions.Add(sd);
            lock (this)
            {
                this.Dispatcher.Invoke(new Action(delegate
                {
                    ModDataView.ItemsSource = null;
                    ModDataView.ItemsSource = view;
                    view.SortDescriptions.Clear();
                }));

                return;
            }
        }

        //首次 加载mod数据 显示在DataGrid上
        private void LoadModData(object sender, RoutedEventArgs e)
        {
            try
            {
                modEnableList = GetModEnableTextList(GetTheDirAndFilePath(PathType.e_file));

                System.IO.DirectoryInfo dirInfo = new System.IO.DirectoryInfo(GetTheDirAndFilePath(PathType.e_dir));

                foreach (ModData m_data in GetAllFilesInfo(dirInfo))
                {
                    dc.ModDataList.Add(m_data);
                }

                //int textCount = dc.ModDataList.Count;

                //IEnumerable<ModData> tempSum = ModDataList;
                ICollectionView view = CollectionViewSource.GetDefaultView(dc.ModDataList);

                ModDataView.ItemsSource = null;
                SortDescription sd = new SortDescription("Name", ListSortDirection.Ascending);
                view.SortDescriptions.Add(sd);
                ModDataView.ItemsSource = view;
                view.SortDescriptions.Clear();

                //SaveTheEableCheckboxModListFileTo(@"default.mlt");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n" + ex.StackTrace + ex.StackTrace, "警告", MessageBoxButton.OK);
                throw;
            }

        }


        //相关文件路径、后缀
        static string modInfoFileFolderPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal) + @"\Paradox Interactive\Stellaris\mod";
        static string modEnableFilePath = Environment.GetFolderPath(Environment.SpecialFolder.Personal) + @"\Paradox Interactive\Stellaris\settings.txt";
        static string pattern = "*.mod";

        //获取目录
        enum PathType
        {
            e_file,
            e_dir
        }
        private string GetTheDirAndFilePath(PathType pathType)
        {
            string m_modinfofilefolderpath = modInfoFileFolderPath;
            string m_modenablefilepath = modEnableFilePath;

            if (!Directory.Exists(m_modinfofilefolderpath))
            {

                //folderBrowserDialog.RootFolder = Environment.SpecialFolder.Personal;
                do
                {
                    System.Windows.Forms.FolderBrowserDialog folderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
                    folderBrowserDialog.Description = "选择群星相关文档所在的文件夹：\n一般情况下该文件夹位于\n...\\我的文档\\Paradox Interactive\\Stellaris";
                    folderBrowserDialog.ShowNewFolderButton = false;
                    folderBrowserDialog.ShowDialog();

                    if (folderBrowserDialog.SelectedPath != string.Empty)
                    {
                        m_modinfofilefolderpath = folderBrowserDialog.SelectedPath + @"\mod";
                        m_modenablefilepath = folderBrowserDialog.SelectedPath + @"\settings.txt";
                        string ErrMsg = "";
                        if (Directory.Exists(m_modinfofilefolderpath))
                        {
                            modInfoFileFolderPath = m_modinfofilefolderpath;
                        }
                        else
                        {
                            ErrMsg += "选择的目录下不存在mod文件夹";
                        }
                        if (File.Exists(m_modenablefilepath))
                        {
                            ErrMsg += "。";
                            modEnableFilePath = m_modenablefilepath;
                            break;
                        }
                        else
                        {
                            ErrMsg += "，且该目录下的setting文件不存在。";
                        }
                        MessageBox.Show(ErrMsg, "提示：群星文档目录选择有误", MessageBoxButton.OK);
                    }
                    else
                    {
                        Environment.Exit(0);
                    }
                } while (true);
                //if (folderBrowserDialog.SelectedPath != string.Empty&& Directory.Exists(modInfoFileFolderPath) && File.Exists(modEnableFilePath + @"\settings.txt"))
                //{
                //    modInfoFileFolderPath = folderBrowserDialog.SelectedPath + @"\mod";
                //    modEnableFilePath = folderBrowserDialog.SelectedPath + @"\settings.txt";
                //}
                //else
                //{
                //    do
                //    {
                //        folderBrowserDialog.ShowDialog();
                //        if (!File.Exists(modEnableFilePath))
                //        {
                //            MessageBox.Show("找不到相应文件", "提示", MessageBoxButton.OK);
                //            Environment.Exit(0);
                //        }
                //    } while (folderBrowserDialog.SelectedPath == string.Empty);
                //重复要求输入文件夹目录信息

            }
            return (pathType == PathType.e_file) ? m_modenablefilepath : m_modinfofilefolderpath;
        }

        //当前启用mod列表
        static List<string> modEnableList = new List<string>();
        private List<string> GetModEnableTextList(string filePath)
        {
            List<string> idList = new List<string>();
            string strData = "";
            try
            {
                string line;
                // 创建一个 StreamReader 的实例来读取文件 ,using 语句也能关闭 StreamReader
                using (System.IO.StreamReader sr = new System.IO.StreamReader(filePath))
                {
                    // 从文件读取并显示行，直到文件的末尾 
                    while ((line = sr.ReadLine()) != null)
                    {
                        //Console.WriteLine(line);
                        strData += line;
                        //strData += "\n";
                    }
                    //strData = sr.ReadToEnd();
                }
                //strData = File.ReadAllText(filePath);
            }
            catch (Exception ex)
            {
                // 向用户显示出错消息
                MessageBox.Show(ex.Message + "\n" + ex.StackTrace, "警告", MessageBoxButton.OK);
                //Console.WriteLine("The file could not be read:");
                //Console.WriteLine(e.Message);
            }
            try
            {
                string matcherId = @"""(\d+?)""";
                string matcherReplace1 = @"mod/ugc_";
                string matcherReplace2 = @".mod";
                //string test = Regex.Match(strData, matcherId1).Groups[1].Value;
                strData = Regex.Replace(strData, matcherReplace1, "");
                strData = Regex.Replace(strData, matcherReplace2, "");
                MatchCollection reg = Regex.Matches(strData, matcherId);

                foreach (Match id in reg)
                {
                    //string test = id.Groups[1].Value;
                    idList.Add(id.Groups[1].Value);

                    //idList.Add(Str2Int(id.Groups[1].Value));
                }

                return idList;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n" + ex.StackTrace, "警告", MessageBoxButton.OK);
                throw;
            }

        }

        //mod信息文件内容 分析
        private static ModData FileTextExtract(string strData)
        {
            try
            {
                //处理字符串
                ModData moddata = new ModData();
                string matcherName = @"name=""(.+?)""a";
                string matcherTag = @"tags={(.+?)}";
                string matcherId = @"remote_file_id=""(\d+)""";
                string matcherVersion = @"supported_version=""(.+)""";

                string name = Regex.Match(strData, matcherName).Groups[1].Value;
                string tag = Regex.Match(strData, matcherTag).Groups[1].Value;
                //tag = Regex.Replace(tag, "\t", "");
                MatchCollection reg = Regex.Matches(tag, @"""(.+?)""");
                tag = "";
                int index = 0;
                foreach (Match matchStr in reg)
                {
                    if (0 != index)
                    {
                        tag += @"/";
                    }
                    string str = matchStr.ToString();
                    str = Regex.Replace(str, @"""", "");
                    tag += str;
                    index++;
                }
                string idStr = Regex.Match(strData, matcherId).Groups[1].Value;
                //int id = Str2Int(idStr);
                string version = Regex.Match(strData, matcherVersion).Groups[1].Value;

                moddata.Name = name;
                moddata.Type = tag;
                moddata.Id = idStr;
                moddata.Version = version;

                bool isExist = true;
                bool isEnable = modEnableList.Exists(p => p == idStr);//modEnableList.Find(idStr);

                moddata.IsExist = isExist;
                moddata.IsEnable = isEnable;
                moddata.IsSelected = isEnable;
                moddata.LinkName = "链接";
                moddata.Http = @"https://steamcommunity.com/sharedfiles/filedetails/?id=" + idStr;
                return moddata;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n" + ex.StackTrace, "警告", MessageBoxButton.OK);
                throw;
            }

        }

        //字符串转int32数字
        //public static int Str2Int(string str)
        //{
        //    //int num1, num2, num3;
        //    //try
        //    //{
        //    //    num1 = int.Parse(str);
        //    //}
        //    //catch (Exception)
        //    //{
        //    //    num1 = int.
        //    //    throw;
        //    //}

        //    //int.TryParse(str, out num2);
        //    //num3 = Convert.ToInt32(str);
        //    //return num3;
        //    long num = Convert.ToInt64();
        //}

        //获得全部Mod文件信息
        private List<ModData> GetAllFilesInfo(DirectoryInfo directory)
        {
            //int totalFile = 0;
            List<ModData> modDatas = new List<ModData>();
            foreach (FileInfo info in directory.GetFiles(pattern))
            {
                string strData = "";
                try
                {
                    string line;
                    // 创建一个 StreamReader 的实例来读取文件 ,using 语句也能关闭 StreamReader
                    using (System.IO.StreamReader sr = new System.IO.StreamReader(info.FullName.ToString()))
                    {
                        // 从文件读取并显示行，直到文件的末尾 
                        while ((line = sr.ReadLine()) != null)
                        {
                            //Console.WriteLine(line);
                            strData += line;
                            //strData += "\n";
                        }
                    }
                }
                catch (Exception ex)
                {
                    // 向用户显示出错消息
                    MessageBox.Show(ex.Message + "\n" + ex.StackTrace, "警告", MessageBoxButton.OK);
                    throw;
                    //Console.WriteLine("The file could not be read:");
                    //Console.WriteLine(e.Message);
                }
                modDatas.Add(FileTextExtract(strData));
            }
            return modDatas;
        }

        //MOD订阅地址 超链接点击事件
        void CheckTheLink(object sender, RoutedEventArgs e)
        {
            Hyperlink link = (Hyperlink)e.OriginalSource;
            System.Diagnostics.Process.Start(link.NavigateUri.AbsoluteUri);
        }

        //应用mod按钮 点击事件
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            DelectedLastMods();
            CheckBoxSelectedWrite2File(modEnableFilePath, false);
            MessageBoxButton button = MessageBoxButton.OK;
            string caption = "群星Stellaris Mod匹配工具";
            MessageBox.Show("当前选中Mod已启用", caption, button);
        }

        //删除settings.txt内的last_mods={}
        private void DelectedLastMods()
        {
            string text = "";
            using (StreamReader sr = new StreamReader(modEnableFilePath))
            {
                //sr.BaseStream.Seek(0, SeekOrigin.End);
                //text = sr.ToString();
                string line;
                // 从文件读取并显示行，直到文件的末尾 
                while ((line = sr.ReadLine()) != null)
                {
                    //Console.WriteLine(line);
                    text += line;
                    text += "\n";
                }
            }
            text = Regex.Replace(text, @"last_mods={\n(\t(.+)\n)+}", "");
            using (FileStream fs = new FileStream(modEnableFilePath, FileMode.Create, FileAccess.Write))
            {
                using (StreamWriter sw = new StreamWriter(fs))
                {
                    string[] sArray = Regex.Split(text, "\n", RegexOptions.IgnoreCase);
                    for (int i = 0; i < sArray.Length; i++)
                    {
                        sw.WriteLine(sArray[i]);
                    }
                    sw.Flush();
                }
            }
            LoadOrRefleshModList();
        }

        //保存当前启用的Mod按钮 点击事件
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            SaveTheEableCheckboxModListFileTo();
        }

        private void SaveTheEableCheckboxModListFileTo(string path = "")
        {
            if (path == string.Empty)
            {
                //创建一个保存文件式的对话框
                SaveFileDialog sfd = new SaveFileDialog
                {
                    //设置这个对话框的起始保存路径
                    InitialDirectory = @"modList\",
                    //设置保存的文件的类型，注意过滤器的语法
                    Filter = "mlt|*.mlt"
                };
                if (sfd.ShowDialog() == true)
                {
                    CheckBoxSelectedWrite2File(sfd.FileName, true);
                    MessageBoxButton button = MessageBoxButton.OK;
                    string caption = "群星Stellaris Mod匹配工具";
                    MessageBox.Show("保存成功", caption, button);
                }
                else
                {
                    //MessageBox.Show("取消保存");
                }
            }
            else
            {
                CheckBoxSelectedWrite2File(path, true);
            }

            //调用ShowDialog()方法显示该对话框，该方法的返回值代表用户是否点击了确定按钮

        }

        //checkbox选择行写入相应路径文件
        private void CheckBoxSelectedWrite2File(string path, bool isOnlyCreate)
        {
            string outputStr = @"last_mods={";
            string outputStr1 = @"""mod/ugc_";
            string outputStr2 = @".mod""";

            using (FileStream fs = new FileStream(path, isOnlyCreate ? FileMode.Create : FileMode.OpenOrCreate, FileAccess.ReadWrite))
            {
                using (StreamWriter sw = new StreamWriter(fs))
                {
                    sw.WriteLine(outputStr);
                    int index = 0;
                    foreach (var m_modData in dc.ModDataList)
                    {
                        if (m_modData.IsSelected)
                        {
                            dc.ModDataList[index].IsEnable = true;
                            sw.WriteLine("\t" + outputStr1 + m_modData.Id.ToString() + outputStr2);
                        }
                        else
                        {
                            dc.ModDataList[index].IsEnable = false;
                        }
                        index++;
                    }
                    sw.BaseStream.Seek(0, SeekOrigin.End);
                    sw.WriteLine("}");
                    sw.Flush();
                }
            }
        }

        //加载mod列表 点击事件
        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            List<string> newModListFromOpenFile = new List<string>();
            //创建一个打开文件式的对话框
            OpenFileDialog ofd = new OpenFileDialog
            {
                //设置这个对话框的起始打开路径
                InitialDirectory = @"modList\",
                //设置打开的文件的类型，注意过滤器的语法
                Filter = "mlt|*.mlt"
            };
            //调用ShowDialog()方法显示该对话框，该方法的返回值代表用户是否点击了确定按钮
            if (ofd.ShowDialog() == true)
            {
                newModListFromOpenFile = GetModEnableTextList(ofd.FileName);
            }
            else
            {
                //MessageBox.Show("没有选择图片");
            }

            int index = 0;
            foreach (var m_modData in dc.ModDataList)
            {
                bool flag = false;
                for (int i = 0; i < newModListFromOpenFile.Count; i++)
                {
                    if (m_modData.Id == newModListFromOpenFile[i])
                    {
                        dc.ModDataList[i].IsSelected = true;
                        flag = true;
                        newModListFromOpenFile.RemoveAt(i);
                        break;
                    }
                }
                if (flag)
                {
                    flag = false;
                    continue;
                }
                dc.ModDataList[index].IsSelected = false;
                index++;
            }

            if (newModListFromOpenFile.Count != 0)
            {
                AddUnDownloadModDelegate deleAdd = new AddUnDownloadModDelegate(AddUnDownloadMod);
                foreach (var idStr in newModListFromOpenFile)
                {
                    ModData temp = new ModData
                    {
                        //temp.Id = Str2Int(idStr);
                        Id = idStr,
                        Name = "-- 查找中 --",
                        Type = "[New]",
                        Version = "--",
                        IsExist = false,
                        IsEnable = false,
                        IsSelected = true,
                        LinkName = "链接",
                        Http = @"https://steamcommunity.com/sharedfiles/filedetails/?id=" + idStr
                    };

                    modEnableList.Add(idStr);
                    dc.ModDataList.Add(temp);

                    Thread thread = new Thread(new ThreadStart(() =>
                    {
                        temp.Name = WebCaptureReturnModName(temp.Http);
                        deleAdd();
                        System.Windows.Threading.Dispatcher.Run();
                    }));

                    thread.SetApartmentState(ApartmentState.MTA);
                    thread.IsBackground = true;
                    thread.Start();
                }
            }
            //加一个刷新操作
            LoadOrRefleshModList();
        }

        //刷新列表 按钮事件
        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            var temp = dc.ModDataList.ToList();
            foreach (var item in temp)
            {
                if (item.IsEnable != item.IsSelected)
                {
                    item.IsSelected = item.IsEnable;
                }
            }
            //ICollectionView view = CollectionViewSource.GetDefaultView(dc.ModDataList);

            //ModDataView.ItemsSource = null;
            //SortDescription sd = new SortDescription("Name", ListSortDirection.Ascending);
            //view.SortDescriptions.Add(sd);
            //ModDataView.ItemsSource = view;
            //view.SortDescriptions.Clear();

            LoadOrRefleshModList();
        }

        //抓取网页mod名
        private string WebCaptureReturnModName(string url)
        {
            try
            {
                string name;
                Uri httpURL = new Uri(url);

                ///HttpWebRequest类继承于WebRequest，并没有自己的构造函数，需通过WebRequest的Creat方法 建立，并进行强制的类型转换   
                HttpWebRequest httpReq = (HttpWebRequest)WebRequest.Create(httpURL);
                //httpReq.Headers.Add("cityen", "tj");


                ///通过HttpWebRequest的GetResponse()方法建立HttpWebResponse,强制类型转换   
                HttpWebResponse httpResp = (HttpWebResponse)httpReq.GetResponse();

                ///GetResponseStream()方法获取HTTP响应的数据流,并尝试取得URL中所指定的网页内容   
                ///若成功取得网页的内容，则以System.IO.Stream形式返回，若失败则产生ProtoclViolationException错 误。
                System.IO.Stream respStream = httpResp.GetResponseStream();

                ///返回的内容是Stream形式的，所以可以利用StreamReader类获取GetResponseStream的内容
                System.IO.StreamReader respStreamReader = new System.IO.StreamReader(respStream, Encoding.UTF8);
                //从流的当前位置读取到结尾
                string strBuff = respStreamReader.ReadToEnd();

                respStreamReader.Close();
                respStream.Close();

                name = Regex.Match(strBuff, @"<title>(.+)</title>").Groups[1].Value;
                name = Regex.Replace(name, @"Steam Workshop :: ", "");
                name = Regex.Replace(name, @"Steam Community :: Error", "-- ModID有误 --");

                return name;
            }

            catch (WebException webEx)
            {

                Console.WriteLine(webEx.Message.ToString());
                string ErrMsg = "-- 读取Mod信息失败，也许需要科学上网 --";
                return ErrMsg;
                //throw;
            }
        }

        List<ModData> findResult = new List<ModData>();

        //文字改变后搜索
        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string input = InputText.Text;
            findResult.Clear();
            ICollectionView view;
            if (input == "")
            {
                view = CollectionViewSource.GetDefaultView(dc.ModDataList);
            }
            else
            {
                input = Regex.Replace(input, @"(\*|\.|\?|\$|\^|\[|\]|\(|\)|\{|\}|\||\\|\/)", @"\$1");
                view = CollectionViewSource.GetDefaultView(findResult);
            }
            for (int i = 0; i < dc.ModDataList.Count; i++)
            {
                ModData m_modData = dc.ModDataList[i];
                MatchCollection match = Regex.Matches(m_modData.Name, input);

                if (match.Count > 0)
                {
                    findResult.Add(m_modData);
                }
            }

            lock (this)
            {
                this.Dispatcher.Invoke(new Action(delegate
                {
                    SortDescription sd = new SortDescription("Name", ListSortDirection.Ascending);
                    view.SortDescriptions.Add(sd);
                    ModDataView.ItemsSource = null;
                    ModDataView.ItemsSource = view;
                    view.SortDescriptions.Clear();
                }));

                return;
            }

            //数据压缩操作
            //private string CodeCompression()
            //数据解压操作
        }

        public class DataCollect : INotifyPropertyChanged
        {
            private static DataCollect _dataCollect = null;
            public static DataCollect GetInstance()
            {
                if (_dataCollect == null)
                    _dataCollect = new DataCollect();

                return _dataCollect;
            }


            private ObservableCollection<ModData> _modDataList = new ObservableCollection<ModData>();
            public ObservableCollection<ModData> ModDataList
            {
                get { return _modDataList; }
                set
                {
                    _modDataList = value;
                    OnChangedProperty("ModDataList");
                }
            }

            public event PropertyChangedEventHandler PropertyChanged;
            public void OnChangedProperty(string name)
            {
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs(name));
                }
            }
        }

        public class ModData : INotifyPropertyChanged
        {
            public bool IsSelected { get; set; }
            public string Id { get; set; }
            public string Name { get; set; }
            public string Type { get; set; }
            public string Version { get; set; }
            public bool IsExist { get; set; }
            public string _IsExist
            {
                get
                {
                    return IsExist ? "存在" : "不存在";
                }
                set { _IsExist = value; }
            }
            public bool IsEnable { get; set; }
            public string _IsEnable
            {
                get
                {
                    return IsEnable ? "启用" : "未启用";
                }
                set { _IsEnable = value; }
            }
            public string LinkName { get; set; }
            public string Http { get; set; }

            public event PropertyChangedEventHandler PropertyChanged;
            public void OnChangedProperty(string name)
            {
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs(name));
                }
            }
        }
    }
}
