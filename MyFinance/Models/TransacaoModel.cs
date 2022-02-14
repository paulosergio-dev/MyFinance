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
        public string DataFinal { get; set; }  //Utilizado para Filtros
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

            //Utilizado pela Wiew Extrato
            string filtro = "";

            if ((Data != null) && (DataFinal != null))
            {
                filtro += $" and t.Data >= '{DateTime.Parse(Data).ToString("yyyy/MM/dd")}' and t.Data <=' {DateTime.Parse(DataFinal).ToString("yyyy/MM/dd")}' ";
            }

            if (Tipo != null)
            {
                if (Tipo != "A")
                {
                    filtro += $" and t.Tipo ='{Tipo}' ";
                }
            }
            if (Conta_Id != 0)
            {
                filtro += $" and t.Conta_Id ='{Conta_Id}' ";
            }

            //FIM

            string id_usuario_logado = HttpContextAccessor.HttpContext.Session.GetString("IdUsuarioLogado");

            string sql = " select t.Id, t.Data, t.Tipo, t.Valor, t.Descricao as Descrição, " +
                         " t.Conta_Id, c.Nome as Conta, t.Plano_Contas_Id, p.Descricao as Plano_Conta " +
                         " from transacao as t inner join conta c " +
                         " on t.Conta_Id = c.Id inner join Plano_Contas as p " +
                         " on t.Plano_Contas_Id = p.Id " +
                        $" where t.usuario_id = {id_usuario_logado} {filtro} order by t.data desc limit 10";

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

        public TransacaoModel CarregarRegistro(int? id)
        {
            TransacaoModel item;

            string id_usuario_logado = HttpContextAccessor.HttpContext.Session.GetString("IdUsuarioLogado");

            string sql = " select t.Id, t.Data, t.Tipo, t.Valor, t.Descricao as Descrição, " +
                         " t.Conta_Id, c.Nome as Conta, t.Plano_Contas_Id, p.Descricao as Plano_Conta " +
                         " from transacao as t inner join conta c " +
                         " on t.Conta_Id = c.Id inner join Plano_Contas as p " +
                         " on t.Plano_Contas_Id = p.Id " +
                        $" where t.usuario_id = {id_usuario_logado} and t.id='{id}'";

            DAL objDAL = new DAL();
            DataTable dt = objDAL.RetDataTable(sql);

            item = new TransacaoModel();
            item.Id = int.Parse(dt.Rows[0]["ID"].ToString());
            item.Data = DateTime.Parse(dt.Rows[0]["Data"].ToString()).ToString("dd/MM/yyyy");
            item.Tipo = dt.Rows[0]["Tipo"].ToString();
            item.Descricao = dt.Rows[0]["Descrição"].ToString();
            item.Conta_Id = int.Parse(dt.Rows[0]["Conta_Id"].ToString());
            item.Valor = Double.Parse(dt.Rows[0]["Valor"].ToString());
            item.NomeConta = dt.Rows[0]["Conta"].ToString();
            item.Plano_Contas_Id = int.Parse(dt.Rows[0]["Plano_Contas_Id"].ToString());
            item.DescricaoPlanoConta = dt.Rows[0]["Plano_Conta"].ToString();

            return item;
        }

        public void Insert()
        {
            string id_usuario_logado = HttpContextAccessor.HttpContext.Session.GetString("IdUsuarioLogado");
            string sql = "";
            if (Id == 0)
            {
                sql = $"INSERT INTO TRANSACAO (DATA, TIPO, DESCRICAO, VALOR, CONTA_ID, PLANO_CONTAS_ID, USUARIO_ID) " +
                    $" VALUES('{DateTime.Parse(Data).ToString("yyyy/MM/dd")}','{Tipo}','{Descricao}','{Valor}','{Conta_Id}','{Plano_Contas_Id}','{id_usuario_logado}')";
            }
            else
            {
                sql = $"UPDATE TRANSACAO SET DATA='{DateTime.Parse(Data).ToString("yyyy/MM/dd")}', " +
                      $"DESCRICAO = '{Descricao}'," +
                      $"TIPO='{Tipo}', " +
                      $"VALOR='{Valor}', " +
                      $"CONTA_ID='{Conta_Id}', " +
                      $"PLANO_CONTAS_ID='{Plano_Contas_Id}' " +
                      $"WHERE USUARIO_ID='{id_usuario_logado}' AND ID='{Id}'";
            }

            DAL objDAL = new DAL();
            objDAL.ExecutarComandoSQL(sql);
        }

        public void Excluir(int id)
        {
            new DAL().ExecutarComandoSQL("DELETE FROM TRANSACAO WHERE ID = " + id);
        }


    }

    public class DashBoard
    {
        public double Total { get; set; }
        public string PlanoConta { get; set; }

        public List<DashBoard> RetornarDadosGraficoPie()
        {
            List<DashBoard> lista = new List<DashBoard>();
            DashBoard item;

            string sql = "select sum(t.valor) as total, p.Descricao from transacao as t inner join plano_contas as p " +
                         "on t.Plano_Contas_Id = p.Id where t.Tipo = 'D' group by p.Descricao;";

            DAL objDAL = new DAL();
            DataTable dt = new DataTable();
            dt = objDAL.RetDataTable(sql);

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                item = new DashBoard();
                item.Total = double.Parse(dt.Rows[i]["Total"].ToString());
                item.PlanoConta = dt.Rows[i]["Descricao"].ToString();
                lista.Add(item);
            }
            return lista;
        }
    }
}
