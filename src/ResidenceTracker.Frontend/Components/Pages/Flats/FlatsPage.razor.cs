using Microsoft.AspNetCore.Components;
using MudBlazor;
using ResidenceTracker.Domain.Entities;
using ResidenceTracker.Domain.Entities.Enums;
using ResidenceTracker.Frontend.Components.Abstractions;
using ResidenceTracker.Infrastructure.Abstractions;
using ResidenceTracker.Infrastructure.DataAccess.PostgreSql.Implementation;
using ResidenceTracker.UseCases.Utilities;

namespace ResidenceTracker.Frontend.Components.Pages.Flats;

public partial class FlatsPage : AbstractCrudBasePage<Flat>
{
    protected override IReadOnlyCollection<string> DisplayOrder { get; } = new List<string>
    {
        "Id",
        "Number",
        "Members.Name",
        "CreatedAt",
        "ModifiedAt",
    };

    [Inject]
    protected IRepository<Member> MembersRepository { get; set; } = null!;
    [Inject]
    protected IRepository<ResidencyEventLog> ResidencyEventLogRepository { get; set; } = null!;
    private Flat? _flat;

    private Member? _member;
    private MudForm _memberForm = null!;
    private bool _isAdd;
    private bool _isMemberFormHidden = true;

    private void ChangeMemberFormState(bool isHidden)
    {
        if (isHidden == _isMemberFormHidden)
            return;

        _isMemberFormHidden = isHidden;
        StateHasChanged();
    }

    private void ShowAddMemberForm()
    {
        ShowMemberForm();
        _isAdd = true;
    }

    private void ShowMemberForm()
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

        ChangeMemberFormState(false);
        _flat = Table.SelectedItems.First();
        _member = null;
    }

    private void ShowRemoveMemberForm()
    {
        ShowMemberForm();
        _isAdd = false;
    }

    private async Task SubmitMemberForm()
    {
        HandleLoadingChange(true);

        if (_flat is null)
        {
            Snackbar.Add("Произошла ошибка", Severity.Error);
            HandleLoadingChange(false);
            return;
        }

        if (_member is null)
        {
            _memberForm.IsValid = false;
            await _memberForm.IsValidChanged.InvokeAsync();

            _memberForm.Errors = new[]
            {
                $"{typeof(House).GetProperty(nameof(House.Flats))!.GetLocalizedDisplayName()} не может быть пустым",
            };

            HandleLoadingChange(false);
            return;
        }

        ChangeMemberFormState(true);

        try
        {
            if (Repository is not FlatRepository flatRepository)
                throw new("FlatRepository cast");

            var existsFlat = await flatRepository.HasMember(_flat.Id, _member.Id, CancellationToken);

            if (_isAdd && !existsFlat)
            {
                if (_member.FlatId is not null)
                {
                    Snackbar.Add($"Человек с именем {_member.Name} уже есть в другой квартире", Severity.Error);
                    return;
                }
                
                if (_flat.Members is not null)
                {
                    _flat.Members.Add(_member);
                }
                else
                {
                    _flat.Members = new List<Member>();
                    _flat.Members.Add(_member);
                }

                await ResidencyEventLogRepository.AddAsync(new ResidencyEventLog()
                {
                    Flat = _flat,
                    Member = _member,
                    EventType = ResidencyEventType.Enter,
                    IsReadonly = true
                    
                }, CancellationToken);
            }
            else if (_isAdd && existsFlat)
            {
                Snackbar.Add($"Человек с именем {_member.Name} уже есть в квартире", Severity.Error);
                return;
            }
            else if (!_isAdd && existsFlat)
            {
                _flat.Members?.Remove(_member);
                await ResidencyEventLogRepository.AddAsync(new ResidencyEventLog()
                {
                    Flat = _flat,
                    Member = _member,
                    EventType = ResidencyEventType.Out,
                    IsReadonly = true
                    
                }, CancellationToken);
            }
            else if (!_isAdd &&
                     !existsFlat)
            {
                Snackbar.Add($"Человек с именем {_member.Name} не находится в квартире", Severity.Error);
                return;
            }

            await Repository.UpdateAsync(_flat, CancellationToken);
            await Table?.UpdateTableData()!;
            Snackbar.Add($"Элемент был добавлен {_flat.Id}", Severity.Success);
        }
        catch (Exception e)
        {
            Snackbar.Add("Произошла ошибка", Severity.Error);

            Logger.LogError(
                e, "Error in {@ClassName} during {@MethodName} execution", GetType().Name, nameof(SubmitMemberForm)
            );
        }
        finally
        {
            _member = null!;
            _flat = null!;
            HandleLoadingChange(false);
        }
    }
}