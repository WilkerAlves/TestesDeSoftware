using System;
using System.Collections.Generic;
using System.Linq;
using NerdStore.Core.DomainObjects;

namespace NerdStore.Vendas.Domain
{
    public class Pedido
    {
        public static int MAX_UNIDADES_ITEM => 15;
        public static int MIN_UNIDADES_ITEM => 1;

        public decimal ValorTotal { get; private set; }
        public PedidoStatus PedidoStatus { get; private set; }
        public Guid ClienteId { get; private set; }
        public IReadOnlyCollection<PedidoItem> PedidoItems => _pedidoItems;
        private readonly List<PedidoItem> _pedidoItems;

        protected Pedido()
        {
            _pedidoItems = new List<PedidoItem>();
        }

        public void AdicionarItem(PedidoItem pedidoItem)
        {
            ValidarQuantidadeItemPermitida(pedidoItem);

            if (PedidoItemExistente(pedidoItem))
            {
                var itemExistente = _pedidoItems.FirstOrDefault(p => p.ProdutoId == pedidoItem.ProdutoId);
                
                itemExistente.AdicionarUnidades(pedidoItem.Quantidade);
                pedidoItem = itemExistente;
                _pedidoItems.Remove(itemExistente);

            }
            _pedidoItems.Add(pedidoItem);
            CalcularValorPedido();
        }

        public void AtualizarItem(PedidoItem pedidoItem)
        {
            ValidarPedidoItemExistente(pedidoItem);
            ValidarQuantidadeItemPermitida(pedidoItem);

            var itemExistente = PedidoItems.FirstOrDefault(p => p.ProdutoId == pedidoItem.ProdutoId);
            
            _pedidoItems.Remove(itemExistente);
            _pedidoItems.Add(pedidoItem);

            CalcularValorPedido();
        }

        public void RemoverItem(PedidoItem pedidoItem)
        {
            ValidarPedidoItemExistente(pedidoItem);
            _pedidoItems.Remove(pedidoItem);
            CalcularValorPedido();
        }

        private void CalcularValorPedido()
        {
            ValorTotal = PedidoItems.Sum(i => i.CalcularValor());
        }

        public void TornarRascunho()
        {
            PedidoStatus = PedidoStatus.Rascunho;
        }

        private bool PedidoItemExistente(PedidoItem pedidoItem)
        {
            return _pedidoItems.Any(p => p.ProdutoId == pedidoItem.ProdutoId);
        }
         
        private void ValidarQuantidadeItemPermitida(PedidoItem item)
        {
            var quantidadeItens = item.Quantidade;
            if (PedidoItemExistente(item))
            {
                var itemExistente = _pedidoItems.FirstOrDefault(p => p.ProdutoId == item.ProdutoId);
                quantidadeItens += itemExistente.Quantidade;
            }

            if (quantidadeItens > MAX_UNIDADES_ITEM) throw new DomainException($"Maximo de {MAX_UNIDADES_ITEM} unidades por produto");
        }

        private void ValidarPedidoItemExistente(PedidoItem item)
        {
            if (!PedidoItemExistente(item)) throw new DomainException($"O item não existe no pedido");
        }

        public static class PedidoFactory
        {
            public static Pedido NovoPedidoRascunho(Guid clienteId)
            {
                var pedido = new Pedido{ ClienteId = clienteId};
                pedido.TornarRascunho();
                return pedido;
            }
        }
    }

    public class PedidoItem
    {
        public Guid ProdutoId { get; private set; }
        public string ProdutoNome { get; private set; }
        public int Quantidade { get; private set; }
        public decimal ValorUnitario { get; private set; }

        public PedidoItem(Guid produtoId, string produtoNome, int quantidade, decimal valorUnitario)
        {
            ProdutoId = produtoId;
            ProdutoNome = produtoNome;
            Quantidade = quantidade;
            ValorUnitario = valorUnitario;

            if (Quantidade < Pedido.MIN_UNIDADES_ITEM) throw new DomainException($"Minimo de {Pedido.MIN_UNIDADES_ITEM} unidades por produto");
        }

        internal void AdicionarUnidades(int unidades)
        {
            Quantidade += unidades;
        }

        internal decimal CalcularValor()
        {
            return Quantidade * ValorUnitario;
        }

    }

    public enum PedidoStatus
    {
        Rascunho = 0,
        Iniciado = 1,
        Pago = 4,
        Entregue = 5,
        Cancelado = 6
    }
}