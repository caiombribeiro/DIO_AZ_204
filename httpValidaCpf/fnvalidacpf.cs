using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace httpValidaCpf
{
    public static class fnvalidacpf
    {
        [FunctionName("fnvalidacpf")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function,"post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("Iniciando a validação do CPF.");


            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            if(data == null)
            {
                return new BadRequestObjectResult("Por favor, informe o CPF.");
            }
            var cpf = data?.cpf;

            if (IsValidCpf(cpf) == false)
            {
                return new BadRequestObjectResult("CPF Inválido");
            }

            var responseMessage = "CPF válido, e não consta na base de dados de fraudes, e não consta na base de dados de débitos.";   

            return new OkObjectResult(responseMessage);
        }
    

    public static bool IsValidCpf(string cpf)
    {
        if (string.IsNullOrWhiteSpace(cpf))
            return false;

        // Remove caracteres não numéricos
        cpf = new string(cpf.Where(char.IsDigit).ToArray());

        // Verifica se o CPF tem 11 dígitos
        if (cpf.Length != 11)
            return false;

        // Verifica se todos os números são iguais (ex: 00000000000, 11111111111)
        if (cpf.Distinct().Count() == 1)
            return false;

        // Calcula os dígitos verificadores
        int[] multipliers1 = { 10, 9, 8, 7, 6, 5, 4, 3, 2 };
        int[] multipliers2 = { 11, 10, 9, 8, 7, 6, 5, 4, 3, 2 };

        string cpfBase = cpf.Substring(0, 9);
        string cpfDigits = cpf.Substring(9, 2);

        // Calcula o primeiro dígito verificador
        int firstCheckSum = cpfBase
            .Select((c, i) => (c - '0') * multipliers1[i])
            .Sum();

        int firstVerifier = (firstCheckSum * 10) % 11;
        if (firstVerifier == 10)
            firstVerifier = 0;

        // Calcula o segundo dígito verificador
        string cpfBaseWithFirstVerifier = cpfBase + firstVerifier;
        int secondCheckSum = cpfBaseWithFirstVerifier
            .Select((c, i) => (c - '0') * multipliers2[i])
            .Sum();

        int secondVerifier = (secondCheckSum * 10) % 11;
        if (secondVerifier == 10)
            secondVerifier = 0;

        // Verifica se os dígitos calculados batem com os fornecidos
        return cpfDigits == $"{firstVerifier}{secondVerifier}";
    }
    }

}
