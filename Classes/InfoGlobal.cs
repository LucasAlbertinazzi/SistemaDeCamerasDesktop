using System;
using System.IO;

namespace SistCamerasGuarita.Classes
{
    public static class InfoGlobal
    {
        public static int IdUser { get; set; }
        public static string Usuario { get; set; }
        public static bool CasaCam { get; set; }
        public static bool Admin { get; set; }
        public static int CodDep { get; set; }
        public static bool Autenticacao { get; set; }
        public static bool AlteracaoCam { get; set; } = false;
        public static string Ambiente { get; set; }

        // Variáveis de caminho relativo
        public static string arquivoOrigem = "SistCamerasGuarita.exe.Config";
        public static string arquivoDestino = "App.config";

        // Métodos para obter os caminhos completos
        public static string ObterCaminhoOrigem()
        {
            return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, arquivoOrigem);
        }

        public static string ObterCaminhoDestino()
        {
            return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..\\..", arquivoDestino);
        }
    }
}
