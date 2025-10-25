using System;
using Ecauspacine.Contracts.Attributes;
using Ecauspacine.Contracts.Lookups;
using Ecauspacine.Wpf.ViewModels.Base;

namespace Ecauspacine.Wpf.ViewModels.Dashboard.Schema;

public class AttributeRuleDisplayViewModel : ViewModelBase
{
    public AttributeRuleDisplayViewModel(AttributeRuleDto rule, AttributeDefDto definition, LookupGroupDto? lookupGroup)
    {
        Rule = rule;
        Definition = definition;
        LookupGroup = lookupGroup;

        _accessMode = rule.AccessMode;
        _isRequired = rule.IsRequired;
        _orderIndex = rule.OrderIndex;
        _defaultValue = rule.DefaultValue?.ToString();
    }

    public AttributeRuleDto Rule { get; }
    public AttributeDefDto Definition { get; }
    public LookupGroupDto? LookupGroup { get; }

    private string _accessMode = string.Empty;
    public string AccessMode
    {
        get => _accessMode;
        set => SetProperty(ref _accessMode, value);
    }

    private bool _isRequired;
    public bool IsRequired
    {
        get => _isRequired;
        set => SetProperty(ref _isRequired, value);
    }

    private int _orderIndex;
    public int OrderIndex
    {
        get => _orderIndex;
        set => SetProperty(ref _orderIndex, value);
    }

    private string? _defaultValue;
    public string? DefaultValue
    {
        get => _defaultValue;
        set => SetProperty(ref _defaultValue, value);
    }

    public string Display => $"{Definition.Label} ({Definition.Code})";
    public string DataKind => Definition.DataKind;
}
