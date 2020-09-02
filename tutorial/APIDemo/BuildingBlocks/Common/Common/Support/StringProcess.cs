using System;
using System.Text;
using System.Text.RegularExpressions;

namespace Common.Support
{
    public class StringProcess
    {
        public static string GetBitInChinese(int bit,int money) 
        {
           
            string moneys = money + "";
            char[] moneySingle = moneys.ToCharArray();
            var length = moneySingle.Length;
            if (bit > length)
            {
                return "";
            }
            if (bit == 0)
            {
                StringBuilder sb = new StringBuilder();
                string total = "";
                for (int i = 1; i <= length; i++)
                {
                    total = AddDollarsTitle(
                        ConvertChineseNumber(
                        Convert.ToString(moneySingle[length - i])
                        )

                        , i - 1) + total;
                    
                }
                
                string R2 = total.Replace("零億", "零").Replace("零仟萬", "零").Replace("零萬", "零").Replace("零仟", "零").Replace("零佰", "零").Replace("零佰萬", "零").Replace("零拾", "零");
                for(int u=0;u<10;u++)
                {
              R2=  R2.Replace("零零", "零");
                }
                return R2;

            }
            else {
                char choiceChar = moneySingle[length - bit];
                return ConvertChineseNumber(choiceChar + ""); 
            }
           
        
        }
        public static string ConvertChineseNumber(string number) {
            int no = Convert.ToInt32(number);
            string result = "";
            switch (no)
            {
                case 0:
                    result= "零" ;
            break;
                case 1: result = "壹";
            break;
                case 2: result = "貳";
            break;
                case 3: result = "參";
            break;
                case 4: result = "肆";
            break;
                case 5: result = "伍";
            break;
                case 6: result = "陸";
            break;
                case 7: result = "柒";
            break;
                case 8: result = "捌";
            break;
                case 9: result = "玖";
            break;
            
            }
            return result;
        
        }
        public static string TrimZeroDollarsTitle(string zerosTitle) {
            if (zerosTitle.Substring(0,1).Equals("零"))
            {
                return "";
            }
            return zerosTitle;
        }
        public static string AddDollarsTitle(string number,int position)
        {
     
            string result = "";
            if (number.Equals("零"))
            {
                return "零";
            }
            else { 
            
            }
            switch (position)
            {
                case 0:
                    result = number+"";
                    break;
                case 1:
                    result = number + "拾";
                    break;
                case 2: result = number + "佰";
                    break;
                case 3: result = number + "仟";
                    break;
                case 4: result = number + "萬";
                    break;
                case 5: result = number + "拾萬";
                    break;
                case 6: result = number + "佰萬";
                    break;
                case 7: result = number + "仟萬";
                    break;
                case 8: result = number + "億";
                    break;
                
            }
            return result;

        }



        public static string AddReturnString(string tabItemTitle)
        {
            StringBuilder sb = new StringBuilder();
            int size = 4;
            for (int i = 0; i < tabItemTitle.Length; i++)
            {
                sb.Append(tabItemTitle[i]);
                if (((i + 1) % size) == 0)
                    sb.Append("\r\n");
            }
            return sb.ToString();
        }

        public enum AppendDirection { 
            Left,Right
        }
        public static String GetComplementLengthWithString(String toFit, int size, String appendedValue,AppendDirection direction)
        {
           
            String result = "";
            if (string.IsNullOrEmpty(toFit))
            {
                return result;
            }
            if (toFit.Length == size)
            {
                result = toFit;
            }
            else if (toFit.Length > size)
            {
                return toFit;

            }
            else
            {
                int diff = size - toFit.Length;

                StringBuilder d = new StringBuilder();
                for (int i = 0; i < diff; i++)
                {

                    d.Append(appendedValue);
                }
                if (direction == AppendDirection.Right)
                {
                   // d.Append(toFit);
                    result = toFit+d.ToString();
                }
                else {
                    d.Append(toFit);
                    result = d.ToString();
                }
                
              //  result = d.ToString();
            }
            return result;
        }
        /* 不足補上特殊符號
       * @param tofit String
       * @param size int
       * @param appendstring String 補上的字串
       * @return String
       */
        public static String GetComplementLengthWithString(String toFit, int size, String appendedValue)
        {
            return GetComplementLengthWithString(toFit, size, appendedValue,AppendDirection.Left);
            
        }
        /// <summary>
        /// 取得有可能是null的東西轉成string
        /// </summary>
        /// <param name="content">The content.</param>
        /// <returns></returns>
        public static string GetStringFromContent(object content)
        {
            return (content == null) ? "" : content.ToString();

        }
          public static MatchCollection MatchRegex(String pattern, String content)
        {
          Regex regex = new Regex(pattern);
            string input = content;            
            MatchCollection matches = regex.Matches(input);
             return matches;


          }

       

    }
}
