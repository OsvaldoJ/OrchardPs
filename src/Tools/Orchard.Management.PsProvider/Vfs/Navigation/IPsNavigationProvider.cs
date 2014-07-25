﻿namespace Orchard.Management.PsProvider.Vfs.Navigation
{
    using Orchard.Management.PsProvider.Vfs.Core;

    /// <summary>
    /// Defines the interface for classes which extend the PowerShell Virtual File System by adding new static nodes
    /// to the VFS tree.
    /// </summary>
    public interface IPsNavigationProvider 
    {
        /// <summary>
        /// Gets the path of the added node's parent node.
        /// </summary>
        string Path { get; }

        /// <summary>
        /// Gets the type of the added node.
        /// </summary>
        NodeType NodeType { get; }

        /// <summary>
        /// Gets the added node.
        /// </summary>
        VfsNode Node { get; }

        /// <summary>
        /// Gets or sets the PowerShell VFS instance.
        /// </summary>
        IPowerShellVfs Vfs { get; set; }

        /// <summary>
        /// Initializes the navigation provider.
        /// </summary>
        void Initialize();
    }
}