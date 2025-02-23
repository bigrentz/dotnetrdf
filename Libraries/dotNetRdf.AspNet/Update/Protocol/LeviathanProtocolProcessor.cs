/*
// <copyright>
// dotNetRDF is free and open source software licensed under the MIT License
// -------------------------------------------------------------------------
// 
// Copyright (c) 2009-2021 dotNetRDF Project (http://dotnetrdf.org/)
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is furnished
// to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR 
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
// WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
// CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
// </copyright>
*/

using VDS.RDF.Query;
using VDS.RDF.Query.Datasets;

namespace VDS.RDF.Update.Protocol
{
    /// <summary>
    /// A processor for the SPARQL Graph Store HTTP Protocol which operates by using the libraries in-memory Leviathan SPARQL engine and converting protocol actions to SPARQL Query/Update commands as appropriate.
    /// </summary>
    public class LeviathanProtocolProcessor 
        : ProtocolToUpdateProcessor
    {
        /// <summary>
        /// Creates a new Leviathan Protocol Processor.
        /// </summary>
        /// <param name="store">Triple Store.</param>
        /// <param name="uriFactory">Factory to use when creating URI instances.</param>
        public LeviathanProtocolProcessor(IInMemoryQueryableStore store, IUriFactory uriFactory = null)
            : this(new InMemoryDataset(store), uriFactory) { }

        /// <summary>
        /// Creates a new Leviathan Protocol Processor.
        /// </summary>
        /// <param name="dataset">SPARQL Dataset.</param>
        /// <param name="uriFactory">Factory to use when creating URI instances.</param>
        public LeviathanProtocolProcessor(ISparqlDataset dataset, IUriFactory uriFactory = null)
            : base(new LeviathanQueryProcessor(dataset), new LeviathanUpdateProcessor(dataset), uriFactory) { }
    }
}
