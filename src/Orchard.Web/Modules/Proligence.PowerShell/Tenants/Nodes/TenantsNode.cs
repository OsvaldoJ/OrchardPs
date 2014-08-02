﻿namespace Proligence.PowerShell.Tenants.Nodes 
{
    using System.Collections.Generic;
    using System.Linq;
    using Orchard.Environment.Configuration;
    using Proligence.PowerShell.Common.Items;
    using Proligence.PowerShell.Provider;
    using Proligence.PowerShell.Provider.Vfs.Core;
    using Proligence.PowerShell.Provider.Vfs.Navigation;

    /// <summary>
    /// Implements a VFS node which groups <see cref="TenantNode"/> nodes for a single Orchard installation.
    /// </summary>
    [SupportedCmdlet("Enable-Tenant")]
    [SupportedCmdlet("Disable-Tenant")]
    [SupportedCmdlet("Remove-Tenant")]
    [SupportedCmdlet("Edit-Tenant")]
    public class TenantsNode : ContainerNode 
    {
        private readonly IShellSettingsManager manager;
        
        public TenantsNode(IPowerShellVfs vfs, IShellSettingsManager manager)
            : base(vfs, "Tenants")
        {
            this.manager = manager;

            this.Item = new CollectionItem(this) 
            {
                Name = "Tenants",
                Description = "Contains all tenants of the Orchard instance."
            };
        }

        /// <summary>
        /// Gets the node's virtual (dynamic) child nodes.
        /// </summary>
        /// <returns>A sequence of child nodes.</returns>
        public override IEnumerable<VfsNode> GetVirtualNodes()
        {
            ShellSettings[] tenants = this.manager.LoadSettings().ToArray();
            return tenants.Select(tenant => new TenantNode(Vfs, tenant));
        }
    }
}