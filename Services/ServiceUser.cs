using SistCamerasGuarita.Classes;
using SistCamerasGuarita.Conexao;
using SistCamerasGuarita.Suporte.Criptografia;
using System;
using System.Data;

namespace SistCamerasGuarita.Services
{
    public class ServiceUser
    {

        #region 1 - VARIÁVEIS
        DAL obj = new DAL();
        #endregion

        #region 2 - MÉTODOS

        public bool AutenticaUsuario(string usuario, string senha)
        {
            string sql = $"SELECT * FROM tbl_usuario WHERE login = '{usuario}'";
            DataTable dt = obj.RetornaTabela(sql);

            if (dt != null && dt.Rows.Count > 0)
            {
                string passCripto = CriptoPass.Descriptografar(dt.Rows[0]["email"].ToString());

                if (passCripto == senha)
                {
                    InfoGlobal.Usuario = dt.Rows[0]["usuario"].ToString();
                    InfoGlobal.IdUser = Convert.ToInt32(dt.Rows[0]["codusuario"]);
                    InfoGlobal.Autenticacao = true;
                    InfoGlobal.CodDep = Convert.ToInt32(dt.Rows[0]["coddep"]); ;

                    return true;
                }

                return false;
            }

            return true;
        }

        public bool DeslogarUsuario()
        {
            return true;
        }

        #endregion
    }
}
