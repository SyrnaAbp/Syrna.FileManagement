﻿using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace Syrna.FileManagement.UnifiedDemo.Data;

/* This is used if database provider does't define
 * IDemoDbSchemaMigrator implementation.
 */
public class NullUnifiedDemoDbSchemaMigrator : IUnifiedDemoDbSchemaMigrator, ITransientDependency
{
    public Task MigrateAsync()
    {
        return Task.CompletedTask;
    }
}
