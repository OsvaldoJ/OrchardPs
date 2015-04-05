﻿namespace Proligence.PowerShell.Core.Tenants.Cmdlets
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Management.Automation;
    using Autofac;
    using Orchard.Environment.Configuration;
    using Proligence.PowerShell.Provider;
    using Proligence.PowerShell.Provider.Utilities;

    [Cmdlet(VerbsCommon.Get, "Tenant", DefaultParameterSetName = "Default", ConfirmImpact = ConfirmImpact.None)]
    public class GetTenant : OrchardCmdlet
    {
        /// <summary>
        /// Gets or sets the name of the tenant to get.
        /// </summary>
        [ValidateNotNullOrEmpty]
        [Parameter(ParameterSetName = "Default", Mandatory = false, Position = 1)]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether only enabled tenants should be retrieved.
        /// </summary>
        [Parameter(ParameterSetName = "Enabled", Mandatory = false)]
        public SwitchParameter Enabled { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether only disabled features should be retrieved.
        /// </summary>
        [Parameter(ParameterSetName = "Disabled", Mandatory = false)]
        public SwitchParameter Disabled { get; set; }

        protected override void ProcessRecord()
        {
            IEnumerable<ShellSettings> tenants = this.GetTenantsFromParameters();
            foreach (var tenant in tenants)
            {
                this.WriteObject(tenant);
            }
        }

        private IEnumerable<ShellSettings> GetTenantsFromParameters()
        {
            var shellSettingsManager = this.OrchardDrive.ComponentContext.Resolve<IShellSettingsManager>();
            IEnumerable<ShellSettings> tenants = shellSettingsManager.LoadSettings().ToArray();

            if (!string.IsNullOrEmpty(this.Name))
            {
                tenants = tenants.Where(t => t.Name.WildcardEquals(this.Name));
            }
            else
            {
                if (this.Enabled.ToBool())
                {
                    tenants = tenants.Where(t => t.State == TenantState.Running);
                }

                if (this.Disabled.ToBool())
                {
                    tenants = tenants.Where(t => t.State == TenantState.Disabled);
                }
            }

            return tenants;
        }
    }
}