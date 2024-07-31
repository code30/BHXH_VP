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
                //btnDmTaiKhoan.Caption = @"Xin chào: " + EasyUser.FullName;
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



        private void btnXn_ItemClick(object sender, ItemClickEventArgs e)
        {

        }



        private void btnDsBacSy_ItemClick(object sender, ItemClickEventArgs e)
        {

        }

        private void btnKy_ItemClick(object sender, ItemClickEventArgs e)
        {
            EasyDialog.OpenDialog<FrmKy>();
        }



        // danh mục món ăn
        private void btnDmMonAn_ItemClick(object sender, ItemClickEventArgs e)
        {

        }

        // danh mục cơ sở kinh doanh
        private void btnDmCSKD_ItemClick(object sender, ItemClickEventArgs e)
        {

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

        private void barButtonItem14_ItemClick(object sender, ItemClickEventArgs e)
        {

        }

        private void btnDmDC_ICD_YHCT_ItemClick(object sender, ItemClickEventArgs e)
        {
            
            string typeName;
            typeName = e.Item.Tag == null ? string.Empty : e.Item.Tag.ToString();
            Form f = GetMdiFormByName(typeName);
            if (f != null)
                f.BringToFront();
            else
            {
                EasyLoadWait.ShowWaitForm();
                f = new FrmDmDC_ICD_YHCT();
                f.Name = "FrmDmDC_ICD_YHCT";
                e.Item.Tag = f.Name;
                f.Text = EasyMessageGlobal.DmDC_ICD_YHCT;
                f.MdiParent = this;
                f.Show();
                EasyLoadWait.CloseWaitForm();
            }
        }

        private void btnDmDC_ICD10_ItemClick(object sender, ItemClickEventArgs e)
        {
            string typeName;
            typeName = e.Item.Tag == null ? string.Empty : e.Item.Tag.ToString();
            Form f = GetMdiFormByName(typeName);
            if (f != null)
                f.BringToFront();
            else
            {
                EasyLoadWait.ShowWaitForm();
                f = new FrmDmDC_ICD10();
                f.Name = "FrmDmDC_ICD10";
                e.Item.Tag = f.Name;
                f.Text = EasyMessageGlobal.DmDC_ICD10;
                f.MdiParent = this;
                f.Show();
                EasyLoadWait.CloseWaitForm();
            }
        }

        private void btnDmDC_ICD9_ItemClick(object sender, ItemClickEventArgs e)
        {
            string typeName;
            typeName = e.Item.Tag == null ? string.Empty : e.Item.Tag.ToString();
            Form f = GetMdiFormByName(typeName);
            if (f != null)
                f.BringToFront();
            else
            {
                EasyLoadWait.ShowWaitForm();
                f = new FrmDmDC_ICD9();
                f.Name = "FrmDmDC_ICD9";
                e.Item.Tag = f.Name;
                f.Text = EasyMessageGlobal.DmDC_ICD9;
                f.MdiParent = this;
                f.Show();
                EasyLoadWait.CloseWaitForm();
            }
        }
    }
}