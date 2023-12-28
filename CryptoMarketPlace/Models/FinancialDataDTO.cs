namespace CryptoMarketPlace.Models
{

    //model representing JSON data 
    public class FinancialDataDTO
    {

      
            public string symbol { get; set; }
            public string priceChange { get; set; }
            public string priceChangePercent { get; set; }
            public string lastPrice { get; set; }
            public string openPrice { get; set; }
          
        

    }
}
