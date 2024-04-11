using Npgsql;
using System.Configuration;
using System.Data;

namespace SistCamerasGuarita.Conexao
{
    public static class ConexaoConfig
    {
        public static IConexao Conexao;
        public static NpgsqlConnection conn = new NpgsqlConnection();

        public static string ObterStringConexao(string nomeConexao)
        {
            string conexaoBanco = ConfigurationManager.ConnectionStrings[nomeConexao].ConnectionString;
            return conexaoBanco;
        }


        public static bool TestarConexao(string nomeConexao)
        {
            try
            {
                string stringConexao = ObterStringConexao(nomeConexao);

                using (var conexao = new NpgsqlConnection(stringConexao))
                {
                    conexao.Open();
                    return conexao.State == ConnectionState.Open;
                }
            }
            catch
            {
                return false;
            }
        }

        public static void AbrirConexao(string nomeConexao)
        {
            string stringConexao = ObterStringConexao(nomeConexao);

            NpgsqlConnection conexao = new NpgsqlConnection(stringConexao);
            ConexaoConfig.conn = conexao;
            Conexao = new ConexaoModel(conn.ConnectionString);
        }

        public static void FecharConexao(NpgsqlConnection conexao)
        {
            if (conexao != null && conexao.State != ConnectionState.Closed)
            {
                conexao.Close();
                ConexaoConfig.conn = null;
            }
        }
    }
}
