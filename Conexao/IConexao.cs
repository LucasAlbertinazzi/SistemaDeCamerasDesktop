using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistCamerasGuarita.Conexao
{
    public interface IConexao
    {
        /// <summary>
        /// String de conexao com a base de dados; deve ser instanciada pelo metodo construtor
        /// </summary>
        string StringConexao { get; }

        /// <summary>
        /// Indica se o proximo comando a executar deve utilizar os parametros que foram armazenados, valor padrao é false
        /// </summary>
        bool IgnorarParametrosNoProximoComando { get; set; }

        /// <summary>
        /// Quantidade de parametros ativos
        /// </summary>
        int QuantidadeParametros { get; }

        /// <summary>
        /// Registro do ultimo comando executado
        /// </summary>
        ComandoConexao UltimoComandoExecutado { get; }

        /// <summary>
        /// Executa um comando SQL
        /// </summary>
        /// <param name="sqlComando">script SQL</param>
        /// <returns>Quantidade de linhas afetadas</returns>
        int ExecutaComando(string sqlComando);

        /// <summary>
        /// Executa um comando SQL, selecionando apenas a primeira linha
        /// </summary>
        /// <param name="slqComando">script SQL</param>
        /// <returns>NpgsqlDataReader</returns>
        NpgsqlDataReader SelecionaDataReader(string slqComando);

        /// <summary>
        /// Executa um comando SQL
        /// </summary>
        /// <param name="sqlComando">script SQL</param>
        /// <returns>DataTable, se nao enconstrar resultados, retorna um DataTable vazio</returns>
        DataTable SelecionaDataTable(string sqlComando);

        /// <summary>
        /// Executa um comando SQL
        /// </summary>
        /// <param name="sqlComando">script SQL</param>
        /// <returns>DataSet, se nao enconstrar resultados, retorna um DataSet vazio</returns>
        DataSet SelecionaDataSet(string sqlComando);

        /// <summary>
        /// Executa um comando SQL
        /// </summary>
        /// <typeparam name="Type">tipo do objeto de retorno</typeparam>
        /// <param name="sqlComando">script SQL</param>
        /// <returns>
        /// Retorna o primeiro valor selecionado se ele for do mesmo tipo 'Type' 
        /// ou ele for conversível para o mesmo, caso contrario retorna o valor default do tipo de 'Type'
        /// </returns>
        Type SelecionaPrimeiroValor<Type>(string sqlComando);

        /// <summary>
        /// Adiciona um objeto a lista de parametros do tipo 'NpgsqlParameter' para o proximo comando que sera executado
        /// </summary>
        /// <param name="obj">
        /// Pode ser um objeto qualquer.
        /// caso o parametro seja do tipo 'NpgsqlParameter', 
        /// ele será adicionado com as MESMAS propriedades ao comando, apenas será alterado a propriedade 'ParameterName'
        /// </param>
        /// <returns>nome do parametro que deve ser acrescentado ao script SQL na ordem correta de execucao</returns>
        string AdicionaParametro(object obj);
    }

    public class ComandoConexao
    {
        public string ComandoTexto { get; private set; }
        public IEnumerable<NpgsqlParameter> Parametros { get; private set; }
        public ComandoConexao(string comandoTexto, IEnumerable<NpgsqlParameter> parametros = null)
        {
            ComandoTexto = comandoTexto;
            Parametros = parametros ?? new List<NpgsqlParameter>();
        }
    }
}
