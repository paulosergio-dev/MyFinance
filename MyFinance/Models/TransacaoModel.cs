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
        public string Data { get; set; }
        public string Tipo { get; set; }
        public double Valor { get; set; }
        [Required(ErrorMessage = "Informe a descrição!")]
        public string Descricao { get; set; }
        public int Conta_Id { get; set; }
        public string NomeConta { get; set; }

        public int Plano_Contas_Id { get; set; }
        public string DescricaoPlanoConta { get; set; }


        public IHttpContextAccessor HttpContextAccessor { get; set; }

        public TransacaoModel()
        {

        }

        //Recebe o contexto para acesso as variáveis de sessão
        public TransacaoModel(IHttpContextAccessor httpContextAccessor)
        {
            HttpContextAccessor = httpContextAccessor;
        }

        public List<TransacaoModel> ListaTransacao()
        {
            List<TransacaoModel> lista = new List<TransacaoModel>();
            TransacaoModel item;

            string id_usuario_logado = HttpContextAccessor.HttpContext.Session.GetString("IdUsuarioLogado");

            string sql = " select t.Id, t.Data, t.Tipo, t.Valor, t.Descricao as Descrição, " +
                         " t.Conta_Id, c.Nome as Conta, t.Plano_Contas_Id, p.Descricao as Plano_Conta " +
                         " from transacao as t inner join conta c " +
                         " on t.Conta_Id = c.Id inner join Plano_Contas as p " +
                         " on t.Plano_Contas_Id = p.Id " +
                        $" where t.usuario_id = {id_usuario_logado} order by t.data desc limit 10";
                                 
            DAL objDAL = new DAL();
            DataTable dt = objDAL.RetDataTable(sql);

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                item = new TransacaoModel();
                item.Id = int.Parse(dt.Rows[i]["ID"].ToString());
                item.Data = DateTime.Parse(dt.Rows[i]["Data"].ToString()).ToString("dd/MM/yyyy");
                item.Tipo = dt.Rows[i]["Tipo"].ToString();
                item.Descricao = dt.Rows[i]["Descrição"].ToString();
                item.Conta_Id = int.Parse(dt.Rows[i]["Conta_Id"].ToString());
                item.Valor = Double.Parse(dt.Rows[i]["Valor"].ToString());
                item.NomeConta = dt.Rows[i]["Conta"].ToString();
                item.Plano_Contas_Id = int.Parse(dt.Rows[i]["Plano_Contas_Id"].ToString());
                item.DescricaoPlanoConta = dt.Rows[i]["Plano_Conta"].ToString();
                lista.Add(item);
            }
            return lista;
        }
    }
}
