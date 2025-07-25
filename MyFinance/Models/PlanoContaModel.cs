using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyFinance.Models;
using MyFinance.Util;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;

namespace MyFinance.Models
{
    public class PlanoContaModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Informe a descrição.")]
        [StringLength(200, ErrorMessage = "A descrição deve ter no máximo 200 caracteres")]
        public string Descricao { get; set; }

        [Required(ErrorMessage = "Informe o tipo.")]
        public string Tipo { get; set; }

        public int Usuario_Id { get; set; }

        public static List<PlanoContaModel> ListaPlanoContas(DAL dal, int usuarioId)
        {
            try
            {
                List<PlanoContaModel> lista = new List<PlanoContaModel>();

                string sql = "SELECT ID, DESCRICAO, TIPO, USUARIO_ID FROM PLANO_CONTAS WHERE USUARIO_ID = @UsuarioId ORDER BY DESCRICAO";
                var parameters = new Dictionary<string, object>
                {
                    { "@UsuarioId", usuarioId }
                };

                DataTable dt = dal.RetDataTable(sql, parameters);

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    var item = new PlanoContaModel
                    {
                        Id = int.Parse(dt.Rows[i]["ID"].ToString()),
                        Descricao = dt.Rows[i]["DESCRICAO"].ToString(),
                        Tipo = dt.Rows[i]["TIPO"].ToString(),
                        Usuario_Id = int.Parse(dt.Rows[i]["USUARIO_ID"].ToString())
                    };
                    lista.Add(item);
                }
                return lista;
            }
            catch (Exception)
            {
                return new List<PlanoContaModel>();
            }
        }

        public static PlanoContaModel CarregarRegistro(DAL dal, int id, int usuarioId)
        {
            try
            {
                string sql = "SELECT ID, DESCRICAO, TIPO, USUARIO_ID FROM PLANO_CONTAS WHERE USUARIO_ID = @UsuarioId AND ID = @Id";
                var parameters = new Dictionary<string, object>
                {
                    { "@UsuarioId", usuarioId },
                    { "@Id", id }
                };

                DataTable dt = dal.RetDataTable(sql, parameters);

                if (dt.Rows.Count == 1)
                {
                    return new PlanoContaModel
                    {
                        Id = int.Parse(dt.Rows[0]["ID"].ToString()),
                        Descricao = dt.Rows[0]["DESCRICAO"].ToString(),
                        Tipo = dt.Rows[0]["TIPO"].ToString(),
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
                // Verificar se já existe plano de conta com a mesma descrição
                string sqlCheck = "SELECT COUNT(*) FROM PLANO_CONTAS WHERE DESCRICAO = @Descricao AND USUARIO_ID = @UsuarioId";
                var checkParams = new Dictionary<string, object>
                {
                    { "@Descricao", Descricao },
                    { "@UsuarioId", usuarioId }
                };

                int count = Convert.ToInt32(dal.ExecutarScalar(sqlCheck, checkParams));
                if (count > 0)
                {
                    return false; // Plano de conta já existe
                }

                string sql = "INSERT INTO PLANO_CONTAS (DESCRICAO, TIPO, USUARIO_ID) VALUES (@Descricao, @Tipo, @UsuarioId)";
                var parameters = new Dictionary<string, object>
                {
                    { "@Descricao", Descricao },
                    { "@Tipo", Tipo },
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
                // Verificar se o plano de conta pertence ao usuário
                string sqlCheck = "SELECT COUNT(*) FROM PLANO_CONTAS WHERE ID = @Id AND USUARIO_ID = @UsuarioId";
                var checkParams = new Dictionary<string, object>
                {
                    { "@Id", Id },
                    { "@UsuarioId", usuarioId }
                };

                int count = Convert.ToInt32(dal.ExecutarScalar(sqlCheck, checkParams));
                if (count == 0)
                {
                    return false; // Plano de conta não pertence ao usuário
                }

                string sql = "UPDATE PLANO_CONTAS SET DESCRICAO = @Descricao, TIPO = @Tipo WHERE USUARIO_ID = @UsuarioId AND ID = @Id";
                var parameters = new Dictionary<string, object>
                {
                    { "@Descricao", Descricao },
                    { "@Tipo", Tipo },
                    { "@UsuarioId", usuarioId },
                    { "@Id", Id }
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
                // Verificar se existem transações associadas ao plano de conta
                string sqlCheck = "SELECT COUNT(*) FROM TRANSACAO WHERE PLANO_CONTAS_ID = @PlanoContaId";
                var checkParams = new Dictionary<string, object>
                {
                    { "@PlanoContaId", id }
                };

                int transacaoCount = Convert.ToInt32(dal.ExecutarScalar(sqlCheck, checkParams));
                if (transacaoCount > 0)
                {
                    return false; // Não pode excluir plano de conta com transações
                }

                string sql = "DELETE FROM PLANO_CONTAS WHERE ID = @Id AND USUARIO_ID = @UsuarioId";
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

