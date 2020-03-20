using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UTDll;
namespace UTool.Test
{
    /// <summary>
    /// 
    /// </summary>
    public class Test_Demo2 : UTest
    {
        public Test_Demo2()
        {
            // 
            // TODO: Add constructor logic here
            //
        }
        [UMethod]
        public void testMyClass1(byte b)
        {
        }
        [UMethod]
        public void test2(DateTime t)
        {
            printf("CurrentTime={0}", t);
        }
        [UMethod]
        public void test3(string s, int i, long l, double dou, bool t)
        {
            Test_Demo2 tx = new Test_Demo2();
            tx.test2(DateTime.Now);

        }
        [UMethod]
        public void test5(int a, int b)
        {
            int result = 1;
            int range = (a > b) ? a : b;
            for (int divisor = 1; divisor < range; divisor++)
            {
                int aQuotient;
                Math.DivRem(a, divisor, out aQuotient);
                int bQuotient = 0;
                Math.DivRem(b, divisor, out bQuotient);
                if (aQuotient == 0 && bQuotient == 0)
                {
                    result = divisor;
                }
            }
            printf("{0} and {1} ; LCM= {2}", a, b, result);
        }
        [UMethod]
        public void test6()
        {
            /*
               口口
            X    口
            _________
              口口
            + 口口
            _________
              口口
            "口" 填入1~9數字不可重複!
            */
            for (int a1 = 1; a1 <= 9; a1++)
                for (int a2 = 1; a2 <= 9; a2++)
                    for (int b1 = 1; b1 <= 9; b1++)
                        for (int c1 = 1; c1 <= 9; c1++)
                            for (int c2 = 1; c2 <= 9; c2++)
                                for (int d1 = 1; d1 <= 9; d1++)
                                    for (int d2 = 1; d2 <= 9; d2++)
                                        for (int e1 = 1; e1 <= 9; e1++)
                                            for (int e2 = 1; e2 <= 9; e2++)
                                            {
                                                if (test(a1, a2, b1, c1, c2, d1, d2, e1, e2))
                                                {
                                                    printf("a=> {0},{1} b=>{2}, c=>{3},{4} d=>{5},{6} e=>{7},{8}", a1, a2, b1, c1, c2, d1, d2, e1, e2);
                                                    return;
                                                }
                                            }


        }
        public static bool test(int a1, int a2, int b1, int c1, int c2, int d1, int d2, int e1, int e2)
        {
            bool t2 = ((a1 * 10 + a2) * b1 == (c1 * 10 + c2));
            bool t3 = ((c1 * 10 + c2 + d1 * 10 + d2) == (e1 * 10 + e2));
            if (t2 && t3)
            {
                var set = new HashSet<int>(new int[] { a1, a2, b1, c1, c2, d1, d2, e1, e2 });               
                if (set.Count == 9)
                {
                    //Console.WriteLine("a=> {0},{1} b=>{2}, c=>{3},{4} d=>{5},{6} e=>{7},{8}", a1, a2, b1, c1, c2, d1, d2, e1, e2);
                    return true;
                }
            }
            return false;
        }
    }
    }

    
