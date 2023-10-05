using Mensagem.Domain.Interfaces;
using Mensagem.Infra.Data.Filas;
using Microsoft.Extensions.Logging;
using Xunit;

namespace Mensagem.Service.Services
{
    public class ProcessaFilaService : IProcessaFilaService
    {
        private readonly ILogger<ProcessaFilaService> _logger;
        private readonly RabbitMqHelper _obj = new RabbitMqHelper();

        public ProcessaFilaService(ILogger<ProcessaFilaService> logger
            )
        {
            _logger = logger;
        }

        public Task<List<string>> ListarMensagens()
        {
            // Arrange
            var factory = _obj.GetConnectionFactory();
            var connection = _obj.CreateConnection(factory);
            var queue = _obj.CreateQueue("QueueName_Message_UnitTest", connection);

            // Act
            List<string> retorno = new List<string>();
            retorno = _obj.RetrieveMessageList("QueueName_Message_UnitTest", connection);

            // Assert
            Assert.True(retorno != null);

            connection.Close();
            connection.Dispose();

            return Task.FromResult(retorno);
        }

        public async Task ProcessarMensagensAsync()
        {
            _logger.LogInformation("Contas - Iniciando processamento das mensagens de Contas");
            try
            {
                bool temMensagens = true;

                while (temMensagens)
                {
                    var listaMensagem = await ListarMensagens();
                    if (!listaMensagem.Any())
                    {
                        temMensagens = false;
                    }
                    else
                    {
                        foreach (var item in listaMensagem)
                        {
                            //Persiste a mensagem na base de dados
                        }
                    }
                }

                _logger.LogInformation("Contas - Concluido processamento das mensagens de Conta");
            }
            catch (Exception ex)
            {
                _logger.LogError("Contas - Erro - {a}. Stack - {b}", ex.Message, ex.StackTrace);
            }
        }
    }
}