using Microsoft.EntityFrameworkCore;
using Wallet.Modules.asset_module;
using Wallet.Modules.user_module;
using Wallet.Tools.database;
using Wallet.Tools.generic_module;

namespace Wallet.Modules.position_module
{
    public class PositionService : GenericService<Position>, IPositionService
    {
        private Context _context;
        private readonly IUserService _userService;

        public PositionService(Context context, IUserService userService)
        {
            _context = context;
            _userService = userService;
        }

        public async Task<List<PositionDTO>> Read()
        {
            var userId = _userService.GetLoggedInUserId();
            var positions = await _context.Position.AsQueryable().Where(a => a.UserId == userId).ToListAsync();

            var positionDTO = new List<PositionDTO>();

            foreach (var position in positions)
            {
                var asset = await _context.Asset.AsQueryable().Where(a => a.Id == position.AssetId).FirstOrDefaultAsync();

                positionDTO.Add(new PositionDTO
                {
                    AssetName = asset.Description,
                    Ticker = asset.Ticker,
                    Amount = position.Amount,
                    AveragePrice = position.AveragePrice,
                    Price = asset.Price,
                    Size = position.Amount * asset.Price,
                    //RelativeSize deve calcular percentual dessa posição com relação a carteira.
                    TradeResult = GetTradeResult(position.Amount, position.AveragePrice, asset.Price),
                    TradeResultPercentage = GetTradeResultPercentage(asset.Price, position.AveragePrice),
                    TotalBought = position.TotalBought,
                    TotalSold = position.TotalSold,
                    Result = position.TotalGainLoss,
                    ResultPercentage = GetResultPercentage(position, asset)
                });
            }
            return positionDTO;
        }

        //scheduller para atualizar os valores dos ativos dos usuários logados de tempos em tempos.



        private double GetTradeResult(double amount, double averagePrice, double price)
        {
            return (price - averagePrice) * amount;
        }

        private double GetTradeResultPercentage(double price, double averagePrice)
        {
            return ((price - averagePrice) / averagePrice) * 100;
        }

        /// <summary>
        /// (total today + total sold + dividends?)/(total bought) - 1
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        private double GetResultPercentage(Position position, Asset asset)
        {
            return ((((position.Amount * asset.Price) + position.TotalSold) / position.TotalBought) - 1) * 100;
        }



        /*Controlar os resultados ao passar do tempo - 
         * 
        Armazenar informações relevantes das transações: Certifique-se de que sua tabela de movimentações(ou qualquer outra tabela que armazene as informações dos trades) contenha os dados necessários, como a data da transação, o ativo negociado, a quantidade negociada, o preço de compra/venda e outras informações relevantes.

        Calcular o valor das posições em cada momento: Com base nas informações das transações, você precisará calcular o valor das posições em cada momento específico.Isso pode ser feito multiplicando a quantidade negociada pelo   preço  de compra/venda para obter o valor da posição em cada trade.
        
        Considerar o peso das posições nos momentos: Se você deseja levar em conta o peso das posições nos momentos específicos, precisará calcular a participação de cada posição no valor total do portfólio em cada momento. Isso    pode   ser feito dividindo o valor de cada posição pelo valor total do portfólio no momento correspondente.
        
        Agregar os resultados diários/mensais: Com os valores das posições e os pesos calculados, você pode agregar os resultados diários ou mensais. Para os resultados diários, some os valores das posições e os pesos para cada dia      específico. Para os resultados mensais, faça o mesmo cálculo, mas agrupe os dados por mês.
        
        Acompanhar o desempenho ao longo do tempo: Com os resultados agregados, você terá uma visão do desempenho do portfólio ao longo do tempo.Poderá calcular retornos diários/mensais, métricas de desempenho, como índice de   Sharpe ou  taxa de retorno ajustada ao risco, e assim por diante.*/


        //EMPRESA(assetId)	CÓDIGO(assetId)		DATA ENTRADA(position resultado do trade)	PREÇO MÉDIO COMPRA(position)	COMPRAS ACUMULADO(position)	PREÇO MÉDIO DE VENDA NO MÊS	VENDAS ACUMULADO(position)	DIVIDENDOS ACUMULADO(position)	RENTABILID ACUMULADO(position)	PREÇO INICIO DO MÊS (ou de compra)	QTDE AÇÕES INICÍO NO MÊS	TOTAL INICIO DO MÊS R$	COMPRAS NO MÊS	VENDAS NO MÊS	QTDE ATUAL DE AÇÕES	PREÇO ATUAL OU DE VENDA NO MÊS	TOTAL HOJE $	PARTICIP ATUAL%	DIVIDENDO PAGO MÊS POR AÇÃO	RENTAB. MÊS

        // Exemplo 3R	RRRP3	RRRP3	06/06/2023	32,03	2.594,43			0,00	2,34% 	32,03	0	0,00	2.594,43 	0,00 	81	32,78	2.655,18	1,97%	0,00	2,34%
        //Exemplo S&P 500 ETF	IVVB11	IVVB11	10/11/2022	220,12	22.672,26	180,22	14.333,42	0,00	4,31% 	233,25	40	9.330,00	0,00 	0,00 	40	232,90	9.316,00	6,92%	0,00	(0,15%)

        //Tudo baseado na planilha, talvez fazer diário o resultado.

    }
}
