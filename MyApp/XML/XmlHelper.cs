using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using XML130.EasyUtils;
using XML130.Libraries;

namespace XML130.XML
{
    public class XmlHelper
    {
        public static Dictionary<string, string> XmlTypes { get; }
            = new Dictionary<string, string>()
            {
                { "XML_GIAMDINHHS", "Giám định hồ sơ" },
                { "XML1", "XML 1" },
                { "XML2", "XML 2" },
                { "XML3", "XML 3" },
                { "XML4", "XML 4" },
                { "XML5", "XML 5" },
                { "XML6", "XML 6" },
                { "XML7", "XML 7" },
                { "XML8", "XML 8" },
                { "XML9", "XML 9" },
                { "XML10", "XML 10" },
                { "XML11", "XML 11" },
            };

        /// <summary>
        /// Đọc file XML giám định hồ sơ vào Dataset với các table tương ứng XML 1, XML 2,...
        /// </summary>
        /// <param name="xmlFile">Đường dẫn file</param>
        /// <returns></returns>
        public static DataSet LoadXmlFile(string xmlFile)
        {
            if (!File.Exists(xmlFile)) return null;
            DataSet dsXmlOpenned = new DataSet();
            DataSet dsXmlFile = new DataSet();
            dsXmlFile.ReadXml(xmlFile);
            #region LẤY THÔNG TIN HỒ SƠ
            DataTable dtGiamDinhHoSo = new DataTable("XML_GIAMDINHHS");
            dtGiamDinhHoSo.Columns.Add("ID");
            dtGiamDinhHoSo.Columns.Add("KEY");
            dtGiamDinhHoSo.Columns.Add("MA_LK");
            dtGiamDinhHoSo.Columns.Add("MACSKCB");
            dtGiamDinhHoSo.Columns.Add("NGAYLAP");
            dtGiamDinhHoSo.Columns.Add("SOLUONGHOSO");
            DataRow drGiamDinhHoSo = dtGiamDinhHoSo.NewRow();
            if (dsXmlFile.Tables.Contains("THONGTINDONVI"))
            {
                drGiamDinhHoSo["MACSKCB"] = dsXmlFile.Tables["THONGTINDONVI"].Rows[0]["MACSKCB"].ToString();
            }
            if (dsXmlFile.Tables.Contains("THONGTINHOSO"))
            {
                drGiamDinhHoSo["NGAYLAP"] = dsXmlFile.Tables["THONGTINHOSO"].Rows[0]["NGAYLAP"].ToString();
                drGiamDinhHoSo["SOLUONGHOSO"] = dsXmlFile.Tables["THONGTINHOSO"].Rows[0]["SOLUONGHOSO"].ToString();
            }
            dtGiamDinhHoSo.Rows.Add(drGiamDinhHoSo);
            dsXmlOpenned.Tables.Add(dtGiamDinhHoSo);
            #endregion
            #region ĐỌC FILE HỒ SƠ
            if (dsXmlFile.Tables.Contains("FILEHOSO") && dsXmlFile.Tables["FILEHOSO"].Rows.Count > 0)
            {
                foreach (DataRow drFileHoSo in dsXmlFile.Tables["FILEHOSO"].Rows)
                {
                    string xmlType = drFileHoSo["LOAIHOSO"].ToString();
                    string xmlContent = drFileHoSo["NOIDUNGFILE"].ToString();

                    using (Stream stream = EasyEncoding.GenerateStreamFromString(EasyEncoding.Base64Decode(xmlContent)))
                    {
                        DataSet dsXmlType = new DataSet();
                        dsXmlType.ReadXml(stream);
                        if (dsXmlType.Tables.Count > 0)
                        {
                            foreach (DataTable dtXmlType in dsXmlType.Tables)
                            {
                                if (dtXmlType.Columns.Contains("MA_LK") && dtXmlType.Rows.Count > 0)
                                {
                                    if (dtGiamDinhHoSo.Rows[0].IsNull("MA_LK")
                                        && !dtXmlType.Rows[0].IsNull("MA_LK"))
                                    {
                                        dtGiamDinhHoSo.Rows[0]["MA_LK"] = dtXmlType.Rows[0]["MA_LK"];
                                    }
                                    DataTable dt = dtXmlType.DefaultView.ToTable(xmlType);
                                    if (!dt.Columns.Contains("MA_LOI")) dt.Columns.Add("MA_LOI");
                                    if (!dt.Columns.Contains("THONGTIN_LOI")) dt.Columns.Add("THONGTIN_LOI");
                                    dsXmlOpenned.Tables.Add(dt);
                                    break;
                                }
                            }
                        }
                    }
                }
            }
            #endregion
            return dsXmlOpenned;
        }

        public static DataSet LoadXmlDataFromDb(string maLK)
        {
            DataSet ds = new DataSet();
            foreach (string xmlType in XmlTypes.Keys)
            {
                DataTable dt = LoadXmlDataFromDb(xmlType, maLK);
                ds.Tables.Add(dt);
            }
            return ds;
        }

        public static DataTable LoadXmlDataFromDb(string xmlType, string maLK)
        {
            string sql = string.Format("IF OBJECT_ID('{0}', 'U') IS NOT NULL SELECT * FROM {0} WHERE ISNULL('{1}','')='' OR MA_LK='{1}';", xmlType, maLK);
            DataTable dt = SQLHelper.ExecuteDataTable(sql);
            dt.TableName = xmlType;
            return dt;
        }

        public static Dictionary<string, List<ClsXmlError>> ValidateXml(ref DataSet dsXmlFile)
        {
            Dictionary<string, List<ClsXmlError>> dictErrors = new Dictionary<string, List<ClsXmlError>>();
            DataTable dtErrors = SQLHelper.ExecuteDataTable("SELECT * FROM XMLERROR");
            if (dtErrors != null && dtErrors.Rows.Count > 0)
            {
                foreach (DataTable dtXml in dsXmlFile.Tables)
                {
                    if (!dictErrors.ContainsKey(dtXml.TableName))
                    {
                        dictErrors.Add(dtXml.TableName, new List<ClsXmlError>());
                    }
                    foreach (DataRow dr in dtXml.Rows)
                    {
                        List<ClsXmlError> lstRowErrors = new List<ClsXmlError>();
                        foreach (DataRow drError in dtErrors.Rows)
                        {
                            ClsXmlError error = new ClsXmlError()
                            {
                                Item = drError["ITEM"].ToString(),
                                XmlType = drError["XML"].ToString(),
                                MaLoiCha = drError["MA_LOI_CHA"].ToString(),
                                MaLoiCon = drError["MA_LOI_CON"].ToString(),
                                NoiDungLoi = drError["NOI_DUNG_LOI"].ToString(),
                            };
                            if (error.XmlType == dtXml.TableName)
                            {
                                string value = dr[error.Item].ToString();
                                if (string.IsNullOrWhiteSpace(value))
                                {
                                    lstRowErrors.Add(error);
                                }
                                switch (error.MaLoiCon)
                                {
                                    #region XML1
                                    case "105002": //105002	GIOI_TINH không tồn tại
                                        {
                                            if (!int.TryParse(value, out int kq) || kq < 1 || kq > 3) lstRowErrors.Add(error);
                                            break;
                                        }
                                    case "108002": //108002	MA_QUOCTICH không tồn tại trong danh mục
                                        {
                                            string sql = string.Format("SELECT [TEN_QUOCTICH] FROM tblDmQD130_QuocTich WHERE [MA_QUOCTICH]='{0}' ", value);
                                            string quocTich = SQLHelper.ExecuteScalar<string>(sql);
                                            if (string.IsNullOrWhiteSpace(quocTich)) lstRowErrors.Add(error);
                                            break;
                                        }
                                    case "110002": //110002	MA_NGHE_NGHIEP không tồn tại trong danh mục
                                        {
                                            if (value != "00000")
                                            {
                                                StringBuilder sb = new StringBuilder();
                                                sb.AppendLine("SELECT TEN_NGHE_NGHIEP FROM tblDmQD130_NgheNghiep ");
                                                sb.AppendFormat("WHERE [MA_NGHE_NGHIEP]='{0}' ", value);
                                                sb.AppendFormat("OR [MA_NGHE_NGHIEP_C1]='{0}' ", value);
                                                sb.AppendFormat("OR [MA_NGHE_NGHIEP_C2]='{0}' ", value);
                                                sb.AppendFormat("OR [MA_NGHE_NGHIEP_C3]='{0}' ", value);
                                                sb.AppendFormat("OR [MA_NGHE_NGHIEP_C4]='{0}' ", value);
                                                sb.AppendFormat("OR [MA_NGHE_NGHIEP_C5]='{0}' ", value);
                                                string ngheNghiep = SQLHelper.ExecuteScalar<string>(sb.ToString());
                                                if (string.IsNullOrWhiteSpace(ngheNghiep)) lstRowErrors.Add(error);
                                            }
                                            break;
                                        }
                                    case "140002": //140002	KET_QUA_DTRI không tồn tại
                                        {
                                            if (!int.TryParse(value, out int kq) || kq < 1 || kq > 7) lstRowErrors.Add(error);
                                            break;
                                        }
                                    case "141002": //141002	MA_LOAI_RV không tồn tại
                                        {
                                            if (!int.TryParse(value, out int kq) || kq < 1 || kq > 5) lstRowErrors.Add(error);
                                            break;
                                        }
                                    case "159002": //159002	CAN_NANG phải là dạng số
                                        {
                                            if (!double.TryParse(value, out double kq)) lstRowErrors.Add(error);
                                            break;
                                        }
                                    #endregion
                                    #region XML2
                                    case "206002":
                                        {
                                            //MA_NHOM không tồn tại trong danh mục
                                            break;
                                        }
                                    case "216002": //216002	PHAM_VI  không tồn tại trong danh mục
                                        {
                                            if (!int.TryParse(value, out int kq) || kq < 1 || kq > 3) lstRowErrors.Add(error);
                                            break;
                                        }
                                    case "217002": //217002	TYLE_TT_BH đang khác 0 với các hồ sơ ngoài phạm vi thanh toán BHYT
                                        {
                                            string phamVi = dr["PHAM_VI"].ToString();
                                            if (!int.TryParse(value, out int kq)
                                                || ((phamVi == "2" || phamVi == "3") && kq != 0))
                                            {
                                                lstRowErrors.Add(error);
                                            }
                                            break;
                                        }
                                    case "2310021": //2310021	MA_KHOA không tồn tại trong danh mục
                                        {
                                            string sql = string.Format("SELECT [TEN] FROM tblDmCSKCB_KhoaPhong WHERE [MA]='{0}' ", value);
                                            string tenKhoa = SQLHelper.ExecuteScalar<string>(sql);
                                            if (string.IsNullOrWhiteSpace(tenKhoa)) lstRowErrors.Add(error);
                                            break;
                                        }
                                    case "232002": //232002	MA_BAC_SI không tồn tại trong danh mục
                                        {
                                            string maBacsi = string.Join("','", value.Split(';'));
                                            string sql = string.Format("SELECT HO_TEN FROM tblDmCSKCB_NhanVienYTe WHERE MACCHN in ('{0}') ", maBacsi);
                                            string tenBacsi = SQLHelper.ExecuteScalar<string>(sql);
                                            if (string.IsNullOrWhiteSpace(tenBacsi)) lstRowErrors.Add(error);
                                            break;
                                        }
                                    case "234002": //234002	NGAY_YL không được nhỏ hơn ngày vào viện
                                        {
                                            if (dsXmlFile.Tables.Contains("XML1"))
                                            {
                                                foreach (DataRow drXml1 in dsXmlFile.Tables["XML1"].Rows)
                                                {
                                                    if (long.TryParse(drXml1["NGAY_VAO"].ToString(), out long ngayVao)
                                                        && long.TryParse(value, out long ngayYL)
                                                        && ngayYL < ngayVao)
                                                    {
                                                        lstRowErrors.Add(error);
                                                    }
                                                }
                                            }
                                            break;
                                        }

                                    ///234003	NGAY_YL không được lớn hơn ngày ra viện
                                    case "234003": 
                                        {
                                            if (dsXmlFile.Tables.Contains("XML1"))
                                            {
                                                foreach (DataRow drXml1 in dsXmlFile.Tables["XML1"].Rows)
                                                {
                                                    if (long.TryParse(drXml1["NGAY_RA"].ToString(), out long ngayRa)
                                                        && long.TryParse(value, out long ngayYL)
                                                        && ngayYL > ngayRa)
                                                    {
                                                        lstRowErrors.Add(error);
                                                    }
                                                }
                                            }
                                            break;
                                        }

                                    ///235001	MA_PTTT không được để trống
                                    case "235001": 
                                        {
                                            if (!int.TryParse(value, out int kq) || kq < 1 || kq > 3) lstRowErrors.Add(error);
                                            break;
                                        }
                                    
                                    /// 236002  NGUON_CTRA không tồn tại trong danh mục
                                    case "236002": 
                                        {
                                            if (!int.TryParse(value, out int kq) || kq < 1 || kq > 4) lstRowErrors.Add(error);
                                            break;
                                        }

                                    ///209001	HAM_LUONG không được để trống
                                    case "209001":
                                        {
                                            if (int.TryParse(dr["MA_NHOM"].ToString(), out int maNhom) && 17== maNhom)
                                            {
                                                lstRowErrors.Remove(error);
                                            }
                                            break;
                                        }

                                    ///210001	DUONG_DUNG không được để trống
                                    case "210001":
                                        {
                                            if (int.TryParse(dr["MA_NHOM"].ToString(), out int maNhom) && 17 == maNhom)
                                            {
                                                lstRowErrors.Remove(error);
                                            }
                                            break;
                                        }

                                    ///214001	SO_DANG_KY không được để trống
                                    case "214001":
                                        {
                                            if (int.TryParse(dr["MA_NHOM"].ToString(), out int maNhom) && 17 == maNhom)
                                            {
                                                lstRowErrors.Remove(error);
                                            }
                                            break;
                                        }

                                    ///2150001	TT_THAU không được để trống
                                    case "2150001":
                                        {
                                            if (int.TryParse(dr["MA_NHOM"].ToString(), out int maNhom) && 17 == maNhom)
                                            {
                                                lstRowErrors.Remove(error);
                                            }
                                            break;
                                        }

                                    #endregion

                                    #region XML3
                                    case "305001":
                                    case "308001":
                                        {
                                            // Trường hợp nếu mã nhóm = 10 thì không ghi lỗi này
                                            if (int.TryParse(dr["MA_NHOM"].ToString(), out int maNhom) && 10 != maNhom)
                                            {
                                                lstRowErrors.Remove(error);
                                            }
                                            break;
                                        }

                                    case "303001": // 303001: MA_DICH_VU không được để trống
                                    case "309001": // 309001: TEN_DICH_VU không được để trống
                                        {
                                            
                                            if (int.TryParse(dr["MA_NHOM"].ToString(), out int maNhom) && 10 == maNhom) // nếu MA_NHOM=10 --> MA_DICH_VU && TEN_DICH_VU: được để trống
                                            {
                                                lstRowErrors.Remove(error);
                                            }
                                            break;
                                        }

                                    case "316001": // 316001: TT_THAU không được để trống
                                    case "316002": // 316002: TT_THAU sai định dạng khi mã nhóm bằng 10 hoặc 11
                                        {

                                            if (int.TryParse(dr["MA_NHOM"].ToString(), out int maNhom) && 1== maNhom) // nếu MA_NHOM=1 (xét nghiệm) --> TT_THAU: được để trống
                                            {
                                                lstRowErrors.Remove(error);
                                            }
                                            if (int.TryParse(dr["MA_NHOM"].ToString(), out int maNhom2) && 2 == maNhom2) // nếu MA_NHOM=2 (cdha) --> TT_THAU: được để trống
                                            {
                                                lstRowErrors.Remove(error);
                                            }
                                            if (int.TryParse(dr["MA_NHOM"].ToString(), out int maNhom3) && 3 == maNhom3) // nếu MA_NHOM=1 (tdcn) --> TT_THAU: được để trống
                                            {
                                                lstRowErrors.Remove(error);
                                            }
                                            if (int.TryParse(dr["MA_NHOM"].ToString(), out int maNhom4) && 8 == maNhom4) // nếu MA_NHOM=1 (phẫu thuật) --> TT_THAU: được để trống
                                            {
                                                lstRowErrors.Remove(error);
                                            }
                                            if (int.TryParse(dr["MA_NHOM"].ToString(), out int maNhom5) && 12 == maNhom5) // nếu MA_NHOM=1 (vận chuyển) --> TT_THAU: được để trống
                                            {
                                                lstRowErrors.Remove(error);
                                            }
                                            if (int.TryParse(dr["MA_NHOM"].ToString(), out int maNhom6) && 13 == maNhom6) // nếu MA_NHOM=1 (khám bệnh) --> TT_THAU: được để trống
                                            {
                                                lstRowErrors.Remove(error);
                                            }
                                            if (int.TryParse(dr["MA_NHOM"].ToString(), out int maNhom7) && 14 == maNhom7) // nếu MA_NHOM=1 (ngày giường ban ngày) --> TT_THAU: được để trống
                                            {
                                                lstRowErrors.Remove(error);
                                            }
                                            if (int.TryParse(dr["MA_NHOM"].ToString(), out int maNhom8) && 15 == maNhom8) // nếu MA_NHOM=1 (ngày giường nội trú) --> TT_THAU: được để trống
                                            {
                                                lstRowErrors.Remove(error);
                                            }
                                            if (int.TryParse(dr["MA_NHOM"].ToString(), out int maNhom9) && 16 == maNhom9) // nếu MA_NHOM=1 (giường lưu trú) --> TT_THAU: được để trống
                                            {
                                                lstRowErrors.Remove(error);
                                            }
                                            if (int.TryParse(dr["MA_NHOM"].ToString(), out int maNhom10) && 18 == maNhom10) // nếu MA_NHOM=1 (thủ thuật) --> TT_THAU: được để trống
                                            {
                                                lstRowErrors.Remove(error);
                                            }
                                            break;
                                        }

                                    case "334001": // 334001: NGUOI_THUC_HIEN không được để trống khi mã nhóm bằng 1 2 3 8 18
                                        {

                                            if (int.TryParse(dr["MA_NHOM"].ToString(), out int maNhom) && 10 == maNhom) // nếu MA_NHOM=10 --> NGUOI_THUC_HIEN: được để trống
                                            {
                                                lstRowErrors.Remove(error);
                                            }
                                            if (int.TryParse(dr["MA_NHOM"].ToString(), out int maNhom2) && 13 == maNhom2) // nếu MA_NHOM=13 --> NGUOI_THUC_HIEN: được để trống
                                            {
                                                lstRowErrors.Remove(error);
                                            }

                                            if (int.TryParse(dr["MA_NHOM"].ToString(), out int maNhom3) && 15 == maNhom3) // nếu MA_NHOM=15 --> NGUOI_THUC_HIEN: được để trống
                                            {
                                                lstRowErrors.Remove(error);
                                            }

                                            break;
                                        }

                                    case "337001": // 337001: NGAY_TH_YL không được để trống khi mã nhóm bằng 1 2 3 8 18  
                                    case "337002": // 337002  NGAY_TH_YL không được nhỏ hơn ngày y lệnh
                                        {

                                            if (int.TryParse(dr["MA_NHOM"].ToString(), out int maNhom) && 7 == maNhom) // nếu MA_NHOM==7 (máu) --> NGAY_TH_YL: được để trống
                                            {
                                                lstRowErrors.Remove(error);
                                            }
                                            if (int.TryParse(dr["MA_NHOM"].ToString(), out int maNhom2) && 10 == maNhom2) // nếu MA_NHOM==10 (vật tư) --> NGAY_TH_YL: được để trống
                                            {
                                                lstRowErrors.Remove(error);
                                            }
                                            if (int.TryParse(dr["MA_NHOM"].ToString(), out int maNhom3) && 11 == maNhom3) // nếu MA_NHOM==11 (vật tư) --> NGAY_TH_YL: được để trống
                                            {
                                                lstRowErrors.Remove(error);
                                            }
                                            if (int.TryParse(dr["MA_NHOM"].ToString(), out int maNhom9) && 12 == maNhom9) // nếu MA_NHOM==12 (vận chuyển) --> NGAY_TH_YL: được để trống
                                            {
                                                lstRowErrors.Remove(error);
                                            }
                                            if (int.TryParse(dr["MA_NHOM"].ToString(), out int maNhom4) && 13 == maNhom4) // nếu MA_NHOM==13 (khám bệnh) --> NGAY_TH_YL: được để trống
                                            {
                                                lstRowErrors.Remove(error);
                                            }

                                            if (int.TryParse(dr["MA_NHOM"].ToString(), out int maNhom5) && 14 == maNhom5) // nếu MA_NHOM==14 (ngày giường ban ngày) --> NGAY_TH_YL: được để trống
                                            {
                                                lstRowErrors.Remove(error);
                                            }

                                            if (int.TryParse(dr["MA_NHOM"].ToString(), out int maNhom6) && 15 == maNhom6) // nếu MA_NHOM==15 (ngày giường nội trú) --> NGAY_TH_YL: được để trống
                                            {
                                                lstRowErrors.Remove(error);
                                            }

                                            if (int.TryParse(dr["MA_NHOM"].ToString(), out int maNhom7) && 16 == maNhom7) // nếu MA_NHOM==16 (ngày giường lưu) --> NGAY_TH_YL: được để trống
                                            {
                                                lstRowErrors.Remove(error);
                                            }

                                            if (int.TryParse(dr["MA_NHOM"].ToString(), out int maNhom8) && 17 == maNhom8) // nếu MA_NHOM==17 (chế phẩm máu) --> NGAY_TH_YL: được để trống
                                            {
                                                lstRowErrors.Remove(error);
                                            }
                                            break;
                                        }

                                        // chưa hiểu tại sao case nhiều luồng như vậy ? --> mới lọc được
                                        //case "338001": // 338001	NGAY_KQ không được nhỏ hơn ngày thực hiện y lệnh
                                        //    {                                            
                                        //        int[] nhomDuocDeTrong = new int[] { 10, 13, 15 };
                                        //        if (string.IsNullOrWhiteSpace(value))
                                        //        {
                                        //            if (int.TryParse(dr["MA_NHOM"].ToString(), out int maNhom) && nhomDuocDeTrong.Contains(maNhom)) //nếu MA_NHOM = { 10,13,15}-- > NGAY_KQ: được để trống
                                        //            {
                                        //                lstRowErrors.Remove(error);
                                        //            }
                                        //        }

                                        //        else // NGAY_KQ không được nhỏ hơn ngày y lệnh
                                        //        {
                                        //            long.TryParse(value, out long ngayKQ);
                                        //            long.TryParse(dr["NGAY_YL"].ToString(), out long ngayYL);
                                        //            long.TryParse(dr["NGAY_TH_YL"].ToString(), out long ngayTHYL);
                                        //            if (ngayKQ < ngayYL || ngayKQ < ngayTHYL)
                                        //            {
                                        //                lstRowErrors.Add(error);
                                        //            }
                                        //        }

                                        //        break;

                                        //    }

                                        //case "338002": // 338002	NGAY_KQ không được nhỏ hơn ngày y lệnh
                                        //    {
                                        //        int[] nhomDuocDeTrong = new int[] { 10, 13, 15 };
                                        //        if (string.IsNullOrWhiteSpace(value))
                                        //        {
                                        //            if (int.TryParse(dr["MA_NHOM"].ToString(), out int maNhom) && nhomDuocDeTrong.Contains(maNhom)) //nếu MA_NHOM = { 10,13,15}-- > NGAY_KQ: được để trống
                                        //            {
                                        //                lstRowErrors.Remove(error);
                                        //            }
                                        //        }

                                        //        else // NGAY_KQ không được nhỏ hơn ngày y lệnh
                                        //        {
                                        //            long.TryParse(value, out long ngayKQ);
                                        //            long.TryParse(dr["NGAY_YL"].ToString(), out long ngayYL);
                                        //            long.TryParse(dr["NGAY_TH_YL"].ToString(), out long ngayTHYL);
                                        //            if (ngayKQ < ngayYL || ngayKQ < ngayTHYL)
                                        //            {
                                        //                lstRowErrors.Add(error);
                                        //            }
                                        //        }

                                        //        break;

                                        //    }
                                        //case "338003": // 338003	NGAY_KQ không được lơn hơn ngày ra viện
                                        //    {
                                        //        int[] nhomDuocDeTrong = new int[] { 10, 13, 15 };
                                        //        if (string.IsNullOrWhiteSpace(value))
                                        //        {
                                        //            if (int.TryParse(dr["MA_NHOM"].ToString(), out int maNhom) && nhomDuocDeTrong.Contains(maNhom)) //nếu MA_NHOM = { 10,13,15}-- > NGAY_KQ: được để trống
                                        //            {
                                        //                lstRowErrors.Remove(error);
                                        //            }
                                        //        }

                                        //        else // NGAY_KQ không được nhỏ hơn ngày y lệnh
                                        //        {
                                        //            long.TryParse(value, out long ngayKQ);
                                        //            long.TryParse(dr["NGAY_YL"].ToString(), out long ngayYL);
                                        //            long.TryParse(dr["NGAY_TH_YL"].ToString(), out long ngayTHYL);
                                        //            if (ngayKQ < ngayYL || ngayKQ < ngayTHYL)
                                        //            {
                                        //                lstRowErrors.Add(error);
                                        //            }
                                        //        }

                                        //        break;

                                        //    }
                                        //case "338004": // 338004	NGAY_KQ không được để trống
                                        //    {

                                        //        //if (int.TryParse(dr["MA_NHOM"].ToString(), out int maNhom) && 10 == maNhom) // nếu MA_NHOM=10 (vật tư) --> NGAY_KQ: được để trống
                                        //        //{
                                        //        //    lstRowErrors.Remove(error);
                                        //        //}
                                        //        //if (int.TryParse(dr["MA_NHOM"].ToString(), out int maNhom2) && 13 == maNhom2) // nếu MA_NHOM=13 (khám bệnh) --> NGAY_KQ: được để trống
                                        //        //{
                                        //        //    lstRowErrors.Remove(error);
                                        //        //}

                                        //        //if (int.TryParse(dr["MA_NHOM"].ToString(), out int maNhom3) && 15 == maNhom3) // nếu MA_NHOM=15 --> NGAY_KQ: được để trống
                                        //        //{
                                        //        //    lstRowErrors.Remove(error);
                                        //        //}

                                        //        //break;
                                        //        int[] nhomDuocDeTrong = new int [] { 10,13,15};
                                        //        if (string.IsNullOrWhiteSpace(value))
                                        //        {
                                        //            if (int.TryParse(dr["MA_NHOM"].ToString(), out int maNhom) && nhomDuocDeTrong.Contains(maNhom)) //nếu MA_NHOM = { 10,13,15}-- > NGAY_KQ: được để trống
                                        //            {
                                        //                lstRowErrors.Remove(error);
                                        //            }
                                        //        }

                                        //        else // NGAY_KQ không được nhỏ hơn ngày y lệnh
                                        //        {
                                        //            long.TryParse(value, out long ngayKQ);
                                        //            long.TryParse(dr["NGAY_YL"].ToString(), out long ngayYL);
                                        //            long.TryParse(dr["NGAY_TH_YL"].ToString(), out long ngayTHYL);
                                        //            if (ngayKQ < ngayYL || ngayKQ < ngayTHYL)
                                        //            {
                                        //                lstRowErrors.Add(error);
                                        //            }
                                        //        }

                                        //        break;

                                        //    }

                                    /// NGAY_KQ không được nhỏ hơn ngày thực hiện y lệnh
                                    case "338001":
                                        {
                                            int[] nhomDuocDeTrong = new int[] { 10, 13, 15 };
                                            if (string.IsNullOrWhiteSpace(value))
                                            {
                                                if (int.TryParse(dr["MA_NHOM"].ToString(), out int maNhom)
                                                    && nhomDuocDeTrong.Contains(maNhom))
                                                {
                                                    lstRowErrors.Remove(error);
                                                }
                                            }
                                            else
                                            {
                                                long.TryParse(value, out long ngayKQ);
                                                long.TryParse(dr["NGAY_TH_YL"].ToString(), out long ngayTHYL);
                                                if (ngayKQ < ngayTHYL)
                                                {
                                                    lstRowErrors.Add(error);
                                                }
                                            }
                                            break;
                                        }
                                    /// NGAY_KQ không được nhỏ hơn ngày y lệnh
                                    case "338002":
                                        {
                                            int[] nhomDuocDeTrong = new int[] { 10, 13, 15 };
                                            if (string.IsNullOrWhiteSpace(value))
                                            {
                                                if (int.TryParse(dr["MA_NHOM"].ToString(), out int maNhom)
                                                    && nhomDuocDeTrong.Contains(maNhom))
                                                {
                                                    lstRowErrors.Remove(error);
                                                }
                                            }
                                            else
                                            {
                                                long.TryParse(value, out long ngayKQ);
                                                long.TryParse(dr["NGAY_YL"].ToString(), out long ngayYL);
                                                if (ngayKQ < ngayYL)
                                                {
                                                    lstRowErrors.Add(error);
                                                }
                                            }
                                            break;
                                        }
                                    /// NGAY_KQ không được lơn hơn ngày ra viện
                                    case "338003":
                                        {
                                            int[] nhomDuocDeTrong = new int[] { 10, 13, 15 };
                                            if (string.IsNullOrWhiteSpace(value))
                                            {
                                                if (int.TryParse(dr["MA_NHOM"].ToString(), out int maNhom)
                                                    && nhomDuocDeTrong.Contains(maNhom))
                                                {
                                                    lstRowErrors.Remove(error);
                                                }
                                            }
                                            else
                                            {
                                                /// Lấy dữ liệu NGAY_RA từ XML1 và so sánh với NGAY_KQ
                                                if (dsXmlFile.Tables.Contains("XML1"))
                                                {
                                                    foreach (DataRow drXml1 in dsXmlFile.Tables["XML1"].Rows)
                                                    {
                                                        if (long.TryParse(drXml1["NGAY_RA"].ToString(), out long ngayRa)
                                                            && long.TryParse(value, out long ngayKQ)
                                                            && ngayKQ > ngayRa)
                                                        {
                                                            lstRowErrors.Add(error);
                                                        }
                                                    }
                                                }
                                            }
                                            break;
                                        }
                                    /// NGAY_KQ không được để trống
                                    case "338004":
                                        {
                                            int[] nhomDuocDeTrong = new int[] { 10, 13, 15 };
                                            if (string.IsNullOrWhiteSpace(value))
                                            {
                                                if (int.TryParse(dr["MA_NHOM"].ToString(), out int maNhom)
                                                    && nhomDuocDeTrong.Contains(maNhom))
                                                {
                                                    lstRowErrors.Remove(error);
                                                }
                                            }
                                            break;
                                        }

                                    case "332001": // 332001  MA_GIUONG không đúng định dạng
                                        {

                                            if (int.TryParse(dr["MA_NHOM"].ToString(), out int maNhom) && 15 != maNhom) // nếu MA_NHOM!=15 --> MA_GIUONG: được để trống
                                            {
                                                lstRowErrors.Remove(error);
                                            }
                                            break;
                                        }


                                    case "343001": // 343001	TAI_SU_DUNG không tồn tại trong danh mục
                                        {

                                            if (int.TryParse(dr["MA_NHOM"].ToString(), out int maNhom) && 11 != maNhom) // nếu MA_NHOM!=11 --> TAI_SU_DUNG: được để trống
                                            {
                                                lstRowErrors.Remove(error);
                                            }
                                            break;
                                        }

                                    case "342001": // 342001	MA_MAY không tồn tại trong danh mục
                                        {

                                            if (int.TryParse(dr["MA_NHOM"].ToString(), out int maNhom) && 7 == maNhom) // nếu MA_NHOM==7 (máu) --> MA_MAY: được để trống
                                            {
                                                lstRowErrors.Remove(error);
                                            }
                                            if (int.TryParse(dr["MA_NHOM"].ToString(), out int maNhom2) && 10 == maNhom2) // nếu MA_NHOM==10 (vật tư) --> MA_MAY: được để trống
                                            {
                                                lstRowErrors.Remove(error);
                                            }
                                            if (int.TryParse(dr["MA_NHOM"].ToString(), out int maNhom3) && 11 == maNhom3) // nếu MA_NHOM==11 (vật tư) --> MA_MAY: được để trống
                                            {
                                                lstRowErrors.Remove(error);
                                            }
                                            if (int.TryParse(dr["MA_NHOM"].ToString(), out int maNhom9) && 12 == maNhom9) // nếu MA_NHOM==12 (vận chuyển) --> MA_MAY: được để trống
                                            {
                                                lstRowErrors.Remove(error);
                                            }
                                            if (int.TryParse(dr["MA_NHOM"].ToString(), out int maNhom4) && 13 == maNhom4) // nếu MA_NHOM==13 (khám bệnh) --> MA_MAY: được để trống
                                            {
                                                lstRowErrors.Remove(error);
                                            }

                                            if (int.TryParse(dr["MA_NHOM"].ToString(), out int maNhom5) && 14 == maNhom5) // nếu MA_NHOM==14 (ngày giường ban ngày) --> MA_MAY: được để trống
                                            {
                                                lstRowErrors.Remove(error);
                                            }

                                            if (int.TryParse(dr["MA_NHOM"].ToString(), out int maNhom6) && 15 == maNhom6) // nếu MA_NHOM==15 (ngày giường nội trú) --> MA_MAY: được để trống
                                            {
                                                lstRowErrors.Remove(error);
                                            }

                                            if (int.TryParse(dr["MA_NHOM"].ToString(), out int maNhom7) && 16 == maNhom7) // nếu MA_NHOM==16 (ngày giường lưu) --> MA_MAY: được để trống
                                            {
                                                lstRowErrors.Remove(error);
                                            }

                                            if (int.TryParse(dr["MA_NHOM"].ToString(), out int maNhom8) && 17 == maNhom8) // nếu MA_NHOM==17 (chế phẩm máu) --> MA_MAY: được để trống
                                            {
                                                lstRowErrors.Remove(error);
                                            }

                                            break;
                                        }


                                    #endregion
                                    default:
                                        break;
                                }
                            }
                        }
                        if (lstRowErrors.Count > 0)
                        {
                            if (dtXml.Columns.Contains("MA_LOI"))
                                dr["MA_LOI"] = string.Join(";", lstRowErrors.Select(x => x.MaLoiCon));
                            if (dtXml.Columns.Contains("THONGTIN_LOI"))
                                dr["THONGTIN_LOI"] = string.Join(";", lstRowErrors.Select(x => x.NoiDungLoi));
                            dictErrors[dtXml.TableName].AddRange(lstRowErrors);
                        }
                        else
                        {
                            if (dtXml.Columns.Contains("MA_LOI"))
                                dr["MA_LOI"] = string.Empty;
                            if (dtXml.Columns.Contains("THONGTIN_LOI"))
                                dr["THONGTIN_LOI"] = string.Empty;
                        }
                    }
                }
            }
            return dictErrors;
        }

        /// <summary>
        /// Cập nhật dữ liệu bảng tương ứng file XML trong DB
        /// </summary>
        /// <param name="dtXmlType">Dữ liệu đầu vào với TableName là XmlType</param>
        /// <param name="callback"></param>
        /// <returns></returns>
        public static bool UpdateXmlType2Db(string xmlTypeName, DataTable dtXmlType, Action<string, bool> callback = null)
        {
            DataTable dtInfo = SQLHelper.GetTableInfo(xmlTypeName);
            if (dtInfo != null && dtInfo.Rows.Count > 0)
            {
                if (dtXmlType.Rows.Count > 0)
                {
                    StringBuilder sbCallback = new StringBuilder();
                    sbCallback.AppendFormat("Cập nhật dữ liệu {0}\n", xmlTypeName);
                    using (SqlConnection conn = new SqlConnection(SQLHelper.Connectionstring))
                    {
                        conn.Open();
                        using (SqlTransaction trans = conn.BeginTransaction())
                        {
                            foreach (DataRow dr in dtXmlType.Rows)
                            {
                                #region XÂY DỰNG QUERY UPDATE
                                string stt = string.Empty;
                                string maLK = dr["MA_LK"].ToString();
                                StringBuilder sbWhere = new StringBuilder();
                                StringBuilder sbUpdate = new StringBuilder();
                                sbUpdate.AppendFormat("UPDATE [{0}] SET [MA_LK]='{1}' ", xmlTypeName, maLK);
                                sbWhere.AppendFormat("WHERE [MA_LK]='{0}' ", maLK);
                                if (dtXmlType.Columns.Contains("STT"))
                                {
                                    stt = dr["STT"].ToString();
                                    sbWhere.AppendFormat("AND [STT]='{0}' ", stt);
                                }
                                sbWhere.Append("AND ( 1=2 ");
                                // Thông điệp trả về
                                sbCallback.AppendFormat("Lượt khám: {1}\n", xmlTypeName, maLK);
                                if (!string.IsNullOrWhiteSpace("STT")) sbCallback.AppendFormat("Số thứ tự: {0}\n", stt);
                                // -----------------
                                #region KIỂM TRA TÍNH HỢP LỆ CỦA DỮ LIỆU
                                foreach (DataRow drInfo in dtInfo.Rows)
                                {
                                    string colName = drInfo["ColumnName"].ToString();
                                    if (colName != "MA_LK" && colName != "STT" && dtXmlType.Columns.Contains(colName))
                                    {
                                        string value = dr[colName].ToString().Replace("'", "''");
                                        bool isNullable = drInfo["AllowDBNull"].ToString().ToUpper() == "TRUE";
                                        if (!string.IsNullOrWhiteSpace(value))
                                        {
                                            sbWhere.AppendFormat("OR [{0}] IS NULL ", colName);
                                            SqlDbType sqlType = (SqlDbType)(int)drInfo["ProviderType"];
                                            switch (sqlType)
                                            {
                                                case SqlDbType.DateTime:
                                                case SqlDbType.DateTime2:
                                                case SqlDbType.SmallDateTime:
                                                case SqlDbType.DateTimeOffset:
                                                    {
                                                        if (DateTime.TryParse(value, out DateTime dtValue))
                                                        {
                                                            value = EasyDateTimeClass.ConvertDateTime(dtValue,
                                                                CultureInfo.CurrentCulture.DateTimeFormat.SortableDateTimePattern);
                                                        }
                                                        sbUpdate.AppendFormat(",[{0}]=CONVERT(DATETIME,'{1}') ", colName, value);
                                                        sbWhere.AppendFormat("OR [{0}]!=CONVERT(DATETIME,'{1}' ", colName, value);
                                                        break;
                                                    }
                                                case SqlDbType.Date:
                                                    {
                                                        if (DateTime.TryParse(value, out DateTime dtValue))
                                                        {
                                                            value = dtValue.ToString("yyyyMMdd");
                                                        }
                                                        sbUpdate.AppendFormat(",[{0}]=CONVERT(DATE,'{1}') ", colName, value);
                                                        sbWhere.AppendFormat("OR FORMAT([{0}],'yyyyMMdd')!='{1}' ", colName, value);
                                                        break;
                                                    }
                                                case SqlDbType.NVarChar:
                                                    sbUpdate.AppendFormat(",[{0}]=N'{1}' ", colName, value);
                                                    sbWhere.AppendFormat("OR [{0}]!=N'{1}' ", colName, value);
                                                    break;
                                                default:
                                                    sbUpdate.AppendFormat(",[{0}]='{1}' ", colName, value);
                                                    sbWhere.AppendFormat("OR [{0}]!='{1}' ", colName, value);
                                                    break;
                                            }
                                        }
                                        else if (isNullable)
                                        {
                                            sbUpdate.AppendFormat(",[{0}]=NULL ", colName);
                                            sbWhere.AppendFormat("OR [{0}] IS NOT NULL ", colName);
                                        }
                                        else
                                        {
                                            sbCallback.AppendFormat("Cột {0} không được để trống!\n", colName);
                                            callback?.Invoke(sbCallback.ToString(), false);
                                            return false;
                                        }
                                    }
                                }
                                #endregion
                                sbWhere.AppendLine(");");
                                sbUpdate.Append(sbWhere);
                                #endregion
                                #region THỰC THI QUERY UPDATE
                                using (SqlCommand command = new SqlCommand(sbUpdate.ToString(), conn, trans))
                                {
                                    try
                                    {
                                        /// Cập nhật các thay đổi vào CSDL
                                        command.ExecuteNonQuery();
                                    }
                                    catch (Exception ex)
                                    {
                                        trans.Rollback();
                                        sbCallback.AppendFormat("Lỗi: {0}", ex.Message);
                                        callback?.Invoke(sbCallback.ToString(), false);
                                        return false;
                                    }
                                }
                                #endregion
                            }
                            trans.Commit();
                            sbCallback.AppendFormat("Cập nhật dữ liệu {0} hoàn tất\n", xmlTypeName);
                            callback?.Invoke(sbCallback.ToString(), true);
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Ghi dữ liệu từ file XML vào bảng tương ứng trong DB
        /// </summary>
        /// <param name="dtXmlType">Dữ liệu đầu vào với TableName là XmlType</param>
        /// <param name="callback"></param>
        /// <returns></returns>
        public static bool ImportXmlType2Db(string xmlTypeName, DataTable dtXmlType, Action<string, bool> callback = null)
        {
            bool isOk = true;
            StringBuilder sbCallback = new StringBuilder();
            sbCallback.AppendFormat("Thêm mới dữ liệu {0}\n", xmlTypeName);
            try
            {
                DataTable dtInfo = SQLHelper.GetTableInfo(xmlTypeName);
                if (dtInfo != null && dtInfo.Rows.Count > 0)
                {
                    foreach (DataRow dr in dtXmlType.Rows)
                    {
                        string maLK = dr["MA_LK"].ToString();
                        string stt = dtXmlType.Columns.Contains("STT") ? dr["STT"].ToString() : string.Empty;
                        sbCallback.AppendFormat("Lượt khám: {0}\n", maLK);
                        if (!string.IsNullOrWhiteSpace(stt))
                        {
                            sbCallback.AppendFormat("Số thứ tự: {0}\n", stt);
                        }
                        #region XÂY DỰNG QUERY INSERT
                        StringBuilder sbValues = new StringBuilder();
                        StringBuilder sbColumns = new StringBuilder();
                        sbColumns.Append("MA_LK");
                        sbValues.AppendFormat("'{0}'", maLK);
                        foreach (DataRow drInfo in dtInfo.Rows)
                        {
                            string colName = drInfo["ColumnName"].ToString();
                            if (colName != "MA_LK" && dtXmlType.Columns.Contains(colName))
                            {
                                string value = dr[colName].ToString().Replace("'", "''");
                                bool isNullable = drInfo["AllowDBNull"].ToString().ToUpper() == "TRUE";
                                if (!string.IsNullOrWhiteSpace(value))
                                {
                                    sbColumns.AppendFormat(",{0}", colName);
                                    SqlDbType sqlType = (SqlDbType)(int)drInfo["ProviderType"];
                                    switch (sqlType)
                                    {
                                        case SqlDbType.DateTime:
                                        case SqlDbType.DateTime2:
                                        case SqlDbType.SmallDateTime:
                                        case SqlDbType.DateTimeOffset:
                                            {
                                                if (DateTime.TryParse(value, out DateTime dtValue))
                                                {
                                                    value = EasyDateTimeClass.ConvertDateTime(dtValue,
                                                        CultureInfo.CurrentCulture.DateTimeFormat.SortableDateTimePattern);
                                                }
                                                sbValues.AppendFormat(",CONVERT(DATETIME,'{0}')", value);
                                                break;
                                            }
                                        case SqlDbType.Date:
                                            {
                                                if (DateTime.TryParse(value, out DateTime dtValue))
                                                {
                                                    value = dtValue.ToString("yyyyMMdd");
                                                }
                                                sbValues.AppendFormat(",CONVERT(DATE,'{0}')", value);
                                                break;
                                            }
                                        case SqlDbType.NVarChar:
                                            sbValues.AppendFormat(",N'{0}'", value);
                                            break;
                                        default:
                                            sbValues.AppendFormat(",'{0}'", value);
                                            break;
                                    }
                                }
                                else if (!isNullable)
                                {
                                    isOk = false;
                                    sbCallback.AppendFormat("Cột {0} không được để trống!", colName);
                                }
                            }
                        }
                        #endregion
                        #region THỰC THI QUERY INSERT
                        if (isOk)
                        {
                            StringBuilder sb = new StringBuilder();
                            sb.AppendFormat("INSERT INTO {0} ({1}) SELECT {2} ", xmlTypeName, sbColumns, sbValues);
                            sb.AppendFormat("WHERE NOT EXISTS ( SELECT 1 FROM {0} WHERE MA_LK='{1}' ", xmlTypeName, maLK);
                            if (!string.IsNullOrWhiteSpace(stt))
                            {
                                sb.AppendFormat("AND STT='{0}' ", stt);
                            }
                            sb.Append(");");
                            int result = SQLHelper.ExecuteNonQuery(sb.ToString());
                            if (result < 1)
                            {
                                isOk = false;
                                sbCallback.AppendLine("Thêm dữ liệu vào CSDL thất bại!");
                                callback?.Invoke(sbCallback.ToString(), false);
                            }
                        }
                        #endregion
                    }
                    sbCallback.AppendLine("Thêm dữ liệu vào CSDL hoàn tất!");
                    callback?.Invoke(sbCallback.ToString(), true);
                }
            }
            catch (Exception ex)
            {
                sbCallback.AppendFormat("Phát sinh lỗi: {0}\n{1}", xmlTypeName, ex.InnerException?.Message ?? ex.Message);
                callback?.Invoke(sbCallback.ToString(), false);
            }
            return isOk;
        }

        /// <summary>
        /// Ghi dữ liệu XML 1, XML 2,... ra file xml và trả về chuỗi base64
        /// </summary>
        /// <param name="dtXmlType">Dữ liệu đầu vào với TableName là XmlType</param>
        /// <param name="xmlTypePath">Đường dẫn file xml được xuất</param>
        /// <param name="writerSettings">Cấu hình XmlWriter</param>
        /// <returns>File XML được mã hóa base64</returns>
        public static string WriteXmlType2Xml(string xmlTypeName, DataTable dtXmlType, string maLK, string xmlTypePath = null, XmlWriterSettings writerSettings = null)
        {
            StringBuilder sbFileHoSoXml = new StringBuilder();
            using (XmlWriter writerXml = XmlWriter.Create(sbFileHoSoXml, writerSettings))
            {
                writerXml.WriteStartDocument();
                switch (xmlTypeName)
                {
                    case "XML1":
                        {
                            WriteXmlRow(writerXml, dtXmlType, "TONG_HOP", maLK);
                            //foreach (DataRow drXmlType in dtXmlType.Rows)
                            //{
                            //    writerXml.WriteStartElement("TONG_HOP");
                            //    foreach (DataColumn col in dtXmlType.Columns)
                            //    {
                            //        string value = drXmlType[col].ToString();
                            //        writerXml.WriteElementString(col.ColumnName, value);
                            //    }
                            //    writerXml.WriteEndElement();
                            //}
                            break;
                        }
                    case "XML2":
                        {
                            writerXml.WriteStartElement("CHITIEU_CHITIET_THUOC");
                            writerXml.WriteStartElement("DSACH_CHI_TIET_THUOC");
                            WriteXmlRow(writerXml, dtXmlType, "CHI_TIET_THUOC", maLK);
                            //foreach (DataRow drXmlType in dtXmlType.Rows)
                            //{
                            //    writerXml.WriteStartElement("CHI_TIET_THUOC");
                            //    foreach (DataColumn col in dtXmlType.Columns)
                            //    {
                            //        string value = drXmlType[col].ToString();
                            //        writerXml.WriteElementString(col.ColumnName, value);
                            //    }
                            //    writerXml.WriteEndElement();
                            //}
                            writerXml.WriteEndElement();
                            writerXml.WriteEndElement();
                            break;
                        }
                    case "XML3":
                        {
                            writerXml.WriteStartElement("CHITIEU_CHITIET_DVKT_VTYT");
                            writerXml.WriteStartElement("DSACH_CHI_TIET_DVKT");
                            WriteXmlRow(writerXml, dtXmlType, "CHI_TIET_DVKT", maLK);
                            //foreach (DataRow drXmlType in dtXmlType.Rows)
                            //{
                            //    writerXml.WriteStartElement("CHI_TIET_DVKT");
                            //    foreach (DataColumn col in dtXmlType.Columns)
                            //    {
                            //        string value = drXmlType[col].ToString();
                            //        writerXml.WriteElementString(col.ColumnName, value);
                            //    }
                            //    writerXml.WriteEndElement();
                            //}
                            writerXml.WriteEndElement();
                            writerXml.WriteEndElement();
                            break;
                        }
                    case "XML4":
                        {
                            writerXml.WriteStartElement("CHITIEU_CHITIET_DICHVUCANLAMSANG");
                            writerXml.WriteStartElement("DSACH_CHI_TIET_CLS");
                            WriteXmlRow(writerXml, dtXmlType, "CHI_TIET_CLS", maLK);
                            //foreach (DataRow drXmlType in dtXmlType.Rows)
                            //{
                            //    writerXml.WriteStartElement("CHI_TIET_CLS");
                            //    foreach (DataColumn col in dtXmlType.Columns)
                            //    {
                            //        string value = drXmlType[col].ToString();
                            //        writerXml.WriteElementString(col.ColumnName, value);
                            //    }
                            //    writerXml.WriteEndElement();
                            //}
                            writerXml.WriteEndElement();
                            writerXml.WriteEndElement();
                            break;
                        }
                    case "XML5":
                        {
                            writerXml.WriteStartElement("CHITIEU_CHITIET_DIENBIENLAMSANG");
                            writerXml.WriteStartElement("DSACH_CHI_TIET_DIEN_BIEN_BENH");
                            WriteXmlRow(writerXml, dtXmlType, "CHI_TIET_DIEN_BIEN_BENH", maLK);
                            //foreach (DataRow drXmlType in dtXmlType.Rows)
                            //{
                            //    writerXml.WriteStartElement("CHI_TIET_DIEN_BIEN_BENH");
                            //    foreach (DataColumn col in dtXmlType.Columns)
                            //    {
                            //        string value = drXmlType[col].ToString();
                            //        writerXml.WriteElementString(col.ColumnName, value);
                            //    }
                            //    writerXml.WriteEndElement();
                            //}
                            writerXml.WriteEndElement();
                            writerXml.WriteEndElement();
                            break;
                        }
                    case "XML6":
                        {
                            writerXml.WriteStartElement("CHI_TIEU_HO_SO_BENH_AN_CHAM_SOC_VA_DIEU_TRI_HIV_AIDS");
                            writerXml.WriteStartElement("DSACH_HO_SO_BENH_AN_CHAM_SOC_VA_DIEU_TRI_HIV_AIDS");
                            WriteXmlRow(writerXml, dtXmlType, "HO_SO_BENH_AN_CHAM_SOC_VA_DIEU_TRI_HIV_AIDS", maLK);
                            //foreach (DataRow drXmlType in dtXmlType.Rows)
                            //{
                            //    writerXml.WriteStartElement("HO_SO_BENH_AN_CHAM_SOC_VA_DIEU_TRI_HIV_AIDS");
                            //    foreach (DataColumn col in dtXmlType.Columns)
                            //    {
                            //        string value = drXmlType[col].ToString();
                            //        writerXml.WriteElementString(col.ColumnName, value);
                            //    }
                            //    writerXml.WriteEndElement();
                            //}
                            writerXml.WriteEndElement();
                            writerXml.WriteEndElement();
                            break;
                        }
                    case "XML7":
                        {
                            WriteXmlRow(writerXml, dtXmlType, "CHI_TIEU_DU_LIEU_GIAY_RA_VIEN", maLK);
                            //foreach (DataRow drXmlType in dtXmlType.Rows)
                            //{
                            //    writerXml.WriteStartElement("CHI_TIEU_DU_LIEU_GIAY_RA_VIEN");
                            //    foreach (DataColumn col in dtXmlType.Columns)
                            //    {
                            //        string value = drXmlType[col].ToString();
                            //        writerXml.WriteElementString(col.ColumnName, value);
                            //    }
                            //    writerXml.WriteEndElement();
                            //}
                            break;
                        }
                    case "XML8":
                        {
                            WriteXmlRow(writerXml, dtXmlType, "CHI_TIEU_DU_LIEU_TOM_TAT_HO_SO_BENH_AN", maLK);
                            //foreach (DataRow drXmlType in dtXmlType.Rows)
                            //{
                            //    writerXml.WriteStartElement("CHI_TIEU_DU_LIEU_TOM_TAT_HO_SO_BENH_AN");
                            //    foreach (DataColumn col in dtXmlType.Columns)
                            //    {
                            //        string value = drXmlType[col].ToString();
                            //        writerXml.WriteElementString(col.ColumnName, value);
                            //    }
                            //    writerXml.WriteEndElement();
                            //}
                            break;
                        }
                    case "XML9":
                        {
                            writerXml.WriteStartElement("CHI_TIEU_DU_LIEU_GIAY_CHUNG_SINH");
                            writerXml.WriteStartElement("DSACH_GIAYCHUNGSINH");
                            WriteXmlRow(writerXml, dtXmlType, "DU_LIEU_GIAY_CHUNG_SINH", maLK);
                            //foreach (DataRow drXmlType in dtXmlType.Rows)
                            //{
                            //    writerXml.WriteStartElement("DU_LIEU_GIAY_CHUNG_SINH");
                            //    foreach (DataColumn col in dtXmlType.Columns)
                            //    {
                            //        string value = drXmlType[col].ToString();
                            //        writerXml.WriteElementString(col.ColumnName, value);
                            //    }
                            //    writerXml.WriteEndElement();
                            //}
                            writerXml.WriteEndElement();
                            writerXml.WriteEndElement();
                            break;
                        }
                    case "XML10":
                        {
                            WriteXmlRow(writerXml, dtXmlType, "CHI_TIEU_DU_LIEU_GIAY_NGHI_DUONG_THAI", maLK);
                            //foreach (DataRow drXmlType in dtXmlType.Rows)
                            //{
                            //    writerXml.WriteStartElement("CHI_TIEU_DU_LIEU_GIAY_NGHI_DUONG_THAI");
                            //    foreach (DataColumn col in dtXmlType.Columns)
                            //    {
                            //        string value = drXmlType[col].ToString();
                            //        writerXml.WriteElementString(col.ColumnName, value);
                            //    }
                            //    writerXml.WriteEndElement();
                            //}
                            break;
                        }
                    case "XML11":
                        {
                            WriteXmlRow(writerXml, dtXmlType, "CHI_TIEU_DU_LIEU_GIAY_CHUNG_NHAN_NGHI_VIEC_HUONG_BAO_HIEM_XA_HOI", maLK);
                            //foreach (DataRow drXmlType in dtXmlType.Rows)
                            //{
                            //    writerXml.WriteStartElement("CHI_TIEU_DU_LIEU_GIAY_CHUNG_NHAN_NGHI_VIEC_HUONG_BAO_HIEM_XA_HOI");
                            //    foreach (DataColumn col in dtXmlType.Columns)
                            //    {
                            //        string value = drXmlType[col].ToString();
                            //        writerXml.WriteElementString(col.ColumnName, value);
                            //    }
                            //    writerXml.WriteEndElement();
                            //}
                            break;
                        }
                    default:
                        break;
                }
                writerXml.WriteEndDocument();
                writerXml.Flush();
            }
            string xmlContent = sbFileHoSoXml.ToString().Replace("encoding=\"utf-16\"", "encoding=\"utf-8\"");
            if (!string.IsNullOrWhiteSpace(xmlTypePath))
            {
                File.WriteAllText(xmlTypePath, xmlContent);
            }
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(xmlContent));
        }

        private static void WriteXmlRow(XmlWriter writerXml, DataTable dtXmlType, string rowName, string maLK)
        {
            foreach (DataRow drXmlType in dtXmlType.Rows)
            {
                if (maLK == drXmlType["MA_LK"].ToString())
                {
                    writerXml.WriteStartElement(rowName);
                    foreach (DataColumn col in dtXmlType.Columns)
                    {
                        if (!col.ColumnName.EndsWith("_Id") && col.ColumnName != "MA_LOI" && col.ColumnName != "THONGTIN_LOI") // bỏ _Id
                        {
                            string value = drXmlType[col].ToString();
                            writerXml.WriteElementString(col.ColumnName, value);
                        }
                    }
                    writerXml.WriteEndElement();
                }
            }
        }
    }

    public class ClsXmlError
    {
        public string XmlType { get; set; }
        public string Item { get; set; }
        public string MaLoiCha { get; set; }
        public string MaLoiCon { get; set; }
        public string NoiDungLoi { get; set; }
    }
}
