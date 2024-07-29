using DevExpress.XtraBars;
using System.Data.SqlClient;
using System.Linq;
using System.Windows.Forms;
using XML130.EasyData;
using XML130.EasyUtils;
using XML130.Func;
using XML130.InterfaceInheritance;
using XML130.Libraries;
using XML130.XML;

namespace XML130
{
    public partial class FrmMain : FrmBase
    {
        public FrmMain()
        {
            InitializeComponent();
            barSearch.Visible = false;
            ltrServer.Visibility = BarItemVisibility.Never;
            ltrDatabase.Visibility = BarItemVisibility.Never;
            btnDmTaiKhoan.Visibility = BarItemVisibility.Never;
        }

        private void FrmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (EasyDialog.ShowYesNoDialog("Bạn có chắc muốn thoát chương trình?") ==
                DialogResult.Yes)
            {
                e.Cancel = false;
            }
            else
            {
                e.Cancel = true;
            }
        }

        private Form GetMdiFormByName(string name)
        {
            return MdiChildren.FirstOrDefault(f => f.Name == name);
        }

        private void Login()
        {
            var f = new FrmLogin();
            if (f.ShowDialog() == DialogResult.OK)
            {
                CloseMdiForm();
                btnDmTaiKhoan.Visibility = BarItemVisibility.Always;
                btnDmTaiKhoan.Caption = @"Xin chào: " + EasyUser.FullName;
                Text = EasyMessageGlobal.MainTitle;
                ltrServer.Visibility = BarItemVisibility.Always;
                ltrDatabase.Visibility = BarItemVisibility.Always;

                string connectionString1 = AppConfig.GetConnectionString(HeThong.AppConfigConnectionStringName);
                string connectionString2 = connectionString1;
                if (HeThong.MaHoaChuoiKetNoi)
                    connectionString2 = EasyEncoding.Descrypt(connectionString1, "ng0ctu@n");
                SqlConnectionStringBuilder connectionStringBuilder = new SqlConnectionStringBuilder(connectionString2);
                
                string server = connectionStringBuilder.DataSource;
                string data = connectionStringBuilder.InitialCatalog;
                ltrServer.Caption = "Máy chủ: " + server;
                ltrDatabase.Caption = "CSDL: " + data;
            }
            else
                Application.ExitThread();
        }

        protected override void OnInit()
        {
            Text = EasyMessageGlobal.MainTitle;
            Login();
        }

        private void CloseMdiForm()
        {
            foreach (Form f in MdiChildren)
            {
                f.Close();
            }
        }

        private void btnLogOut_ItemClick(object sender, ItemClickEventArgs e)
        {
            CloseMdiForm();
            Login();
        }

        private void btnDoiMk_ItemClick(object sender, ItemClickEventArgs e)
        {
            var f = new FrmChangePassword();
            f.Text = EasyMessageGlobal.ChangePasswordTitle;
            f.ShowDialog();
        }

        private void btnLoaiDV_ItemClick(object sender, ItemClickEventArgs e)
        {
            //string typeName;
            //typeName = e.Item.Tag == null ? string.Empty : e.Item.Tag.ToString();
            //Form f = GetMdiFormByName(typeName);
            //if (f != null)
            //    f.BringToFront();
            //else
            //{
            //    EasyLoadWait.ShowWaitForm();
            //    f = new FrmLoaiDV();
            //    f.Name = "FrmLoaiDV";
            //    e.Item.Tag = f.Name;
            //    f.Text = EasyMessageGlobal.LoaiDVTitle;
            //    f.MdiParent = this;
            //    f.Show();
            //    EasyLoadWait.CloseWaitForm();
            //}
        }

        private void btnConnection_ItemClick(object sender, ItemClickEventArgs e)
        {
            EasyDialog.OpenDialog<FrmThietLapKetNoi>();
        }

        private void btnBackup_ItemClick(object sender, ItemClickEventArgs e) => SqlConnector.SaoLuu();

        private void btnRestore_ItemClick(object sender, ItemClickEventArgs e) => SqlConnector.PhucHoi();

        private void btnDichVu_ItemClick(object sender, ItemClickEventArgs e)
        {

        }

        // Danh mục nhà cung cấp
        private void btnSoXn_ItemClick(object sender, ItemClickEventArgs e)
        {
            
        }

        private void btnDmNCC_ItemClick(object sender, ItemClickEventArgs e)
        {
            string typeName;
            typeName = e.Item.Tag == null ? string.Empty : e.Item.Tag.ToString();
            Form f = GetMdiFormByName(typeName);
            if (f != null)
                f.BringToFront();
            else
            {
                EasyLoadWait.ShowWaitForm();
                f = new FrmNCC();
                f.Name = "FrmNCC";
                e.Item.Tag = f.Name;
                f.Text = EasyMessageGlobal.DmNCCTitle;
                f.MdiParent = this;
                f.Show();
                EasyLoadWait.CloseWaitForm();
            }
        }

        private void btnXn_ItemClick(object sender, ItemClickEventArgs e)
        {

        }

        private void btnThongSoXn_ItemClick(object sender, ItemClickEventArgs e)
        {
            string typeName;
            typeName = e.Item.Tag == null ? string.Empty : e.Item.Tag.ToString();
            Form f = GetMdiFormByName(typeName);
            if (f != null)
                f.BringToFront();
            else
            {
                EasyLoadWait.ShowWaitForm();
                //f = new FrmXetNghiemMap();
                f.Name = "FrmXetNghiemMap";
                e.Item.Tag = f.Name;
                f.Text = EasyMessageGlobal.TsXnTitle;
                f.MdiParent = this;
                f.Show();
                EasyLoadWait.CloseWaitForm();
            }
        }

        private void btnBenhNhan_ItemClick(object sender, ItemClickEventArgs e)
        {
            string typeName;
            typeName = e.Item.Tag == null ? string.Empty : e.Item.Tag.ToString();
            Form f = GetMdiFormByName(typeName);
            if (f != null)
                f.BringToFront();
            else
            {
                EasyLoadWait.ShowWaitForm();
                //f = new FrmBenhNhan();
                f.Name = "FrmBenhNhan";
                e.Item.Tag = f.Name;
                f.Text = EasyMessageGlobal.BnTitle;
                f.MdiParent = this;
                f.Show();
                EasyLoadWait.CloseWaitForm();
            }
        }

        private void btnChiDinh_ItemClick(object sender, ItemClickEventArgs e)
        {
            string typeName;
            typeName = e.Item.Tag == null ? string.Empty : e.Item.Tag.ToString();
            Form f = GetMdiFormByName(typeName);
            if (f != null)
                f.BringToFront();
            else
            {
                EasyLoadWait.ShowWaitForm();
                //f = new FrmChiDinhDv();
                f.Name = "FrmChiDinhXn";
                e.Item.Tag = f.Name;
                f.Text = EasyMessageGlobal.ChiDinhTitle;
                f.MdiParent = this;
                f.Show();
                EasyLoadWait.CloseWaitForm();
            }
        }

        private void btnDsBacSy_ItemClick(object sender, ItemClickEventArgs e)
        {

        }

        private void btnThietBi_ItemClick(object sender, ItemClickEventArgs e)
        {
            string typeName;
            typeName = e.Item.Tag == null ? string.Empty : e.Item.Tag.ToString();
            Form f = GetMdiFormByName(typeName);
            if (f != null)
                f.BringToFront();
            else
            {
                EasyLoadWait.ShowWaitForm();
                //f = new FrmThietBi();
                f.Name = "FrmThietBi";
                e.Item.Tag = f.Name;
                f.Text = EasyMessageGlobal.TbTitle;
                f.MdiParent = this;
                f.Show();
                EasyLoadWait.CloseWaitForm();
            }
        }

        private void btnKy_ItemClick(object sender, ItemClickEventArgs e)
        {
            EasyDialog.OpenDialog<FrmKy>();
        }

        private void btnChiDinhHa_ItemClick(object sender, ItemClickEventArgs e)
        {
            string typeName;
            typeName = e.Item.Tag == null ? string.Empty : e.Item.Tag.ToString();
            Form f = GetMdiFormByName(typeName);
            if (f != null)
                f.BringToFront();
            else
            {
                EasyLoadWait.ShowWaitForm();
                //f = new FrmChiDinhDHA();
                f.Name = "FrmChiDinhDHA";
                e.Item.Tag = f.Name;
                f.Text = EasyMessageGlobal.ChiDinhHATitle;
                f.MdiParent = this;
                f.Show();
                EasyLoadWait.CloseWaitForm();
            }
        }

        // báo cáo - thống kê: mẫu 1A
        //private void btnDanhThuXN_ItemClick(object sender, ItemClickEventArgs e)
        //{
            
        //}

        private void btnMauBC1A_ItemClick(object sender, ItemClickEventArgs e)
        {
            //string typeName;
            //typeName = e.Item.Tag == null ? string.Empty : e.Item.Tag.ToString();
            //Form f = GetMdiFormByName(typeName);
            //if (f != null)
            //    f.BringToFront();
            //else
            //{
            //    EasyLoadWait.ShowWaitForm();
            //    //f = new FrmBCMau1A();
            //    f.Name = "FrmBCMau1A";
            //    e.Item.Tag = f.Name;
            //    f.Text = EasyMessageGlobal.FrmBCMau1ATitle;
            //    f.MdiParent = this;
            //    f.Show();
            //    EasyLoadWait.CloseWaitForm();
            //}
        }

        // báo cáo - thống kê: mẫu 1B
        private void btnMauBC1B_ItemClick(object sender, ItemClickEventArgs e)
        {
            string typeName;
            typeName = e.Item.Tag == null ? string.Empty : e.Item.Tag.ToString();
            Form f = GetMdiFormByName(typeName);
            if (f != null)
                f.BringToFront();
            else
            {
                EasyLoadWait.ShowWaitForm();
                //f = new FrmBCMau1B();
                f.Name = "FrmBCMau1B";
                e.Item.Tag = f.Name;
                f.Text = EasyMessageGlobal.FrmBCMau1BTitle;
                f.MdiParent = this;
                f.Show();
                EasyLoadWait.CloseWaitForm();
            }
        }

        // báo cáo - thống kê: mẫu 2
        private void btnSoXetNghiem_ItemClick(object sender, ItemClickEventArgs e)
        {
            string typeName;
            typeName = e.Item.Tag == null ? string.Empty : e.Item.Tag.ToString();
            Form f = GetMdiFormByName(typeName);
            if (f != null)
                f.BringToFront();
            else
            {
                EasyLoadWait.ShowWaitForm();
                //f = new FrmBCMau2();
                f.Name = "FrmBCMau2";
                e.Item.Tag = f.Name;
                f.Text = EasyMessageGlobal.FrmBCMau2Title;
                f.MdiParent = this;
                f.Show();
                EasyLoadWait.CloseWaitForm();
            }
        }

        // báo cáo - thống kê: mẫu 3
        private void btnMauBC3_ItemClick(object sender, ItemClickEventArgs e)
        {
            string typeName;
            typeName = e.Item.Tag == null ? string.Empty : e.Item.Tag.ToString();
            Form f = GetMdiFormByName(typeName);
            if (f != null)
                f.BringToFront();
            else
            {
                EasyLoadWait.ShowWaitForm();
                //f = new FrmBCMau3();
                f.Name = "FrmBCMau3";
                e.Item.Tag = f.Name;
                f.Text = EasyMessageGlobal.FrmBCMau3Title;
                f.MdiParent = this;
                f.Show();
                EasyLoadWait.CloseWaitForm();
            }
        }

        // báo cáo - thống kê: mẫu 4
        private void btnMauBC4_ItemClick(object sender, ItemClickEventArgs e)
        {
            string typeName;
            typeName = e.Item.Tag == null ? string.Empty : e.Item.Tag.ToString();
            Form f = GetMdiFormByName(typeName);
            if (f != null)
                f.BringToFront();
            else
            {
                EasyLoadWait.ShowWaitForm();
                //f = new FrmBCMau4();
                f.Name = "FrmBCMau4";
                e.Item.Tag = f.Name;
                f.Text = EasyMessageGlobal.FrmBCMau4Title;
                f.MdiParent = this;
                f.Show();
                EasyLoadWait.CloseWaitForm();
            }
        }

        // báo cáo - thống kê: mẫu 5
        private void btnMauBC5_ItemClick(object sender, ItemClickEventArgs e)
        {
            string typeName;
            typeName = e.Item.Tag == null ? string.Empty : e.Item.Tag.ToString();
            Form f = GetMdiFormByName(typeName);
            if (f != null)
                f.BringToFront();
            else
            {
                EasyLoadWait.ShowWaitForm();
                //f = new FrmBCMau5();
                f.Name = "FrmBCMau5";
                e.Item.Tag = f.Name;
                f.Text = EasyMessageGlobal.FrmBCMau5Title;
                f.MdiParent = this;
                f.Show();
                EasyLoadWait.CloseWaitForm();
            }
        }

        // danh mục món ăn
        private void btnDmMonAn_ItemClick(object sender, ItemClickEventArgs e)
        {

        }

        // danh mục cơ sở kinh doanh
        private void btnDmCSKD_ItemClick(object sender, ItemClickEventArgs e)
        {
            string typeName;
            typeName = e.Item.Tag == null ? string.Empty : e.Item.Tag.ToString();
            Form f = GetMdiFormByName(typeName);
            if (f != null)
                f.BringToFront();
            else
            {
                EasyLoadWait.ShowWaitForm();
                //f = new FrmDmCSKD();
                f.Name = "FrmDmCSKD";
                e.Item.Tag = f.Name;
                f.Text = EasyMessageGlobal.DmCSKDTitle;
                f.MdiParent = this;
                f.Show();
                EasyLoadWait.CloseWaitForm();
            }
        }

        // Danh mục thực phẩm đông lạnh
        private void btnDmTPDL_ItemClick(object sender, ItemClickEventArgs e)
        {
            string typeName;
            typeName = e.Item.Tag == null ? string.Empty : e.Item.Tag.ToString();
            Form f = GetMdiFormByName(typeName);
            if (f != null)
                f.BringToFront();
            else
            {
                EasyLoadWait.ShowWaitForm();
               // f = new FrmDmTPDL();
                f.Name = "FrmDmTPDL";
                e.Item.Tag = f.Name;
                f.Text = EasyMessageGlobal.DmTPDLTitle;
                f.MdiParent = this;
                f.Show();
                EasyLoadWait.CloseWaitForm();
            }
        }

        // Quản lý nhập liệu bước 1A
        private void btnBuoc1A_ItemClick(object sender, ItemClickEventArgs e)
        {
            string typeName;
            typeName = e.Item.Tag == null ? string.Empty : e.Item.Tag.ToString();
            Form f = GetMdiFormByName(typeName);
            if (f != null)
                f.BringToFront();
            else
            {
                EasyLoadWait.ShowWaitForm();
                //f = new FrmNLMau1A();
                f.Name = "FrmNLMau1A";
                e.Item.Tag = f.Name;
                f.Text = EasyMessageGlobal.FrmNLMau1ATitle;
                f.MdiParent = this;
                f.Show();
                EasyLoadWait.CloseWaitForm();
            }
        }

        // Quản lý nhập liệu bước 1B
        private void btnBuoc1B_ItemClick(object sender, ItemClickEventArgs e)
        {
            string typeName;
            typeName = e.Item.Tag == null ? string.Empty : e.Item.Tag.ToString();
            Form f = GetMdiFormByName(typeName);
            if (f != null)
                f.BringToFront();
            else
            {
                EasyLoadWait.ShowWaitForm();
               // f = new FrmNLMau1B();
                f.Name = "FrmNLMau1B";
                e.Item.Tag = f.Name;
                f.Text = EasyMessageGlobal.FrmNLMau1BTitle;
                f.MdiParent = this;
                f.Show();
                EasyLoadWait.CloseWaitForm();
            }
        }

        // Quản lý nhập liệu bước 2
        private void btnBuoc2_ItemClick(object sender, ItemClickEventArgs e)
        {
            string typeName;
            typeName = e.Item.Tag == null ? string.Empty : e.Item.Tag.ToString();
            Form f = GetMdiFormByName(typeName);
            if (f != null)
                f.BringToFront();
            else
            {
                EasyLoadWait.ShowWaitForm();
                //f = new FrmNLMau2();
                f.Name = "FrmNLMau2";
                e.Item.Tag = f.Name;
                f.Text = EasyMessageGlobal.FrmNLMau2Title;
                f.MdiParent = this;
                f.Show();
                EasyLoadWait.CloseWaitForm();
            }
        }

        // Quản lý nhập liệu bước 3
        private void btnBuoc3_ItemClick(object sender, ItemClickEventArgs e)
        {
            string typeName;
            typeName = e.Item.Tag == null ? string.Empty : e.Item.Tag.ToString();
            Form f = GetMdiFormByName(typeName);
            if (f != null)
                f.BringToFront();
            else
            {
                EasyLoadWait.ShowWaitForm();
                //f = new FrmNLMau3();
                f.Name = "FrmNLMau3";
                e.Item.Tag = f.Name;
                f.Text = EasyMessageGlobal.FrmNLMau3Title;
                f.MdiParent = this;
                f.Show();
                EasyLoadWait.CloseWaitForm();
            }
        }

        // Quản lý nhập liệu bước 5
        private void btnBuoc5_ItemClick(object sender, ItemClickEventArgs e)
        {
            string typeName;
            typeName = e.Item.Tag == null ? string.Empty : e.Item.Tag.ToString();
            Form f = GetMdiFormByName(typeName);
            if (f != null)
                f.BringToFront();
            else
            {
                EasyLoadWait.ShowWaitForm();
                //f = new FrmNLMau5();
                f.Name = "FrmNLMau5";
                e.Item.Tag = f.Name;
                f.Text = EasyMessageGlobal.FrmNLMau5Title;
                f.MdiParent = this;
                f.Show();
                EasyLoadWait.CloseWaitForm();
            }
        }

        // Danh mục Lưu mẫu thức ăn
        private void btnDmLuuMauTA_ItemClick(object sender, ItemClickEventArgs e)
        {
            //string typeName;
            //typeName = e.Item.Tag == null ? string.Empty : e.Item.Tag.ToString();
            //Form f = GetMdiFormByName(typeName);
            //if (f != null)
            //    f.BringToFront();
            //else
            //{
            //    EasyLoadWait.ShowWaitForm();
            //   // f = new FrmDmMauTA();
            //    f.Name = "FrmDmMauTA";
            //    e.Item.Tag = f.Name;
            //    f.Text = EasyMessageGlobal.DmMauTATitle;
            //    f.MdiParent = this;
            //    f.Show();
            //    EasyLoadWait.CloseWaitForm();
            //}
        }

        private void btnDmQD130_DoiTuongKCB_ItemClick(object sender, ItemClickEventArgs e)
        {
            string typeName;
            typeName = e.Item.Tag == null ? string.Empty : e.Item.Tag.ToString();
            Form f = GetMdiFormByName(typeName);
            if (f != null)
                f.BringToFront();
            else
            {
                EasyLoadWait.ShowWaitForm();
                f = new FrmDmQD130_DoiTuongKCB();
                f.Name = "DmQD130_DoiTuongKCB";
                e.Item.Tag = f.Name;
                f.Text = EasyMessageGlobal.DmQD130_DoiTuongKCB;
                f.MdiParent = this;
                f.Show();
                EasyLoadWait.CloseWaitForm();
            }
        }

        private void btnDmQD130_QuocTich_ItemClick(object sender, ItemClickEventArgs e)
        {
            string typeName;
            typeName = e.Item.Tag == null ? string.Empty : e.Item.Tag.ToString();
            Form f = GetMdiFormByName(typeName);
            if (f != null)
                f.BringToFront();
            else
            {
                EasyLoadWait.ShowWaitForm();
                f = new FrmDmQD130_QuocTich();
                f.Name = "DmQD130_QuocTich";
                e.Item.Tag = f.Name;
                f.Text = EasyMessageGlobal.DmQD130_QuocTich;
                f.MdiParent = this;
                f.Show();
                EasyLoadWait.CloseWaitForm();
            }
        }

        private void btnDmQD130_NgheNghiep_ItemClick(object sender, ItemClickEventArgs e)
        {
            string typeName;
            typeName = e.Item.Tag == null ? string.Empty : e.Item.Tag.ToString();
            Form f = GetMdiFormByName(typeName);
            if (f != null)
                f.BringToFront();
            else
            {
                EasyLoadWait.ShowWaitForm();
                f = new FrmDmQD130_NgheNghiep();
                f.Name = "DmQD130_NgheNghiep";
                e.Item.Tag = f.Name;
                f.Text = EasyMessageGlobal.DmQD130_NgheNghiep;
                f.MdiParent = this;
                f.Show();
                EasyLoadWait.CloseWaitForm();
            }
        }

        private void btnDmQD130_MaTaiNan_ItemClick(object sender, ItemClickEventArgs e)
        {
            string typeName;
            typeName = e.Item.Tag == null ? string.Empty : e.Item.Tag.ToString();
            Form f = GetMdiFormByName(typeName);
            if (f != null)
                f.BringToFront();
            else
            {
                EasyLoadWait.ShowWaitForm();
                f = new FrmDmQD130_MaTaiNan();
                f.Name = "FrmDmQD130_MaTaiNan";
                e.Item.Tag = f.Name;
                f.Text = EasyMessageGlobal.DmQD130_MaTaiNan;
                f.MdiParent = this;
                f.Show();
                EasyLoadWait.CloseWaitForm();
            }
        }

        private void btnDmQD130_KetQuaDieuTri_ItemClick(object sender, ItemClickEventArgs e)
        {
            string typeName;
            typeName = e.Item.Tag == null ? string.Empty : e.Item.Tag.ToString();
            Form f = GetMdiFormByName(typeName);
            if (f != null)
                f.BringToFront();
            else
            {
                EasyLoadWait.ShowWaitForm();
                f = new FrmDmQD130_KetQuaDieuTri();
                f.Name = "FrmDmQD130_KetQuaDieuTri";
                e.Item.Tag = f.Name;
                f.Text = EasyMessageGlobal.DmQD130_KetQuaDieuTri;
                f.MdiParent = this;
                f.Show();
                EasyLoadWait.CloseWaitForm();
            }
        }

        private void btnDmQD130_MaLoaiRaVien_ItemClick(object sender, ItemClickEventArgs e)
        {
            string typeName;
            typeName = e.Item.Tag == null ? string.Empty : e.Item.Tag.ToString();
            Form f = GetMdiFormByName(typeName);
            if (f != null)
                f.BringToFront();
            else
            {
                EasyLoadWait.ShowWaitForm();
                f = new FrmDmQD130_MaLoaiRaVien();
                f.Name = "FrmDmQD130_MaLoaiRaVien";
                e.Item.Tag = f.Name;
                f.Text = EasyMessageGlobal.DmQD130_MaLoaiRaVien;
                f.MdiParent = this;
                f.Show();
                EasyLoadWait.CloseWaitForm();
            }

        }

        private void btnDmQD130_MaPhamViThanhToan_ItemClick(object sender, ItemClickEventArgs e)
        {
            string typeName;
            typeName = e.Item.Tag == null ? string.Empty : e.Item.Tag.ToString();
            Form f = GetMdiFormByName(typeName);
            if (f != null)
                f.BringToFront();
            else
            {
                EasyLoadWait.ShowWaitForm();
                f = new FrmDmQD130_MaPhamViThanhToan();
                f.Name = "FrmDmQD130_MaPhamViThanhToan";
                e.Item.Tag = f.Name;
                f.Text = EasyMessageGlobal.DmQD130_MaPVTT;
                f.MdiParent = this;
                f.Show();
                EasyLoadWait.CloseWaitForm();
            }
        }

        private void btnDmQD130_MaPTTT_ItemClick(object sender, ItemClickEventArgs e)
        {
            string typeName;
            typeName = e.Item.Tag == null ? string.Empty : e.Item.Tag.ToString();
            Form f = GetMdiFormByName(typeName);
            if (f != null)
                f.BringToFront();
            else
            {
                EasyLoadWait.ShowWaitForm();
                f = new FrmDmQD130_MaPTTT();
                f.Name = "FrmDmQD130_MaPTTT";
                e.Item.Tag = f.Name;
                f.Text = EasyMessageGlobal.DmQD130_MaPTTT;
                f.MdiParent = this;
                f.Show();
                EasyLoadWait.CloseWaitForm();
            }
        }

        private void btnDmQD130_NguonThanhToan_ItemClick(object sender, ItemClickEventArgs e)
        {
            string typeName;
            typeName = e.Item.Tag == null ? string.Empty : e.Item.Tag.ToString();
            Form f = GetMdiFormByName(typeName);
            if (f != null)
                f.BringToFront();
            else
            {
                EasyLoadWait.ShowWaitForm();
                f = new FrmDmQD130_NguonThanhToan();
                f.Name = "FrmDmQD130_NguonThanhToan";
                e.Item.Tag = f.Name;
                f.Text = EasyMessageGlobal.DmQD130_MaPTTT;
                f.MdiParent = this;
                f.Show();
                EasyLoadWait.CloseWaitForm();
            }
        }

        private void btnDmQD130_PhuongPhapVoCam_ItemClick(object sender, ItemClickEventArgs e)
        {
            string typeName;
            typeName = e.Item.Tag == null ? string.Empty : e.Item.Tag.ToString();
            Form f = GetMdiFormByName(typeName);
            if (f != null)
                f.BringToFront();
            else
            {
                EasyLoadWait.ShowWaitForm();
                f = new FrmDmQD130_PhuongPhapVoCam();
                f.Name = "FrmDmQD130_PhuongPhapVoCam";
                e.Item.Tag = f.Name;
                f.Text = EasyMessageGlobal.DmQD130_PTVC;
                f.MdiParent = this;
                f.Show();
                EasyLoadWait.CloseWaitForm();
            }
        }

        private void btnDmCSKCB_DichVuKyThuat_ItemClick(object sender, ItemClickEventArgs e)
        {
            string typeName;
            typeName = e.Item.Tag == null ? string.Empty : e.Item.Tag.ToString();
            Form f = GetMdiFormByName(typeName);
            if (f != null)
                f.BringToFront();
            else
            {
                EasyLoadWait.ShowWaitForm();
                f = new FrmDmCSKCB_DichVuKyThuat();
                f.Name = "FrmDmCSKCB_DichVuKyThuat";
                e.Item.Tag = f.Name;
                f.Text = EasyMessageGlobal.DmCSKCB_DVKT;
                f.MdiParent = this;
                f.Show();
                EasyLoadWait.CloseWaitForm();
            }
        }

        private void btnDmCSKCB_Thuoc_ItemClick(object sender, ItemClickEventArgs e)
        {
            string typeName;
            typeName = e.Item.Tag == null ? string.Empty : e.Item.Tag.ToString();
            Form f = GetMdiFormByName(typeName);
            if (f != null)
                f.BringToFront();
            else
            {
                EasyLoadWait.ShowWaitForm();
                f = new FrmDmCSKCB_Thuoc();
                f.Name = "FrmDmCSKCB_Thuoc";
                e.Item.Tag = f.Name;
                f.Text = EasyMessageGlobal.DmCSKCB_Thuoc;
                f.MdiParent = this;
                f.Show();
                EasyLoadWait.CloseWaitForm();
            }
        }

        private void btnDmCSKCB_VatTu_ItemClick(object sender, ItemClickEventArgs e)
        {
            string typeName;
            typeName = e.Item.Tag == null ? string.Empty : e.Item.Tag.ToString();
            Form f = GetMdiFormByName(typeName);
            if (f != null)
                f.BringToFront();
            else
            {
                EasyLoadWait.ShowWaitForm();
                f = new FrmDmCSKCB_VatTu();
                f.Name = "FrmDmCSKCB_VatTu";
                e.Item.Tag = f.Name;
                f.Text = EasyMessageGlobal.DmCSKCB_VatTu;
                f.MdiParent = this;
                f.Show();
                EasyLoadWait.CloseWaitForm();
            }
        }

        private void btnDmCSKCB_NVYT_ItemClick(object sender, ItemClickEventArgs e)
        {
            string typeName;
            typeName = e.Item.Tag == null ? string.Empty : e.Item.Tag.ToString();
            Form f = GetMdiFormByName(typeName);
            if (f != null)
                f.BringToFront();
            else
            {
                EasyLoadWait.ShowWaitForm();
                f = new FrmDmCSKCB_NVYT();
                f.Name = "FrmDmCSKCB_NVYT";
                e.Item.Tag = f.Name;
                f.Text = EasyMessageGlobal.DmCSKCB_NVYT;
                f.MdiParent = this;
                f.Show();
                EasyLoadWait.CloseWaitForm();
            }
        }

        private void btnDmCSKCB_KhoaPhong_ItemClick(object sender, ItemClickEventArgs e)
        {
            string typeName;
            typeName = e.Item.Tag == null ? string.Empty : e.Item.Tag.ToString();
            Form f = GetMdiFormByName(typeName);
            if (f != null)
                f.BringToFront();
            else
            {
                EasyLoadWait.ShowWaitForm();
                f = new FrmDmCSKCB_KhoaPhong();
                f.Name = "FrmDmCSKCB_KhoaPhong";
                e.Item.Tag = f.Name;
                f.Text = EasyMessageGlobal.DmCSKCB_KhoaPhong;
                f.MdiParent = this;
                f.Show();
                EasyLoadWait.CloseWaitForm();
            }
        }

        private void btnDmCSKCB_TTB_ItemClick(object sender, ItemClickEventArgs e)
        {
            string typeName;
            typeName = e.Item.Tag == null ? string.Empty : e.Item.Tag.ToString();
            Form f = GetMdiFormByName(typeName);
            if (f != null)
                f.BringToFront();
            else
            {
                EasyLoadWait.ShowWaitForm();
                f = new FrmDmCSKCB_TTB();
                f.Name = "FrmDmCSKCB_TTB";
                e.Item.Tag = f.Name;
                f.Text = EasyMessageGlobal.DmCSKCB_TTB;
                f.MdiParent = this;
                f.Show();
                EasyLoadWait.CloseWaitForm();
            }
        }

        private void barButtonItem24_ItemClick(object sender, ItemClickEventArgs e)
        {
            string typeName;
            typeName = e.Item.Tag == null ? string.Empty : e.Item.Tag.ToString();
            Form f = GetMdiFormByName(typeName);
            if (f != null)
                f.BringToFront();
            else
            {
                EasyLoadWait.ShowWaitForm();
                //f = new FrmDmQD130_ImportXml();
                //f.Name = "FrmDmQD130_ImportXml";
                f = new FrmQuanLyXml();
                f.Name = "FrmQuanLyXml";
                e.Item.Tag = f.Name;
                f.Text = EasyMessageGlobal.ImportXMLtoDB;
                f.MdiParent = this;
                f.Show();
                EasyLoadWait.CloseWaitForm();
            }
        }

        private void btnDLCC_HCVaoRa_ItemClick(object sender, ItemClickEventArgs e)
        {
            //string typeName;
            //typeName = e.Item.Tag == null ? string.Empty : e.Item.Tag.ToString();
            //Form f = GetMdiFormByName(typeName);
            //if (f != null)
            //    f.BringToFront();
            //else
            //{
            //    EasyLoadWait.ShowWaitForm();
            //    f = new FrmChamCong_DLCC_HCVaoRa();
            //    f.Name = "FrmChamCongKhuonMat";
            //    e.Item.Tag = f.Name;
            //    f.Text = EasyMessageGlobal.DLCC_HanhChinh_Vao_Ra;
            //    f.MdiParent = this;
            //    f.Show();
            //    EasyLoadWait.CloseWaitForm();
            //}
        }

        private void btnDLCC_LogCC_ItemClick(object sender, ItemClickEventArgs e)
        {
            //string typeName;
            //typeName = e.Item.Tag == null ? string.Empty : e.Item.Tag.ToString();
            //Form f = GetMdiFormByName(typeName);
            //if (f != null)
            //    f.BringToFront();
            //else
            //{
            //    EasyLoadWait.ShowWaitForm();
            //    f = new FrmChamCong_DLCC_LichSuChamCong();
            //    f.Name = "FrmChamCongKhuonMat_DataLog";
            //    e.Item.Tag = f.Name;
            //    f.Text = EasyMessageGlobal.DLCC_LogChamCong;
            //    f.MdiParent = this;
            //    f.Show();
            //    EasyLoadWait.CloseWaitForm();
            //}

        }

        private void btnChamCong_DmNhanVien_ItemClick(object sender, ItemClickEventArgs e)
        {
            //string typeName;
            //typeName = e.Item.Tag == null ? string.Empty : e.Item.Tag.ToString();
            //Form f = GetMdiFormByName(typeName);
            //if (f != null)
            //    f.BringToFront();
            //else
            //{
            //    EasyLoadWait.ShowWaitForm();
            //    //f = new FrmChamCong_DmNhanVien();
            //    f.Name = "FrmChamCong_DmNhanVien";
            //    e.Item.Tag = f.Name;
            //    f.Text = EasyMessageGlobal.DLCC_LogChamCong;
            //    f.MdiParent = this;
            //    f.Show();
            //    EasyLoadWait.CloseWaitForm();
            //}
        }

        private void btnDsDeNghiThanhToan_ItemClick(object sender, ItemClickEventArgs e)
        {
            //FrmGDBHXH_DSDeNghiThanhToan
            string typeName;
            typeName = e.Item.Tag == null ? string.Empty : e.Item.Tag.ToString();
            Form f = GetMdiFormByName(typeName);
            if (f != null)
                f.BringToFront();
            else
            {
                EasyLoadWait.ShowWaitForm();
                f = new FrmGDBHXH_DSDeNghiThanhToan();
                f.Name = "FrmGDBHXH_DSDeNghiThanhToan";
                e.Item.Tag = f.Name;
                f.Text = EasyMessageGlobal.DLCC_LogChamCong;
                f.MdiParent = this;
                f.Show();
                EasyLoadWait.CloseWaitForm();
            }
        }
    }
}