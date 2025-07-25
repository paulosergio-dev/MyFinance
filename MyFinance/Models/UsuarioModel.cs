using MyFinance.Util;
using MyFinance.Services;
using System;
using System.Data;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace MyFinance.Models
{
    public class UsuarioModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Informe seu nome!")]
        [StringLength(100, ErrorMessage = "O nome deve ter no máximo 100 caracteres")]
        public string Nome { get; set; }

        [Required(ErrorMessage = "Informe seu Email!")]
        [DataType(DataType.EmailAddress)]
        [EmailAddress(ErrorMessage = "O e-mail informado é invalido")]
        [StringLength(150, ErrorMessage = "O email deve ter no máximo 150 caracteres")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Informe sua senha!")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "A senha deve ter entre 6 e 100 caracteres")]
        public string Senha { get; set; }

        [Required(ErrorMessage = "Informe sua Data de Nascimento!")]
        [DataType(DataType.Date)]
        public string Data_Nascimento { get; set; }

        public bool ValidarLogin(DAL dal, IPasswordHashService passwordHashService)
        {
            try
            {
                string sql = "SELECT ID, NOME, DATA_NASCIMENTO, SENHA FROM USUARIO WHERE EMAIL = @Email";
                var parameters = new Dictionary<string, object>
                {
                    { "@Email", Email }
                };

                DataTable dt = dal.RetDataTable(sql, parameters);

                if (dt != null && dt.Rows.Count == 1)
                {
                    string hashedPassword = dt.Rows[0]["SENHA"].ToString();
                    
                    if (passwordHashService.VerifyPassword(Senha, hashedPassword))
                    {
                        Id = int.Parse(dt.Rows[0]["ID"].ToString());
                        Nome = dt.Rows[0]["NOME"].ToString();
                        Data_Nascimento = dt.Rows[0]["DATA_NASCIMENTO"].ToString();
                        return true;
                    }
                }
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool RegistrarUsuario(DAL dal, IPasswordHashService passwordHashService)
        {
            try
            {
                // Verificar se email já existe
                string sqlCheck = "SELECT COUNT(*) FROM USUARIO WHERE EMAIL = @Email";
                var checkParams = new Dictionary<string, object>
                {
                    { "@Email", Email }
                };

                int count = Convert.ToInt32(dal.ExecutarScalar(sqlCheck, checkParams));
                if (count > 0)
                {
                    return false; // Email já existe
                }

                // Hash da senha
                string hashedPassword = passwordHashService.HashPassword(Senha);

                // Inserir usuário
                string dataNascimento = DateTime.Parse(Data_Nascimento).ToString("yyyy-MM-dd");
                string sql = "INSERT INTO USUARIO (NOME, EMAIL, SENHA, DATA_NASCIMENTO) VALUES (@Nome, @Email, @Senha, @DataNascimento)";
                
                var parameters = new Dictionary<string, object>
                {
                    { "@Nome", Nome },
                    { "@Email", Email },
                    { "@Senha", hashedPassword },
                    { "@DataNascimento", dataNascimento }
                };

                int result = dal.ExecutarComandoSQL(sql, parameters);
                return result > 0;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
