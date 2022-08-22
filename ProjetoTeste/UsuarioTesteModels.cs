using System;
using Xunit;
using MyFinance.Models;

namespace ProjetoTeste
{
    public class UsuarioTesteModels
    {
        [Fact(DisplayName = "Teste login usuário.")]
        public void TestLoginUsuario()
        {
            UsuarioModel usuarioModel = new UsuarioModel();
            usuarioModel.Email = "paulosergio@unigranrio.br";
            usuarioModel.Senha = "123";
            bool result = usuarioModel.ValidarLogin();

            Assert.True(result);          
        }

        //[Fact(DisplayName = "Teste registrar usuário.")]
        //public void TestRegistrarUsuario()
        //{
        //    UsuarioModel usuarioModel = new UsuarioModel();
        //    usuarioModel.Nome = "Teste";
        //    usuarioModel.Email = "usuario@gmail.br";
        //    usuarioModel.Senha = "123456";
        //    usuarioModel.Data_Nascimento = "1952/05/22";          
        //    usuarioModel.RegistrarUsuario();
        //    bool result = usuarioModel.ValidarLogin();

        //    Assert.True(result);
        //}
    }
}
