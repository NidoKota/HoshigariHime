using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public static class Utility
{
    /// <summary>
    /// このindexが要素数内か判定する
    /// </summary>
    public static bool Within<T>(this IEnumerable<T> i, int index)
    {
        return i.Count() > index && index >= 0;
    }
}
