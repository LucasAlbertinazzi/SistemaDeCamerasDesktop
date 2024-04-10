using SistCamerasGuarita.Classes;
using SistCamerasGuarita.Conexao;
using System;
using System.Collections.Generic;
using System.Data;

namespace SistCamerasGuarita.Services
{
    public class ServiceLayout
    {
        DAL obj = new DAL();
        public List<CamerasLayout> CarregaLayout()
        {
            string sql = $"SELECT * FROM tbl_cameras_cd_layout";

            if (InfoGlobal.CasaCam)
            {
                sql = $"SELECT * FROM tbl_cameras_cd_layout AND casacam = 'true'";
            }

            DataTable dt = obj.RetornaTabela(sql);

            List<CamerasLayout> listLayout = new List<CamerasLayout>();

            if (dt != null && dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    listLayout.Add(new CamerasLayout
                    {
                        IdLayout = Convert.ToInt32(dt.Rows[i]["idlayout"]),
                        Linhas = Convert.ToInt32(dt.Rows[i]["linhas"]),
                        Colunas = Convert.ToInt32(dt.Rows[i]["colunas"]),
                        Monitor = Convert.ToInt32(dt.Rows[i]["monitor"]),
                        UserCadastra = Convert.ToInt32(dt.Rows[i]["usercadastra"]),
                        UserAtualiza = Convert.ToInt32(dt.Rows[i]["useratualiza"]),
                        DataCadastra = Convert.ToDateTime(dt.Rows[i]["datacadastra"]),
                        DataAtualiza = Convert.ToDateTime(dt.Rows[i]["dataatualiza"])
                    });
                }
            }

            return listLayout;
        }
    }
}
