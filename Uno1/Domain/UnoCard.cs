﻿
namespace Domain;

public class UnoCard
{
    public Color CardColor { get; set; }
    public Value CardValue { get; set; }

    public UnoCard(Color color, Value value)
    {
        CardColor = color;
        CardValue = value;
    }
    
    // For deserialization
    public UnoCard()
    {
    }

    public override string ToString()
    {
        return $"{CardColor}_{CardValue}";
    }
    
    public override bool Equals(object? obj)
    {
        if (obj == null || GetType() != obj.GetType())
        {
            return false;
        }

        UnoCard otherCard = (UnoCard)obj;
        return CardColor == otherCard.CardColor && CardValue == otherCard.CardValue;
    }


    public enum Color
    {
        Red, Blue, Green, Yellow, Wild
    }

    public enum Value
    {
        Zero, One, Two, Three, Four, Five, Six, Seven, Eight, Nine, DrawTwo, Skip, Reverse, Wild, WildFour
    }
}