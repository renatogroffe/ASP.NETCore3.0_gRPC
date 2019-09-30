using System.Text;
using System.Threading.Tasks;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using gRPCServiceProdutos.Business;
using gRPCServiceProdutos.Data;

namespace gRPCServiceProdutos
{
    public class ProdutoSvcgRPC : ProdutoSvc.ProdutoSvcBase
    {
        private readonly ILogger<ProdutoSvcgRPC> _logger;
        private readonly CatalogoDbContext _catalogoContext;
        private readonly ProdutoBO _produtoBO;

        public ProdutoSvcgRPC(ILogger<ProdutoSvcgRPC> logger,
            CatalogoDbContext catalogoContext,
            ProdutoBO produtoBO)
        {
            _logger = logger;
            _catalogoContext = catalogoContext;
            _produtoBO = produtoBO;
        }

        public override async Task Listar(
            ListarProdutosRequest request,
            IServerStreamWriter<ListarProdutosReply> responseStream,
            ServerCallContext context)
        {
            _logger.LogInformation("Listando Produtos...");
            var dadosProdutos = _produtoBO.ListarTodos();            

            foreach (var produto in dadosProdutos)
            {
                await responseStream.WriteAsync(
                    new ListarProdutosReply { Produto = produto });
            }
        }

        public override Task<ProdutoReply> Incluir(
            DadosProduto dadosProduto, ServerCallContext context)
        {
            _logger.LogInformation("Inclusão de Produto...");
            
            bool sucesso = _produtoBO.Incluir(dadosProduto,
                out StringBuilder inconsistencias);            
            return Task.FromResult(new ProdutoReply
            {
                Sucesso = sucesso,
                Mensagem = sucesso ? "Inclusão realizada com sucesso" : "Dados inconsistentes",
                Inconsistencias = inconsistencias.ToString()
            });
        }

        public override Task<ProdutoReply> Alterar(
            DadosProduto dadosProduto, ServerCallContext context)
        {
            _logger.LogInformation("Alteração de Produto...");
            bool sucesso = _produtoBO.Alterar(dadosProduto,
                out StringBuilder inconsistencias);
            
            return Task.FromResult(new ProdutoReply
            {
                Sucesso = sucesso,
                Mensagem = sucesso ? "Alteração realizada com sucesso" : "Dados inconsistentes",
                Inconsistencias = inconsistencias.ToString()
            });
        }
    }
}