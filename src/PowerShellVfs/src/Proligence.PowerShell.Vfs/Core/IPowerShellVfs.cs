﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IPowerShellVfs.cs" company="Proligence">
//   Copyright (c) Proligence, All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Proligence.PowerShell.Vfs.Core
{
    using Proligence.PowerShell.Vfs.Navigation;

    /// <summary>
    /// Defines the interface for objects which implement the PowerShell Virtual File System (VFS).
    /// </summary>
    public interface IPowerShellVfs 
    {
        /// <summary>
        /// Gets the root node of the VFS.
        /// </summary>
        RootVfsNode Root { get; }

        /// <summary>
        /// Gets the navigation provider manager for the VFS instance.
        /// </summary>
        INavigationProviderManager NavigationProviderManager { get; }

        /// <summary>
        /// Gets the object which implements input path validation.
        /// </summary>
        IPathValidator PathValidator { get; }
        
        /// <summary>
        /// Initializes the VFS instance.
        /// </summary>
        void Initialize();

        /// <summary>
        /// Gets the node under the specified path in the VFS.
        /// </summary>
        /// <param name="path">The path of the node to get.</param>
        /// <returns>
        /// The node under the specified path or <c>null</c> if there is no node under the specified path.
        /// </returns>
        VfsNode NavigatePath(string path);
    }
}