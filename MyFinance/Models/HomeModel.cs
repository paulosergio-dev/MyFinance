using MyFinance.Util;
using System.Data;
using Microsoft.Extensions.Configuration;

namespace MyFinance.Models
{
    public class HomeModel
    {
        private readonly IConfiguration _configuration;

        public HomeModel(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string LerNomeUsuario()
        {
            DAL objDAL = new DAL(_configuration);
            DataTable dt = objDAL.RetDataTable("select * from usuario");

            if (dt != null)
            {
                if (dt.Rows.Count > 0)
                {
                    return dt.Rows[0]["Nome"].ToString();
                }
            }
            return "Nome não encontrado na nossa base de dados!";
        }
    }
}