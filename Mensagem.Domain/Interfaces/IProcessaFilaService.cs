namespace Mensagem.Domain.Interfaces
{
    public interface IProcessaFilaService
    {
        /// <summary>
        /// Rotina responsável pela execução da leitura das filas de mensageria
        /// </summary>
        /// <returns></returns>
        Task ProcessarMensagensAsync();
    }
}