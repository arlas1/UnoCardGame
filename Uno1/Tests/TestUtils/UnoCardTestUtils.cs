﻿using Domain;

namespace Tests.TestUtils;

public static class UnoCardTestUtils
{
    public static List<UnoCard.Value> GetAllPossibleValuesToAvoid()
    {
        return 
        [
            UnoCard.Value.Skip,
            UnoCard.Value.Reverse,
            UnoCard.Value.DrawTwo,
            UnoCard.Value.Wild,
            UnoCard.Value.WildFour
        ];
    }
    
    // public static UnoCard GetWith
}