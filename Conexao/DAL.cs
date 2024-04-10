using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SistCamerasGuarita.Conexao
{
    public class DAL
    {
        private IConexao _conexao;
        private IConexao _conn
        {
            get
            {
                if (_conexao == null)
                {
                    if (!string.IsNullOrEmpty(ConexaoConfig.conn?.ConnectionString))
                    {
                        _conexao = new ConexaoModel(ConexaoConfig.conn.ConnectionString);
                    }
                }
                return _conexao;
            }
        }

        #region 2 - MÉTODOS
        public DataTable RetornaTabela(string sqlx)
        {
            return ExecutaRetornoTabela(sqlx);
        }

        public int ComandoSql(string sql)
        {
            return ExecutaComandoSql(sql);
        }

        public string ComandoRetornaValor(string sql)
        {
            string valor = RetornaValor(sql, "0", true);

            return valor;
        }

        #endregion

        #region 3 - SUB MÉTODOS
        private DataTable ExecutaRetornoTabela(string sqlx, bool comandoComParametros = false)
        {
            Cursor.Current = Cursors.WaitCursor;
            DataTable dttabela;
            try
            {
                _conn.IgnorarParametrosNoProximoComando = !comandoComParametros;
                dttabela = _conn.SelecionaDataTable(sqlx);
            }
            catch
            {
                dttabela = new DataTable();
            }
            Cursor.Current = Cursors.Default;
            return dttabela;
        }

        private int ExecutaComandoSql(string sql, bool comandoComParametros = false)
        {
            Cursor.Current = Cursors.WaitCursor;
            int linhasAfetadas = 0;
            try
            {
                _conn.IgnorarParametrosNoProximoComando = !comandoComParametros;
                linhasAfetadas = _conn.ExecutaComando(sql);

            }
            catch
            {
                return 0;
            }

            Cursor.Current = Cursors.Default;
            return linhasAfetadas;
        }

        private T RetornaValor<T>(string sql, T valorDefault = default, bool comandoComParametros = false)
        {
            Cursor.Current = Cursors.WaitCursor;
            T retorno = valorDefault;
            try
            {
                _conn.IgnorarParametrosNoProximoComando = !comandoComParametros;
                retorno = _conn.SelecionaPrimeiroValor<T>(sql);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erro ao executar SQL: " + ex.Message);
                return retorno;
            }
            Cursor.Current = Cursors.Default;
            return retorno;
        }
        #endregion
    }

}
