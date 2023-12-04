using Microsoft.AspNetCore.Components;
using MudBlazor;
using ResidenceTracker.Domain.Abstractions;
using ResidenceTracker.Frontend.Components.Custom;
using ResidenceTracker.Infrastructure.Abstractions;
using ResidenceTracker.UseCases.Validation.Abstractions;

namespace ResidenceTracker.Frontend.Components.Abstractions;

public abstract class AbstractCrudBasePage<T> : ComponentBase where T : AbstractEntity, new()
{
    protected CancellationToken CancellationToken => _cancellationTokenSource.Token;
    protected T ChangeableItem { get; private set; } = new();

    protected abstract IReadOnlyCollection<string> DisplayOrder { get; }

    protected MudForm Form { get; set; } = null!;

    protected bool IsFormHidden { get; private set; } = true;
    protected bool IsLoading { get; private set; }
    [Inject]
    protected ILogger<AbstractCrudBasePage<T>> Logger { get; private set; } = null!;

    [Inject]
    protected NavigationManager NavigationManager { get; set; } = null!;
    [Inject]
    protected IRepository<T> Repository { get; private set; } = null!;
    [Inject]
    protected ISnackbar Snackbar { get; private set; } = null!;
    protected Table<T>? Table { get; set; }

    [Inject]
    protected AbstractFormValidator<T> Validator { get; private set; } = null!;
    private readonly CancellationTokenSource _cancellationTokenSource = new();
    private bool _isEdit;

    protected void ChangeFormState(bool isHidden)
    {
        if (isHidden == IsFormHidden)
            return;

        IsFormHidden = isHidden;
        StateHasChanged();
    }

    protected async Task HandleAddNewItem()
    {
        await Table?.UpdateTableData()!;
        ChangeFormState(false);
        ChangeableItem = new();
        _isEdit = false;
        StateHasChanged();
    }

    protected async Task HandleChangeItem(T selectedItem)
    {
        await Table?.UpdateTableData()!;
        ChangeFormState(false);
        ChangeableItem = selectedItem;
        _isEdit = true;
        StateHasChanged();
    }

    protected void HandleLoadingChange(bool loading)
    {
        IsLoading = loading;
    }

    protected override Task OnInitializedAsync()
    {
        Snackbar.Clear();
        Snackbar.Configuration.PositionClass = Defaults.Classes.Position.TopCenter;
        Snackbar.Configuration.SnackbarVariant = Variant.Filled;
        Snackbar.Configuration.MaxDisplayedSnackbars = 3;
        return Task.CompletedTask;
    }

    protected async Task Submit()
    {
        HandleLoadingChange(true);
        await Form.Validate();

        if (!Form.IsValid)
        {
            HandleLoadingChange(false);
            return;
        }

        ChangeFormState(true);

        try
        {
            if (_isEdit)
                await Repository.UpdateAsync(ChangeableItem, CancellationToken);
            else
                await Repository.AddAsync(ChangeableItem, CancellationToken);

            await Table?.UpdateTableData()!;
            Snackbar.Add($"Элемент был добавлен {ChangeableItem.Id}", Severity.Success);
        }
        catch (Exception e)
        {
            Snackbar.Add("Произошла ошибка", Severity.Error);

            Logger.LogError(
                e, "Error in {@ClassName} during {@MethodName} execution", GetType().Name, nameof(Submit)
            );
        }
        finally
        {
            HandleLoadingChange(false);
        }
    }
}