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

using System;
using VDS.RDF.Query.Construct;

namespace VDS.RDF.Query.Patterns
{
    /// <summary>
    /// Pattern which matches Variables.
    /// </summary>
    public class VariablePattern 
        : PatternItem
    {
        private readonly string _varname;

        /// <summary>
        /// Creates a new Variable Pattern.
        /// </summary>
        /// <param name="name">Variable name.</param>
        public VariablePattern(string name)
        {
            _varname = name;

            // Strip leading ?/$ if present
            if (_varname.StartsWith("?") || _varname.StartsWith("$"))
            {
                _varname = _varname.Substring(1);
            }
        }

        /// <summary>
        /// Creates a new Variable Pattern.
        /// </summary>
        /// <param name="name">Variable name.</param>
        /// <param name="rigorousEvaluation">Whether to force rigorous evaluation.</param>
        public VariablePattern(string name, bool rigorousEvaluation)
            : this(name)
        {
            RigorousEvaluation = rigorousEvaluation;
        }

        /// <summary>
        /// Checks whether the given Node is a valid value for the Variable in the current Binding Context.
        /// </summary>
        /// <param name="context">Evaluation Context.</param>
        /// <param name="obj">Node to test.</param>
        /// <returns></returns>
        public override bool Accepts(IPatternEvaluationContext context, INode obj)
        {
            if (context.RigorousEvaluation || RigorousEvaluation)
            {
                if (context.ContainsVariable(_varname))
                {
                    return context.ContainsValue(_varname, obj);
                }
                else if (Repeated)
                {
                    return true;
                }
                else
                {
                    return true;
                }
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// Constructs a Node based on the given Set.
        /// </summary>
        /// <param name="context">Construct Context.</param>
        /// <returns>The Node which is bound to this Variable in this Solution.</returns>
        public override INode Construct(ConstructContext context)
        {
            INode value = context.Set[_varname];

            if (value == null) throw new RdfQueryException("Unable to construct a Value for this Variable for this solution as it is bound to a null");
            switch (value.NodeType)
            {
                case NodeType.Blank:
                    return new BlankNode(((IBlankNode)value).InternalID);
                    //if (!context.PreserveBlankNodes && value.GraphUri != null)
                    //{
                    //    // Rename Blank Node based on the Graph Uri Hash Code
                    //    var hash = value.GraphUri.GetEnhancedHashCode();
                    //    if (hash >= 0)
                    //    {
                    //        return new BlankNode(((IBlankNode)value).InternalID + "-" + value.GraphUri.GetEnhancedHashCode());
                    //    }
                    //    else
                    //    {
                    //        return new BlankNode(((IBlankNode)value).InternalID + hash);
                    //    }
                    //}
                    //else
                    //{
                    //    return new BlankNode(((IBlankNode)value).InternalID);
                    //}

                default:
                    return context.GetNode(value);
            }
        }

        /// <summary>
        /// Gets the String representation of this pattern.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return "?" + _varname;
        }

        /// <summary>
        /// Gets the Name of the Variable this Pattern matches.
        /// </summary>
        public override string VariableName => _varname;
    }
}
