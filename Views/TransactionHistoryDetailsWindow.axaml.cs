using System;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Data.Converters;
using TBGL.ViewModels;

namespace TBGL.Views;

public sealed partial class TransactionHistoryDetailsWindow : ViewBase<TransactionHistoryDetailsWindowViewModel>
{
    public TransactionHistoryDetailsWindow(IServiceProvider services) 
        : base(services)
    {
        InitializeComponent();
    }

    private void GenerateColumnFormat(object? sender, DataGridAutoGeneratingColumnEventArgs e)
    {
        if (e.PropertyType != typeof(decimal?))
            return;

        if (e.Column is DataGridTextColumn { Binding: BindingBase binding } textColumn)
        {
            binding.Mode = BindingMode.OneWay;
            binding.StringFormat = "{0:C}";
            binding.Converter = new StringFormatValueConverter(binding.StringFormat, null);
        }
    }
}