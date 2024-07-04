using DevExpress.XtraBars.Docking2010;
using DevExpress.XtraBars.Navigation;
using DevExpress.XtraEditors;
using DevExpress.XtraLayout;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using XML130.CustomGridLookUpEdit;
using XML130.EasyUtils;

namespace XML130.XML
{
    public partial class FrmQuanLyXml : XtraForm
    {
        private const string USER_NAME = "26001_BV";
        private const string PASSWORD = "Dkvp26001@";
        private const string MA_TINH = "26";
        private const string MA_CSKCB = "26001";
        private const string LOAI_HO_SO = "130";

        //private string _maCSKCB;
        private DataSet _dsXmlFile;
        private bool _isLogin = false;
        private readonly XmlWriterSettings _xmlWriterSettings = new XmlWriterSettings()
        {
            Indent = true,
            Encoding = new UTF8Encoding(),
            ConformanceLevel = ConformanceLevel.Auto
        };

        public FrmQuanLyXml()
        {
            InitializeComponent();
        }
        #region Protected methods
        protected virtual void OnInit()
        {
            try
            {
                OnClear();
                Login(USER_NAME, PASSWORD);
                cboXmlTypes.Properties.Items.Add("Tất cả");
                cboXmlTypes.Properties.Items.AddRange(XmlHelper.XmlTypes.Keys);
            }
            catch (Exception exception)
            {
                EasyDialog.ShowErrorDialog("Phát sinh lỗi. (" + exception.Message + ")");
            }
        }
        protected virtual void OnClear()
        {
            tcXmls.Pages.Clear();
        }
        protected virtual void OnLoadFromDB()
        {
            try
            {
                EasyLoadWait.ShowWaitForm("Đang tải dữ liệu!", this);
                if (cboXmlTypes.SelectedIndex < 1)
                {
                    _dsXmlFile = XmlHelper.LoadXmlDataFromDb(txtMaLK.Text);
                }
                else
                {
                    _dsXmlFile = new DataSet();
                    DataTable dt = XmlHelper.LoadXmlDataFromDb(cboXmlTypes.Text, txtMaLK.Text);
                    _dsXmlFile.Tables.Add(dt);
                }
            }
            catch (Exception exception)
            {
                EasyDialog.ShowErrorDialog("Phát sinh lỗi. (" + exception.Message + ")");
            }
            finally
            {
                OnCheckXml();
                EasyLoadWait.CloseWaitForm();
            }
        }
        protected virtual void OnOpenXmlFile()
        {
            OpenFileDialog openFile = new OpenFileDialog()
            {
                CheckFileExists = true,
                CheckPathExists = true,
                FileName = "Chọn file xml",
                Title = "Tải dữ liệu XML130",
                Filter = "XML Document (*.xml)|*.xml|All file (*.*)|*.*"
            };
            if (openFile.ShowDialog() == DialogResult.OK)
            {
                EasyLoadWait.ShowWaitForm("Đang import dữ liệu XML130", this);
                try
                {
                    _dsXmlFile = XmlHelper.LoadXmlFile(openFile.FileName);
                    if (_dsXmlFile == null)
                    {
                        EasyDialog.ShowErrorDialog($"Tải dữ liệu file {openFile.FileName} thất bại!");
                    }
                }
                catch (Exception exception)
                {
                    EasyDialog.ShowErrorDialog($"Phát sinh lỗi. ({exception.Message})");
                }
                finally
                {
                    OnCheckXml();
                    EasyLoadWait.CloseWaitForm();
                }
            }
        }
        protected virtual void OnOpenXmlFolder()
        {
            OpenFileDialog openFolder = new OpenFileDialog()
            {
                CheckFileExists = false,
                CheckPathExists = false,
                FileName = "Chọn thư mục",
                Title = "Chọn thư mục chứa dữ liệu XML130 cần import",
            };
            if (openFolder.ShowDialog() == DialogResult.OK)
            {
                EasyLoadWait.ShowWaitForm("Đang import", this);
                try
                {
                    _dsXmlFile = null;
                    string folderPath = Path.GetDirectoryName(openFolder.FileName);
                    string[] xmlFiles = Directory.GetFiles(folderPath, "*.xml", SearchOption.AllDirectories);
                    foreach (string xmlFile in xmlFiles)
                    {
                        if (_dsXmlFile == null)
                        {
                            _dsXmlFile = XmlHelper.LoadXmlFile(xmlFile);
                        }
                        else
                        {
                            DataSet dsXmlFile = XmlHelper.LoadXmlFile(xmlFile);
                            foreach (DataTable dtXmlFile in dsXmlFile.Tables)
                            {
                                if (_dsXmlFile.Tables.Contains(dtXmlFile.TableName))
                                {
                                    foreach (DataRow dr in dtXmlFile.Rows)
                                    {
                                        _dsXmlFile.Tables[dtXmlFile.TableName].Rows.Add(dr.ItemArray);
                                    }
                                }
                                else
                                {
                                    _dsXmlFile.Tables.Add(dtXmlFile.Clone());
                                }
                            }
                        }
                        //XmlHelper.ValidateXml(ref dsXmlFile);
                        //foreach (DataTable dtXmlType in dsXmlFile.Tables)
                        //{
                        //    if (!XmlHelper.ImportXmlType2Db(dtXmlType.TableName, dtXmlType, WriteLog))
                        //    {
                        //        EasyDialog.ShowErrorDialog($"Lưu dữ liệu {dtXmlType.TableName} thất bại!");
                        //    }
                        //}
                    }
                }
                catch (Exception exception)
                {
                    EasyDialog.ShowErrorDialog($"Phát sinh lỗi. ({exception.Message})");
                }
                finally
                {
                    OnCheckXml();
                    EasyLoadWait.CloseWaitForm();
                    EasyDialog.ShowInfoDialog("Import hoàn tất!");
                }
            }
        }
        protected virtual void OnCheckXml()
        {
            try
            {
                OnClear();
                EasyLoadWait.ShowWaitForm("Đang kiểm tra dữ liệu!", this);
                Dictionary<string, List<ClsXmlError>> dictErrors = XmlHelper.ValidateXml(ref _dsXmlFile);
                IEnumerable<ClsXmlError> errorItems = dictErrors.Values.SelectMany(x => x);
                AddPage(0, "Tổng hợp lỗi", errorItems);
                if (_dsXmlFile.Tables.Contains("XML_GIAMDINHHS"))
                {
                    AddPage(1, "Giám định hồ sơ", _dsXmlFile.Tables["XML_GIAMDINHHS"]);
                }
                for (int i = 1; i < 12; i++)
                {
                    string xmlType = string.Format("XML{0}", i);
                    if (_dsXmlFile.Tables.Contains(xmlType)
                        && _dsXmlFile.Tables[xmlType].Rows.Count > 0)
                    {
                        string captibn = xmlType;
                        if (dictErrors.TryGetValue(xmlType, out List<ClsXmlError> lstErrors) && lstErrors.Count > 0)
                        {
                            captibn += string.Format(" <color=Red>({0} lỗi)</color>", lstErrors.Count);
                        }
                        AddPage(i + 1, captibn, _dsXmlFile.Tables[xmlType]);
                    }
                }
            }
            catch (Exception exception)
            {
                EasyDialog.ShowErrorDialog("Phát sinh lỗi. (" + exception.Message + ")");
            }
            finally
            {
                tcXmls.SelectedPageIndex = 0;
                EasyLoadWait.CloseWaitForm();
            }
        }
        protected virtual void OnSaveToDB()
        {
            if (_dsXmlFile != null && _dsXmlFile.Tables.Count > 0)
            {
                try
                {
                    foreach (DataTable dtXmlType in _dsXmlFile.Tables)
                    {
                        if (dtXmlType != null && dtXmlType.Rows.Count > 0)
                        {
                            string maLK = dtXmlType.Rows[0]["MA_LK"].ToString();
                            EasyLoadWait.ShowWaitForm(string.Format("Đang cập nhật dữ liệu lượt khám {0}", maLK), this);
                            if (!XmlHelper.ImportXmlType2Db(dtXmlType.TableName, dtXmlType, WriteLog))
                            {
                                if (!XmlHelper.UpdateXmlType2Db(dtXmlType.TableName, dtXmlType, WriteLog))
                                {
                                    WriteLog($"Cập nhật dữ liệu lượt khám {maLK} tại bảng {dtXmlType.TableName} thất bại!", false);
                                    return;
                                }
                                WriteLog($"Cập nhật dữ liệu lượt khám {maLK} tại bảng {dtXmlType.TableName} thành công!", true);
                            }
                        }
                    }
                }
                catch (Exception exception)
                {
                    EasyDialog.ShowErrorDialog("Phát sinh lỗi. (" + exception.Message + ")");
                }
                finally
                {
                    EasyLoadWait.CloseWaitForm();
                }
            }
        }
        protected virtual void OnExportXml()
        {
            //_base64HoSoXml = string.Empty;
            if (_dsXmlFile != null && _dsXmlFile.Tables.Count > 0)
            {
                SaveFileDialog saveFile = new SaveFileDialog()
                {
                    Title = "Thư mục lưu Xml",
                    FileName = "Chọn thư mục lưu Xml",
                    Filter = "XML Document (*.xml)|*.xml|All file (*.*)|*.*"
                };
                if (saveFile.ShowDialog() == DialogResult.OK)
                {
                    foreach (DataRow drGiamDinhHS in _dsXmlFile.Tables["XML_GIAMDINHHS"].Rows)
                    {
                        string maCSKCB = drGiamDinhHS["MACSKCB"].ToString();
                        string maLK = drGiamDinhHS["MA_LK"].ToString();
                        string ngayLap = drGiamDinhHS["NGAYLAP"].ToString();
                        string soLuongHoSo = drGiamDinhHS["SOLUONGHOSO"].ToString();
                        EasyLoadWait.ShowWaitForm(string.Format("Đang xuất dữ liệu lượt khám {0} ra file xml", maLK), this);
                        try
                        {
                            #region XUẤT DỮ LIỆU RA XML
                            StringBuilder sbXmlHoSo = new StringBuilder();
                            using (XmlWriter writerHoSoXml = XmlWriter.Create(sbXmlHoSo, _xmlWriterSettings))
                            {
                                writerHoSoXml.WriteStartDocument();
                                writerHoSoXml.WriteStartElement("GIAMDINHHS");

                                writerHoSoXml.WriteStartElement("THONGTINDONVI");
                                writerHoSoXml.WriteElementString("MACSKCB", maCSKCB);
                                writerHoSoXml.WriteEndElement();

                                writerHoSoXml.WriteStartElement("THONGTINHOSO");
                                if (DateTime.TryParse(ngayLap, out DateTime dtValue))
                                {
                                    ngayLap = dtValue.ToString("yyyyMMdd");
                                }
                                writerHoSoXml.WriteElementString("NGAYLAP", ngayLap);
                                writerHoSoXml.WriteElementString("SOLUONGHOSO", soLuongHoSo);

                                writerHoSoXml.WriteStartElement("DANHSACHHOSO");
                                writerHoSoXml.WriteStartElement("HOSO");

                                foreach (DataTable dtXmlType in _dsXmlFile.Tables)
                                {
                                    if (dtXmlType.TableName != "XML_GIAMDINHHS"
                                        && dtXmlType.Rows.Cast<DataRow>().Any(dr => maLK == dr["MA_LK"].ToString()))
                                    {
                                        writerHoSoXml.WriteStartElement("FILEHOSO");
                                        writerHoSoXml.WriteElementString("LOAIHOSO", dtXmlType.TableName);
                                        string xmlBase64 = XmlHelper.WriteXmlType2Xml(dtXmlType.TableName, dtXmlType, maLK);
                                        writerHoSoXml.WriteElementString("NOIDUNGFILE", xmlBase64);
                                        writerHoSoXml.WriteEndElement();
                                    }
                                }

                                writerHoSoXml.WriteEndElement();
                                writerHoSoXml.WriteEndElement();

                                writerHoSoXml.WriteEndElement();

                                writerHoSoXml.WriteEndElement();
                                writerHoSoXml.WriteEndDocument();
                                writerHoSoXml.Flush();
                            }
                            string xmlContent = sbXmlHoSo.ToString().Replace("encoding=\"utf-16\"", "encoding=\"utf-8\"");
                            string xmlFolder = Path.GetDirectoryName(saveFile.FileName);
                            string xmlFilePath = Path.Combine(xmlFolder, string.Format("{0}.xml", maLK));
                            File.WriteAllText(xmlFilePath, xmlContent);
                            //_base64HoSoXml = Convert.ToBase64String(Encoding.UTF8.GetBytes(xmlContent));
                            WriteLog(string.Format("Xuất XML lượt khám: {0}\nĐường dẫn file: {1}", maLK, xmlFilePath), true);
                            #endregion
                        }
                        catch (Exception exception)
                        {
                            EasyDialog.ShowErrorDialog("Phát sinh lỗi. (" + exception.Message + ")");
                        }
                        finally
                        {
                            EasyLoadWait.CloseWaitForm();
                        }
                    }
                }
            }
        }
        protected virtual void OnSendAPI()
        {
            if (!_isLogin)
            {
                EasyDialog.ShowErrorDialog("Vui lòng đăng nhập tài khoản API!");
                tcXmls.SelectedPage = AddLoginPage();
                return;
            }
            //if (string.IsNullOrWhiteSpace(_base64HoSoXml))
            //{
            //    EasyDialog.ShowErrorDialog("Chưa có file XML để gửi!\nVui lòng xuất file XML trước!");
            //    return;
            //}
            try
            {
                OpenFileDialog openFile = new OpenFileDialog()
                {
                    Multiselect = true,
                    Title = "Chọn file XML gửi đến API",
                    Filter = "XML Document (*.xml)|*.xml|All file (*.*)|*.*"
                };
                if (openFile.ShowDialog() == DialogResult.OK)
                {
                    foreach (string fileName in openFile.FileNames)
                    {
                        string base64HoSoXml = Convert.ToBase64String(File.ReadAllBytes(fileName));
                        ClientHelper.Instance.SendBase64Xml(
                            new ClientHelper.SendXmlRequest
                            {
                                maTinh = MA_TINH,
                                maCSKCB = MA_CSKCB,
                                username = USER_NAME,
                                loaiHoSo = LOAI_HO_SO,
                                fileHSBase64 = base64HoSoXml
                            }, WriteLog);
                    }
                }
            }
            catch (Exception ex)
            {
                WriteLog(string.Format("Gửi API thất bại!\n{0}", ex.Message), false);
            }
        }
        #endregion
        #region Private methods
        private void WriteLog(string message, bool isOk)
        {
            string[] lines = message.Split('\n');
            foreach (string line in lines)
            {
                if (isOk)
                {
                    lbLogs.Items.Add(string.Format("<color=Blue>{0}</color>", line));
                }
                else
                {
                    lbLogs.Items.Add(string.Format("<color=Red>{0}</color>", line));
                }
            }
        }
        private bool Login(string userName, string password)
        {
            userName = ClientHelper.Instance.Login(userName, password, WriteLog);
            _isLogin = !string.IsNullOrWhiteSpace(userName);
            if (_isLogin)
            {
                linkLogin.Text = string.Format("Xin chào {0}", userName);
            }
            return _isLogin;
        }
        private TabNavigationPage AddLoginPage()
        {
            TabNavigationPage page = tcXmls.Pages.FirstOrDefault(p => p.Name == "pageLogin") as TabNavigationPage;
            if (page != null)
            {
                tcXmls.SelectedPage = page;
                return page;
            }
            TextEdit txtUserName = new TextEdit();
            TextEdit txtPassword = new TextEdit();
            txtPassword.Properties.UseSystemPasswordChar = true;
            SimpleButton btnLogin = new SimpleButton() { Text = "Đăng nhập" };
            btnLogin.Click += (sender, e) =>
            {
                if (Login(txtUserName.Text, txtPassword.Text)) tcXmls.Pages.Remove(page);
            };
            LayoutControl layoutControl = new LayoutControl() { Dock = DockStyle.Fill };
            layoutControl.AddItem(new LayoutControlItem(layoutControl, txtUserName) { TextLocation = DevExpress.Utils.Locations.Top, Text = "Tên đăng nhập" });
            layoutControl.AddItem(new LayoutControlItem(layoutControl, txtPassword) { TextLocation = DevExpress.Utils.Locations.Top, Text = "Mật khẩu" });
            layoutControl.AddItem(new LayoutControlItem(layoutControl, btnLogin) { TextVisible = false });
            page = new TabNavigationPage() { Name = "pageLogin", Caption = "Đăng nhập API" };
            page.Controls.Add(layoutControl);
            tcXmls.Pages.Add(page);
            tcXmls.SelectedPage = page;
            txtUserName.Focus();
            return page;
        }
        private TabNavigationPage AddPage(int index, string caption, object dataSource)
        {
            CustomGridControl gridControl = new CustomGridControl { Dock = DockStyle.Fill };
            CustomGridView gridView = new CustomGridView() { GridControl = gridControl };
            gridView.CustomDrawCell += GridView_CustomDrawCell;
            gridView.OptionsView.ColumnAutoWidth = false;
            gridControl.MainView = gridView;
            gridControl.DataSource = dataSource;
            TabNavigationPage page = new TabNavigationPage() { Caption = caption };
            page.Controls.Add(gridControl);
            if (index < tcXmls.Pages.Count)
            {
                tcXmls.Pages.Insert(index, page);
            }
            else
            {
                tcXmls.Pages.Add(page);
            }
            gridView.BestFitColumns();
            return page;
        }
        #endregion
        #region Events
        private void FrmQuanLyXml_Load(object sender, EventArgs e)
        {
            OnInit();
        }
        private void txtMaLK_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                OnLoadFromDB();
            }
        }
        private void pnlButtons_ButtonClick(object sender, ButtonEventArgs e)
        {
            if (e.Button is WindowsUIButton btn)
            {
                switch (btn.Tag?.ToString())
                {
                    case "cmdImportFolder": OnOpenXmlFolder(); break;
                    case "cmdOpenXmlFile": OnOpenXmlFile(); break;
                    case "cmdCheckXml": OnCheckXml(); break;
                    case "cmdSaveDb": OnSaveToDB(); break;
                    case "cmdExportXml": OnExportXml(); break;
                    case "cmdSendApi": OnSendAPI(); break;
                    default: break;
                }
            }
        }
        private void cboXmlTypes_SelectedIndexChanged(object sender, EventArgs e)
        {
            OnLoadFromDB();
        }
        private void GridView_CustomDrawCell(object sender, DevExpress.XtraGrid.Views.Base.RowCellCustomDrawEventArgs e)
        {
            if (sender is CustomGridView gridView)
            {
                if (gridView.GetRow(e.RowHandle) is DataRowView dr
                    && dr.Row.Table.Columns.Contains("THONGTIN_LOI"))
                {
                    string error = dr["THONGTIN_LOI"].ToString();
                    if (error.Contains(e.Column.FieldName))
                    {
                        e.Appearance.BackColor = Color.Red;
                        e.Appearance.ForeColor = Color.Yellow;
                    }
                }
            }
        }
        #endregion

        private void linkLogin_Click(object sender, EventArgs e)
        {
            if (_isLogin)
            {
                _isLogin = false;
                linkLogin.Text = "Đăng nhập API";
            }
            else
            {
                tcXmls.SelectedPage = AddLoginPage();
            }
        }
    }
}