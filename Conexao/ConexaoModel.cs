using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistCamerasGuarita.Conexao
{
    internal class ConexaoModel : IConexao
    {
        #region propriedades
        private readonly List<NpgsqlParameter> parametros;
        public string StringConexao { get; private set; }
        public ComandoConexao UltimoComandoExecutado { get; private set; }
        public int QuantidadeParametros => parametros.Count;
        public bool IgnorarParametrosNoProximoComando { get; set; }
        #endregion propriedades

        #region construtor
        public ConexaoModel(string connectionString)
        {
            StringConexao = connectionString;
            parametros = new List<NpgsqlParameter>();
        }
        #endregion construtor

        #region metodos publicos
        public int ExecutaComando(string sqlComando)
        {
            int linhasAfetadas = 0;
            try
            {
                using (NpgsqlConnection con = NovaConexao())
                {
                    using (NpgsqlCommand comando = new NpgsqlCommand(sqlComando, con))
                    {
                        AuxiliarCriaComando(comando);
                        linhasAfetadas = comando.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }

            return linhasAfetadas;
        }

        public NpgsqlDataReader SelecionaDataReader(string sqlComando)
        {
            try
            {
                using (NpgsqlConnection con = NovaConexao())
                {
                    using (NpgsqlCommand comando = new NpgsqlCommand(sqlComando, con))
                    {
                        AuxiliarCriaComando(comando);
                        var dataReader = comando.ExecuteReader();
                        dataReader.Read();
                        return dataReader;
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public DataTable SelecionaDataTable(string sqlComando)
        {
            try
            {
                using (NpgsqlConnection con = NovaConexao())
                {
                    using (NpgsqlCommand comando = new NpgsqlCommand(sqlComando, con))
                    {
                        AuxiliarCriaComando(comando);
                        DataTable dt = new DataTable();
                        using (NpgsqlDataAdapter dataAdapter = new NpgsqlDataAdapter(comando))
                        {
                            dataAdapter.Fill(dt);
                        }

                        return dt;
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public DataSet SelecionaDataSet(string sqlComando)
        {
            try
            {
                DataSet ds = new DataSet();
                using (NpgsqlConnection con = NovaConexao())
                {
                    using (NpgsqlCommand comando = new NpgsqlCommand(sqlComando, con))
                    {
                        AuxiliarCriaComando(comando);
                        using (NpgsqlDataAdapter dataAdapter = new NpgsqlDataAdapter(comando))
                        {
                            dataAdapter.Fill(ds, "Tabela");
                            return ds;
                        }
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public Type SelecionaPrimeiroValor<Type>(string sqlComando)
        {
            Type valorPadrao = default(Type);

            try
            {
                using (NpgsqlConnection con = NovaConexao())
                {
                    using (NpgsqlCommand comando = new NpgsqlCommand(sqlComando, con))
                    {
                        AuxiliarCriaComando(comando);
                        object result = comando.ExecuteScalar();

                        if (result != null && result != DBNull.Value)
                        {
                            return (Type)Convert.ChangeType(result, typeof(Type), CultureInfo.InvariantCulture);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erro ao executar SQL: " + ex.Message);
            }

            return valorPadrao;
        }


        public string AdicionaParametro(object obj)
        {
            string key = $":{parametros.Count + 1}";
            NpgsqlParameter npgsqlParameter;
            if (obj is NpgsqlParameter param)
            {
                npgsqlParameter = param;
                npgsqlParameter.ParameterName = key;
            }
            else
            {
                npgsqlParameter = new NpgsqlParameter(key, obj);
            }
            parametros.Add(npgsqlParameter);
            return key;
        }

        #endregion metodos publicos

        #region metodos auxiliares
        /// <summary>
        /// Instancia um novo objeto do tipo NpgsqlConnection com a string de conexão passada via construtor;
        /// </summary>
        /// <returns>NpgsqlConnection com estado aberto</returns>
        private NpgsqlConnection NovaConexao()
        {
            NpgsqlConnection conexao = new NpgsqlConnection(StringConexao);
            conexao.Open();
            return conexao;
        }

        /// <summary>
        /// Registra o ultimo comando e limpa os parametros se existir
        /// </summary>
        /// <param name="command"></param>
        private void AuxiliarCriaComando(NpgsqlCommand command)
        {
            if (!IgnorarParametrosNoProximoComando)
            {
                parametros.ForEach(param => command.Parameters.Add(param));
                UltimoComandoExecutado = new ComandoConexao(command.CommandText, new List<NpgsqlParameter>(parametros));
                parametros.Clear();
            }
            else
            {
                UltimoComandoExecutado = new ComandoConexao(command.CommandText);
            }

            IgnorarParametrosNoProximoComando = false;
        }
        #endregion metodos auxiliares
    }
}
