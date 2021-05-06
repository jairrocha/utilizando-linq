using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace JrTunes.MetodoExtensao
{
    public static class LinqExtension
    {

        //Dica: Copiamos a assinatura do método: Average (Isso facilitou nossa vida ;D)


        //Observe a assinatura.
        public static decimal Mediana<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, decimal>> selector)
        {

            //Veja abaixo como o método era antes...

            /*
               private static Decimal Mediana(IQueryable<decimal> query)
               {

           
                    int qtndeElementos = query.Count();

                    var aux_elementoCentral1 = query.OrderBy(total => total).Skip(qtndeElementos / 2).First();
                    var aux_elementoCentral2 = query.OrderBy(total => total).Skip((qtndeElementos - 1) / 2).First();

                    if (qtndeElementos % 2 == 0)
                    {
                        return (aux_elementoCentral1 + aux_elementoCentral2) / 2;
                    }

            
                    return aux_elementoCentral1;

                }
            */


            //Veja as adptações que tivemos que fazer para torna-lo genérico.


            //source substituiu query
            //note que tivemos que adicionar o 'Select(funcSelector)'

            int qtndeElementos = source.Count();

            var funcSelector = selector.Compile(); // Obrigatório. Temos que declarar uma variável para o seletor.Copile

            var aux_elementoCentral1 = source.Select(funcSelector).OrderBy(q => q).Skip(qtndeElementos / 2).First();
            var aux_elementoCentral2 = source.Select(funcSelector).OrderBy(q => q).Skip((qtndeElementos - 1) / 2).First();

            if (qtndeElementos % 2 == 0)
            {
                return (aux_elementoCentral1 + aux_elementoCentral2) / 2;
            }


            return aux_elementoCentral1;

        }
    }
}
