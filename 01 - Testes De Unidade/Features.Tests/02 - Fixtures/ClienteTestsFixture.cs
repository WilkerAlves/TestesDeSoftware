using System;
using Features.Clientes;
using Xunit;

// anotações

// as classes de testes são recriadas a cada metodo de teste,
// para evitar que aja algum com compartilhamento de estado e informações entre testes já existentes.

// uma fixture é preparado antes de começar a classe de testes, e só destruido depois que o ultimo teste rodar
// os objetos criados na fixture são validos durante todas as execuções dos testes que fazendo parte da coleção por ex: [CollectionDefinition(nameof(ClienteCollection))]

// as classe que usaram essa fixture deve possuir a seguinte anotação [Collection(nameof(ClienteCollection))]
namespace Features.Tests
{
    [CollectionDefinition(nameof(ClienteCollection))]
    public class ClienteCollection : ICollectionFixture<ClienteTestsFixture>
    {}

    public class ClienteTestsFixture : IDisposable
    {
        public Cliente GerarClienteValido()
        {
            var cliente = new Cliente(
                Guid.NewGuid(),
                "Eduardo",
                "Pires",
                DateTime.Now.AddYears(-30),
                "edu@edu.com",
                true,
                DateTime.Now);

            return cliente;
        }

        public Cliente GerarClienteInValido()
        {
            var cliente = new Cliente(
                Guid.NewGuid(),
                "",
                "",
                DateTime.Now,
                "edu2edu.com",
                true,
                DateTime.Now);

            return cliente;
        }

        public void Dispose()
        {
        }
    }
}

