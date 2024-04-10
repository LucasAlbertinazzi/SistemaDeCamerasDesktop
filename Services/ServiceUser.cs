using SistCamerasGuarita.Classes;
using SistCamerasGuarita.Conexao;
using SistCamerasGuarita.Suporte.Criptografia;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistCamerasGuarita.Services
{
    public class ServiceUser
    {
        DAL obj = new DAL();

        public bool AutenticaUsuario(string usuario, string senha)
        {
            string sql = $"SELECT * FROM tbl_usuario WHERE login = '{usuario}'";
            DataTable dt = obj.RetornaTabela(sql);

            if(dt != null && dt.Rows.Count > 0)
            {
                string passCripto = CriptoPass.Descriptografar(dt.Rows[0]["email"].ToString());

                if(passCripto == senha)
                {
                    InfoGlobal.Usuario = dt.Rows[0]["usuario"].ToString();
                    InfoGlobal.IdUser = Convert.ToInt32(dt.Rows[0]["codusuario"]);
                    InfoGlobal.Autenticacao = true;
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
    }
}
