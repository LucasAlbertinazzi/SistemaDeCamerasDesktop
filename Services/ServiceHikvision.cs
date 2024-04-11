using SistCamerasGuarita.Classes;
using SistCamerasGuarita.Classes.Hikvision;
using SistCamerasGuarita.Conexao;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Runtime.InteropServices;

namespace SistCamerasGuarita.Services
{
    public class ServiceHikvision
    {
        #region 1 - VARIÁVEIS
        DAL obj = new DAL();

        private bool m_bInitSDK = false;
        private Int32 m_lUserID = -1;
        private Int32 m_lRealHandle = -1;

        CHCNetSDK.REALDATACALLBACK RealData = null;
        #endregion

        #region 2 - MÉTODOS
        public bool HikvisionAutenticacao(string ip, Int32 porta, string user, string senha)
        {
            m_bInitSDK = CHCNetSDK.NET_DVR_Init();
            CHCNetSDK.NET_DVR_DEVICEINFO_V30 DeviceInfo = new CHCNetSDK.NET_DVR_DEVICEINFO_V30();
            m_lUserID = CHCNetSDK.NET_DVR_Login_V30(ip, porta, user, senha, ref DeviceInfo);

            if (m_lUserID < 0)
            {
                return false;
            }

            return true;
        }

        public void HikvisionRealDataCallBack(Int32 lRealHandle, UInt32 dwDataType, IntPtr pBuffer, UInt32 dwBufSize, IntPtr pUser)
        {
            if (dwBufSize > 0)
            {
                byte[] sData = new byte[dwBufSize];
                Marshal.Copy(pBuffer, sData, 0, (Int32)dwBufSize);

                string str = "ÊµÊ±Á÷Êý¾Ý.ps";
                FileStream fs = new FileStream(str, FileMode.Create);
                int iLen = (int)dwBufSize;
                fs.Write(sData, 0, iLen);
                fs.Close();
            }
        }

        public List<CamerasInfos> HikvisionCameras()
        {
            DataTable dt = HikvisionCarregaCameras();

            if (dt.Rows.Count > 0)
            {
                List<CamerasInfos> cameras = new List<CamerasInfos>();

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    HikvisionAutenticacao(dt.Rows[i]["ip"].ToString(), Convert.ToInt32(dt.Rows[i]["porta"]), dt.Rows[i]["usercamera"].ToString(), dt.Rows[i]["senhacamera"].ToString());

                    CHCNetSDK.NET_DVR_PREVIEWINFO lpPreviewInfo = new CHCNetSDK.NET_DVR_PREVIEWINFO();
                    IntPtr pUser = new IntPtr();

                    string enderecoMemoria = dt.Rows[i]["componente"].ToString();
                    IntPtr componente = new IntPtr(Convert.ToInt32(enderecoMemoria));

                    lpPreviewInfo.hPlayWnd = componente;
                    lpPreviewInfo.lChannel = 1;
                    lpPreviewInfo.dwStreamType = 1;
                    lpPreviewInfo.dwLinkMode = 0;
                    lpPreviewInfo.bBlocked = true;
                    lpPreviewInfo.dwDisplayBufNum = 1;
                    lpPreviewInfo.byProtoType = 0;
                    lpPreviewInfo.byPreviewMode = 0;

                    RealData = new CHCNetSDK.REALDATACALLBACK(HikvisionRealDataCallBack);
                    m_lRealHandle = CHCNetSDK.NET_DVR_RealPlay_V40(m_lUserID, ref lpPreviewInfo, null, pUser);

                    cameras.Add(new CamerasInfos
                    {
                        Id = Convert.ToInt32(dt.Rows[i]["id"]),
                        IdMarca = Convert.ToInt32(dt.Rows[i]["idmarca"]),
                        Posicao = Convert.ToInt32(dt.Rows[i]["posicao"]),
                        IdLayout = Convert.ToInt32(dt.Rows[i]["idlayout"]),
                        Monitor = Convert.ToInt32(dt.Rows[i]["monitor"]),
                        Componente = componente,
                        Camera = dt.Rows[i]["camera"].ToString(),
                        Ip = dt.Rows[i]["ip"].ToString(),
                        Porta = Convert.ToInt32(dt.Rows[i]["porta"]),
                        UserCamera = dt.Rows[i]["usercamera"].ToString(),
                        SenhaCamera = dt.Rows[i]["senhacamera"].ToString(),
                        Ativo = Convert.ToBoolean(dt.Rows[i]["ativo"]),
                        UserCadastra = Convert.ToInt32(dt.Rows[i]["usercadastra"]),
                        UserExclui = Convert.ToInt32(dt.Rows[i]["userexclui"]),
                        UserAtualiza = Convert.ToInt32(dt.Rows[i]["useratualiza"]),
                        DataCadastra = dt.Rows[i]["datacadastra"] != DBNull.Value ? Convert.ToDateTime(dt.Rows[i]["datacadastra"]) : DateTime.MinValue,
                        DataExclui = dt.Rows[i]["dataexclui"] != DBNull.Value ? Convert.ToDateTime(dt.Rows[i]["dataexclui"]) : DateTime.MinValue,
                        DataAtualiza = dt.Rows[i]["dataatualiza"] != DBNull.Value ? Convert.ToDateTime(dt.Rows[i]["dataatualiza"]) : DateTime.MinValue
                    });
                }

                return cameras;
            }

            return new List<CamerasInfos>();
        }

        public DataTable HikvisionCarregaCameras()
        {
            string sql = $"SELECT * FROM tbl_cameras_cd " +
                         $"WHERE ativo = 'true' " +
                         $"AND userexclui = 0 " +
                         $"AND idmarca = 1 " +
                         $"AND posicao > 0 " +
                         $"AND casacam = '{InfoGlobal.CasaCam}'";

            DataTable dt = obj.RetornaTabela(sql);

            if (dt.Rows.Count > 0)
            {
                return dt;
            }

            return new DataTable();
        }

        public void PreviewCameras(CamerasInfos camerasInfos)
        {
            HikvisionAutenticacao(camerasInfos.Ip, camerasInfos.Porta, camerasInfos.UserCamera, camerasInfos.SenhaCamera);
            CHCNetSDK.NET_DVR_PREVIEWINFO lpPreviewInfo = new CHCNetSDK.NET_DVR_PREVIEWINFO();
            IntPtr pUser = new IntPtr();

            lpPreviewInfo.hPlayWnd = camerasInfos.Componente;
            lpPreviewInfo.lChannel = 1;
            lpPreviewInfo.dwStreamType = 1;
            lpPreviewInfo.dwLinkMode = 0;
            lpPreviewInfo.bBlocked = true;
            lpPreviewInfo.dwDisplayBufNum = 1;
            lpPreviewInfo.byProtoType = 0;
            lpPreviewInfo.byPreviewMode = 0;

            RealData = new CHCNetSDK.REALDATACALLBACK(HikvisionRealDataCallBack);
            m_lRealHandle = CHCNetSDK.NET_DVR_RealPlay_V40(m_lUserID, ref lpPreviewInfo, null, pUser);
        }
        #endregion
    }
}
