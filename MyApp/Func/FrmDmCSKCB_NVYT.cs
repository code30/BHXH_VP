using XML130.Libraries;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;
using DevExpress.Export;
using XML130.EasyEnum;
using XML130.EasyHelper;
using XML130.EasyUtils;
using XML130.InterfaceInheritance;

namespace XML130.Func
{
    public partial class FrmDmCSKCB_NVYT : FrmBase
    {


        public FrmDmCSKCB_NVYT()
        {
            InitializeComponent();

            EasyGridStyleHelper = new EasyGridStyleHelper(this, customGridControl, customGridView, false, true, false, false,
                   false, false, true, true);

            barSearch.Visible = false;
        }


        protected override void OnInit()
        {
            try
            {
                DataTable dataTable = SQLHelper.ExecuteDataTable(string.Format("SELECT * FROM tblDmCSKCB_NhanVienYTe"));
                customGridControl.DataSource = dataTable;

                EasyCommon.BestFitColumnsGridView(customGridView);
            }
            catch (Exception exception)
            {
                EasyDialog.ShowErrorDialog("Phát sinh lỗi. (" + exception.Message + ")");
            }
        }

        // hàm làm tươi
        protected override void OnReload()
        {
            OnInit();
        }


        // sửa
        protected override void OnEdit()
        {
            //if (customGridView.FocusedRowHandle < 0) return;
            //TopRowIndex = customGridView.TopRowIndex;
            //RowIndex = customGridView.FocusedRowHandle;
            //var id = customGridView.GetRowCellValue(customGridView.FocusedRowHandle, colMA_QUOCTICH).ToString(); //
            //var name = customGridView.GetRowCellValue(customGridView.FocusedRowHandle, colTEN_QUOCTICH).ToString(); //

            //EasyLoadWait.ShowWaitForm();
            //DataTable dataTable = SQLHelper.ExecuteDataTable(string.Format("SELECT * FROM tblDmNCC WHERE MaNCC = '{0}'", id));
            //F = new FrmNCCDtl(dataTable, ECommand.EUpdate);
            //F.Text = EasyMessageGlobal.EditTitle + name;
            //F.ShowDialog();
            //EasyLoadWait.CloseWaitForm();

            //if (F.IsChange) OnInit();
            //customGridView.FocusedRowHandle = RowIndex;
            //customGridView.TopRowIndex = TopRowIndex;

            ShowAlertString("Thông báo", "Bạn không có quyền sửa bản ghi này!", IconAlert.Warning);
        }

        // thêm mới
        protected override void OnAdd()
        {
            //DataTable dt = null;
            //F = new FrmNCCDtl(dt, ECommand.EAdd);
            //F.Text = EasyMessageGlobal.AddTitle;
            //F.ShowDialog();
            //if (F.IsChange) OnInit();

            ShowAlertString("Thông báo", "Bạn không có quyền thêm tính năng này!", IconAlert.Warning);
        }

        // xóa
        protected override void OnDelete()
        {
            //try
            //{
            //    if (customGridView.FocusedRowHandle < 0) return;
            //    TopRowIndex = customGridView.TopRowIndex;
            //    RowIndex = customGridView.FocusedRowHandle;
            //    var id = customGridView.GetRowCellValue(customGridView.FocusedRowHandle, colMA_QUOCTICH).ToString();
            //    if (EasyDialog.ShowYesNoDialog("Bạn có chắc chắn muốn (Xoá) mục đang chọn?") == DialogResult.Yes)
            //    {
            //        EasyLoadWait.ShowWaitForm();

            //        SQLHelper.ExecuteNonQuery(string.Format("DELETE tblDmNCC WHERE MaNCC = '{0}'", id));

            //        OnInit();
            //        EasyLoadWait.CloseWaitForm();
            //        ShowAlertString("Thông báo", "Xử lý thành công!", IconAlert.Done);
            //    }

            //    customGridView.FocusedRowHandle = RowIndex;
            //    customGridView.TopRowIndex = TopRowIndex;
            //}
            //catch (SqlException sqlEx)
            //{
            //    switch (sqlEx.Number)
            //    {
            //        case 547:
            //            ShowAlertString("Thông báo",
            //                "Không thể xoá bán ghi này!",
            //                IconAlert.Warning);
            //            break;
            //        default:
            //            throw;
            //    }
            //}
            //catch (Exception exception)
            //{
            //    EasyDialog.ShowErrorDialog("Phát sinh lỗi. (" + exception.Message + ")");
            //}
            ShowAlertString("Thông báo", "Bạn không có quyền xoá bản ghi này!", IconAlert.Warning);
        }

        private void customGridControl_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            //if (e.Button == MouseButtons.Left)
            //{
            //    OnEdit();
            //}
        }

        protected override void OnExport()
        {
            EasyCommon.ExportToExcel(Text, customGridView, null, AutoSize, ExportType.WYSIWYG, true);
        }

    }
}
