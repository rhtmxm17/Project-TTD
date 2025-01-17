using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CurrencyFormat
{
    private static string[] symbol = { "K", "M", "G", "T", "P", "E", "Z" }; 
    
    public static string Trans(long value)
    { 
        string st = value.ToString();
        
        if (st.Length <= 4)
        {
            return st;
        }
        
        //1만 이상부터 축약
        for (int i = 0; i < symbol.Length; i++)
        {
            if (4 + 3 * i <= st.Length && st.Length < 4 + 3 * (i + 1))
            {
                int n = st.Length % 3;
        
                n = n == 0 ? 3 : n;
        
                st = $"{st.Substring(0, n)}.{st.Substring(n, 2)}"; 
                st += symbol[i];
                break;
            } 
        }
        
        return st;
    } 
}
