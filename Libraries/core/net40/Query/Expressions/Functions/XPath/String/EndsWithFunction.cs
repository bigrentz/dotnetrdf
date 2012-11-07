/*
dotNetRDF is free and open source software licensed under the MIT License

-----------------------------------------------------------------------------

Copyright (c) 2009-2012 dotNetRDF Project (dotnetrdf-developer@lists.sf.net)

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is furnished
to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR 
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VDS.RDF.Parsing;
using VDS.RDF.Nodes;

namespace VDS.RDF.Query.Expressions.Functions.XPath.String
{
    /// <summary>
    /// Represents the XPath fn:ends-with() function
    /// </summary>
    public class EndsWithFunction
        : BaseBinaryStringFunction
    {
        /// <summary>
        /// Creates a new XPath Ends With function
        /// </summary>
        /// <param name="stringExpr">Expression</param>
        /// <param name="suffixExpr">Suffix Expression</param>
        public EndsWithFunction(ISparqlExpression stringExpr, ISparqlExpression suffixExpr)
            : base(stringExpr, suffixExpr, false, XPathFunctionFactory.AcceptStringArguments) { }

        /// <summary>
        /// Gets the Value of the function as applied to the given String Literal and Argument
        /// </summary>
        /// <param name="stringLit">Simple/String typed Literal</param>
        /// <param name="arg">Argument</param>
        /// <returns></returns>
        public override IValuedNode ValueInternal(ILiteralNode stringLit, ILiteralNode arg)
        {
            if (stringLit.Value.Equals(string.Empty))
            {
                if (arg.Value.Equals(string.Empty))
                {
                    //The Empty String ends with the Empty String
                    return new BooleanNode(null, true);
                }
                else
                {
                    //Empty String doesn't end with a non-empty string
                    return new BooleanNode(null, false);
                }
            }
            else if (arg.Value.Equals(string.Empty))
            {
                //Any non-empty string ends with the empty string
                return new BooleanNode(null, true);
            }
            else
            {
                //Otherwise evalute the EndsWith
                return new BooleanNode(null, stringLit.Value.EndsWith(arg.Value));
            }
        }

        /// <summary>
        /// Gets the String representation of the function
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return "<" + XPathFunctionFactory.XPathFunctionsNamespace + XPathFunctionFactory.EndsWith + ">(" + this._expr.ToString() + "," + this._arg.ToString() + ")";
        }

        /// <summary>
        /// Gets the Functor of the Expression
        /// </summary>
        public override string Functor
        {
            get
            {
                return XPathFunctionFactory.XPathFunctionsNamespace + XPathFunctionFactory.EndsWith;
            }
        }

        /// <summary>
        /// Transforms the Expression using the given Transformer
        /// </summary>
        /// <param name="transformer">Expression Transformer</param>
        /// <returns></returns>
        public override ISparqlExpression Transform(IExpressionTransformer transformer)
        {
            return new EndsWithFunction(transformer.Transform(this._expr), transformer.Transform(this._arg));
        }
    }
}
