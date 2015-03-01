﻿namespace Proligence.PowerShell.Content.Nodes
{
    using Orchard.ContentManagement.MetaData.Models;
    using Proligence.PowerShell.Provider.Vfs;
    using Proligence.PowerShell.Provider.Vfs.Navigation;

    /// <summary>
    /// Implements a VFS node which represents an Orchard content type definition.
    /// </summary>
    public class ContentTypeNode : ObjectNode
    {
        public ContentTypeNode(IPowerShellVfs vfs, ContentTypeDefinition definition)
            : base(vfs, definition.Name, definition)
        {
        }
    }
}