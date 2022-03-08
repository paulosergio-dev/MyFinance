using System;
using Xunit;
using MyFinance.Models;

namespace ProjetoTeste
{
    public class UnitTesteModels
    {
        [Fact]
        public void TestLoginUsuario()
        {
            UsuarioModel usuarioModel = new UsuarioModel();

            usuarioModel.Email = "paulosergio@unigranrio.br";
            usuarioModel.Senha = "123";
            bool result = usuarioModel.ValidarLogin();
            Assert.True(result);
          
        }

        [Fact]
        public void TestRegistrarUsuario()
        {
            UsuarioModel usuarioModel = new UsuarioModel();
            usuarioModel.Nome = "Teste";
            usuarioModel.Data_Nascimento = "1952/05/22";
            usuarioModel.Email = "teste@unigranrio.br";
            usuarioModel.Senha = "123";
            usuarioModel.RegistrarUsuario();
            bool result = usuarioModel.ValidarLogin();

            Assert.True(result);

        }
    }
}
