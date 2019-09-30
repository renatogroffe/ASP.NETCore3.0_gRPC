using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using gRPCServiceProdutos.Data;
using gRPCServiceProdutos.Models;

namespace gRPCServiceProdutos.Business
{
    public class ProdutoBO
    {
        private CatalogoDbContext _context;

        public ProdutoBO(CatalogoDbContext context)
        {
            _context = context;
        }

        public List<DadosProduto> ListarTodos()
        {
            return _context.Produtos.Select(
                p => new DadosProduto() 
                {
                    CodigoBarras = p.CodigoBarras,
                    Nome = p.Nome,
                    Preco = p.Preco
                }).ToList();
        }

        public bool Incluir(DadosProduto dadosProduto,
            out StringBuilder inconsistencias)
        {
            inconsistencias = DadosValidos(dadosProduto);
            if (inconsistencias.Length == 0)
            {
                Produto produto = _context.Produtos.Where(
                    p => p.CodigoBarras == dadosProduto.CodigoBarras.Trim().ToUpper())
                    .FirstOrDefault();

                if (produto == null)
                {
                    produto = new Produto();
                    produto.CodigoBarras = dadosProduto.CodigoBarras;
                    produto.Nome = dadosProduto.Nome;
                    produto.Preco = dadosProduto.Preco;
                    
                    _context.Produtos.Add(produto);
                    _context.SaveChanges();
                }
                else
                {
                    inconsistencias.Append(
                        "Código de Barras já cadastrado |");
                }                
            }

            if (inconsistencias.Length > 0)
                inconsistencias.Insert(0, "| ");
            
            return inconsistencias.Length == 0; 
        }

        public bool Alterar(DadosProduto dadosProduto,
            out StringBuilder inconsistencias)
        {
            inconsistencias = DadosValidos(dadosProduto);
            if (inconsistencias.Length == 0)
            {
                Produto produto = _context.Produtos.Where(
                    p => p.CodigoBarras == dadosProduto.CodigoBarras.Trim().ToUpper())
                    .FirstOrDefault();

                if (produto == null)
                {
                    inconsistencias.Append(
                        "Produto não encontrado |");
                }
                else
                {
                    produto.Nome = dadosProduto.Nome;
                    produto.Preco = dadosProduto.Preco;
                    _context.SaveChanges();
                }                
            }

            if (inconsistencias.Length > 0)
                inconsistencias.Insert(0, "| ");
            return inconsistencias.Length == 0; 
        }

        private StringBuilder DadosValidos(DadosProduto produto)
        {
            var resultado = new StringBuilder();
            if (produto == null)
            {                
                resultado.Append(
                    " Preencha os Dados do Produto |");
            }
            else
            {
                if (String.IsNullOrWhiteSpace(produto.CodigoBarras))
                {
                    resultado.Append(
                        " Preencha o Código de Barras |");
                }
                if (String.IsNullOrWhiteSpace(produto.Nome))
                {
                    resultado.Append(
                        " Preencha o Nome do Produto |");
                }
                if (produto.Preco <= 0)
                {
                    resultado.Append(
                        " O Preço do Produto deve ser maior do que zero |");
                }
            }

            return resultado;
        }
    }
}