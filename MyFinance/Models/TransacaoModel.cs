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
    public class TransacaoModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Informe a data!")]
        [DataType(DataType.Date)]
        public string Data { get; set; }

        public string DataFinal { get; set; }  //Utilizado para Filtros

        [Required(ErrorMessage = "Informe o tipo!")]
        public string Tipo { get; set; }

        [Required(ErrorMessage = "Informe o valor!")]
        [Range(0.01, double.MaxValue, ErrorMessage = "O valor deve ser maior que zero")]
        public decimal Valor { get; set; }

        [Required(ErrorMessage = "Informe a descrição!")]
        [StringLength(200, ErrorMessage = "A descrição deve ter no máximo 200 caracteres")]
        public string Descricao { get; set; }

        [Required(ErrorMessage = "Selecione uma conta!")]
        public int Conta_Id { get; set; }

        public string NomeConta { get; set; }

        [Required(ErrorMessage = "Selecione um plano de conta!")]
        public int Plano_Contas_Id { get; set; }

        public string DescricaoPlanoConta { get; set; }

        public static List<TransacaoModel> ListaTransacao(DAL dal, int usuarioId, string dataInicio = null, string dataFinal = null, string tipo = null, int contaId = 0, int limite = 10)
        {
            try
            {
                List<TransacaoModel> lista = new List<TransacaoModel>();
                var parameters = new Dictionary<string, object>
                {
                    { "@UsuarioId", usuarioId },
                    { "@Limite", limite }
                };

                string filtros = "";

                if (!string.IsNullOrEmpty(dataInicio) && !string.IsNullOrEmpty(dataFinal))
                {
                    filtros += " AND t.Data >= @DataInicio AND t.Data <= @DataFinal ";
                    parameters.Add("@DataInicio", DateTime.Parse(dataInicio));
                    parameters.Add("@DataFinal", DateTime.Parse(dataFinal));
                }

                if (!string.IsNullOrEmpty(tipo) && tipo != "A")
                {
                    filtros += " AND t.Tipo = @Tipo ";
                    parameters.Add("@Tipo", tipo);
                }

                if (contaId > 0)
                {
                    filtros += " AND t.Conta_Id = @ContaId ";
                    parameters.Add("@ContaId", contaId);
                }

                string sql = $@"
                    SELECT TOP (@Limite) t.Id, t.Data, t.Tipo, t.Valor, t.Descricao, 
                           t.Conta_Id, c.Nome as Conta, t.Plano_Contas_Id, p.Descricao as Plano_Conta 
                    FROM TRANSACAO t 
                    INNER JOIN CONTA c ON t.Conta_Id = c.Id 
                    INNER JOIN PLANO_CONTAS p ON t.Plano_Contas_Id = p.Id 
                    WHERE t.Usuario_Id = @UsuarioId {filtros} 
                    ORDER BY t.Data DESC";

                DataTable dt = dal.RetDataTable(sql, parameters);

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    var item = new TransacaoModel
                    {
                        Id = int.Parse(dt.Rows[i]["ID"].ToString()),
                        Data = DateTime.Parse(dt.Rows[i]["Data"].ToString()).ToString("dd/MM/yyyy"),
                        Tipo = dt.Rows[i]["Tipo"].ToString(),
                        Descricao = dt.Rows[i]["Descricao"].ToString(),
                        Conta_Id = int.Parse(dt.Rows[i]["Conta_Id"].ToString()),
                        Valor = decimal.Parse(dt.Rows[i]["Valor"].ToString()),
                        NomeConta = dt.Rows[i]["Conta"].ToString(),
                        Plano_Contas_Id = int.Parse(dt.Rows[i]["Plano_Contas_Id"].ToString()),
                        DescricaoPlanoConta = dt.Rows[i]["Plano_Conta"].ToString()
                    };
                    lista.Add(item);
                }
                return lista;
            }
            catch (Exception)
            {
                return new List<TransacaoModel>();
            }
        }

        public static TransacaoModel CarregarRegistro(DAL dal, int id, int usuarioId)
        {
            try
            {
                string sql = @"
                    SELECT t.Id, t.Data, t.Tipo, t.Valor, t.Descricao, 
                           t.Conta_Id, c.Nome as Conta, t.Plano_Contas_Id, p.Descricao as Plano_Conta 
                    FROM TRANSACAO t 
                    INNER JOIN CONTA c ON t.Conta_Id = c.Id 
                    INNER JOIN PLANO_CONTAS p ON t.Plano_Contas_Id = p.Id 
                    WHERE t.Usuario_Id = @UsuarioId AND t.Id = @Id";

                var parameters = new Dictionary<string, object>
                {
                    { "@UsuarioId", usuarioId },
                    { "@Id", id }
                };

                DataTable dt = dal.RetDataTable(sql, parameters);

                if (dt.Rows.Count == 1)
                {
                    return new TransacaoModel
                    {
                        Id = int.Parse(dt.Rows[0]["ID"].ToString()),
                        Data = DateTime.Parse(dt.Rows[0]["Data"].ToString()).ToString("dd/MM/yyyy"),
                        Tipo = dt.Rows[0]["Tipo"].ToString(),
                        Descricao = dt.Rows[0]["Descricao"].ToString(),
                        Conta_Id = int.Parse(dt.Rows[0]["Conta_Id"].ToString()),
                        Valor = decimal.Parse(dt.Rows[0]["Valor"].ToString()),
                        NomeConta = dt.Rows[0]["Conta"].ToString(),
                        Plano_Contas_Id = int.Parse(dt.Rows[0]["Plano_Contas_Id"].ToString()),
                        DescricaoPlanoConta = dt.Rows[0]["Plano_Conta"].ToString()
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
                // Verificar se conta e plano de conta pertencem ao usuário
                string sqlValidacao = @"
                    SELECT COUNT(*) FROM CONTA WHERE Id = @ContaId AND Usuario_Id = @UsuarioId;
                    SELECT COUNT(*) FROM PLANO_CONTAS WHERE Id = @PlanoContaId AND Usuario_Id = @UsuarioId;";

                var validacaoParams = new Dictionary<string, object>
                {
                    { "@ContaId", Conta_Id },
                    { "@PlanoContaId", Plano_Contas_Id },
                    { "@UsuarioId", usuarioId }
                };

                // Validar se conta pertence ao usuário
                string sqlConta = "SELECT COUNT(*) FROM CONTA WHERE Id = @ContaId AND Usuario_Id = @UsuarioId";
                int contaCount = Convert.ToInt32(dal.ExecutarScalar(sqlConta, validacaoParams));
                
                if (contaCount == 0)
                    return false;

                // Validar se plano de conta pertence ao usuário
                string sqlPlanoConta = "SELECT COUNT(*) FROM PLANO_CONTAS WHERE Id = @PlanoContaId AND Usuario_Id = @UsuarioId";
                int planoContaCount = Convert.ToInt32(dal.ExecutarScalar(sqlPlanoConta, validacaoParams));
                
                if (planoContaCount == 0)
                    return false;

                string sql = @"
                    INSERT INTO TRANSACAO (DATA, TIPO, DESCRICAO, VALOR, CONTA_ID, PLANO_CONTAS_ID, USUARIO_ID) 
                    VALUES (@Data, @Tipo, @Descricao, @Valor, @ContaId, @PlanoContaId, @UsuarioId)";

                var parameters = new Dictionary<string, object>
                {
                    { "@Data", DateTime.Parse(Data) },
                    { "@Tipo", Tipo },
                    { "@Descricao", Descricao },
                    { "@Valor", Valor },
                    { "@ContaId", Conta_Id },
                    { "@PlanoContaId", Plano_Contas_Id },
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
                // Verificar se a transação pertence ao usuário
                string sqlCheck = "SELECT COUNT(*) FROM TRANSACAO WHERE Id = @Id AND Usuario_Id = @UsuarioId";
                var checkParams = new Dictionary<string, object>
                {
                    { "@Id", Id },
                    { "@UsuarioId", usuarioId }
                };

                int count = Convert.ToInt32(dal.ExecutarScalar(sqlCheck, checkParams));
                if (count == 0)
                    return false;

                string sql = @"
                    UPDATE TRANSACAO SET 
                        DATA = @Data, 
                        TIPO = @Tipo, 
                        DESCRICAO = @Descricao, 
                        VALOR = @Valor, 
                        CONTA_ID = @ContaId, 
                        PLANO_CONTAS_ID = @PlanoContaId 
                    WHERE Id = @Id AND Usuario_Id = @UsuarioId";

                var parameters = new Dictionary<string, object>
                {
                    { "@Data", DateTime.Parse(Data) },
                    { "@Tipo", Tipo },
                    { "@Descricao", Descricao },
                    { "@Valor", Valor },
                    { "@ContaId", Conta_Id },
                    { "@PlanoContaId", Plano_Contas_Id },
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
                string sql = "DELETE FROM TRANSACAO WHERE Id = @Id AND Usuario_Id = @UsuarioId";
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

    public class DashBoard
    {
        public decimal Total { get; set; }
        public string PlanoConta { get; set; }

        public static List<DashBoard> RetornarDadosGraficoPie(DAL dal, int usuarioId)
        {
            try
            {
                List<DashBoard> lista = new List<DashBoard>();

                string sql = @"
                    SELECT SUM(t.Valor) as Total, p.Descricao 
                    FROM TRANSACAO t 
                    INNER JOIN PLANO_CONTAS p ON t.Plano_Contas_Id = p.Id 
                    WHERE t.Tipo = 'D' AND t.Usuario_Id = @UsuarioId 
                    GROUP BY p.Descricao 
                    ORDER BY Total DESC";

                var parameters = new Dictionary<string, object>
                {
                    { "@UsuarioId", usuarioId }
                };

                DataTable dt = dal.RetDataTable(sql, parameters);

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    var item = new DashBoard
                    {
                        Total = decimal.Parse(dt.Rows[i]["Total"].ToString()),
                        PlanoConta = dt.Rows[i]["Descricao"].ToString()
                    };
                    lista.Add(item);
                }
                return lista;
            }
            catch (Exception)
            {
                return new List<DashBoard>();
            }
        }
    }
}
