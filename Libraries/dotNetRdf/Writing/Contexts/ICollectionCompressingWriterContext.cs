// unset

using System.Collections.Generic;

namespace VDS.RDF.Writing.Contexts
{
    /// <summary>
    /// Interface for Writer Contexts which store collection compression data.
    /// </summary>
    public interface ICollectionCompressingWriterContext : IWriterContext
    {
        /// <summary>
        /// Gets the mapping from Blank Nodes to Collections.
        /// </summary>
        Dictionary<INode, OutputRdfCollection> Collections
        {
            get;
        }

        /// <summary>
        /// Gets the Triples that should be excluded from standard output as they are part of collections.
        /// </summary>
        BaseTripleCollection TriplesDone
        {
            get;
        }
    }
}