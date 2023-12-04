using Microsoft.AspNetCore.Components;
using MudBlazor;
using ResidenceTracker.Domain.Entities;
using ResidenceTracker.Frontend.Components.Abstractions;
using ResidenceTracker.Infrastructure.Abstractions;
using ResidenceTracker.Infrastructure.DataAccess.PostgreSql.Implementation;
using ResidenceTracker.UseCases.Utilities;

namespace ResidenceTracker.Frontend.Components.Pages.Houses;

public partial class HousesPage : AbstractCrudBasePage<House>
{
    protected override IReadOnlyCollection<string> DisplayOrder { get; } = new List<string>
    {
        "Id",
        "Number",
        "Flats.Number",
        "CreatedAt",
        "ModifiedAt",
    };

    [Inject]
    protected IRepository<Flat> FlatRepository { get; set; } = null!;

    private Flat? _flat;
    private MudForm _flatForm = null!;
    private House? _house;
    private bool _isAdd;
    private bool _isFlatFormHidden = true;

    private void ChangeFlatFormState(bool isHidden)
    {
        if (isHidden == _isFlatFormHidden)
            return;

        _isFlatFormHidden = isHidden;
        StateHasChanged();
    }

    private void ShowAddFlatForm()
    {
        ShowFlatForm();
        _isAdd = true;
    }

    private void ShowFlatForm()
    {
        if (Table is null)
        {
            Snackbar.Add("Произошла ошибка", Severity.Error);
            return;
        }

        if (Table.SelectedItems.Count != 1)
        {
            Snackbar.Add(
                "Нужно выбрать один элемент. Возможно выбранные элементы нельзя изменить или удалить.",
                Severity.Error
            );

            return;
        }

        ChangeFlatFormState(false);
        _house = Table.SelectedItems.First();
        _flat = null;
    }

    private void ShowRemoveFlatForm()
    {
        ShowFlatForm();
        _isAdd = false;
    }

    private async Task SubmitFlatForm()
    {
        HandleLoadingChange(true);

        if (_house is null)
        {
            Snackbar.Add("Произошла ошибка", Severity.Error);
            HandleLoadingChange(false);
            return;
        }

        if (_flat is null)
        {
            Snackbar.Add(
                $"{typeof(House).GetProperty(nameof(House.Flats))!.GetLocalizedDisplayName()} не может быть пустым",
                Severity.Error
            );

            HandleLoadingChange(false);
            return;
        }

        ChangeFlatFormState(true);

        try
        {
            HouseRepository? houseRepository = Repository as HouseRepository;

            if (houseRepository is null)
                throw new("HouseRepository cast");

            bool existsFlat = await houseRepository.HasFlat(_house.Id, _flat.Id, CancellationToken);

            if (_isAdd && !existsFlat)
            {
                if (_house.Flats is not null)
                {
                    _house.Flats.Add(_flat);
                }
                else
                {
                    _house.Flats = new List<Flat>();
                    _house.Flats.Add(_flat);
                }
            }
            else if (_isAdd && existsFlat)
            {
                Snackbar.Add($"Квартира с номером {_flat.Number} уже есть", Severity.Error);
                return;
            }
            else if (!_isAdd && existsFlat)
            {
                _house.Flats?.Remove(_flat);
            }
            else if (!_isAdd &&
                     !existsFlat)
            {
                Snackbar.Add($"Нет квартиры с номером {_flat.Number}", Severity.Error);
                return;
            }

            await Repository.UpdateAsync(_house, CancellationToken);
            await Table?.UpdateTableData()!;
            Snackbar.Add($"Элемент был добавлен {_house.Id}", Severity.Success);
        }
        catch (Exception e)
        {
            Snackbar.Add("Произошла ошибка", Severity.Error);

            Logger.LogError(
                e, "Error in {@ClassName} during {@MethodName} execution", GetType().Name,
                nameof(SubmitFlatForm)
            );
        }
        finally
        {
            _flat = null!;
            _house = null!;
            HandleLoadingChange(false);
        }
    }
}