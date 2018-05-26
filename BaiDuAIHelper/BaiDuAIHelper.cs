using System;
using System.Collections.Generic;

namespace BaiDuAIHelper
{
    public static class BaiDuAIHelper
    {
        private static String API_KEY = "M35jGMa9p3aHm4SZO9p8RawI";

        private static String SECRET_KEY = "lakOfQtGGecLXk6WRfSRF0eGvW8ZZ8DP";
        public static string Idcard(byte[] img, string id_card_side)
        {
            var client = new Baidu.Aip.Ocr.Ocr(API_KEY, SECRET_KEY);
            client.Timeout = 60000;  // 修改超时时间 
            var options = new Dictionary<string, object>{
        {"detect_direction", "true"},
        {"detect_risk", "false"}
    };
            var result = client.Idcard(img, id_card_side,options);
            return result.ToString();
        }

    }
}
