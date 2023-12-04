using System.Collections.ObjectModel;
using System.Reflection;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using MudBlazor;
using Npgsql;
using ResidenceTracker.Domain.Abstractions;
using ResidenceTracker.Domain.Pagination;
using ResidenceTracker.Infrastructure.Abstractions;
using ResidenceTracker.UseCases.Utilities;

namespace ResidenceTracker.Frontend.Components.Custom;

public partial class Table<TModel> : ComponentBase where TModel : AbstractEntity
{
    [Parameter]
    public CancellationToken CancellationToken { get; set; } = CancellationToken.None;

    [Parameter]
    public IEnumerable<string> DisplayOrder { get; set; } = null!;

    [Parameter]
    public bool IsLoading { get; set; } = true;

    [Parameter]
    public int MaxItemsPerPage { get; set; } = int.MaxValue;

    [Parameter]
    public Func<Task>? OnAddNewItem { get; set; }

    [Parameter]
    public Func<TModel, Task>? OnChangeItem { get; set; }

    [Parameter]
    public EventCallback<bool> OnChangeLoadingState { get; set; } = EventCallback<bool>.Empty;

    [Parameter]
    public IRepository<TModel> Repository { get; set; } = null!;
    public HashSet<TModel> SelectedItems { get; private set; } = new();

    [Parameter]
    public bool ShowButtons { get; set; } = true;

    [Parameter]
    public ISnackbar Snackbar { get; set; } = null!;

    [Parameter]
    public string TableTitle { get; set; } = string.Empty;

    [Inject]
    protected ILogger<Table<TModel>> Logger { get; set; } = null!;

    private PagedResult<TModel> _pagedData = new(null!, 0, 1, 1);
    private MudPagination? _pagination;
    private string _searchText = string.Empty;

    public async Task ChangeLoadingState(bool isLoading)
    {
        if (isLoading == IsLoading)
            return;

        IsLoading = isLoading;
        await OnChangeLoadingState.InvokeAsync(isLoading);
    }

    public async Task UpdateTableData()
    {
        await UpdateTableData(_pagination?.Selected ?? 1);
    }

    protected override async Task OnInitializedAsync()
    {
        await UpdateTableData(1);
    }

    private async Task AddNewItem()
    {
        if (OnAddNewItem is not null)
            await OnAddNewItem.Invoke();
    }

    private async Task ChangeItem()
    {
        ReadOnlyCollection<TModel> items = SelectedItems.Where(x => !x.IsReadonly).ToList().AsReadOnly();

        if (items.Count != 1)
        {
            Snackbar.Add(
                "Нужно выбрать один элемент. Возможно выбранные элементы нельзя изменить или удалить.",
                Severity.Error
            );

            return;
        }

        if (OnChangeItem is not null)
            await OnChangeItem.Invoke(items.First());
    }

    private async Task ChangeSearchText(string text)
    {
        await OnChangeLoadingState.InvokeAsync(true);
        _searchText = text;
        await UpdateTableData(_pagination?.Selected ?? 1);
    }

    private async Task DeleteItems()
    {
        ReadOnlyCollection<TModel> items = SelectedItems.Where(x => !x.IsReadonly).ToList().AsReadOnly();

        if (items.Count < 1)
        {
            Snackbar.Add(
                "Не выбрано ни одного элемента. Возможно выбранные элементы нельзя изменить или удалить.",
                Severity.Error
            );

            return;
        }

        await ChangeLoadingState(true);

        try
        {
            await Repository.DeleteBatchAsync(items, CancellationToken);
            await UpdateTableData(_pagination?.Selected ?? 1);
            Snackbar.Add("Элементы были удалены", Severity.Success);
        }
        catch (DbUpdateException e) when (e.InnerException is PostgresException { SqlState: "23503", })
        {
            Snackbar.Add(
                "Элемент был привязан к другому элементу. Прежде нужно удалить родительский элемент",
                Severity.Error
            );
        }

        // ef not throw exception, some time exception is invalid operation
        catch (Exception e) when (e.Message.Contains("association between entity types"))
        {
            Snackbar.Add(
                "Элемент был привязан к другому элементу. Прежде нужно удалить родительский элемент",
                Severity.Error
            );
        }
        catch (Exception e)
        {
            Snackbar.Add("Произошла ошибка", Severity.Error);

            Logger.LogError(
                e, "Error in {@ClassName} during {@MethodName} execution", GetType().Name, nameof(DeleteItems)
            );
        }
        finally
        {
            await ChangeLoadingState(false);
        }
    }

    private string GetDisplayName(string propertyName)
    {
        if (propertyName.Contains("."))
        {
            string[] propertyNames = propertyName.Split('.');
            PropertyInfo? parentProperty = typeof(TModel).GetProperty(propertyNames[0]);

            if (parentProperty != null)
            {
                PropertyInfo? nestedProperty;

                if (parentProperty.PropertyType.IsGenericType &&
                    parentProperty.PropertyType.GetGenericTypeDefinition() == typeof(ICollection<>))
                {
                    Type elementType = parentProperty.PropertyType.GetGenericArguments()[0];
                    nestedProperty = elementType.GetProperty(propertyNames[1]);
                }
                else
                {
                    nestedProperty = parentProperty.PropertyType.GetProperty(propertyNames[1]);
                }

                if (nestedProperty is not null)
                    return
                        $"{nestedProperty.GetLocalizedDisplayName()} ({parentProperty.GetLocalizedDisplayName()})";
            }
        }
        else
        {
            PropertyInfo? propertyInfo = typeof(TModel).GetProperty(propertyName);

            if (propertyInfo != null)
                return $"{propertyInfo.GetLocalizedDisplayName()}";
        }

        return string.Empty;
    }

    private object? GetPropertyValue(TModel model, string propertyName)
    {
        if (propertyName.Contains("."))
        {
            string[] propertyNames = propertyName.Split('.');
            PropertyInfo? parentProperty = model.GetType().GetProperty(propertyNames[0]);

            if (parentProperty != null)
            {
                PropertyInfo? nestedProperty;

                if (parentProperty.PropertyType.IsGenericType &&
                    parentProperty.PropertyType.GetGenericTypeDefinition() == typeof(ICollection<>))
                {
                    Type elementType = parentProperty.PropertyType.GetGenericArguments()[0];
                    nestedProperty = elementType.GetProperty(propertyNames[1]);

                    if (nestedProperty is not null)
                    {
                        object? parentValue = parentProperty.GetValue(model);

                        if (parentValue is not null)
                            return ((IEnumerable<object?>)parentValue).Select(
                                x => x!.GetType().GetProperty(propertyNames[1])!.GetValue(x)
                            );
                    }
                }
                else
                {
                    nestedProperty = parentProperty.PropertyType.GetProperty(propertyNames[1]);
                }

                if (nestedProperty is not null)
                {
                    object? parentValue = parentProperty.GetValue(model);

                    if (parentValue is not null)
                    {
                        object? nestedValue = nestedProperty.GetValue(parentValue);
                        return nestedValue;
                    }
                }
            }
        }
        else
        {
            return model.GetType().GetProperty(propertyName)?.GetValue(model);
        }

        return null;
    }

    private async Task UpdateTableData(int pageCurrent)
    {
        await ChangeLoadingState(true);

        try
        {
            if (string.IsNullOrWhiteSpace(_searchText))
                _pagedData = await Repository.GetPagedAsync(pageCurrent, MaxItemsPerPage, CancellationToken);
            else
                _pagedData = await Repository.Search(
                                 _searchText, _pagination?.Selected ?? 1, MaxItemsPerPage, CancellationToken
                             );

            if (pageCurrent < _pagedData.PageNumber ||
                pageCurrent > _pagedData.TotalPages)
                _pagination?.NavigateTo(_pagedData.PageNumber);

            await ChangeLoadingState(false);
        }
        catch (Exception e)
        {
            Snackbar.Add("Произошла ошибка", Severity.Error);

            Logger.LogError(
                e, "Error in {@ClassName} during {@MethodName} execution", GetType().Name,
                nameof(UpdateTableData)
            );
        }
    }
}