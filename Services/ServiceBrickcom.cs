using SistCamerasGuarita.Classes;
using SistCamerasGuarita.Conexao;
using System;
using System.Collections.Generic;
using System.Data;

namespace SistCamerasGuarita.Services
{
    public class ServiceBrickcom
    {
        DAL obj = new DAL();
        private DataTable BrickcomCarregaCameras()
        {
            string sql = "SELECT * FROM tbl_cameras_cd WHERE ativo = 'true' AND userexclui = 0 AND idmarca = 2 AND posicao > 0";

            if (InfoGlobal.CasaCam)
            {
                sql = "SELECT * FROM tbl_cameras_cd WHERE ativo = 'true' AND userexclui = 0 AND idmarca = 2 AND posicao > 0 AND casacam = 'true'";
            }

            DataTable dt = obj.RetornaTabela(sql);

            if (dt.Rows.Count > 0)
            {
                return dt;
            }

            return new DataTable();
        }

        public List<CamerasInfos> BrickcomCameras()
        {
            DataTable dt = BrickcomCarregaCameras();

            if (dt.Rows.Count > 0)
            {
                List<CamerasInfos> cameras = new List<CamerasInfos>();

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    cameras.Add(new CamerasInfos
                    {
                        Id = Convert.ToInt32(dt.Rows[i]["id"]),
                        IdMarca = Convert.ToInt32(dt.Rows[i]["idmarca"]),
                        Posicao = Convert.ToInt32(dt.Rows[i]["posicao"]),
                        IdLayout = Convert.ToInt32(dt.Rows[i]["idlayout"]),
                        Monitor = Convert.ToInt32(dt.Rows[i]["monitor"]),
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
    }
}
