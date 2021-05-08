using JrTunes.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JrTunes
{
    public static class curso_parte_2
    {

        public static void Paginacao()
        {

            const int TAMANHO_PAGINA = 10;


            using (var contexto = new JrTunesEntities())
            {
                int numeroDeLinhas = contexto.NotaFiscais.Count();

                int numeroDePaginas = (int)Math.Ceiling((decimal)(numeroDeLinhas / TAMANHO_PAGINA));

                for (int p = 0; p <= numeroDePaginas; p++)
                {
                    ImprimirPagina(TAMANHO_PAGINA, p, contexto);

                }


            }



        }
        public static void SubQuery()
        {
            using (var contexto = new JrTunesEntities())
            {

                /*Trazer vendas com valor acima da média*/

                decimal queryVendaMedia = contexto.NotaFiscais.Average(q => q.Total);

                var query = from nf in contexto.NotaFiscais
                            where nf.Total > queryVendaMedia //<----SubQuery
                            orderby nf.Total descending
                            select new
                            {
                                Numero = nf.NotaFiscalId,
                                Data = nf.DataNotaFiscal,
                                Cliente = nf.Cliente.PrimeiroNome + " " + nf.Cliente.Sobrenome,
                                Valor = nf.Total

                            }
                            ;


                foreach (var nf in query)
                {
                    Console.WriteLine("{0}\t{1}\t{2}\t{3}", nf.Numero, nf.Data, nf.Cliente.PadRight(30), nf.Valor);
                }


                Console.WriteLine("A média é: {0}", queryVendaMedia);

            }
        }

        public static void UtilizandoPropriedadeDeOutraQuery_UtilizandoVariaelLocalQuery()
        {
            /*Tazer o produto mais vendido e os clientes que 
                         * compraram o produto mais vendido.
                         */

            using (var contexto = new JrTunesEntities())
            {
                var faixaQuery = from f in contexto.Faixas
                                 where f.ItemNotaFiscals.Count() > 0
                                 /*Variável interna da consulta (let)*/
                                 let TotalDeVendas = f.ItemNotaFiscals.Sum(q => q.Quantidade * q.PrecoUnitario)
                                 orderby TotalDeVendas descending
                                 select new
                                 {
                                     Id = f.FaixaId,
                                     Nome = f.Nome,
                                     Total = TotalDeVendas

                                 };

                var produtoMaisVendido = faixaQuery.First();

                Console.WriteLine("{0}\t{1}\t{2}", produtoMaisVendido.Id,
                                                   produtoMaisVendido.Nome.PadRight(100),
                                                   produtoMaisVendido.Total);

                var query = from inf in contexto.ItemNotaFiscal
                            where inf.FaixaId == produtoMaisVendido.Id
                            select new
                            {
                                Cliente = inf.NotaFiscal.Cliente.PrimeiroNome + " " + inf.NotaFiscal.Cliente.Sobrenome,
                            };

                foreach (var cliente in query)
                {
                    Console.WriteLine("Nome do cliente: {0}", cliente.Cliente);
                }

            }
        }



















        private static void ImprimirPagina(int TAMANHO_PAGINA, int numeroDaPagina, JrTunesEntities contexto)
        {

            //contexto.Database.Log = Console.WriteLine;

            var query = from nf in contexto.NotaFiscais
                        orderby nf.NotaFiscalId /*o 'Skip' só funciona se a conulta estiver ordenada*/
                        select new
                        {
                            Numero = nf.NotaFiscalId,
                            Data = nf.DataNotaFiscal,
                            Cliente = nf.Cliente.PrimeiroNome + " " + nf.Cliente.Sobrenome,
                            Total = nf.Total
                        };



            int numeroDePulos = (numeroDaPagina) * TAMANHO_PAGINA;

            query = query.Skip(numeroDePulos).Take(TAMANHO_PAGINA);


            Console.WriteLine("\nNumero da página: {0}\n", numeroDaPagina);

            foreach (var nf in query)
            {

                Console.WriteLine("{0}\t{1}\t{2}\t{3}", nf.Numero, nf.Data, nf.Cliente.PadRight(30), nf.Total);
            }
        }
       


    }
}
