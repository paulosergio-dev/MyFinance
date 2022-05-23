using MyFinance.Util;
using System.Data;

namespace MyFinance.Models
{
    public class HomeModel
    {
        public string LerNomeUsuario()
        {
            DAL objDAL = new DAL();
            DataTable dt = objDAL.RetDataTable("select * from usuario");

            if (dt != null)
            {
                if (dt.Rows.Count > 0)
                {
                    return dt.Rows[0]["Nome"].ToString();
                }
            }
            return "Nome não encontrado!";
        }
    }
}
