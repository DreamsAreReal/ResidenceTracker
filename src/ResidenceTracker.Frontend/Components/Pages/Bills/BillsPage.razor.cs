using Microsoft.AspNetCore.Components;
using MudBlazor;
using ResidenceTracker.Domain.Entities;
using ResidenceTracker.Frontend.Components.Abstractions;
using ResidenceTracker.Infrastructure.Abstractions;

namespace ResidenceTracker.Frontend.Components.Pages.Bills;

public partial class BillsPage : AbstractCrudBasePage<Bill>
{
    protected override IReadOnlyCollection<string> DisplayOrder { get; } = new List<string>
    {
        "Id",
        "Flat.Number",
        "AmountInRubles",
        "PaidIn",
        "CreatedAt",
        "ModifiedAt",
    };

    [Inject]
    protected IRepository<Flat> FlatRepository { get; set; } = null!;

    private async Task BillsPaid()
    {
        if (Table is null)
        {
            Snackbar.Add("Произошла ошибка", Severity.Error);
            return;
        }

        List<Bill> items = Table.SelectedItems.Where(x => !x.IsReadonly).ToList();

        if (items.Count < 1)
        {
            Snackbar.Add(
                "Не выбрано ни одного элемента. Возможно выбранные счета были уже оплачены.", Severity.Error
            );

            return;
        }

        await Table.ChangeLoadingState(true);

        try
        {
            items.ForEach(
                x =>
                {
                    x.PaidIn = DateTime.UtcNow;
                    x.IsReadonly = true;
                }
            );

            await Repository.UpdateRangeAsync(items, CancellationToken);
            await Table.UpdateTableData();
            Snackbar.Add("Коммунальные платежи оплачены", Severity.Success);
        }
        catch (Exception e)
        {
            Snackbar.Add("Произошла ошибка", Severity.Error);

            Logger.LogError(
                e, "Error in {@ClassName} during {@MethodName} execution", GetType().Name, nameof(BillsPaid)
            );
        }
        finally
        {
            await Table.ChangeLoadingState(false);
        }
    }
}