using Microsoft.AspNetCore.Http;
using MyFinance.Util;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace MyFinance.Models
{
    public class ContaModel
    {
        public int Id { get; set; }
        
        [Required(ErrorMessage = "Favor informar o nome da conta.")]
        [StringLength(100, ErrorMessage = "O nome deve ter no máximo 100 caracteres")]
        public string Nome { get; set; }
        
        [Required(ErrorMessage = "Favor informar o saldo atual da conta.")]
        [Range(0, double.MaxValue, ErrorMessage = "O saldo deve ser maior ou igual a zero")]
        public decimal Saldo { get; set; }
        
        public int Usuario_Id { get; set; }

        public static List<ContaModel> ListaConta(DAL dal, int usuarioId)
        {
            try
            {
                List<ContaModel> lista = new List<ContaModel>();
                
                string sql = "SELECT ID, NOME, SALDO, USUARIO_ID FROM CONTA WHERE USUARIO_ID = @UsuarioId ORDER BY NOME";
                var parameters = new Dictionary<string, object>
                {
                    { "@UsuarioId", usuarioId }
                };
                
                DataTable dt = dal.RetDataTable(sql, parameters);

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    var item = new ContaModel
                    {
                        Id = int.Parse(dt.Rows[i]["ID"].ToString()),
                        Nome = dt.Rows[i]["NOME"].ToString(),
                        Saldo = decimal.Parse(dt.Rows[i]["SALDO"].ToString()),
                        Usuario_Id = int.Parse(dt.Rows[i]["USUARIO_ID"].ToString())
                    };
                    lista.Add(item);
                }
                return lista;
            }
            catch (Exception)
            {
                return new List<ContaModel>();
            }
        }

        public static ContaModel CarregarRegistro(DAL dal, int id, int usuarioId)
        {
            try
            {
                string sql = "SELECT ID, NOME, SALDO, USUARIO_ID FROM CONTA WHERE ID = @Id AND USUARIO_ID = @UsuarioId";
                var parameters = new Dictionary<string, object>
                {
                    { "@Id", id },
                    { "@UsuarioId", usuarioId }
                };
                
                DataTable dt = dal.RetDataTable(sql, parameters);
                
                if (dt.Rows.Count == 1)
                {
                    return new ContaModel
                    {
                        Id = int.Parse(dt.Rows[0]["ID"].ToString()),
                        Nome = dt.Rows[0]["NOME"].ToString(),
                        Saldo = decimal.Parse(dt.Rows[0]["SALDO"].ToString()),
                        Usuario_Id = int.Parse(dt.Rows[0]["USUARIO_ID"].ToString())
                    };
                }
                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public bool Insert(DAL dal, int usuarioId)
        {
            try
            {
                // Verificar se já existe conta com o mesmo nome para o usuário
                string sqlCheck = "SELECT COUNT(*) FROM CONTA WHERE NOME = @Nome AND USUARIO_ID = @UsuarioId";
                var checkParams = new Dictionary<string, object>
                {
                    { "@Nome", Nome },
                    { "@UsuarioId", usuarioId }
                };
                
                int count = Convert.ToInt32(dal.ExecutarScalar(sqlCheck, checkParams));
                if (count > 0)
                {
                    return false; // Conta já existe
                }

                string sql = "INSERT INTO CONTA (NOME, SALDO, USUARIO_ID) VALUES (@Nome, @Saldo, @UsuarioId)";
                var parameters = new Dictionary<string, object>
                {
                    { "@Nome", Nome },
                    { "@Saldo", Saldo },
                    { "@UsuarioId", usuarioId }
                };
                
                int result = dal.ExecutarComandoSQL(sql, parameters);
                return result > 0;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool Atualizar(DAL dal, int usuarioId)
        {
            try
            {
                // Verificar se a conta pertence ao usuário
                string sqlCheck = "SELECT COUNT(*) FROM CONTA WHERE ID = @Id AND USUARIO_ID = @UsuarioId";
                var checkParams = new Dictionary<string, object>
                {
                    { "@Id", Id },
                    { "@UsuarioId", usuarioId }
                };
                
                int count = Convert.ToInt32(dal.ExecutarScalar(sqlCheck, checkParams));
                if (count == 0)
                {
                    return false; // Conta não pertence ao usuário
                }

                string sql = "UPDATE CONTA SET NOME = @Nome, SALDO = @Saldo WHERE ID = @Id AND USUARIO_ID = @UsuarioId";
                var parameters = new Dictionary<string, object>
                {
                    { "@Nome", Nome },
                    { "@Saldo", Saldo },
                    { "@Id", Id },
                    { "@UsuarioId", usuarioId }
                };
                
                int result = dal.ExecutarComandoSQL(sql, parameters);
                return result > 0;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static bool Excluir(DAL dal, int id, int usuarioId)
        {
            try
            {
                // Verificar se existem transações associadas à conta
                string sqlCheck = "SELECT COUNT(*) FROM TRANSACAO WHERE CONTA_ID = @ContaId";
                var checkParams = new Dictionary<string, object>
                {
                    { "@ContaId", id }
                };
                
                int transacaoCount = Convert.ToInt32(dal.ExecutarScalar(sqlCheck, checkParams));
                if (transacaoCount > 0)
                {
                    return false; // Não pode excluir conta com transações
                }

                string sql = "DELETE FROM CONTA WHERE ID = @Id AND USUARIO_ID = @UsuarioId";
                var parameters = new Dictionary<string, object>
                {
                    { "@Id", id },
                    { "@UsuarioId", usuarioId }
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
