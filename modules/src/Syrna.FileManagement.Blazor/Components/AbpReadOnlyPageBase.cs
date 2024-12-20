using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blazorise;
using Blazorise.DataGrid;
using Localization.Resources.AbpUi;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.AspNetCore.Components;
using Volo.Abp.AspNetCore.Components.Web.Extensibility.EntityActions;
using Volo.Abp.AspNetCore.Components.Web.Extensibility.TableColumns;
using Volo.Abp.Authorization;
using Volo.Abp.BlazoriseUI;
using Volo.Abp.BlazoriseUI.Components;
using Volo.Abp.Localization;
using Volo.Abp.ObjectExtending;
using Volo.Abp.ObjectExtending.Modularity;
using BreadcrumbItem = Volo.Abp.BlazoriseUI.BreadcrumbItem;

namespace Syrna.FileManagement.Blazor.Components;

public abstract class AbpReadOnlyPageBase<
    TAppService,
    TGetOutputDto,
    TGetListOutputDto,
    TKey,
    TGetListInput,
    TListViewModel>
    : AbpComponentBase
    where TAppService : IReadOnlyAppService<
        TGetOutputDto,
        TGetListOutputDto,
        TKey,
        TGetListInput>
    where TGetOutputDto : IEntityDto<TKey>
    where TGetListOutputDto : IEntityDto<TKey>
    where TGetListInput : new()
    where TListViewModel : IEntityDto<TKey>
{
    [Inject] protected TAppService AppService { get; set; } = default!;
    [Inject] protected IStringLocalizer<AbpUiResource> UiLocalizer { get; set; } = default!;
    [Inject] public IAbpEnumLocalizer AbpEnumLocalizer { get; set; } = default!;
    [Inject] protected ExtensionPropertyPolicyChecker ExtensionPropertyPolicyChecker { get; set; } = default!;
    protected virtual int PageSize { get; } = LimitedResultRequestDto.DefaultMaxResultCount;

    protected int CurrentPage = 1;
    protected string CurrentSorting = default!;
    protected int? TotalCount;
    protected TGetListInput GetListInput = new TGetListInput();
    protected IReadOnlyList<TListViewModel> Entities = Array.Empty<TListViewModel>();
    protected List<BreadcrumbItem> BreadcrumbItems = new List<BreadcrumbItem>(2);
    protected DataGridEntityActionsColumn<TListViewModel> EntityActionsColumn = default!;
    protected EntityActionDictionary EntityActions { get; set; }
    protected TableColumnDictionary TableColumns { get; set; }

    protected AbpReadOnlyPageBase()
    {
        TableColumns = new TableColumnDictionary();
        EntityActions = new EntityActionDictionary();
    }

    protected override async Task OnInitializedAsync()
    {
        await TrySetPermissionsAsync();
        await TrySetEntityActionsAsync();
        await TrySetTableColumnsAsync();
        await InvokeAsync(StateHasChanged);
    }

    protected virtual async Task SetPermissionsAsync()
    {
        await Task.CompletedTask;
    }

    private async Task TrySetPermissionsAsync()
    {
        if (IsDisposed)
        {
            return;
        }

        await SetPermissionsAsync();
    }
    
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await SetToolbarItemsAsync();
            await SetBreadcrumbItemsAsync();
        }
        await base.OnAfterRenderAsync(firstRender);
    }

    protected virtual async Task GetEntitiesAsync()
    {
        try
        {
            await UpdateGetListInputAsync();
            var result = await AppService.GetListAsync(GetListInput);
            Entities = MapToListViewModel(result.Items);
            TotalCount = (int?)result.TotalCount;
        }
        catch (Exception ex)
        {
            await HandleErrorAsync(ex);
        }
    }

    private IReadOnlyList<TListViewModel> MapToListViewModel(IReadOnlyList<TGetListOutputDto> dtos)
    {
        if (typeof(TGetListOutputDto) == typeof(TListViewModel))
        {
            return dtos.As<IReadOnlyList<TListViewModel>>();
        }

        return ObjectMapper.Map<IReadOnlyList<TGetListOutputDto>, List<TListViewModel>>(dtos);
    }

    protected virtual Task UpdateGetListInputAsync()
    {
        if (GetListInput is ISortedResultRequest sortedResultRequestInput)
        {
            sortedResultRequestInput.Sorting = CurrentSorting;
        }

        if (GetListInput is IPagedResultRequest pagedResultRequestInput)
        {
            pagedResultRequestInput.SkipCount = (CurrentPage - 1) * PageSize;
        }

        if (GetListInput is ILimitedResultRequest limitedResultRequestInput)
        {
            limitedResultRequestInput.MaxResultCount = PageSize;
        }

        return Task.CompletedTask;
    }

    protected virtual async Task SearchEntitiesAsync()
    {
        var currentPage = CurrentPage;
        CurrentPage = 1;
        if (currentPage == 1)
        {
            await GetEntitiesAsync();
        }
        await InvokeAsync(StateHasChanged);
    }

    protected virtual async Task OnDataGridReadAsync(DataGridReadDataEventArgs<TListViewModel> e)
    {
        CurrentSorting = e.Columns
            .Where(c => c.SortDirection != SortDirection.Default)
            .OrderBy(c => c.SortIndex)
            .Select(c => c.SortField + (c.SortDirection == SortDirection.Descending ? " DESC" : ""))
            .JoinAsString(",");
        CurrentPage = e.Page;

        await GetEntitiesAsync();

        await InvokeAsync(StateHasChanged);
    }

    /// <summary>
    /// Calls IAuthorizationService.CheckAsync for the given <paramref name="policyName"/>.
    /// Throws <see cref="AbpAuthorizationException"/> if given policy was not granted for the current user.
    ///
    /// Does nothing if <paramref name="policyName"/> is null or empty.
    /// </summary>
    /// <param name="policyName">A policy name to check</param>
    protected virtual async Task CheckPolicyAsync(string policyName)
    {
        if (string.IsNullOrEmpty(policyName))
        {
            return;
        }

        await AuthorizationService.CheckAsync(policyName);
    }

    protected virtual ValueTask SetBreadcrumbItemsAsync()
    {
        return ValueTask.CompletedTask;
    }

    private async ValueTask TrySetEntityActionsAsync()
    {
        if (IsDisposed)
        {
            return;
        }

        await SetEntityActionsAsync();
    }

    protected virtual ValueTask SetEntityActionsAsync()
    {
        return ValueTask.CompletedTask;
    }

    private async ValueTask TrySetTableColumnsAsync()
    {
        if (IsDisposed)
        {
            return;
        }

        await SetTableColumnsAsync();
    }

    protected virtual ValueTask SetTableColumnsAsync()
    {

        return ValueTask.CompletedTask;
    }

    protected virtual ValueTask SetToolbarItemsAsync()
    {
        return ValueTask.CompletedTask;
    }

    protected virtual async Task<List<TableColumn>> GetExtensionTableColumnsAsync(string moduleName, string entityType)
    {
        var tableColumns = new List<TableColumn>();
        var properties = ModuleExtensionConfigurationHelper.GetPropertyConfigurations(moduleName, entityType).ToList();
        foreach (var propertyInfo in properties)
        {
            if (!await ExtensionPropertyPolicyChecker.CheckPolicyAsync(propertyInfo.Policy))
            {
                continue;
            }

            if (propertyInfo.IsAvailableToClients && propertyInfo.UI.OnTable.IsVisible)
            {
                if (propertyInfo.Name.EndsWith("_Text"))
                {
                    var lookupPropertyName = propertyInfo.Name.RemovePostFix("_Text");
                    var lookupPropertyDefinition = properties.SingleOrDefault(t => t.Name == lookupPropertyName)!;
                    tableColumns.Add(new TableColumn
                    {
                        Title = lookupPropertyDefinition.GetLocalizedDisplayName(StringLocalizerFactory),
                        Data = $"ExtraProperties[{propertyInfo.Name}]",
                        PropertyName = propertyInfo.Name
                    });
                }
                else
                {
                    var column = new TableColumn
                    {
                        Title = propertyInfo.GetLocalizedDisplayName(StringLocalizerFactory),
                        Data = $"ExtraProperties[{propertyInfo.Name}]",
                        PropertyName = propertyInfo.Name
                    };

                    if (propertyInfo.IsDate() || propertyInfo.IsDateTime())
                    {
                        column.DisplayFormat = propertyInfo.GetDateEditInputFormatOrNull();
                    }

                    if (propertyInfo.Type.IsEnum)
                    {
                        column.ValueConverter = (val) =>
                            AbpEnumLocalizer.GetString(propertyInfo.Type, val.As<ExtensibleObject>().ExtraProperties[propertyInfo.Name]!, new IStringLocalizer[] { StringLocalizerFactory.CreateDefaultOrNull() });
                    }

                    tableColumns.Add(column);
                }
            }
        }

        return tableColumns;
    }
}

public abstract class AbpReadOnlyPageBase<
    TAppService,
    TGetOutputDto,
    TKey,
    TGetListInput>
    : AbpReadOnlyPageBase<TAppService, TGetOutputDto, TGetOutputDto, TKey, TGetListInput, TGetOutputDto>
    where TAppService : IReadOnlyAppService<
        TGetOutputDto,
        TGetOutputDto,
        TKey,
        TGetListInput>
    where TGetOutputDto : IEntityDto<TKey>
    where TGetListInput : new()
{
}