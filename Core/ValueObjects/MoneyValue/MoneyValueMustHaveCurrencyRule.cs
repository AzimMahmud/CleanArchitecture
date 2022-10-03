﻿using Core.Common;

namespace Core.ValueObjects.MoneyValue;

public class MoneyValueMustHaveCurrencyRule :  IBusinessRule
{
    private readonly string _currency;

    public MoneyValueMustHaveCurrencyRule(string currency)
    {
        _currency = currency;
    }
    public bool IsBroken() => string.IsNullOrEmpty(_currency);

    public string Message => "Money value must have currency";
}