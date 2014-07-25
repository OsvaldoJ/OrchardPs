﻿namespace Orchard.Management.PsProvider.Vfs.Navigation
{
    using System;
    using System.Collections;
    using System.IO;
    using System.Management.Automation.Provider;

    /// <summary>
    /// Implements the <see cref="IContentReader"/> for <see cref="PropertyNode"/> VFS nodes.
    /// </summary>
    public sealed class PropertyContentReader : IContentReader
    {
        /// <summary>
        /// The name of the property which will be retrieved by this content reader.
        /// </summary>
        private readonly string propertyName;

        /// <summary>
        /// The <see cref="PropertyStoreNode"/> which contains the property to retrieve.
        /// </summary>
        private readonly PropertyStoreNode propertyStoreNode;

        /// <summary>
        /// Indicates whether the value of the property has been read.
        /// </summary>
        private bool propertyValueRead;
        
        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyContentReader"/> class.
        /// </summary>
        /// <param name="propertyName">The name of the property which will be retrieved by this content reader.</param>
        /// <param name="propertyStoreNode">
        /// The <see cref="PropertyStoreNode"/> which contains the property to retrieve.
        /// </param>
        public PropertyContentReader(string propertyName, PropertyStoreNode propertyStoreNode)
        {
            this.propertyName = propertyName;
            this.propertyStoreNode = propertyStoreNode;
        }

        /// <summary>
        /// Reads the specified read count.
        /// </summary>
        /// <param name="readCount">
        /// The number of blocks to be read. If a negative number or <c>0</c> is specified, all blocks in the item
        /// are read.
        /// </param>
        /// <returns>An <see cref="IList"/> collection that contains the blocks read.</returns>
        public IList Read(long readCount)
        {
            if (!this.propertyValueRead)
            {
                this.propertyValueRead = true;
                return new[] { this.propertyStoreNode.GetValue(this.propertyName) };
            }

            return new object[0];
        }

        /// <summary>
        /// Moves the content reader to a position in the content.
        /// </summary>
        /// <param name="offset">The offset from the starting point specified by the origin parameter.</param>
        /// <param name="origin">
        /// A <see cref="SeekOrigin"/> enumeration constant that specifies the starting point.
        /// </param>
        public void Seek(long offset, SeekOrigin origin)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Closes the content reader.
        /// </summary>
        public void Close()
        {
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
        }
    }
}