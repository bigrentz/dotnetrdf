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
    /// Pattern which matches temporary variables.
    /// </summary>
    public class BlankNodePattern 
        : PatternItem
    {
        private string _name;

        /// <summary>
        /// Creates a new Pattern representing a Blank Node.
        /// </summary>
        /// <param name="name">Blank Node ID.</param>
        public BlankNodePattern(string name)
        {
            _name = "_:" + name;
        }

        /// <summary>
        /// Creates a new Pattern representing a Blank Node.
        /// </summary>
        /// <param name="name">Blank Node ID.</param>
        /// <param name="rigorousEvaluation">Whether to force rigorous evaluation.</param>
        public BlankNodePattern(string name, bool rigorousEvaluation)
            : this(name)
        {
            RigorousEvaluation = rigorousEvaluation;
        }

        /// <summary>
        /// Gets the Blank Node ID.
        /// </summary>
        public string ID
        {
            get
            {
                return _name;
            }
        }

        /// <summary>
        /// Checks whether the given Node is a valid value for the Temporary Variable.
        /// </summary>
        /// <param name="context">Evaluation Context.</param>
        /// <param name="obj">Node to test.</param>
        /// <returns></returns>
        public override bool Accepts(IPatternEvaluationContext context, INode obj)
        {
            if (context.RigorousEvaluation || RigorousEvaluation)
            {
                if (context.ContainsVariable(_name))
                {
                    return context.ContainsValue(_name, obj);
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
        /// <returns></returns>
        public override INode Construct(ConstructContext context)
        {
            return context.GetBlankNode(_name);
        }

        /// <summary>
        /// Gets the String representation of this Pattern.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return _name;
        }

        /// <summary>
        /// Gets the Temporary Variable Name of this Pattern.
        /// </summary>
        public override string VariableName
        {
            get
            {
                return _name;
            }
        }
    }
}
