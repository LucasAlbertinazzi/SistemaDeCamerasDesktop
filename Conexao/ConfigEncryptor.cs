using SistCamerasGuarita.Classes;
using System;
using System.Configuration;
using System.IO;

namespace SistCamerasGuarita.Conexao
{
    public static class ConfigEncryptor
    {
        public static void Criptografar()
        {
            var confgFile = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            ConnectionStringsSection section = confgFile.GetSection("connectionStrings") as ConnectionStringsSection;
            section.SectionInformation.ProtectSection("DataProtectionConfigurationProvider");
            confgFile.Save();
            UpdateArquivo();
        }

        public static void Descriptografar()
        {
            var confgFile = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            ConnectionStringsSection section = confgFile.GetSection("connectionStrings") as ConnectionStringsSection;
            section.SectionInformation.UnprotectSection();
            confgFile.Save();
            UpdateArquivo();
        }

        public static void UpdateArquivo()
        {
            try
            {
                string origemFull = InfoGlobal.ObterCaminhoOrigem();
                string destinoFull = InfoGlobal.ObterCaminhoDestino();

                if (File.Exists(destinoFull))
                {
                    File.Delete(destinoFull);
                }

                File.Copy(origemFull, destinoFull);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao atualizar arquivo: {ex.Message}");
            }
        }
    }
}
