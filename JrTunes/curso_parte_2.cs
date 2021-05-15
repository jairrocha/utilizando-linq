using JrTunes.Data;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZXing;

namespace JrTunes
{
    public static class curso_parte_2
    {

        public static void Paginacao()
        {

            const int TAMANHO_PAGINA = 10;


            using (var contexto = new JrTunesEntities())
            {
                int numeroDeLinhas = contexto.NotaFiscals.Count();

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

                decimal queryVendaMedia = contexto.NotaFiscals.Average(q => q.Total);

                var query = from nf in contexto.NotaFiscals
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
        public static void Analise_de_afinidade()
        {
            /* Ralizando analise de finalide (Recomendação de compra com base em outras compras)
                         * Analise de finalidade nada mais é que uma consulta que retorna pedidos (faixas) compradas
                         * por que qm comprou a faixa indiciada ()
                         */

            //faixa indicada
            var faixaMusica = "Smells like Teen Spirit";

            using (var contexto = new JrTunesEntities())
            {


                var faixaIds = contexto.Faixas.Where(q => q.Nome == faixaMusica).Select(f => f.FaixaId);



                //Self join = auto join (join sobre a mesma entidade)
                Console.WriteLine("RECOMENDAÇÃO DE COMPRA JRTUNES \n");

                Console.WriteLine("\nQuem comprou a faixa: '{0}', acabou comprando tbm as faixas: \n", faixaMusica);

                var query = from comprouItem in contexto.ItemNotaFiscal
                            join comprouTbm in contexto.ItemNotaFiscal
                                on comprouItem.NotaFiscalId equals comprouTbm.NotaFiscalId
                            where faixaIds.Contains(comprouTbm.FaixaId) //Equivalendo ao IN
                            && comprouItem.FaixaId != comprouTbm.FaixaId // Elimina a faixa indicada
                            select comprouItem;

                foreach (var item in query)
                {
                    Console.WriteLine("{0}\t{1}", item.NotaFiscalId, item.Faixa.Nome);
                }


            }
        }
        public static void ExecucaoImediata()
        {
            /*
            * Execução imediada
            */

            using (var contexto = new JrTunesEntities())
            {

                var mesAniversario = 1;

                while (mesAniversario < 12)
                {


                    Console.WriteLine("Aniversariantes do mês: {0}", mesAniversario);

                    /* Qndo usamos o linq, por padão a exeução é tardia, ou seja, ela é executada 
                     * somente na leitura (no nosso caso estava sendo executa somente no foreach).
                     * Podemos forçar a execução da mesma usando o .ToList(), ToArray() e etc.
                     * Isso é últimos qndo tirar uma foto do estado naquele momento e manter.
                     * A a tardia, traz a informação atualizada no momento da leitura (for each).
                     * Veja o exemplo abaixo
                     */

                    var lista = (from f in contexto.Funcionarios
                                 where f.DataNascimento.Value.Month == mesAniversario
                                 orderby f.DataNascimento.Value.Month, f.DataNascimento.Value.Day
                                 select f).ToList(); //Consulta sendo carregada aqui


                    /*Aqui estamos incrementando o mes aniversário observe que nesse caso não tem problema
                     pois a consulta já foi carrada graça ao to List
                    

                    se executássemos a consulta abaixo teriamos um problema pois iriamos iriamos
                    criar uma definiciação de consulta para o mês 2 e no for each iriamos imprimir
                    essa definição para o mês 1.

                    >>EXEMPLO:
                    
                    >CONSULTA:
                    
                     var lista = from f in contexto.Funcionarios
                                where f.DataNascimento.Value.Month == mesAniversario
                                orderby f.DataNascimento.Value.Month, f.DataNascimento.Value.Day
                                select f; //Consulta não carrega. Apenas criação de definição. (Será executa na leitura)
                     
                    >SAIDA:

                    Anicerariantes do Mês: 1
                    18/02   Andrew Adams
                    Anicerariantes do Mês: 2
                    03/03   Steve Johnson
                    Anicerariantes do Mês: 3
                    Anicerariantes do Mês: 4
                    29/05   Robert King
                    Anicerariantes do Mês: 5
                    Anicerariantes do Mês: 6
                    01/07   Michael Mitchell
                    Anicerariantes do Mês: 7
                    29/08   Jane Peacock
                    Anicerariantes do Mês: 8
                    19/09   Margaret Park
                    Anicerariantes do Mês: 9
                    Anicerariantes do Mês: 10
                    Anicerariantes do Mês: 11
                    08/12   Nancy Edwards

                     */


                    mesAniversario++;

                    foreach (var f in lista)
                    {
                        Console.WriteLine("{0:dd/MM}\t{1} {2}", f.DataNascimento, f.PrimeiroNome, f.Sobrenome);
                    }

                }
            }
        }
        public static void Paralelismo_na_criacao_de_qrcode()
        {

           /*
           
           Install-Package ZXing.Net (Biblioteca de QRCode)
           Add reference: System.Drawing (Nescessário para gerar imagens)
           Site que lê qrcode https://webqr.com/ 
           
            */


            var Imagens = "IMG";

            var barcodWriter = new BarcodeWriter();

            barcodWriter.Format = BarcodeFormat.QR_CODE;
            barcodWriter.Options = new ZXing.Common.EncodingOptions
            {
                Width = 100,
                Height = 100
            };


            if (!Directory.Exists(Imagens)) // Verifica se diretório existe
                Directory.CreateDirectory(Imagens); // Cria diretório

            using (var contexto = new JrTunesEntities())
            {
                var queryFaixas = from f in contexto.Faixas
                                  select f;

                var listaFaixas = queryFaixas.ToList();


                Stopwatch stopwatch = Stopwatch.StartNew(); //Inicia cronômetro

                //Sem o AsParallel: 16 segundos
                //com o AsParallel: 8 segundos

                var queryCodigos = listaFaixas
                                   .AsParallel() //Pareliza a execução entre vário núcleos (roda mais rápido)
                                   .Select(f => new
                                   {
                                       Arquivo = string.Format("{0}\\{1}.jpg", Imagens, f.FaixaId),
                                       Imagem = barcodWriter.Write(string.Format("jrtunes.com/faixa/{0}", f.FaixaId))
                                   });

                int contagem = queryCodigos.Count();


                //foreach (var item in queryCodigos)
                //{
                //    item.Imagem.Save(item.Arquivo, ImageFormat.Jpeg); // Salva imagem
                //}

                //Paraleizando o foreach. Faz com que a tarefa seja divida em núcleos
                queryCodigos.ForAll(item => item.Imagem.Save(item.Arquivo, ImageFormat.Jpeg));


                stopwatch.Stop(); //Para cronômetro

                Console.WriteLine("Código gerados: {0} segundos em {1} segundos",
                                    contagem,
                                    stopwatch.ElapsedMilliseconds / 1000); //Exibe cronômetro

            }

            //imprimentdo texto no qrcode e salando imagem
            //barcodWriter.Write("Meu teste").Save("QRCODE.jpg", ImageFormat.Jpeg);
        }
        public static void Consumindo_Stored_Procedure()
        {
            //Consumindo stored procedure com o LINQ

            //1)Compilar procedure no BD

            /* 
            CREATE PROCEDURE[dbo].[ps_Itens_Por_Cliente] @clienteId int = 0
            AS
            BEGIN

                SELECT
                i.FaixaId,
                i.ItemNotaFiscalId,
                i.NotaFiscalId,
                i.PrecoUnitario,
                i.Quantidade,
                i.PrecoUnitario* i.Quantidade As Total,
                n.DataNotaFiscal,
                f.Nome
                FROM ItemNotaFiscal i
                JOIN NotaFiscal n ON i.NotaFiscalId = n.NotaFiscalId
                JOIN Faixa f ON f.FaixaId = i.FaixaId
                WHERE n.ClienteId = @clienteId
            END
            */

            //2)Atualizar o JrTunes.edmx para que a procedure seja adicionada ao modelo


            int clienteId = 17;

            using (var contexto = new JrTunesEntities())
            {

                var vendasPorCliente =
                from v in contexto.ps_Itens_Por_Cliente(clienteId)
                group v by new { v.DataNotaFiscal.Year, v.DataNotaFiscal.Month }
                into agrupado
                orderby agrupado.Key.Year, agrupado.Key.Month
                select new
                {
                    Ano = agrupado.Key.Year,
                    Mes = agrupado.Key.Month,
                    Total = agrupado.Sum(a => a.Total)
                };

                foreach (var item in vendasPorCliente)
                {
                    Console.WriteLine("{0}\t{1}\t{2}", item.Ano, item.Mes, item.Total);
                }


            }
        }















        private static void ImprimirPagina(int TAMANHO_PAGINA, int numeroDaPagina, JrTunesEntities contexto)
        {

            //contexto.Database.Log = Console.WriteLine;

            var query = from nf in contexto.NotaFiscals
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
